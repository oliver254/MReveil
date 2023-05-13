using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MReveil.Models;

namespace MReveil.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        TimeSpan? _duration;

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
                        Duration = TimeSpan.FromMinutes(15);
                        break;                            
                    }
                case ActivityType.ShortBreak:
                    {
                        Duration = TimeSpan.FromMinutes(5);
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
