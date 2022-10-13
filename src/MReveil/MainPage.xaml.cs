using MReveil.Drawables;

namespace MReveil
{
    public partial class MainPage : ContentPage
    {
        private readonly IDrawable _arcDrawable;

        public MainPage()
        {
            InitializeComponent();
            _arcDrawable = new CircularDrawable();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ProgressView.Drawable = _arcDrawable;
        }

    }
}