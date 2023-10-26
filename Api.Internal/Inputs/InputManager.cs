using Api.Inputs;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinApi;
using static Api.Inputs.IInputManager;

namespace Api.Internal.Inputs
{
    internal class InputManager : IInputManager
    {
        private readonly IDictionary<VirtualKey, KeyState> _states;

        public event EventHandler<InputKeyEventArgs>? InputKeyEvent;
        public event KeyUpDelegate? KeyUp;
        public event KeyDownDelegate? KeyDown;

        private readonly int _width;
        private readonly int _height;
        private Vector2 _mousePosition = Vector2.Zero;

        public InputManager()
        {
            _width =  WindowsApi.GetSystemMetrics(WindowsApi.SystemMetric.VirtualScreenWidth);
            _height = WindowsApi.GetSystemMetrics(WindowsApi.SystemMetric.VirtualScreenHeight);
            _states = new Dictionary<VirtualKey, KeyState>();
            GlobalKeyboardHook.KeyboardPressed += GlobalKeyboardHookKeyboardPressed;
            GlobalMouseHook.MouseEvent += GlobalMouseHookOnMouseEvent;
        }

        public KeyState GetKeyState(int key)
        {
            return GetKeyState((VirtualKey)key);
        }

        public KeyState GetKeyState(VirtualKey virtualKey)
        {
            if(_states.TryGetValue(virtualKey, out var result))
            {
                return result;
            }

            return KeyState.Unknown;
        }

