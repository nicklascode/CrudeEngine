using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using CrudeEngine.Utils;
using Sandbox.RenderProperties;
using Sandbox.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.PBR
{
    public class PBREntity : Entity, ICloneable
    {
        public object Clone()
        {
            PBREntity clone = new PBREntity();
            clone.Transform.Position = this.Transform.Position;
            clone.Transform.Rotation = this.Transform.Rotation;
            clone.Transform.Scale = this.Transform.Scale;

            foreach (var component in this.components)
            {
                //if we already have a component with the same name, we should not clone it
                if (clone.components.ContainsKey(component.Key))
                {
                    continue;
                }

                clone.AddComponent(component.Key, component.Value);
            }

            return clone;
        }

        public PBREntity()
        {
            // Initialize the PBR entity with a transform and a PBR shader component
            transform.Position = new Vector3(0, 0, 0);
            PBRMaterial material = new PBRMaterial();
            material.SetDiffuse(Texture.LoadTexture("Textures/Ground078_2K-PNG/Ground078_2K-PNG_Color.png")); // Load a diffuse texture
            material.SetNormalMap(Texture.LoadTexture("Textures/Ground078_2K-PNG/Ground078_2K-PNG_NormalGL.png")); // Load a normal map texture
            material.SetDisplacement(Texture.LoadTexture("Textures/Ground078_2K-PNG/Ground078_2K-PNG_Displacement.png")); // Load a displacement map texture
            material.SetRoughness(Texture.LoadTexture("Textures/Ground078_2K-PNG/Ground078_2K-PNG_Roughness.png")); // Load a roughness map texture

            MaterialComponent materialComponent = new MaterialComponent(material);
            materialComponent.SetMaterial(material);
            AddComponent("Material", materialComponent);
            RenderInfo renderInfo = new RenderInfo(new DefaultRenderProperties(), new PBRShader());
            Renderer renderer = new Renderer();
            MeshVBO meshVBO = new MeshVBO();
            meshVBO.Allocate(OBJLoader.Load("Objs/Sphere.obj")); // Load a PBR model mesh
            renderer.SetMeshVBO(meshVBO);
            renderer.SetRenderInfo(renderInfo);
            AddComponent("Renderer", renderer);
        }
    }
}
