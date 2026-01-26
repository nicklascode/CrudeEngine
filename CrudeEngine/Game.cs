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
            Config = new GameConfig { GameName = "Crude Engine", GameVersion = "1.0" };
            var props = new WindowProps("Crude Engine", 800, 600);
            _window = new Window(props);
            Input.Initialize(_window.Handle);
            _input = Input.Instance;
        }

        public Game(WindowProps props, GameConfig config)
        {
            Config = config;
            _window = new Window(props);
            Input.Initialize(_window.Handle);
            _input = Input.Instance;
        }

        public void SetScene(Scene scene)
        {
            _currentScene = scene;
            Run();
        }

        private void Initialize()
        {
            if(_isRunning)
            {
                Console.WriteLine("Game is already running.");
                return;
            }

            if (!_window.Initialize())
            {
                Console.WriteLine("Failed to initialize the window.");
                return;
            }

            // Only call is running if the window opengl context is created successfully
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

        protected virtual void Start()
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

        protected virtual void Update(float deltaTime) {
            if (_currentScene != null)
            {
                _currentScene.Update(deltaTime);
            }
            else
            {
                Console.WriteLine("No scene set to update.");
            }
            _input.Update();
        }

        protected virtual void Render()
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
            // Indicate loading of game resources
            _window.Clear(1f, 0f, 0f, 1f); // Clear with red color
            _window.Present(); // Present the cleared window

            Start();

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            while (_isRunning)
            {
                _isRunning = _window.PollEvents();

                float deltaTime = (float)timer.Elapsed.TotalSeconds;
                timer.Restart();

                Update(deltaTime);
                _window.Clear();   // Default black background
                Render();
                _window.Present();
            }

            Destroy();
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
