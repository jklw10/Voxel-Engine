using System;
using OpenTK.Mathematics;
namespace Voxel_Engine
{
    public class Voxel : IComponent
    {
        public Color4 Color;
        public Transform Transform;

        public Voxel(Color4 color, Vector3 pos)
        {
            Color = color;
            Transform = new Transform(pos, new Quaternion(), new Vector3(1, 1, 1));
        }
    }
}
