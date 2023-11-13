using System.Runtime.InteropServices;

namespace Tests;

public class AppWindow
{
    #region Imports

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr WindowCreate();

    [DllImport("Native.dll")]
    private static extern void WindowDestroy();

    [DllImport("Native.dll")]
    private static extern void WindowRun();

    [DllImport("Native.dll")]
    private static extern void WindowClose();
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AppWindowUpdateCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterAppWindowUpdateCallback(AppWindowUpdateCallback callback);

    #endregion

    private IntPtr _windowPtr = IntPtr.Zero;

    public delegate void OnUpdateDelegate(float deltaTime);

    public event OnUpdateDelegate? OnUpdate;

    public void Create()
    {
        _windowPtr = WindowCreate();
        RegisterAppWindowUpdateCallback(OnWindowUpdate);
    }

    public void Run()
    {
        WindowRun();
    }

    public void Close()
    {
        WindowClose();
    }

    private void OnWindowUpdate(float deltaTime)
    {
        OnUpdate?.Invoke(deltaTime);
    }
}