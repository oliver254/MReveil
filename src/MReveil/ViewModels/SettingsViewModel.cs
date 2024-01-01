using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Monbsoft.MReveil.Messaging;
using Monbsoft.MReveil.Services;
using Monbsoft.MReveil.Views;

namespace Monbsoft.MReveil.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly TimerManager _timerManager;
    private readonly ThemeService _themeService;   
    private readonly SettingsService _settingsService;
    private TimeSpan? _customizedTime;
    private int _longDuration;
    private bool _loopPlayback;
    private string _mode;        
    private int _shortDuration;
    private int _sprintDuration;        

    public SettingsViewModel(TimerManager timerManager, ThemeService themeService, SettingsService settingsService)
    {
        _timerManager = timerManager;
        _themeService = themeService;
        _settingsService = settingsService;
        _customizedTime = DateTime.Now.TimeOfDay;
        Initialize();
    }

    public TimeSpan? CustomizedTime
    {
        get => _customizedTime;
        set
        {
            SetProperty(ref _customizedTime, value);
        }
    }
    public int LongBreakDuration
    {
        get => _longDuration;
        set
        {
            SetProperty(ref _longDuration, value);
            _settingsService.LongBreak = value;
        }
    }
    public bool LoopPlayback
    {
        get => _loopPlayback;
        set
        {
            SetProperty(ref _loopPlayback, value);
            _settingsService.LoopPlayback = value;
        }
    }
    public string Mode
    {
        get => _mode;
        set
        {
            SetProperty(ref _mode, value);
            _themeService.SetMode(value);
        }
    }
    public int ShortBreakDuration
    {
        get => _shortDuration;
        set
        {
            SetProperty(ref _shortDuration, value);
            _settingsService.ShortBreak = value;
        }
    }
    public int SprintDuration
    {
        get => _sprintDuration;
        set
        {
            SetProperty(ref _sprintDuration, value);
            _settingsService.Sprint = value;
        }
    }

    [RelayCommand]
    public void Clear()
    {
        Preferences.Clear();
        Initialize();            
    }
    private void Initialize()
    {
        Mode = _themeService.Mode;
        SprintDuration = _settingsService.Sprint;
        ShortBreakDuration = _settingsService.ShortBreak;
        LongBreakDuration = _settingsService.LongBreak;
        LoopPlayback = _settingsService.LoopPlayback;
    }
    [RelayCommand]
    public async Task SetTime()
    {
        if (CustomizedTime is null)
            return;

        var duration = (DateTime.Today + CustomizedTime) - DateTime.Now;
        if (duration is null || duration.Value.Ticks < 0)
            return;

        _timerManager.Play(duration.Value);
        await Shell.Current.GoToAsync(new ShellNavigationState($"//{nameof(MainPage)}"));
    }
}
