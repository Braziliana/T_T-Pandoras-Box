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
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MenuItemGetId(IntPtr instance);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FreeMenuItemIdBuffer(IntPtr strPtr);
    
    //FreeMenuItemIdBuffer
    
    protected IntPtr Ptr;
    private string _title;
    
    public string Name
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

    public string SaveId
    {
        get
        {
            var strPtr = MenuItemGetId(Ptr);
            var saveId = Marshal.PtrToStringAnsi(strPtr);
            FreeMenuItemIdBuffer(strPtr);
            if (string.IsNullOrWhiteSpace(saveId))
            {
                return _title;
            }

            return saveId.ToLower().Replace(" ", "_");
        }
    }

    public string Description { get; } = string.Empty;

    public virtual void Render()
    {
        MenuItemRender(Ptr);
    }

    public abstract void LoadSettings(ISettingsProvider settingsProvider);
    public abstract void SaveSettings(ISettingsProvider settingsProvider);
}