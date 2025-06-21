using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using MobileApplication.Maui.Creator;

namespace MobileApplication.Maui.ViewModel
{
    public partial class CurrentOrderPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private OrderDisplayItem currentOrderDisplay;
        private LastOrder lastOrder = new LastOrder();
        private CreateOrders createOrders = new CreateOrders();
        private CurrentOrder currentOrderCreate = new CurrentOrder();

        public CurrentOrderPageViewModel()
        {
        }

        public async Task LoadCurrentOrderAsync()
        {
            var lastDate = Preferences.Get("LastUpdateDate", "");
            var currentOrder = new Order();
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
            var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            if (Preferences.ContainsKey("Current Order") == false || PreferencesCheck() == true)
            {
                if (Preferences.ContainsKey("LastOrder") == false)
                {
                    await createOrders.CreateNewOrderAsync();
                    await lastOrder.CreateLastOrder(DeliveryServices.id);
                }

                var fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));
                int count = fullOrder.Where(o => o.DeliveryStates.LastOrDefault().State >= 3).Count();
                int all = fullOrder.Count();
                if (count == all)
                {
                    await createOrders.CreateNewOrderAsync();
                    await lastOrder.CreateLastOrder(DeliveryServices.id);
                    Preferences.Set("Completed", "all orders completed");
                }
            }

            else if (!DateTime.TryParse(lastDate, out DateTime parsedLastDate) || parsedLastDate.Date < DateTime.Today)
            {
                await createOrders.CreateNewOrderAsync();
            }


            if (Preferences.ContainsKey("Current Order"))
            {
                currentOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
            }

            else
            {
                await currentOrderCreate.CreateCurrentOrder();
                currentOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
            }
                CurrentOrderDisplay = new OrderDisplayItem
                {
                    Id = currentOrder.Id,
                    OrderDate = currentOrder.OrderDate,
                    CustomerDisplay = $"Customer: {currentOrder.Customer?.Name ?? "N/A"}, Address: {currentOrder.Customer?.Address ?? "N/A"}",
                    DeliveryStateDisplay = $"Last Delivery State: {currentOrder.DeliveryStates.LastOrDefault().State}"
                };
        }

    private bool PreferencesCheck()
        {
            var currentOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
            if (currentOrder.DeliveryStates.LastOrDefault().State >= 3)
            {
                Preferences.Remove("Current Order");
                return true;
            }
            return false;
        }

        public class OrderDisplayItem
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
            public string CustomerDisplay { get; set; }
            public string DeliveryStateDisplay { get; set; }
        }
    }
}