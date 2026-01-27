using CrudeEngine.Graphics;
using CrudeEngine.IO;
using Sandbox.Scenes;
using System;

namespace CrudeEngine.CrudeEditor
{
    public class EngineEditor : CrudeEngine.Game
    {
        public ImGuiController Controller;

        public EngineEditor() : base(new WindowProps("Crude Editor", 1280, 720), new GameConfig("Crude Editor", "0.0.1"))
        {
            AssetLoader.SetRootPath(Path.Combine(AppContext.BaseDirectory, "Assets"));
            Controller = new ImGuiController();
            _currentScene = new EditorScene();

            Controller.Run();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
