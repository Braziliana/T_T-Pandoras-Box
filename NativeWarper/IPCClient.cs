using System.Numerics;
using System.Runtime.InteropServices;

namespace NativeWarper;

public class IPCClient
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr IPCClient_GetInstance();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_Connect();

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void IPCClient_Disconnect();

    // Marshal string argument as LPStr (null-terminated string of single-byte characters).
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_PrintChat([MarshalAs(UnmanagedType.LPStr)] string message);
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_CastSpell(int spellSlot);
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_CastSpellSelf(int spellSlot);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_CastSpellScreenPosition(int spellSlot, ref Vector2 screenPosition);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_CastSpellWorldPosition(int spellSlot, ref Vector3 screenPosition);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_CastSpellTarget(int spellSlot, IntPtr target);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_MoveTo(ref Vector2 screenPosition);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_MoveToWorldPosition(ref Vector3 screenPosition);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool IPCClient_Attack(IntPtr target);

    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool IPCClient_IsConnected();
    
    public bool Connect() => IPCClient_Connect();

    public void Disconnect() => IPCClient_Disconnect();

    public bool PrintChat(string message) => IPCClient_PrintChat(message);

    public bool CastSpell(int spellSlot) => IPCClient_CastSpell(spellSlot);
    public bool CastSpellSelf(int spellSlot) => IPCClient_CastSpellSelf(spellSlot);

    public bool CastSpell(int spellSlot, Vector2 screenPosition) => IPCClient_CastSpellScreenPosition(spellSlot, ref screenPosition);

    public bool CastSpell(int spellSlot, Vector3 worldPosition) => IPCClient_CastSpellWorldPosition(spellSlot, ref worldPosition);

    public bool CastSpell(int spellSlot, IntPtr target) => IPCClient_CastSpellTarget(spellSlot, target);

    public bool MoveTo(Vector2 screenPosition) => IPCClient_MoveTo(ref screenPosition);
    public bool MoveTo(Vector3 worldPosition) => IPCClient_MoveToWorldPosition(ref worldPosition);
    
    public bool Attack(IntPtr target) => IPCClient_Attack(target);

    public bool IsConnected() => IPCClient_IsConnected();
}