        public void KeyboardSendDown(VirtualKey virtualKey)
        {
            var inputs = new[]
            {
                InternalKey(virtualKey, true),
            };
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void KeyboardSendUp(VirtualKey virtualKey)
        {
            var inputs = new[]
            {
                InternalKey(virtualKey, false)
            };
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void KeyboardSend(VirtualKey virtualKey)
        {
            var inputs = new[]
            {
                InternalKey(virtualKey, true),
                InternalKey(virtualKey, false)
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void MouseSendDown(MouseButton mouseButton)
        {
            var inputs = new[]
            {
                InternalMouse(mouseButton, true),
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void MouseSendUp(MouseButton mouseButton)
        {
            var inputs = new[]
            {
                InternalMouse(mouseButton, false),
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void MouseSend(MouseButton mouseButton)
        {
            var inputs = new[]
            {
                InternalMouse(mouseButton, true),
                InternalMouse(mouseButton, false),
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public void MouseSend(MouseButton mouseButton, Vector2 position)
        {
            var inputs = new[]
            {
                InternalMouse(position, mouseButton, true),
                InternalMouse(mouseButton, false),
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }
        
        public void MouseSetPosition(Vector2 position)
        {
            var inputs = new[]
            {
                InternalMouse(position)
            };
            
            WindowsApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsApi.Input)));
        }

        public Vector2 GetMousePosition()
        {
            return _mousePosition;
        }

        private WindowsApi.MouseEvent MapButton(MouseButton mouseButton, bool down)
        {
            return mouseButton switch
            {
                MouseButton.Left => down ? WindowsApi.MouseEvent.LeftDown : WindowsApi.MouseEvent.LeftUp,
                MouseButton.Right => down ? WindowsApi.MouseEvent.RightDown : WindowsApi.MouseEvent.RightUp,
                MouseButton.Middle => down ? WindowsApi.MouseEvent.MiddleDown : WindowsApi.MouseEvent.MiddleUp,
                _ => throw new ArgumentOutOfRangeException(nameof(mouseButton), mouseButton, null)
            };
        }
        
        private void InternalMouse(Vector2 position, ref WindowsApi.Input input)
        {
            input.u.mi.dwFlags |= (uint)(WindowsApi.MouseEvent.Move | WindowsApi.MouseEvent.Absolute | WindowsApi.MouseEvent.VirtualDesk);
            input.u.mi.dx = (int)position.X * 65535 / _width;
            input.u.mi.dy = (int)position.Y * 65535 / _height;
        }
        
        private void InternalMouse(MouseButton mouseButton, bool down, ref WindowsApi.Input input)
        {
            input.u.mi.dwFlags |= (uint)MapButton(mouseButton, down);
        }

        private WindowsApi.Input GetMouseInput()
        {
            return new WindowsApi.Input
            {
                type = (int)WindowsApi.InputType.Mouse,
                u = new WindowsApi.InputUnion
                {
                    mi = new WindowsApi.MouseInput
                    {
                        dwExtraInfo = WindowsApi.GetMessageExtraInfo()
                    }
                }
            };
        }
        
        private WindowsApi.Input InternalMouse(Vector2 position)
        {
            var input = GetMouseInput();
            InternalMouse(position, ref input);
            return input;
        }

        private WindowsApi.Input InternalMouse(MouseButton mouseButton, bool down)
        {
            var input = GetMouseInput();
            InternalMouse(mouseButton, down, ref input);
            return input;
        }

        private WindowsApi.Input InternalMouse(Vector2 position, MouseButton mouseButton, bool down)
        {
            var input = GetMouseInput();
            InternalMouse(position, ref input);
            InternalMouse(mouseButton, down, ref input);
            return input;
        }
        
        private WindowsApi.Input InternalKey(VirtualKey virtualKey, bool down)
        {
            var flags = (down ? WindowsApi.KeyboardEvent.KeyDown : WindowsApi.KeyboardEvent.KeyUp);
            flags |= WindowsApi.KeyboardEvent.Scancode;
            var input = new WindowsApi.Input
            {
                type = (int) WindowsApi.InputType.Keyboard,
                u = new WindowsApi.InputUnion
                {
                    ki = new WindowsApi.KeyboardInput
                    {
                        scan = WindowsApi.MapVirtualKey((ushort)virtualKey, 0),
                        flags = (uint)flags
                    }
                }
            };
            
            return input;
        }
        
        private void GlobalKeyboardHookKeyboardPressed(object? sender, GlobalKeyboardHookEventArgs e)
        {
            var (isValid, keyEvent) = IsValid(e);
            if (!isValid)
            {
                return;
            }

            _states[keyEvent.VirtualKey] = keyEvent.KeyState;

            var eventArguments = new InputKeyEventArgs(keyEvent);
            InputKeyEvent?.Invoke(this, eventArguments);
            if(eventArguments.BlockInput)
            {
                e.Handled = true;
                return;
            }

            switch (keyEvent.KeyState)
            {
                case KeyState.KeyDown:
                    KeyDown?.Invoke(keyEvent.VirtualKey);
                    break;
                case KeyState.KeyUp:
                    KeyUp?.Invoke(keyEvent.VirtualKey);
                    break;
                case KeyState.Unknown:
                    break;
            }
        }

        private (bool isValid, KeyEvent keyEvent) IsValid(GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState is WindowsApi.KeyboardState.SysKeyDown or WindowsApi.KeyboardState.SysKeyUp)
            {
                return (false, default);
            }

            var keyEvent = new KeyEvent(
                (VirtualKey)e.KeyboardData.VirtualCode,
                MapKeyState(e.KeyboardState),
                e.KeyboardData.TimeStamp
            );

            if (_states.TryGetValue(keyEvent.VirtualKey, out var currentState))
            {
                if (currentState == keyEvent.KeyState)
                {
                    return (false, default);
                }
            }

            return (true, keyEvent);
        }

        private KeyState MapKeyState(WindowsApi.KeyboardState keyboardState)
        {
            return keyboardState switch
            {
                WindowsApi.KeyboardState.KeyDown => KeyState.KeyDown,
                WindowsApi.KeyboardState.KeyUp => KeyState.KeyUp,
                _ => KeyState.Unknown,
            };
        }
        
        private void GlobalMouseHookOnMouseEvent(object? sender, GlobalMouseHookEventArgs e)
        {
            var (isValid, keyEvent) = IsValid(e);
            if (!isValid)
            {
                return;
            }
            
            _states[keyEvent.VirtualKey] = keyEvent.KeyState;
            
            var eventArguments = new InputKeyEventArgs(keyEvent);
            InputKeyEvent?.Invoke(this, eventArguments);
            if(eventArguments.BlockInput)
            {
                e.Handled = true;
                return;
            }
            
            switch (keyEvent.KeyState)
            {
                case KeyState.KeyDown:
                    KeyDown?.Invoke(keyEvent.VirtualKey);
                    break;
                case KeyState.KeyUp:
                    KeyUp?.Invoke(keyEvent.VirtualKey);
                    break;
                case KeyState.Unknown:
                    break;
            }
        }

        private (bool isValid, KeyEvent keyEvent) IsValid(GlobalMouseHookEventArgs e)
        {
            _mousePosition.X = e.MouseData.point.x;
            _mousePosition.Y = e.MouseData.point.y;
            
            var virtualKey = MapVirtualKey(e.MouseState, e.MouseData.MouseData);
            var keyState = MapKeyState(e.MouseState);

            var keyEvent = new KeyEvent(
                virtualKey,
                keyState,
                e.MouseData.TimeStamp
            );

            if (_states.TryGetValue(keyEvent.VirtualKey, out var currentState))
            {
                if (currentState == keyEvent.KeyState)
                {
                    return (false, default);
                }
            }

            return (true, keyEvent);
        }
        
        private KeyState MapKeyState(WindowsApi.MouseState mouseState)
        {
            return mouseState switch
            {
                WindowsApi.MouseState.LeftButtonDown => KeyState.KeyDown,
                WindowsApi.MouseState.RightButtonDown => KeyState.KeyDown,
                WindowsApi.MouseState.MouseWheelDown => KeyState.KeyDown,
                WindowsApi.MouseState.XButtonDown =>  KeyState.KeyDown,
                
                WindowsApi.MouseState.LeftButtonUp => KeyState.KeyUp,
                WindowsApi.MouseState.RightButtonUp => KeyState.KeyUp,
                WindowsApi.MouseState.MouseWheelUp => KeyState.KeyUp,
                WindowsApi.MouseState.XButtonUp => KeyState.KeyUp,
               
                _ => KeyState.Unknown,
            };
        }
        
        
        private VirtualKey MapVirtualKey(WindowsApi.MouseState mouseState, uint mouseData)
        {
            VirtualKey MapXKey(uint md)
            {
                var key = WindowsApi.HIWORD(md);
                Console.WriteLine(key);
                return key switch
                {
                    1 => VirtualKey.MouseX1,
                    2 => VirtualKey.MouseX2,
                    _ => VirtualKey.Unknown
                };
            }
            
            return mouseState switch
            {
                WindowsApi.MouseState.LeftButtonDown => VirtualKey.MouseLeft,
                WindowsApi.MouseState.LeftButtonUp =>  VirtualKey.MouseLeft,
                WindowsApi.MouseState.RightButtonDown => VirtualKey.MouseRight,
                WindowsApi.MouseState.RightButtonUp => VirtualKey.MouseRight,
                WindowsApi.MouseState.MouseWheelDown => VirtualKey.MouseMiddle,
                WindowsApi.MouseState.MouseWheelUp => VirtualKey.MouseMiddle,
                WindowsApi.MouseState.MouseWheel => VirtualKey.MouseMiddle,
                WindowsApi.MouseState.MouseHorizontalWheel => VirtualKey.MouseMiddle,
                WindowsApi.MouseState.XButtonDown => MapXKey(mouseData),
                WindowsApi.MouseState.XButtonUp => MapXKey(mouseData),
                _ => VirtualKey.Unknown,
            };
        }
    }
}
