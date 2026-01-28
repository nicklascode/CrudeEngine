using OpenGL;
using SDL2;
using System;
using static SDL2.SDL;

namespace CrudeEngine.Graphics
{
    public struct WindowProps
    {
        public string Title;
        public int Width;
        public int Height;
        public bool IsExternal;

        public SDL.SDL_WindowFlags Flags;

        public WindowProps(string title, int width, int height, SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE, bool isExternal = false)
        {
            Title = title;
            Width = width;
            Height = height;
            IsExternal = isExternal;
            Flags = flags;
        }
    }

    public class Window
    {
        private IntPtr _window;
        private IntPtr _glContext;
        private WindowProps _props;
        private bool _isExternal;

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public string Title { get; }

        public bool IsOpenGLInitialized => _glContext != IntPtr.Zero;

        public IntPtr Handle => _window;
        public IntPtr GLContext => _glContext;
        public bool IsExternal => _isExternal;

        // Hook points for external UI systems (e.g., ImGui)
        public event Action? OnInitialized;
        public event Action<SDL.SDL_Event>? OnEvent;
        public event Action<float>? OnUpdate;
        public event Action? OnRender;
        public event Action? OnDestroy;

        public Window(WindowProps props)
        {
            Title = props.Title;
            Width = props.Width;
            Height = props.Height;
            _isExternal = props.IsExternal;
            _props = props;
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
            _window = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, Width, Height, _props.Flags | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
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
            
            // Notify subscribers that initialization is complete
            OnInitialized?.Invoke();
            
            return true;
        }

        public void InitializeHeadless()
        {
            _isExternal = true;
            // SDL Can  go bye bye!

            OpenGL.Gl.Initialize();
            Gl.Viewport(0, 0, Width, Height);
            
            OnInitialized?.Invoke();
        }

        public bool PollEvents()
        {
            if (!_isExternal)
            {
                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    // Forward event to subscribers (e.g., ImGui)
                    OnEvent?.Invoke(e);
                    if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
                    {
                        if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        {
                            Width = e.window.data1;
                            Height = e.window.data2;

                            Gl.Viewport(0, 0, Width, Height);
                        }
                    }

                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        return false;
                }
            }

            return true;
        }

        public void UpdateHooks(float deltaTime)
        {
            OnUpdate?.Invoke(deltaTime);
        }

        public void RenderHooks()
        {
            OnRender?.Invoke();
        }

        public void Clear(float r = 0f, float g = 0f, float b = 0f, float a = 1f)
        {
            Gl.ClearColor(r, g, b, a);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.Viewport(0, 0, Width, Height);
        }

        public void Present()
        {
            // For external handles, we still need to swap buffers
            SDL.SDL_GL_SwapWindow(_window);
        }

        public void Destroy()
        {
            OnDestroy?.Invoke();
            
            if (_glContext != IntPtr.Zero)
            {
                SDL.SDL_GL_DeleteContext(_glContext);
                _glContext = IntPtr.Zero;
            }

            if (_window != IntPtr.Zero && !_isExternal)
            {
                SDL.SDL_DestroyWindow(_window);
            }
            _window = IntPtr.Zero;

            SDL.SDL_Quit();
        }
    }
}
