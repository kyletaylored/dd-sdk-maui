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
        // Apply tab bar styling for iOS
        ConfigureTabBarAppearance();

        // Load Datadog configuration from environment
        DatadogConfig.LoadFromEnvironment();

        InitializeDatadog();

        return base.FinishedLaunching(application, launchOptions);
    }

    private void ConfigureTabBarAppearance()
    {
        // Configure tab bar appearance for iOS
        var appearance = UITabBar.Appearance;
        appearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4); // Purple background
        appearance.TintColor = UIColor.White; // Selected item color
        appearance.UnselectedItemTintColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f); // Unselected item color (70% opacity)

        // For iOS 15+, we need to use UITabBarAppearance
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            var tabBarAppearance = new UITabBarAppearance();
            tabBarAppearance.ConfigureWithOpaqueBackground();
            tabBarAppearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4); // Purple background

            // Configure normal state (unselected items)
            var normalAppearance = new UITabBarItemAppearance();
            normalAppearance.Normal.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f)
            };
            normalAppearance.Normal.IconColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f);

            // Configure selected state
            normalAppearance.Selected.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
            normalAppearance.Selected.IconColor = UIColor.White;

            tabBarAppearance.StackedLayoutAppearance = normalAppearance;
            tabBarAppearance.InlineLayoutAppearance = normalAppearance;
            tabBarAppearance.CompactInlineLayoutAppearance = normalAppearance;

            UITabBar.Appearance.StandardAppearance = tabBarAppearance;
            UITabBar.Appearance.ScrollEdgeAppearance = tabBarAppearance;
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
                    textAndInputPrivacyLevel: DDTextAndInputPrivacyLevel.MaskSensitiveInputs,
                    imagePrivacyLevel: DDImagePrivacyLevel.MaskNone,
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
