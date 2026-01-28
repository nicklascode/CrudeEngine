using ClickableTransparentOverlay;
using CrudeEngine.CrudeEditor.UI;
using CrudeEngine.CrudeEditor.Windows;
using ImGuiNET;
using System.Collections.Generic;

namespace CrudeEngine.CrudeEditor
{
    public class ImGuiController : Overlay
    {
        private List<EditorWindow> windows = new();
        private EditorDockspace _dockspace = new();
        private EditorMenuBar _menuBar = new();

        public ImGuiController() : base(1920, 1080)
        {
            windows.Add(new ProjectWindow());
            windows.Add(new SceneWindow());
            windows.Add(new EntityEditorWindow());
        }

        protected override Task PostInitialized()
        {
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            VSync = true;
            return Task.CompletedTask;
        }

        public void AddWindow(EditorWindow window)
        {
            windows.Add(window);
            window.OnOpen();
        }

        public void RemoveWindow(EditorWindow window)
        {
            window.OnClose();
            windows.Remove(window);
        }

        protected override void Render()
        {
            _dockspace.Render();
            _menuBar.Render();
            _dockspace.EndRender();

            foreach (var window in windows)
            {
                window.Draw();
            }
        }
    }
}
