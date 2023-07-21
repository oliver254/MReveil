﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Monbsoft.MReveil.Messaging;
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
            WeakReferenceMessenger.Default.Register<MainViewModel, DurationSetMessage>(this, (r, m) => r.Duration = m.Value);
            _settingsService = settingsService;

        }       

        [RelayCommand]
        public void SetDuration(ActivityType activityType)
        {
            switch(activityType)
            {
                case ActivityType.Pomodoro:
                    {
                        Duration = TimeSpan.FromMinutes(_settingsService.Sprint);
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
