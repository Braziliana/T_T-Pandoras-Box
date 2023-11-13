using System.Numerics;
using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;
using Api.Utils;
using Scripts.Utils;
using Timer = Scripts.Utils.Timer;

namespace Scripts.CSharpScripts.Orbwlakers;

public class OrbWalker : IOrbWalkScript
{
    public string Name => "OrbWalker";
    public ScriptType ScriptType => ScriptType.OrbWalker;
    public bool Enabled { get; set; }

    private readonly IScriptingState _scriptingState;
    private readonly IGameManager _gameManager;
    private readonly ILocalPlayer _localPlayer;
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;
    private readonly IMinionSelector _minionSelector;
    private readonly IGameInput _gameInput;
    private readonly ITargetSelector _targetSelector;
    private readonly ITurretManager _turretManager;
    private readonly IGameCamera _gameCamera;
    private readonly IRandomGenerator _randomGenerator;

    private readonly Timer _humanizerTimer;
    private readonly Timer _attackTimer;
    private readonly Timer _moveTimer;
    
    private readonly IToggle _supportMode;
    private readonly IToggle _drawAttackRange;
    private readonly IToggle _drawKillableMinions;
    private readonly IToggle _humanizerSliderAddRandomDelay;
    private readonly IValueSlider _humanizerSlider;
    private readonly IValueSlider _pingSlider;
    private readonly IValueSlider _extraWindupSlider;
    private readonly IValueSlider _stoppingDistanceSlider;
    
    public OrbWalker(
        IMainMenu mainMenu,
        IScriptingState scriptingState,
        IGameManager gameManager,
        ILocalPlayer localPlayer,
        IRenderer renderer,
        IGameState gameState,
        IMinionSelector minionSelector,
        IGameInput gameInput,
        Timer humanizerTimer,
        Timer moveTimer,
        Timer attackTimer,
        ITargetSelector targetSelector,
        ITurretManager turretManager,
        IGameCamera gameCamera, IRandomGenerator randomGenerator)
    {
        _scriptingState = scriptingState;
        _gameManager = gameManager;
        _localPlayer = localPlayer;
        _renderer = renderer;
        _gameState = gameState;
        _minionSelector = minionSelector;
        _gameInput = gameInput;
        
        _humanizerTimer = humanizerTimer;
        _moveTimer = moveTimer;
        _attackTimer = attackTimer;
        _targetSelector = targetSelector;
        _turretManager = turretManager;
        _gameCamera = gameCamera;
        _randomGenerator = randomGenerator;

        var menu = mainMenu.CreateMenu(Name, ScriptType.OrbWalker);
        _humanizerSliderAddRandomDelay = menu.AddToggle("Humanizer random delay", true);
        _humanizerSlider = menu.AddFloatSlider("Humanizer", 75, 25, 300, 1, 0);
        _pingSlider = menu.AddFloatSlider("Ping", 35, 5, 300, 1, 0);
        _extraWindupSlider = menu.AddFloatSlider("Extra windup", 25, 0, 100, 1, 0);
        _stoppingDistanceSlider = menu.AddFloatSlider("Stopping distance", 70, 0, 250, 1, 0);

        _supportMode = menu.AddToggle("Support mode", false);
        _drawAttackRange = menu.AddToggle("Draw attack range", true);
        _drawKillableMinions = menu.AddToggle("Draw killable minions", true);
    }
    
    public void OnLoad()
    {
    }

    public void OnUnload()
    {
    }
    
    private float GetAttackTime()
    {
        return MathF.Max(1.0f / _localPlayer.AttackSpeed, _pingSlider.Value / 100);
    }

    private float GetWindupTime()
    {
        return (1.0f / _localPlayer.AttackSpeed) * _localPlayer.BasicAttackWindup + _extraWindupSlider.Value / 1000.0f;
    }
    
