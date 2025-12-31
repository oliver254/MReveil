using Monbsoft.MReveil.Models;

namespace Monbsoft.MReveil.Services;

public class StatisticsService
{
    private readonly DatabaseService _databaseService;

    public StatisticsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<DailyStatistics> GetDailyStatisticsAsync(DateTime date)
    {
        var sessions = await _databaseService.GetSessionsByDateAsync(date);
        var journalEntry = await _databaseService.GetJournalEntryByDateAsync(date);

        var completedPomodoros = sessions.Count(s => s.IsCompleted && s.Type == ActivityType.Pomodoro);
        var totalFocusMinutes = sessions
            .Where(s => s.IsCompleted && s.Type == ActivityType.Pomodoro)
            .Sum(s => s.ActualDuration);

        return new DailyStatistics
        {
            Date = date,
            CompletedPomodoros = completedPomodoros,
            TotalFocusMinutes = totalFocusMinutes,
            Notes = journalEntry?.Content ?? string.Empty
        };
    }

    public async Task<WeeklyStatistics> GetWeeklyStatisticsAsync(DateTime weekStart)
    {
        var stats = new WeeklyStatistics { WeekStart = weekStart };
        var totalPomodoros = 0;
        var totalMinutes = 0;

        for (int i = 0; i < 7; i++)
        {
            var date = weekStart.AddDays(i);
            var dailyStats = await GetDailyStatisticsAsync(date);
            stats.DailyStats.Add(dailyStats);
            totalPomodoros += dailyStats.CompletedPomodoros;
            totalMinutes += dailyStats.TotalFocusMinutes;
        }

        stats.TotalPomodoros = totalPomodoros;
        stats.TotalFocusMinutes = totalMinutes;
        stats.AveragePomodoros = totalPomodoros / 7d;

        return stats;
    }

    public async Task<int> GetCurrentStreakAsync()
    {
        var streak = 0;
        var currentDate = DateTime.Now.Date.AddDays(-1); // Start from yesterday

        while (true)
        {
            var stats = await GetDailyStatisticsAsync(currentDate);
            if (stats.CompletedPomodoros == 0)
                break;

            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }

    public async Task<List<DailyStatistics>> GetMonthlyStatisticsAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var stats = new List<DailyStatistics>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var daily = await GetDailyStatisticsAsync(date);
            stats.Add(daily);
        }

        return stats;
    }
}

public class DailyStatistics
{
    public DateTime Date { get; set; }
    public int CompletedPomodoros { get; set; }
    public int TotalFocusMinutes { get; set; }
    public string Notes { get; set; }
}

public class WeeklyStatistics
{
    public DateTime WeekStart { get; set; }
    public int TotalPomodoros { get; set; }
    public int TotalFocusMinutes { get; set; }
    public double AveragePomodoros { get; set; }
    public List<DailyStatistics> DailyStats { get; } = [];
}
