using Api;
using Api.Game.Managers;
using Api.GameProcess;
using Api.Scripts;
using NativeWarper;

namespace T_T_PandorasBox;

public class Overlay : IDisposable
{
    private readonly AppWindow _appWindow;
    private readonly IGameManager _gameManager;
    private readonly IScriptManager _scriptManager;
    private readonly IRenderer _renderer;
    private readonly ITargetProcess _targetProcess;
    
    public Overlay(AppWindow appWindow, IGameManager gameManager, IScriptManager scriptManager, IRenderer renderer, ITargetProcess targetProcess)
    {
        _appWindow = appWindow;
        _gameManager = gameManager;
        _scriptManager = scriptManager;
        _renderer = renderer;
        _targetProcess = targetProcess;
        _appWindow.Create();
        _renderer.Init();
        _appWindow.OnUpdate += AppWindowOnUpdate;
        _renderer.OnRender += AppWindowOnRender;
        _targetProcess.Hook();
    }

    private void AppWindowOnUpdate(float deltaTime)
    {
        _gameManager.Update(deltaTime);
        _renderer.SetProjectionViewMatrix(_gameManager.GameCamera.ViewProjMatrix);
        _scriptManager.Update(deltaTime);
    }

    private void AppWindowOnRender(float deltaTime)
    {
        _scriptManager.Render(deltaTime);
    }

    public void Run()
    {
        _appWindow.Run();
    }
    
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _appWindow.Dispose();
            _gameManager.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Overlay()
    {
        Dispose(false);
    }
}