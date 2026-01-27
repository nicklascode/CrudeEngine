using CrudeEngine.ECS;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics;
using CrudeEngine.SceneSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrudeEngine.Graphics.Light;
using System.Numerics;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using CrudeEngine.Utils;
using CrudeEngine.Base.Shaders;

namespace Sandbox.Scenes
{
    public class EditorScene : Scene
    {
        private Camera mainCamera;

        public override void Start()
        {
            base.Start();
            mainCamera = new Camera(70f, 1280f / 720f, 0.1f, 1000f);
            Camera.SetMain(mainCamera);

            AddLight(new Light(new Vector3(0, 5, 0), new Vector3(1,1,1), 100f));

            Entity cubeEntity = new Entity();
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            Renderer renderer = new Renderer();
            renderer.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
            MeshVBO meshVBO = new MeshVBO();
            meshVBO.Allocate(OBJLoader.Load("Base/Objs/Cube.obj")); // Load a simple cube
            renderer.SetMeshVBO(meshVBO);
            cubeEntity.AddComponent("Material", new MaterialComponent(baseMaterial));
            cubeEntity.AddComponent("Renderer", renderer);


            // Initialize entities
            AddEntity(cubeEntity);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            mainCamera.Update(deltaTime);
        }
    }
}
