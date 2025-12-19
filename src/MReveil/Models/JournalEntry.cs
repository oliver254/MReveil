using SQLite;

namespace Monbsoft.MReveil.Models;

[Table("journal_entries")]
public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public DateTime Date { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int PomodorosCompleted { get; set; }

    public int TotalFocusMinutes { get; set; }

    public string Mood { get; set; } = "😐"; // emoji ou value

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
