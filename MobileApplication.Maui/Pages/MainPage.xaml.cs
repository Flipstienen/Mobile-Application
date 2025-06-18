using Microsoft.Maui.Controls;

namespace MobileApplication.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        
        InitializeComponent();
    }

    private void OnCurrentOrderClicked(object sender, EventArgs e)
    {
        DisplayAlert("Current Order", "Navigating to Current Order", "OK");
    }

    private void OnScheduleClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    private void OnTrackingClicked(object sender, EventArgs e)
    {
        DisplayAlert("Tracking", "Navigating to Tracking", "OK");
    }

    private void OnWeatherClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(WeatherPage));
    }
}
