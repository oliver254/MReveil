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

            // view models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();

            // services
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<TimerManager>();
            builder.Services.AddSingleton<SettingsService>();

            return builder.Build();
        }
    }
}