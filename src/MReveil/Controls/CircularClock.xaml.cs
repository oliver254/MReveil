using MReveil.Drawables;
using MReveil.Models;

namespace MReveil.Controls;

public partial class CircularClock : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan?), typeof(CircularClock), null, propertyChanged: OnDurationChanged);
    public static readonly BindableProperty ElapsedTimeProperty = BindableProperty.Create(nameof(ElapsedTime), typeof(TimeSpan?), typeof(CircularClock), null);
    public static readonly BindableProperty EndDateProperty = BindableProperty.Create(nameof(EndDate), typeof(DateTime?), typeof(CircularClock), null);
    public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(SprintStatus), typeof(CircularClock), SprintStatus.Clock);
    private readonly CircularDrawable _circularDrawable;
    private IDispatcherTimer _timer;

    public CircularClock()
    {
        InitializeComponent();
        _circularDrawable = new CircularDrawable();
        ClockView.Drawable = _circularDrawable;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMicroseconds(500);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    ~CircularClock() => _timer.Tick -= OnTimerTick;

    public event EventHandler Alarm;

    public TimeSpan? Duration
    {
        get { return (TimeSpan?)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public TimeSpan ElapsedTime
    {
        get { return (TimeSpan)GetValue(ElapsedTimeProperty); }
        set { SetValue(ElapsedTimeProperty, value); }
    }

    public DateTime? EndDate
    {
        get { return (DateTime?)GetValue(EndDateProperty); }
        set { SetValue(EndDateProperty, value); }
    }

    public SprintStatus Status
    {
        get { return (SprintStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        switch (Status)
        {
            case SprintStatus.Alarm:
                {
                    break;
                }
            case SprintStatus.Choose:
                {
                    ElapsedTime = Duration.Value;
                    break;
                }
            case SprintStatus.Clock:
                {
                    ElapsedTime = DateTime.Now.TimeOfDay;
                    break;
                }
            case SprintStatus.Playing:
                {
                    ElapsedTime = EndDate.Value - DateTime.Now;
                    if (ElapsedTime < TimeSpan.Zero)
                    {
                        ElapsedTime = TimeSpan.Zero;
                        ActivateAlarm();
                    }
                    break;
                }
        }
        _circularDrawable.Minute = ElapsedTime.Minutes;
        _circularDrawable.Second = ElapsedTime.Seconds;
        TimeLabel.Text = ElapsedTime.ToString(@"hh\:mm\:ss");
        ClockView.Invalidate();
    }

    public void Pause()
    {
        if (EndDate == null || EndDate < DateTime.Now)
            return;
        Duration = EndDate.Value - DateTime.Now;
        Status = SprintStatus.Paused;
    }

    public void Run()
    {
        if (Duration is null)
            return;
        EndDate = DateTime.Now.Add(Duration.Value);
        Status = SprintStatus.Playing;
    }

    private static void OnDurationChanged(BindableObject d, object oldValue, object value)
    {
        CircularClock clock = d as CircularClock;

        if (clock is not null && value is not null)
        {
            clock.Status = SprintStatus.Choose;
        }
        else
        {
            clock.Status = SprintStatus.Clock;
        }
    }
    private void ActivateAlarm()
    {
        Alarm?.Invoke(this, EventArgs.Empty);
        Status = SprintStatus.Alarm;
    }
}