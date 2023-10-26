using Api.Inputs;
using Api.Menus;
using Api.Scripts;

namespace Scripts;

public class ScriptingState : IScriptingState
{
    public VirtualKey ComboKey { get => _comboKey.VirtualKey; set => _comboKey.VirtualKey = value; }
    public VirtualKey HarasKey { get => _harasKey.VirtualKey; set => _harasKey.VirtualKey = value; }
    public VirtualKey FarmKey { get => _farmKey.VirtualKey; set => _farmKey.VirtualKey = value; }
    public VirtualKey ClearKey { get => _clearKey.VirtualKey; set => _clearKey.VirtualKey = value; }
    public ActionType ActionType { get; private set; } = ActionType.None;
    
    public bool IsCombo => (ActionType & ActionType.Combo) == ActionType.Combo;
    public bool IsHaras => (ActionType & ActionType.Haras) == ActionType.Haras;
    public bool IsFarm => (ActionType & ActionType.Farm) == ActionType.Farm;
    public bool IsClear => (ActionType & ActionType.Clear) == ActionType.Clear;

    private IHotkey _comboKey;
    private IHotkey _harasKey;
    private IHotkey _farmKey;
    private IHotkey _clearKey;
    
    public ScriptingState(IInputManager inputManager, IMainMenu mainMenu)
    {
        inputManager.KeyDown += InputManagerOnKeyDown;
        inputManager.KeyUp += InputManagerOnKeyUp;

        var menu = mainMenu.CreateMenu("Hotkeys", ScriptType.Utility);
        _comboKey = menu.AddHotkey("Combo", string.Empty, VirtualKey.Spacebar, HotkeyType.Press);
        _harasKey = menu.AddHotkey("Haras", string.Empty, VirtualKey.C, HotkeyType.Press);
        _farmKey = menu.AddHotkey("Farm", string.Empty, VirtualKey.X, HotkeyType.Press);
        _clearKey = menu.AddHotkey("Clear", string.Empty, VirtualKey.V, HotkeyType.Press);
    }

    private void InputManagerOnKeyDown(VirtualKey virtualKey)
    {
        if (virtualKey == ComboKey)
        {
            ActionType |= ActionType.Combo;
        }
        else if (virtualKey == HarasKey)
        {
            ActionType |= ActionType.Haras;
        }
        else if (virtualKey == FarmKey)
        {
            ActionType |= ActionType.Farm;
        }
        else if (virtualKey == ClearKey)
        {
            ActionType |= ActionType.Clear;
        }
    }

    private void InputManagerOnKeyUp(VirtualKey virtualKey)
    {
        if (virtualKey == ComboKey)
        {
            ActionType &= ~ActionType.Combo;
        }
        else if (virtualKey == HarasKey)
        {
            ActionType &= ~ActionType.Haras;
        }
        else if (virtualKey == FarmKey)
        {
            ActionType &= ~ActionType.Farm;
        }
        else if (virtualKey == ClearKey)
        {
            ActionType &= ~ActionType.Clear;
        }
    }
}