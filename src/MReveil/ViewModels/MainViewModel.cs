using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TimerManager _timerManager;
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    public bool _alarm;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    public IState _state;

    public MainViewModel(TimerManager timerManager, SettingsService settingsService)
    {
        //WeakReferenceMessenger.Default.Register<MainViewModel, DurationSetMessage>(this, (r, m) => r.Duration = m.Value);
        _timerManager = timerManager;
        _settingsService = settingsService;
        _state = _timerManager.State;
        _timerManager.PropertyChanged += TimerManager_PropertyChanged;
    }

    [RelayCommand]
    public void ActivateAlarm()
    {

    }

    [RelayCommand]
    public void Pause()
    {
        _timerManager.Pause();
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    public void Stop()
    {
        _timerManager.Stop();
    }

    [RelayCommand]
    public void SetDuration(ActivityType activityType)
    {
        switch (activityType)
        {
            case ActivityType.Pomodoro:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settingsService.Sprint));
                    break;
                }
            case ActivityType.LongBreak:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settingsService.LongBreak));
                    break;
                }
            case ActivityType.ShortBreak:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settingsService.ShortBreak));
                    break;
                }
            default:
                {
                    break;
                }
        }
        State = _timerManager.State;
    }

    private bool CanStop()
    {
        return State is CountdownState;
    }
    private void TimerManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        TimerManager timerManager = sender as TimerManager;
        if (timerManager is not null)
        {
            switch(e.PropertyName)
            {
                case nameof(State):
                    {
                        State = timerManager.State;
                        break;
                    }
            }
            if (e.PropertyName == nameof(State))
            {
                State = timerManager.State;
            }
        }
    }
}