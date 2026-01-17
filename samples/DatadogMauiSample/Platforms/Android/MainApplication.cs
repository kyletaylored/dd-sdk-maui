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
            Console.WriteLine($"[Datadog] - Client Token: {DatadogConfig.AndroidClientToken.Substring(0, 10)}...{DatadogConfig.AndroidClientToken.Substring(DatadogConfig.AndroidClientToken.Length - 4)}");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            // Enable WebView Tracking
            try
            {
                WebViewTracking.Enable();
                Console.WriteLine("[Datadog] WebView tracking enabled");
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
