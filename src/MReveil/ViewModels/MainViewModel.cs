using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Monbsoft.MReveil.Messaging;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TimerManager _timerManager;
    private readonly PomodoroSessionService _pomodoroSessionService;
    private ActivityType _currentActivityType;

    [ObservableProperty]
    public bool _alarm;

    [ObservableProperty]
    public SettingsViewModel _settings;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    public IState _state;


    public MainViewModel(TimerManager timerManager, SettingsViewModel settingsViewModel, PomodoroSessionService pomodoroSessionService)
    {
        _timerManager = timerManager;
        _settings = settingsViewModel;
        _pomodoroSessionService = pomodoroSessionService;
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
        WeakReferenceMessenger.Default.Send(new ResetAlarmMessage(true));
        _currentActivityType = activityType;
        
        int duration = activityType switch
        {
            ActivityType.Pomodoro => _settings.SprintDuration,
            ActivityType.LongBreak => _settings.LongBreakDuration,
            ActivityType.ShortBreak => _settings.ShortBreakDuration,
            _ => 25
        };

        _pomodoroSessionService.StartSession(activityType, duration);
        _timerManager.Play(TimeSpan.FromMinutes(duration));
        State = _timerManager.State;
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    public async void Stop()
    {
        WeakReferenceMessenger.Default.Send(new ResetAlarmMessage(true));
        
        if (_pomodoroSessionService.GetCurrentSession() is not null)
        {
            await _pomodoroSessionService.CompleteSessionAsync((int)(_timerManager.State.Time.TotalMinutes));
        }
        
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

        }
    }
}