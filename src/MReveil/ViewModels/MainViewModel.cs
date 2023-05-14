using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SettingsService _settingsService;
        [ObservableProperty]
        private TimeSpan? _duration;

        public MainViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [RelayCommand]
        public void SetDuration(ActivityType activityType)
        {
            switch(activityType)
            {
                case ActivityType.Pomodoro:
                    {
                        Duration = TimeSpan.FromSeconds(30);
                        break;
                    }
                case ActivityType.LongBreak:
                    {
                        Duration = TimeSpan.FromMinutes(_settingsService.LongBreak);
                        break;                            
                    }
                case ActivityType.ShortBreak:
                    {
                        Duration = TimeSpan.FromMinutes(_settingsService.ShortBreak);
                        break;
                    }
                default:
                    {
                        Duration = null;
                        break;
                    }
            }
        }

    }
}
