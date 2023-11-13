using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public class SubMenu : MenuBase, ISubMenu
{
    public SubMenu(IntPtr menuPointer, string title) : base(menuPointer, title)
    {
        
    }

    public string SaveId { get; }
    public string Name { get; }
    public string Description { get; }

    public void LoadSettings(ISettingsProvider settingsProvider)
    {
    }

    public void SaveSettings(ISettingsProvider settingsProvider)
    {
    }

   
}