using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Engine.Rendering
{
    public enum TextureType
    {
        Color,
        Depth,
    }
    public class Texture
    {
        public TextureType Type;
        public string Target;
        public int handle;


        public Texture(TextureType type)
        {
            Type = type;
            Target = "";
            handle = CreateTexture(type, Engine.window.Size);
        }
        public Texture(TextureType type, string target)
        {
            Type = type;
            Target = target;
            handle = CreateTexture(type, Engine.window.Size);
        }
        public Texture(TextureType type, string target, Vector2i size)
        {
            Type = type;
            Target = target;
            handle = CreateTexture(type, size);
        }
        public Texture(TextureType type, Vector2i size)
        {
            Type = type;
            Target = "";
            handle = CreateTexture(type, size);
        }

        public static int CreateTexture(TextureType type, Vector2i size)
        {
            int tex = GL.GenTexture();
            switch (type)
            {
                case TextureType.Color:
                    BindColor(tex, size);
                    break;
                case TextureType.Depth:
                    BindDepth(tex, size);
                    break;
                default:
                    break;
            }
            return tex;
        }
        public static void BindDepth(int textureBuffer, Vector2i size)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, size.X, size.Y, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);
            GL.BindTexture(TextureTarget.Texture2D, 0);//unbind
        }
        public static void BindColor(int textureBuffer, Vector2i size)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Srgb, size.X, size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);//unbind
        }
    }
}
