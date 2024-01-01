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

    }

    public ICommand PlayCommand { get; set; } 
    public ICommand AlarmCommand { get; set; }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        WeakReferenceMessenger.Default.Register<AlarmMessage>(this, (r, m) =>
        {
            if (m is not null && m.Value)
            {
                MediaElement.Play();
            }
        });
        WeakReferenceMessenger.Default.Register<ResetAlarmMessage>(this, (r, m) =>
        {
            if(m is not null && m.Value)
            {
                MediaElement.Stop();
            }
        });

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<AlarmMessage>(this);
        WeakReferenceMessenger.Default.Unregister<ResetAlarmMessage>(this);

    }

}