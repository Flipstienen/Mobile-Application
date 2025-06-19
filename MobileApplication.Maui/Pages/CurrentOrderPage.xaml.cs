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
        Console.WriteLine("DEBUG: OnAppearing called");
        if (BindingContext is CurrentOrderPageViewModel viewModel)
        {
            Console.WriteLine("DEBUG: Found ViewModel in BindingContext");
            await viewModel.LoadRandomOrderAsync();
        }
        else
        {
            Console.WriteLine("DEBUG: BindingContext is not CurrentOrderPageViewModel");
        }
    }

}