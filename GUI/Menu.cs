using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.GUI
{
    public class Menu
    {
        static readonly Menu[] _defaultMenus =
        {
            new Menu(() => DebugUI.GraphFrameTimes(ref Time.DeltaTime),true,"frame time graph",Default.Debug),

        };
        public static readonly Menu? template = new Menu(() => { });
        public Menu? this[Default a]
        {
            get
            {
                return a switch
                {
                    Default.Debug => _defaultMenus[0],
                    _ => null,
                };
            }
        }
        public static int DefaultToInt(Default a) { return (int)a; }

        [Flags]
        public enum Default : int
        {
            NONE    =0,
            Debug   =1,
            Player  =2,
            Pause   =4,
            Main    =8,
            Loading =16,
        }
        Enum namer;
        public bool NeedsCursor;
        public bool IsActive;
        private Action ImguiAction;
        bool toggle;
        public string Name;
        public void Open()
        {
            if(NeedsCursor) Engine.window.CursorGrabbed = true;

        }
        public void Enable()
        {
            IsActive = true;
        }
        public void Exit()
        {
            if (NeedsCursor) Engine.window.CursorGrabbed = false;
        }
        public void Disable()
        {
            IsActive = false;
        }
        public void Toggle() 
        {
            toggle = !toggle;
            if (toggle)
            {
                Open();
                Enable();
            }
            else
            {
                Exit();
                Disable();
            }
        }
        public Menu(Action menuToShow, bool needsCursor = false, string name = "", Enum? e = null)
        {
            ImguiAction = menuToShow;
            NeedsCursor = needsCursor;
            Name = name;
            namer = e ?? Default.NONE;
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
