using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Engine.Rendering;
public struct Program
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

    public void SetMatrix(string name, ref Matrix4 value)
    {
        GL.ProgramUniformMatrix4(Handle, GL.GetUniformLocation(Handle, name), true, ref value);
    }
    public void SetUniform3(string name, Vector3 value)
    {
        GL.ProgramUniform3(Handle, GL.GetUniformLocation(Handle, name), value);
    }
    public void SetUniform3i(string name, Vector3i value)
    {
        GL.ProgramUniform3(Handle, GL.GetUniformLocation(Handle, name), value.X, value.Y, value.Z);
    }
    public void SetUniform4(string name, Vector4 value)
    {
        GL.ProgramUniform4(Handle, GL.GetUniformLocation(Handle, name), value);
    }
    public void SetUniform2(string name, Vector2 value)
    {
        GL.ProgramUniform2(Handle, GL.GetUniformLocation(Handle, name), value);
    }
    public void SetUniform1(string name, int value)
    {
        GL.ProgramUniform1(Handle, GL.GetUniformLocation(Handle, name), value);
    }
    public void SetUniform1(string name, float value)
    {
        GL.ProgramUniform1(Handle, GL.GetUniformLocation(Handle, name), value);
    }
}
public class ProgramLocationContainer
{

}
public struct ProgramLocation
{
    public readonly string ProjectionMatrix = "ProjMatrix";
    public readonly string CameraPosition = "ViewPos";
    public readonly string CameraRotation = "CamRot";
    public readonly string ViewMatrix = "ViewMatrix";
    public readonly string Resolution = "Resolution";


    public ProgramLocation()
    {

    }
}
