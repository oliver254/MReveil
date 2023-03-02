using MReveil.Drawables;

namespace MReveil.Controls;

public partial class CircularClock : ContentView
{
    public static readonly BindableProperty EndTimeProperty = BindableProperty.Create(nameof(EndTime), typeof(DateTime?), typeof(CircularClock), null, propertyChanged: OnEndTimeChanged);
    private readonly CircularDrawable _circularDrawable;

    public CircularClock()
    {
        InitializeComponent();
        _circularDrawable = new CircularDrawable();
        secondView.Drawable = _circularDrawable;
    }

    public DateTime? EndTime
    {
        get { return (DateTime?)GetValue(EndTimeProperty); }
        set { SetValue(EndTimeProperty, value); }
    }

    private static void OnEndTimeChanged(BindableObject d, object oldValue, object value)
    {
        CircularClock clock = (CircularClock)d;
    }

    public void UpdateArcs()
    {
        if (EndTime == null)
            return;

        TimeSpan elapsedTime = EndTime.Value - DateTime.Now;
        TimeLabel.Text = elapsedTime.ToString(@"hh\:mm\:ss");
        _circularDrawable.Minute = elapsedTime.Minutes;
        _circularDrawable.Second = elapsedTime.Seconds;

        secondView.Invalidate();
        if (elapsedTime.TotalSeconds <= 0)
        {
            return;
        }
    }
}