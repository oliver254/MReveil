using MReveil.Models;

namespace MReveil.Controls;

public class CircularClock : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(CircularClock), new TimeSpan());
    public static readonly BindableProperty EndDateProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan?), typeof(CircularClock), new TimeSpan());
    public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(SprintStatus), typeof(CircularClock), SprintStatus.Clock);
    private IDispatcherTimer _timer;

    public CircularClock()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }
    ~CircularClock() => _timer.Tick -= OnTimerTick;

    public event EventHandler UpdateStatus;

    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }
    public DateTime? EndDate
    {
        get { return (TimeSpan)GetValue(EndDateProperty); }
        set { SetValue(EndDateProperty, value); }
    }
    public SprintStatus Status
    {
        get { return (SprintStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }


    private void OnTimerTick(object sender, EventArgs e)
    {

    }
    public void Pause()
    {
        Status = SprintStatus.Paused;
    }
    public void Play()
    {
        var test = Duration + DateTime.Now;
        Status = SprintStatus.Playing;
    }
}