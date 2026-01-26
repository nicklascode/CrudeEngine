using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using CrudeEngine.Utils;
using Sandbox.RenderProperties;
using Sandbox.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Skydome
{
    public class SkydomeEntity : Entity
    {
        public SkydomeEntity()
        {
            // Initialize the skydome entity with a transform and a skydome shader component
            transform.Position = new Vector3(0, 0, 0);
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            AddComponent("Material", new MaterialComponent(baseMaterial));

            RenderInfo renderInfo = new RenderInfo(new CCWRenderProperty(), new SkydomeShader());
            Renderer renderer = new Renderer();
            MeshVBO meshVBO = new MeshVBO();
            meshVBO.Allocate(OBJLoader.Load("Objs/Skydome.obj")); // Load a skydome mesh
            renderer.SetMeshVBO(meshVBO);
            renderer.SetRenderInfo(renderInfo);
            AddComponent("Renderer", renderer);
        }

    }
}
