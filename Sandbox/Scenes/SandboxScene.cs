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
using CrudeEngine.PhysicsEngine;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine;

namespace Sandbox.Scenes
{
    public class SandboxScene : Scene
    {
        private Camera mainCamera;
        Entity aabbEntity;
        Entity aabb2Entity;

        public SandboxScene()
        {

        }

        public override void Start()
        {
            base.Start();
            mainCamera = new Camera(70f, 1280f / 720f, 0.1f, 1000f);
            Camera.SetMain(mainCamera);

            AddLight(new Light(new Vector3(0, 0, 0), new Vector3(1,0,0), 100f));

            Entity bobEntity = new Entity();
            BaseMaterial baseMaterial = new BaseMaterial(new Vector3(1.0f, 1.0f, 1.0f));
            Renderer renderer = new Renderer();
            renderer.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
            MeshVBO meshVBO = new MeshVBO();
            meshVBO.Allocate(OBJLoader.Load("Objs/Bob.obj")); // Load a simple sphere mesh
            renderer.SetMeshVBO(meshVBO);
            bobEntity.AddComponent("Material", new MaterialComponent(baseMaterial));
            bobEntity.AddComponent("Renderer", renderer);
            bobEntity.Transform.Position = new Vector3(20, 0, 0); // Position the entity at the origin


            // Initialize entities
            AddEntity(bobEntity);
            AddEntity(new SkydomeEntity());
            AddEntity(new PBREntity());
            AddEntity(new TerrainEntity());

            // Add a water entity
            WaterEntity waterEntity = new WaterEntity();
            waterEntity.Transform.Position = new Vector3(0, -5, 0); // Position the water below the terrain
            AddEntity(waterEntity);

            // AABB Testing
            AABB aabb = new AABB(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            AABB aabb2 = new AABB(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));

            aabbEntity = new Entity();
            aabbEntity.Transform.Position = new Vector3(0, 0, 0);
            aabbEntity.AddComponent("AABB", aabb);

            aabb2Entity = new Entity();
            aabb2Entity.Transform.Position = new Vector3(0, 5, 0);
            aabb2Entity.AddComponent("AABB", aabb2);

            Renderer rendererAABB = new Renderer();
            rendererAABB.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
            MeshVBO meshAABB_VBO = new MeshVBO();
            meshAABB_VBO.Allocate(OBJLoader.Load("Objs/Cube.obj")); // Load a simple sphere mesh
            rendererAABB.SetMeshVBO(meshAABB_VBO);
            aabbEntity.AddComponent("Material", new MaterialComponent(baseMaterial));
            aabbEntity.AddComponent("Renderer", rendererAABB);

            Renderer rendererAABB2 = new Renderer();
            rendererAABB2.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
            rendererAABB2.SetMeshVBO(meshAABB_VBO); // Reuse the same mesh for simplicity
            aabb2Entity.AddComponent("Material", new MaterialComponent(baseMaterial));
            aabb2Entity.AddComponent("Renderer", rendererAABB2);

            AddEntity(aabbEntity);
            AddEntity(aabb2Entity);

            Console.WriteLine("Sandbox Scene Started");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            mainCamera.Update(deltaTime);

            // Set light position to follow the camera
            foreach (var light in GetLights())
            {
                light.Transform.Position = mainCamera.Transform.Position + new Vector3(0, 5, 0); // Position light above camera
            }

            if (Input.Instance.IsKeyPressed(SDL2.SDL.SDL_Keycode.SDLK_DOWN))
            {
                aabb2Entity.Transform.Position += new Vector3(0, -0.1f, 0);
            }
            if (Input.Instance.IsKeyPressed(SDL2.SDL.SDL_Keycode.SDLK_UP))
            {
                aabb2Entity.Transform.Position += new Vector3(0, 0.1f, 0);
            }

            if (aabb2Entity.GetComponent<AABB>() != null && aabbEntity.GetComponent<AABB>() != null)
            {
                AABB aabb1 = aabbEntity.GetComponent<AABB>();
                AABB aabb2 = aabb2Entity.GetComponent<AABB>();
                
                if(aabb2.IsColliding(aabb1))
                {
                    Console.WriteLine("AABBs are colliding!");
                }
            }
        }
    }
}
