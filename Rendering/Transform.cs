using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace Voxel_Engine
{
    public class Transform : IComponent
    {
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        public Matrix4 TransformMatrix;

        public Vector3 Position { get => position; set { position = value; UpdateMatrix(); } }
        public Quaternion Rotation { get => rotation; set { rotation = value; UpdateMatrix(); } }
        public Vector3 Scale { get => scale; set { scale = value; UpdateMatrix(); } }

        private void UpdateMatrix()
        {
            TransformMatrix =   Matrix4.CreateTranslation(position) *
                                Matrix4.CreateFromQuaternion(rotation) *
                                Matrix4.CreateScale(scale);
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3? scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale ?? Vector3.One;
            UpdateMatrix();
        }
    }
}
