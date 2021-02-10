using System;
using OpenTK.Mathematics;

using Voxel_Engine.DataHandling;

namespace Voxel_Engine.Rendering
{
    public class Voxel : IComponent
    {
        public Color4 Color;
        public Transform Transform;
        public bool visible;
        public Voxel(Color4 color, Vector3 pos)
        {
            Color = color;
            Transform = new Transform(pos, Quaternion.Identity, new Vector3(1, 1, 1));
        }
    }
}
