using System.Runtime.InteropServices;

namespace Tests.Menus;

public class Menu : MenuBase
{
    [DllImport("Native.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr MenuGetInstance();

    public Menu() : base(MenuGetInstance(), "T_T Pandoras Box")
    {
    }
}