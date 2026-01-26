using CrudeEngine.Graphics.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrudeEngine.Utils
{
    public class BufferUtil
    {
        public static float[] CreateVertexBuffer(List<Vertex> vertices)
        {
            float[] buffer = new float[vertices.Count * 8];
            for (int i = 0; i < vertices.Count; i++)
            {
                buffer[i * 8 + 0] = vertices[i].Position.X;
                buffer[i * 8 + 1] = vertices[i].Position.Y;
                buffer[i * 8 + 2] = vertices[i].Position.Z;

                buffer[i * 8 + 3] = vertices[i].Normal.X;
                buffer[i * 8 + 4] = vertices[i].Normal.Y;
                buffer[i * 8 + 5] = vertices[i].Normal.Z;

                buffer[i * 8 + 6] = vertices[i].TexCoord.X;
                buffer[i * 8 + 7] = vertices[i].TexCoord.Y;
            }
            return buffer;
        }
    }
}
