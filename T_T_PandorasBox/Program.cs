using Api;
using Api.GameProcess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Scripts;
using T_T_PandorasBox;
using T_T_PandorasBox.Rendering;
using T_T_PandorasBox.States;
using T_T_PandorasBox.States.MainWindowViews;
using WinApi;

void RegisterMainWindows(IServiceCollection collection)
{
    collection.AddScoped<IMainWindowView, MainWindowTosView>();
    collection.AddScoped<IMainWindowView, MainWindowLobbyView>();
    collection.AddScoped<IMainWindowView, MainWindowDataView>();
}

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
        collection.AddSingleton<IRenderer, Renderer>();
        InternalServiceInstaller.InstallServices(collection);
        ScriptsServiceInstaller.InstallServices(collection);

        RegisterMainWindows(collection);
        collection.AddSingleton<InGameAppState>();
        collection.AddSingleton<AppStateManager>();
        collection.AddSingleton<MainAppState>();
    })
    .Build();

var appStateManager = host.Services.GetRequiredService<AppStateManager>();
appStateManager.Run();

while (!appStateManager.ShouldExit)
{
    appStateManager.Update();
}

GlobalKeyboardHook.Unhook();
GlobalMouseHook.Unhook();