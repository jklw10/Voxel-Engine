using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.GUI
{
    public class UITools
    {
        public static bool ToggleButton(string name,ref bool toggle)
        {
            bool pop = toggle;
            if (toggle) ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.7f, 0f, 1));

            if (ImGui.Button(name)) toggle = !toggle;

            if (pop) ImGui.PopStyleColor();

            return toggle;
        }
    }
}