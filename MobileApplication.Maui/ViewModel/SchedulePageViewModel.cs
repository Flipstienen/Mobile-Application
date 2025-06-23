using System.Collections.ObjectModel;
using System.Text.Json;

using CommunityToolkit.Mvvm.ComponentModel;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using MobileApplication.Maui.Creator;

namespace MobileApplication.Maui.ViewModel
{
    public partial class SchedulePageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<OrderDisplayItem> orders = new();
        private LastOrder lastOrder = new LastOrder();

        private CreateOrders createOrders = new CreateOrders();
        public SchedulePageViewModel()
        {

        }

        public async Task LoadOrdersAsync()
        {
            var fullOrder = new List<Order>();
            if (Preferences.ContainsKey("LastOrder") == true)
            {
                fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));
            }
            string apiKey = EnvHelper.Instance.GetEnvironmentVariable("API_KEY", "");
            var DeliveryServices = await ApiHelper.Instance.GetAsync<DeliveryService>($"/api/DeliveryServices/{apiKey}");

            if (DeliveryServices == null)
            {
                return;
            }

            try
            {
                var lastDate = Preferences.Get("LastUpdateDate", "");

                if (Preferences.ContainsKey("Orders") == false || Preferences.Get("Orders", "") == "")
                {
                    await createOrders.CreateNewOrderAsync();
                }
                else
                    try
                    {
                        List<int> orderList = JsonSerializer.Deserialize<List<int>>(Preferences.Get("Orders", "")) ?? new List<int>();
                        await ApiHelper.Instance.GetAsync<Order>($"/api/order/{orderList.FirstOrDefault()}");
                    }

                    catch (Exception ex)
                    {
                        await createOrders.CreateNewOrderAsync();
                    }

                if (!DateTime.TryParse(lastDate, out DateTime parsedLastDate) || parsedLastDate.Date < DateTime.Today)
                {
                    await createOrders.CreateNewOrderAsync();
                    fullOrder = null;
                }

                if (fullOrder == null)
                {
                    await lastOrder.CreateLastOrder(DeliveryServices.id);
                    fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", "")) ?? new List<Order>();
                }

                Orders = new ObservableCollection<OrderDisplayItem>(fullOrder.OrderBy(o => o.Id).Select(o => new OrderDisplayItem
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerDisplay = $"Customer: {o.Customer?.Name ?? "N/A"}, Address: {o.Customer?.Address ?? "N/A"}",
                    DeliveryStateDisplay = $"{o.DeliveryStates.LastOrDefault().State}"
                }));
            }
            catch (Exception ex)
            {
                return;
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
