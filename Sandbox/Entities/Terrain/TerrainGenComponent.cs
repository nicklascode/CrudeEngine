using CrudeEngine;
using CrudeEngine.ECS;
using CrudeEngine.Graphics;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine.Graphics.Renderer;
using CrudeEngine.Graphics.Renderer.RenderProperties;
using CrudeEngine.Math;
using CrudeEngine.Utils;
using OpenGL;
using Sandbox.Shaders;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities.Terrain
{
    public class TerrainGenComponent : Component
    {
        private Mesh mesh;
        private int width;
        private int height;
        private int resolution = 1; // Resolution of the terrain mesh

        private float[,] heightMap;
        private float noiseScale = 8f;
        private Texture terrainTexture;
        private int seed = 42;

        public TerrainGenComponent(int width, int height, float noiseScale = 8f, int resolution = 1)
        {
            this.width = width;
            this.height = height;
            this.noiseScale = noiseScale;
            this.resolution = resolution;
            // Ensure width and height are multiples of resolution for even grid
            if (width % resolution != 0 || height % resolution != 0)
            {
                throw new ArgumentException("Width and height must be multiples of the resolution.");
            }
        }

        public override void Start()
        {
            base.Start();
            mesh = new Mesh();
            GenerateHeightMap();
            GenerateMesh();
        }

        public override void Update(float deltaTime)
        {
            base.Update();

            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_j))
            {
                noiseScale += 0.1f;
                GenerateHeightMap();
                GenerateMesh();
            }
            else if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_k))
            {
                noiseScale -= 0.1f;
                GenerateHeightMap();
                GenerateMesh();
            }

            if (Input.Instance.IsKeyPressed(SDL.SDL_Keycode.SDLK_r))
            {
                seed = new Random().Next();
                GenerateHeightMap();
                GenerateMesh();
            }
        }

        private void GenerateHeightMap()
        {
            heightMap = new float[width, height];
            var perlin = new PerlinNoise(seed); // Use a fixed seed for reproducibility

            float scale = noiseScale;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float nx = (float)x / width * scale;
                    float ny = (float)y / height * scale;

                    // Octaves for fractal noise
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;
                    float persistence = 0.5f;
                    float lacunarity = 2f;
                    int octaves = 8;
                    float falloff = 1.2f; // Island falloff factor

                    for (int o = 0; o < octaves; o++)
                    {
                        float perlinValue = perlin.Noise(nx * frequency, ny * frequency);
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    nx += (float)(seed % 1000) / 1000f; // Add some randomness based on seed
                    ny += (float)(seed % 1000) / 1000f; // Add some randomness based on seed

                    // Apply falloff to create an island effect
                    float centerX = width / 2f;
                    float centerY = height / 2f;
                    float distanceFromCenter = MathF.Sqrt(MathF.Pow(x - centerX, 2) + MathF.Pow(y - centerY, 2));
                    float maxDistance = MathF.Sqrt(MathF.Pow(centerX, 2) + MathF.Pow(centerY, 2));
                    float falloffFactor = MathF.Max(0, 1 - (distanceFromCenter / maxDistance) * falloff);
                    noiseHeight *= falloffFactor;

                    // Normalize to [-1, 1]
                    noiseHeight = noiseHeight / ((1 - MathF.Pow(persistence, octaves)) / (1 - persistence));
                    noiseHeight = MathHelper.Clamp(noiseHeight * 2f - 1f, -1f, 1f);

                    heightMap[x, y] = noiseHeight;
                }
            }
        }

        private void GenerateTexture()
        {
            int texWidth = width;
            int texHeight = height;
            byte[] pixelData = new byte[texWidth * texHeight * 4]; // RGBA

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    float heightValue = (heightMap[x, y] + 1f) * 0.5f; // Normalize to [0,1]
                    byte color = (byte)(heightValue * 255);

                    int index = (y * texWidth + x) * 4;
                    pixelData[index + 0] = color; // R
                    pixelData[index + 1] = color; // G
                    pixelData[index + 2] = color; // B
                    pixelData[index + 3] = 255;   // A

                    if (heightValue < 0.3f)
                    {
                        pixelData[index + 0] = 194; // R (Sand)
                        pixelData[index + 1] = 178; // G
                        pixelData[index + 2] = 128; // B

                    }
                    else if (heightValue < 0.6f)
                    {
                        pixelData[index + 0] = 34;  // R (Grass)
                        pixelData[index + 1] = 139; // G
                        pixelData[index + 2] = 34;  // B
                    }
                    else
                    {
                        pixelData[index + 0] = 139; // R (Rock)
                        pixelData[index + 1] = 137; // G
                        pixelData[index + 2] = 137; // B
                    }
                }
            }

            // Smooth the texture by averaging neighboring pixels
            for (int x = 1; x < texWidth - 1; x++)
            {
                for (int y = 1; y < texHeight - 1; y++)
                {
                    int index = (y * texWidth + x) * 4;
                    byte r = (byte)((pixelData[index - texWidth * 4] + pixelData[index + texWidth * 4] +
                                     pixelData[index - 4] + pixelData[index + 4]) / 4);
                    byte g = (byte)((pixelData[index - texWidth * 4 + 1] + pixelData[index + texWidth * 4 + 1] +
                                     pixelData[index - 3] + pixelData[index + 5]) / 4);
                    byte b = (byte)((pixelData[index - texWidth * 4 + 2] + pixelData[index + texWidth * 4 + 2] +
                                     pixelData[index - 2] + pixelData[index + 6]) / 4);
                    pixelData[index] = r;
                    pixelData[index + 1] = g;
                    pixelData[index + 2] = b;
                }
            }

            terrainTexture = new Texture(texWidth, texHeight, pixelData);
        }


        private void GenerateMesh()
        {
            List<Vertex> vertices = new List<Vertex>();
            Dictionary<(int, int), int> vertexIndices = new Dictionary<(int, int), int>();

            int vertIndex = 0;
            for (int x = 0; x < width; x += resolution)
            {
                for (int y = 0; y < height; y += resolution)
                {
                    float heightValue = heightMap[x, y];
                    Vector3 position = new Vector3(x, heightValue * 10f, y); // Scale height
                    Vector2 uv = new Vector2((float)x / width, (float)y / height);
                    Vector3 normal = Vector3.UnitY; // Flat normal for simplicity

                    vertices.Add(new Vertex(position, normal, uv));
                    vertexIndices[(x, y)] = vertIndex++;
                }
            }

            List<uint> indices = new List<uint>();
            for (int x = 0; x < width - resolution; x += resolution)
            {
                for (int y = 0; y < height - resolution; y += resolution)
                {
                    uint topLeft = (uint)vertexIndices[(x, y)];
                    uint topRight = (uint)vertexIndices[(x, y + resolution)];
                    uint bottomLeft = (uint)vertexIndices[(x + resolution, y)];
                    uint bottomRight = (uint)vertexIndices[(x + resolution, y + resolution)];

                    // Flip indices for backface culling
                    // Triangle 1
                    indices.Add(topLeft);
                    indices.Add(topRight);
                    indices.Add(bottomLeft);

                    // Triangle 2
                    indices.Add(topRight);
                    indices.Add(bottomRight);
                    indices.Add(bottomLeft);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices);

            GenerateTexture();

            if (terrainTexture != null)
            {
                if (entity.GetComponent<MaterialComponent>() is MaterialComponent existingMaterialComponent)
                {
                    BaseMaterial existingMaterial = existingMaterialComponent.GetMaterial<BaseMaterial>();
                    existingMaterial.SetTexture(terrainTexture);
                }
                else
                {
                    BaseMaterial material = new BaseMaterial();
                    material.SetTexture(terrainTexture);
                    entity.AddComponent("MaterialComponent", new MaterialComponent(material));
                }
            }

            if (entity.GetComponent<Renderer>() is Renderer renderer)
            {
                MeshVBO meshVBO = new MeshVBO();
                meshVBO.Allocate(mesh);
                renderer.SetMeshVBO(meshVBO);
            }
            else
            {
                Renderer newRenderer = new Renderer();
                MeshVBO meshVBO = new MeshVBO();
                meshVBO.Allocate(mesh);
                newRenderer.SetMeshVBO(meshVBO);
                newRenderer.SetRenderInfo(new RenderInfo(new DefaultRenderProperties(), new DefaultShader()));
                entity.AddComponent("Renderer", newRenderer);
            }
        }
    }
}
