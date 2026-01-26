using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using CrudeEngine.ECS;
using CrudeEngine.Math;
using CrudeEngine.Utils;
using OpenGL;
using SDL2;

namespace CrudeEngine.Graphics
{
    public class Camera
    {
        public static Camera Main { get; private set; }
        public static void SetMain(Camera camera) => Main = camera;

        public Transform Transform { get; private set; }
        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }
        public float FieldOfView { get; private set; }
        public float AspectRatio { get; private set; }
        public float NearPlane { get; private set; }
        public float FarPlane { get; private set; }

        public Camera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            Transform = new Transform();
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            NearPlane = nearPlane;
            FarPlane = farPlane;
            UpdateProjectionMatrix();
        }

        public void Update(float deltaTime)
        {
            UpdateViewMatrix();
            FreeCam();
        }

        float speed = 1f; // Speed of camera movement
        public void FreeCam()
        {
            float mouseX = Input.Instance.GetMouseDelta().dx;
            float mouseY = Input.Instance.GetMouseDelta().dy;

            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_w))
                Transform.Position += Transform.GetForward() * speed;
            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_s))
                Transform.Position -= Transform.GetForward() * speed;
            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_a))
                Transform.Position += Transform.GetRight() * speed;
            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_d))
                Transform.Position -= Transform.GetRight() * speed;
            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_q))
                Transform.Position -= Transform.GetUp() * speed;
            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_e))
                Transform.Position += Transform.GetUp() * speed;

            // Mouse movement
            float sensitivity = 0.1f;
            float yaw = mouseX * sensitivity;
            float pitch = mouseY * sensitivity;

            if(Input.Instance.IsMouseButtonPressed(MouseButton.Right))
            {
                // Update rotation based on mouse movement
                Transform.Rotation = Quaternion.CreateFromAxisAngle(-Vector3.UnitY, MathHelper.DegreesToRadians(yaw)) * Transform.Rotation;
                Transform.Rotation = Quaternion.CreateFromAxisAngle(Transform.GetRight(), MathHelper.DegreesToRadians(pitch)) * Transform.Rotation;
                Transform.Rotation = Quaternion.Normalize(Transform.Rotation);
            }

            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_LSHIFT) || Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_RSHIFT))
            {
                speed = 1f; // Increase speed when shift is pressed
            }
            else
            {
                speed = 0.1f; // Reset to normal speed
            }
        }

        public void UpdateProjectionMatrix()
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(FieldOfView),
                AspectRatio,
                NearPlane,
                FarPlane);
        }
        public void UpdateViewMatrix()
        {
            Vector3 forward = Transform.GetForward();
            Vector3 up = Transform.GetUp();
            Vector3 right = Transform.GetRight();
            ViewMatrix = Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + forward, up);
        }
    }
}
