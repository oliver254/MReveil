using Monbsoft.MReveil.Models;

namespace Monbsoft.MReveil.Services;

public class TaskService
{
    private readonly DatabaseService _databaseService;
    private int? _currentTaskId;

    public TaskService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    // CRUD de base
    public async Task<TodoTask> CreateTaskAsync(string title, DateTime date)
    {
        var task = new TodoTask
        {
            Title = title,
            Date = date.Date,
            CreatedAt = DateTime.Now
        };

        task.Id = await _databaseService.SaveTaskAsync(task);
        return task;
    }

    public async Task<TodoTask?> GetTaskAsync(int id)
    {
        return await _databaseService.GetTaskAsync(id);
    }

    public async Task<List<TodoTask>> GetTasksByDateAsync(DateTime date)
    {
        var tasks = await _databaseService.GetTasksByDateAsync(date);
        
        // Tri: priorité, non complétées en premier, puis par date de création
        return tasks
            .OrderByDescending(t => !t.IsCompleted)
            .ThenByDescending(t => t.Priority)
            .ThenByDescending(t => t.IsInProgress)
            .ThenBy(t => t.CreatedAt)
            .ToList();
    }

    // Alias pour correspondre au ViewModel
    public async Task<List<TodoTask>> GetTasksAsync(DateTime date)
    {
        return await GetTasksByDateAsync(date);
    }

    // Récupérer toutes les tâches (non liées à une date)
    public async Task<List<TodoTask>> GetAllTasksAsync()
    {
        var tasks = await _databaseService.GetAllTasksAsync();
        
        // Tri: non complétées en premier, puis par priorité et date de création
        return tasks
            .OrderByDescending(t => !t.IsCompleted)
            .ThenByDescending(t => t.Priority)
            .ThenByDescending(t => t.IsInProgress)
            .ThenBy(t => t.CreatedAt)
            .ToList();
    }

    public async Task<List<TodoTask>> GetAllIncompleteTasksAsync()
    {
        return await _databaseService.GetIncompleteTasksAsync();
    }

    public async Task UpdateTaskAsync(TodoTask task)
    {
        task.UpdatedAt = DateTime.Now;
        await _databaseService.SaveTaskAsync(task);
    }

    public async Task AddTaskAsync(TodoTask task)
    {
        task.CreatedAt = DateTime.Now;
        await _databaseService.SaveTaskAsync(task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _databaseService.DeleteTaskAsync(id);
        
        // Si c'était la tâche courante, la retirer
        if (_currentTaskId == id)
        {
            _currentTaskId = null;
        }
    }

    // Opérations métier
    public async Task<bool> ToggleCompletionAsync(int id)
    {
        var task = await GetTaskAsync(id);
        if (task == null)
            return false;

        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.Now : null;
        task.UpdatedAt = DateTime.Now;

        await _databaseService.SaveTaskAsync(task);
        return task.IsCompleted;
    }

    public async Task UpdateTaskCompletionAsync(int id, bool isCompleted)
    {
        var task = await GetTaskAsync(id);
        if (task == null)
            return;

        task.IsCompleted = isCompleted;
        task.CompletedAt = isCompleted ? DateTime.Now : null;
        task.UpdatedAt = DateTime.Now;

        await _databaseService.SaveTaskAsync(task);
    }

    public async Task IncrementPomodorosAsync(int id)
    {
        var task = await GetTaskAsync(id);
        if (task == null)
            return;

        task.PomodorosCount++;
        task.UpdatedAt = DateTime.Now;
        await _databaseService.SaveTaskAsync(task);
    }

    public TodoTask? GetCurrentTask()
    {
        if (!_currentTaskId.HasValue)
            return null;

        return GetTaskAsync(_currentTaskId.Value).Result;
    }

    public void SetCurrentTask(int? taskId)
    {
        _currentTaskId = taskId;
    }

    public int? GetCurrentTaskId()
    {
        return _currentTaskId;
    }

    // Statistiques
    public async Task<int> GetCompletedTasksCountAsync(DateTime date)
    {
        var tasks = await GetTasksByDateAsync(date);
        return tasks.Count(t => t.IsCompleted);
    }

    public async Task<int> GetTotalTasksCountAsync(DateTime date)
    {
        var tasks = await GetTasksByDateAsync(date);
        return tasks.Count;
    }
}
