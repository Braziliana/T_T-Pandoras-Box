using System.Runtime.InteropServices;
using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe class MenuBase : MenuItem
{
    protected IntPtr menuPointer;
    
    [DllImport("Native.dll")]
    protected static extern IntPtr MenuBaseAddSubMenu(IntPtr instance, string title);
    
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddToggle(IntPtr instance, string title, bool toggled);
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddFloatSlider(IntPtr instance, string title, float value, float minValue, float maxValue, float step, int precision);
    [DllImport("Native.dll")]
    private static extern IntPtr MenuBaseAddHotkey(IntPtr instance, string title, uint hotkey, int hotkeyType, bool toggled);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MenuBaseAddComboBox(IntPtr instance, string title, string[] items, int itemCount, int selectedIndex);

    public MenuBase(IntPtr menuPointer, string title) : base(menuPointer, title)
    {
        this.menuPointer = menuPointer;
    }

    public ISubMenu AddSubMenu(string title)
    {
        return new SubMenu(MenuBaseAddSubMenu(menuPointer, title), title);
    }

    public IToggle AddToggle(string title, bool toggled)
    {
        return new Toggle(MenuBaseAddToggle(menuPointer, title, toggled), title);
    }

    public IValueSlider AddFloatSlider(string title, float value, float minValue, float maxValue, float step, int precision)
    {
        return new FloatSlider(MenuBaseAddFloatSlider(menuPointer, title, value, minValue, maxValue, step, precision), title);
    }

    public IHotkey AddHotkey(string title, VirtualKey hotkey, HotkeyType hotkeyType, bool toggled)
    {
        var hotkeyItem = MenuBaseAddHotkey(menuPointer, title, (ushort)hotkey, (int)hotkeyType, toggled);
        return new Hotkey(hotkeyItem, title);
    }

    public IComboBox AddComboBox(string title, string[] items, int selectedIndex)
    {
        var comboBox = MenuBaseAddComboBox(menuPointer, title, items, items.Length, selectedIndex);
        return new ComboBox(comboBox, title, items, selectedIndex);
    }

    public IEnumComboBox<T> AddEnumComboBox<T>(string title, T selectedItem)  where T : Enum
    {
        var items = (string[])Enum.GetNames(typeof(T));
        var comboBox = MenuBaseAddComboBox(menuPointer, title, items, items.Length, Array.IndexOf(items, selectedItem.ToString()));
        return new EnumComboBox<T>(comboBox, title, selectedItem);
    }
    
    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
    }
}