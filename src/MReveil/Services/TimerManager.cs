using CommunityToolkit.Mvvm.ComponentModel;
using Monbsoft.MReveil.Models;

namespace Monbsoft.MReveil.Services;

public partial class TimerManager : ObservableObject   
{
    [ObservableProperty]
    private IState _state;
    public TimerManager()
    {
        State = new ClockState();
    }

    public void Pause()
    {      
        switch(State)
        {
            case CountdownState countdownState:
                {
                    countdownState.Pause();
                    var duration = countdownState.End - DateTime.Now;
                    break;
                }
        }
    }   
    public void Play(TimeSpan duration)
    {
        State = new CountdownState(duration);
    }
    public void Stop()
    {
        if (State is not ClockState)
        {
            State = new ClockState();
        }
    }

}
