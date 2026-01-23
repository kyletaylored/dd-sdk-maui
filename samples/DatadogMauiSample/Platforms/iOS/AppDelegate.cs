using Foundation;
using UIKit;
using DatadogMauiSample.Config;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.Logs;
using Datadog.iOS.Trace;
using Datadog.iOS.CrashReporting;
using Datadog.iOS.SessionReplay;
using Datadog.iOS.WebViewTracking;

namespace DatadogMauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        // Configure tab bar appearance before MAUI initializes
        ConfigureTabBarAppearance();

        // Load Datadog configuration from environment
        DatadogConfig.LoadFromEnvironment();

        InitializeDatadog();

        var result = base.FinishedLaunching(application, launchOptions);

        // Remove liquid glass effect after MAUI creates the tab bar
        RemoveLiquidGlassEffect();

        return result;
    }

    private void RemoveLiquidGlassEffect()
    {
        // Find and remove the _UIBarBackground view that creates the liquid glass effect
        if (UIApplication.SharedApplication.KeyWindow?.RootViewController is UIViewController rootVC)
        {
            RemoveBackgroundFromTabBar(rootVC);
        }
    }

    private void RemoveBackgroundFromTabBar(UIViewController viewController)
    {
        // Check if it's a UITabBarController or contains one
        if (viewController is UITabBarController tabBarController)
        {
            RemoveBarBackground(tabBarController.TabBar);
        }
        else if (viewController.PresentedViewController != null)
        {
            RemoveBackgroundFromTabBar(viewController.PresentedViewController);
        }

        // Check child view controllers
        foreach (var child in viewController.ChildViewControllers)
        {
            RemoveBackgroundFromTabBar(child);
        }
    }

    private void RemoveBarBackground(UITabBar tabBar)
    {
        foreach (var subview in tabBar.Subviews)
        {
            var typeName = subview.GetType().Name;
            // Remove _UIBarBackground which creates the liquid glass effect
            if (typeName == "_UIBarBackground")
            {
                subview.RemoveFromSuperview();
            }
        }
    }

    private void ConfigureTabBarAppearance()
    {
        // Configure tab bar for iOS 15+
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            var appearance = new UITabBarAppearance();
            appearance.ConfigureWithOpaqueBackground();

            // Set solid background color
            appearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);

            // Disable shadow/separator
            appearance.ShadowColor = UIColor.Clear;

            // Configure tab bar items using the inline item appearance
            var itemAppearance = appearance.InlineLayoutAppearance;

            // Normal state (unselected)
            itemAppearance.Normal.IconColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f);
            itemAppearance.Normal.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f)
            };

            // Selected state
            itemAppearance.Selected.IconColor = UIColor.White;
            itemAppearance.Selected.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            // Apply to all layout types
            appearance.StackedLayoutAppearance = itemAppearance;
            appearance.InlineLayoutAppearance = itemAppearance;
            appearance.CompactInlineLayoutAppearance = itemAppearance;

            // Apply the appearance
            UITabBar.Appearance.StandardAppearance = appearance;
            UITabBar.Appearance.ScrollEdgeAppearance = appearance;
        }
        else
        {
            // Fallback for iOS 14 and earlier
            UITabBar.Appearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);
            UITabBar.Appearance.TintColor = UIColor.White;
            UITabBar.Appearance.UnselectedItemTintColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f);
            UITabBar.Appearance.BarTintColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);
        }
    }

    private void InitializeDatadog()
    {
        try
        {
            Console.WriteLine($"[Datadog] Initializing for iOS");
            Console.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            Console.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

            // Safely mask the client token
            var maskedToken = string.IsNullOrEmpty(DatadogConfig.IosClientToken)
                ? "NOT_CONFIGURED"
                : DatadogConfig.IosClientToken.Length > 14
                    ? $"{DatadogConfig.IosClientToken.Substring(0, 10)}...{DatadogConfig.IosClientToken.Substring(DatadogConfig.IosClientToken.Length - 4)}"
                    : "***CONFIGURED***";

            Console.WriteLine($"[Datadog] - Client Token: {maskedToken}");
            Console.WriteLine($"[Datadog] - RUM Application ID: {DatadogConfig.IosRumApplicationId}");

            // Initialize Datadog Core
            var configuration = new DDConfiguration(
                clientToken: DatadogConfig.IosClientToken,
                env: DatadogConfig.Environment
            );
            configuration.Service = DatadogConfig.ServiceName;
            configuration.BatchSize = DDBatchSize.Small;
            configuration.UploadFrequency = DDUploadFrequency.Frequent;

            // Initialize SDK with tracking consent
            DDDatadog.InitializeWithConfiguration(configuration, DDTrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            // Set verbosity level for debugging
            if (DatadogConfig.VerboseLogging)
            {
                DDDatadog.VerbosityLevel = DDCoreLoggerLevel.Debug;
            }

            // Enable Logs
            DDLogs.EnableWith(new DDLogsConfiguration());
            Console.WriteLine("[Datadog] Logs enabled");

            // Enable Crash Reporting
            try
            {
                DDCrashReporter.Enable();
                Console.WriteLine("[Datadog] Crash Reporting enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Crash Reporting failed: {ex.Message}");
            }

            // Enable RUM (Real User Monitoring)
            var rumConfiguration = new DDRUMConfiguration(applicationID: DatadogConfig.IosRumApplicationId);
            rumConfiguration.TrackFrustrations = true;
            rumConfiguration.TrackBackgroundEvents = true;
            rumConfiguration.VitalsUpdateFrequency = DDRUMVitalsFrequency.Frequent;
            rumConfiguration.SessionSampleRate = 100.0f;

            DDRUM.EnableWith(rumConfiguration);
            Console.WriteLine("[Datadog] RUM enabled");

            // Enable Session Replay
            try
            {
                var sessionReplayConfig = new DDSessionReplayConfiguration(
                    replaySampleRate: DatadogConfig.SessionReplaySampleRate,
                    textAndInputPrivacyLevel: DDTextAndInputPrivacyLevel.SensitiveInputs,
                    imagePrivacyLevel: DDImagePrivacyLevel.None,
                    touchPrivacyLevel: DDTouchPrivacyLevel.Show
                );

                DDSessionReplay.EnableWith(sessionReplayConfig);
                Console.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            // Enable APM Tracing
            try
            {
                var traceConfig = new DDTraceConfiguration();
                DDTrace.EnableWith(traceConfig);
                Console.WriteLine("[Datadog] APM Tracing enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            // Enable WebView Tracking
            // Note: WebViewTracking requires the WebView instance and allowed hosts list.
            // This should be called when you have a WebView instance, not during app initialization.
            try
            {
                // DDWebViewTracking will be enabled per-WebView in the WebView handler
                Console.WriteLine("[Datadog] WebView tracking configuration skipped (call Enable when WebView is available)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] WebView tracking failed: {ex.Message}");
            }

            Console.WriteLine("[Datadog] Successfully initialized for iOS");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Initialization failed: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
