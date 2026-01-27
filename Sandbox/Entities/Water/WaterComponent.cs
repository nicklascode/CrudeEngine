using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine.Graphics.Renderer;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Water
{
    public class WaterComponent : Component
    {
        private Mesh mesh;
        private int width;
        private int height;

        private float density = 1f; // Density of the water mesh

        public WaterComponent(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override void Start()
        {
            base.Start();
            mesh = new Mesh();
            GenerateMesh();

            if (GetEntity().GetComponent<Renderer>() != null)
            {
                Renderer renderer = GetEntity().GetComponent<Renderer>();
                if (renderer != null)
                {
                    MeshVBO meshVBO = new MeshVBO();
                    meshVBO.Allocate(mesh);
                    renderer.SetMeshVBO(meshVBO);
                }
            }

            SetupTexture();
        }

        private void SetupTexture()
        {
            // Load the water texture
            Texture waterTexture = Texture.LoadTexture("Textures/WaterTexture.png");

            waterTexture.SetParameter(TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            waterTexture.SetParameter(TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            waterTexture.SetParameter(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            waterTexture.SetParameter(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            if (GetEntity().GetComponent<MaterialComponent>() != null)
            {
                MaterialComponent material = GetEntity().GetComponent<MaterialComponent>();
                if (material != null && material.GetMaterial<BaseMaterial>() is BaseMaterial baseMaterial)
                {
                    baseMaterial.SetTexture(waterTexture);
                }
            }
        }

        private void GenerateMesh()
        {
            int vertexCountX = (int)(width * density);
            int vertexCountZ = (int)(height * density);

            Vertex[] vertices = new Vertex[vertexCountX * vertexCountZ];
            int index = 0;

            for (int x = 0; x < vertexCountX; x++)
            {
                for (int z = 0; z < vertexCountZ; z++)
                {
                    float posX = x / density; // Scale position by density
                    float posZ = z / density;

                    Vector3 position = new Vector3(posX, 0, posZ);
                    Vector3 normal = new Vector3(0, 1, 0); // Flat water surface
                    Vector2 uv = new Vector2((float)x / (vertexCountX - 1), (float)z / (vertexCountZ - 1));
                    vertices[index++] = new Vertex(position, normal, uv);
                }
            }

            uint[] indices = new uint[(vertexCountX - 1) * (vertexCountZ - 1) * 6];
            index = 0;

            for (int x = 0; x < vertexCountX - 1; x++)
            {
                for (int z = 0; z < vertexCountZ - 1; z++)
                {
                    uint topLeft = (uint)(x * vertexCountZ + z);
                    uint topRight = topLeft + 1;
                    uint bottomLeft = (uint)((x + 1) * vertexCountZ + z);
                    uint bottomRight = bottomLeft + 1;

                    // First triangle
                    indices[index++] = topLeft;
                    indices[index++] = topRight;
                    indices[index++] = bottomLeft;

                    // Second triangle
                    indices[index++] = topRight;
                    indices[index++] = bottomRight;
                    indices[index++] = bottomLeft;
                }
            }

            List<Vertex> vertexList = new List<Vertex>(vertices);
            List<uint> indexList = new List<uint>(indices);

            mesh.SetVertices(vertexList);
            mesh.SetIndices(indexList);

            Console.WriteLine($"Water mesh generated with {vertexCountX}x{vertexCountZ} vertices and {indexList.Count} indices.");
        }

    }
}
