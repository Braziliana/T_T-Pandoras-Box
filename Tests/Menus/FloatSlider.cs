using System.Runtime.InteropServices;

namespace Tests.Menus;

public unsafe class FloatSlider : MenuItem
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
}