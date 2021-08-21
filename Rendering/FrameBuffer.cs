using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Engine.Rendering
{
    public class FrameBuffer
    {
        public int handle;
        public int[] output;
        public FrameBuffer(params Texture[] textures)
        {
            handle = GL.GenFramebuffer();
            CreateBuffer(handle, textures);
            output = textures.Select(x => x.handle).ToArray();
        }
        public FrameBuffer(int handle)
        {
            this.handle = handle;
            output = Array.Empty<int>();
        }
        static readonly int maxDraw = GL.GetInteger(GetPName.MaxDrawBuffers);
        public static void CreateBuffer(int frameBuffer, Texture[] textures)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            {
                int colorTextures = 0;
                int depthTextures = 0;
                foreach (Texture t in textures)
                {
                    if(t.Type == TextureType.Color)
                    {
                        if (colorTextures >= maxDraw)
                            throw new ApplicationException("can't bind more than "+ maxDraw + " color attachments");
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, 
                            (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0+ colorTextures), 
                            TextureTarget.Texture2D, t.handle, 0);
                        colorTextures++;
                    }
                    if (t.Type == TextureType.Depth)
                    {
                        if (depthTextures > 0)
                            throw new ApplicationException("can't bind more than 1 depth attachment");
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                            (FramebufferAttachment)((int)FramebufferAttachment.DepthAttachment),
                            TextureTarget.Texture2D, t.handle, 0);
                        depthTextures++;
                    }
                }

                var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    Console.WriteLine("FrameBuffer Was not complete at Camera.CreateRenderBuffers()");
                    Console.WriteLine("" + status);
                }
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // unbind
        }
    }
}
