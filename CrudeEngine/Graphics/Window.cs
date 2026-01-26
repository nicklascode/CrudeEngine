using System;
using SDL2;
using OpenGL;

namespace CrudeEngine.Graphics
{
    public struct WindowProps
    {
        public string Title;
        public int Width;
        public int Height;

        public WindowProps(string title, int width, int height)
        {
            Title = title;
            Width = width;
            Height = height;
        }
    }

    public class Window
    {
        private IntPtr _window;
        private IntPtr _glContext;

        public int Width { get; }
        public int Height { get; }
        public string Title { get; }

        public bool IsOpenGLInitialized => _glContext != IntPtr.Zero;

        public IntPtr Handle => _window;

        public Window(WindowProps props)
        {
            Title = props.Title;
            Width = props.Width;
            Height = props.Height;
        }

        public bool Initialize()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
                return false;
            }
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            _window = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, Width, Height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (_window == IntPtr.Zero)
            {
                Console.WriteLine($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                return false;
            }
            OpenGL.Gl.Initialize();
            _glContext = SDL.SDL_GL_CreateContext(_window);
            if (_glContext == IntPtr.Zero)
            {
                Console.WriteLine($"OpenGL context could not be created! SDL_Error: {SDL.SDL_GetError()}");
                return false;
            }
            Gl.Viewport(0, 0, Width, Height);

            Console.WriteLine($"OpenGL context created successfully. Version: {Gl.GetString(StringName.Version)}");
            return true;
        }

        public bool PollEvents()
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    return false;

            }
            return true;
        }

        public void Clear(float r = 0f, float g = 0f, float b = 0f, float a = 1f)
        {
            Gl.ClearColor(r, g, b, a);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.Viewport(0, 0, Width, Height);
        }

        public void Present()
        {
            SDL.SDL_GL_SwapWindow(_window);
        }

        public void Destroy()
        {
            SDL.SDL_GL_DeleteContext(_glContext);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }
}
