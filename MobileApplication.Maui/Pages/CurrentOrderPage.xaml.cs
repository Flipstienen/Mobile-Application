using System.Text.Json;

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
        await DisplayAlert("completed", "Order is Completed", "ok");
        await Shell.Current.Navigation.PopToRootAsync();
        var item = JsonSerializer.Deserialize<Order>(Preferences.Get("Current Order", ""));
        CompletedOrder completedOrder = new CompletedOrder();
        await completedOrder.OrderCompleted(item.Id);

        var currentOrderPageViewModel = new CurrentOrderPageViewModel();
        await currentOrderPageViewModel.LoadCurrentOrderAsync();
        if (Preferences.ContainsKey("Completed"))
        {
            await DisplayAlert("Order Completed", Preferences.Get("Completed", ""), "OK");
            Preferences.Remove("Completed");
        }
        await Shell.Current.GoToAsync(nameof(CurrentOrderPage));
    }
}