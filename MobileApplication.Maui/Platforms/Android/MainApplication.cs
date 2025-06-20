using Android.App;
using Android.Runtime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MobileApplication.Maui;


[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership) { }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void OnCreate()
    {
        base.OnCreate();
        LocalNotificationCenter.CreateNotificationChannels(new List<NotificationChannelRequest>
       {
           new NotificationChannelRequest
           {
               Id = "default",
               Name = "General",
               Importance = AndroidImportance.Default,
           }
       });
    }
}

