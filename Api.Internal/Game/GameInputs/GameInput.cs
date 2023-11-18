using System.Numerics;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Inputs;
using Api.Menus;
using Api.Scripts;

namespace Api.Internal.Game.GameInputs;

internal class GameInput : IGameInput
{
    private readonly IInputManager _inputManager;
    private readonly IGameCamera _gameCamera;
    private readonly ILocalPlayer _localPlayer;
    private Task? _currentTask;
    private bool _mouseInputBlocked = false;
    private int _ticksToResetMouse = 1;

    private readonly IValueSlider _castSpellMouseHoldDuration;
    
    public GameInput(
        IInputManager inputManager,
        IGameCamera gameCamera,
        ILocalPlayer localPlayer,
        IMainMenu mainMenu)
    {
        _inputManager = inputManager;
        _gameCamera = gameCamera;
        _localPlayer = localPlayer;
        
        var gameInputMenu = mainMenu.CreateMenu("GameInput", ScriptType.Input);
        _castSpellMouseHoldDuration = gameInputMenu.AddFloatSlider("Cast extra hoover time", 1, 0, 50, 1, 2);
        
    }

    public Vector2 MousePosition { get; private set; }

    public bool IssueOrder(Vector2 position, IssueOrderType issueOrderType)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        switch (issueOrderType)
        {
            case IssueOrderType.Move:
                _inputManager.MouseSend(MouseButton.Right, position);
                break;
            case IssueOrderType.Attack:
                return SendInput(MouseButton.Left, position, VirtualKey.A);
            case IssueOrderType.MoveAttack:
                return SendInput(MouseButton.Left, position, VirtualKey.A);
            case IssueOrderType.AttackHero:
                return SendInput(MouseButton.Left, position, VirtualKey.A, VirtualKey.Backtick);
            default:
                throw new ArgumentOutOfRangeException(nameof(issueOrderType), issueOrderType, null);
        }

