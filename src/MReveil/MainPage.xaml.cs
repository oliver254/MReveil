﻿using CommunityToolkit.Maui.Views;
using MReveil.Drawables;
using MReveil.ViewModels;

namespace MReveil
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void PlayButton_Clicked(object sender, EventArgs e)
        {
            CircularClock.Run();
        }
        private void CircularClock_Alarm(object sender, EventArgs e)
        {
            MediaElement.Play();
        }
    }
}