namespace Monbsoft.MReveil.Models;

public class ClockState : IState
{
    public TimeSpan Time
    {
        get;
        private set;
    }

    public void Refresh()
    {
        Time = DateTime.Now.TimeOfDay;
    }

}
