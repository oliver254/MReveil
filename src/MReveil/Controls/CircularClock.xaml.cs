using Monbsoft.MReveil.Drawables;
using Monbsoft.MReveil.Models;
using Monbsoft.MReveil.Services;

namespace Monbsoft.MReveil.Controls;

public partial class CircularClock : ContentView
{
    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(IState), typeof(CircularClock), null);
    public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(TimeSpan?), typeof(CircularClock), null);
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
        Time = new TimeSpan();

    }

    ~CircularClock() => _timer.Tick -= OnTimerTick;

    public event EventHandler Alarm;


    public TimeSpan Time
    {
        get { return (TimeSpan)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, value); }
    }

    public IState State
    {
        get { return (IState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        State.Refresh();
        if (State.Time.TotalSeconds != Time.TotalSeconds)
        {
            Time = State.Time;
            _circularDrawable.Minute = Time.Minutes;
            _circularDrawable.Second = Time.Seconds;

            TimeLabel.Text = Time.ToString(@"hh\:mm\:ss");
            ClockView.Invalidate();
        }
    }

    private void ActivateAlarm()
    {
        Alarm?.Invoke(this, EventArgs.Empty);

    }
}