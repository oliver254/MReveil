using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MReveil.Models;

namespace MReveil.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private TimeSpan? _duration;

        [ObservableProperty]
        DateTime? _endTime;

        [RelayCommand]
        public void SetDuration(ActivityType activityType)
        {
            switch(activityType)
            {
                case ActivityType.Pomodoro:
                    {
                        _duration = TimeSpan.FromMinutes(45);
                        break;
                    }
                case ActivityType.LongBreak:
                    {
                        _duration = TimeSpan.FromMinutes(15);
                        break;                            
                    }
                case ActivityType.ShortBreak:
                    {
                        _duration = TimeSpan.FromMinutes(5);
                        break;
                    }
                default:
                    {
                        _duration = null;
                        break;
                    }
            }
        }

        [RelayCommand]
        public void Start()
        {
            if(_duration == null)
            {
                EndTime = null;
            }
            else
            {
                EndTime = DateTime.Now.Add(_duration.Value);
            }
        }

    }
}
