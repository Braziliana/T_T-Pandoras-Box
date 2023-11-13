using System.Numerics;
using Api;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;

namespace Scripts.Utils;

public enum TargetSelectorMode
{
    Weighted,
    MinHealth,
    MaxAp,
    MaxAd,
    Closest,
    ClosestToCursor
}

public interface ITargetSelector : IScript
{
    TargetSelectorMode TargetSelectorMode { get; set; }
    public float HealthWeight { get; set; }
    public float AbilityPowerWeight { get; set; }
    public float DamageWeight { get; set; }
    public IHero? GetTarget();
    public IHero? GetTarget(float range);
}

public class TargetSelector : ITargetSelector
{
    public string Name => "TargetSelector";
    public ScriptType ScriptType => ScriptType.TargetSelector;
    public bool Enabled { get; set; }
    
    private readonly ILocalPlayer _localPlayer;
    private readonly IHeroManager _heroManager;
    public TargetSelectorMode TargetSelectorMode { get; set; }
    public float HealthWeight
    {
        get => _healthWeightSlider?.Value ?? 0.5f;
        set
        {
            if (_healthWeightSlider is not null)
            {
                _healthWeightSlider.Value = value;
            }
        }
    }

    public float AbilityPowerWeight
    {
        get => _abilityPowerWeightSlider?.Value ?? 0.5f;
        set
        {
            if (_abilityPowerWeightSlider is not null)
            {
                _abilityPowerWeightSlider.Value = value;
            }
        }
    }

    public float DamageWeight
    {
        get => _damageWeightSlider?.Value ?? 0.5f;
        set
        {
            if (_damageWeightSlider is not null)
            {
                _damageWeightSlider.Value = value;
            }
        }
    }

    private readonly IGameCamera _gameCamera;
    private readonly IGameInput _gameInput;
    private readonly IMainMenu _mainMenu;
    
    private IMenu? _menu;
    private IEnumComboBox<TargetSelectorMode>? _targetSelectorComboBox;
    private IValueSlider? _healthWeightSlider;
    private IValueSlider? _abilityPowerWeightSlider;
    private IValueSlider? _damageWeightSlider;
    private IHero? _target;
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;
    
    public TargetSelector(
        ILocalPlayer localPlayer,
        IHeroManager heroManager,
        IMainMenu mainMenu,
        IGameCamera gameCamera,
        IGameInput gameInput,
        IRenderer renderer,
        IGameState gameState)
    {
        _localPlayer = localPlayer;
        _heroManager = heroManager;
        _mainMenu = mainMenu;
        _gameCamera = gameCamera;
        _gameInput = gameInput;
        _renderer = renderer;
        _gameState = gameState;
        TargetSelectorMode = TargetSelectorMode.Weighted;
    }

    public IHero? GetTarget()
    {
        return _target;
    }

    public IHero? GetTarget(float range)
    {
        return Selection(_heroManager.GetEnemyHeroes(range)
            .Where(x => x is { IsVisible: true, Targetable: true }));
    }

    private float GetWeight(IHero hero)
    {
        var hpFactor = 1.0f - hero.Health / hero.MaxHealth;

        return hpFactor * HealthWeight +
               hero.AbilityPower * AbilityPowerWeight +
               hero.TotalAttackDamage * DamageWeight;
    }

    private IHero? GetClosestToCursorHero(IEnumerable<IHero> heroes)
    {
        var closestDistance = float.MaxValue;
        IHero? closest = null;
        foreach (var hero in heroes)
        {
            if (!_gameCamera.WorldToScreen(hero.Position, out var screenPos))
            {
                continue;
            }

            var distance = Vector2.Distance(screenPos, _gameInput.MousePosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hero;
            }
        }

        return closest;
    }
    
    private IHero? Selection(IEnumerable<IHero> heroes)
    {
        switch (TargetSelectorMode)
        {
            case TargetSelectorMode.Weighted:
                return heroes.MaxBy(GetWeight);
            case TargetSelectorMode.MaxAp:
                return heroes.MaxBy(x => x.AbilityPower);
            case TargetSelectorMode.MaxAd:
                return heroes.MaxBy(x => x.TotalAttackDamage);
            case TargetSelectorMode.Closest:
                return heroes.MinBy(x => x.Distance(_localPlayer));
            case TargetSelectorMode.ClosestToCursor:
                return GetClosestToCursorHero(heroes);
            case TargetSelectorMode.MinHealth:
            default:
                return heroes.MinBy(x => x.Health);
        }
    }

    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Target selector", ScriptType.TargetSelector);
        _targetSelectorComboBox =
            _menu.AddEnumComboBox("Target selector mode", TargetSelectorMode.Weighted);

        _targetSelectorComboBox.SelectionChanged += (TargetSelectorMode targetSelectorMode) =>
        {
            TargetSelectorMode = targetSelectorMode;
        };

        _healthWeightSlider = _menu.AddFloatSlider("Health weight", 0.5f, 0.1f, 1.0f, 0.1f, 1);
        _abilityPowerWeightSlider = _menu.AddFloatSlider("Ability power weight", 0.5f, 0.1f, 1.0f, 0.1f, 1);
        _damageWeightSlider = _menu.AddFloatSlider("Attack damage weight", 0.5f, 0.1f, 1.0f, 0.1f, 1);
    }

    public void OnUnload()
    {
        if (_menu != null)
        {
            _mainMenu.RemoveMenu(_menu);
        }

        _healthWeightSlider = null;
        _abilityPowerWeightSlider = null;
        _damageWeightSlider = null;
    }

    public void OnUpdate(float deltaTime)
    {
        _target = GetTarget(_localPlayer.AttackRange);
    }

    public void OnRender(float deltaTime)
    {
        if (_target is not null)
        {
            _renderer.CircleBorder3D(_target.Position, _target.CollisionRadius, Color.Yellow, 1);
        }
    }
}