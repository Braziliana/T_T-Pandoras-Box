using System;
using System.Collections.Generic;
using System.Windows;
using LCU;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using T_T_Launcher.Data;

namespace T_T_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            services.AddSingleton(settings);
            
            services.AddSingleton<UnitDataDictionary>();
            services.AddSingleton<SpellDataDictionary>();
            services.AddSingleton<MissileDataDictionary>();
            services.AddSingleton<LcuClient>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Lobby>();
            services.AddSingleton<GameData>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}