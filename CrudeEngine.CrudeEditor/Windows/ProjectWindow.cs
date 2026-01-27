using CrudeEngine.CrudeEditor.Project;
using ImGuiNET;
using System.Numerics;

namespace CrudeEngine.CrudeEditor.Windows
{
    public class ProjectWindow : EditorWindow
    {
        public ProjectWindow() : base("Project")
        {
            DefaultSize = new Vector2(300, 200);
        }

        protected override void OnRender()
        {
            if (Project.Project.Current != null)
            {
                RenderProjectInfo();
            }
            else
            {
                RenderNoProject();
            }
        }

        private void RenderProjectInfo()
        {
            var project = Project.Project.Current!;

            ImGui.TextColored(new Vector4(0.4f, 0.8f, 0.4f, 1.0f), "Project Loaded");
            ImGui.Separator();

            ImGui.Text("Name:");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.6f, 1.0f), project.Name);

            ImGui.Spacing();

            ImGui.Text("Path:");
            ImGui.TextWrapped(project.ProjectPath);

            ImGui.Spacing();

            ImGui.Text("Assets:");
            ImGui.TextWrapped(project.AssetsPath);

            if (Directory.Exists(project.AssetsPath))
            {
                ImGui.TextColored(new Vector4(0.4f, 0.8f, 0.4f, 1.0f), "Assets folder found");
            }
            else
            {
                ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.4f, 1.0f), "Assets folder not found");
            }
        }

        private void RenderNoProject()
        {
            ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), "No project loaded");
            ImGui.Separator();
            ImGui.Spacing();
            ImGui.TextWrapped("Use Project > Open... from the menu bar to load a project.");
        }
    }
}