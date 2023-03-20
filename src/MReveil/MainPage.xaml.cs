using CommunityToolkit.Maui.Views;
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
            mediaElement.Source = MediaSource.FromResource("alarm-clock.mp3");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer.Stop();
        }
        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            // Stop and cleanup MediaElement when we navigate away
            mediaElement.Handler?.DisconnectHandler();
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            clock.DrawArcs();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            mediaElement.Play();
        }
    }
}