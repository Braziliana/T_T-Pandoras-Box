using System.Runtime.InteropServices;

namespace Tests.Menus;

public unsafe class MenuItem
{
    [DllImport("Native.dll")]
    private static extern IntPtr MenuItemSetTitle(IntPtr instance, string title);
    
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
    
}