using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace OpenGL__Tutorial
{
    public class Game : GameWindow
    {
        #region Variables initialization
        int VertexArrayObject;
        private double _time;
        Shader shader;
        Shader lampShader;
        private Camera camera;
        Model.GeneralModel generalModel;
        Model.GeneralModel queenGeneralModel;
        Model.GeneralModel rookGeneralModel;
        Model.GeneralModel kingGeneralModel;

        private bool firstMove = true;
        private Vector2 lastPos;
        private bool followQueenMode = false;
        private bool staticCameraMode = false;
        private bool keyPad1Pressed = false;
        private bool keyPad2Pressed = false;
        private bool fogEnabled = false;

        Matrix4 queenModel = Matrix4.CreateTranslation(-10.0f, 3.0f, -10.0f) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(90)) * Matrix4.CreateScale(0.1f);
        Vector3 queenPosition = new Vector3(-10.0f, 3.0f, -10.0f);
        float queenRotationY = 0.0f;

        Vector3 lightPos = new Vector3(1.0f, 3.0f, 1.0f);
        Vector3 lightPos2 = new Vector3(-6.0f, 2.0f, -2.0f);
        #endregion
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
                    camera.Position = new Vector3(-3.0f, 8.0f, -3.0f);
                    camera.Pitch = -90.0f;
                    camera.Yaw = -90.0f;
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

            //if (input.IsKeyDown(Keys.KeyPad2) && !keyPad2Pressed)
            //{
            //    followQueenMode = !followQueenMode;
            //    keyPad2Pressed = true;

            //}
            //else if (input.IsKeyReleased(Keys.KeyPad2))
            //{
            //    keyPad2Pressed = false;
            //}


            //if (followQueenMode)
            //{
            //    camera.Position = queenPosition + new Vector3(9.0f, -2.0f, 10.0f);
            //}
            //else
            //{
            //    camera.Position = new Vector3(-3.0f, 2.0f, 6.0f);
            //}

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


            if (input.IsKeyDown(Keys.F) && !fogEnabled)
            {
                // Enable fog
                EnableFog();
                fogEnabled = true;
            }
            else if (input.IsKeyDown(Keys.F) && fogEnabled)
            {
                // Disable fog
                DisableFog();
                fogEnabled = false;
            }
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
            rookGeneralModel = new Model.GeneralModel("resources/SM_PieceBlackRook.obj");

            kingGeneralModel = new Model.GeneralModel("resources/SM_PieceWhiteKing.obj");

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
            shader.SetVector3("lightPos2", lightPos2);
            shader.SetVector3("viewPos", camera.Position);

            generalModel.DrawModel(shader);
            #endregion

            #region Queen piece
            
            shader.SetMatrix4("model", queenModel);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            queenGeneralModel.DrawModel(shader);
            #endregion

            #region Rook piece
            Matrix4 rookModel = Matrix4.CreateTranslation(-40.0f, 4.0f, -25.0f) * Matrix4.CreateScale(0.1f);

            shader.SetMatrix4("model", rookModel);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            rookGeneralModel.DrawModel(shader);
            #endregion

            #region King piece
            Matrix4 kingModel = Matrix4.CreateTranslation(-30.0f, 4.0f, -20.0f) * Matrix4.CreateScale(0.1f);

            shader.SetMatrix4("model", kingModel);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            kingGeneralModel.DrawModel(shader);
            #endregion

            Lamp lamp1 = new Lamp(lampShader, lightPos, camera);
            lamp1.Draw();
            Lamp lamp2 = new Lamp(lampShader, lightPos2, camera);
            lamp2.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            camera.AspectRatio = Size.X / (float)Size.Y;
        }

        private void EnableFog()
        {
            GL.Enable(EnableCap.Fog);
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear); // Choose fog mode
            GL.Fog(FogParameter.FogColor, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            GL.Fog(FogParameter.FogDensity, 0.5f);
            GL.Fog(FogParameter.FogStart, 1.0f); // Set fog start distance
            GL.Fog(FogParameter.FogEnd, -50.0f); // Set fog end distance
        }

        // Method to disable fog
        private void DisableFog()
        {
            GL.Disable(EnableCap.Fog);
        }
    }
}
