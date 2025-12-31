using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.Services;

public class PomodoroSessionService
{
    private readonly DatabaseService _databaseService;
    private readonly TaskService _taskService;
    private PomodoroSession _currentSession;

    public PomodoroSessionService(DatabaseService databaseService, TaskService taskService)
    {
        _databaseService = databaseService;
        _taskService = taskService;
    }

    public void StartSession(ActivityType activityType, int plannedDuration, int? taskId = null)
    {
        _currentSession = new PomodoroSession
        {
            Type = activityType,
            Date = DateTime.Now,
            PlannedDuration = plannedDuration,
            ActualDuration = 0,
            IsCompleted = false,
            TaskId = taskId,
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
        
        // Si une tâche est associée et que c'est un Pomodoro, incrémenter le compteur
        if (_currentSession.TaskId.HasValue && _currentSession.Type == ActivityType.Pomodoro)
        {
            await _taskService.IncrementPomodorosAsync(_currentSession.TaskId.Value);
        }
        
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
