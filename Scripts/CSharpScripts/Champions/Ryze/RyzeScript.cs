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
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;
    private readonly IGameCamera _gameCamera;
    
    public RyzeScript( IMainMenu mainMenu,
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
    }

    public void OnRender(float deltaTime)
    {
    }
}