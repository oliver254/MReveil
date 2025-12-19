using SQLite;

namespace Monbsoft.MReveil.Models;

[Table("pomodoro_sessions")]
public class PomodoroSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public DateTime Date { get; set; }

    public ActivityType Type { get; set; }

    public int PlannedDuration { get; set; } // en minutes

    public int ActualDuration { get; set; } // en minutes

    public bool IsCompleted { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }
}
