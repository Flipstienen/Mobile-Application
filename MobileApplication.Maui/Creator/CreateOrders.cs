using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;

namespace MobileApplication.Maui.Creator
{
    public class CreateOrders
    {
        public async Task CreateNewOrderAsync()
        {
            try
            {
                for (int i = 0; i < 15; i++)
                {
                    var newOrder = new Order
                    {
                        OrderDate = DateTime.UtcNow,
                        CustomerId = 1,
                    };
                    await ApiHelper.Instance.CreateOrderAsync<Order>(newOrder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error", ex.Message, "OK");
            }
        }
    }
}