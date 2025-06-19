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
            var orderlist = await ApiHelper.Instance.GetAsync<List<Order>>("/api/Order");

            if (orderlist != null)
            {
                int orders = orderlist.Count();
                while (fullOrder.Count() != 15)
                {
                    fullOrder = new List<Order>();
                    for (int i = 0; orders >= 1; orders -= 1)
                    {
                        var order = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{orders}");
                        if (order.DeliveryStates.Count() != 0)
                        {
                            if (order.DeliveryStates.LastOrDefault().DeliveryServiceId == deliveryServiceId)
                            {
                                fullOrder.Add(order);
                            }
                            if (fullOrder.Count() == 15)
                            {
                                Preferences.Set("LastOrder", JsonSerializer.Serialize(fullOrder));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
