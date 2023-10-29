namespace Monbsoft.MReveil.Models;

public class CountdownState : IState
{
    public CountdownState(TimeSpan duration)
    {
        End = DateTime.Now + duration;
        if(End.Ticks < 0)
        {
            throw new InvalidOperationException();
        }
    }

    public bool Alarm { get; private set; }
    public DateTime End { get; }
    public TimeSpan Time { get; private set; }

    public void Refresh()
    {
        if(Alarm)
        {
            Time = TimeSpan.Zero;
            return;
        }
        var duration  = End - DateTime.Now;
        if(duration <= TimeSpan.Zero) 
        {
            Alarm = true;
            duration = TimeSpan.Zero;
        }
        Time = duration;
    }
}
