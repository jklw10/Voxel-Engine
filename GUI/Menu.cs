using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.GUI
{
    public static class Mcontrol
    {
        public static void Open([Menu] this  Enum toUse)
        {
            Menu.GetMenu(toUse)?.Open();
        }
        public static void Close(this Enum toUse)
        {
            Menu.GetMenu(toUse)?.Close();
        }
        public static void Toggle(this Enum toUse)
        {
            Menu.GetMenu(toUse)?.Toggle();
        }
    }
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Parameter, Inherited = false, AllowMultiple = true)]
    public sealed class MenuAttribute : Attribute
    {
        public MenuAttribute()
        {
            //just marks it as menu nothing special here.
        }
    }
    public class Menu
    {
        [Flags]
        [Menu]
        public enum Default : int
        {
            NONE    = 0 ,
            Debug   = 1 << 0,
            Player  = 1 << 1,
            Pause   = 1 << 2,
            Main    = 1 << 3,
            Loading = 1 << 4,
        }
        public static Menu? GetMenu(Enum namer)
        {
            return _accessible.Find(x => x.namer.HasFlag(namer));
        }
        public static void MakeDefaults()
        {
            _ = new Menu(() => DebugUI.GraphFrameTimes(), true, "frame time graph", Default.Debug);

        }
        static readonly List<Menu> _accessible = new();

        
        readonly Enum namer;
        readonly Action ImguiAction;
        public readonly bool NeedsCursor;
        
        public bool IsActive;
        public string Name;
        public void Open()
        {
            IsActive = true;
            UI.ImGuiAction -= ImguiAction;
            UI.ImGuiAction += ImguiAction;
            if(NeedsCursor) UI.Detach(); //needs cursor. make it available
        }
        public void Close()
        {
            IsActive = false;
            UI.ImGuiAction -= ImguiAction;
            if (NeedsCursor) UI.Attach();
        }
        public void Toggle() 
        {
            if (IsActive)
            {
                Close();
                return;
            }
            Open();
        }
        public Menu(Action menuToShow, bool needsCursor = false, string name = "", Enum? e = null, bool accessible = true)
        {
            ImguiAction = menuToShow;
            NeedsCursor = needsCursor;
            Name = name;
            namer = e ?? Default.NONE;
            if(accessible) _accessible.Add(this);
        }
        public static Menu? operator +(Menu? a, Menu b)
        {
            if (a is null && b is null) return null;
            return new Menu(a?.ImguiAction + b.ImguiAction,  (a?.NeedsCursor ?? false)  || b.NeedsCursor, a?.Name ?? b.Name);
        }
        public static Menu operator -(Menu a, Menu b)
        {
            var action = a.ImguiAction - b.ImguiAction;
            if(action is null)
            {
                throw new InvalidOperationException(a +" + " +b+ " resulted in a Menu without anu ImguiAction.");
            }
            return new Menu(action, a.NeedsCursor || b.NeedsCursor);
        }
    }
}
