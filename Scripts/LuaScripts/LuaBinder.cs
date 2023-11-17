using System.Numerics;
using Api;
using Api.Game.Calculations;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Inputs;
using Api.Menus;
using Api.Scripts;
using NLua;
using Scripts.Extensions;
using Scripts.Utils;

namespace Scripts.LuaScripts;

internal class LuaBinder
{
    private readonly IGameManager _gameManager;
    private readonly IRenderer _renderer;
    private readonly IInputManager _inputManager;
    private readonly IGameInput _gameInput;
    private readonly IDamageCalculator _damageCalculator;
    private readonly IDamagePrediction _damagePrediction;
    private readonly IMainMenu _mainMenu;
    
    private readonly IMinionSelector _minionSelector;
    private readonly IScriptingState _scriptingState;
    private readonly ITargetSelector _targetSelector;
    private readonly IPrediction _prediction;
    private readonly ISpellCaster _spellCaster;

    public LuaBinder(
        IGameManager gameManager,
        IRenderer renderer,
        IInputManager inputManager,
        IGameInput gameInput,
        IDamageCalculator damageCalculator,
        IDamagePrediction damagePrediction,
        IMainMenu mainMenu,
        IMinionSelector minionSelector,
        IScriptingState scriptingState,
        ITargetSelector targetSelector,
        IPrediction prediction,
        ISpellCaster spellCaster)
    {
        _gameManager = gameManager;
        _renderer = renderer;
        _inputManager = inputManager;
        _gameInput = gameInput;
        _damageCalculator = damageCalculator;
        _damagePrediction = damagePrediction;
        _mainMenu = mainMenu;
        _minionSelector = minionSelector;
        _scriptingState = scriptingState;
        _targetSelector = targetSelector;
        _prediction = prediction;
        _spellCaster = spellCaster;
    }

    public Lua Create()
    {
        var lua = new Lua();
        lua.LoadCLRPackage();
        lua["Renderer"] = _renderer;
        lua["DamageCalculator"] = _damageCalculator;
        lua["DamagePrediction"] = _damagePrediction;
        lua["GameCamera"] = _gameManager.GameCamera;
        lua["Hero"] = _gameManager.LocalPlayer;
        lua["GameState"] = _gameManager.GameState;
        lua["SpellCaster"] = _spellCaster;

        var objectManager = _gameManager.ObjectManager;
        lua["MinionManager"] = objectManager.MinionManager;
        lua["MonsterManager"] = objectManager.MonsterManager;
        lua["PlantManager"] = objectManager.PlantManager;
        lua["WardManager"] = objectManager.WardManager;
        lua["TrapManager"] = objectManager.TrapManager;
            
        lua["ObjectManager"] = objectManager;
        lua["TurretManager"] = _gameManager.TurretManager;
        lua["InhibitorManager"] = _gameManager.InhibitorManager;
        lua["MissileManager"] = _gameManager.MissileManager;
        lua["HeroManager"] = _gameManager.HeroManager;
        lua["GameInput"] = _gameInput;
        
        lua["MinionSelector"] = _minionSelector;
        lua["ScriptingState"] = _scriptingState;
        lua["TargetSelector"] = _targetSelector;
        lua["Prediction"] = _prediction;
        
        lua.RegisterFunction("Vector2", null, typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) }));
        lua.RegisterFunction("Color", null, typeof(Color).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) }));
            
        lua.RegisterFunction("Vector3", null, typeof(Vector3).GetConstructor(new[] { typeof(float), typeof(float), typeof(float) }));
            
        lua.DoString("Math = {}");
        lua["Math.Distance"] = (Func<Vector3, Vector3, float>) Vector3.Distance;
        lua["Math.Distance"] = (Func<Vector2, Vector2, float>) Vector2.Distance;
            
        lua.RegisterFunction("Distance", null, typeof(Vector3).GetMethod("Distance"));
        
        lua.RegisterEnum<MouseButton>();
        lua.RegisterEnum<VirtualKey>();
        lua.RegisterEnum<KeyState>();
        lua.RegisterEnum<IssueOrderType>();
        lua.RegisterEnum<DamageType>();
        lua.RegisterEnum<CollisionType>();
        lua.RegisterEnum<PredictionType>();
        
        BindMenu(lua);
        
        return lua;
    }

    private void BindMenu(Lua lua)
    {
        lua.RegisterEnum<ScriptType>();
        lua.RegisterEnum<HotkeyType>();
        lua.RegisterFunction("CreateMenu", _mainMenu, typeof(IMainMenu).GetMethod("CreateMenu"));
        lua.RegisterFunction("RemoveMenu", _mainMenu, typeof(IMainMenu).GetMethod("RemoveMenu"));
        
    }
}