using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

}