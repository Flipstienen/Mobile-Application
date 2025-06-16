using MobileApplication.Core.Helpers;
using MobileApplication.Core.Model;
using System;
using System.Linq;
using System.Text.Json;

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
            var shallowOrders = await ApiHelper.Instance.GetAsync<List<Order>>("/api/Order");
            var fullOrders = new List<Order>();

            foreach (var o in shallowOrders)
            {
                var fullOrder = await ApiHelper.Instance.GetAsync<Order>($"/api/Order/{o.Id}");
                fullOrders.Add(fullOrder);
            }

            OrderCollectionView.ItemsSource = fullOrders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                CustomerDisplay = $"Customer: {o.Customer?.Name ?? "N/A"}, Address: {o.Customer?.Address ?? "N/A"}",
                ProductsDisplay = "Products: " + string.Join(", ", o.Products?.Select(p => p.Name) ?? new List<string> { "N/A" }),
                DeliveryStateDisplay = o.DeliveryStates != null && o.DeliveryStates.Any()
                    ? $"Latest Delivery State: {o.DeliveryStates.OrderByDescending(ds => ds.DateTime).First().State} @ {o.DeliveryStates.OrderByDescending(ds => ds.DateTime).First().DateTime:yyyy-MM-dd HH:mm}"
                    : "No delivery states"
            }).ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}