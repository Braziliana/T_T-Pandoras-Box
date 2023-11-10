using System.Runtime.InteropServices;

namespace Tests.Menus;

public unsafe class Toggle : MenuItem
{
    [DllImport("Native.dll")]
    private static extern IntPtr ToggleGetToggledPointer(IntPtr instance);

    private readonly bool* _toggled;

    public bool Toggled
    {
        get => *_toggled;
        set => *_toggled = value;
    }
    
    public Toggle(IntPtr ptr, string title) : base(ptr, title)
    {
        _toggled = (bool*)ToggleGetToggledPointer(ptr);
    }
}