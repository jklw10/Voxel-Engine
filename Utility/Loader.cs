using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Resources;

namespace Voxel_Engine
{
    class Shaders
    {
        public static int LoadFile(string path, ShaderType type)
        {
            string Path = "Shaders/" + path;
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("file not found at Shaders/" + path);
            }
            string Text = File.ReadAllText(Path) + "\n";

            int ID = GL.CreateShader(type);
            GL.ShaderSource(ID, Text);
            GL.CompileShader(ID);

            Console.WriteLine(GL.GetShaderInfoLog(ID));

            GL.GetShader(ID, ShaderParameter.CompileStatus, out int output);
            if (output == 0)
            {
#if DEBUG
                throw new Exception($"brok :( {GL.GetShaderInfoLog(ID)}");
#else
                
#endif
            }
            return ID;
        }
        public static int LoadResource(string path, ShaderType type)
        {
            if (!(Properties.Resources.ResourceManager.GetObject(path) is byte[] obj))
            {
                throw new ApplicationException("reading from resources failed. path:" + path);
            }
            string Text = Encoding.UTF8.GetString(obj) + "\n ";
            

            int ID = GL.CreateShader(type);
            GL.ShaderSource(ID, Text);
            GL.CompileShader(ID);

            Console.WriteLine(GL.GetShaderInfoLog(ID));

            GL.GetShader(ID, ShaderParameter.CompileStatus, out int output);
            if (output == 0)
            {
#if DEBUG
                throw new Exception($"brok :( {GL.GetShaderInfoLog(ID)}");
#else
                
#endif
            }
            return ID;
        }
        public static int CreateVisuals(params int[] Shaders)
        {
            int ID = GL.CreateProgram();
            foreach (var Unit in Shaders)
            {
                GL.AttachShader(ID, Unit);
            }
            GL.LinkProgram(ID);

            return ID;
        }
    }
}
