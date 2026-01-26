using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Utils;
using OpenGL;
using Sandbox.Entities.PBR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Shaders
{
    public class PBRShader : Shader
    {
        public PBRShader()
        {
            // Define the vertex shader code  
            string vertexShaderCode = AssetLoader.LoadShader("Shaders/PBR/PBRVertexShader.glsl");
            // Add the vertex shader to the program  
            AddVertexShader(vertexShaderCode);
            // Define the fragment shader code  
            string fragmentShaderCode = AssetLoader.LoadShader("Shaders/PBR/PBRFragmentShader.glsl");
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
            AddUniform("uMVP");
            AddUniform("displacementMap");

            AddUniform("material.diffuse");
            AddUniform("material.normalMap");
            AddUniform("material.roughnessMap");
            AddUniform("material.displacementMap");
            AddUniform("material.ambient");
            AddUniform("material.diffuseColor");
            AddUniform("material.specular");
            AddUniform("material.shininess");
            AddUniform("lightPos");
            AddUniform("viewPos");
        }

        public override void UpdateUniforms(Entity entity)
        {
            // Get the view and projection matrices from the camera  
            Matrix4x4 viewMatrix = Camera.Main.ViewMatrix;
            Matrix4x4 projectionMatrix = Camera.Main.ProjectionMatrix;
            Matrix4x4 modelMatrix = MathHelper.CreateTransformMatrix(entity.Transform);
            Matrix4x4 mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;
            SetUniform("uMVP", mvpMatrix);

            PBRMaterial? pbrMaterial = entity.GetComponent<MaterialComponent>()?.GetMaterial<PBRMaterial>();
            if (pbrMaterial == null)
            {
                throw new Exception($"PBRMaterial not found in entity {entity}");
            }

            Texture? diffuse = pbrMaterial.Diffuse;
            Texture? normalMap = pbrMaterial.NormalMap;
            Texture? displacementMap = pbrMaterial.Displacement;
            Texture? roughnessMap = pbrMaterial.Roughness;

            // Check if diffuse or normalMap is null
            if (diffuse == null)
            {
                throw new Exception($"Diffuse texture is null for entity {entity}");
            }

            if (normalMap == null)
            {
                throw new Exception($"Normal map texture is null for entity {entity}");
            }

            if (displacementMap == null)
            {
                throw new Exception($"Displacement map texture is null for entity {entity}");
            }

            if (roughnessMap == null)
            {
                throw new Exception($"Roughness map texture is null for entity {entity}");
            }

            // Set material properties
            SetUniform("material.diffuseColor", pbrMaterial.DiffuseColor);
            diffuse.Bind(0); // Bind to texture unit 0
            SetUniform("material.diffuse", 0); // Texture unit 0
            normalMap.Bind(1); // Bind to texture unit 1
            SetUniform("material.normalMap", 1); // Texture unit 1
            displacementMap.Bind(2); // Bind to texture unit 2
            SetUniform("material.displacementMap", 2); // Texture unit 2
            SetUniform("displacementMap", 2); // Texture unit 2 for displacement map
            roughnessMap.Bind(3); // Bind to texture unit 3
            SetUniform("material.roughnessMap", 3); // Texture unit 3
            SetUniform("material.ambient", pbrMaterial.Ambient);
            SetUniform("material.specular", pbrMaterial.Specular);
            SetUniform("material.shininess", pbrMaterial.Shininess);

            // Hardcoded material properties for PBR shader
            SetUniform("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
            SetUniform("viewPos", Camera.Main.Transform.Position);
        }
    }
}
