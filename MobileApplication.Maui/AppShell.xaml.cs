namespace MobileApplication.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
    }
}
