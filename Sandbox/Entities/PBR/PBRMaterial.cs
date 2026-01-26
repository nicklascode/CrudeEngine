using CrudeEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.PBR
{
    public class PBRMaterial : BaseMaterial
    {
        public Texture? Diffuse { get; set; }
        public Texture? NormalMap { get; set; }
        public Texture? Displacement { get; set; }
        public Texture? Roughness { get; set; }
        public float Ambient { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 Specular { get; set; }
        public float Shininess { get; set; }

        public PBRMaterial()
        {
            Ambient = 0.1f; // Default ambient light intensity
            DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f); // Default diffuse color is white
            Specular = new Vector3(1.0f, 1.0f, 1.0f); // Default specular color is white
            Shininess = 32.0f; // Default shininess value
        }

        public PBRMaterial(Vector3 diffuseColor, float ambient = 0.1f, Vector3 specular = default, float shininess = 32.0f)
        {
            Ambient = ambient;
            DiffuseColor = diffuseColor;
            Specular = specular == default ? new Vector3(1.0f, 1.0f, 1.0f) : specular; // Default specular color is white
            Shininess = shininess;
        }

        public void SetDiffuse(Texture diffuse)
        {
            Diffuse = diffuse;
        }

        public Texture? GetDiffuse()
        {
            return Diffuse;
        }

        public void SetNormalMap(Texture normalMap)
        {
            NormalMap = normalMap;
        }

        public Texture? GetNormalMap()
        {
            return NormalMap;
        }

        public void SetDisplacement(Texture displacement)
        {
            Displacement = displacement;
        }

        public Texture? GetDisplacement()
        {
            return Displacement;
        }

        public void SetRoughness(Texture roughness)
        {
            Roughness = roughness;
        }

        public Texture? GetRoughness()
        {
            return Roughness;
        }
    }
}
