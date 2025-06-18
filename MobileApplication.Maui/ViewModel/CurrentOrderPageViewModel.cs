using System;
using System.Collections.Generic;
using System.Linq;
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
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");

            // Get a single delivery service object (not a list) based on API key
            var deliveryService = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            if (deliveryService == null)
            {
                Console.WriteLine("DEBUG: No delivery service found!");
                CurrentOrder = null;
                return;
            }

            Console.WriteLine($"DEBUG: Found delivery service: {deliveryService.name}");

            // Get the orders for the delivery service
            var orderList = await ApiHelper.Instance.GetAsync<List<Order>>($"/api/Order?deliveryServiceId={deliveryService.id}");

            if (orderList == null || orderList.Count == 0)
            {
                Console.WriteLine($"DEBUG: No orders found for delivery service '{deliveryService.name}'");
                CurrentOrder = null;
                return;
            }

            // Take the last 15 orders or fewer if not enough
            var recentOrders = orderList.TakeLast(15).ToList();

            var random = new Random();
            var randomOrder = recentOrders[random.Next(recentOrders.Count)];

            // Fetch full details for the selected order
            var fullOrder = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{randomOrder.Id}");

            if (fullOrder == null)
            {
                Console.WriteLine("DEBUG: Selected order details not found.");
                CurrentOrder = null;
                return;
            }

            CurrentOrder = new OrderDisplayItem
            {
                Id = fullOrder.Id,
                OrderDate = fullOrder.OrderDate,
                CustomerDisplay = $"Customer: {fullOrder.Customer?.Name ?? "N/A"}, Address: {fullOrder.Customer?.Address ?? "N/A"}",
                DeliveryStateDisplay = fullOrder.DeliveryStates != null && fullOrder.DeliveryStates.Count > 0
                    ? $"Last Delivery State: {fullOrder.DeliveryStates[^1].State}"
                    : "Last Delivery State: N/A"
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
