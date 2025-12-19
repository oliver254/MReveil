using Monbsoft.MReveil.Drawables;
using Monbsoft.MReveil.Models;

namespace Monbsoft.MReveil.Controls;

public partial class CircularClock : ContentView
{
    public static readonly BindableProperty StateProperty = 
        BindableProperty.Create(
            nameof(State), 
            typeof(IState), 
            typeof(CircularClock), 
            null, 
            propertyChanged: OnStateChanged);

    public static readonly BindableProperty TimeProperty = 
        BindableProperty.Create(
            nameof(Time), 
            typeof(TimeSpan), 
            typeof(CircularClock), 
            TimeSpan.Zero,
            propertyChanged: OnTimeChanged);

    private readonly CircularDrawable _circularDrawable;
    private IDispatcherTimer? _timer;
    private bool _isDisposed;

    public CircularClock()
    {
        InitializeComponent();
        
        _circularDrawable = new CircularDrawable();
        ClockView.Drawable = _circularDrawable;

        InitializeTimer();
    }

    public event EventHandler? Alarm;

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public IState? State
    {
        get => (IState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    private void InitializeTimer()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += OnTimerTick;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        // Démarrer le timer dès que le contrôle est chargé
        _timer?.Start();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _timer?.Stop();
    }

    private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularClock clock && newValue is IState state)
        {
            clock.UpdateTime(state.Time);
        }
    }

    private static void OnTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CircularClock clock && newValue is TimeSpan time)
        {
            clock.UpdateDisplay(time);
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_isDisposed || State == null)
            return;

        try
        {
            State.Refresh();
            
            if (State.Time != Time)
            {
                UpdateTime(State.Time);
                
                // Déclencher l'alarme si le temps est écoulé (seulement pour CountdownState)
                if (State is CountdownState && State.Time <= TimeSpan.Zero)
                {
                    OnAlarm();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in CircularClock timer: {ex.Message}");
        }
    }

    private void UpdateTime(TimeSpan time)
    {
        Time = time;
        UpdateDisplay(time);
    }

    private void UpdateDisplay(TimeSpan time)
    {
        if (_isDisposed)
            return;

        // Pour ClockState (horloge), afficher heures:minutes:secondes
        // Pour CountdownState (chronomètre), afficher le temps restant
        if (State is ClockState)
        {
            // Affichage de l'heure actuelle
            _circularDrawable.Hour = time.Hours;
            _circularDrawable.Minute = time.Minutes;
            _circularDrawable.Second = time.Seconds;
            _circularDrawable.ShowHours = true;
            TimeLabel.Text = time.ToString(@"hh\:mm\:ss");
        }
        else
        {
            // Affichage du chronomètre (compte à rebours)
            _circularDrawable.Hour = 0;
            _circularDrawable.Minute = time.Minutes;
            _circularDrawable.Second = time.Seconds;
            _circularDrawable.ShowHours = false;
            TimeLabel.Text = time.ToString(@"mm\:ss");
        }
        
        // Invalidation du GraphicsView
        ClockView?.Invalidate();
    }

    private void OnAlarm()
    {
        Alarm?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler == null)
        {
            Dispose();
        }
    }

    private void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }

        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }
}