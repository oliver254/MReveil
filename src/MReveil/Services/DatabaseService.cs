using SQLite;
using Monbsoft.MReveil.Models;

namespace Monbsoft.MReveil.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private static readonly string DbPath = Path.Combine(FileSystem.AppDataDirectory, "mreveil.db");
    private readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private bool _initialized;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _initSemaphore.WaitAsync();
        try
        {
            if (_initialized)
                return;

            _connection = new SQLiteAsyncConnection(DbPath);
            await _connection.CreateTableAsync<PomodoroSession>();
            await _connection.CreateTableAsync<JournalEntry>();
            _initialized = true;
        }
        finally
        {
            _initSemaphore.Release();
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (!_initialized)
        {
            await InitializeAsync();
        }
    }

    // PomodoroSession operations
    public async Task<int> SaveSessionAsync(PomodoroSession session)
    {
        await EnsureInitializedAsync();
        if (session.Id == 0)
            return await _connection!.InsertAsync(session);
        else
            return await _connection!.UpdateAsync(session);
    }

    public async Task<PomodoroSession> GetSessionAsync(int id)
    {
        await EnsureInitializedAsync();
        return await _connection!.FindAsync<PomodoroSession>(id);
    }

    public async Task<List<PomodoroSession>> GetSessionsByDateAsync(DateTime date)
    {
        await EnsureInitializedAsync();
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _connection!.Table<PomodoroSession>()
            .Where(s => s.Date >= startOfDay && s.Date < endOfDay)
            .ToListAsync();
    }

    public async Task<List<PomodoroSession>> GetSessionsAsync(DateTime startDate, DateTime endDate)
    {
        await EnsureInitializedAsync();
        return await _connection!.Table<PomodoroSession>()
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task DeleteSessionAsync(int id)
    {
        await EnsureInitializedAsync();
        await _connection!.DeleteAsync<PomodoroSession>(id);
    }

    public async Task<int> GetCompletedSessionsCountAsync(DateTime date)
    {
        await EnsureInitializedAsync();
        var sessions = await GetSessionsByDateAsync(date);
        return sessions.Count(s => s.IsCompleted && s.Type == ActivityType.Pomodoro);
    }

    public async Task<int> GetTotalFocusMinutesAsync(DateTime date)
    {
        await EnsureInitializedAsync();
        var sessions = await GetSessionsByDateAsync(date);
        return sessions
            .Where(s => s.IsCompleted && s.Type == ActivityType.Pomodoro)
            .Sum(s => s.ActualDuration);
    }

    // JournalEntry operations
    public async Task<int> SaveJournalEntryAsync(JournalEntry entry)
    {
        await EnsureInitializedAsync();
        entry.UpdatedAt = DateTime.Now;
        if (entry.Id == 0)
            return await _connection!.InsertAsync(entry);
        else
            return await _connection!.UpdateAsync(entry);
    }

    public async Task<JournalEntry> GetJournalEntryAsync(int id)
    {
        await EnsureInitializedAsync();
        return await _connection!.FindAsync<JournalEntry>(id);
    }

    public async Task<JournalEntry> GetJournalEntryByDateAsync(DateTime date)
    {
        await EnsureInitializedAsync();
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _connection!.Table<JournalEntry>()
            .FirstOrDefaultAsync(j => j.Date >= startOfDay && j.Date < endOfDay);
    }

    public async Task<List<JournalEntry>> GetJournalEntriesAsync(DateTime startDate, DateTime endDate)
    {
        await EnsureInitializedAsync();
        return await _connection!.Table<JournalEntry>()
            .Where(j => j.Date >= startDate && j.Date <= endDate)
            .OrderByDescending(j => j.Date)
            .ToListAsync();
    }

    public async Task DeleteJournalEntryAsync(int id)
    {
        await EnsureInitializedAsync();
        await _connection!.DeleteAsync<JournalEntry>(id);
    }
}
