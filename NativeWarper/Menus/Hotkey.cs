using System.Runtime.InteropServices;
using Api.Inputs;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe class Hotkey : MenuItem, IHotkey
{
    [DllImport("Native.dll")]
    private static extern bool* HotkeyGetToggledPointer(IntPtr instance);
    
    [DllImport("Native.dll")]
    private static extern HotkeyType* HotkeyGetHotkeyTypePointer(IntPtr instance);
    [DllImport("Native.dll")]
    private static extern VirtualKey* HotkeyGetHotkeyPointer(IntPtr instance);
    
    private readonly bool* _toggled;
    private readonly HotkeyType* _hotkeyType;
    private readonly VirtualKey* _virtualKey;

    public bool Enabled
    {
        get => *_toggled;
        set => *_toggled = value;
    }
    
    public VirtualKey VirtualKey { get => *_virtualKey; set => *_virtualKey = value; }
    public HotkeyType HotkeyType { get => *_hotkeyType; set => *_hotkeyType = value; }
    
    public Hotkey(IntPtr ptr, string title) : base(ptr, title)
    {
        _toggled = HotkeyGetToggledPointer(ptr);
        _hotkeyType = HotkeyGetHotkeyTypePointer(ptr);
        _virtualKey = HotkeyGetHotkeyPointer(ptr);
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
        settingsProvider.SetValue($"{SaveId}.{nameof(Enabled)}", Enabled);
        settingsProvider.SetValue($"{SaveId}.{nameof(VirtualKey)}", VirtualKey);
        settingsProvider.SetValue($"{SaveId}.{nameof(HotkeyType)}", HotkeyType);
    }

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        if (settingsProvider.ReadValue($"{SaveId}.{nameof(Enabled)}", out bool value))
        {
            Enabled = value;
        }
        if (settingsProvider.ReadValue($"{SaveId}.{nameof(VirtualKey)}", out VirtualKey virtualKey))
        {
            VirtualKey = virtualKey;
        }
        if (settingsProvider.ReadValue($"{SaveId}.{nameof(HotkeyType)}", out HotkeyType hotkeyType))
        {
            HotkeyType = hotkeyType;
        }
    }
}