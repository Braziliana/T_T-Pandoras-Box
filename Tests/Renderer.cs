using System.Runtime.InteropServices;

namespace Tests;

public class Renderer
{
    #region Imports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RenderCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterRenderCallback(RenderCallback callback);
    
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