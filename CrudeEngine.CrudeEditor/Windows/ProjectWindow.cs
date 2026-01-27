using CrudeEngine.CrudeEditor.Project;
using ImGuiNET;
using System.Numerics;

namespace CrudeEngine.CrudeEditor.Windows
{
    public class ProjectWindow : EditorWindow
    {
        private string _projectPathInput = string.Empty;
        private bool _showFileBrowser = false;
        private string _currentBrowserPath = AppContext.BaseDirectory;

        public ProjectWindow() : base("Project")
        {
            DefaultSize = new Vector2(500, 400);
        }

        protected override void OnRender()
        {
            if (Project.Project.Current != null)
            {
                RenderCurrentProject();
            }
            else
            {
                RenderNoProject();
            }

            if (_showFileBrowser)
            {
                RenderFileBrowser();
            }
        }

        private void RenderCurrentProject()
        {
            var project = Project.Project.Current!;

            ImGui.Text($"Project: {project.Name}");
            ImGui.Separator();
            ImGui.Text($"Path: {project.ProjectPath}");
            ImGui.Text($"Assets: {project.AssetsPath}");

            ImGui.Spacing();
            if (ImGui.Button("Unload Project"))
            {
                Project.Project.Unload();
            }
        }

        private void RenderNoProject()
        {
            ImGui.Text("No project loaded");
            ImGui.Separator();

            ImGui.InputText("Project Path", ref _projectPathInput, 512);
            ImGui.SameLine();
            if (ImGui.Button("Browse..."))
            {
                _showFileBrowser = true;
            }

            if (ImGui.Button("Load Project") && !string.IsNullOrWhiteSpace(_projectPathInput))
            {
                try
                {
                    Project.Project.Load(_projectPathInput);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load project: {ex.Message}");
                }
            }
        }

        private void RenderFileBrowser()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Select Project Directory", ref _showFileBrowser))
            {
                ImGui.Text($"Current: {_currentBrowserPath}");
                ImGui.Separator();

                // Parent directory button
                if (ImGui.Button(".."))
                {
                    var parent = Directory.GetParent(_currentBrowserPath);
                    if (parent != null)
                    {
                        _currentBrowserPath = parent.FullName;
                    }
                }

                // List directories
                try
                {
                    var directories = Directory.GetDirectories(_currentBrowserPath);
                    foreach (var dir in directories)
                    {
                        var dirName = Path.GetFileName(dir);
                        if (ImGui.Selectable($"[DIR] {dirName}"))
                        {
                            _currentBrowserPath = dir;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), "Access denied");
                }

                ImGui.Separator();
                if (ImGui.Button("Select This Folder"))
                {
                    _projectPathInput = _currentBrowserPath;
                    _showFileBrowser = false;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    _showFileBrowser = false;
                }
            }
            ImGui.End();
        }
    }
}