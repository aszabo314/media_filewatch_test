﻿namespace Demo

open Aardvark.Service

open System

open Aardvark.Base
open Aardvark.Base.Geometry
open Aardvark.Base.Incremental
open Aardvark.Base.Incremental.Operators
open Aardvark.Base.Rendering
open Aardvark.Application
open Aardvark.Application.WinForms
open System.Collections.Generic
open System.Collections.Concurrent
open Aardvark.SceneGraph
open Aardvark.UI
open Aardvark.Rendering.Text
open Demo.TestApp
open Demo.TestApp.Mutable


module ArcBallController =
    open Aardvark.Base.Incremental.Operators

    type Message = 
        | Down of button : MouseButtons * pos : V2i
        | Up of button : MouseButtons
        | Move of V2i
        | StepTime
        | KeyDown of key : Keys
        | KeyUp of key : Keys
        | Blur
        | Pick of V3d

    let initial =
        {
            view = CameraView.lookAt (6.0 * V3d.III) V3d.Zero V3d.OOI
            dragStart = V2i.Zero
            look = false
            zoom = false
            pan = false
            forward = false; backward = false; left = false; right = false
            moveVec = V3i.Zero
            lastTime = None
            orbitCenter = Some V3d.Zero
            stash = None
        }

    let sw = Diagnostics.Stopwatch()
    do sw.Start()

    let withTime (model : CameraControllerState) =
        { model with lastTime = Some sw.Elapsed.TotalSeconds }

    let update (model : CameraControllerState) (message : Message) =
        match message with
            | Blur ->
                { initial with view = model.view; lastTime = None; orbitCenter = model.orbitCenter }
            | Pick p -> 
                let cam = model.view
                let newForward = p - cam.Location |> Vec.normalize
                let tempCam = cam.WithForward newForward

                
                

                { model with orbitCenter = Some p; view = CameraView.lookAt cam.Location p cam.Up }                
            | StepTime ->
                let now = sw.Elapsed.TotalSeconds
                let cam = model.view

                let cam = 
                    match model.lastTime with
                        | Some last ->
                            let dt = now - last

                            let dir = 
                                cam.Forward * float model.moveVec.Z +
                                cam.Right * float model.moveVec.X +
                                cam.Sky * float model.moveVec.Y

                            if model.moveVec = V3i.Zero then
                                printfn "useless time %A" now

                            cam.WithLocation(model.view.Location + dir * 1.5 * dt)

                        | None -> 
                            cam

                { model with lastTime = Some now; view = cam }

            | KeyDown Keys.W ->
                if not model.forward then
                    withTime { model with forward = true; moveVec = model.moveVec + V3i.OOI }
                else
                    model

            | KeyUp Keys.W ->
                if model.forward then
                    withTime { model with forward = false; moveVec = model.moveVec - V3i.OOI }
                else
                    model

            | KeyDown Keys.S ->
                if not model.backward then
                    withTime { model with backward = true; moveVec = model.moveVec - V3i.OOI }
                else
                    model

            | KeyUp Keys.S ->
                if model.backward then
                    withTime { model with backward = false; moveVec = model.moveVec + V3i.OOI }
                else
                    model

            | KeyDown Keys.A ->
                if not model.left then
                    withTime { model with left = true; moveVec = model.moveVec - V3i.IOO }
                else
                    model

            | KeyUp Keys.A ->
                if model.left then
                    withTime { model with left = false; moveVec = model.moveVec + V3i.IOO }
                else
                    model

            | KeyDown Keys.D ->
                if not model.right then
                    withTime { model with right = true; moveVec = model.moveVec + V3i.IOO }
                else
                    model

            | KeyUp Keys.D ->
                if model.right then
                    withTime { model with right = false; moveVec = model.moveVec - V3i.IOO }
                else
                    model

            | KeyDown _ | KeyUp _ ->
                model


            | Down(button,pos) ->
                let model = { model with dragStart = pos }
                match button with
                    | MouseButtons.Left -> { model with look = true }
                    | MouseButtons.Middle -> { model with pan = true }
                    | MouseButtons.Right -> { model with zoom = true }
                    | _ -> model

            | Up button ->
                match button with
                    | MouseButtons.Left -> { model with look = false }
                    | MouseButtons.Middle -> { model with pan = false }
                    | MouseButtons.Right -> { model with zoom = false }
                    | _ -> model

            | Move pos  ->
                
                let cam = model.view
                let delta = pos - model.dragStart

                let cam =
                    if model.look && model.orbitCenter.IsSome then
                        let trafo = 
                            M44d.Translation (model.orbitCenter.Value) *
                            M44d.Rotation (cam.Right, float delta.Y * -0.01) * 
                            M44d.Rotation (cam.Up, float delta.X * -0.01) *
                            M44d.Translation (-model.orbitCenter.Value)
                     
                        let newLocation = trafo.TransformPos (cam.Location)

                        let newUp = trafo.TransformDir (cam.Up)
                        let newRight = trafo.TransformDir (cam.Right)

                        //let tempcam = cam.WithLocation newLocation
                        
                        // make cam with up vector


                        //tempcam.WithForward newForward
                        let newForward = model.orbitCenter.Value - newLocation |> Vec.normalize
                        CameraView(cam.Sky, newLocation, newForward, newUp, newRight)
                    else
                        cam

                // change zoom and pan !!!!!!
                let cam =
                    if model.zoom then
                        let step = -0.05 * (cam.Forward * float delta.Y)
                        cam.WithLocation(cam.Location + step)
                    else
                        cam

                let cam, center =
                    if model.pan && model.orbitCenter.IsSome then
                        let step = 0.05 * (cam.Down * float delta.Y + cam.Right * float delta.X)
                        let center = model.orbitCenter.Value + step
                        cam.WithLocation(cam.Location + step), Some center
                    else
                        cam, model.orbitCenter            

                { model with view = cam; dragStart = pos; orbitCenter = center }


    let extractAttributes (state : MCameraControllerState) (f : Message -> 'msg) (frustum : IMod<Frustum>)  =
        AttributeMap.ofListCond [
            always (onBlur (fun _ -> f Blur))
            always (onMouseDown (fun b p -> f (Down(b,p))))
            always (onMouseUp (fun b p -> f (Up b)))
            always (onKeyDown (KeyDown >> f))
            always (onKeyUp (KeyUp >> f))
            onlyWhen (state.look %|| state.pan %|| state.zoom) (onMouseMove (Move >> f))
        ] |> AttributeMap.toAMap

    let controlledControl (state : MCameraControllerState) (f : Message -> 'msg) (frustum : IMod<Frustum>) (att : AttributeMap<'msg>) (sg : ISg<'msg>) =
        let attributes =
            AttributeMap.ofListCond [
                always (onBlur (fun _ -> f Blur))
                always (onMouseDown (fun b p -> f (Down(b,p))))
                always (onMouseUp (fun b p -> f (Up b)))
                always (onKeyDown (KeyDown >> f))
                always (onKeyUp (KeyUp >> f))
                onlyWhen (state.look %|| state.pan %|| state.zoom) (onMouseMove (Move >> f))
//                onlyWhen (state.moveVec |> Mod.map (fun v -> v <> V3i.Zero)) (onRendered (fun s c t -> f StepTime))
            ]

        let attributes = AttributeMap.union att attributes


//        let camSg : ISg<Message> =
//            failwith ""

        let cam = Mod.map2 Camera.create state.view frustum 
        Incremental.renderControl cam attributes sg

//
//    type Tool<'model, 'msg> = 
//        {
//            initial : 'model
//            update : 'model -> 'msg -> 'model
//            view : 'model -> hmap<string, AttributeValue<'msg>>
//        }
//
//    let cameraControllerApp =
//        {
//            view = 
//                fun model ->
//                    HMap.ofList [
//                        if model.zoom || model.pan || model.look then
//                            yield onMouseMove Move
//
//                        yield onMouseDown (fun b p -> Down(b,p))
//                        yield onMouseUp (fun b _ -> Up b)
//                    ]
//            update = update
//            initial = initial
//        }

//    let run (f : 'model -> 'msg) (app : Tool<'model, 'm>) : amap<string, AttributeValue<'msg>> =
////        let rec run (m : 'model) =
////            
////        failwith ""
//
////    let att = cameraControllerApp |> run (fun (cam : CameraControllerState) -> Down (MouseButtons.None, V2i.Zero))
//
//    let controlWASD (msg : CameraControllerState -> 'msg) : Attribute<'msg> =
//        failwith ""

    let view (state : MCameraControllerState) =
        let frustum = Frustum.perspective 60.0 0.1 100.0 1.0
        div [attribute "style" "display: flex; flex-direction: row; width: 100%; height: 100%; border: 0; padding: 0; margin: 0"] [
  
            controlledControl state id 
                (Mod.constant frustum)
                (AttributeMap.empty)
                (
                    Sg.box' C4b.Green (Box3d(-V3d.III, V3d.III))
                        |> Sg.shader {
                            do! DefaultSurfaces.trafo
                            do! DefaultSurfaces.vertexColor
                            do! DefaultSurfaces.simpleLighting
                        }
                        |> Sg.noEvents
                )
        ]




    let threads (state : CameraControllerState) =
        let pool = ThreadPool.empty
       
        let rec time() =
            proclist {
                do! Proc.Sleep 10
                yield StepTime
                yield! time()
            }

        if state.moveVec <> V3i.Zero then
            ThreadPool.add "timer" (time()) pool

        else
            pool


    let start () =
        App.start {
            unpersist = Unpersist.instance
            view = view
            threads = threads
            update = update
            initial = initial
        }