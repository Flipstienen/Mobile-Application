using Microsoft.Maui.Controls;

namespace MobileApplication.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        
        InitializeComponent();
    }

    private async void OnCurrentOrderClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CurrentOrderPage));
    }

    private async void OnScheduleClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    private async void OnTrackingClicked(object sender, EventArgs e)
    {
        DisplayAlert("Tracking", "Navigating to Tracking", "OK");
    }

    private async void OnWeatherClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(WeatherPage));
    }
}
