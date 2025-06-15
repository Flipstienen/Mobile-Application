using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using System;
using System.Linq;

namespace MobileApplication.Maui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrderAsync();
    }

    private async Task LoadOrderAsync()
    {
        try
        {
            var order = await ApiHelper.Instance.GetAsync<Order>("/api/Order/1");

            OrderIdLabel.Text = $"Order ID: {order.Id}, Date: {order.OrderDate:yyyy-MM-dd}";
            CustomerLabel.Text = $"Customer: {order.Customer?.Name}, Address: {order.Customer?.Address}";

            ProductsLabel.Text = "Products: " + string.Join(", ", order.Products.Select(p => p.Name));

            var latestState = order.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault();
            LatestDeliveryStateLabel.Text = $"Latest Delivery State: {latestState?.State} @ {latestState?.DateTime:yyyy-MM-dd HH:mm}";
        }
        catch (Exception ex)
        {
            OrderIdLabel.Text = $"Error: {ex.Message}";
        }
    }
}