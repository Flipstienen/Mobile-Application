using MobileApplication.Maui.Pages;
using MobileApplication.Maui;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.ViewModel
{
    public class SchedulePageViewModel
    {
        public async Task LoadOrder()
        {
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
            var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            if (DeliveryServices == null)
            {
                Console.WriteLine("Error", "Delivery service not found!", "OK");
                return;
            }

            try
            {
                await CreateNewOrderAsync();
                var orderlist = await ApiHelper.Instance.GetAsync<List<Order>>($"/api/Order?deliveryServiceId={DeliveryServices.id}");

                if (order != null)
                {
                    OrderIdLabel.Text = order.Id.ToString();
                    CustomerNameLabel.Text = order.Customer?.Name ?? "Unknown";
                    CustomerAdressLabel.Text = order.Customer?.Address ?? "Unknown";
                    OrderDateLabel.Text = order.OrderDate.ToString("g");
                    if (order.DeliveryStates != null && order.DeliveryStates.Any())
                    {
                        DeliveryServiceLabel.Text = order.DeliveryStates.LastOrDefault().State.ToString();
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
                var lastDate = Preferences.Get("LastUpdateDate", "");
                if (!DateTime.TryParse(lastDate, out DateTime parsedLastDate) || parsedLastDate.Date < DateTime.Today)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
                        var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");
                        var newOrder = new Order
                        {
                            OrderDate = DateTime.UtcNow,
                            CustomerId = 1,
                        };

                        await ApiHelper.Instance.CreateOrderAsync<Order>(newOrder);
                    }
                    Preferences.Set("LastUpdateDate", DateTime.Today.ToString("yyyy-MM-dd"));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

