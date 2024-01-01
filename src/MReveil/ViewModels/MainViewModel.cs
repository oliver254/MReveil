using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TimerManager _timerManager;
    [ObservableProperty]
    public bool _alarm;

    [ObservableProperty]
    public SettingsViewModel _settings;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    public IState _state;


    public MainViewModel(TimerManager timerManager, SettingsViewModel settingsViewModel)
    {
        //WeakReferenceMessenger.Default.Register<MainViewModel, DurationSetMessage>(this, (r, m) => r.Duration = m.Value);
        _timerManager = timerManager;
        _settings = settingsViewModel;
        _state = _timerManager.State;
        _timerManager.PropertyChanged += TimerManager_PropertyChanged;
    }

    [RelayCommand]
    public void Pause()
    {
        _timerManager.Pause();
    }

    [RelayCommand]
    public void SetDuration(ActivityType activityType)
    {
        switch (activityType)
        {
            case ActivityType.Pomodoro:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settings.SprintDuration));
                    break;
                }
            case ActivityType.LongBreak:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settings.LongBreakDuration));
                    break;
                }
            case ActivityType.ShortBreak:
                {
                    _timerManager.Play(TimeSpan.FromMinutes(_settings.ShortBreakDuration));
                    break;
                }
            default:
                {
                    break;
                }
        }
        State = _timerManager.State;
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    public void Stop()
    {
        _timerManager.Stop();
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
            switch (e.PropertyName)
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