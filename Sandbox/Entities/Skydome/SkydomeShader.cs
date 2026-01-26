using CrudeEngine;
using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Utils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Skydome
{
    public class SkydomeShader : Shader
    {
        public SkydomeShader()
        {
            // Load and compile vertex shader
            string vertexShaderCode = AssetLoader.LoadShader("Shaders/Skydome/SkydomeVertexShader.glsl");
            AddVertexShader(vertexShaderCode);
            // Load and compile fragment shader
            string fragmentShaderCode = AssetLoader.LoadShader("Shaders/Skydome/SkydomeFragmentShader.glsl");
            AddFragmentShader(fragmentShaderCode);
            // Link the shader program
            Gl.LinkProgram(_programId);
            // Check for linking errors
            ErrorCode error = Gl.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new Exception($"Failed to link skydome shader program. Error code: {error}");
            }
            // Add uniforms
            AddUniform("viewMatrix");
            AddUniform("projectionMatrix");
            AddUniform("cameraPosition");
            AddUniform("resolution");
        }

        public override void UpdateUniforms(Entity entity)
        {
            // Get the view and projection matrices from the camera
            Matrix4x4 viewMatrix = Camera.Main.ViewMatrix;
            Matrix4x4 projectionMatrix = Camera.Main.ProjectionMatrix;
            // Set the uniforms for the shader
            SetUniform("viewMatrix", viewMatrix);
            SetUniform("projectionMatrix", projectionMatrix);
            SetUniform("cameraPosition", Camera.Main.Transform.Position);
            SetUniform("resolution", new Vector2(Game.Instance.GetWindow().Width, Game.Instance.GetWindow().Height));
        }
    }
}
