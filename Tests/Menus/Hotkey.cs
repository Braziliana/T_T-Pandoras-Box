using System.Runtime.InteropServices;

namespace Tests.Menus;

public enum HotkeyType
{
    Press,
    Toggle,
}

public unsafe class Hotkey : MenuItem
{
    [DllImport("Native.dll")]
    private static extern IntPtr HotkeyGetToggledPointer(IntPtr instance);

    private readonly bool* _toggled;

    public bool Toggled
    {
        get => *_toggled;
        set => *_toggled = value;
    }
    
    public Hotkey(IntPtr ptr, string title) : base(ptr, title)
    {
        _toggled = (bool*)HotkeyGetToggledPointer(ptr);
    }
}