    private void MoveTo(Vector2 position)
    {
        if(!_moveTimer.IsReady) return;
        
        if (!_gameManager.GameCamera.WorldToScreen(_localPlayer.Position, out var playerScreenPosition))
        {
            return;
        }

        if (Vector2.Distance(playerScreenPosition, _gameInput.MousePosition) <= _stoppingDistanceSlider.Value)
        {
            return;
        }
        
        if (_gameInput.IssueOrder(position, IssueOrderType.Move))
        {
            var value = _humanizerSlider.Value;
            if (_humanizerSliderAddRandomDelay.Toggled)
            {
                value+=_randomGenerator.NextFloat(5, 50.0f);
            }
            _moveTimer.SetDelay(value / 1000.0f);
        }
    }

    private bool Attack(IAttackableUnit attackableUnit)
    {
        if(!_attackTimer.IsReady) return false;

        if (!_gameInput.Attack(attackableUnit)) return false;
        
        _attackTimer.SetDelay(GetAttackTime());
        _moveTimer.SetDelay(GetWindupTime());

        _humanizerTimer.SetDelay(_humanizerSlider.Value/1000);
        return true;

    }
    
    public void OnUpdate(float deltaTime)
    {
        if (_scriptingState.ActionType == ActionType.None || !_localPlayer.IsAlive)
        {
            return;
        }

        if (_scriptingState.IsCombo)
        {
            Combo(deltaTime);
        }
        else if (_scriptingState.IsHaras)
        {
            Haras(deltaTime);
        }
        else if (_scriptingState.IsFarm)
        {
            Farm(deltaTime);
        }
        else if (_scriptingState.IsClear)
        {
            Clear(deltaTime);
        }
    }

    private void Combo(float deltaTime)
    {
        var target = _targetSelector.GetTarget();
        if (target is not null)
        {
            if (Attack(target))
            {
                return;
            }
        }
        MoveTo(_gameInput.MousePosition);
    }

    private void Haras(float deltaTime)
    {
        IAttackableUnit? target = GetKillableMinion();
        
        if (target is null)
        {
            target = _targetSelector.GetTarget();
        }
        
        if (target is not null)
        {
            if (Attack(target))
            {
                return;
            }
        }
        
        MoveTo(_gameInput.MousePosition);
    }
    
    private void Farm(float deltaTime)
    {
        var target = GetKillableMinion();
        if (target is not null)
        {
            if (Attack(target))
            {
                return;
            }
        }
        
        MoveTo(_gameInput.MousePosition);
    }

    private void Clear(float deltaTime)
    {
        IAttackableUnit? target = GetKillableMinion();

        if (target is null)
        {
            target = _turretManager.GetEnemyTurrets(_localPlayer.AttackRange).FirstOrDefault();
        }
        
        if (target is null)
        {
            target = _minionSelector.GetHealthiestMinion(_localPlayer.AttackRange);
        }
        
        if (target is null)
        {
            var monsters = _gameManager.ObjectManager.MonsterManager.GetMonsters(_localPlayer.AttackRange);
            target = monsters.MinBy(x => x.Health);
        }
        
        if (target is not null)
        {
            if (Attack(target))
            {
                return;
            }
        }
        
        MoveTo(_gameInput.MousePosition);
    }
    
    public void OnRender(float deltaTime)
    {
        if (_drawAttackRange.Toggled)
        {
            _renderer.CircleBorder3D(_localPlayer.Position, _localPlayer.AttackRange, Color.White, 2);
        }

        if (_stoppingDistanceSlider.Value > 50)
        {
            _renderer.CircleBorder3D(_localPlayer.Position, _stoppingDistanceSlider.Value, Color.Green, 2);
        }
        
        DrawKillableMinions();
    }
    
    private void DrawKillableMinions()
    {
        if (!_drawKillableMinions.Toggled)
        {
            return;
        }
        
        var range = MathF.Max(_localPlayer.AttackRange + 300, 1000);

        foreach (var minion in _minionSelector.GetKillableMinions(range).Select(x => x.Minion))
        {
            if(minion is null) continue;
            _renderer.CircleBorder3D(minion.Position, minion.CollisionRadius, Color.Red, 1);
        }
    }
    
    private IMinion? GetKillableMinion()
    {
        if (_supportMode.Toggled && _gameManager.HeroManager.GetAllyHeroes(1000).Any())
        {
            return null;
        }
        var target = _minionSelector.GetBestKillableMinion(_localPlayer.AttackRange);
        return target.Minion;
    }
}