using CommunityToolkit.Maui.Views;
using Monbsoft.MReveil.ViewModels;
using System.Windows.Input;

namespace Monbsoft.MReveil.Views;

public partial class MainPage : ContentPage
{
    public static readonly BindableProperty PauseProperty = BindableProperty.Create(nameof(PauseCommand), typeof(ICommand), typeof(MainPage), null);
    public static readonly BindableProperty PlayProperty = BindableProperty.Create(nameof(PlayCommand), typeof(ICommand), typeof(MainPage), null);
    

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public ICommand PlayCommand { get; set; } 
    public ICommand PauseCommand { get; set; }

}