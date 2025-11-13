using CommunityToolkit.Maui;
using Monbsoft.MReveil.Services;
using Monbsoft.MReveil.ViewModels;
using Monbsoft.MReveil.Views;

namespace Monbsoft.MReveil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<JournalPage>();
            builder.Services.AddSingleton<StatisticsPage>();

            // view models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<JournalViewModel>();
            builder.Services.AddSingleton<StatisticsViewModel>();

            // services
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<TimerManager>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<StatisticsService>();
            builder.Services.AddSingleton<PomodoroSessionService>();

            // Initialize database
            var app = builder.Build();
            InitializeDatabase(app.Services);

            return app;
        }

        private static void InitializeDatabase(IServiceProvider services)
        {
            var databaseService = services.GetRequiredService<DatabaseService>();
            databaseService.InitializeAsync().Wait();
        }
    }
}