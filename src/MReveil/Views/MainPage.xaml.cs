using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Monbsoft.MReveil.Messaging;
using Monbsoft.MReveil.ViewModels;
using System.Windows.Input;

namespace Monbsoft.MReveil.Views;

public partial class MainPage : ContentPage
{
    public static readonly BindableProperty AlarmProperty = BindableProperty.Create(nameof(AlarmCommand), typeof(ICommand), typeof(MainPage), null);
    public static readonly BindableProperty PlayProperty = BindableProperty.Create(nameof(PlayCommand), typeof(ICommand), typeof(MainPage), null);


    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        WeakReferenceMessenger.Default.Register<AlarmMessage>(this, (r, m) =>
        {
            if(m is not null && m.Value)
            {
                ActiveAlarm();
            }
        });
    }

    ~MainPage()
    {
        WeakReferenceMessenger.Default.Unregister<AlarmMessage>(this);
        CircularClock = null;

    }

    public ICommand PlayCommand { get; set; } 
    public ICommand AlarmCommand { get; set; }

    private void ActiveAlarm()
    {
        MediaElement.Play();
    }

}