using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using System.Numerics;

namespace CrudeEngine.CrudeEditor.Windows
{
    public abstract class EditorWindow
    {
        public string Title { get; protected set; }
        private bool _isOpen = true;
        public bool IsOpen
        {
            get => _isOpen;
            set => _isOpen = value;
        }
        public Vector2 DefaultSize { get; protected set; } = new Vector2(400, 300);

        protected EditorWindow(string title)
        {
            Title = title;
        }

        public void Draw()
        {
            if (!IsOpen)
                return;

            ImGui.SetNextWindowSize(DefaultSize, ImGuiCond.FirstUseEver);

            bool isOpen = IsOpen;
            if (ImGui.Begin(Title, ref isOpen, ImGuiWindowFlags.None))
            {
                OnRender();
            }
            ImGui.End();
            IsOpen = isOpen;
        }

        protected abstract void OnRender();

        public virtual void OnOpen() { }
        public virtual void OnClose() { }
    }
}
