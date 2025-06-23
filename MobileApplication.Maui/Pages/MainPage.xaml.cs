using System.Text.Json;
using System.Text.Json.Nodes;

namespace MobileApplication.Maui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {

        InitializeComponent();
    }

    private async void OnCurrentOrderClicked(object sender, EventArgs e)
    {
        if (JsonSerializer.Deserialize<bool>(Preferences.Get("working", "")) == false)
        {
            await Shell.Current.GoToAsync(nameof(CurrentOrderPage));
        }
    }

    private async void OnScheduleClicked(object sender, EventArgs e)
    {
        if (JsonSerializer.Deserialize<bool>(Preferences.Get("working", "")) == false)
        {
            await Shell.Current.GoToAsync(nameof(SchedulePage));
        }
    }

    private async void OnWeatherClicked(object sender, EventArgs e)
    {
        if (JsonSerializer.Deserialize<bool>(Preferences.Get("working", "")) == false)
        {
            await Shell.Current.GoToAsync(nameof(WeatherPage));
        }
    }
}
