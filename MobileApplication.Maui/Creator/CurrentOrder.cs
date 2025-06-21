using System.Text.Json;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.Creator
{
    public class CurrentOrder
    {
        public async Task CreateCurrentOrder()
        {
            var fullOrder = new List<Order>();
            fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));
            fullOrder = fullOrder.Where(o => o.DeliveryStates.LastOrDefault().State < 3).ToList();
            if (fullOrder.Count == 0)
            {
                Console.WriteLine("Error", "No orders available for delivery.", "OK");
                return;
            }
            Order currentOrder = fullOrder.FirstOrDefault();
            Preferences.Set("Current Order", JsonSerializer.Serialize(currentOrder));
        }
    }
}
