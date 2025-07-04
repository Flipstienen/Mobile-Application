using MobileApplication.Maui.ViewModel;

namespace MobileApplication.Maui.Pages;

public partial class SchedulePage : ContentPage
{
    public SchedulePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SchedulePageViewModel viewModel)
        {
            await viewModel.LoadOrdersAsync();
        }
    }
}