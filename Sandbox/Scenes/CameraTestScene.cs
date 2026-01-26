using CrudeEngine.ECS;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics;
using CrudeEngine.SceneSystem;
using Sandbox.Entities.PBR;
using Sandbox.Entities.Skydome;
using Sandbox.Entities.Terrain;
using Sandbox.Entities.Water;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrudeEngine.Graphics.Light;
using System.Numerics;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using Sandbox.Shaders;
using CrudeEngine.Utils;

namespace Sandbox.Scenes
{
    public class CameraTestScene : Scene
    {
        private Camera mainCamera;

        public override void Start()
        {
            base.Start();
            mainCamera = new Camera(70f, 1280f / 720f, 0.1f, 1000f);
            Camera.SetMain(mainCamera);

            AddLight(new Light(new Vector3(0, 5, 0), new Vector3(1,1,1), 100f));

            Entity bobEntity = new Entity();
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            Renderer renderer = new Renderer();
            renderer.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
            MeshVBO meshVBO = new MeshVBO();
            meshVBO.Allocate(OBJLoader.Load("Objs/Bob.obj")); // Load a simple sphere mesh
            renderer.SetMeshVBO(meshVBO);
            bobEntity.AddComponent("Material", new MaterialComponent(baseMaterial));
            bobEntity.AddComponent("Renderer", renderer);


            // Initialize entities
            AddEntity(bobEntity);
            AddEntity(new SkydomeEntity());
            AddEntity(new PBREntity());
            AddEntity(new TerrainEntity());

            // Add a water entity
            WaterEntity waterEntity = new WaterEntity();
            waterEntity.Transform.Position = new Vector3(0, -5, 0); // Position the water below the terrain
            AddEntity(waterEntity);
            Console.WriteLine("Sandbox Scene Started");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            mainCamera.Update(deltaTime);
        }
    }
}
