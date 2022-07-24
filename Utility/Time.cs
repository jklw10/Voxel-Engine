using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Engine
{
    public static class Time
    {
        public static double Now { get => GLFW.GetTime(); }

        public static double DeltaTime { get; private set; }
        static double oldTime = 0;
        public static void Update()
        {
            double newtime = Now;
            DeltaTime = (newtime - oldTime)*1000;
            oldTime = newtime;
        }
        public static double PhysicsDeltaTime { get; private set; }
        static double physicsOldTime = 0;
        public static void PhysicsUpdate()
        {
            double newtime = Now;
            PhysicsDeltaTime = (newtime - physicsOldTime) * 1000;
            physicsOldTime = newtime;
        }
    }
}
