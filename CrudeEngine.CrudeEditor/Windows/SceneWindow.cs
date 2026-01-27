using CrudeEngine.CrudeEditor.Project;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace CrudeEngine.CrudeEditor.Windows
{
    public class SceneWindow : EditorWindow
    {
        private bool _showSceneFileBrowser = false;
        private string _sceneFileBrowserPath = AppContext.BaseDirectory;

        public SceneWindow() : base("Scene")
        {
            DefaultSize = new Vector2(500, 400);
        }

        protected override void OnRender()
        {
            var project = Project.Project.Current;
            if (project == null)
            {
                ImGui.Text("No project loaded");
                return;
            }

            ImGui.Text($"Project: {project.Name}");
            ImGui.Separator();

            if (ImGui.Button("Load Scene..."))
            {
                _sceneFileBrowserPath = project.ProjectPath;
                _showSceneFileBrowser = true;
            }

            if (_showSceneFileBrowser)
            {
                RenderSceneFileBrowser();
            }
        }

        private void RenderSceneFileBrowser()
        {
            ImGui.SetNextWindowSize(new Vector2(500, 400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Select Scene File", ref _showSceneFileBrowser))
            {
                ImGui.Text($"Current: {_sceneFileBrowserPath}");
                ImGui.Separator();

                if (ImGui.Button(".."))
                {
                    var parent = Directory.GetParent(_sceneFileBrowserPath);
                    if (parent != null)
                    {
                        _sceneFileBrowserPath = parent.FullName;
                    }
                }

                try
                {
                    var directories = Directory.GetDirectories(_sceneFileBrowserPath);
                    foreach (var dir in directories)
                    {
                        var dirName = Path.GetFileName(dir);
                        if (ImGui.Selectable($"[DIR] {dirName}"))
                        {
                            _sceneFileBrowserPath = dir;
                        }
                    }

                    var files = Directory.GetFiles(_sceneFileBrowserPath, "*.cs");
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        if (ImGui.Selectable(fileName))
                        {
                            LoadSceneFromFile(file);
                            _showSceneFileBrowser = false;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Access denied");
                }

                ImGui.Separator();
                if (ImGui.Button("Cancel"))
                {
                    _showSceneFileBrowser = false;
                }
            }
            ImGui.End();
        }

        private void LoadSceneFromFile(string filePath)
        {
            try
            {
                var project = Project.Project.Current;
                if (project == null) return;

                var dllPath = Path.Combine(project.ProjectPath, "bin", "Debug", "net10.0", $"{project.Name}.dll");

                if (!File.Exists(dllPath))
                {
                    Console.WriteLine($"Project assembly not found: {dllPath}");
                    return;
                }

                // Use LoadFile to load into a new context, avoiding type conflicts
                var assembly = Assembly.LoadFile(dllPath);
                var sceneBaseType = typeof(CrudeEngine.SceneSystem.Scene);

                var className = Path.GetFileNameWithoutExtension(filePath);

                Console.WriteLine($"Searching for scene class: {className}");
                Console.WriteLine($"Available types in assembly:");

                foreach (var type in assembly.GetTypes())
                {
                    Console.WriteLine($"  - {type.FullName} (IsSubclassOf Scene: {sceneBaseType.IsAssignableFrom(type)})");

                    if (sceneBaseType.IsAssignableFrom(type) && !type.IsAbstract &&
                        (type.Name == className || type.Name.EndsWith(className)))
                    {
                        Console.WriteLine($"Found matching type: {type.FullName}");

                        var sceneInstance = Activator.CreateInstance(type) as CrudeEngine.SceneSystem.Scene;
                        if (sceneInstance != null)
                        {
                            Console.WriteLine($"Created scene instance: {sceneInstance.GetType().FullName}");
                            EngineEditor.Instance.SetScene(sceneInstance);
                            Console.WriteLine($"Loaded scene: {type.FullName}");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"Failed to cast instance to Scene. Instance type: {Activator.CreateInstance(type)?.GetType().FullName}");
                        }
                    }
                }

                Console.WriteLine($"No Scene subclass found matching: {className}");
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"ReflectionTypeLoadException: {ex.Message}");
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    Console.WriteLine($"  Loader exception: {loaderEx?.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load scene: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}