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
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterAppWindowUpdateCallback(IntPtr handler);
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void RegisterAppWindowExitCallback(IntPtr handler);

    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool WindowIsRunning();
    
    #endregion

    private IntPtr _windowPtr = IntPtr.Zero;

    public delegate void OnUpdateDelegate(float deltaTime);
    public event OnUpdateDelegate? OnUpdate;
    
    private readonly OnUpdateDelegate _onUpdateDelegate;

    public delegate void OnExitDelegate();
    public event OnExitDelegate? OnExit;
    
    private readonly OnExitDelegate _onExitDelegate;
    
    public AppWindow()
    {
        _onUpdateDelegate = new OnUpdateDelegate(OnWindowUpdate);
        _onExitDelegate = new OnExitDelegate(OnWindowExit);
    }

    private void OnWindowExit()
    {
        Console.WriteLine("Exit app");
        OnExit?.Invoke();
    }

    public void Create()
    {
        _windowPtr = WindowCreate();
        RegisterAppWindowUpdateCallback(Marshal.GetFunctionPointerForDelegate(_onUpdateDelegate));
        RegisterAppWindowExitCallback(Marshal.GetFunctionPointerForDelegate(_onExitDelegate));
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