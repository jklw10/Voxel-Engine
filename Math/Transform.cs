using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

using Voxel_Engine.DataHandling;

namespace Voxel_Engine
{
    public class Transform : IComponent
    {
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        private Matrix4 transformMatrix;
        public Matrix4 TransformMatrix 
        { 
            get 
            {
                if (!useMatrix)
                {
                    useMatrix = true;
                    Update();
                }
                return transformMatrix; 
            }
        }
        private bool useMatrix;
        public bool Dirty = false;

        public Vector3 Position { get => position; set { position = value; Update(); } }
        public Quaternion Rotation { get => rotation; set { rotation = value; Update(); } }
        public Vector3 Scale { get => scale; set { scale = value; Update(); } }

        private void Update()
        {
            if (useMatrix)
            {
                transformMatrix = Matrix4.CreateScale(scale) *
                                  Matrix4.CreateFromQuaternion(rotation) *
                                  Matrix4.CreateTranslation(position);
            }
            Dirty = true;
        }

        public Transform(Vector3 position, Quaternion? rotation = null, Vector3? scale = null)
        {
            this.position = position;
            this.rotation = rotation ?? Quaternion.Identity;
            this.scale = scale ?? Vector3.One;
            useMatrix = true;
            Update();
            useMatrix = false;
        }
    }
}
