using Foundation;
using UIKit;
using DatadogMauiSample.Config;
using Datadog.iOS.DatadogCore;
using Datadog.iOS.DatadogLogs;
using Datadog.iOS.DatadogTrace;

namespace DatadogMauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        // Load Datadog configuration from environment
        DatadogConfig.LoadFromEnvironment();

        InitializeDatadog();

        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        Console.WriteLine("[Datadog] iOS bindings are now available!");
        Console.WriteLine("[Datadog] Bindings loaded successfully:");
        Console.WriteLine($"[Datadog] - DatadogCore: {typeof(DDDatadog).FullName}");
        Console.WriteLine($"[Datadog] - DatadogLogs: {typeof(DDLogs).FullName}");
        Console.WriteLine($"[Datadog] - DatadogTrace: {typeof(DDTrace).FullName}");
        Console.WriteLine($"[Datadog] Environment: {DatadogConfig.Environment}");
        Console.WriteLine($"[Datadog] Service: {DatadogConfig.ServiceName}");
        
        // TODO: Add full Datadog initialization once we verify the bindings work
        // The bindings are available but we need to determine the correct initialization API
        // from the Datadog iOS SDK documentation
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
