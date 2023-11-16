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
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;
    private readonly IGameCamera _gameCamera;
    private readonly ITrapManager _trapManager;
    private readonly IHeroManager _heroManager;
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

    
    private IToggle _autoWCC;

    public CaitlynScript(
        IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        IGameInput gameInput,
        IRenderer renderer,
        IGameState gameState,
        IGameCamera gameCamera,
        ITrapManager trapManager,
        IHeroManager heroManager)
    {
        _mainMenu = mainMenu;
        _localPlayer = localPlayer;
        _scriptingState = scriptingState;
        _targetSelector = targetSelector;
        _prediction = prediction;
        _gameInput = gameInput;
        _renderer = renderer;
        _gameState = gameState;
        _gameCamera = gameCamera;
        _trapManager = trapManager;
        _heroManager = heroManager;
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

        var hitchanceMenu = _menu.AddSubMenu("Hit chcnce");
        _QHitChance = hitchanceMenu.AddFloatSlider("Q hit chcance", 0.8f, 0.0f, 1.0f, 0.05f, 2);
        _WHitChance = hitchanceMenu.AddFloatSlider("W hit chcance", 0.9f, 0.0f, 1.0f, 0.05f, 2);
        _EHitChance = hitchanceMenu.AddFloatSlider("E hit chcance", 0.9f, 0.0f, 1.0f, 0.05f, 2);

        _QReactionTime = hitchanceMenu.AddFloatSlider("Q reaction time", 50f, 0.0f, 300f, 5f, 2);
        _WReactionTime = hitchanceMenu.AddFloatSlider("W reaction time", 0.00f, 0.0f, 300f, 5f, 2);
        _EReactionTime = hitchanceMenu.AddFloatSlider("E reaction time", 50f, 0.0f, 300f, 5f, 2);

        _autoWCC = _menu.AddToggle("Auto W CC enemy", true);
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

        if (AutoWCC())
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
    
    private bool AutoWCC()
    {
        if (!_autoWCC.Toggled || !CanCast(_localPlayer.W))
        {
            return false;
        }
        var enemies = _heroManager.GetEnemyHeroes(_localPlayer.W.Range);

        foreach (var enemy in enemies)
        {
            var immobileTime = GetImmobileBuffDuration(enemy);
            if (immobileTime > _localPlayer.W.SpellData.CastDelayTime + 0.1f)
            {
                CastW(enemy);
                return true;
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

        if (_useQInCombo.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.Q) && CastQ(target))
        {
            return true;
        }

        if (_useWInCombo.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.W) && CastW(target))
        {
            return true;
        }

        if (_useEInCombo.Toggled && distance <= _localPlayer.Q.Range && CanCast(_localPlayer.E) && CastE(target))
        {
            return true;
        }

        return false;
    }

    private bool Harass()
    {
        if (_scriptingState.IsCombo == false)
        {
            return false;
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

        if (_useEInHarass.Toggled && CanCast(_localPlayer.E))
        {
            var target = _targetSelector.GetTarget(_localPlayer.E.Range);
            if (target != null && CastE(target))
            {
                return true;
            }
        }

        return false;
    }

    private bool CanCast(ISpell spell)
    {
        return spell.IsReady &&
               spell.ManaCost < _localPlayer.Mana &&
               !(_localPlayer.ActiveCastSpell.IsActive && _localPlayer.ActiveCastSpell.Type != ActiveSpellType.AutoAttack);
    }

    private bool CastQ(IHero target)
    {
        var spell = _localPlayer.Q;
        if (spell.SpellData == null) return false;
        
        var prediction = _prediction.PredictPosition(
            target,
            _localPlayer.Position,
            spell.SpellData.CastDelayTime,
            spell.SpellData.Speed,
            spell.SpellData.Width,
            spell.SpellData.Range,
            _QReactionTime.Value / 1000,
            0.0f,
            CollisionType.None,
            PredictionType.Line);

        if (prediction.HitChance < _QHitChance.Value)
        {
            return false;
        }
        
        _gameInput.CastSpell(spell.SpellSlot, prediction.Position);
        return true;
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

        float width = 60;
        if (_trapManager.GetAllyTraps(target.Position, 200).Where(x => x.ObjectNameHash == _trapNameHash).Any())
        {
            return false;
        }

        var trapDelay = 0.1f;
        var prediction = _prediction.PredictPosition(
            target,
            _localPlayer.Position,
            spell.SpellData.CastDelayTime + trapDelay,
            spell.SpellData.Speed,
            width,
            spell.SpellData.Range,
            _WReactionTime.Value / 1000,
            10.0f,
            CollisionType.None,
            PredictionType.Point);

        if (prediction.HitChance < _WHitChance.Value)
        {
            return false;
        }
        
        _gameInput.CastSpell(spell.SpellSlot, prediction.Position);
        return true;
    }

    private bool CastE(IHero target)
    {
        var spell = _localPlayer.E;
        if (spell.SpellData == null) return false;

        var prediction = _prediction.PredictPosition(
            target,
            _localPlayer.Position,
            spell.SpellData.CastDelayTime,
            spell.SpellData.Speed,
            spell.SpellData.Width,
            spell.SpellData.Range,
            _EReactionTime.Value / 1000,
            0.0f,
            CollisionType.Minion,
            PredictionType.Line);

        if (prediction.HitChance < _EHitChance.Value)
        {
            return false;
        }
        
        _gameInput.CastSpell(spell.SpellSlot, prediction.Position);
        return true;
    }

    public void OnRender(float deltaTime)
    {

    }
}