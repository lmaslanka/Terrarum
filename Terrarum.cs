namespace Terrarum
{
    using System;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Graphics;
    using OpenTK.Input;
    using OpenTK;
    using System.Drawing;

    public class Terrarum : GameWindow
    {
        private readonly VertexBufferObject vbo;
        private readonly VertexBufferObject vbo2;

        // We modify the vertex array to include four vertices for our rectangle.
        private readonly float[] _vertices =
        {
            -0.2f, -0.8f, 0.0f, // top right
            -0.2f, -0.2f, 0.0f, // bottom right
            -0.8f, -0.2f, 0.0f, // bottom left
            -0.8f, -0.8f, 0.0f, // top left
        };

        private readonly float[] _vertices2 =
        {
            0.8f, 0.8f, 0.0f, // top right
            0.8f, 0.2f, 0.0f, // bottom right
            0.2f, 0.2f, 0.0f, // bottom left
            0.2f, 0.8f, 0.0f, // top left
        };

        // Then, we create a new array: indices.
        // This array controls how the EBO will use those vertices to create triangles
        private readonly uint[] _indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        public Terrarum (int width, int height) : base (width, height, new GraphicsMode(32, 8, 0, 0), "Terrarum v0", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
        {
            VSync = VSyncMode.On;
            TargetRenderFrequency = 60;
            TargetUpdateFrequency = 60;

            this.vbo = new VertexBufferObject();
            this.vbo2 = new VertexBufferObject();
        }

        protected override void OnLoad (EventArgs e)
        {
            this.vbo.OnLoad(_vertices, _indices);
            this.vbo2.OnLoad(_vertices2, _indices);

            base.OnLoad (e);
        }

        protected override void OnUnload (EventArgs e)
        {
            vbo.UnLoad();
            vbo2.UnLoad();

            base.OnUnload (e);
        }

        protected override void OnResize (EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize (e);
        }

        protected override void OnRenderFrame (FrameEventArgs e)
        {
            GL.Clear (ClearBufferMask.ColorBufferBit);

            vbo.Render();
            vbo2.Render();

            SwapBuffers ();
            
            base.OnRenderFrame (e);
        }

        protected override void OnUpdateFrame (FrameEventArgs e)
        {
            HandleKeyboard ();

            base.OnUpdateFrame (e);
        }

        private void HandleKeyboard ()
        {
            var keyState = Keyboard.GetState ();

            if (keyState.IsKeyDown (Key.Escape))
            {
                Exit ();
            }
        }
    }
}
