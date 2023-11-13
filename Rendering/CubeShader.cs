using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voxel_Engine.Utility;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Engine.Rendering
{
    using DataHandling;
    public class CubeRenderer : IRenderable
    {
        static CubeRenderer? instancebacking;
        public static CubeRenderer Instance {
            get {
                instancebacking ??= new();
                return instancebacking;
            }
        }

        public RenderObject GetRenderable() => RenderObject;
        public readonly RenderObject RenderObject;
        public readonly ElementBufferObject EBO;
        public void Render()
        {
            RenderObject.Use();
            GL.DrawElements(PrimitiveType.Triangles,EBO.IBO.DataCount,DrawElementsType.UnsignedInt,0);
        }
        public CubeRenderer(FrameBuffer? output = null, params Texture[] textures) 
        {
            uint[] indices =
            {
                //right hand rule, thumb = normal.
                2, 1, 0, //top
                3, 2, 0,

                4, 5, 6, //bottom
                4, 6, 7,

                0, 1, 4, //back
                4, 1, 5,

                2, 3, 6, //front
                6, 3, 7,

                1, 2, 5, //left 1256
                2, 6, 5,

                0, 4, 3, //right 0347
                3, 4, 7,
            };
            float[] vertices =
            {
                   0.5f,  0.5f,  0.5f, //top clockwise from top left
                  -0.5f,  0.5f,  0.5f,
                  -0.5f,  0.5f, -0.5f,
                   0.5f,  0.5f, -0.5f,

                   0.5f, -0.5f,  0.5f, //bottom
                  -0.5f, -0.5f,  0.5f,
                  -0.5f, -0.5f, -0.5f,
                   0.5f, -0.5f, -0.5f,
            };

            EBO = new(new(),indices,
                new VertexBufferObject(BufferUsageHint.StaticDraw,0,3,vertices));

            RenderObject = new(EBO, textures, new("Base", "Base"), output);
        }
    }
}
