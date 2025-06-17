using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui;

public partial class SchedulePage : ContentPage
{
	public SchedulePage()
	{
		InitializeComponent();
	}


    protected override async void OnAppearing()
    {
        string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");

        if (string.IsNullOrEmpty(apiKey))
        {
            await DisplayAlert("Error", "API key is missing", "OK");
            return;
        }

        else
        {
            await DisplayAlert("work", "API key is Not missing", "OK");
            var order = ApiHelper.Instance.GetAsync<APIReturn>(apiKey);
            Console.WriteLine("je moeder ",order);
            return;
        }
    }
}