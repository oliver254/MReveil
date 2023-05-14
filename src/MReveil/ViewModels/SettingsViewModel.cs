using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ThemeService _themeService;   
        private readonly SettingsService _settingsService;
        private TimeSpan? _customizedTime;
        private int _longDuration;
        private string _mode;        
        private int _shortDuration;
        private int _sprintDuration;        

        public SettingsViewModel(ThemeService themeService, SettingsService settingsService)
        {
            _themeService = themeService;
            _settingsService = settingsService;

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
        }
    }
}
