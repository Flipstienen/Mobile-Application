using System.Text.Json;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.Creator
{
    public class CompletedOrder
    {
        public async Task OrderCompleted(int orderId)
        {
            var fullOrder = JsonSerializer.Deserialize<List<Order>>(Preferences.Get("LastOrder", ""));
            Order Order = fullOrder.Where(o => o.Id == orderId).LastOrDefault();
            Order.DeliveryStates.LastOrDefault().State = 3;
            Preferences.Set("LastOrder", JsonSerializer.Serialize(fullOrder));
            Preferences.Set("Current Order", JsonSerializer.Serialize(Order));
        }
    }
}
