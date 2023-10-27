using System.Runtime.InteropServices;

namespace Tests;

public class Renderer
{
    #region Imports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RenderCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterRenderCallback(IntPtr windowPtr, RenderCallback callback);
    
    #endregion
    
    private readonly IntPtr _rendererPtr;
    
    public delegate void OnRenderDelegate(float deltaTime);

    public event OnRenderDelegate? OnRender;
    
    public Renderer(IntPtr rendererPtr)
    {
        _rendererPtr = rendererPtr;
        RegisterRenderCallback(_rendererPtr, OnRendererRender);
    }
    
    private void OnRendererRender(float deltaTime)
    {
        OnRender?.Invoke(deltaTime);
    }
}