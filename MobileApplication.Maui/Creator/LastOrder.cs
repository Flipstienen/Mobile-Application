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
    public class LastOrder
    {
        public async Task CreateLastOrder(int deliveryServiceId)
        {
            var fullOrder = new List<Order>();
            List<int> orderlist = JsonSerializer.Deserialize<List<int>>(Preferences.Get("Orders",""));

            if (orderlist != null)
            {
                
                fullOrder = new List<Order>();
                foreach(var orders in orderlist)
                {
                    var order = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{orders}");
                    if (order.DeliveryStates.Count() != 0)
                    {
                        if (order.DeliveryStates.LastOrDefault().DeliveryServiceId == deliveryServiceId)
                        {
                                fullOrder.Add(order);
                        }
                    }
                }
                Preferences.Set("LastOrder", JsonSerializer.Serialize(fullOrder));
            }
        }
    }
}
