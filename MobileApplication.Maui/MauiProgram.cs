using Microsoft.Extensions.Logging;
using Shiny;
using Shiny.Notifications;

namespace MobileApplication.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        DotNetEnv.Env.Load();

        builder
            .UseMauiApp<App>()
            .UseShiny()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddNotifications<SampleNotificationDelegate>();
#if DEBUG
        builder.Logging.AddDebug();

#endif

        return builder.Build();
    }
}

public class SampleNotificationDelegate : INotificationDelegate
{
    public Task OnEntry(NotificationResponse response)
    {
        return Task.CompletedTask;
    }
}