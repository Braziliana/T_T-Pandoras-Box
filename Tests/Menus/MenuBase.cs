using System.Runtime.InteropServices;

namespace Tests.Menus;

public unsafe class MenuBase : MenuItem
{
    protected IntPtr menuPointer;
    
    /*
 __declspec(dllexport) SubMenu* MenuBaseAddSubMenu(MenuBase* instance, const char* title);
    __declspec(dllexport) Toggle* MenuBaseAddToggle(MenuBase* instance, const char* title, bool toggled);
    __declspec(dllexport) FloatSlider* MenuBaseAddFloatSlider(MenuBase* instance, const char* title, float value, float minValue, float maxValue, float step, int precision);
    __declspec(dllexport) Hotkey* MenuBaseAddHotkey(MenuBase* instance, const std::string& title, unsigned int hotkey, HotkeyType hotkeyType, bool toggled);
     */
    
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddSubMenu(IntPtr instance, string title);
    
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddToggle(IntPtr instance, string title, bool toggled);
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddFloatSlider(IntPtr instance, string title, float value, float minValue, float maxValue, float step, int precision);
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddHotkey(IntPtr instance, string title, uint hotkey, HotkeyType hotkeyType, bool toggled);
    
    public MenuBase(IntPtr menuPointer, string title) : base(menuPointer, title)
    {
        this.menuPointer = menuPointer;
    }

    public SubMenu AddSubMenu(string title)
    {
        return new SubMenu(MenuBaseAddSubMenu(menuPointer, title), title);
    }

    public Toggle AddToggle(string title, bool toggled)
    {
        return new Toggle(MenuBaseAddToggle(menuPointer, title, toggled), title);
    }

    public FloatSlider AddFloatSlider(string title, float value, float minValue, float maxValue, float step, int precision)
    {
        return new FloatSlider(MenuBaseAddFloatSlider(menuPointer, title, value, minValue, maxValue, step, precision), title);
    }

    public Hotkey AddHotkey(string title, uint hotkey, HotkeyType hotkeyType, bool toggled)
    {
        return new Hotkey(MenuBaseAddHotkey(menuPointer, title, hotkey, hotkeyType, toggled), title);
    }
}