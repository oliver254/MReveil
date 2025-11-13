using Monbsoft.MReveil.ViewModels;

namespace Monbsoft.MReveil.Views;

public partial class JournalPage : ContentPage
{
    public JournalPage(JournalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
