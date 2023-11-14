using System.Globalization;
using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
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

    private IToggle _useQInCombo;
    
    public CaitlynScript(
        IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        IGameInput gameInput,
        IRenderer renderer,
        IGameState gameState,
        IGameCamera gameCamera)
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
    }

    public void OnLoad()
    {
        _menu = _mainMenu.CreateMenu("Caitlin", ScriptType.Champion);
        var comboMenu = _menu.AddSubMenu("Combo");
        _useQInCombo = comboMenu.AddToggle("Use Q in combo", true);
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

        CastSpell(_localPlayer.Q);
        CastSpell(_localPlayer.W);
        CastSpell(_localPlayer.E);
        //CastSpell(_localPlayer.R);
    }

    private void CastSpell(ISpell spell)
    {
        if (spell.IsReady && spell.ManaCost < _localPlayer.Mana && !(_localPlayer.ActiveCastSpell.IsActive &&
                                                                     _localPlayer.ActiveCastSpell.Type !=
                                                                     ActiveSpellType.AutoAttack))
        {
            var target = _targetSelector.GetTarget(spell.Range);
            if (target == null)
            {
                return;
            }

            var prediction = _prediction.PredictPosition(target, _localPlayer.Position, spell.SpellData.CastTime,
                spell.SpellData.Speed, spell.SpellData.Width, spell.SpellData.Range);
            if (prediction.HitChance > 20)
            {
                _gameInput.CastSpell(spell.SpellSlot, prediction.Position);
            }
        }
    }

    public void OnRender(float deltaTime)
    { 
        // if (_gameCamera.WorldToScreen(_localPlayer.Position, out var sp))
        // {
        //     _renderer.Text(_localPlayer.IsDead ? "Dead" : "Alive", sp, 18, Color.Cyan);
        // }
        var target = _targetSelector.GetTarget(_localPlayer.Q.Range);
        if (target == null)
        {
            return;
        }
        
        var qSpell = _localPlayer.Q;
        var prediction = _prediction.PredictPosition(target, _localPlayer.Position, qSpell.SpellData.CastDelayTime, qSpell.SpellData.Speed, qSpell.SpellData.Width, qSpell.SpellData.Range);
        //if (prediction.HitChance > 50)
        {
               _renderer.CircleBorder3D(prediction.Position, 120, Color.Cyan, 1);
            
             if (_gameCamera.WorldToScreen(prediction.Position, out var ps))
             {
                 _renderer.Text(prediction.HitChance.ToString(CultureInfo.InvariantCulture), ps, 21, Color.Cyan);
             }
        }
    }
}