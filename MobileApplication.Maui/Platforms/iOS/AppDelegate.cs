using Foundation;
using UIKit;
using Plugin.LocalNotification;

namespace MobileApplication.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();


    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        LocalNotificationCenter.Current.RequestNotificationPermission();
        return base.FinishedLaunching(app, options);
    }

}
