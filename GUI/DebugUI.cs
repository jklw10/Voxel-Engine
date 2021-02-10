using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;
using Voxel_Engine.Utility;
using System.Collections;
using OpenTK.Windowing.Common;

namespace Voxel_Engine.GUI
{
    public static class DebugUI
    {
        
        const int ammount = 120;
        static float[] frameTimes = new float[ammount];
        static int beginning;
        static float zoom = 10;
        static float highestEver;
        static float AverageEver;
        static bool showGraph = true;
        static bool showNumbers = true;
        static bool pause = false;
        static bool fpsCap = false;
        static bool toggle = false;
        static bool called = false;
        public static void Toggle()
        {
            toggle = !toggle;
            if (toggle)
            {
                UI.ImGuiAction -= () => GraphFrameTimes(ref Time.DeltaTime);
                UI.ImGuiAction += () => GraphFrameTimes(ref Time.DeltaTime);
            }
            else
            {
                UI.ImGuiAction -= () => GraphFrameTimes(ref Time.DeltaTime);
            }
        }
        public static void GraphFrameTimes(ref double timeDelta)
        {
            if (showGraph && !pause)
            {
                frameTimes.ShiftLeft();
                frameTimes[ammount-1] = (((float)timeDelta));
            }
            if (showNumbers)
            {
                if (++beginning >= ammount)
                {
                    beginning = 0;
                    //if no previous average use this one otherwise just get average of this and previous
                    AverageEver = AverageEver == 0 ?
                        frameTimes.Average() :
                        (AverageEver + frameTimes.Average()) / 2;

                }
                //if(!showGraph&& !pause) frameTimes[beginning] = (float)timeDelta;

                if (highestEver < timeDelta)
                {
                    highestEver = (float)timeDelta;
                }
            }
            ImGui.Begin("FrameTimeGraph");
            if (!called) UI.Style();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 1));
            if(ImGui.Button("FPS cap"))
            {
                fpsCap = !fpsCap;
                if (fpsCap)
                {
                    Engine.window.RenderFrequency = 0;
                    Engine.window.VSync = VSyncMode.Off;
                }
                else
                {
                    Engine.window.RenderFrequency = 144;
                    Engine.window.VSync = VSyncMode.Adaptive;
                }
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();

            ImGui.SetWindowSize(new Vector2(800, 300));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            

            showGraph   = ImGui.Button("Show Graph") ? !showGraph : showGraph;
            ImGui.SameLine();
            showNumbers = ImGui.Button("Show Numbers") ? !showNumbers : showNumbers;
            ImGui.SameLine();
            pause       = ImGui.Button("Pause") ? !pause : pause;
            if (showGraph)
            {
                ImGui.PushButtonRepeat(true);
                zoom *= ImGui.Button("Zoom +") ? 1.5f : 1;
                ImGui.SameLine();
                zoom /= ImGui.Button("Zoom -") ? 1.5f : 1;
                ImGui.PushButtonRepeat(false);

                ImGui.PlotHistogram("", ref frameTimes[0], ammount, 0, MathF.Round(zoom, 1) + "ms max", 0.0f, zoom, new Vector2(800, 100)); 
            }
            if (showNumbers)
            {

                ImGui.Text("Last frame in ms:" + timeDelta);
                ImGui.Text("Highest recent in ms:" + frameTimes.Max());
                ImGui.Text("Average recent in ms:" + frameTimes.Average());
                ImGui.Text("Highest ever in ms:" + highestEver);
                ImGui.SameLine();
                highestEver = ImGui.Button("Reset") ? 0 : highestEver;
                ImGui.Text("Average ever in ms:" + AverageEver);
                ImGui.SameLine();
                AverageEver = ImGui.Button("Reset") ? 0 : AverageEver;
            }
            ImGui.End();
            called = true;
        }
    }
}
