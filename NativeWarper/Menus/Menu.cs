using Api.Menus;
using Api.Scripts;

namespace NativeWarper.Menus;

public class Menu : SubMenu, IMenu
{
    public ScriptType ScriptType { get; }
    public Menu(IntPtr menuPointer, string title, ScriptType scriptType) : base(menuPointer, title)
    {
        ScriptType = scriptType;
    }
}