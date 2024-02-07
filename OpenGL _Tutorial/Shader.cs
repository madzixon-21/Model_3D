using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;
using System.Xml.Linq;


namespace OpenGL__Tutorial
{
    public class Shader
    {
        public int Handle;
        int VertexShader;
        int FragmentShader;

        Matrix4 model;
        Matrix4 view;
        Matrix4 projection;


        Vector3 lightColor;
        Vector3 lightPos;
        Vector3 lightPos2;
        Vector3 viewPos;
        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);

            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);

            int location = GL.GetUniformLocation(Handle, "model");

            GL.UniformMatrix4(location, true, ref model);

            location = GL.GetUniformLocation(Handle, "view");

            GL.UniformMatrix4(location, true, ref view);

            location = GL.GetUniformLocation(Handle, "projection");

            GL.UniformMatrix4(location, true, ref projection);

        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = -1;

            if (name == "model")
            {
                location = GL.GetUniformLocation(Handle, "model");
                if (location != -1)
                    GL.UniformMatrix4(location, true, ref matrix);

            }
            else if (name == "view")
            {
                view = matrix;
            }
            else
            {
                projection = matrix;
            }
        }

        public void SetVector3(string name, Vector3 vector)
        {
            if (name == "lightColor")
            {
                lightColor = vector;
                int location = GL.GetUniformLocation(Handle, "lightColor");
                GL.Uniform3(location, vector);
            }
            else if (name == "lightPos")
            {
                lightPos = vector;
                int location = GL.GetUniformLocation(Handle, "lightPos");
                GL.Uniform3(location, vector);
            }
            else if (name == "lightPos2")
            {
                lightPos2 = vector;
                int location = GL.GetUniformLocation(Handle, "lightPos2");
                GL.Uniform3(location, vector);
            }
            else if (name == "viewPos")
            {
                viewPos = vector;
                int location = GL.GetUniformLocation(Handle, "viewPos");
                GL.Uniform3(location, vector);
            }
            else
            {
                Console.WriteLine($"Unknown uniform name: {name}");
            }
        }
        

        public int GetAttribLocation(string attributeName)
        {
            if (Handle == 0)
            {
                Console.WriteLine("Shader program not compiled or linked.");
                return -1;
            }

            int location = GL.GetAttribLocation(Handle, attributeName);

            if (location == -1)
            {
                Console.WriteLine($"Attribute '{attributeName}' not found in the shader program.");
            }

            return location;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
