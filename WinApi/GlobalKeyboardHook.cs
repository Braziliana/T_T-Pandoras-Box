using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinApi
{
    public class GlobalKeyboardHookEventArgs : HandledEventArgs
    {
        public WindowsApi.KeyboardState KeyboardState { get; private set; }
        public WindowsApi.LowLevelKeyboardInputEvent KeyboardData { get; private set; }

        public GlobalKeyboardHookEventArgs(
            WindowsApi.LowLevelKeyboardInputEvent keyboardData,
            WindowsApi.KeyboardState keyboardState)
        {
            KeyboardData = keyboardData;
            KeyboardState = keyboardState;
        }
    }

    public static unsafe class GlobalKeyboardHook
    {
        private static IntPtr _hookId = IntPtr.Zero;
        public static event EventHandler<GlobalKeyboardHookEventArgs>? KeyboardPressed;
        private static WindowsApi.KeyboardHookProc? _hookDelegate;
        
        
        static GlobalKeyboardHook()
        {
            Hook();
        }
        
        private static IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, WindowsApi.LowLevelKeyboardInputEvent* lParam)
        {
            var wParamTyped = wParam.ToInt32();
            var isSimulated = false;
            if ((lParam->Flags & WindowsApi.LLKHF_INJECTED) != 0)
            {
                lParam->Flags &= ~WindowsApi.LLKHF_INJECTED;
                isSimulated = true;
            }

            if ((lParam->Flags & WindowsApi.LOWER_IL_INJECTED) != 0)
            {
                lParam->Flags &= ~WindowsApi.LOWER_IL_INJECTED;
                isSimulated = true;
            }

            if (isSimulated || !Enum.IsDefined(typeof(WindowsApi.KeyboardState), wParamTyped))
                return WindowsApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            
            var eventArguments = new GlobalKeyboardHookEventArgs(*lParam, (WindowsApi.KeyboardState)wParamTyped);

            KeyboardPressed?.Invoke(null, eventArguments);
            
            return eventArguments.Handled ? (IntPtr)1 : WindowsApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        public static void Hook()
        {
            Unhook();
            
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            _hookDelegate = LowLevelKeyboardProc;
            _hookId = WindowsApi.SetWindowsHookEx(WindowsApi.HookType.KeyboardLowLevel, _hookDelegate, WindowsApi.GetModuleHandle(curModule.ModuleName), 0);
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
}
