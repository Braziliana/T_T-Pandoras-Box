using System.Runtime.InteropServices;

namespace Tests;

public class InputManager : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseMoveEvent
    {
        public Vector2 position;
        public Vector2 delta;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStateEvent
    {
        public uint key;
        public bool isDown;
    }
    
    public delegate void MouseMoveEventDelegate(MouseMoveEvent evt);
    public delegate void KeyStateEventDelegate(KeyStateEvent evt);
    
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int InputManagerAddMouseMoveHandler(IntPtr handler);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InputManagerRemoveMouseMoveHandler(int key);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int InputManagerAddKeyStateEventHandler(IntPtr handler);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InputManagerRemoveKeyStateEventHandler(int key);
    
    public event MouseMoveEventDelegate? MouseMoveHandler;
    public event KeyStateEventDelegate? KeyStateHandler;

    private MouseMoveEventDelegate _mouseMoveEventDelegate;
    private KeyStateEventDelegate _keyStateEventDelegate;
    private int _mouseMoveHandlerId = -1;
    private int _keyStateHandlerId = -1;

    public InputManager()
    {
        RegisterEvents();
    }

    public void RegisterEvents()
    {
        _mouseMoveEventDelegate = new MouseMoveEventDelegate(OnMouseMoveHandler);
        _keyStateEventDelegate = new KeyStateEventDelegate(OnKeyStateHandler);

        _mouseMoveHandlerId = InputManagerAddMouseMoveHandler(Marshal.GetFunctionPointerForDelegate(_mouseMoveEventDelegate));
        _keyStateHandlerId = InputManagerAddKeyStateEventHandler(Marshal.GetFunctionPointerForDelegate(_keyStateEventDelegate));
    }

    private void OnMouseMoveHandler(MouseMoveEvent evt)
    {
        MouseMoveHandler?.Invoke(evt);
    }

    private void OnKeyStateHandler(KeyStateEvent evt)
    {
        KeyStateHandler?.Invoke(evt);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_mouseMoveHandlerId > -1)
        {
            InputManagerRemoveMouseMoveHandler(_mouseMoveHandlerId);
            _mouseMoveHandlerId = -1;
        }
        if (_keyStateHandlerId > -1)
        {
            InputManagerRemoveKeyStateEventHandler(_keyStateHandlerId);
            _keyStateHandlerId = -1;
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~InputManager()
    {
        ReleaseUnmanagedResources();
    }
}