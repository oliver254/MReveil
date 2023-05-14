using CommunityToolkit.Mvvm.ComponentModel;
using MReveil.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MReveil.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ThemeService _themeService;   
        private readonly SettingsService _settingsService;
        private int _longDuration;
        private string _mode;        
        private int _shortDuration;
        private int _sprintDuration;        

        public SettingsViewModel(ThemeService themeService, SettingsService settingsService)
        {
            _mode = themeService.Mode;
            _sprintDuration = settingsService.Sprint;
            _shortDuration = settingsService.ShortBreak;

            _themeService = themeService;
            _settingsService = settingsService; 
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
    }
}
