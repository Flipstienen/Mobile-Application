using System.Collections.ObjectModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MobileApplication.Maui.ViewModel
{
    public partial class SchedulePageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<OrderDisplayItem> orders = new();
        public SchedulePageViewModel()
        {
            
        }

        public async Task LoadOrdersAsync()
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
                var fullLastOrders =  new List<Order>();
                var orderlist = await ApiHelper.Instance.GetAsync<List<Order>>($"/api/Order?deliveryServiceId={DeliveryServices.id}");
                if (orderlist != null)
                {
                    foreach(var item in orderlist.TakeLast(15))
                    {
                        fullLastOrders.Add(await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{item.Id}"));
                    }
                    Orders = new ObservableCollection<OrderDisplayItem>(fullLastOrders.Select(o => new OrderDisplayItem
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        CustomerDisplay = $"Customer: {o.Customer?.Name ?? "N/A"}, Address: {o.Customer?.Address ?? "N/A"}",
                        DeliveryStateDisplay = $"Last Delivery State: {o.DeliveryStates.LastOrDefault().State}"
                    })); 
                }
                
                
                else
                {
                    Console.WriteLine("Order details not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }
        }

        private async Task CreateNewOrderAsync()
        {
            try
            {
                var lastDate = Preferences.Get("LastUpdateDate", "");
                if (!DateTime.TryParse(lastDate, out DateTime parsedLastDate) || parsedLastDate.Date < DateTime.Today)
                {
                    string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
                    var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");
                    for (int i = 0; i < 15; i++)
                    {
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
                Console.WriteLine("Error", ex.Message, "OK");
            }
        }
    }
}

public class OrderDisplayItem
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerDisplay { get; set; }
    public string DeliveryStateDisplay { get; set; }
}
