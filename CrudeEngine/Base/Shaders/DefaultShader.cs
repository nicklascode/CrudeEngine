using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Light;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.IO;
using CrudeEngine.SceneSystem;
using CrudeEngine.Utils;
using OpenGL;
using System;
using System.Numerics;

namespace CrudeEngine.Base.Shaders
{
    public class DefaultShader : Shader
    {
        public DefaultShader() : base()
        {
            // Load the shader source code from files
            string vertexShaderSource = AssetLoader.LoadShader("Base/Shaders/DefaultVertexShader.glsl");
            string fragmentShaderSource = AssetLoader.LoadShader("Base/Shaders/DefaultFragmentShader.glsl");

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
            AddUniform("uModel");
            AddUniform("uProjection");
            AddUniform("uView");

            AddUniform("uMaterial.color");
            AddUniform("uMaterial.texture");
            AddUniform("useTexture");

            // Light
            const int MAX_LIGHTS = 8;
            for (int i = 0; i < MAX_LIGHTS; i++)
            {
                AddUniform($"uLights[{i}].position");
                AddUniform($"uLights[{i}].color");
            }
            AddUniform("lightCount");

        }

        public override void UpdateUniforms(Entity entity)
        {
            // Compute matrices  
            Matrix4x4 model = MathHelper.CreateTransformMatrix(entity.Transform);
            Matrix4x4 view = Camera.Main.ViewMatrix;
            Matrix4x4 projection = Camera.Main.ProjectionMatrix;
            Matrix4x4 mvp = model * view * projection;

            // Set uniforms
            SetUniform("uModel", model);
            SetUniform("uView", view);
            SetUniform("uProjection", projection);

            MaterialComponent material = entity.GetComponent<MaterialComponent>() as MaterialComponent;
            if (material == null)
                throw new Exception($"MaterialComponent not found in entity {entity}");

            BaseMaterial baseMaterial = material.GetMaterial<BaseMaterial>();
            if (baseMaterial == null)
                throw new Exception($"BaseMaterial not found in MaterialComponent of entity {entity}");

            SetUniform("uMaterial.color", baseMaterial.Color);

            if (baseMaterial.Texture != null)
            {
                baseMaterial.Texture.Bind(); // Bind to texture unit 0  
                SetUniform("uMaterial.texture", 0); // Tell shader to sample from unit 0  
                SetUniform("useTexture", 1);
            }
            else
            {
                SetUniform("useTexture", 0); // No texture, fallback to solid color  
            }

            // Set light uniforms  
            Scene scene = Game.Instance.GetCurrentScene();
            const int MAX_LIGHTS = 8;
            if (scene != null)
            {
                List<Light>? lights = scene.GetLights() as List<Light>;
                if (lights != null)
                {
                    int count = System.Math.Min(lights.Count, MAX_LIGHTS);
                    SetUniform("lightCount", count);
                    for (int i = 0; i < count; i++)
                    {
                        Light light = lights[i];
                        SetUniform($"uLights[{i}].position", light.Transform.Position);
                        SetUniform($"uLights[{i}].color", light.Color);
                    }
                }
            }
        }
    }
}
