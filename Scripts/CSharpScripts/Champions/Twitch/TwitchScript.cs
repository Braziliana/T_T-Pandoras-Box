using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;
using Scripts.Utils;

namespace Scripts.CSharpScripts.Champions.Twitch;

public class TwitchScript : IChampionScript
{
    public string Name => "Twitch";

    public string Champion => "Twitch";
    public ScriptType ScriptType => ScriptType.Champion;
    public bool Enabled { get; set; }
    
    private readonly IMainMenu _mainMenu;
    private IMenu? _menu = null;
    private readonly ILocalPlayer _localPlayer;
    private readonly IScriptingState _scriptingState;
    private readonly ITargetSelector _targetSelector;
    private readonly IPrediction _prediction;
    private readonly IGameInput _gameInput;
    private readonly IGameState _gameState;
    private readonly ITrapManager _trapManager;
    private readonly IHeroManager _heroManager;
    private readonly ISpellCaster _spellCaster;
    private readonly IDamageCalculator _damageCalculator;
    private readonly IRenderer _renderer;
    private readonly IGameCamera _gameCamera;

    private IToggle _useQInCombo;
    private IToggle _useWInCombo;
    private IToggle _useEInCombo;
    
    private IToggle _useWInHarass;
    private IToggle _useEInHarass;
    
    private IValueSlider _WHitChance;
    private IValueSlider _WReactionTime;

    private IToggle _autoEKs;
    private IToggle _autoWCC;
    private IToggle _autoWDashing;
    
    private readonly float[] _eDamage = new float[]{20, 30, 40, 50, 60};
    private readonly float[] _eStackDamage = new float[]{15, 20, 25, 30, 35};
    
    public TwitchScript(
        IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        IGameInput gameInput,
        IGameState gameState,
        ITrapManager trapManager,
        IHeroManager heroManager,
        ISpellCaster spellCaster,
        IDamageCalculator damageCalculator, IRenderer renderer, IGameCamera gameCamera)
    {
        _mainMenu = mainMenu;
        _localPlayer = localPlayer;
        _scriptingState = scriptingState;
        _targetSelector = targetSelector;
        _prediction = prediction;
        _gameInput = gameInput;
        _gameState = gameState;
        _trapManager = trapManager;
        _heroManager = heroManager;
        _spellCaster = spellCaster;
        _damageCalculator = damageCalculator;
        _renderer = renderer;
        _gameCamera = gameCamera;
    }
    
    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Twitch", ScriptType.Champion);
        var comboMenu = _menu.AddSubMenu("Combo");
        _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
        _useWInCombo = comboMenu.AddToggle("Use W in combo", true);
        _useEInCombo = comboMenu.AddToggle("Use E in combo 5 stacks", true);

        var harassMenu = _menu.AddSubMenu("Harass");
        _useWInHarass = harassMenu.AddToggle("Use W in harass", true);
        _useEInHarass = harassMenu.AddToggle("Use E in harass 5 stacks", true);
        
        var hitChanceMenu = _menu.AddSubMenu("Hit chance");
        _WHitChance = hitChanceMenu.AddFloatSlider("W hit chance", 0.8f, 0.0f, 1.0f, 0.05f, 2);

        _WReactionTime = hitChanceMenu.AddFloatSlider("W reaction time", 0.00f, 0.0f, 300f, 5f, 2);

