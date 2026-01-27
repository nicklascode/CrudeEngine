using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.IO;
using CrudeEngine.Utils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Shaders
{
    public class WaterShader : Shader
    {
        public WaterShader() : base()
        {
            // Load the shader source code from files
            string vertexShaderSource = AssetLoader.LoadShader("Shaders/Water/WaterVertexShader.glsl");
            string fragmentShaderSource = AssetLoader.LoadShader("Shaders/Water/WaterFragmentShader.glsl");

            // Add shaders to program
            AddVertexShader(vertexShaderSource);
            AddFragmentShader(fragmentShaderSource);
            Link();

            // Error check
            ErrorCode error = Gl.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine($"OpenGL Error: {error}");
            }

            // Add uniforms
            AddUniform("uMVP");
            AddUniform("uTime");
            AddUniform("uWaveAmplitude");
            AddUniform("uWaveFrequency");
            AddUniform("uTexture");
        }

        public override void UpdateUniforms(Entity entity)
        {
            // Compute matrices
            Matrix4x4 model = MathHelper.CreateTransformMatrix(entity.Transform);
            Matrix4x4 view = Camera.Main.ViewMatrix;
            Matrix4x4 projection = Camera.Main.ProjectionMatrix;
            Matrix4x4 mvp = model * view * projection;

            SetUniform("uMVP", mvp);
            SetUniform("uTime", (float)DateTime.UtcNow.TimeOfDay.TotalSeconds);
            SetUniform("uWaveAmplitude", 0.1f);
            SetUniform("uWaveFrequency", 1.0f);

            // Texture 
            if (entity.GetComponent<MaterialComponent>() != null)
            {
                MaterialComponent material = entity.GetComponent<MaterialComponent>();
                if (material != null && material.GetMaterial<BaseMaterial>() is BaseMaterial baseMaterial)
                {
                    baseMaterial.GetTexture()?.Bind(0);
                    SetUniform("uTexture", 0); // Bind texture to unit 0
                }
                else
                {
                    Console.WriteLine("BaseMaterial not found in MaterialComponent.");
                }
            }
            else
            {
                Console.WriteLine("No MaterialComponent found on entity.");
            }

            // Depth texture
        }
    }
}

