

using OpenTK.Platform.Windows;
using System.Linq;

namespace Voxel_Engine.Rendering;
public struct FrameBuffer
{
    public static readonly FrameBuffer Default = new();
    public readonly int handle;
    public readonly Texture[]? output;
    readonly DrawBuffersEnum[] DrawBuffers = Array.Empty<DrawBuffersEnum>();

    public readonly void Use()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
        if (handle != 0) GL.DrawBuffers(DrawBuffers.Length, DrawBuffers);
    }
    public readonly void Resize()
    {
        foreach (var tex in output ?? Array.Empty<Texture>())
            tex.Resize(Engine.Window.Size);
    }
    public FrameBuffer(params Texture[] textures)
    {
        handle = 0;
        output = textures ?? Array.Empty<Texture>();
        if (output.Length <= 0)
            return;

        handle = GL.GenFramebuffer();
        DrawBuffers = CreateBuffer(handle,output);
    }
    static readonly int maxDraw = GL.GetInteger(GetPName.MaxColorAttachments);
    static private DrawBuffersEnum[] CreateBuffer(int handle,Texture[] textures)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

        int colorTextures = 0;
        int depthTextures = 0;
        if (textures.Length >= maxDraw)
            throw new ApplicationException("can't bind more than " + maxDraw + "attachments");

        var DrawBuffers = new DrawBuffersEnum[textures.Length];
        int i = 0;
        foreach (Texture texture in textures)
        {
            var current = colorTextures + depthTextures;
            switch (texture.Type)
            {
                case TextureType.Depth:
                    if (depthTextures > 0)
                        throw new ApplicationException("can't bind more than 1 depth attachment");
                    DrawBuffers[i] = (DrawBuffersEnum.ColorAttachment0 + current);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                        FramebufferAttachment.DepthAttachment,
                        TextureTarget.Texture2D, texture.handle, 0);
                    depthTextures++;
                    break;
                case TextureType.Color4:
                case TextureType.Float3:
                case TextureType.Float:
                    DrawBuffers[i] = (DrawBuffersEnum.ColorAttachment0 + current);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                        (FramebufferAttachment.ColorAttachment0 + colorTextures),
                        TextureTarget.Texture2D, texture.handle, 0);
                    colorTextures++;
                    break;
                default: throw new NotImplementedException("texture type of: " + texture.Type + " not implemented");
            }
            i++;
        }
        var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != FramebufferErrorCode.FramebufferComplete)
            throw new ApplicationException("Unable to create FrameBuffer:" + status.ToString());

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // unbind
        return DrawBuffers.ToArray();
    }
}
