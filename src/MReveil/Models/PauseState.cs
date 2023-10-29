namespace Monbsoft.MReveil.Models;

public class PauseState : IState
{
    public PauseState(TimeSpan remainingDuration)
    {
        RemainingDuration = remainingDuration;
        Time = TimeSpan.Zero;
    }
    public TimeSpan RemainingDuration { get; }
    public TimeSpan Time { get; private set; }

    public void Refresh()
    {
    }
}
