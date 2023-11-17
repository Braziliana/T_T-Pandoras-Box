using Api.GameProcess;
using Api.Inputs;
using Api.Menus;
using Api.Scripts;
using Api.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scripts.CSharpScripts;
using Scripts.CSharpScripts.Champions.Caitlyn;
using Scripts.CSharpScripts.Champions.Ezreal;
using Scripts.CSharpScripts.Champions.Ryze;
using Scripts.CSharpScripts.Orbwlakers;
using Scripts.CSharpScripts.Utility;
using Scripts.LuaScripts;
using Scripts.Utils;

namespace Scripts;

public static class ScriptsServiceInstaller
{
    public static void InstallServices(IServiceCollection collection)
    {
        collection.AddTransient<Utils.Timer>();
        collection.TryAddSingleton<IScriptingState, ScriptingState>();
        collection.TryAddSingleton<IMinionSelector, MinionSelector>();
        collection.TryAddSingleton<ITargetSelector, CurrentTargetSelector>();
        InstallCSharpScripts(collection);
        collection.AddSingleton<IScriptProvider, CSharpScriptProvider>();
        collection.AddSingleton<IScriptProvider, LuaScriptProvider>();
        collection.TryAddSingleton<IScriptManager, ScriptManager>();
        collection.AddSingleton<LuaBinder>();
    }

    private static void InstallCSharpScripts(IServiceCollection collection)
    {
        collection.AddSingleton<IScript, OrbWalker>();
        collection.AddSingleton<IScript, Tracker>();
        collection.AddSingleton<IScript, AutoSmite>();
        collection.AddSingleton<IScript, CaitlynScript>();
        collection.AddSingleton<IScript, EzrealScript>();
        collection.AddSingleton<IScript, RyzeScript>();
        collection.AddSingleton<IScript, ProjectileViewer>();
        collection.AddSingleton<IScript, TargetSelector>();
    }
}