using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Voxel_Engine
{
    public class Time
    {
        public static double DeltaTime = 0;

        static double oldTime = 0;
        public static void Update()
        {
            double newtime = GLFW.GetTime();
            DeltaTime = (newtime - oldTime)*1000;
            oldTime = newtime;
        }
    }
}