        return true;
    }

    public bool IssueOrder(Vector3 position, IssueOrderType issueOrderType)
    {
        if (!_gameCamera.WorldToScreen(position, out var screenPosition))
        {
            return false;
        }
        
        return IssueOrder(screenPosition, IssueOrderType.Attack);
    }

    public bool Attack(IGameObject target)
    {
        return IssueOrder(target.Position, IssueOrderType.Attack);
    }

    public void CastEmote(int emote)
    {
        var emoteKey = emote switch
        {
            1 => VirtualKey.Key1,
            2 => VirtualKey.Key2,
            3 => VirtualKey.Key3,
            4 => VirtualKey.Key4,
            _ => VirtualKey.Key1
        };
        _inputManager.KeyboardSendDown(VirtualKey.LeftControl);
        _inputManager.KeyboardSend(emoteKey);
        _inputManager.KeyboardSendUp(VirtualKey.LeftControl);
    }

    public void Update(float deltaTime)
    {
        _ticksToResetMouse = (int)MathF.Round(deltaTime);
        if (!_mouseInputBlocked)
        {
            MousePosition = _inputManager.GetMousePosition();
        }
    }

    private VirtualKey MapSpellSlot(SpellSlot spellSlot)
    {
        return spellSlot switch
        {
            SpellSlot.Q => VirtualKey.Q,
            SpellSlot.W => VirtualKey.W,
            SpellSlot.E => VirtualKey.E,
            SpellSlot.R => VirtualKey.R,
            SpellSlot.Summoner1 => VirtualKey.D,
            SpellSlot.Summoner2 => VirtualKey.F,
            _ => throw new ArgumentOutOfRangeException(nameof(spellSlot), spellSlot, null)
        };
    }
    
    public bool LevelUpSpell(SpellSlot spellSlot)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _inputManager.KeyboardSendDown(VirtualKey.LeftControl);
        _inputManager.KeyboardSend(MapSpellSlot(spellSlot));
        _inputManager.KeyboardSendUp(VirtualKey.LeftControl);

        return true;
    }
    
    public bool CastSpell(SpellSlot spellSlot)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _inputManager.KeyboardSend(MapSpellSlot(spellSlot));
        return true;
    }

    public bool SelfCastSpell(SpellSlot spellSlot)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _inputManager.KeyboardSendDown(VirtualKey.LeftAlt);
        _inputManager.KeyboardSend(MapSpellSlot(spellSlot));
        _inputManager.KeyboardSendUp(VirtualKey.LeftAlt);

        return true;
    }

    public bool CastSpell(SpellSlot spellSlot, Vector2 position)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        return SendSpellInput(position, MapSpellSlot(spellSlot));
    }

    public bool CastSpell(SpellSlot spellSlot, Vector3 position)
    {
        return _gameCamera.WorldToScreen(position, out var screenPosition) && CastSpell(spellSlot, screenPosition);
    }

    public bool CastSpell(SpellSlot spellSlot, IGameObject target)
    {
        return CastSpell(spellSlot, target.Position);
    }

    private bool SendInput(Vector2 position, VirtualKey virtualKey)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }

        _currentTask = Task.Factory.StartNew(async () =>
        {
            _mouseInputBlocked = true;
            _inputManager.BlockMouseInput(true);
            var prevPos = MousePosition;
            _inputManager.MouseSetPosition(position);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.KeyboardSend(virtualKey);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.MouseSetPosition(prevPos);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.BlockMouseInput(false);
            MousePosition = prevPos;
            _mouseInputBlocked = false;
        });

        return true;
    }
    
    private bool SendSpellInput(Vector2 position, VirtualKey virtualKey)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }

        _currentTask = Task.Factory.StartNew(async () =>
        {
            _mouseInputBlocked = true;
            _inputManager.BlockMouseInput(true);
            var prevPos = MousePosition;
            await Task.Delay(_ticksToResetMouse);
            _inputManager.MouseSetPosition(position);
            await Task.Delay(_ticksToResetMouse + (int)_castSpellMouseHoldDuration.Value);
            _inputManager.KeyboardSend(virtualKey);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.MouseSetPosition(prevPos);
            await Task.Delay(_ticksToResetMouse + (int)_castSpellMouseHoldDuration.Value);
            _inputManager.BlockMouseInput(false);
            MousePosition = prevPos;
            _mouseInputBlocked = false;
        });

        return true;
    }

    private bool SendInput(MouseButton mouseButton, Vector2 position, VirtualKey virtualKey)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _currentTask = Task.Factory.StartNew(async () =>
        {
            _mouseInputBlocked = true;
            _inputManager.BlockMouseInput(true);
            var prevPos = MousePosition;
            _inputManager.MouseSetPosition(position);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.KeyboardSend(virtualKey);
            _inputManager.MouseSend(mouseButton);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.MouseSetPosition(prevPos);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.BlockMouseInput(false);
            MousePosition = prevPos;
            _mouseInputBlocked = false;
        });

        return true;
    }
    
    private bool SendInput(MouseButton mouseButton, Vector2 position, VirtualKey virtualKey, VirtualKey press)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _currentTask = Task.Factory.StartNew(async () =>
        {
            _mouseInputBlocked = true;
            _inputManager.BlockMouseInput(true);
            var prevPos = MousePosition;
            _inputManager.MouseSetPosition(position);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.KeyboardSendDown(press);
            _inputManager.KeyboardSend(virtualKey);
            _inputManager.MouseSend(mouseButton);
            _inputManager.KeyboardSendUp(press);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.MouseSetPosition(prevPos);
            await Task.Delay(_ticksToResetMouse);
            _inputManager.BlockMouseInput(false);
            MousePosition = prevPos;
            _mouseInputBlocked = false;
        });

        return true;
    }

    //
    // public void UseItem(ItemSlot itemSlot)
    // {
    // }
    //
    // public void UseSpell(ItemSlot itemSlot, Vector2 position)
    // {
    // }
    //
    // public void UseSpell(ItemSlot itemSlot, Vector3 position)
    // {
    // }
    //
    // public void UseSpell(ItemSlot itemSlot, IGameObject target)
    // {
    // }
}