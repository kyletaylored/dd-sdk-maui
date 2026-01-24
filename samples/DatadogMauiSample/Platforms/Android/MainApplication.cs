using Android.App;
using Android.Runtime;
using Android.Util;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Log;
using Com.Datadog.Android.Ndk;
using Com.Datadog.Android.Privacy;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Sessionreplay;
using Com.Datadog.Android.Trace;
using Com.Datadog.Android.Webview;
using DatadogMauiSample.Config;

namespace DatadogMauiSample;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Load Datadog configuration from environment
        DatadogConfig.LoadFromEnvironment();

        InitializeDatadog();
    }

    private void InitializeDatadog()
    {
        try
        {
            Console.WriteLine($"[Datadog] Initializing for Android");
            Console.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            Console.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

            // Safely mask the client token
            string maskedToken;
            if (string.IsNullOrEmpty(DatadogConfig.AndroidClientToken))
            {
                maskedToken = "NOT_CONFIGURED";
            }
            else if (DatadogConfig.AndroidClientToken.Length > 14)
            {
                maskedToken = $"{DatadogConfig.AndroidClientToken.Substring(0, 10)}...{DatadogConfig.AndroidClientToken.Substring(DatadogConfig.AndroidClientToken.Length - 4)}";
            }
            else
            {
                maskedToken = "***CONFIGURED***";
            }

            Console.WriteLine($"[Datadog] - Client Token: {maskedToken}");
            Console.WriteLine($"[Datadog] - RUM Application ID: {DatadogConfig.AndroidRumApplicationId}");

            // Create Datadog configuration
            var config = new Configuration.Builder(
                DatadogConfig.AndroidClientToken,
                DatadogConfig.Environment,
                string.Empty,  // variant - use empty string if no build variants
                DatadogConfig.ServiceName
            )
            .SetFirstPartyHosts(DatadogConfig.FirstPartyHosts)
            .SetBatchSize(BatchSize.Small)
            .SetUploadFrequency(UploadFrequency.Frequent)
            .Build();

            // Note: In Android SDK v3, site configuration is not exposed via Configuration.Builder
            // The site is typically set via environment variables or other configuration methods

            // Initialize Datadog SDK
            Com.Datadog.Android.Datadog.Initialize(this, config, TrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            // Set verbosity level for debugging
            if (DatadogConfig.VerboseLogging)
            {
                Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Verbose;
            }

            // Enable Logs
            var logsConfig = new LogsConfiguration.Builder().Build();
            Logs.Enable(logsConfig);

            Console.WriteLine("[Datadog] Logs enabled");

            // Enable NDK crash reports
            try
            {
                NdkCrashReports.Enable();
                Console.WriteLine("[Datadog] NDK crash reports enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] NDK crash reports failed: {ex.Message}");
            }

            // Enable RUM (Real User Monitoring)
            var rumConfiguration = new RumConfiguration.Builder(DatadogConfig.AndroidRumApplicationId)
                .TrackUserInteractions()
                .TrackLongTasks()
                .TrackFrustrations(true)
                .TrackBackgroundEvents(true)
                .TrackNonFatalAnrs(true)
                .SetTelemetrySampleRate(100f)
                .Build();

            Rum.Enable(rumConfiguration);

            Console.WriteLine("[Datadog] RUM enabled");

            // Initialize Global RUM Monitor
            _ = GlobalRumMonitor.Instance;
            _ = GlobalRumMonitor.Get();

            // Enable Session Replay
            try
            {
                var sessionReplayConfig = new SessionReplayConfiguration.Builder(
                    DatadogConfig.SessionReplaySampleRate
                )
                .SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)
                .SetImagePrivacy(ImagePrivacy.MaskNone)
                .SetTouchPrivacy(TouchPrivacy.Show)
                .Build();

                SessionReplay.Enable(sessionReplayConfig, Com.Datadog.Android.Datadog.Instance);
                Console.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            // Enable APM Tracing
            try
            {
                var traceConfig = new TraceConfiguration.Builder().Build();
                Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);
                Console.WriteLine("[Datadog] APM Tracing enabled");

                // Note: In SDK v3.x, the global tracer is automatically registered when Trace.Enable() is called.
                // The Datadog.Maui.Tracing.Tracer class will access it via GlobalDatadogTracer.get() using reflection.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            // Enable WebView Tracking
            // Note: WebViewTracking.Enable() requires a WebView instance and allowed hosts list.
            // This should be called when you have a WebView instance, not during app initialization.
            // Example: WebViewTracking.Enable(webView, new List<string> { "example.com" });
            try
            {
                // WebViewTracking.Enable(); // Commented out - requires WebView instance
                Console.WriteLine("[Datadog] WebView tracking configuration skipped (call Enable when WebView is available)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] WebView tracking failed: {ex.Message}");
            }

            Console.WriteLine("[Datadog] Successfully initialized for Android");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Failed to initialize: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
