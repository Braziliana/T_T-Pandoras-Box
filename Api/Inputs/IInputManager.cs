using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Api.Inputs
{
    public interface IInputManager
    {
        delegate void KeyUpDelegate(VirtualKey virtualKey);
        delegate void KeyDownDelegate(VirtualKey virtualKey);
        event KeyUpDelegate? KeyUp;
        event KeyDownDelegate? KeyDown;
        KeyState GetKeyState(VirtualKey virtualKey);

        void KeyboardSendDown(VirtualKey virtualKey);
        void KeyboardSendUp(VirtualKey virtualKey);
        void KeyboardSend(VirtualKey virtualKey);
        void MouseSendDown(MouseButton mouseButton);
        void MouseSendUp(MouseButton mouseButton);
        void MouseSend(MouseButton mouseButton);
        void MouseSend(MouseButton mouseButton, Vector2 position);
        void MouseSetPosition(Vector2 position);
        Vector2 GetMousePosition();
        void BlockMouseInput(bool blockMouseInput);
    }
}
