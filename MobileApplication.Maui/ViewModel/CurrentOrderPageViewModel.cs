using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.ViewModel
{
    public partial class CurrentOrderPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private OrderDisplayItem currentOrder;

        public CurrentOrderPageViewModel()
        {
        }
        
        public async Task LoadRandomOrderAsync()
        {
            var fullOrder = new Order();
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");

            var deliveryService = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            var orderList = await ApiHelper.Instance.GetAsync<List<Order>>($"/api/Order?deliveryServiceId={deliveryService.id}");

            if (orderList == null || orderList.Count == 0)
            {
                Console.WriteLine($"There are no orders");
                CurrentOrder = null;
                return;
            }
            
            try
            {
                fullOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
            } 
            
            catch
            {
                var recentOrders = orderList.TakeLast(15).ToList();

                var random = new Random();
                var randomOrder = recentOrders[random.Next(recentOrders.Count)];

                fullOrder = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{randomOrder.Id}");
                if (fullOrder == null)
                {
                    Console.WriteLine("notting in the order.");
                    CurrentOrder = null;
                    return;
                }
            }

            Preferences.Set("Current Order", JsonSerializer.Serialize(fullOrder));


            CurrentOrder = new OrderDisplayItem
            {
                Id = fullOrder.Id,
                OrderDate = fullOrder.OrderDate,
                CustomerDisplay = $"Customer: {fullOrder.Customer?.Name ?? "N/A"}, Address: {fullOrder.Customer?.Address ?? "N/A"}",
                DeliveryStateDisplay = $"Last Delivery State: {fullOrder.DeliveryStates.LastOrDefault().State}"
            };
        }
    }

    public class OrderDisplayItem
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerDisplay { get; set; }
        public string DeliveryStateDisplay { get; set; }
    }
}
