using System.Runtime.InteropServices;
using Api.Menus;
using Api.Scripts;

namespace NativeWarper.Menus;

public class MainMenu : MenuBase, IMainMenu
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MenuGetInstance();

    public MainMenu() : base(MenuGetInstance(), "T_T Pandoras Box")
    {
    }

    public event Action? MenuOpen;
    public event Action? MenuClose;

    public void LoadSettings()
    {
    }

    public void SaveSettings()
    {
    }

    public void RemoveMenu(IMenu menu)
    {
    }

    public IMenu CreateMenu(string title, ScriptType scriptType)
    {
        return new Menu(MenuBaseAddSubMenu(menuPointer, title), title, scriptType);
    }
}