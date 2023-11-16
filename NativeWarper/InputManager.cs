using System.Numerics;
using System.Runtime.InteropServices;
using Api.Inputs;

namespace NativeWarper;

[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}
[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    public ushort vk;
    public ushort scan;
    public uint flags;
    public uint time;
    public IntPtr extraInfo;
}
        
[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput
{
    public uint msg;
    public ushort paramL;
    public ushort paramH;
}
        
[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    [FieldOffset(0)] public MouseInput mi;
    [FieldOffset(0)] public KeyboardInput ki;
    [FieldOffset(0)] public HardwareInput hi;
}  
[StructLayout(LayoutKind.Sequential)]
public struct Input
{
    public uint type;
    public InputUnion u;
}

public class InputManager : IInputManager, IDisposable
{
    #region NATIVE
    
    [StructLayout(LayoutKind.Sequential)]
    private struct MouseMoveEvent
    {
        public Vector2 position;
        public Vector2 delta;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyStateEvent
    {
        public ushort key;
        [MarshalAs(UnmanagedType.I1)]
        public bool isDown;
    }
    
    private delegate void MouseMoveEventDelegate(MouseMoveEvent evt);
    private delegate void KeyStateEventDelegate(KeyStateEvent evt);
    
    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int InputManagerAddMouseMoveHandler(IntPtr handler);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerRemoveMouseMoveHandler(int key);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int InputManagerAddKeyStateEventHandler(IntPtr handler);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerRemoveKeyStateEventHandler(int key);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerReset();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool InputManagerGetKeyState(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerGetMousePosition(ref Vector2 position);

    
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern Input InputManagerCreateMouseClickInput(ushort vkCode, bool down);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern Input InputManagerCreateMouseMoveInput(ref Vector2 position);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerMouseSendDown(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerMouseSendUp(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerMouseMove(ref Vector2 position);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerMouseSend(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern Input InputManagerCreateKeyboardInput(ushort vkCode, bool down);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerKeyboardSendDown(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerKeyboardSendUp(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerKeyboardSend(ushort vkCode);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerSendInputs(Input[] inputs, uint count);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void InputManagerSetBlockUserMouseInput(bool blockInput);
    
    #endregion
    
    private readonly MouseMoveEventDelegate _mouseMoveEventDelegate;
    private readonly KeyStateEventDelegate _keyStateEventDelegate;
    private int _mouseMoveHandlerId = -1;
    private int _keyStateHandlerId = -1;
    
    public event IInputManager.KeyUpDelegate? KeyUp;
    public event IInputManager.KeyDownDelegate? KeyDown;
    
    public InputManager()
    {
        _mouseMoveEventDelegate = new MouseMoveEventDelegate(OnMouseMoveHandler);
        _keyStateEventDelegate = new KeyStateEventDelegate(OnKeyStateHandler);
        
        RegisterEvents();
    }

    public void RegisterEvents()
    {
        _mouseMoveHandlerId = InputManagerAddMouseMoveHandler(Marshal.GetFunctionPointerForDelegate(_mouseMoveEventDelegate));
        _keyStateHandlerId = InputManagerAddKeyStateEventHandler(Marshal.GetFunctionPointerForDelegate(_keyStateEventDelegate));
    }

    private void OnMouseMoveHandler(MouseMoveEvent evt)
    {
        //MouseMoveHandler?.Invoke(evt);
    }

    private void OnKeyStateHandler(KeyStateEvent evt)
    {
        //KeyStateHandler?.Invoke(evt);
        if (evt.isDown)
        {
            KeyDown?.Invoke((VirtualKey)evt.key);
        }
        else
        {
            KeyUp?.Invoke((VirtualKey)evt.key);
        }
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

    public KeyState GetKeyState(VirtualKey virtualKey)
    {
        return InputManagerGetKeyState((ushort)virtualKey) ? KeyState.KeyDown : KeyState.KeyUp;
    }

    public void KeyboardSendDown(VirtualKey virtualKey)
    {
        InputManagerKeyboardSendDown((ushort)virtualKey);
    }

    public void KeyboardSendUp(VirtualKey virtualKey)
    {
        InputManagerKeyboardSendUp((ushort)virtualKey);
    }

    public void KeyboardSend(VirtualKey virtualKey)
    {
        InputManagerKeyboardSend((ushort)virtualKey);
    }

    public void MouseSendDown(MouseButton mouseButton)
    {
        InputManagerMouseSendDown((ushort)mouseButton);
    }

    public void MouseSendUp(MouseButton mouseButton)
    {
        InputManagerMouseSendUp((ushort)mouseButton);
    }

    public void MouseSend(MouseButton mouseButton)
    {
        InputManagerMouseSend((ushort)mouseButton);
    }

    public void MouseSend(MouseButton mouseButton, Vector2 position)
    {
        MouseSetPosition(position);
        MouseSend(mouseButton);
    }

    public void MouseSetPosition(Vector2 position)
    {
        InputManagerMouseMove(ref position);
    }

    public Vector2 GetMousePosition()
    {
        var mousePos = Vector2.Zero;
        InputManagerGetMousePosition(ref mousePos);
        return mousePos;
    }

    public void BlockMouseInput(bool blockMouseInput)
    {
        InputManagerSetBlockUserMouseInput(blockMouseInput);
    }
}