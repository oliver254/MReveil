using Microsoft.Extensions.Configuration;
using MReveil.ViewModels;

namespace MReveil.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		
	}
}