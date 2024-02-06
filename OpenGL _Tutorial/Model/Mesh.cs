using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGL__Tutorial.Model
{
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 color;
        public Vector3 normal;
    }
    public class Mesh
    {
        private List<Vertex> vertices = new List<Vertex>();
        private List<uint> indices = new List<uint>();

        private int VAO { get;  set; }
        private int VBO { get;  set; }
        private int EBO { get;  set; }
        public Mesh(Assimp.Mesh mesh) 
        {
            for(int idx = 0; idx < mesh.VertexCount; idx++)
            {
                var vertex = new Vertex();
                vertex.position = new Vector3(mesh.Vertices[idx].X, mesh.Vertices[idx].Y, mesh.Vertices[idx].Z);

                if (mesh.HasVertexColors(0))
                {
                    vertex.color = new Vector3(mesh.VertexColorChannels[0][idx].R, mesh.VertexColorChannels[0][idx].G, mesh.VertexColorChannels[0][idx].B);
                }
                else
                {
                    vertex.color = new Vector3(0.0f, 0.0f, 1.0f);
                }

                if (mesh.HasNormals)
                {
                    vertex.normal = new Vector3(mesh.Normals[idx].X, mesh.Normals[idx].Y, mesh.Normals[idx].Z);
                }
                else
                {
                    throw new Exception("Model has no normals!");
                }

                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                foreach (var index in face.Indices)
                    indices.Add((uint)index);
            }

            CreateBufferObjects();
        }

        private void CreateBufferObjects()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(Vertex)) * vertices.Count, vertices.ToArray(), BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Count, indices.ToArray(), BufferUsageHint.StaticDraw);

            // vertex Positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), 0);

            // vertex colors
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "color"));

            // vertex normals
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "normal"));
        }

        public void DrawMesh(Shader shader)
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }
    }
}
