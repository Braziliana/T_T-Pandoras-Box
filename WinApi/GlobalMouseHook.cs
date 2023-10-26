using System.ComponentModel;
using System.Diagnostics;

namespace WinApi;

public class GlobalMouseHookEventArgs : HandledEventArgs
{
    public WindowsApi.MouseState MouseState { get; private set; }
    public WindowsApi.LowLevelMouseInputEvent MouseData { get; private set; }

    public GlobalMouseHookEventArgs(
        WindowsApi.LowLevelMouseInputEvent mouseData,
        WindowsApi.MouseState mouseState)
    {
        MouseData = mouseData;
        MouseState = mouseState;
    }
}


public static unsafe class GlobalMouseHook
{
    private static IntPtr _hookId = IntPtr.Zero;
    public static event EventHandler<GlobalMouseHookEventArgs>? MouseEvent;
    private static WindowsApi.MouseHookProc? _hookDelegate;
    
    static GlobalMouseHook()
    {
        Hook();
    }
    
    private static IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, WindowsApi.LowLevelMouseInputEvent* lParam)
    {
        var wParamTyped = wParam.ToInt32();

        var isSimulated = false;
        if ((lParam->Flags & WindowsApi.LLMHF_INJECTED) != 0)
        {
            lParam->Flags &= ~WindowsApi.LLMHF_INJECTED;
            isSimulated = true;
        }

        if ((lParam->Flags & WindowsApi.LOWER_IL_INJECTED) != 0)
        {
            lParam->Flags &= ~WindowsApi.LOWER_IL_INJECTED;
            isSimulated = true;
        }
        
        if (isSimulated || !Enum.IsDefined(typeof(WindowsApi.MouseState), wParamTyped))
            return WindowsApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

        var mouseState = (WindowsApi.MouseState)wParamTyped;
        
        var eventArguments = new GlobalMouseHookEventArgs(*lParam, mouseState);
         
         MouseEvent?.Invoke(null, eventArguments);
            
        return eventArguments.Handled ? (IntPtr)1 : WindowsApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }
    
    public static void Hook()
    {
        Unhook();
            
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        _hookDelegate = LowLevelKeyboardProc;
        _hookId = WindowsApi.SetWindowsHookEx(WindowsApi.HookType.MouseLowLevel, _hookDelegate, WindowsApi.GetModuleHandle(curModule.ModuleName), 0);
    }

    public static void Unhook()
    {
        if (_hookId != IntPtr.Zero)
        {
            WindowsApi.UnhookWindowsHookEx(_hookId);
        }

        _hookId = IntPtr.Zero;
    }
}