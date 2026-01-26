using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Graphics.Mesh
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector3 Tangent;
        public Vector3 Bitangent;

        public Vertex(Vector3 position)
        {
            Position = position;
            Normal = Vector3.Zero;
            TexCoord = Vector2.Zero;
            Tangent = Vector3.Zero;
            Bitangent = Vector3.Zero;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            Tangent = Vector3.Zero;
            Bitangent = Vector3.Zero;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, Vector3 tangent, Vector3 bitangent)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            Tangent = tangent;
            Bitangent = bitangent;
        }
    }
    public class Mesh
    {
        private List<Vertex> vertices;
        private List<uint> indices;

        public Mesh()
        {
            vertices = new List<Vertex>();
            indices = new List<uint>();
        }

        public void SetVertices(List<Vertex> vertices)
        {
            this.vertices = vertices;
        }

        public void SetIndices(List<uint> indices)
        {
            this.indices = indices;
        }

        public List<Vertex> GetVertices()
        {
            return vertices;
        }

        public List<uint> GetIndices()
        {
            return indices;
        }

        public uint GetVertexCount()
        {
            return (uint)vertices.Count;
        }

        public uint GetIndexCount()
        {
            return (uint)indices.Count;
        }
    }
}
