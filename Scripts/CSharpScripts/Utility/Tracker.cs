using Api;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Menus;
using Api.Scripts;

namespace Scripts.CSharpScripts.Utility;

public class Tracker : IScript
{
    public string Name => "Tracker";
    public ScriptType ScriptType => ScriptType.Utility;
    public bool Enabled { get; set; }

    private readonly ITurretManager _turretManager;
    private readonly IHeroManager _heroManager;
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;
    private readonly ILocalPlayer _localPlayer;
    private readonly IGameCamera _gameCamera;
    private readonly IObjectManager _objectManager;

    private readonly IToggle _showAllyAutoAttacksRange;
    private readonly IToggle _showEnemyAutoAttacksRange;
    
    private readonly IToggle _showAllyTurretRange;
    private readonly IToggle _showEnemyTurretRange;
    
    private readonly IToggle _showAllyPath;
    private readonly IToggle _showEnemyPath;
    
    private readonly IToggle _showEnemyWards;
    private readonly IToggle _showEnemyTraps;
    
    public Tracker(
        IMainMenu mainMenu,
        ITurretManager turretManager,
        IHeroManager heroManager,
        IRenderer renderer,
        IGameState gameState,
        ILocalPlayer localPlayer,
        IGameCamera gameCamera,
        IObjectManager objectManager)
    {
        _turretManager = turretManager;
        _heroManager = heroManager;
        _renderer = renderer;
        _gameState = gameState;
        _localPlayer = localPlayer;
        _gameCamera = gameCamera;
        _objectManager = objectManager;

        var menu = mainMenu.CreateMenu("Tracker", ScriptType.Utility);
        var subMenuRanges = menu.AddSubMenu("Ranges");
        
        _showAllyAutoAttacksRange = subMenuRanges.AddToggle("Ally range indicator", false);
        _showEnemyAutoAttacksRange = subMenuRanges.AddToggle("Enemy range indicator", true);
        _showAllyTurretRange = subMenuRanges.AddToggle("Ally turret range indicator", false);
        _showEnemyTurretRange = subMenuRanges.AddToggle("Enemy turret range indicator", true);
        
        
        var subMenuPaths = menu.AddSubMenu("Paths");
        _showAllyPath = subMenuPaths.AddToggle("Ally path indicator", false);
        _showEnemyPath = subMenuPaths.AddToggle("Enemy path indicator", false);

        _showEnemyWards = menu.AddToggle("Enemy wards indicator", true);
        _showEnemyTraps = menu.AddToggle("Enemy traps indicator", true);
    }
    
    public void OnLoad()
    {
    }

    public void OnUnload()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnRender(float deltaTime)
    {
        if (_showEnemyAutoAttacksRange.Toggled)
        {
            foreach (var hero in _heroManager.GetEnemyHeroes().Where(x => x.IsVisible && x.Distance(_localPlayer) <= 1200))
            {
                DrawRange(hero, Color.Red);
            }
        }
        if (_showAllyAutoAttacksRange.Toggled)
        {
            foreach (var hero in _heroManager.GetAllyHeroes().Where(x => !x.IsLocalHero && x.Distance(_localPlayer) <= 12000))
            {
                DrawRange(hero, Color.Blue);
            }
        }
        
        if (_showEnemyTurretRange.Toggled)
        {
            foreach (var turret in _turretManager.GetEnemyTurrets().Where(x => x.Distance(_localPlayer) <= x.AttackRange + 500))
            {
                DrawRange(turret, Color.Red);
            }
        }
        if (_showAllyTurretRange.Toggled)
        {
            foreach (var turret in _turretManager.GetAllyTurrets().Where(x => x.Distance(_localPlayer) <= x.AttackRange + 500))
            {
                DrawRange(turret, Color.Blue);
            }
        }
        
        if (_showEnemyPath.Toggled)
        {
            foreach (var hero in _heroManager.GetEnemyHeroes())
            {
                DrawPath(hero, Color.Red);
            }
        }
        if (_showAllyPath.Toggled)
        {
            foreach (var hero in _heroManager.GetAllyHeroes().Where(x => !x.IsLocalHero))
            {
                DrawPath(hero, Color.Blue);
            }
        }

        if (_showEnemyWards.Toggled)
        {
            foreach (var ward in _objectManager.WardManager.GetEnemyWards())
            {
                var color = ward.WardType switch
                {
                    WardType.Unknown => Color.Red,
                    WardType.Yellow => Color.Yellow,
                    WardType.Pink => Color.Magenta,
                    WardType.Blue => Color.Blue,
                    WardType.Crab => Color.Green,
                    _ => Color.Red
                };
                _renderer.CircleBorder3D(ward.Position, ward.CollisionRadius, color, 1);
            }
        }

        if (_showEnemyTraps.Toggled)
        {
            foreach (var enemyTrap in _objectManager.TrapManager.GetEnemyTraps())
            {
                _renderer.CircleBorder3D(enemyTrap.Position, 80, Color.Red, 5);

                if (_gameCamera.WorldToScreen(enemyTrap.Position, out var trapSp))
                {
                    _renderer.Text(enemyTrap.Name, trapSp, 21, Color.White);
                }
            }
        }
    }

    private void DrawRange(IAiBaseUnit unit, Color color)
    {
        _renderer.CircleBorder3D(unit.Position, unit.AttackRange, color, 1);
    }
    
    private void DrawPath(IHero hero, Color color)
    {
        // _renderer.RenderLines(hero.AiManager.RemainingPath, 1, color);
        // if (_gameCamera.WorldToScreen(hero.AiManager.TargetPosition, out var targetScreenPosition))
        // {
        //     _renderer.Text(hero.Name, targetScreenPosition, 21, color);
        // }
    }
}