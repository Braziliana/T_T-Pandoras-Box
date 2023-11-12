using System.Runtime.InteropServices;
using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe class Hotkey : MenuItem, IHotkey
{
    [DllImport("Native.dll")]
    private static extern IntPtr HotkeyGetToggledPointer(IntPtr instance);
    
    public VirtualKey VirtualKey { get; set; }
    public HotkeyType HotkeyType { get; set; }
    
    private readonly bool* _toggled;

    public bool Enabled
    {
        get => *_toggled;
        set => *_toggled = value;
    }
    
    public Hotkey(IntPtr ptr, string title) : base(ptr, title)
    {
        _toggled = (bool*)HotkeyGetToggledPointer(ptr);
    }

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
    }
}