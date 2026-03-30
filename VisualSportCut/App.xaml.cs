using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Services;
using VisualSportCut.Infrastructure.Services;
using VisualSportCut.Presentation.ViewModels;

namespace VisualSportCut
{

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
            services.AddSingleton<IJsonLoader, JsonLoader>();

            services.AddSingleton<IStatisticService, StatisticService>();

            services.AddTransient<MainViewModel>();

            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
