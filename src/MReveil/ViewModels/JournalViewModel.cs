using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Monbsoft.MReveil.Messaging;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;
using System.Collections.ObjectModel;

namespace Monbsoft.MReveil.ViewModels;

public partial class JournalViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly StatisticsService _statisticsService;
    private readonly TaskService _taskService;

    [ObservableProperty]
    private ObservableCollection<TodoTask> tasks = new ObservableCollection<TodoTask>();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Now.Date;  // ✅ Initialiser avec .Date

    [ObservableProperty]
    private int completedPomodoros;

    [ObservableProperty]
    private int totalFocusMinutes;

    [ObservableProperty]
    private int tasksCompleted;

    [ObservableProperty]
    private string newTaskTitle = string.Empty;

    [ObservableProperty]
    private TodoTask? currentPomodoroTask;

    public JournalViewModel(DatabaseService databaseService, StatisticsService statisticsService, TaskService taskService)
    {
        _databaseService = databaseService;
        _statisticsService = statisticsService;
        _taskService = taskService;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            await LoadTodayEntryAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error initializing JournalViewModel: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task LoadTodayEntryAsync()
    {
        try
        {
            SelectedDate = DateTime.Now.Date;
            
            CompletedPomodoros = await _databaseService.GetCompletedSessionsCountAsync(SelectedDate);
            TotalFocusMinutes = await _databaseService.GetTotalFocusMinutesAsync(SelectedDate);
            TasksCompleted = await _taskService.GetCompletedTasksCountAsync(SelectedDate);
            
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur dans LoadTodayEntryAsync: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task LoadEntryByDateAsync(DateTime date)
    {
        SelectedDate = date.Date;  // ✅ Normaliser à 00:00:00
        
        CompletedPomodoros = await _databaseService.GetCompletedSessionsCountAsync(SelectedDate);
        TotalFocusMinutes = await _databaseService.GetTotalFocusMinutesAsync(SelectedDate);
        TasksCompleted = await _taskService.GetCompletedTasksCountAsync(SelectedDate);
        
        await LoadTasksAsync();
    }

    // Task Management
    [RelayCommand]
    public async Task LoadTasksAsync()
    {
        try
        {
            // Charger TOUTES les tâches, sans filtre de date
            var taskList = await _taskService.GetAllTasksAsync();
            
            Tasks.Clear();
            foreach (var task in taskList)
            {
                Tasks.Add(task);
            }
            
            var currentTaskId = _taskService.GetCurrentTaskId();
            if (currentTaskId.HasValue)
            {
                CurrentPomodoroTask = await _taskService.GetTaskAsync(currentTaskId.Value);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur dans LoadTasksAsync: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task AddTaskAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;

            var task = new TodoTask
            {
                Title = NewTaskTitle,
                IsCompleted = false,
                Date = DateTime.Now.Date // Utiliser la date actuelle comme référence
            };

            await _taskService.AddTaskAsync(task);
            NewTaskTitle = string.Empty;
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur dans AddTaskAsync: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task DeleteTaskAsync(TodoTask task)
    {
        if (task == null)
            return;

        await _taskService.DeleteTaskAsync(task.Id);
        await LoadTasksAsync();
        await LoadTodayEntryAsync(); // Rafraîchir les stats
    }

    [RelayCommand]
    public async Task ToggleTaskCompletionAsync(TodoTask task)
    {
        if (task == null)
            return;

        // Mettre à jour avec la valeur actuelle (déjà changée par le binding)
        await _taskService.UpdateTaskCompletionAsync(task.Id, task.IsCompleted);
        await LoadTasksAsync();
        await LoadTodayEntryAsync(); // Rafraîchir les stats
    }

    [RelayCommand]
    public async Task StartPomodoroWithTaskAsync(TodoTask task)
    {
        if (task == null)
            return;

        // Définir cette tâche comme tâche courante
        _taskService.SetCurrentTask(task.Id);
        CurrentPomodoroTask = task;

        // Naviguer vers la page principale et démarrer le Pomodoro
        // Note: Cela nécessite une communication avec MainViewModel
        WeakReferenceMessenger.Default.Send(new StartPomodoroWithTaskMessage(task.Id));
        
        // Naviguer vers MainPage
        await Shell.Current.GoToAsync("//MainPage");
    }
}
