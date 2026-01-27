using ClickableTransparentOverlay;
using CrudeEngine.CrudeEditor.Windows;
using CrudeEngine.Graphics;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CrudeEngine.CrudeEditor
{
    public class ImGuiController : Overlay
    {
        private List<EditorWindow> windows = new();

        public ImGuiController() : base(1920, 1080)
        {
            // Register default windows
            windows.Add(new ProjectWindow());
            windows.Add(new SceneWindow());
        }

        protected override Task PostInitialized()
        {
            // Enable docking
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            // Allow overlay to not always be on top
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
            //// Setup dockspace window
            //ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

            //ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Always);
            //ImGui.SetNextWindowSize(new Vector2(Game.Instance.GetWindow().Width, Game.Instance.GetWindow().Height));
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            //windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
            //        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
            //        ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus |
            //        ImGuiWindowFlags.NoBackground;

            //ImGui.Begin("DockSpace Demo", windowFlags);
            //ImGui.PopStyleVar(2);

            //// DockSpace - use PassthruCentralNode to allow interaction with underlying window
            //ImGuiDockNodeFlags dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;
            //ImGui.DockSpace(ImGui.GetID("DockSpace"), Vector2.Zero, dockspaceFlags);

            //ImGui.End();

            ImGui.ShowDemoWindow();

            foreach (var window in windows)
            {
                window.Draw();
            }
        }
    }
}
