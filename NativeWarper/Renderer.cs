using System.Numerics;
using System.Runtime.InteropServices;
using Api;

namespace NativeWarper;

public class Renderer : IRenderer
{
    #region Imports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RenderCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterRenderCallback(IntPtr handler);
    
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
    private static extern void RendererCircleFilled2D(ref Vector2 position, float size, ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererCircleFilled3D(ref Vector3 position, float size, ref Color color);

    [DllImport("Native.dll")]
    private static extern void RendererCircleFilledBordered2D(ref Vector2 position, float size, ref Color color, ref Color borderColor, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererCircleFilledBordered3D(ref Vector3 position, float size, ref Color color, ref Color borderColor, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererCircleBorder2D(ref Vector2 position, float size, ref Color color, float borderSize);

    [DllImport("Native.dll")]
    private static extern void RendererCircleBorder3D(ref Vector3 position, float size, ref Color color, float borderSize);

    [DllImport("Native.dll", CharSet = CharSet.Ansi)]
    private static extern void RendererText2D(string text, ref Vector2 position, float size, ref Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);

    [DllImport("Native.dll", CharSet = CharSet.Ansi)]
    private static extern void RendererTextRect2D(string text, ref Vector2 start, ref Vector2 end, float size, ref Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset);
    
    [DllImport("Native.dll", CharSet = CharSet.Ansi)]
    private static extern void RendererSet3DMatrix(ref Matrix4x4 matrix);
    
    #endregion
 
    public delegate void OnRenderDelegate(float deltaTime);
    
    private readonly OnRenderDelegate _onRenderDelegate;

    public Renderer()
    {
        _onRenderDelegate = new OnRenderDelegate(OnRendererRender);
    }

    public void Init()
    {
        RegisterRenderCallback(Marshal.GetFunctionPointerForDelegate(_onRenderDelegate));
    }
    
    private void OnRendererRender(float deltaTime)
    {
        OnRender?.Invoke(deltaTime);
    }

    public event IRenderer.OnRenderDelegate? OnRender;

    public void RectFilled2D(Vector2 position, Vector2 size, Color color)
    {
        RendererRectFilled2D(ref position, ref size, ref color);
    }

    public void RectFilled3D(Vector3 position, Vector2 size, Color color)
    {
        RendererRectFilled3D(ref position, ref size, ref color);
    }

    public void RectFilledBordered2D(Vector2 position, Vector2 size, Color color, Color borderColor, float borderSize)
    {
        RendererRectFilledBordered2D(ref position, ref size, ref color, ref borderColor, borderSize);
    }

    public void RectFilledBordered3D(Vector3 position, Vector2 size, Color color, Color borderColor, float borderSize)
    {
        RendererRectFilledBordered3D(ref position, ref size, ref color, ref borderColor, borderSize);
    }

    public void RectBorder2D(Vector2 position, Vector2 size, Color color, float borderSize)
    {
        RendererRectBorder2D(ref position, ref size, ref color, borderSize);
    }

    public void RectBorder3D(Vector3 position, Vector2 size, Color color, float borderSize)
    {
        RendererRectBorder3D(ref position, ref size, ref color, borderSize);
    }

    public void CircleFilled2D(Vector2 position, float size, Color color)
    {
        RendererCircleFilled2D(ref position, size, ref color);
    }

    public void CircleFilled3D(Vector3 position, float size, Color color)
    {
        RendererCircleFilled3D(ref position, size, ref color);
    }

    public void CircleFilledBordered2D(Vector2 position, float size, Color color, Color borderColor, float borderSize)
    {
        RendererCircleFilledBordered2D(ref position, size, ref color, ref borderColor, borderSize);
    }

    public void CircleFilledBordered3D(Vector3 position, float size, Color color, Color borderColor, float borderSize)
    {
        RendererCircleFilledBordered3D(ref position, size, ref color, ref borderColor, borderSize);
    }

    public void CircleBorder2D(Vector2 position, float size, Color color, float borderSize)
    {
        RendererCircleBorder2D(ref position, size, ref color, borderSize);
    }

    public void CircleBorder3D(Vector3 position, float size, Color color, float borderSize)
    {
        RendererCircleBorder3D(ref position, size, ref color, borderSize);
    }

    public void Text(string text, Vector2 position, float size, Color color)
    {
        Text(text, position, size, color, TextHorizontalOffset.Center, TextVerticalOffset.Center);
    }

    public void Text(string text, Vector2 start, Vector2 end, float size, Color color)
    {
        Text(text, start, end, size, color, TextHorizontalOffset.Center, TextVerticalOffset.Center);
    }

    public void Text(string text, Vector2 position, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)
    {
        RendererText2D(text, ref position, size, ref color, textHorizontalOffset, textVerticalOffset);
    }

    public void Text(string text, Vector2 start, Vector2 end, float size, Color color, TextHorizontalOffset textHorizontalOffset, TextVerticalOffset textVerticalOffset)
    {
        RendererTextRect2D(text, ref start, ref end, size, ref color, textHorizontalOffset, textVerticalOffset);
    }

    public bool IsOnScreen(Vector2 position)
    {
        return true;
    }

    public void SetProjectionViewMatrix(Matrix4x4 matrix4X4)
    {
        RendererSet3DMatrix(ref matrix4X4);
    }
}