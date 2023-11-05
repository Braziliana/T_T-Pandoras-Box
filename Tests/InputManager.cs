using System.Runtime.InteropServices;

namespace Tests;

public class InputManager
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
    
    [DllImport("Native.dll")]
    public static extern void InputManagerSetOnMouseMoveEvent(IntPtr handler);

    [DllImport("Native.dll")]
    public static extern void InputManagerSetKeyStateEvent(IntPtr handler);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MouseMoveEventDelegate(MouseMoveEvent evt);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void KeyStateEventDelegate(KeyStateEvent evt);

    private static MouseMoveEventDelegate? _mouseMoveHandler;
    private static KeyStateEventDelegate? _keyStateHandler;

    public static void OnMouseMove(MouseMoveEvent evt)
    {
        Console.WriteLine($"{evt.position.x} {evt.position.y} - {evt.delta.x} {evt.delta.y}");
    }

    public static void OnKeyState(KeyStateEvent evt)
    {
        Console.WriteLine($"{evt.key} {evt.isDown}");
    }

    public static void RegisterEvents()
    {
        _mouseMoveHandler = new MouseMoveEventDelegate(OnMouseMove);
        _keyStateHandler = new KeyStateEventDelegate(OnKeyState);

        InputManagerSetOnMouseMoveEvent(Marshal.GetFunctionPointerForDelegate(_mouseMoveHandler));
        InputManagerSetKeyStateEvent(Marshal.GetFunctionPointerForDelegate(_keyStateHandler));
    }
}