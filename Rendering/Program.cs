using System.Reflection.Metadata;
using static Voxel_Engine.Rendering.IGLType;

namespace Voxel_Engine.Rendering;
public readonly struct Program
{
    public static readonly Program Empty = new();
    public readonly int Handle = -1;
    public Program() { }
    public Program(string vert, string frag)
    {
        Handle = GL.CreateProgram();
        int VS = Loader.LoadShader(vert + ".vert", ShaderType.VertexShader);
        int FS = Loader.LoadShader(frag + ".frag", ShaderType.FragmentShader);
        CreateProgram(VS, FS);
    }
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    void CreateProgram(params int[] Shaders)
    {
        foreach (var Unit in Shaders)
        {
            GL.AttachShader(Handle, Unit);
        }
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int Success);
        if (Success == 0)
        {
            string Info = GL.GetProgramInfoLog(Handle);
            Console.WriteLine($"GL.LinkProgram had info log :\n{Info}");
        }
        //detach and delete shaders  because they're not used after program creation
        foreach (var Unit in Shaders)
        {
            GL.DetachShader(Handle, Unit);
            GL.DeleteShader(Unit);
        }

    }

    public void SetUniform(Uniform assignment)
    {
        int loc = ProgramLocationContainer.GetLocation(this, assignment);
        switch (assignment.value)
        {
            case Int1(var val):     GL.ProgramUniform1(Handle, loc, val); break;
            case Int2(var val):     GL.ProgramUniform2(Handle, loc, val); break;
            case Int3(var val):     GL.ProgramUniform3(Handle, loc, val); break;
            case Int4(var val):     GL.ProgramUniform4(Handle, loc, val); break;
            case Float1(var val):   GL.ProgramUniform1(Handle, loc, val); break;
            case Float2(var val):   GL.ProgramUniform2(Handle, loc, val); break;
            case Float3(var val):   GL.ProgramUniform3(Handle, loc, val); break;
            case Float4(var val):   GL.ProgramUniform4(Handle, loc, val); break;
            case Float4x4(var val): GL.ProgramUniformMatrix4(Handle, loc, false, ref val); break;
        };
    }
}
public interface IGLType
{
    public  record Int1(int Value) : IGLType;
    public  record Int2(Vector2i Value) : IGLType;
    public  record Int3(Vector3i Value) : IGLType;
    public  record Int4(Vector4i Value) : IGLType;

    public  record Float1(float Value) : IGLType;
    public  record Float2(Vector2 Value) : IGLType;
    public  record Float3(Vector3 Value) : IGLType;
    public  record Float4(Vector4 Value) : IGLType;

    public  record Float4x4(Matrix4 Value) : IGLType;
}
public static class ProgramLocationContainer
{
    static Dictionary<(Program, Uniform), int> storedLocations = new();
    public static int GetLocation(this Program p, Uniform loc)
    {
        if (storedLocations.TryGetValue((p,loc), out int val))
        {
            return val;
        }
        int i_loc = GL.GetUniformLocation(p.Handle, loc.location);
        if (i_loc >= 0)
        {
            storedLocations.Add((p, loc), i_loc);
        }

        return i_loc;
    }
}

public struct Uniform
{
    public string location;
    public IGLType value;
    public static Uniform ProjectionMatrix(Float4x4 value) => new("ProjMatrix", value);
    public static Uniform ViewMatrix(Float4x4 value) => new ("ViewMatrix", value);
    public static Uniform CameraPosition(Float3 value) =>  new ("ViewPos", value); 
    public static Uniform CameraRotation(Float4 value) => new("CamRot", value);
    public static Uniform Resolution(Float2 value) => new("Resolution", value);
    //public static Uniform TextureTarget(string location, Int1 value) => new(location, value);

    public Uniform(string loc, IGLType value)
    {
        this.location = loc;
        this.value = value;
    }
    
}

