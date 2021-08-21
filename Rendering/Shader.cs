using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Rendering
{
    class Shader
    {
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
