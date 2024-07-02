using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

using Voxel_Engine.DataHandling;

namespace Voxel_Engine
{
    public struct Transform : IComponent
    {
        public Matrix4 TransformMatrix()
        {
            Matrix4 transformMatrix = Matrix4.CreateScale(Scale) *
                                  Matrix4.CreateFromQuaternion(Rotation) *
                                  Matrix4.CreateTranslation(Position);
            return transformMatrix; 
        }

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;


        public Transform(Vector3 position, Quaternion? rotation = null, Vector3? scale = null, Action? onUpdate = null)
        {
            Position = position;
            Rotation = rotation ?? Quaternion.Identity;
            Scale = scale ?? Vector3.One;
        }
        public Transform Lerp(Transform other, double t)
        {
            return
            new(Position + (other.Position - Position) * (float)t,
                Rotation + (other.Rotation - Rotation) * (float)t,
                Scale + (other.Scale - Scale) * (float)t);
        }

        /// <summary>
        /// x,y,tilt
        /// </summary>
        /// <param name="dir"></param>
        public void Rotate(Vector3 dir)
        {
            if (dir.LengthSquared == 0) return;

            float MouseSensitivity = 0.2f;
            Quaternion pitch = Quaternion.FromAxisAngle(
                -Vector3.UnitZ, MathHelper.DegreesToRadians(dir.Y * MouseSensitivity));
            Quaternion yaw = Quaternion.FromAxisAngle(
                -Vector3.UnitY, MathHelper.DegreesToRadians(dir.X * MouseSensitivity));
            
            Rotation = yaw * Rotation * pitch;
            
        }
    }
}
