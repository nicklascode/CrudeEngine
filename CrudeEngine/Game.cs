using System;
using CrudeEngine.Graphics;
using CrudeEngine.SceneSystem;
using OpenGL;

namespace CrudeEngine
{
    public class Game
    {
        public static Game Instance { get; private set; } = new Game();

        public struct GameConfig
        {
            public string GameName { get; set; }
            public string GameVersion { get; set; }

            public GameConfig(string name, string version)
            {
                GameName = name;
                GameVersion = version;
            }
        }

        public GameConfig Config { get; private set; }
        protected Window _window;
        protected Input _input;
        protected Scene _currentScene;
        protected bool _isRunning;

        public Game()
        {
            Instance = this;
            Config = new GameConfig { GameName = "Crude Engine", GameVersion = "1.0" };
            var props = new WindowProps("Crude Engine", 800, 600);
            _window = new Window(props);
            // Don't initialize Input here - defer until after window is created
        }

        public Game(WindowProps props, GameConfig config)
        {
            Instance = this;
            Config = config;
            _window = new Window(props);
            // Don't initialize Input here - defer until after window is created
        }

        protected void InitializeInput()
        {
            if (_window.Handle != IntPtr.Zero)
            {
                Input.Initialize(_window.Handle);
                _input = Input.Instance;
            }
        }

        public void SetScene(Scene scene)
        {
            _currentScene = scene;
        }

        private void Initialize()
        {
            if (_isRunning)
            {
                Console.WriteLine("Game is already running.");
                return;
            }

            if (!_window.IsExternal)
            {
                if (!_window.Initialize())
                {
                    Console.WriteLine("Failed to initialize the window.");
                    return;
                }
            }
            else
            {
                _window.InitializeHeadless();
            }

            // Initialize Input after the window is created
            InitializeInput();

            if (!_window.IsExternal)
            {
                if (_window.IsOpenGLInitialized)
                {
                    _isRunning = true;
                    Console.WriteLine("Window initialized successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to create OpenGL context.");
                    _isRunning = false;
                }
            }
        }

        public virtual void Start()
        {
            if (_currentScene != null)
            {
                _currentScene.Start();
            }
            else
            {
                Console.WriteLine("No scene set to start.");
            }
        }

        public virtual void Update(float deltaTime) {
            if (_currentScene != null)
            {
                _currentScene.Update(deltaTime);
            }
            else
            {
                Console.WriteLine("No scene set to update.");
            }
            _input?.Update();
        }

        public virtual void Render()
        {
            if (_currentScene != null)
            {
                _currentScene.Render();
            }
            else
            {
                Console.WriteLine("No scene set to render.");
            }
        }

        public void Run()
        {
            Initialize();
            if (!_isRunning) return;

            if (!_window.IsExternal)
            {
                _window.Clear(1f, 0f, 0f, 1f);
                _window.Present();
            }

            Start();

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            while (_isRunning)
            {
                if (!_window.IsExternal)
                    _isRunning = _window.PollEvents();
                else
                    _isRunning = true;

                float deltaTime = (float)timer.Elapsed.TotalSeconds;
                timer.Restart();

                Update(deltaTime);
                _window.UpdateHooks(deltaTime);
                
                if (!_window.IsExternal)
                    _window.Clear();
                Render();
                _window.RenderHooks();
                
                if (!_window.IsExternal)
                    _window.Present();
            }

            Destroy();
        }

        public void Stop()
        {
            _isRunning = false;
            Environment.Exit(1);
        }

        public void Destroy()
        {
            _window?.Destroy();
            _window = null;
        }

        public Window GetWindow()
        {
            return _window;
        }

        public Input GetInput()
        {
            return _input;
        }

        public Scene GetCurrentScene()
        {
            return _currentScene;
        }
    }
}
