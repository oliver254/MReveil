using MReveil.Drawables;

namespace MReveil.Controls;

public partial class CircularClock : ContentView
{
    private readonly CircularDrawable _circularDrawable;

    private CancellationTokenSource _cancellationTokenSource = new();
    private DateTime _endTime;


    public CircularClock()
    {
        InitializeComponent();
        _circularDrawable = new CircularDrawable();
        SecondView.Drawable = _circularDrawable;
    } 

    private void StartButton_OnClicked(object sender, EventArgs e)
    {
        _endTime = DateTime.Now.AddMinutes(5);
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateArcs();
    }
    private async void UpdateArcs()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            TimeSpan elapsedTime = _endTime - DateTime.Now;
            TimeLabel.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            _circularDrawable.Minute = elapsedTime.Minutes;
            _circularDrawable.Second = elapsedTime.Seconds;
           
            SecondView.Invalidate();
            if (elapsedTime.TotalSeconds <= 0)
            {
                _cancellationTokenSource.Cancel();
                return;
            }
            await Task.Delay(500);
        }
    }
}