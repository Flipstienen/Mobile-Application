using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MobileApplication.Core.Helpers;
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
