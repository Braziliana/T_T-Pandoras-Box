using System.Numerics;
using System.Runtime.InteropServices;

namespace Tests;
[StructLayout(LayoutKind.Sequential)]
public struct Color {
    public float r, g, b, a;
}

public enum TextHorizontalOffset
{
    None,
    Left,
    Center,
    Right
}

public enum TextVerticalOffset
{
    None,
    Top,
    Center,
    Bottom
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
    private static extern void RenderSetClearColor(ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererRectFilled2D(ref Vector2 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererRectFilled3D(ref Vector3 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererRectFilledBordered2D(ref Vector2 position, ref Vector2 size, ref Color color, ref Color borderColor, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererRectFilledBordered3D(ref Vector3 position, ref Vector2 size, ref Color color, ref Color borderColor, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererRectBorder2D(ref Vector2 position, ref Vector2 size, ref Color color, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererRectBorder3D(ref Vector3 position, ref Vector2 size, ref Color color, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererDrawCircle2D(ref Vector2 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererDrawCircle3D(ref Vector3 position, ref Vector2 size, ref Color color);

    [DllImport("Native.dll", CharSet = CharSet.Ansi)]
    private static extern void RendererText2D(string text, ref Vector2 position, float size, ref Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);

    [DllImport("Native.dll", CharSet = CharSet.Ansi)]
    private static extern void RendererTextRect2D(string text, ref Vector2 start, ref Vector2 end, float size, ref Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);
    
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
    
    public void RectFilled(Vector2 position, Vector2 size, Color color)
    {
        RendererRectFilled2D(ref position, ref size, ref color);
    }

    public void RectFilled(Vector3 position, Vector2 size, Color color)
    {
        RendererRectFilled3D(ref position, ref size, ref color);
    }

    public void RectFilledBordered(Vector2 position, Vector2 size, Color color, Color borderColor, float borderSize)
    {
        RendererRectFilledBordered2D(ref position, ref size, ref color, ref borderColor, borderSize);
    }

    public void RectFilledBordered(Vector3 position, Vector2 size, Color color, Color borderColor, float borderSize)
    {
        RendererRectFilledBordered3D(ref position, ref size, ref color, ref borderColor, borderSize);
    }

    public void RectBorder(Vector2 position, Vector2 size, Color color, float borderSize)
    {
        RendererRectBorder2D(ref position, ref size, ref color, borderSize);
    }

    public void RectBorder(Vector3 position, Vector2 size, Color color, float borderSize)
    {
        RendererRectBorder3D(ref position, ref size, ref color, borderSize);
    }

    public void DrawCircle(Vector2 position, Vector2 size, Color color)
    {
        RendererDrawCircle2D(ref position, ref size, ref color);
    }

    public void DrawCircle(Vector3 position, Vector2 size, Color color)
    {
        RendererDrawCircle3D(ref position, ref size, ref color);
    }

    public void Text(string text, Vector2 position, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)
    {
        RendererText2D(text, ref position, size, ref color, textHorizontalOffset, textVerticalOffset);
    }

    public void Text(string text, Vector2 start, Vector2 end, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)
    {
        RendererTextRect2D(text, ref start, ref end, size, ref color, textHorizontalOffset, textVerticalOffset);
    }
}