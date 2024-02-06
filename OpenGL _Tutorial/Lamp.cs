
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Reflection;

namespace OpenGL__Tutorial
{
    public class Lamp
    {
        private Shader shader;
        Vector3 lampPos;
        Camera camera;
        float[] cubeVertices =
            {
                // Position
                -0.5f, -0.5f, -0.5f, // Front face
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,

                -0.5f, -0.5f,  0.5f, // Back face
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
                -0.5f, -0.5f,  0.5f,

                -0.5f,  0.5f,  0.5f, // Left face
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,

                 0.5f,  0.5f,  0.5f, // Right face
                 0.5f,  0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,

                -0.5f, -0.5f, -0.5f, // Bottom face
                 0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                -0.5f, -0.5f,  0.5f,
                -0.5f, -0.5f, -0.5f,

                -0.5f,  0.5f, -0.5f, // Top face
                 0.5f,  0.5f, -0.5f,
                 0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f, -0.5f
            };

        public Lamp(Shader shader, Vector3 lampPos, Camera camera)
        {
            this.shader = shader;
            this.lampPos = lampPos;
            this.camera = camera;
        }

        public void Draw()
        {

            int vboLamp = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboLamp);
            GL.BufferData(BufferTarget.ArrayBuffer, cubeVertices.Length * sizeof(float), cubeVertices, BufferUsageHint.StaticDraw);

            int vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(vaoLamp);

            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindVertexArray(vaoLamp);
            shader.Use();

            Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
            lampMatrix = lampMatrix * Matrix4.CreateTranslation(lampPos);

            shader.SetMatrix4("model", lampMatrix);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
    
}
