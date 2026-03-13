using OpenGL;
using SDL2;
using System;
using System.IO;
using System.Runtime.InteropServices;
using StbImageSharp;
using CrudeEngine.IO;

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

        // Hook points
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

            // Try to set the default icon from Assets/CrudeEnigneLogo.png
            try
            {
                SetWindowIcon();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set window icon: {ex.Message}");
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

        public bool SetWindowIcon(string? relativePath = null)
        {
            if (_window == IntPtr.Zero)
                return false;

            // Default to the base assets
            string iconName = string.IsNullOrEmpty(relativePath) ? Path.Combine("Base", "CrudeEnigneLogo.png") : relativePath;
            string fullPath = iconName;
            if (!string.IsNullOrEmpty(AssetLoader.ROOT_PATH))
                fullPath = Path.Combine(AssetLoader.ROOT_PATH, iconName);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Icon file not found: {fullPath}");
                return false;
            }

            // Load image using StbImageSharp
            using (var fs = File.OpenRead(fullPath))
            {
                ImageResult image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);

                // Pin managed pixel array
                GCHandle handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                try
                {
                    IntPtr pixelPtr = handle.AddrOfPinnedObject();
                    int width = image.Width;
                    int height = image.Height;
                    int depth = 32; // RGBA 8 bits each
                    int pitch = width * 4;

                    // Create an SDL surface from the pixel data
                    IntPtr surface = SDL.SDL_CreateRGBSurfaceFrom(pixelPtr, width, height, depth, pitch,
                        0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

                    if (surface == IntPtr.Zero)
                    {
                        Console.WriteLine($"SDL_CreateRGBSurfaceFrom failed: {SDL.SDL_GetError()}");
                        return false;
                    }

                    // Set the window icon (SDL will make its own copy)
                    SDL.SDL_SetWindowIcon(_window, surface);

                    // Free the surface created by SDL
                    SDL.SDL_FreeSurface(surface);

                    return true;
                }
                finally
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
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
