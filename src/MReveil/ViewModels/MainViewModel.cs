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
    private readonly TaskService _taskService;
    private ActivityType _currentActivityType;

    [ObservableProperty]
    public bool _alarm;

    [ObservableProperty]
    public SettingsViewModel _settings;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    public IState _state;

    [ObservableProperty]
    public TodoTask? _currentTask;


    public MainViewModel(TimerManager timerManager, SettingsViewModel settingsViewModel, PomodoroSessionService pomodoroSessionService, TaskService taskService)
    {
        _timerManager = timerManager;
        _settings = settingsViewModel;
        _pomodoroSessionService = pomodoroSessionService;
        _taskService = taskService;
        _state = _timerManager.State;
        _timerManager.PropertyChanged += TimerManager_PropertyChanged;
        
        // S'abonner au message pour démarrer un Pomodoro avec une tâche
        WeakReferenceMessenger.Default.Register<StartPomodoroWithTaskMessage>(this, (r, m) =>
        {
            SetDurationWithTask(ActivityType.Pomodoro, m.TaskId);
        });
    }

    [RelayCommand]
    public void Pause()
    {
        _timerManager.Pause();
    }

    [RelayCommand]
    public void SetDuration(ActivityType activityType)
    {
        SetDurationWithTask(activityType, null);
    }

    private void SetDurationWithTask(ActivityType activityType, int? taskId)
    {
        WeakReferenceMessenger.Default.Send(new ResetAlarmMessage(true));
        _currentActivityType = activityType;
        
        int duration = activityType switch
        {
            ActivityType.Pomodoro => Settings.SprintDuration,
            ActivityType.LongBreak => Settings.LongBreakDuration,
            ActivityType.ShortBreak => Settings.ShortBreakDuration,
            _ => 25
        };

        _pomodoroSessionService.StartSession(activityType, duration, taskId);
        _timerManager.Play(TimeSpan.FromMinutes(duration));
        State = _timerManager.State;
        
        // Charger la tâche courante si une tâche est associée
        LoadCurrentTask(taskId);
    }

    private async void LoadCurrentTask(int? taskId)
    {
        if (taskId.HasValue)
        {
            CurrentTask = await _taskService.GetTaskAsync(taskId.Value);
        }
        else
        {
            CurrentTask = null;
        }
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
        
        // Réinitialiser la tâche courante
        CurrentTask = null;
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