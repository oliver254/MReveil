using Microsoft.Extensions.Configuration;
using Monbsoft.MReveil.ViewModels;

namespace Monbsoft.MReveil.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		
	}
}