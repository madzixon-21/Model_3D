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
        int VertexArrayObject;
        private double _time;
        Shader shader;
        Shader lampShader;
        private Camera camera;
        Model.GeneralModel generalModel;
        Model.GeneralModel queenGeneralModel;

        private bool firstMove = true;
        private Vector2 lastPos;
        private bool staticCameraMode = false;
        private bool keyPad1Pressed = false;
        Matrix4 queenModel = Matrix4.CreateTranslation(-10.0f, 3.0f, -10.0f) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(90)) * Matrix4.CreateScale(0.1f);

        Vector3 queenPosition = new Vector3(-10.0f, 3.0f, -10.0f);
        float queenRotationY = 0.0f;

        Vector3 lightPos = new Vector3(1.0f, 3.0f, 1.0f);
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            #region Camera change logic
            if (input.IsKeyDown(Keys.KeyPad1) && !keyPad1Pressed)
            {
                staticCameraMode = !staticCameraMode;
                keyPad1Pressed = true;

                if (staticCameraMode)
                {
                    camera.Position = new Vector3(-3.0f, 7.0f, -3.0f);
                    camera.Pitch = -90.0f;
                    camera.Yaw = 0.0f;
                    CursorState = CursorState.Normal;
                }
                else
                {
                    camera.Position = new Vector3(-3.0f, 2.0f, 6.0f);
                    camera.Pitch = 0.0f;
                    camera.Yaw = -MathHelper.PiOver2 - 90.0f;
                    CursorState = CursorState.Normal;
                }
            }
            else if (input.IsKeyReleased(Keys.KeyPad1))
            {
                keyPad1Pressed = false;
            }
            #endregion

            if (!staticCameraMode)
            {
                #region Camera movement logic
                const float cameraSpeed = 1.5f;

                if (input.IsKeyDown(Keys.W))
                {
                    camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
                }

                if (input.IsKeyDown(Keys.S))
                {
                    camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Keys.A))
                {
                    camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Keys.D))
                {
                    camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
                }

                var mouse = MouseState;
                const float sensitivity = 0.2f;

                if (firstMove)
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y);

                    camera.Yaw += deltaX * sensitivity;
                    camera.Pitch -= deltaY * sensitivity;
                }
                #endregion
            }


            #region Queen piece movement and rotation
            const float queenSpeed = 3.0f;
            const float queenRotationSpeed = 2.0f;

            if (input.IsKeyDown(Keys.Up))
            {
                queenPosition += new Vector3(0.0f, 0.0f, -queenSpeed * (float)e.Time); // Move forward
            }
            if (input.IsKeyDown(Keys.Down))
            {
                queenPosition += new Vector3(0.0f, 0.0f, +queenSpeed * (float)e.Time); // Move backward
            }
            if (input.IsKeyDown(Keys.Left))
            {
                queenPosition += new Vector3(-queenSpeed * (float)e.Time, 0.0f, 0.0f); // Move left
            }
            if (input.IsKeyDown(Keys.Right))
            {
                queenPosition += new Vector3(queenSpeed * (float)e.Time, 0.0f, 0.0f); // Move right
            }
            if (input.IsKeyDown(Keys.R))
            {
                queenRotationY += queenRotationSpeed * (float)e.Time; // Rotate right
            }
            if (input.IsKeyDown(Keys.L))
            {
                queenRotationY -= queenRotationSpeed * (float)e.Time; // Rotate left
            }

            
            queenModel = Matrix4.CreateTranslation(queenPosition) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(queenRotationY)) * Matrix4.CreateScale(0.1f);
            
            #endregion

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

            generalModel = new Model.GeneralModel("resources/SM_ChessBoard.obj");
            queenGeneralModel = new Model.GeneralModel("resources/SM_PieceBlackQueen.obj");

            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            lampShader = new Shader("shader.vert", "lampShader.frag");

            camera = new Camera(new Vector3(-3.0f, 2.0f, 6.0f), Size.X / (float)Size.Y);
;
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _time += 5.0 * e.Time;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.BindVertexArray(VertexArrayObject);

            shader.Use();

            #region Chessboard
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f)) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(180)) * Matrix4.CreateScale(0.1f);
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetVector3("lightPos", lightPos);

            generalModel.DrawModel(shader);
            #endregion

            #region Queen piece
            
            shader.SetMatrix4("model", queenModel);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            queenGeneralModel.DrawModel(shader);
            #endregion

            Lamp lamp1 = new Lamp(lampShader, lightPos, camera);
            lamp1.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
