using MReveil.Services;

namespace MReveil
{
    public partial class App : Application
    {
        public App(ThemeService themeService)
        {
            themeService.Initialize();
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}