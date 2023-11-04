using System.Runtime.InteropServices;

namespace Tests;

public class AppWindow
{
    #region Imports

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr WindowCreate();

    [DllImport("Native.dll")]
    private static extern void WindowDestroy(IntPtr window);

    [DllImport("Native.dll")]
    private static extern void WindowRun(IntPtr window);

    [DllImport("Native.dll")]
    private static extern void WindowClose(IntPtr window);

    [DllImport("Native.dll")]
    private static extern IntPtr WindowGetRenderer(IntPtr appWindow);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AppWindowUpdateCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterAppWindowUpdateCallback(IntPtr windowPtr, AppWindowUpdateCallback callback);

    #endregion

    private IntPtr _windowPtr = IntPtr.Zero;

    public delegate void OnUpdateDelegate(float deltaTime);

    public event OnUpdateDelegate? OnUpdate;

    public void Create()
    {
        _windowPtr = WindowCreate();
        RegisterAppWindowUpdateCallback(_windowPtr, OnWindowUpdate);
    }

    public void Run()
    {
        WindowRun(_windowPtr);
    }

    public void Close()
    {
        WindowClose(_windowPtr);
    }

    public IntPtr GetRenderer()
    {
        return WindowGetRenderer(_windowPtr);
    }

    private void OnWindowUpdate(float deltaTime)
    {
        OnUpdate?.Invoke(deltaTime);
    }
}