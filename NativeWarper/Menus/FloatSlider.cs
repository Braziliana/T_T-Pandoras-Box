using System.Runtime.InteropServices;
using Api.Menus;
using Api.Settings;

namespace NativeWarper.Menus;

public unsafe class FloatSlider : MenuItem, IValueSlider
{
    
    [DllImport("Native.dll")]
    private static extern IntPtr FloatSliderGetValuePointer(IntPtr instance);
    [DllImport("Native.dll")]
    private static extern IntPtr FloatSliderGetMinValuePointer(IntPtr instance);
    [DllImport("Native.dll")]
    private static extern IntPtr FloatSliderGetMaxValuePointer(IntPtr instance);
    [DllImport("Native.dll")]
    private static extern IntPtr FloatSliderGetStepValuePointer(IntPtr instance);
    [DllImport("Native.dll")]
    private static extern IntPtr FloatSliderGetPrecisionPointer(IntPtr instance);


    public float Value
    {
        get => *_value;
        set => *_value = value;
    }
    
    
    private float* _value;
    private float* _minValue;
    private float* _maxValue;
    private float* _step;
    private int* _precision;

    public FloatSlider(IntPtr ptr, string title) : base(ptr, title)
    {
        _value = (float*)FloatSliderGetValuePointer(ptr);
        _minValue = (float*)FloatSliderGetMinValuePointer(ptr);
        _maxValue = (float*)FloatSliderGetMaxValuePointer(ptr);
        _step = (float*)FloatSliderGetStepValuePointer(ptr);
        _precision = (int*)FloatSliderGetPrecisionPointer(ptr);
    }

    public override void LoadSettings(ISettingsProvider settingsProvider)
    {
    }

    public override void SaveSettings(ISettingsProvider settingsProvider)
    {
    }
}