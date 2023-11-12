using System.Runtime.InteropServices;
using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe abstract class MenuItem : IMenuElement
{
    [DllImport("Native.dll")]
    private static extern IntPtr MenuItemSetTitle(IntPtr instance, string title);
    
    [DllImport("Native.dll")]
    private static extern IntPtr MenuItemRender(IntPtr instance);
    
    protected IntPtr Ptr;
    private string _title;
    
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            MenuItemSetTitle(Ptr, _title);
        }
    }

    public MenuItem(IntPtr ptr, string title)
    {
        Ptr = ptr;
        _title = title;
    }

    public string SaveId { get; }
    public string Name { get; }
    public string Description { get; }

    public virtual void Render()
    {
        MenuItemRender(Ptr);
    }

    public abstract void LoadSettings(ISettingsProvider settingsProvider);
    public abstract void SaveSettings(ISettingsProvider settingsProvider);
}