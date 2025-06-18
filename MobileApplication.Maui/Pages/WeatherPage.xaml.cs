using System.Text.Json;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace MobileApplication.Maui.Pages;

public partial class WeatherPage : ContentPage
{
    string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY_WEATHER", "");

    public WeatherPage()
    {
        InitializeComponent();
        LoadWeather();
    }

    private async void LoadWeather()
    {
        try
        {
            string city = await GetCurrentCity();
            if (string.IsNullOrEmpty(city))
                city = "Heerlen"; // Zuyd fallback (cause Zuyd is in Heerlen)

            CityLabel.Text = city;
            DateLabel.Text = DateTime.Now.ToString("dddd, MMMM dd");

            var url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={city}&days=4&aqi=no";
            using HttpClient client = new();
            var response = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            var current = root.GetProperty("current");
            var forecastDays = root.GetProperty("forecast").GetProperty("forecastday");

            var iconUrl = "https:" + current.GetProperty("condition").GetProperty("icon").GetString();


            // Current
            var condition = current.GetProperty("condition").GetProperty("text").GetString();
            var tempC = current.GetProperty("temp_c").GetDecimal();
            var windKph = current.GetProperty("wind_kph").GetDecimal();
            var windDir = current.GetProperty("wind_dir").GetString();

            var todayForecast = forecastDays[0].GetProperty("day");
            var lowTemp = todayForecast.GetProperty("mintemp_c").GetDecimal();
            var highTemp = todayForecast.GetProperty("maxtemp_c").GetDecimal();
            var rainChance = todayForecast.GetProperty("daily_chance_of_rain").GetInt32().ToString();

            TemperatureLabel.Text = $"{tempC}°C";
            ConditionLabel.Text = condition;
            LowLabel.Text = $"Low {lowTemp}°C";
            HighLabel.Text = $"High {highTemp}°C";
            WindLabel.Text = $"Wind {windKph}kph / {windDir}";
            RainChanceLabel.Text = $"Chance of Rain {rainChance}%";
            WeatherIcon.Source = ImageSource.FromUri(new Uri(iconUrl));

            // Forecast
            var forecastList = new List<ForecastDisplay>();

            for (int i = 1; i <= 3; i++)
            {
                var day = forecastDays[i];
                var date = DateTime.Parse(day.GetProperty("date").GetString() ?? "");
                var summary = day.GetProperty("day").GetProperty("condition").GetProperty("text").GetString();
                var lowForecastTemp = day.GetProperty("day").GetProperty("mintemp_c").GetDecimal();
                var HighForecastTemp = day.GetProperty("day").GetProperty("maxtemp_c").GetDecimal();
                var avgTemp = day.GetProperty("day").GetProperty("avgtemp_c").GetDecimal();

                forecastList.Add(new ForecastDisplay
                {
                    Date = date.ToString("dddd, MMMM dd"),
                    Summary = $"{summary}, {lowForecastTemp}/{HighForecastTemp}°C"
                });
            }

            ForecastCollection.ItemsSource = forecastList;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load weather: {ex.Message}", "OK");
        }
    }

    private async Task<string> GetCurrentCity()
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });
            }

            if (location != null)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                var placemark = placemarks?.FirstOrDefault();
                return placemark?.Locality;
            }
        }
        catch
        {
        }
        return null;
    }
}
