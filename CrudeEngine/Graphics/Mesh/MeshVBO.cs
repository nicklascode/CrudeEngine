using OpenGL;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CrudeEngine.Graphics.Mesh;
using CrudeEngine.Utils;
using System.Drawing;

public class MeshVBO
{
    private uint vbo;
    private uint ibo;
    private uint vaoId;
    private int indexCount;

    public MeshVBO()
    {
        vbo = Gl.GenBuffer();
        ibo = Gl.GenBuffer();
        vaoId = Gl.GenVertexArray();
    }

    public void Allocate(Mesh mesh)
    {
        float[] vertexData = BufferUtil.CreateVertexBuffer(mesh.GetVertices());
        uint[] indexData = mesh.GetIndices().ToArray();
        indexCount = indexData.Length;

        Gl.BindVertexArray(vaoId);

        // Vertex Buffer
        Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(IntPtr)(vertexData.Length * sizeof(float)), vertexData, BufferUsage.StaticDraw);

        // Index Buffer
        Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(IntPtr)(indexData.Length * sizeof(uint)), indexData, BufferUsage.StaticDraw);

        // Vertex Attributes: assuming 3 position, 3 normal, 2 texcoord (8 floats per vertex)
        int stride = 8 * sizeof(float);

        Gl.EnableVertexAttribArray(0); // Position
        Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, stride, IntPtr.Zero);

        Gl.EnableVertexAttribArray(1); // Normal
        Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, stride, (IntPtr)(3 * sizeof(float)));

        Gl.EnableVertexAttribArray(2); // TexCoord
        Gl.VertexAttribPointer(2, 2, VertexAttribType.Float, false, stride, (IntPtr)(6 * sizeof(float)));

        Gl.BindVertexArray(0);

        ErrorCode error = Gl.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error: {error}");
        }
    }

    public void Draw()
    {
        Gl.BindVertexArray(vaoId);
        Gl.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        Gl.BindVertexArray(0);
    }

    public void Delete()
    {
        Gl.DeleteBuffers(1, vbo);
        Gl.DeleteBuffers(1, ibo);
        Gl.DeleteVertexArrays(1, vaoId);
    }

    internal uint GetVaoId()
    {
        return vaoId;
    }

    internal int GetIndexCount()
    {
        return indexCount;
    }
}
