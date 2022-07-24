using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Rendering
{
    public struct FrameBuffer
    {
        public static readonly FrameBuffer Default = new();
        public readonly int handle;
        public readonly Texture[] output;
        readonly List<DrawBuffersEnum> DrawBuffers = new();

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            if (handle != 0) GL.DrawBuffers(DrawBuffers.Count, DrawBuffers.ToArray());
        }
        public void Resize()
        {
            foreach(var tex in output)
                tex.Resize(Engine.Window.Size);
        }
        public FrameBuffer(params Texture[] textures)
        {
            handle = 0;
            output = textures;
            if (textures.Length <= 0)
                return;

            handle = GL.GenFramebuffer();
            CreateBuffer(textures);
        }
        private void CreateBuffer(Texture[] textures)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

            foreach (Texture t in textures)
                AttachTexture(t);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new ApplicationException("Unable to create FrameBuffer:" + status.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // unbind
        }
        static readonly int maxDraw = GL.GetInteger(GetPName.MaxColorAttachments);
        int colorTextures = 0;
        int depthTextures = 0;
        public void AttachTexture(Texture texture)
        {
            if (colorTextures + depthTextures >= maxDraw)
                throw new ApplicationException("can't bind more than " + maxDraw + "attachments");

            switch (texture.Type) {
                case TextureType.Depth:
                    if (depthTextures > 0)
                        throw new ApplicationException("can't bind more than 1 depth attachment");
                    DrawBuffers.Add(DrawBuffersEnum.ColorAttachment0 + colorTextures);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                        FramebufferAttachment.DepthAttachment,
                        TextureTarget.Texture2D, texture.handle, 0);
                    depthTextures++;
                    break;
                case TextureType.Color4:
                case TextureType.Float3:
                case TextureType.Float:
                    DrawBuffers.Add(DrawBuffersEnum.ColorAttachment0 + colorTextures + depthTextures);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                        (FramebufferAttachment.ColorAttachment0 + colorTextures),
                        TextureTarget.Texture2D, texture.handle, 0);
                    colorTextures++;
                    break;
                default: throw new NotImplementedException("texture type of: "+texture.Type+" not implemented");
            }
        }
    }
}
