using CommunityToolkit.Maui.Views;
using MReveil.Drawables;
using MReveil.Models;

namespace MReveil.Controls;

public partial class CircularClock2 : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan?), typeof(CircularClock), null, propertyChanged: OnDurationChanged);
    private readonly CircularDrawable _circularDrawable;
    private DateTime? _endTime;
    private SprintState _state;
    private TimeSpan? _sprint;

    public CircularClock2()
    {
        InitializeComponent();
        _state = SprintState.Clock;
        _circularDrawable = new CircularDrawable();
        clockView.Drawable = _circularDrawable;
        mediaElement.Source = MediaSource.FromResource("bird.mp3");
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
            case SprintState.Completed:
                {
                    mediaElement.Play();
                    break;
                }
            case SprintState.Run:
                {                    
                    TimeSpan elapsedTime = _endTime.Value - DateTime.Now;
                    if (elapsedTime.TotalSeconds < 0)
                    {
                        _state = SprintState.Completed;
                        return;
                    }
                    timeLabel.Text = elapsedTime.ToString(@"hh\:mm\:ss");
                    _circularDrawable.Minute = elapsedTime.Minutes;
                    _circularDrawable.Second = elapsedTime.Seconds;
                    clockView.Invalidate();
                    break;
                }
            case SprintState.Start:
                {
                    if (_sprint.HasValue)
                    {
                        timeLabel.Text = _sprint.Value.ToString(@"hh\:mm\:ss");
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
        var durationValue = (TimeSpan?)value;

        if(durationValue != null)
        {
            clockView._sprint = durationValue;
            clockView._state = SprintState.Start;
        }
        else
        {
            clockView._sprint = null;
            clockView._state = SprintState.Clock;
        }
    }    
    private void ContentView_Unloaded(object sender, EventArgs e)
    {
        // Stop and cleanup MediaElement when we navigate away
        mediaElement.Handler?.DisconnectHandler();
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        _state = SprintState.Clock;
        _endTime = null;
        _sprint = null;
    }

    private void Sprint_Clicked(object sender, EventArgs e)
    {
        switch(_state)
        {
            case SprintState.Start:
                {
                    Run();
                    break;
                }
            case SprintState.Run:
                {
                    Stop();
                    break;
                }
            default:
                {
                    Reset();
                    break;
                }
        }
    }
    private void Run()
    {
        if (_sprint == null)
            return;
        _endTime = DateTime.Now.Add(_sprint.Value);
        _state = SprintState.Run;
        VisualStateManager.GoToState(mainLayout, "Stop");

    }
    private void Reset()
    {
        _state = SprintState.Clock;
        _endTime = null;        
        VisualStateManager.GoToState(mainLayout, "Start");
        mediaElement.Stop();
    }
    private void Stop()
    {
        _sprint = _endTime - DateTime.Now;        
        _state = SprintState.Start;
        VisualStateManager.GoToState(mainLayout, "Start");
        mediaElement.Stop();
    }

}