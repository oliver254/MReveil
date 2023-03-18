using MReveil.Drawables;
using MReveil.Models;

namespace MReveil.Controls;

public partial class CircularClock : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan?), typeof(CircularClock), null, propertyChanged: OnDurationChanged);
    private readonly CircularDrawable _circularDrawable;
    private DateTime? _endTime;
    private ClockState _state;

    public CircularClock()
    {
        InitializeComponent();
        _state = ClockState.Clock;
        _circularDrawable = new CircularDrawable();
        clockView.Drawable = _circularDrawable;
    }

    public TimeSpan? Duration
    {
        get { return (TimeSpan?)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public void DrawArcs()
    {
        switch (_state)
        {
            case ClockState.Alarm:
                {

                    break;
                }
            case ClockState.Timer:
                {
                    TimeSpan elapsedTime = _endTime.Value - DateTime.Now;
                    if (elapsedTime.TotalSeconds < 0)
                    {
                        _state = ClockState.Alarm;
                        return;
                    }
                    timeLabel.Text = elapsedTime.ToString(@"hh\:mm\:ss");
                    _circularDrawable.Minute = elapsedTime.Minutes;
                    _circularDrawable.Second = elapsedTime.Seconds;
                    clockView.Invalidate();
                    break;
                }
            case ClockState.Duration:
                {
                    if (Duration.HasValue)
                    {
                        timeLabel.Text = Duration.Value.ToString(@"hh\:mm\:ss");
                        _circularDrawable.Minute = Duration.Value.Minutes;
                        _circularDrawable.Second = Duration.Value.Seconds;
                        clockView.Invalidate();
                    }
                    break;
                }
            default:
                {
                    DateTime now = DateTime.Now;
                    timeLabel.Text = now.ToString(@"HH\:mm\:ss");
                    _circularDrawable.Minute = now.Minute;
                    _circularDrawable.Second = now.Second;
                    clockView.Invalidate();
                    break;
                }
        }
    }

    private static void OnDurationChanged(BindableObject d, object oldValue, object value)
    {
        var clockView = (CircularClock)d;
        if (value != null)
        {
            clockView._state = ClockState.Duration;
        }
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        _state = ClockState.Clock;
        startButton.IsVisible = true;
        stopButton.IsVisible = false;
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        _state = ClockState.Timer;
        _endTime = DateTime.Now.Add(Duration.Value);
        startButton.IsVisible = false;
        stopButton.IsVisible = true;
    }

    private void Stop_Clicked(object sender, EventArgs e)
    {
        Duration = _endTime.Value - DateTime.Now;
        _state = ClockState.Duration;
        startButton.IsVisible = true;
        stopButton.IsVisible = false;
    }
}