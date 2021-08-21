using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Diagnostics;

namespace Voxel_Engine.Rendering
{
    class Loader
    {
        public static int LoadShader(string path, ShaderType type)
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            string Path = 
            System.IO.Path.GetDirectoryName(loc) + "\\Content\\" + path;
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("file not found at " + Path);
            }
            string Text = File.ReadAllText(Path) + "\n";
            
            int ID = GL.CreateShader(type);
            GL.ShaderSource(ID, Text);
            GL.CompileShader(ID);

            Console.WriteLine(GL.GetShaderInfoLog(ID));

            GL.GetShader(ID, ShaderParameter.CompileStatus, out int output);
            if (output == 0)
            {
                throw new Exception($"brok :( {GL.GetShaderInfoLog(ID)}");   
            }
            return ID;
        }
    }
}
