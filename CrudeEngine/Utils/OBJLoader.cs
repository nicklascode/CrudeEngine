using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.IO;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine.IO;

namespace CrudeEngine.Utils
{
    public class OBJLoader
    {
        public static Mesh Load(string relativeFilePath)
        {
            string rootPath = AssetLoader.ROOT_PATH;
            string filePath = Path.Combine(rootPath, relativeFilePath);

            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();

            var vertices = new List<Vertex>();
            var indices = new List<uint>();
            var vertexMap = new Dictionary<string, uint>();

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                switch (parts[0])
                {
                    case "v":
                        positions.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)));
                        break;

                    case "vn":
                        normals.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)));
                        break;

                    case "vt":
                        texCoords.Add(new Vector2(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)));
                        break;

                    case "f":
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] faceParts = parts[i].Split('/');
                            int posIndex = int.Parse(faceParts[0]) - 1;
                            int texIndex = faceParts.Length > 1 && faceParts[1] != "" ? int.Parse(faceParts[1]) - 1 : -1;
                            int normIndex = faceParts.Length > 2 ? int.Parse(faceParts[2]) - 1 : -1;

                            string key = $"{posIndex}/{texIndex}/{normIndex}";

                            if (!vertexMap.TryGetValue(key, out uint vertexIndex))
                            {
                                var vertex = new Vertex
                                {
                                    Position = positions[posIndex],
                                    TexCoord = texIndex >= 0 ? texCoords[texIndex] : Vector2.Zero,
                                    Normal = normIndex >= 0 ? normals[normIndex] : Vector3.Zero
                                };
                                vertices.Add(vertex);
                                vertexIndex = (uint)(vertices.Count - 1);
                                vertexMap[key] = vertexIndex;
                            }

                            indices.Add(vertexIndex);
                        }
                        break;
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices);

            Console.WriteLine($"Loaded OBJ: {relativeFilePath}");

            return mesh;
        }
    }
}
