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

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MenuBaseRemoveItem(IntPtr instance, IntPtr menuItem);

    protected readonly List<IMenuElement> _items;
    
    public MenuBase(IntPtr menuPointer, string title) : base(menuPointer, title)
    {
        this.menuPointer = menuPointer;
        _items = new List<IMenuElement>();
    }

    public void RemoveItem(IMenuElement menuItem)
    {
        _items.Remove(menuItem);
        MenuBaseRemoveItem(Ptr, menuItem.GetPtr());
    }

    public ISubMenu AddSubMenu(string title)
    {
        var item = new SubMenu(MenuBaseAddSubMenu(menuPointer, title), title);
        _items.Add(item);
        return item;
    }

    public IToggle AddToggle(string title, bool toggled)
    {
        var item = new Toggle(MenuBaseAddToggle(menuPointer, title, toggled), title);
        _items.Add(item);
        return item;
    }

    public IValueSlider AddFloatSlider(string title, float value, float minValue, float maxValue, float step, int precision)
    {
        var item = new FloatSlider(MenuBaseAddFloatSlider(menuPointer, title, value, minValue, maxValue, step, precision), title);
        _items.Add(item);
        return item;
    }

    public IHotkey AddHotkey(string title, VirtualKey hotkey, HotkeyType hotkeyType, bool toggled)
    {
        var hotkeyItem = MenuBaseAddHotkey(menuPointer, title, (ushort)hotkey, (int)hotkeyType, toggled);
        var item = new Hotkey(hotkeyItem, title);
        _items.Add(item);
        return item;
    }

    public IComboBox AddComboBox(string title, string[] items, int selectedIndex)
    {
        var comboBox = MenuBaseAddComboBox(menuPointer, title, items, items.Length, selectedIndex);
        var item = new ComboBox(comboBox, title, items, selectedIndex);
        _items.Add(item);
        return item;
    }

    public IEnumComboBox<T> AddEnumComboBox<T>(string title, T selectedItem)  where T : Enum
    {
        var items = (string[])Enum.GetNames(typeof(T));
        var comboBox = MenuBaseAddComboBox(menuPointer, title, items, items.Length, Array.IndexOf(items, selectedItem.ToString()));
        var item = new EnumComboBox<T>(comboBox, title, selectedItem);
        _items.Add(item);
        return item;
    }
    
    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        foreach (var item in _items)
        {
            item.LoadSettings(settingsProvider);
        }
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
        foreach (var item in _items)
        {
            item.SaveSettings(settingsProvider);
        }
    }
}