using System.Runtime.InteropServices;
using Api.Menus;
using Api.Scripts;
using Api.Settings;

namespace NativeWarper.Menus;

public class MainMenu : MenuBase, IMainMenu
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MenuGetInstance();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MenuSetCloseCallback(IntPtr instance, IntPtr handler);

    private readonly ISettingsProvider _settingsProvider;
    
    private delegate void OnCloseDelegate();
    
    private readonly OnCloseDelegate _onCloseDelegate;
    
    public MainMenu(ISettingsProvider settingsProvider) : base(MenuGetInstance(), "T_T Pandoras Box")
    {
        _settingsProvider = settingsProvider;
        _onCloseDelegate = new OnCloseDelegate(SaveSettings);
        MenuSetCloseCallback(menuPointer, Marshal.GetFunctionPointerForDelegate(_onCloseDelegate));
    }

    public void LoadSettings()
    {
        _settingsProvider.Load();
        LoadSettings(_settingsProvider);
    }

    public void SaveSettings()
    {
        SaveSettings(_settingsProvider);
        _settingsProvider.Save();
    }

    public void RemoveMenu(IMenu menu)
    {
    }

    public IMenu CreateMenu(string title, ScriptType scriptType)
    {
        var item = new Menu(MenuBaseAddSubMenu(menuPointer, title), title, scriptType);
        _items.Add(item);
        return item;
    }
}