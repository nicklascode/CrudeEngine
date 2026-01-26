using SDL2;
using System;
using System.Runtime.InteropServices;

namespace CrudeEngine
{
    public enum MouseButton
    {
        Left = (int)SDL.SDL_BUTTON_LEFT,
        Middle = (int)SDL.SDL_BUTTON_MIDDLE,
        Right = (int)SDL.SDL_BUTTON_RIGHT
    }

    public class Input
    {
        public static Input Instance { get; private set; }

        private IntPtr _windowHandle;
        private byte[] _keyboardState;
        private int _numKeys;

        private int _mouseX, _mouseY;
        private int _lastMouseX, _lastMouseY;
        private int _mouseScrollDeltaX, _mouseScrollDeltaY;

        private Input(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            SDL.SDL_GetKeyboardState(out _numKeys);
            _keyboardState = new byte[_numKeys];
        }

        public static void Initialize(IntPtr windowHandle)
        {
            if (Instance == null)
                Instance = new Input(windowHandle);
        }

        public void Update()
        {
            SDL.SDL_PumpEvents();
            IntPtr statePtr = SDL.SDL_GetKeyboardState(out _);
            Marshal.Copy(statePtr, _keyboardState, 0, _numKeys);

            _lastMouseX = _mouseX;
            _lastMouseY = _mouseY;
            SDL.SDL_GetMouseState(out _mouseX, out _mouseY);

            int windowWidth, windowHeight;
            SDL.SDL_GetWindowSize(_windowHandle, out windowWidth, out windowHeight);

            if (IsMouseCursorGrabbed())
                Input.Instance.WarpMouseIfGrabbed(windowWidth / 2, windowHeight / 2);

            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
                {
                    _mouseScrollDeltaX += e.wheel.x;
                    _mouseScrollDeltaY += e.wheel.y;
                }
            }
        }

        public bool IsKeyPressed(SDL.SDL_Keycode key)
        {
            var scanCode = SDL.SDL_GetScancodeFromKey(key);
            int index = (int)scanCode;
            if (index < 0 || index >= _numKeys)
                return false;
            return _keyboardState[index] != 0;
        }

        public bool IsMouseButtonPressed(MouseButton button)
        {
            int state = (int)SDL.SDL_GetMouseState(out _, out _);
            return (state & SDL.SDL_BUTTON((uint)(int)button)) != 0;
        }
        public (int x, int y) GetMousePosition() => (_mouseX, _mouseY);

        public (int dx, int dy) GetMouseDelta() => (_mouseX - _lastMouseX, _mouseY - _lastMouseY);
        public (int scrollX, int scrollY) GetMouseScrollDelta() => (_mouseScrollDeltaX, _mouseScrollDeltaY);

        public void SetMousePosition(int x, int y)
        {
            SDL.SDL_WarpMouseInWindow(_windowHandle, x, y);
        }

        public void GrabMouseCursor(bool grab)
        {
            if (grab)
            {
                // Hide the cursor when grabbing
                ShowMouseCursor(false);
            }
            else
            {
                // Show the cursor when releasing
                ShowMouseCursor(true);
            }

            SDL.SDL_SetRelativeMouseMode(grab ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }

        public void WarpMouseIfGrabbed(int x, int y)
        {
            bool wasGrabbed = IsMouseCursorGrabbed();

            if (wasGrabbed)
                SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_FALSE); // Temporarily disable relative mode

            SDL.SDL_WarpMouseInWindow(_windowHandle, x, y);

            if (wasGrabbed)
                SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_TRUE); // Re-enable it
        }


        public bool IsMouseCursorGrabbed()
        {
            return SDL.SDL_GetRelativeMouseMode() == SDL.SDL_bool.SDL_TRUE;
        }

        public void ShowMouseCursor(bool show)
        {
            SDL.SDL_ShowCursor(show ? 1 : 0);
        }

        public bool IsMouseCursorVisible()
        {
            return SDL.SDL_ShowCursor(-1) == 1; // -1 returns the current visibility state
        }
    }
}
