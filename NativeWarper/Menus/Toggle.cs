using System.Runtime.InteropServices;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe class Toggle : MenuItem, IToggle
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

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
        
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
    }
}