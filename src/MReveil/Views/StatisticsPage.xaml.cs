using Monbsoft.MReveil.ViewModels;

namespace Monbsoft.MReveil.Views;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
