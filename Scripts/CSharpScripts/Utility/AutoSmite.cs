using System.Numerics;
using Api;
using Api.Game.GameInputs;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Menus;
using Api.Scripts;

namespace Scripts.CSharpScripts.Utility;

public class AutoSmite : IScript
{
    private class MonsterSettings
    {
        public MonsterType MonsterType { get; set; }
        public IToggle SmiteToggle { get; set; }
        public bool HasExtraSettings { get; set; }
        public IToggle? RestrictSmite { get; set; }
        public bool IsEnabledByDefault { get; set; }
    }
    
    public string Name => "AutoSmite";
    public ScriptType ScriptType => ScriptType.Utility;
    public bool Enabled { get; set; }

    private readonly MonsterSettings[] _monsterSettings = new MonsterSettings[]
    {
        new MonsterSettings
        {
            MonsterType = MonsterType.Baron,
            HasExtraSettings = false,
        },
        new MonsterSettings
        {
            MonsterType = MonsterType.Herald,
            HasExtraSettings = false,
        },
        new MonsterSettings
        {
            MonsterType = MonsterType.Dragon,
            HasExtraSettings = false,
        },
        new MonsterSettings
        {
            MonsterType = MonsterType.Crab,
            HasExtraSettings = true,
        },
        new MonsterSettings
        {
            MonsterType = MonsterType.Blue,
            HasExtraSettings = true,
        },
        new MonsterSettings
        {
            MonsterType = MonsterType.Red,
            HasExtraSettings = true,
        },
    };

    private readonly ILocalPlayer _localPlayer;
    private readonly IMonsterManager _monsterManager;
    private readonly IHeroManager _heroManager;
    private readonly IGameInput _gameInput;
    private readonly IObjectManager _objectManager;
    private readonly IRenderer _renderer;
    private readonly IGameCamera _gameCamera;
    private readonly IToggle _drawKillableMonsters;
    private readonly int _smiteHash = "SummonerSmite".GetHashCode();
    
    public AutoSmite(
        IMainMenu mainMenu,
        ILocalPlayer localPlayer,
        IMonsterManager monsterManager,
        IHeroManager heroManager,
        IGameInput gameInput,
        IObjectManager objectManager,
        IRenderer renderer,
        IGameCamera gameCamera)
    {
        _localPlayer = localPlayer;
        _monsterManager = monsterManager;
        _heroManager = heroManager;
        _gameInput = gameInput;
        _objectManager = objectManager;
        _renderer = renderer;
        _gameCamera = gameCamera;

        var menu = mainMenu.CreateMenu("AutoSmite", ScriptType.Utility);

        _drawKillableMonsters = menu.AddToggle("Draw monsters", true);
        for (var i = 0; i < _monsterSettings.Length; i++)
        {
            var ms = _monsterSettings[i];
            var subMenu = menu.AddSubMenu(ms.MonsterType.ToString());
            _monsterSettings[i].SmiteToggle =
                subMenu.AddToggle("Smite", true);

            if (_monsterSettings[i].HasExtraSettings)
            {
                _monsterSettings[i].RestrictSmite =
                    subMenu.AddToggle("Restrict smite", true);
            }
        }
    }
    
    private bool CanSmite(MonsterType monsterType, ISpell spell)
    {
        for (var i = 0; i < _monsterSettings.Length; i++)
        {
            var ms = _monsterSettings[i];
            if (ms.MonsterType != monsterType) continue;
            if (ms.RestrictSmite is null || !ms.RestrictSmite.Toggled) return ms.SmiteToggle.Toggled;
            return spell.Stacks > 1 || _heroManager.GetEnemyHeroes(1000).Any();
        }

        return false;
    }

    private ISpell? GetSmite()
    {
        if (_localPlayer.Summoner1.NameHash == _smiteHash)
        {
            return _localPlayer.Summoner1;
        }
        
        if (_localPlayer.Summoner2.NameHash == _smiteHash)
        {
            return _localPlayer.Summoner2;
        }

        return null;
    }
    
    public void OnLoad()
    {
    }

    public void OnUnload()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        var smite = GetSmite();
        if (smite is null || !smite.SmiteIsReady)
        {
            return;
        }
     
        var range = 500 + _localPlayer.CollisionRadius;
        var monsters = _monsterManager.GetMonsters(range);
        foreach (var monster in monsters)
        {
            if (CanSmite(monster.MonsterType, smite) && monster.Health <= smite.Damage)
            {
                _gameInput.CastSpell(smite.SpellSlot, monster);
            }
        }
    }

    public void OnRender(float deltaTime)
    {
        if (!_drawKillableMonsters.Toggled)
        {
            return;
        }
        
        var smite = GetSmite();
        if (smite is null || !smite.SmiteIsReady)
        {
            return;
        }
        var range = 500 + _localPlayer.CollisionRadius;
        var monsters = _monsterManager.GetMonsters(range);
        foreach (var monster in monsters)
        {
            if (monster.Health <= smite.Damage)
            {
                _renderer.CircleBorder3D(monster.Position, monster.CollisionRadius, Color.Magenta, 1);
            }
        }
    }
}