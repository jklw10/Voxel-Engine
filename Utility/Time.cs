using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Engine
{
    public class Time
    {
#pragma warning disable CA2211 //needed to pass as refrence.
        public static double DeltaTime;
#pragma warning restore CA2211 
        static double oldTime = 0;
        public static void Update()
        {
            double newtime = GLFW.GetTime();
            DeltaTime = (newtime - oldTime)*1000;
            oldTime = newtime;
        }

#pragma warning disable CA2211 //needed to pass as refrence.
        public static double PhysicsDeltaTime;
#pragma warning restore CA2211 
        static double physicsOldTime = 0;
        public static void PhysicsUpdate()
        {
            double newtime = GLFW.GetTime();
            PhysicsDeltaTime = (newtime - physicsOldTime) * 1000;
            physicsOldTime = newtime;
        }
    }
}
