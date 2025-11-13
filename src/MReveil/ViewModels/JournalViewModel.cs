using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.ViewModels;

public partial class JournalViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly StatisticsService _statisticsService;

    [ObservableProperty]
    private JournalEntry currentEntry = new();

    [ObservableProperty]
    private List<JournalEntry> journalEntries = [];

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Now;

    [ObservableProperty]
    private string mood = "😐";

    [ObservableProperty]
    private int completedPomodoros;

    [ObservableProperty]
    private int totalFocusMinutes;

    public JournalViewModel(DatabaseService databaseService, StatisticsService statisticsService)
    {
        _databaseService = databaseService;
        _statisticsService = statisticsService;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            await LoadTodayEntryAsync();
            await LoadJournalEntriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing JournalViewModel: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task LoadTodayEntryAsync()
    {
        SelectedDate = DateTime.Now;
        var entry = await _databaseService.GetJournalEntryByDateAsync(SelectedDate);
        
        CurrentEntry = entry ?? new JournalEntry { Date = SelectedDate };
        Mood = CurrentEntry.Mood;
        CompletedPomodoros = CurrentEntry.PomodorosCompleted;
        TotalFocusMinutes = CurrentEntry.TotalFocusMinutes;
    }

    [RelayCommand]
    public async Task LoadEntryByDateAsync(DateTime date)
    {
        SelectedDate = date;
        var entry = await _databaseService.GetJournalEntryByDateAsync(date);
        CurrentEntry = entry ?? new JournalEntry { Date = date };

        Mood = CurrentEntry.Mood;
        CompletedPomodoros = await _databaseService.GetCompletedSessionsCountAsync(date);
        TotalFocusMinutes = await _databaseService.GetTotalFocusMinutesAsync(date);
    }

    [RelayCommand]
    public async Task SaveEntryAsync()
    {
        if (CurrentEntry == null)
            return;

        CurrentEntry.Date = SelectedDate;
        CurrentEntry.Mood = Mood;
        CurrentEntry.PomodorosCompleted = CompletedPomodoros;
        CurrentEntry.TotalFocusMinutes = TotalFocusMinutes;

        await _databaseService.SaveJournalEntryAsync(CurrentEntry);
        await LoadJournalEntriesAsync();
    }

    [RelayCommand]
    public async Task DeleteEntryAsync()
    {
        if (CurrentEntry?.Id == 0)
            return;

        await _databaseService.DeleteJournalEntryAsync(CurrentEntry.Id);
        CurrentEntry = new JournalEntry { Date = SelectedDate };
        await LoadJournalEntriesAsync();
    }

    [RelayCommand]
    public async Task LoadJournalEntriesAsync()
    {
        var endDate = DateTime.Now;
        var startDate = endDate.AddMonths(-1);

        JournalEntries = await _databaseService.GetJournalEntriesAsync(startDate, endDate);
    }

    [RelayCommand]
    public async Task SetMoodAsync(string moodEmoji)
    {
        Mood = moodEmoji;
    }

    [RelayCommand]
    public async Task LoadMonthlyViewAsync(int month, int year)
    {
        JournalEntries = await _databaseService.GetJournalEntriesAsync(
            new DateTime(year, month, 1),
            new DateTime(year, month, 1).AddMonths(1).AddDays(-1)
        );
    }
}
