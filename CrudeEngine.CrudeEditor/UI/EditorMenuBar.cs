using CrudeEngine.Graphics;
using ImGuiNET;
using System.Numerics;

namespace CrudeEngine.CrudeEditor.UI
{
    public class EditorMenuBar
    {
        private bool _showFileBrowser = false;
        private string _currentBrowserPath = AppContext.BaseDirectory;

        public void Render()
        {
            if (ImGui.BeginMainMenuBar())
            {
                RenderProjectMenu();
                ImGui.EndMainMenuBar();
            }

            if (_showFileBrowser)
            {
                RenderFileBrowser();
            }
        }

        private void RenderProjectMenu()
        {
            if (ImGui.BeginMenu("Project"))
            {
                if (ImGui.MenuItem("Open..."))
                {
                    _showFileBrowser = true;
                    _currentBrowserPath = AppContext.BaseDirectory;
                }

                if (ImGui.MenuItem("Close Project", Project.Project.Current != null))
                {
                    Project.Project.Unload();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Exit"))
                {
                    Game.Instance.Stop();
                }

                ImGui.EndMenu();
            }
        }

        private void RenderFileBrowser()
        {
            ImGui.SetNextWindowSize(new Vector2(500, 400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Open Project", ref _showFileBrowser))
            {
                ImGui.Text($"Current: {_currentBrowserPath}");
                ImGui.Separator();

                if (ImGui.Button(".."))
                {
                    var parent = Directory.GetParent(_currentBrowserPath);
                    if (parent != null)
                    {
                        _currentBrowserPath = parent.FullName;
                    }
                }

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
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Access denied");
                }

                ImGui.Separator();
                if (ImGui.Button("Open"))
                {
                    try
                    {
                        Project.Project.Load(_currentBrowserPath);
                        _showFileBrowser = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load project: {ex.Message}");
                    }
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