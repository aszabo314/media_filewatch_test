namespace PRo3DModels

open System
open Aardvark.Base
open Aardvark.Base.Incremental

[<AutoOpen>]
module Mutable =

    [<StructuredFormatDisplay("{AsString}")>]
    type MAnnotation private(__initial : PRo3DModels.Annotation) =
        let mutable __current = __initial
        let _geometry = ResetMod(__initial.geometry)
        let _points = ResetMod(__initial.points)
        let _segments = ResetMod(__initial.segments)
        let _color = ResetMod(__initial.color)
        let _thickness = Aardvark.UI.Mutable.MNumericInput.Create(__initial.thickness)
        let _projection = ResetMod(__initial.projection)
        let _visible = ResetMod(__initial.visible)
        let _text = ResetMod(__initial.text)
        
        member x.geometry = _geometry :> IMod<_>
        member x.points = _points :> IMod<_>
        member x.segments = _segments :> IMod<_>
        member x.color = _color :> IMod<_>
        member x.thickness = _thickness
        member x.projection = _projection :> IMod<_>
        member x.visible = _visible :> IMod<_>
        member x.text = _text :> IMod<_>
        
        member x.Update(__model : PRo3DModels.Annotation) =
            if not (Object.ReferenceEquals(__model, __current)) then
                __current <- __model
                _geometry.Update(__model.geometry)
                _points.Update(__model.points)
                _segments.Update(__model.segments)
                _color.Update(__model.color)
                _thickness.Update(__model.thickness)
                _projection.Update(__model.projection)
                _visible.Update(__model.visible)
                _text.Update(__model.text)
        
        static member Create(initial) = MAnnotation(initial)
        
        override x.ToString() = __current.ToString()
        member private x.AsString = sprintf "%A" __current
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MAnnotation =
        let inline geometry (m : MAnnotation) = m.geometry
        let inline points (m : MAnnotation) = m.points
        let inline segments (m : MAnnotation) = m.segments
        let inline color (m : MAnnotation) = m.color
        let inline thickness (m : MAnnotation) = m.thickness
        let inline projection (m : MAnnotation) = m.projection
        let inline visible (m : MAnnotation) = m.visible
        let inline text (m : MAnnotation) = m.text
    
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Annotation =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let geometry =
                { new Lens<PRo3DModels.Annotation, PRo3DModels.Geometry>() with
                    override x.Get(r) = r.geometry
                    override x.Set(r,v) = { r with geometry = v }
                    override x.Update(r,f) = { r with geometry = f r.geometry }
                }
            let points =
                { new Lens<PRo3DModels.Annotation, PRo3DModels.Points>() with
                    override x.Get(r) = r.points
                    override x.Set(r,v) = { r with points = v }
                    override x.Update(r,f) = { r with points = f r.points }
                }
            let segments =
                { new Lens<PRo3DModels.Annotation, Microsoft.FSharp.Collections.list<PRo3DModels.Segment>>() with
                    override x.Get(r) = r.segments
                    override x.Set(r,v) = { r with segments = v }
                    override x.Update(r,f) = { r with segments = f r.segments }
                }
            let color =
                { new Lens<PRo3DModels.Annotation, Aardvark.Base.C4b>() with
                    override x.Get(r) = r.color
                    override x.Set(r,v) = { r with color = v }
                    override x.Update(r,f) = { r with color = f r.color }
                }
            let thickness =
                { new Lens<PRo3DModels.Annotation, Aardvark.UI.NumericInput>() with
                    override x.Get(r) = r.thickness
                    override x.Set(r,v) = { r with thickness = v }
                    override x.Update(r,f) = { r with thickness = f r.thickness }
                }
            let projection =
                { new Lens<PRo3DModels.Annotation, PRo3DModels.Projection>() with
                    override x.Get(r) = r.projection
                    override x.Set(r,v) = { r with projection = v }
                    override x.Update(r,f) = { r with projection = f r.projection }
                }
            let visible =
                { new Lens<PRo3DModels.Annotation, Microsoft.FSharp.Core.bool>() with
                    override x.Get(r) = r.visible
                    override x.Set(r,v) = { r with visible = v }
                    override x.Update(r,f) = { r with visible = f r.visible }
                }
            let text =
                { new Lens<PRo3DModels.Annotation, Microsoft.FSharp.Core.string>() with
                    override x.Get(r) = r.text
                    override x.Set(r,v) = { r with text = v }
                    override x.Update(r,f) = { r with text = f r.text }
                }
    [<StructuredFormatDisplay("{AsString}")>]
    type MRenderingParameters private(__initial : PRo3DModels.RenderingParameters) =
        let mutable __current = __initial
        let _fillMode = ResetMod(__initial.fillMode)
        let _cullMode = ResetMod(__initial.cullMode)
        
        member x.fillMode = _fillMode :> IMod<_>
        member x.cullMode = _cullMode :> IMod<_>
        
        member x.Update(__model : PRo3DModels.RenderingParameters) =
            if not (Object.ReferenceEquals(__model, __current)) then
                __current <- __model
                _fillMode.Update(__model.fillMode)
                _cullMode.Update(__model.cullMode)
        
        static member Create(initial) = MRenderingParameters(initial)
        
        override x.ToString() = __current.ToString()
        member private x.AsString = sprintf "%A" __current
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MRenderingParameters =
        let inline fillMode (m : MRenderingParameters) = m.fillMode
        let inline cullMode (m : MRenderingParameters) = m.cullMode
    
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module RenderingParameters =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let fillMode =
                { new Lens<PRo3DModels.RenderingParameters, Aardvark.Base.Rendering.FillMode>() with
                    override x.Get(r) = r.fillMode
                    override x.Set(r,v) = { r with fillMode = v }
                    override x.Update(r,f) = { r with fillMode = f r.fillMode }
                }
            let cullMode =
                { new Lens<PRo3DModels.RenderingParameters, Aardvark.Base.Rendering.CullMode>() with
                    override x.Get(r) = r.cullMode
                    override x.Set(r,v) = { r with cullMode = v }
                    override x.Update(r,f) = { r with cullMode = f r.cullMode }
                }
    [<StructuredFormatDisplay("{AsString}")>]
    type MComposedViewerModel private(__initial : PRo3DModels.ComposedViewerModel) =
        let mutable __current = __initial
        let _camera = Demo.TestApp.Mutable.MCameraControllerState.Create(__initial.camera)
        let _singleAnnotation = MAnnotation.Create(__initial.singleAnnotation)
        let _rendering = MRenderingParameters.Create(__initial.rendering)
        
        member x.camera = _camera
        member x.singleAnnotation = _singleAnnotation
        member x.rendering = _rendering
        
        member x.Update(__model : PRo3DModels.ComposedViewerModel) =
            if not (Object.ReferenceEquals(__model, __current)) then
                __current <- __model
                _camera.Update(__model.camera)
                _singleAnnotation.Update(__model.singleAnnotation)
                _rendering.Update(__model.rendering)
        
        static member Create(initial) = MComposedViewerModel(initial)
        
        override x.ToString() = __current.ToString()
        member private x.AsString = sprintf "%A" __current
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MComposedViewerModel =
        let inline camera (m : MComposedViewerModel) = m.camera
        let inline singleAnnotation (m : MComposedViewerModel) = m.singleAnnotation
        let inline rendering (m : MComposedViewerModel) = m.rendering
    
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module ComposedViewerModel =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let camera =
                { new Lens<PRo3DModels.ComposedViewerModel, Demo.TestApp.CameraControllerState>() with
                    override x.Get(r) = r.camera
                    override x.Set(r,v) = { r with camera = v }
                    override x.Update(r,f) = { r with camera = f r.camera }
                }
            let singleAnnotation =
                { new Lens<PRo3DModels.ComposedViewerModel, PRo3DModels.Annotation>() with
                    override x.Get(r) = r.singleAnnotation
                    override x.Set(r,v) = { r with singleAnnotation = v }
                    override x.Update(r,f) = { r with singleAnnotation = f r.singleAnnotation }
                }
            let rendering =
                { new Lens<PRo3DModels.ComposedViewerModel, PRo3DModels.RenderingParameters>() with
                    override x.Get(r) = r.rendering
                    override x.Set(r,v) = { r with rendering = v }
                    override x.Update(r,f) = { r with rendering = f r.rendering }
                }
