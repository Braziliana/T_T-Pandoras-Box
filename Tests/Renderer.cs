using System.Numerics;
using System.Runtime.InteropServices;

namespace Tests;
[StructLayout(LayoutKind.Sequential)]
public struct Color {
    public float r, g, b, a;
}
public class Renderer
{
    #region Imports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RenderCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterRenderCallback(RenderCallback callback);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterRenderHudCallback(RenderCallback callback);
    
    [DllImport("Native.dll")]
    public static extern void RenderSetClearColor(ref Color color);
    
    [DllImport("Native.dll")]
    public static extern void RendererDrawRect2D(ref Vector2 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    public static extern void RendererDrawRect3D(ref Vector3 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    public static extern void RendererDrawCircle2D(ref Vector2 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    public static extern void RendererDrawCircle3D(ref Vector3 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    public static extern void RendererText(string text, ref Vector2 position, float size, ref Color color);

    [DllImport("Native.dll")]
    public static extern void RendererTextCenter(string text, ref Vector2 position, float size, ref Color color);
    
    #endregion
    
    public delegate void OnRenderDelegate(float deltaTime);

    public event OnRenderDelegate? OnRender;
    
    public Renderer()
    {
        RegisterRenderCallback(OnRendererRender);
    }
    
    private void OnRendererRender(float deltaTime)
    {
        OnRender?.Invoke(deltaTime);
    }
}