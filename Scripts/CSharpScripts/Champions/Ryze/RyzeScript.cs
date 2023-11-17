using System.Globalization;
using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Menus;
using Api.Scripts;
using Scripts.Utils;


namespace Scripts.CSharpScripts.Champions.Ryze;

public class RyzeScript : IChampionScript
{
    public string Name => "Ryze";

    public string Champion => "Ryze";
    public ScriptType ScriptType => ScriptType.Champion;
    public bool Enabled { get; set; }

    private readonly IMainMenu _mainMenu;
    private IMenu? _menu = null;
    private readonly ILocalPlayer _localPlayer;
    private readonly IScriptingState _scriptingState;
    private readonly ITargetSelector _targetSelector;
    private readonly IPrediction _prediction;
    private readonly IGameInput _gameInput;
    private readonly ISpellCaster _spellCaster;

    public RyzeScript(IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        IGameInput gameInput,
        ISpellCaster spellCaster)
    {
        _mainMenu = mainMenu;
        _localPlayer = localPlayer;
        _scriptingState = scriptingState;
        _targetSelector = targetSelector;
        _prediction = prediction;
        _gameInput = gameInput;
        _spellCaster = spellCaster;
    }

    private IToggle _useQInCombo;
    private IToggle _useWInCombo;
    private IToggle _useEInCombo;

    private IToggle _useQInHaras;
    private IValueSlider _QMinManaHaras;
    private IToggle _useWInHaras;
    private IValueSlider _WMinManaHaras;
    private IToggle _useEInHaras;
    private IValueSlider _EMinManaHaras;

    private IValueSlider _QHitChance;
    private IValueSlider _QReactionTime;

    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Ryze", ScriptType.Champion);

        var comboMenu = _menu.AddSubMenu("Combo");
        _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
        _useWInCombo = comboMenu.AddToggle("Use W in combo", true);
        _useEInCombo = comboMenu.AddToggle("Use E in combo", true);

        var harassMenu = _menu.AddSubMenu("Haras");
        _useQInHaras = harassMenu.AddToggle("Use Q in haras", true);
        _QMinManaHaras = harassMenu.AddFloatSlider("Q min mana percent", 50.0f, 0.0f, 100.0f, 5f, 0);
        _useWInHaras = harassMenu.AddToggle("Use W in haras", true);
        _WMinManaHaras = harassMenu.AddFloatSlider("W min mana percent", 50.0f, 0.0f, 100.0f, 5f, 0);
        _useEInHaras = harassMenu.AddToggle("Use E in haras", true);
        _EMinManaHaras = harassMenu.AddFloatSlider("E min mana percent", 50.0f, 0.0f, 100.0f, 5f, 0);

        var hitchanceMenu = _menu.AddSubMenu("hit chcnce");
        _QHitChance = hitchanceMenu.AddFloatSlider("Q hit chcance", 0.8f, 0.0f, 1.0f, 0.05f, 2);
        _QReactionTime = hitchanceMenu.AddFloatSlider("Q reaction time", 50f, 0.0f, 300f, 5f, 2);
    }

    public void OnUnload()
    {
        if (_menu is not null)
        {
            _mainMenu.RemoveMenu(_menu);
        }
    }

    private float[] spellRanges = new float[3];

    private float GetMinSpellRange()
    {
        if (_localPlayer.Q.IsReady)
        {
            spellRanges[0] = _localPlayer.Q.Range;
        }
        else
        {
            spellRanges[0] = float.MaxValue;
        }

        if (_localPlayer.W.IsReady)
        {
            spellRanges[1] = _localPlayer.W.Range;
        }
        else
        {
            spellRanges[1] = float.MaxValue;
        }

        if (_localPlayer.E.IsReady)
        {
            spellRanges[2] = _localPlayer.E.Range;
        }
        else
        {
            spellRanges[2] = float.MaxValue;
        }

        return spellRanges.Min();
    }

    public void OnUpdate(float deltaTime)
    {
        if (!_localPlayer.IsAlive)
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

    private bool Combo()
    {
        if (_scriptingState.IsCombo == false)
        {
            return false;
        }

        var target = _targetSelector.GetTarget(GetMinSpellRange());
        if (target is null)
        {
            return false;
        }

        if (_useEInCombo.Toggled && CanCast(_localPlayer.E) && CastE(target))
        {
            return true;
        }

        if (_useWInCombo.Toggled && CanCast(_localPlayer.W) && CastW(target))
        {
            return true;
        }

        if (_useQInCombo.Toggled && CanCast(_localPlayer.Q) && CastQ(target))
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

        var target = _targetSelector.GetTarget(GetMinSpellRange());
        if (target is null)
        {
            return false;
        }

        if (_useEInHaras.Toggled && CanCast(_localPlayer.E) && _localPlayer.ManaPercent >= _EMinManaHaras.Value &&
            CastE(target))
        {
            return true;
        }

        if (_useWInHaras.Toggled && CanCast(_localPlayer.W) && _localPlayer.ManaPercent >= _WMinManaHaras.Value &&
            CastW(target))
        {
            return true;
        }

        if (_useQInHaras.Toggled && CanCast(_localPlayer.Q) && _localPlayer.ManaPercent >= _QMinManaHaras.Value &&
            CastQ(target))
        {
            return true;
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
            _QHitChance.Value, CollisionType.Minion, PredictionType.Line);
    }

    private bool CastW(IHero target)
    {
        return _spellCaster.TryCast(_localPlayer.W, target);
    }

    private bool CastE(IHero target)
    {
        return _spellCaster.TryCast(_localPlayer.E, target);
    }

    public void OnRender(float deltaTime)
    {
    }
}