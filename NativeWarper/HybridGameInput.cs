using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Api.Game.GameInputs;
using Api.Game.Objects;

namespace NativeWarper;

public class HybridGameInput : IGameInput
{
    [StructLayout(LayoutKind.Sequential)]
    private  struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    /// <summary>
    /// Retrieves the cursor's position, in screen coordinates.
    /// </summary>
    /// <see>See MSDN documentation for further information.</see>
    [DllImport("user32.dll")]
    private  static extern bool GetCursorPos(out POINT lpPoint);

    private static Vector2 GetCursorPosition()
    {
        POINT lpPoint;
        GetCursorPos(out lpPoint);
        // NOTE: If you need error handling
        // bool success = GetCursorPos(out lpPoint);
        // if (!success)
        
        return new Vector2(lpPoint.X, lpPoint.Y);
    }
    
    private readonly IPCClient _ipcClient;
    private bool connected;

    public HybridGameInput()
    {
        _ipcClient = new IPCClient();
        connected = false;
    }
    
    public Vector2 MousePosition { get; private set; }
    public bool IssueOrder(Vector2 position, IssueOrderType issueOrderType)
    {
        return _ipcClient.MoveTo(position);
    }

    public bool IssueOrder(Vector3 position, IssueOrderType issueOrderType)
    {
        return _ipcClient.MoveTo(position);
    }

    public bool Attack(IGameObject target)
    {
        return _ipcClient.Attack(target.Pointer);
    }

    public void CastEmote(int emote)
    {
        
    }

    public void Update(float deltaTime)
    {
        MousePosition = GetCursorPosition();
        if (!_ipcClient.IsConnected())
        {
            connected = _ipcClient.Connect();
            Console.WriteLine("Try to connect");
            if (connected)
            {
                Console.WriteLine("Connected to hybrid.");
                _ipcClient.PrintChat("<font color=#FC6A03>Welcome to T_T Pandora's box hybrid</font>");
            }
        }
    }

    public bool LevelUpSpell(SpellSlot spellSlot)
    {
        return false;
    }

    public bool CastSpell(SpellSlot spellSlot)
    {
        return _ipcClient.CastSpell((int)spellSlot);
    }

    public bool SelfCastSpell(SpellSlot spellSlot)
    {
        return _ipcClient.CastSpellSelf((int)spellSlot);
    }

    public bool CastSpell(SpellSlot spellSlot, Vector2 position)
    {
        return _ipcClient.CastSpell((int)spellSlot, position);
    }

    public bool CastSpell(SpellSlot spellSlot, Vector3 position)
    {
        return _ipcClient.CastSpell((int)spellSlot, position);
    }

    public bool CastSpell(SpellSlot spellSlot, IGameObject target)
    {
        return _ipcClient.CastSpell((int)spellSlot, target.Pointer);
    }
}