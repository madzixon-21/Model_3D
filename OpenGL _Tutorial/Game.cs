using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace OpenGL__Tutorial
{
    public class Game : GameWindow
    {
        float[] vertices = {
            // Base
            -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.5f, 0.5f, 0.0f,

            // Apex
            0.0f,  0.5f,  0.0f,  0.5f, 0.0f, 0.5f
        };

        uint[] indices = {
            // Base
            0, 1, 2,
            2, 3, 0,

            // Sides
            0, 4, 1,
            1, 4, 2,
            2, 4, 3,
            3, 4, 0
        };

        int VertexBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;
        private double _time;
        Shader shader;
        //private Camera _camera;

        Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        // Then, we create two matrices to hold our view and projection. They're initialized at the bottom of OnLoad.
        // The view matrix is what you might consider the "camera". It represents the current viewport in the window.
        private Matrix4 _view;

        // This represents how the vertices will be projected. It's hard to explain through comments,
        // so check out the web version for a good demonstration of what this does.
        private Matrix4 _projection;

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            //const float cameraSpeed = 1.5f;
            //const float sensitivity = 0.2f;

            //if (input.IsKeyDown(Keys.W))
            //{
            //    _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            //}

            //if (input.IsKeyDown(Keys.S))
            //{
            //    _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            //}
            //if (input.IsKeyDown(Keys.A))
            //{
            //    _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            //}
            //if (input.IsKeyDown(Keys.D))
            //{
            //    _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            //}
            //if (input.IsKeyDown(Keys.Space))
            //{
            //    _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            //}
            //if (input.IsKeyDown(Keys.LeftShift))
            //{
            //    _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            //}
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            shader.Dispose();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);


            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();


            //_camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);


            // We make the mouse cursor invisible and captured so we can have proper FPS-camera movement.
            //CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _time += 5.0 * e.Time;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            //Code goes here.

            GL.BindVertexArray(VertexArrayObject);

            shader.Use();

            Matrix4 model = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));

            // Then, we pass all of these matrices to the vertex shader.
            // You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects.

            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            // This means that we retain the same multiplication order in both OpenTK c# code and GLSL shader code.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model * view * projection".
            // You can think like this: first apply the modelToWorld (aka model) matrix, then apply the worldToView (aka view) matrix, 
            // and finally apply the viewToProjectedSpace (aka projection) matrix.
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _view);
            shader.SetMatrix4("projection", _projection);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            SwapBuffers();

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            //_camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
