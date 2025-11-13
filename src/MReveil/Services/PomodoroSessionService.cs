using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.Services;

public class PomodoroSessionService
{
    private readonly DatabaseService _databaseService;
    private PomodoroSession _currentSession;

    public PomodoroSessionService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void StartSession(ActivityType activityType, int plannedDuration)
    {
        _currentSession = new PomodoroSession
        {
            Type = activityType,
            Date = DateTime.Now,
            PlannedDuration = plannedDuration,
            ActualDuration = 0,
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
    }

    public async Task CompleteSessionAsync(int actualDuration)
    {
        if (_currentSession == null)
            return;

        _currentSession.ActualDuration = actualDuration;
        _currentSession.IsCompleted = true;
        _currentSession.CompletedAt = DateTime.Now;

        await _databaseService.SaveSessionAsync(_currentSession);
        _currentSession = null;
    }

    public async Task CancelSessionAsync()
    {
        _currentSession = null;
    }

    public PomodoroSession GetCurrentSession()
    {
        return _currentSession;
    }

    public async Task<List<PomodoroSession>> GetTodaySessionsAsync()
    {
        return await _databaseService.GetSessionsByDateAsync(DateTime.Now);
    }

    public async Task<List<PomodoroSession>> GetSessionsAsync(DateTime startDate, DateTime endDate)
    {
        return await _databaseService.GetSessionsAsync(startDate, endDate);
    }
}
