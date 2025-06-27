using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using MobileApplication.Maui.Creator;
using Shiny;
using Shiny.Notifications;

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
            int count = 0;
            var fullOrder = new List<Order>();

            var lastDate = Preferences.Get("LastUpdateDate", "");
            
            var currentOrder = new Order();
            
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");

            var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            bool isNewOrderCreated = false;

            if (!Preferences.ContainsKey("Current Order") || PreferencesCheck())
            {
                if (!Preferences.ContainsKey("LastOrder"))
                {
                    await createOrders.CreateNewOrderAsync();
                    await lastOrder.CreateLastOrder(DeliveryServices.id);
                    isNewOrderCreated = true;
                }
                fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));

                count = fullOrder.Count(o => o.DeliveryStates.LastOrDefault().State >= 3);
                if (count == fullOrder.Count())
                {
                    await createOrders.CreateNewOrderAsync();
                    await lastOrder.CreateLastOrder(DeliveryServices.id);
                    Preferences.Set("Completed", "all orders completed");
                    isNewOrderCreated = true;
                }
            }
            else if (!DateTime.TryParse(lastDate, out DateTime parsedLastDate) || parsedLastDate.Date < DateTime.Today)
            {
                await createOrders.CreateNewOrderAsync();
                await lastOrder.CreateLastOrder(DeliveryServices.id);
                isNewOrderCreated = true;
            }


            if (Preferences.ContainsKey("Current Order"))
            {
                currentOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
            }

            else
            {
                await currentOrderCreate.CreateCurrentOrder();
                currentOrder = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
                isNewOrderCreated = true;
            }
            fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));
            count = fullOrder.Count(o => o.DeliveryStates.LastOrDefault().State >= 3);
            CurrentOrderDisplay = new OrderDisplayItem
            {
                Id = currentOrder.Id,
                OrderDate = currentOrder.OrderDate,
                CustomerDisplay = $"Customer: {currentOrder.Customer?.Name ?? "N/A"}, Address: {currentOrder.Customer?.Address ?? "N/A"}",
                CompletedOrders = $"Completed: {count}/{fullOrder.Count()}"
            };

            if (isNewOrderCreated)
            {
                var notif = new Notification
                {
                    Title = "New Order Created",
                    Message = $"Order #{currentOrder.Id} is now active.",
                    BadgeCount = 1
                };

                var accessStatus = await Shiny.Hosting.Host.GetService<INotificationManager>().RequestAccess();
                if (accessStatus == AccessState.Available)
                {
                    await Shiny.Hosting.Host.GetService<INotificationManager>().Send(notif);
                }
            }
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
            public string CompletedOrders { get; set; }
        }
    }
}