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
        private readonly Mesh vbo;

        // We modify the vertex array to include four vertices for our rectangle.
        private readonly float[] _vertices =
        {
            -0.2f, -0.8f, 0.0f, 1.0f, 1.0f, // top right
            -0.2f, -0.2f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.8f, -0.2f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.8f, -0.8f, 0.0f, 0.0f, 1.0f  // top left
        };

        // Then, we create a new array: indices.
        // This array controls how the EBO will use those vertices to create triangles
        private readonly uint[] _indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        // We create a double to hold how long has passed since the program was opened.
        private double _time;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        public Terrarum (int width, int height) : base (width, height, new GraphicsMode(32, 8, 0, 0), "Terrarum v0", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
        {
            VSync = VSyncMode.On;
            TargetRenderFrequency = 60;
            TargetUpdateFrequency = 60;

            this.vbo = new Mesh();
        }

        protected override void OnLoad (EventArgs e)
        {
            this.vbo.OnLoad(_vertices, _indices);

            // We initialize the camera so that it is 3 units back from where the rectangle is
            // and give it the proper aspect ratio
            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);

            // We make the mouse cursor invisible so we can have proper FPS-camera movement
            CursorVisible = true;

            base.OnLoad (e);
        }

        protected override void OnUnload (EventArgs e)
        {
            vbo.UnLoad();

            base.OnUnload (e);
        }

        protected override void OnResize (EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize (e);
        }

        protected override void OnRenderFrame (FrameEventArgs e)
        {
            _time += 4.0 * e.Time;
            GL.Clear (ClearBufferMask.ColorBufferBit);

            // Finally, we have the model matrix. This determines the position of the model.
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));

            vbo.Render(_time, model, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());

            SwapBuffers ();
            
            base.OnRenderFrame (e);
        }

        protected override void OnUpdateFrame (FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            HandleKeyboard ();

            base.OnUpdateFrame (e);
        }

        // This function's main purpose is to set the mouse position back to the center of the window
        // every time the mouse has moved. So the cursor doesn't end up at the edge of the window where it cannot move
        // further out
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        // In the mouse wheel function we manage all the zooming of the camera
        // this is simply done by changing the FOV of the camera
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
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
