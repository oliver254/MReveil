using MReveil.Drawables;
using MReveil.ViewModels;

namespace MReveil
{
    public partial class MainPage : ContentPage
    {
        private readonly IDrawable _arcDrawable;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _arcDrawable = new CircularDrawable();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ProgressView.Drawable = _arcDrawable;
        }

    }
}