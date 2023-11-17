using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public class SubMenu : MenuBase, ISubMenu
{
    public SubMenu(IntPtr menuPointer, string title) : base(menuPointer, title)
    {
        
    }   
}