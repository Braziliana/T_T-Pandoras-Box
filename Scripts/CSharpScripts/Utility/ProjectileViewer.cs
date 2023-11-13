using Api;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Internal.Game.Managers;
using Api.Menus;
using Api.Scripts;

namespace Scripts.CSharpScripts.Utility;

public class ProjectileViewer : IScript
{
    public string Name => "ProjectileViewer";
    public ScriptType ScriptType => ScriptType.Utility;
    public bool Enabled { get; set; }

    private readonly IMissileManager _missileManager;
    private readonly IHeroManager _heroManager;
    private readonly ILocalPlayer _localPlayer;
    private readonly IRenderer _renderer;
    private readonly IGameState _gameState;

    private readonly IToggle _showEnemyMissiles;

    public ProjectileViewer(
        IMainMenu mainMenu,
        IMissileManager missileManager,
        IHeroManager heroManager,
        ILocalPlayer localPlayer,
        IRenderer renderer,
        IGameState gameState)
    {
        _missileManager = missileManager;
        _heroManager = heroManager;
        _localPlayer = localPlayer;
        _renderer = renderer;
        _gameState = gameState;

        var menu = mainMenu.CreateMenu("Projectile Viewer", ScriptType.Utility);
        _showEnemyMissiles = menu.AddToggle("Show enemy missiles", true);
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
        if (!_showEnemyMissiles.Toggled)
        {
            return;
        }
        
        foreach (var missile in _missileManager.GetMissiles().Where(x => x.DestinationIndex == 0))
        {
            var hero = _heroManager.GetHero(missile.SourceIndex);
            if(hero is null || hero.Team == _localPlayer.Team) continue;
            
            //_renderer.RectBorder(missile.StartPosition, missile.EndPosition, Color.Red, missile.Width);
        }
    }
}