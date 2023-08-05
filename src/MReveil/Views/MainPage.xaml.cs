using CommunityToolkit.Maui.Views;
using Monbsoft.MReveil.ViewModels;

namespace Monbsoft.MReveil.Views;

public partial class MainPage : ContentPage
{

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        switch(CircularClock.Status)
        {
            case Models.SprintStatus.Playing:
                {
                    CircularClock.Pause();
                    VisualStateManager.GoToState(GridLayout, "Pause");
                    break;
                }
            default:
                {
                    CircularClock.Run();
                    VisualStateManager.GoToState(GridLayout, "Playing");
                    break;
                }
        }
    }
    private void CircularClock_Alarm(object sender, EventArgs e)
    {
        MediaElement.Play();
    }
}