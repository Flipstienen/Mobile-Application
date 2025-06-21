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
    public class CreateOrders
    {
        public async Task CreateNewOrderAsync()
        {
            Preferences.Remove("Orders");
            Preferences.Remove("LastOrder");
            Preferences.Remove("Current Order");
            try
            {
                for (int i = 0; i < 15; i++)
                {
                    var newOrder = new Order
                    {
                        OrderDate = DateTime.UtcNow,
                        CustomerId = 1,
                    };
                    int id = await ApiHelper.Instance.CreateOrderAsync<Order>(newOrder);
                    if (Preferences.ContainsKey("Orders"))
                    {
                        List<int> orders = JsonSerializer.Deserialize<List<int>>(Preferences.Get("Orders", ""));
                        orders.Add(id);
                        Preferences.Set("Orders",JsonSerializer.Serialize(orders));
                    }
                    else
                    {
                        List<int> one = new List<int> { id};
                        Preferences.Set("Orders", JsonSerializer.Serialize(one));
                    }
                }
                Preferences.Set("LastUpdateDate", DateTime.Today.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", ex.Message, "OK");
            }
        }
    }
}