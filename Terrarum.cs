namespace Terrarum
{
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using System;

    public class Terrarum : GameWindow
    {
        Shader shader;

        public Terrarum(int width, int height)
            : base(width, height, GraphicsMode.Default, "Terrarum v0", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
        {
            VSync = VSyncMode.On;
            TargetRenderFrequency = 60;
            TargetUpdateFrequency = 60;
        }

        protected override void OnLoad(EventArgs e)
        {
            shader = new Shader(@"Shaders\vertex.glsl", @"Shaders\fragment.glsl");

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Triangles);

            GL.Vertex2(1f, 0.0f);
            GL.Vertex2(1.0f, 1.0f);
            GL.Vertex2(0.5f, 0.0f);

            GL.End();
            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();

            base.OnUpdateFrame(e);
        }

        private void HandleKeyboard()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
            {
                Exit();
            }
        }
    }
}
