namespace Terrarum
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using OpenTK.Graphics.OpenGL;

    public class Vao
    {
        private static void AttachBuffer(float[] bufferData, int buffer, int index)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, bufferData.Length * sizeof(float), bufferData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(index);
        }

        public static Vao CreateVao(float[] vertices, params float[][] attributes)
        {
            if (vertices == null || vertices.Length == 0)
            {
                return null;
            }

            int bufferCount = 1 + attributes.Length;
            int[] buffers = new int[bufferCount];

            GL.GenVertexArrays(1, out int vao);
            GL.BindVertexArray(vao);
            GL.GenBuffers(bufferCount, buffers);

            AttachBuffer(vertices, buffers[0], 0);
            for (int i = 0; i < attributes.Length; i++)
            {
                AttachBuffer(attributes[i], buffers[i + 1], i + 1);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(bufferCount, buffers);

            return new Vao(vao, vertices.Length / 3);
        }

        public static void DeleteVao(Vao vao)
        {
            GL.DeleteVertexArray(vao.handle);
        }

        private readonly int handle;
        private readonly int vertexCount;

        private Vao(int handle, int vertexCount) => (this.handle, this.vertexCount) = (handle, vertexCount);

        public void Render()
        {
            GL.BindVertexArray(handle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        }
    }
}
