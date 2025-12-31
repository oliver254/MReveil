namespace Monbsoft.MReveil.Messaging;

public class StartPomodoroWithTaskMessage
{
    public int TaskId { get; }

    public StartPomodoroWithTaskMessage(int taskId)
    {
        TaskId = taskId;
    }
}
