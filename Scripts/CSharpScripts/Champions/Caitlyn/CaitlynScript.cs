using System.Globalization;
using System.Numerics;

using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;
using Scripts.Utils;

namespace Scripts.CSharpScripts.Champions.Caitlyn;

public class CaitlynScript : IChampionScript
{
    public string Name => "Caitlyn";

    public string Champion => "Caitlyn";
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
    private readonly int _trapNameHash = "CaitlynTrap".GetHashCode();

    private IToggle _useQInCombo;
    private IToggle _useWInCombo;
    private IToggle _useEInCombo;

    private IToggle _useQInHarass;
    private IToggle _useWInHarass;
    private IToggle _useEInHarass;

    private IValueSlider _QHitChance;
    private IValueSlider _WHitChance;
    private IValueSlider _EHitChance;

    private IValueSlider _QReactionTime;
    private IValueSlider _WReactionTime;
    private IValueSlider _EReactionTime;

    private IToggle _autoQCC;
    private IToggle _autoWCC;
    private IToggle _autoWDashing;

    private IToggle _DrawRKillable;

    private readonly float[] _rDamage = new float[]{300, 500, 700};    

    public CaitlynScript(
        IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        IGameInput gameInput,
        IGameState gameState,
        ITrapManager trapManager,
        IHeroManager heroManager,
        ISpellCaster spellCaster)
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
    }

    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Caitlin", ScriptType.Champion);
        var comboMenu = _menu.AddSubMenu("Combo");
        _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
        _useWInCombo = comboMenu.AddToggle("Use W in combo", true);
        _useEInCombo = comboMenu.AddToggle("Use E in combo", true);

        var harassMenu = _menu.AddSubMenu("Harass");
        _useQInHarass = harassMenu.AddToggle("Use Q in harass", true);
        _useWInHarass = harassMenu.AddToggle("Use W in harass", true);
        _useEInHarass = harassMenu.AddToggle("Use E in harass", true);

        var hitChanceMenu = _menu.AddSubMenu("Hit chance");
        _QHitChance = hitChanceMenu.AddFloatSlider("Q hit chance", 0.8f, 0.0f, 1.0f, 0.05f, 2);
        _WHitChance = hitChanceMenu.AddFloatSlider("W hit chance", 0.9f, 0.0f, 1.0f, 0.05f, 2);
        _EHitChance = hitChanceMenu.AddFloatSlider("E hit chance", 0.9f, 0.0f, 1.0f, 0.05f, 2);

        _QReactionTime = hitChanceMenu.AddFloatSlider("Q reaction time", 50f, 0.0f, 300f, 5f, 2);
        _WReactionTime = hitChanceMenu.AddFloatSlider("W reaction time", 0.00f, 0.0f, 300f, 5f, 2);
        _EReactionTime = hitChanceMenu.AddFloatSlider("E reaction time", 50f, 0.0f, 300f, 5f, 2);

        var autoMenu = _menu.AddSubMenu("Auto");
        _autoQCC = autoMenu.AddToggle("Auto Q CC enemy", true);
        _autoWCC = autoMenu.AddToggle("Auto W CC enemy", true);
        _autoWDashing = autoMenu.AddToggle("Auto W dashing enemy", true);

        var DrawMenu = _menu.AddSubMenu("Drawing");
        _DrawRKillable = DrawMenu.AddToggle("Draw R Killable Status (Currently no CritBonus)", true);
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

    private float GetrDamage(IHero target)
    {
        var spell = _localPlayer.R;
        if (!spell.IsReady)
        {
            return 0;
        }
        // TODO: Fix CriticalChance not being returned as 0 even after adding to AIBaseUnit 
        var physicalDamage = _rDamage[spell.Level-1] + (1.7f + (1.7f * (0.5f * _localPlayer.CriticalChance))) * _localPlayer.BonusAttackDamage;
       
        return _damageCalculator.GetPhysicalDamage(_localPlayer, target, physicalDamage);
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

        foreach (var enemy in enemies)
        {
            var immobileTime = GetImmobileBuffDuration(enemy);
            var distance = enemy.Distance(_localPlayer);
            if (distance <= _localPlayer.W.Range && _autoWCC.Toggled && CanCast(_localPlayer.W))
            {
                if (immobileTime > _localPlayer.W.SpellData.CastDelayTime + 0.1f)
                {
                    if (CastW(enemy))
                    {
                        return true;
                    }
                }
            }

            if (_autoWDashing.Toggled && enemy.AiManager.IsDashing && CanCast(_localPlayer.W))
            {
                if (CastW(enemy))
                {
                    return true;
                }
            }

            if(distance <= _localPlayer.Q.Range && _autoQCC.Toggled && CanCast(_localPlayer.Q))
            {
                if (immobileTime > _localPlayer.Q.SpellData.CastDelayTime)
                {
                    if (CastQ(enemy))
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
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

        var distance = Vector3.Distance(_localPlayer.Position, target.Position);


        if (_useEInCombo.Toggled && distance <= _localPlayer.E.Range && CanCast(_localPlayer.E) && CastE(target))
        {
            return true;
        }

        if (_useQInCombo.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.Q) && CastQ(target))
        {
            return true;
        }

        if (_useWInCombo.Toggled && distance <= _localPlayer.W.Range && CanCast(_localPlayer.W) && CastW(target))
        {
            return true;
        }

        return false;
    }

    private bool Harass()
    {
        if (_scriptingState.IsHaras == false)
        {
            return false;
        }

        if (_useEInHarass.Toggled && CanCast(_localPlayer.E))
        {
            var target = _targetSelector.GetTarget(_localPlayer.E.Range);
            if (target != null && CastE(target))
            {
                return true;
            }
        }

        if (_useQInHarass.Toggled && CanCast(_localPlayer.Q))
        {
            var target = _targetSelector.GetTarget(_localPlayer.Q.Range);
            if (target != null && CastQ(target))
            {
                return true;
            }
        }

        if (_useWInHarass.Toggled && CanCast(_localPlayer.W))
        {
            var target = _targetSelector.GetTarget(_localPlayer.W.Range);
            if (target != null && CastW(target))
            {
                return true;
            }
        }

        return false;
    }

    private bool CanCast(ISpell spell)
    {
        return _spellCaster.CanCast(spell);
    }

    private bool CastQ(IHero target)
    {
        return _spellCaster.TryCastPredicted(_localPlayer.Q, target, _QReactionTime.Value / 1000.0f, 0.0f,
            _QHitChance.Value, CollisionType.None, PredictionType.Line);
    }

    private bool CastW(IHero target)
    {
        var spell = _localPlayer.W;
        if (spell.SpellData == null) return false;

        var buff = target.GetBuff("caitlynwsight");
        if (buff != null && buff.EndTime > _gameState.Time)
        {
            return false;
        }

        if (_trapManager.GetAllyTraps(target.Position, 200).Any(x => x.ObjectNameHash == _trapNameHash))
        {
            return false;
        }


        var spellData = spell.SpellData;
        if (spellData == null) return false;
        return _spellCaster.TryCastPredicted(spell, target, spellData.CastDelayTime + 0.1f, 0.0f,
            spellData.Width, spellData.Range, _WReactionTime.Value / 1000.0f, 0.0f,
            _WHitChance.Value, CollisionType.None, PredictionType.Point);
    }

    private bool CastE(IHero target)
    {
        return _spellCaster.TryCastPredicted(_localPlayer.E, target, _EReactionTime.Value / 1000.0f, 0.0f,
            _EHitChance.Value, CollisionType.None, PredictionType.Line);
    }

    public void OnRender(float deltaTime)
    {
        if (_DrawRKillable.Toggled)
        {
            foreach (var enemyHero in _heroManager.GetEnemyHeroes(_localPlayer.R.Range))
            {
                if (enemyHero.Health < GetrDamage(enemyHero))
    			{
                    _renderer.Text($"R Killable", enemyHero.Position, 36, Color.Green);
    			}
            }
        }
    }
}
