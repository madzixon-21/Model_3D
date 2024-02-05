using System;
using System.Collections.Generic;
using Assimp;
using Assimp.Configs;
using System.Drawing;
using System.Drawing.Imaging;

namespace OpenGL__Tutorial.Model
{
    public class GeneralModel
    {
        public List<Mesh> meshes = new List<Mesh>();
        public GeneralModel(string path) 
        {
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f)); // Set normal smoothing angle

            try
            {
                Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            
                if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) != 0 || scene.RootNode == null)
                {
                    Console.WriteLine("ERROR::ASSIMP");
                    return;
                }
                // Mesh processing
                foreach (var mesh in scene.Meshes)
                {
                    meshes.Add(new Mesh(mesh));
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Problem with loading your model!");
            }
        }

        public void DrawModel(Shader shader)
        {
            foreach( var mesh in meshes)
            {
                mesh.DrawMesh(shader);
            }
        }
    }
}
