using MobileApplication.Core.Model;
using MobileApplication.Core.Helpers;
namespace MobileApplication.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
	
	

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}