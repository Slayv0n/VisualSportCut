using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
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
            // 1. Регистрируем JsonLoader
            // Singleton: экземпляр создается один раз и переиспользуется (рекомендуется для файловых сервисов)
            services.AddSingleton<IJsonLoader, JsonLoader>();

            // 2. Регистрируем остальные сервисы
            services.AddSingleton<IStatisticService, StatisticService>();

            // 3. Регистрируем ViewModels (обычно Transient, чтобы создавать новые для окон)
            services.AddTransient<MainViewModel>();

            // 4. Регистрируем главное окно
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Получаем MainWindow из DI (вместе с внедренной ViewModel)
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
