using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly StatisticsService _statisticsService;

    [ObservableProperty]
    private int todayPomodoros;

    [ObservableProperty]
    private int todayFocusMinutes;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int weeklyPomodoros;

    [ObservableProperty]
    private double weeklyAverage;

    [ObservableProperty]
    private string streakMessage = "🔥 Loading...";

    [ObservableProperty]
    private List<string> weekDays = [];

    [ObservableProperty]
    private List<int> weekPomodoros = [];

    public StatisticsViewModel(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            await LoadTodayStatsAsync();
            await LoadWeeklyStatsAsync();
            await LoadStreakAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing StatisticsViewModel: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task LoadTodayStatsAsync()
    {
        var stats = await _statisticsService.GetDailyStatisticsAsync(DateTime.Now);
        TodayPomodoros = stats.CompletedPomodoros;
        TodayFocusMinutes = stats.TotalFocusMinutes;
    }

    [RelayCommand]
    public async Task LoadWeeklyStatsAsync()
    {
        var today = DateTime.Now;
        var dayOfWeek = (int)today.DayOfWeek;
        var weekStart = today.AddDays(-dayOfWeek);

        var weeklyStats = await _statisticsService.GetWeeklyStatisticsAsync(weekStart);

        WeeklyPomodoros = weeklyStats.TotalPomodoros;
        WeeklyAverage = Math.Round(weeklyStats.AveragePomodoros, 2);

        var newWeekDays = new List<string>();
        var newWeekPomodoros = new List<int>();

        foreach (var daily in weeklyStats.DailyStats)
        {
            newWeekDays.Add(daily.Date.ToString("ddd"));
            newWeekPomodoros.Add(daily.CompletedPomodoros);
        }

        WeekDays = newWeekDays;
        WeekPomodoros = newWeekPomodoros;
    }

    [RelayCommand]
    public async Task LoadStreakAsync()
    {
        CurrentStreak = await _statisticsService.GetCurrentStreakAsync();
        StreakMessage = CurrentStreak switch
        {
            0 => "🔥 Start your streak today!",
            1 => "🔥 1 day streak! Keep going!",
            > 1 and < 7 => $"🔥 {CurrentStreak} days! Almost a week!",
            >= 7 and < 30 => $"🔥 {CurrentStreak} days! Amazing!",
            >= 30 => $"🔥 {CurrentStreak} days! You're a legend!",
        };
    }

    [RelayCommand]
    public async Task LoadMonthlyStatsAsync(int year, int month)
    {
        var monthlyStats = await _statisticsService.GetMonthlyStatisticsAsync(year, month);
    }
}
