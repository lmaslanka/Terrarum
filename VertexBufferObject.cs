namespace Terrarum
{
    using System;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Graphics;
    using OpenTK.Input;
    using OpenTK;
    using System.Drawing;

    public class VertexBufferObject
    {
        Shader shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private float[] _vertices;
        private uint[] _indices;

        public VertexBufferObject() {}

        public void OnLoad(float[] vertices, uint[] indices)
        {
            this._vertices = vertices;
            this._indices = indices;
            
            if (OperatingSystem.IsWindows())
            {
                shader = new Shader (@"Shaders\vertex.glsl", @"Shaders\fragment.glsl");
            } 
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
            {
                shader = new Shader (@"Shaders/vertex.glsl", @"Shaders/fragment.glsl");
            }

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            // We create/bind the EBO the same way as the VBO, just with a different BufferTarget.
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            // We also buffer data to the EBO the same way.
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            shader.Use();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            // We bind the EBO here too, just like with the VBO in the previous tutorial.
            // Now, the EBO will be bound when we bind the VAO.
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            // The EBO has now been properly setup. Go to the Render function to see how we draw our rectangle now!

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void UnLoad()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(shader.Handle);
        }

        public void Render()
        {
            shader.Use();

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}