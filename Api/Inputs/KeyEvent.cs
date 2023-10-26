using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Inputs
{
    public struct KeyEvent
    {
        public VirtualKey VirtualKey { get; set; }
        public KeyState KeyState { get; private set; }
        public uint TimeStamp { get; private set; }

        public KeyEvent(VirtualKey virtualKey, KeyState keyState, uint timeStamp)
        {
            VirtualKey = virtualKey;
            KeyState = keyState;
            TimeStamp = timeStamp;
        }
    }
}
