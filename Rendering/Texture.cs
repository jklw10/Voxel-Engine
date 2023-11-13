

namespace Voxel_Engine.Rendering;
public enum TextureType
{
    Color4,
    Float3,
    Float,
    Depth,
}
public struct Texture
{
    public TextureType Type;
    public ProgramLocation Target;
    public int handle;


    public Texture(TextureType type, ProgramLocation? target = null, Vector2i? size = null)
    {
        Type = type;
        Target = target ?? ProgramLocation.Null;
        handle = CreateTexture(type, size ?? Engine.Window.Size);
    }
    public static void BindTexture(int handle, TextureType type, Vector2i size)
    {
        GL.BindTexture(TextureTarget.Texture2D, handle);
        switch (type)
        {
            case TextureType.Color4:
                FormatColor4(size);
                break;
            case TextureType.Float3:
                FormatFloat3(size);
                break;
            case TextureType.Float:
                FormatFloat(size);
                break;
            case TextureType.Depth:
                FormatDepth(size);
                break;
            default:
                throw new ArgumentException("not a valid type of texture");
        }
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
    public void Resize(Vector2i size)
    {
        BindTexture(handle, Type, size);
    }

    public static int CreateTexture(TextureType type, Vector2i size)
    {
        int tex = GL.GenTexture();
        BindTexture(tex, type, size);
        return tex;
    }
    private static void FormatDepth(Vector2i size)
    {

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, size.X, size.Y, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)null);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);

    }
    private static void FormatColor4(Vector2i size)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, size.X, size.Y, 0, PixelFormat.Rgba, PixelType.Int, (IntPtr)null);
        DefaultParameters();
    }
    private static void FormatFloat3(Vector2i size)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, size.X, size.Y, 0, PixelFormat.Rgb, PixelType.Float, (IntPtr)null);
        DefaultParameters();
    }
    private static void FormatFloat(Vector2i size)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, size.X, size.Y, 0, PixelFormat.Red, PixelType.Float, (IntPtr)null);
        DefaultParameters();
    }
    private static void DefaultParameters()
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
    }
}

