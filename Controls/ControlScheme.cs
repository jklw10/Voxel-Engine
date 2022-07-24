using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace Voxel_Engine.Controls;
public static class ControlTools
{

    public static readonly float MouseSensitivity = 0.2f;
    /// <summary>
    /// x,y,tilt, lol no there's no tilt.
    /// </summary>
    /// <param name="dir"></param>
    public static Quaternion Rotate(Vector3 dir)
    {
        if (dir.LengthSquared == 0) return Quaternion.Identity;

        Quaternion pitch = Quaternion.FromAxisAngle(
            -Vector3.UnitZ, MathHelper.DegreesToRadians(dir.Y * MouseSensitivity));
        Quaternion yaw = Quaternion.FromAxisAngle(
            -Vector3.UnitY, MathHelper.DegreesToRadians(dir.X * MouseSensitivity));
        
        return yaw * pitch;
    }
    public static Vector2 Axial(Keys[] wasd)
    {
        Vector2 output = new(0);
        if (Input.KeyDown(wasd[0]))
            output += Vector2.UnitX;
        
        if (Input.KeyDown(wasd[1]))
            output += -Vector2.UnitY;
        
        if (Input.KeyDown(wasd[2]))
            output += -Vector2.UnitX;
        
        if (Input.KeyDown(wasd[3]))
            output += Vector2.UnitY;

        if (output != Vector2.Zero)
            output.Normalize();

        return output;
    }
    public static Vector3 Direction(Quaternion direction)
    {
        Vector3 output = new(0);
        if (Input.KeyDown(Keys.W))
        {
            output += direction * Vector3.UnitX;
        }
        if (Input.KeyDown(Keys.A))
        {
            output += direction * -Vector3.UnitZ;
        }
        if (Input.KeyDown(Keys.S))
        {
            output += direction * -Vector3.UnitX;
        }
        if (Input.KeyDown(Keys.D))
        {
            output += direction * Vector3.UnitZ;
        }
        if (Input.KeyDown(Keys.Space))
        {
            output += Vector3.UnitY;
        }
        if (Input.KeyDown(Keys.LeftControl))
        {
            output -= Vector3.UnitY;
        }
        if (output != Vector3.Zero)
        {
            output.Normalize();
        }
        return output;
    }
    public static bool Move(Keys down, Keys up, ref int value)
    {
        if (Input.KeyPress(up))
        {
            value++;
            return true;
        }
        if (Input.KeyPress(down))
        {
            value--;
            return true;
        }
        return false;
    }
    public static double ModifierKey(Keys key, double down, double up) => Input.KeyDown(key) ? down : up;
}

