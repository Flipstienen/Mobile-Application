using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MobileApplication.Core.Model;
using MobileApplication.Maui.Creator;
using MobileApplication.Maui.ViewModel;

namespace MobileApplication.Maui.Pages;

public partial class CurrentOrderPage : ContentPage
{
    public CurrentOrderPage()
    {
        InitializeComponent();
        BindingContext = new CurrentOrderPageViewModel();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CurrentOrderPageViewModel viewModel)
        {
            await viewModel.LoadCurrentOrderAsync();
        }
    }

    private async void OnClickComplete(object send, EventArgs e)
    {
        var item = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
        CompletedOrder completedOrder = new CompletedOrder();
        await completedOrder.OrderCompleted(item.Id);

        var currentOrderPageViewModel = new CurrentOrderPageViewModel();
        await currentOrderPageViewModel.LoadCurrentOrderAsync();
        await Shell.Current.GoToAsync(nameof(CurrentOrderPage));
    }
}