        var autoMenu = _menu.AddSubMenu("Auto");
        _autoEKs = autoMenu.AddToggle("Auto E ks", true);
        _autoWCC = autoMenu.AddToggle("Auto W CC enemy", true);
        _autoWDashing = autoMenu.AddToggle("Auto W dashing enemy", true);
    }

    public void OnUnload()
    {
        if (_menu is not null)
        {
            _mainMenu.RemoveMenu(_menu);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (!_localPlayer.IsAlive)
        {
            return;
        }
        
        if (Auto())
        {
            return;
        }
        
        if (Combo())
        {
            return;
        }

        if (Harass())
        {
            return;
        }
    }

    private int GetEStacks(IHero hero)
    {
        var buff = hero.GetBuff("TwitchDeadlyVenom");
        if (buff is null)
        {
            return 0;
        }
        
        if(buff.CountAlt2 > 6 || buff.CountAlt2 < 0)
        {
            return 0;
        }

        return buff.CountAlt2;
    }

    private float GetEDamage(IHero target)
    {
        var spell = _localPlayer.E;
        if (!spell.IsReady)
        {
            return 0;
        }
        
        var stacks = GetEStacks(target);
        if (stacks < 1) return 0.0f;
        
        var physicalDamage = _eDamage[spell.Level-1] + 0.35f * stacks * _localPlayer.BonusAttackDamage + _eStackDamage[spell.Level - 1];
        var magicDamage = (0.30f * stacks * _localPlayer.AbilityPower);
        
        return _damageCalculator.GetPhysicalDamage(_localPlayer, target, physicalDamage) + _damageCalculator.GetMagicDamage(_localPlayer, target, magicDamage);
    }

    private bool Combo()
    {
        if (_scriptingState.IsCombo == false)
        {
            return false;
        }
        
        var target = _targetSelector.GetTarget();
        if (target == null)
        {
            return false;
        }
        
        if (_useWInCombo.Toggled && CastW(target))
        {
            return true;
        }
        
        if (_useQInCombo.Toggled && CastQ())
        {
            return true;
        }
        
        if (_useEInCombo.Toggled && CanCast(_localPlayer.E))
        {
            var stacks = GetEStacks(target);
            if (stacks > 1)
            {
                if(stacks == 6 || target.Distance(_localPlayer) >= _localPlayer.AttackRange - 50.0f)
                {
                    if (CastE())
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    private bool Harass()
    {
        if (_scriptingState.IsHaras == false)
        {
            return false;
        }
        
        var target = _targetSelector.GetTarget();
        if (target == null)
        {
            return false;
        }
        
        if (_useWInHarass.Toggled && CastW(target))
        {
            return true;
        }
        
        if (_useEInHarass.Toggled && CanCast(_localPlayer.E) && GetEStacks(target) == 6 && CastE())
        {
            return true;
        }
        
        return false;
    }
    
    private float GetImmobileBuffDuration(IHero hero)
    {
        float duration = 0;
        foreach (var buff in hero.Buffs.Where(x => x.IsHardCC()))
        {
            var buffDuration = buff.EndTime - _gameState.Time;
            if (duration < buff.EndTime - _gameState.Time)
            {
                duration = buffDuration;
            }
        }
        return duration;
    }

    private bool Auto()
    {
        var enemies = _heroManager.GetEnemyHeroes();
        if (_autoEKs.Toggled && CanCast(_localPlayer.E))
        {
            var eSpell = _localPlayer.E;
            foreach (var hero in enemies.Where(x => x.Distance(_localPlayer) <= eSpell.Range))
            {
                if (hero.Health < GetEDamage(hero))
                {
                    _spellCaster.TryCast(eSpell);
                    return true;
                }
            }
        }

        if (CanCast(_localPlayer.W))
        {
            foreach (var enemy in enemies)
            {
                var immobileTime = GetImmobileBuffDuration(enemy);
                var distance = enemy.Distance(_localPlayer);
                if (distance <= _localPlayer.W.Range && _autoWCC.Toggled)
                {
                    if (immobileTime > _localPlayer.W.SpellData.CastDelayTime)
                    {
                        if (CastW(enemy))
                        {
                            return true;
                        }
                    }
                }

                if (_autoWDashing.Toggled && enemy.AiManager.IsDashing)
                {
                    if (CastW(enemy))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CastQ()
    {
        return _spellCaster.TryCast(_localPlayer.Q);
    }
    
    private bool CastW(IHero target)
    {
        return _spellCaster.TryCastPredicted(_localPlayer.W, target, _WReactionTime.Value / 1000.0f, 0.0f,
            _WHitChance.Value, CollisionType.None, PredictionType.Point);
    }

    private bool CastE()
    {
        return _spellCaster.TryCast(_localPlayer.E);
    }
    
    private bool CanCast(ISpell spell)
    {
        return _spellCaster.CanCast(spell);
    }
    
    public void OnRender(float deltaTime)
    {
        foreach (var enemyHero in _heroManager.GetEnemyHeroes(_localPlayer.E.Range))
        {
            if (_gameCamera.WorldToScreen(enemyHero.Position, out var screenPosition))
            {
                _renderer.Text(GetEStacks(enemyHero).ToString(), screenPosition, 16, Color.Cyan, TextHorizontalOffset.Center, TextVerticalOffset.Center);
            }
        }
    }
}