using Api;
using Api.GameProcess;
using Api.Inputs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Internal;
using Api.Menus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NativeWarper;
using NativeWarper.Menus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Scripts;
using T_T_PandorasBox;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: false); 
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .ConfigureServices((hostContext, collection) =>
    {
        collection.AddLogging();
        collection.TryAddSingleton(hostContext.Configuration);
        
        collection.Configure<TargetProcessData>(hostContext.Configuration.GetSection("TargetProcessData"));
     
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        collection.AddSingleton(settings);
        collection.AddSingleton<AppWindow>();
        collection.AddSingleton<ITargetProcess, GameProcess>();
        collection.AddSingleton<IRenderer, Renderer>();
        collection.AddSingleton<IInputManager, InputManager>();
        collection.AddSingleton<IMainMenu, MainMenu>();
        collection.AddSingleton<Overlay>();
        InternalServiceInstaller.InstallServices(collection);
        ScriptsServiceInstaller.InstallServices(collection);
        
    })
    .Build();

if(args.Any(x => x == "Hybrid=true"))
{
    var process = host.Services.GetRequiredService<ITargetProcess>();
    process.SetTargetProcessName("League of Legends.exe");
    process.Hook();
    if (process.LoadDll("T_THybrid.dll"))
    {
        Console.WriteLine("Dll Loaded");
    }
    else
    {
        Console.WriteLine("Failed to load dll");
    }
}

var overlay = host.Services.GetRequiredService<Overlay>();
overlay.Run();

// var appStateManager = host.Services.GetRequiredService<AppStateManager>();
// appStateManager.Run();
//
// while (!appStateManager.ShouldExit)
// {
//     appStateManager.Update();
// }
//
// GlobalKeyboardHook.Unhook();
// GlobalMouseHook.Unhook();