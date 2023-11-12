using System.Runtime.InteropServices;

namespace NativeWarper;

public class AppWindow : IDisposable
{
    #region Imports

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr WindowCreate();

    [DllImport("Native.dll")]
    private static extern void WindowDestroy();

    [DllImport("Native.dll")]
    private static extern void WindowRun();
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AppWindowUpdateCallback(float deltaTime);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterAppWindowUpdateCallback(IntPtr handler);

    #endregion

    private IntPtr _windowPtr = IntPtr.Zero;

    public delegate void OnUpdateDelegate(float deltaTime);

    public event OnUpdateDelegate? OnUpdate;

    
    private readonly OnUpdateDelegate _onUpdateDelegate;

    public AppWindow()
    {
        _onUpdateDelegate = new OnUpdateDelegate(OnWindowUpdate);
    }

    public void Create()
    {
        _windowPtr = WindowCreate();
        RegisterAppWindowUpdateCallback(Marshal.GetFunctionPointerForDelegate(_onUpdateDelegate));
    }

    public void Run()
    {
        WindowRun();
    }

    private void OnWindowUpdate(float deltaTime)
    {
        OnUpdate?.Invoke(deltaTime);
    }

    private void ReleaseUnmanagedResources()
    {
        WindowDestroy();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~AppWindow()
    {
        ReleaseUnmanagedResources();
    }
}