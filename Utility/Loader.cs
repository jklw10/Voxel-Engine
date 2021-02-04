using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Resources;

namespace Voxel_Engine.Utility
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
            //delete BOF and add a newline to the end to avoid the EOF
            string Text = Encoding.UTF8.GetString(obj[3..]) + "\n ";
            
            int ID = GL.CreateShader(type);
            GL.ShaderSource(ID, Text);
            GL.CompileShader(ID);

            Console.WriteLine(GL.GetShaderInfoLog(ID));

            GL.GetShader(ID, ShaderParameter.CompileStatus, out int output);
            if (output == 0)
            {
#if DEBUG
                throw new Exception($"brok :( {GL.GetShaderInfoLog(ID)}");
#endif
            }
            return ID;
        }
        public static int CreateProgram(params int[] Shaders)
        {
            int Program = GL.CreateProgram();
            foreach (var Unit in Shaders)
            {
                GL.AttachShader(Program, Unit);
            }
            GL.LinkProgram(Program);

            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out int Success);
            if (Success == 0)
            {
                string Info = GL.GetProgramInfoLog(Program);
                Console.WriteLine($"GL.LinkProgram had info log :\n{Info}");
            }
            //detach and delete shaders  because they're not used after program creation
            foreach (var Unit in Shaders)
            {
                GL.DetachShader(Program, Unit);
                GL.DeleteShader(Unit);
            }
            return Program;
        }
    }
}
