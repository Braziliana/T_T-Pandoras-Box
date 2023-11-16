using System.Globalization;
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
    private readonly int trapNameHash = "CaitlynTrap".GetHashCode();

    private IToggle _useQInCombo;
    private IToggle _useWInCombo;
    private IToggle _useEInCombo;

    private IValueSlider _QHitChance;
    private IValueSlider _WHitChance;
    private IValueSlider _EHitChance;


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
        ITrapManager trapManager)
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
    }

    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Caitlin", ScriptType.Champion);
        var comboMenu = _menu.AddSubMenu("Combo");
        _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
        _useWInCombo = comboMenu.AddToggle("Use W in combo", true);
        _useEInCombo = comboMenu.AddToggle("Use E in combo", true);

        var hitchanceMenu = _menu.AddSubMenu("hit chcnce");
        _QHitChance = hitchanceMenu.AddFloatSlider("Q hit chcance", 0.8f, 0.0f, 1.0f, 0.05f, 2);
        _WHitChance = hitchanceMenu.AddFloatSlider("W hit chcance", 0.9f, 0.0f, 1.0f, 0.05f, 2);
        _EHitChance = hitchanceMenu.AddFloatSlider("E hit chcance", 0.9f, 0.0f, 1.0f, 0.05f, 2);
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
        if (_scriptingState.IsCombo == false)
        {
            return;
        }

        if (_useQInCombo.Toggled && CanCast(_localPlayer.Q))
        {
            var target = _targetSelector.GetTarget(_localPlayer.Q.Range);
            if (target != null && CastQ(target))
            {
                return;
            }
        }

        if (_useWInCombo.Toggled && CanCast(_localPlayer.W))
        {
            var target = _targetSelector.GetTarget(_localPlayer.W.Range);
            if (target != null && CastW(target))
            {
                return;
            }
        }

        if (_useEInCombo.Toggled && CanCast(_localPlayer.E))
        {
            var target = _targetSelector.GetTarget(_localPlayer.E.Range);
            if (target != null && CastE(target))
            {
                return;
            }
        }
        
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
            0.1f,
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

        float width = 80;
        if (_trapManager.GetAllyTraps(target.Position, width*2).Where(x => x.ObjectNameHash == trapNameHash).Any())
        {
            return false;
        }

        var prediction = _prediction.PredictPosition(
            target,
            _localPlayer.Position,
            spell.SpellData.CastDelayTime,
            spell.SpellData.Speed,
            width,
            spell.SpellData.Range,
            0.15f,
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
            0.1f,
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