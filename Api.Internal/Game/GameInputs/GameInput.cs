using System.Numerics;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Inputs;

namespace Api.Internal.Game.GameInputs;

internal class GameInput : IGameInput
{
    private readonly int _delay = 5;
    private readonly IInputManager _inputManager;
    private readonly IGameCamera _gameCamera;
    private readonly ILocalPlayer _localPlayer;
    private Task? _currentTask;

    public GameInput(
        IInputManager inputManager,
        IGameCamera gameCamera,
        ILocalPlayer localPlayer)
    {
        _inputManager = inputManager;
        _gameCamera = gameCamera;
        _localPlayer = localPlayer;
    }

    public Vector2 MousePosition { get; private set; }

    public bool IssueOrder(Vector2 position, IssueOrderType issueOrderType)
    {
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
        return _gameCamera.WorldToScreen(position, out var screenPosition) && IssueOrder(screenPosition, IssueOrderType.Attack);
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

    public void Update()
    {
        MousePosition = _inputManager.GetMousePosition();
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

    private bool CanCast(SpellSlot spellSlot)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        if (_localPlayer.ActiveCastSpell.IsActive)
        {
            if (_localPlayer.ActiveCastSpell.Type != ActiveSpellType.Unknown &&
                _localPlayer.ActiveCastSpell.Type != ActiveSpellType.AutoAttack)
            {
                return false;
            }
        }

        return true;
    }
    
    public bool CastSpell(SpellSlot spellSlot)
    {
        if (!CanCast(spellSlot))
        {
            return false;
        }
        
        _inputManager.KeyboardSend(MapSpellSlot(spellSlot));
        return true;
    }

    public bool SelfCastSpell(SpellSlot spellSlot)
    {
        if (!CanCast(spellSlot))
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
        if (!CanCast(spellSlot))
        {
            return false;
        }
        return SendInput(MouseButton.Left, position, MapSpellSlot(spellSlot));
    }

    public bool CastSpell(SpellSlot spellSlot, Vector3 position)
    {
        return _gameCamera.WorldToScreen(position, out var screenPosition) && CastSpell(spellSlot, screenPosition);
    }

    public bool CastSpell(SpellSlot spellSlot, IGameObject target)
    {
        return CastSpell(spellSlot, target.Position);
    }

    private bool SendInput(MouseButton mouseButton, Vector2 position, VirtualKey virtualKey)
    {
        if (_currentTask is not null && !_currentTask.IsCompleted)
        {
            return false;
        }
        
        _currentTask = Task.Factory.StartNew(async () =>
        {
            var prevPos = MousePosition;
            _inputManager.KeyboardSend(virtualKey);
            _inputManager.MouseSend(mouseButton, position);
            await Task.Delay(_delay);
            _inputManager.MouseSetPosition(prevPos);
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
            var prevPos = MousePosition;
            _inputManager.KeyboardSendDown(press);
            _inputManager.KeyboardSend(virtualKey);
            _inputManager.MouseSend(mouseButton, position);
            _inputManager.KeyboardSendUp(press);
            await Task.Delay(_delay);
            _inputManager.MouseSetPosition(prevPos);
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