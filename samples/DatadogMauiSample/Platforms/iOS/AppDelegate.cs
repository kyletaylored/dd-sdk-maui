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

/// <summary>
/// iOS application delegate for DatadogMauiSample.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <summary>
    /// Called when the application finishes launching.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <param name="launchOptions">The launch options.</param>
    /// <returns>True if launch was successful.</returns>
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        // Create marker file to show AppDelegate ran
        try
        {
            var markerFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".datadog_initialized");
            File.WriteAllText(markerFile, $"Initialized at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }
        catch { /* Ignore file errors */ }

        // Configure tab bar appearance before MAUI initializes
        ConfigureTabBarAppearance();

        // Load Datadog configuration from environment
        DatadogConfig.LoadFromEnvironment();

        InitializeDatadog();

        var result = base.FinishedLaunching(application, launchOptions ?? new NSDictionary());

        // Remove liquid glass effect after MAUI creates the tab bar
        RemoveLiquidGlassEffect();

        return result;
    }

    private void RemoveLiquidGlassEffect()
    {
        // Find and remove the _UIBarBackground view that creates the liquid glass effect
        UIViewController? rootVC = null;

        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        {
            // iOS 13+ - use window scenes
            var windowScene = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);

            rootVC = windowScene?.Windows.FirstOrDefault()?.RootViewController;
        }
        else
        {
            // iOS 12 and earlier - use deprecated KeyWindow
            #pragma warning disable CA1422
            rootVC = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            #pragma warning restore CA1422
        }

        if (rootVC != null)
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
            System.Diagnostics.Debug.WriteLine($"[Datadog] Initializing for iOS");
            System.Diagnostics.Debug.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

            // Safely mask the client token
            var maskedToken = string.IsNullOrEmpty(DatadogConfig.IosClientToken)
                ? "NOT_CONFIGURED"
                : DatadogConfig.IosClientToken.Length > 14
                    ? $"{DatadogConfig.IosClientToken.Substring(0, 10)}...{DatadogConfig.IosClientToken.Substring(DatadogConfig.IosClientToken.Length - 4)}"
                    : "***CONFIGURED***";

            System.Diagnostics.Debug.WriteLine($"[Datadog] - Client Token: {maskedToken}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] - RUM Application ID: {DatadogConfig.IosRumApplicationId}");

            // Initialize Datadog Core
            var configuration = new DDConfiguration(
                clientToken: DatadogConfig.IosClientToken,
                env: DatadogConfig.Environment
            );
            configuration.Service = DatadogConfig.ServiceName;
            configuration.Site = GetDatadogSite(DatadogConfig.Site);
            configuration.BatchSize = DDBatchSize.Small;
            configuration.UploadFrequency = DDUploadFrequency.Frequent;

            // Initialize SDK with tracking consent
            DDDatadog.InitializeWithConfiguration(configuration, DDTrackingConsent.Granted);

            System.Diagnostics.Debug.WriteLine("[Datadog] Core SDK initialized");

            // Set verbosity level for debugging
            if (DatadogConfig.VerboseLogging)
            {
                DDDatadog.VerbosityLevel = DDCoreLoggerLevel.Debug;
            }

            // Enable Logs
            DDLogs.EnableWith(new DDLogsConfiguration());
            System.Diagnostics.Debug.WriteLine("[Datadog] Logs enabled");

            // Enable Crash Reporting
            try
            {
                DDCrashReporter.Enable();
                System.Diagnostics.Debug.WriteLine("[Datadog] Crash Reporting enabled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] Crash Reporting failed: {ex.Message}");
            }

            // Enable RUM (Real User Monitoring)
            var rumConfiguration = new DDRUMConfiguration(applicationID: DatadogConfig.IosRumApplicationId);
            rumConfiguration.TrackFrustrations = true;
            rumConfiguration.TrackBackgroundEvents = true;
            rumConfiguration.VitalsUpdateFrequency = DDRUMVitalsFrequency.Frequent;
            rumConfiguration.SessionSampleRate = 100.0f;

            // Enable automatic UIKit tracking for native view controllers and actions
            rumConfiguration.UiKitViewsPredicate = new DDDefaultUIKitRUMViewsPredicate();
            rumConfiguration.UiKitActionsPredicate = new DDDefaultUIKitRUMActionsPredicate();
            System.Diagnostics.Debug.WriteLine("[Datadog] Enabled automatic UIKit view and action tracking");

            DDRUM.EnableWith(rumConfiguration);
            System.Diagnostics.Debug.WriteLine("[Datadog] RUM enabled");

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
                System.Diagnostics.Debug.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            // Enable APM Tracing
            try
            {
                var traceConfig = new DDTraceConfiguration();
                traceConfig.SampleRate = 100.0f;  // Sample 100% of traces

                // Configure URLSession tracking with first-party hosts for automatic HTTP tracing
                var firstPartyHosts = new[] { "fakestoreapi.com" };

                if (firstPartyHosts.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Datadog] Configuring URLSession tracking for {firstPartyHosts.Length} first-party hosts");

                    var hosts = new NSSet<NSString>(
                        firstPartyHosts.Select(h => new NSString(h)).ToArray()
                    );

                    var firstPartyHostsTracing = new DDTraceFirstPartyHostsTracing(hosts);
                    var urlSessionTracking = new DDTraceURLSessionTracking(firstPartyHostsTracing);
                    traceConfig.SetURLSessionTracking(urlSessionTracking);

                    System.Diagnostics.Debug.WriteLine("[Datadog] ✓ URLSession tracking configured");
                    foreach (var host in firstPartyHosts)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Datadog]   - {host}");
                    }
                }

                DDTrace.EnableWith(traceConfig);
                Console.WriteLine("[Datadog] ✓ APM Tracing enabled with URLSession tracking");
                Console.WriteLine("[Datadog] ℹ HTTP requests to first-party hosts will be traced");

                // Try to verify tracer is accessible
                try
                {
                    var tracer = DDTracer.Shared;
                    Console.WriteLine($"[Datadog] ✓ DDTracer.Shared accessible: {tracer?.GetType().Name ?? "null"}");
                }
                catch (Exception tracerEx)
                {
                    Console.WriteLine($"[Datadog] ✗ DDTracer.Shared NOT accessible: {tracerEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ APM Tracing failed: {ex.Message}");
                Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
            }

            // Enable WebView Tracking
            // Note: WebViewTracking requires the WebView instance and allowed hosts list.
            // This should be called when you have a WebView instance, not during app initialization.
            try
            {
                // DDWebViewTracking will be enabled per-WebView in the WebView handler
                System.Diagnostics.Debug.WriteLine("[Datadog] WebView tracking configuration skipped (call Enable when WebView is available)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] WebView tracking failed: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("[Datadog] Successfully initialized for iOS");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Datadog] Initialization failed: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    private static DDSite GetDatadogSite(string site)
    {
        return site.ToUpperInvariant() switch
        {
            "US1" => DDSite.Us1,
            "US3" => DDSite.Us3,
            "US5" => DDSite.Us5,
            "EU1" => DDSite.Eu1,
            "AP1" => DDSite.Ap1,
            "GOV" => DDSite.Us1_fed,
            _ => DDSite.Us1
        };
    }

    /// <summary>
    /// Creates the MAUI application.
    /// </summary>
    /// <returns>The MAUI application instance.</returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
