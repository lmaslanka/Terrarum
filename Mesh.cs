namespace Terrarum
{
    using System;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Graphics;
    using OpenTK.Input;
    using OpenTK;
    using System.Drawing;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Advanced;
    using System.Collections.Generic;

    public class Mesh
    {
        float[] texCoords = {
            0.0f, 0.0f,  // lower-left corner  
            1.0f, 0.0f,  // lower-right corner
            0.5f, 1.0f   // top-center corner
        };
        Shader shader;
        private Texture _texture;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private float[] _vertices;
        private uint[] _indices;

        public Mesh() {}

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
            
            _texture = new Texture(@"./Textures/grass_texture.png");
            _texture.Use();

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
            var vertexLocation = shader.GetAttribLocation("vPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

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

        public void Render(double _time, Matrix4 model, Matrix4 _view, Matrix4 _projection)
        {
            shader.Use();
            _texture.Use();

            // Then, we pass all of these matrices to the vertex shader.
            // You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects

            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model, view, projection",
            // but if you do it here and then pass it to the vertex, you have to do it in order "projection, view, model".
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _view);
            shader.SetMatrix4("projection", _projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}