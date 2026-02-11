using Microsoft.Extensions.Logging;
using Datadog.Maui.Extensions;

namespace DatadogMauiSample;

/// <summary>
/// Simplified example of MauiProgram using UseDatadogFromConfiguration().
/// This eliminates the need for custom DatadogSettings classes and manual configuration loading.
/// </summary>
public static class MauiProgramSimple
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                // Register custom WebView handler for Android to enable Datadog tracking
                handlers.AddHandler<WebView, Platforms.Android.DatadogWebViewHandler>();
#endif
            })
            // Configure Datadog - reads from appsettings.json automatically
            .UseDatadogFromConfiguration();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
