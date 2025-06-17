using System.Runtime.InteropServices.Marshalling;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.Pages;

public partial class SchedulePage : ContentPage
{
    public SchedulePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrder();
    }
    private async Task LoadOrder()
    {
        string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
        var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

        if (DeliveryServices == null)
        {
            await DisplayAlert("Error", "Delivery service not found!", "OK");
            return;
        }

        try
        {
            await CreateNewOrderAsync();
            var orderlist = await ApiHelper.Instance.GetAsync<List<Order>>($"/api/Order?deliveryServiceId={DeliveryServices.id}");
            var order = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/1");

            if (order != null)
            {
                OrderIdLabel.Text = order.Id.ToString();
                CustomerNameLabel.Text = order.Customer?.Name ?? "Unknown";
                CustomerAdressLabel.Text = order.Customer?.Address ?? "Unknown";
                ProductsLabel.Text = (order.Products != null && order.Products.Any())
                    ? string.Join(", ", order.Products.Select(p => p.Name))
                    : "No products";
                OrderDateLabel.Text = order.OrderDate.ToString("g");
                if (order.DeliveryStates != null && order.DeliveryStates.Any())
                {
                    DeliveryServiceLabel.Text = order.DeliveryStates.LastOrDefault().Id.ToString();
                }
                else
                {
                    DeliveryServiceLabel.Text = "No delivery states found";
                }
            }
            else
            {
                Console.WriteLine("Order details not found.");
                await DisplayAlert("Warning", "Order details not found.", "OK");
                OrderIdLabel.Text = "Not Found";
                CustomerNameLabel.Text = "Not Found";
                CustomerAdressLabel.Text = "Not Found";
                ProductsLabel.Text = "Not Found";
                OrderDateLabel.Text = "Not Found";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            await DisplayAlert("Error", $"API Error: {ex.Message}", "OK");
        }
    }

    private async Task CreateNewOrderAsync()
    {
        try
        {
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
            var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                CustomerId = 1,
                Products = new List<Product>
                {
                    new Product
                            {
                                Id = 1,
                                Name = "Nebuchadnezzar",
                                Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen",
                                Price = 10000.0,
                            }
                },
                DeliveryStates = new List<DeliveryState>
                {
                    new DeliveryState
                    {
                        State = 1,
                        DateTime = DateTime.UtcNow,
                        DeliveryServiceId = DeliveryServices.id
                    }
                }
            };
            var result = await ApiHelper.Instance.PostAsync<Order>("/api/Order", newOrder);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }
}

