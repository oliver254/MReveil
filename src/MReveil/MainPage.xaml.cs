using MReveil.Drawables;
using MReveil.ViewModels;

namespace MReveil
{
    public partial class MainPage : ContentPage
    {
        private readonly IDrawable _arcDrawable;
        private IDispatcherTimer _timer;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _arcDrawable = new CircularDrawable();
            BindingContext = viewModel;
        }

        ~MainPage() => _timer.Tick -= OnTimerTick;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            clock.UpdateArcs();
        }
    }
}