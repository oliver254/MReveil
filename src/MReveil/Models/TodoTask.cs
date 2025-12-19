using SQLite;

namespace Monbsoft.MReveil.Models;

[Table("tasks")]
public class TodoTask
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    [Indexed]
    public DateTime Date { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int PomodorosCount { get; set; }

    public int EstimatedPomodoros { get; set; } = 1;

    public int Priority { get; set; } // 0=Normal, 1=Haute, 2=Urgente

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    // Propriétés calculées
    public bool IsInProgress => PomodorosCount > 0 && !IsCompleted;
    
    public string DisplayPomodoros => PomodorosCount > 0 ? $"{PomodorosCount}🍅" : "";
}
