using CrudeEngine;
using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using CrudeEngine.SceneSystem;
using CrudeEngine.Utils;
using OpenGL;
using Sandbox.Entities.PBR;
using Sandbox.Entities.Skydome;
using Sandbox.Entities.Terrain;
using Sandbox.Entities.Water;
using Sandbox.Scenes;
using Sandbox.Shaders;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    public class SandboxGame : CrudeEngine.Game
    {
        public SandboxGame() : base(new WindowProps("Sandbox Game", 1280, 720))
        {
            AssetLoader.SetRootPath(Path.Combine(AppContext.BaseDirectory, "Assets"));
            _currentScene = new SandboxScene();
        }

        protected override void Start()
        {
            base.Start();
            Console.WriteLine("Sandbox Game Started");
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if(_input.IsKeyPressed(SDL.SDL_Keycode.SDLK_F1))
            {
                long fps = (long)(1.0f / deltaTime);
                Console.WriteLine($"FPS: {fps}");
            }
        }
    }
}
