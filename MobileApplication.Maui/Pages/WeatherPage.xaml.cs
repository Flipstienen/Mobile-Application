using System.Text.Json;
using Microsoft.Maui.Controls;
using MobileApplication.Core.Helpers;

namespace MobileApplication.Maui.Pages;

public partial class WeatherPage : ContentPage
{
    string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY_WEATHER", "");
    private const string City = "Heerlen";

    public WeatherPage()
    {
        InitializeComponent();
        LoadWeather();
    }

    private async void LoadWeather()
    {
        try
        {
            var url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={City}&aqi=no";
            using HttpClient client = new();
            var response = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            var current = root.GetProperty("current");
            var condition = current.GetProperty("condition").GetProperty("text").GetString();
            var tempC = current.GetProperty("temp_c").GetDecimal();
            var windKph = current.GetProperty("wind_kph").GetDecimal();
            var windDir = current.GetProperty("wind_dir").GetString();
            var rainChance = current.TryGetProperty("precip_mm", out var precip) ? (precip.GetDecimal() > 0 ? "Chance of Rain 17%" : "Chance of Rain 0%") : "Chance of Rain —";

            var forecastLow = "Low —";

            TemperatureLabel.Text = $"{tempC}°";
            ConditionLabel.Text = condition;
            LowLabel.Text = forecastLow;
            WindLabel.Text = $"Wind {windKph}kph / {windDir}";
            RainChanceLabel.Text = rainChance;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load weather: {ex.Message}", "OK");
        }
    }
}
