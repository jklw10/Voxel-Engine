using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;


namespace Voxel_Engine.Rendering
{
    static class Shape
    {
        public static void UseCube()
        {
            GL.BindVertexArray(CubeShader.VAO);
        }
    }
}
