using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MReveil.Models;
using System.Windows.Input;

namespace MReveil.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [RelayCommand]
        public void Start(ActivityType test)
        {

        }
    }
}
