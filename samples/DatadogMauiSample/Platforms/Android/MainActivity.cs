using Android.App;
using Android.Content.PM;
using Android.OS;
using DatadogMauiSample.Config;
using DatadogCore = Datadog.Android.Datadog;
using DatadogConfiguration = Datadog.Android.Core.Configuration.Configuration;
using DatadogSite = Datadog.Android.DatadogSite;
using TrackingConsent = Datadog.Android.Privacy.TrackingConsent;
using BatchSize = Datadog.Android.Core.Configuration.BatchSize;
using UploadFrequency = Datadog.Android.Core.Configuration.UploadFrequency;
using RumConfiguration = Datadog.Android.RUM.RumConfiguration;
using Rum = Datadog.Android.RUM.Rum;
using LogsConfiguration = Datadog.Android.Logs.LogsConfiguration;
using Logs = Datadog.Android.Logs.Logs;
using TraceConfiguration = Datadog.Android.Trace.TraceConfiguration;
using Trace = Datadog.Android.Trace.Trace;
using VitalsUpdateFrequency = Datadog.Android.RUM.Configuration.VitalsUpdateFrequency;
using SessionReplay = Datadog.Android.SessionReplay.SessionReplay;
using SessionReplayConfiguration = Datadog.Android.SessionReplay.SessionReplayConfiguration;
using SessionReplayPrivacy = Datadog.Android.SessionReplay.SessionReplayPrivacy;

namespace DatadogMauiSample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Initialize Datadog BEFORE calling base.OnCreate()
        InitializeDatadog();

        base.OnCreate(savedInstanceState);
    }

    private void InitializeDatadog()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[Datadog] Initializing for Android");

            // Load configuration from environment
            DatadogConfig.LoadFromEnvironment();

            System.Diagnostics.Debug.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

            // Safely mask the client token
            var maskedToken = string.IsNullOrEmpty(DatadogConfig.AndroidClientToken)
                ? "NOT_CONFIGURED"
                : DatadogConfig.AndroidClientToken.Length > 14
                    ? $"{DatadogConfig.AndroidClientToken.Substring(0, 10)}...{DatadogConfig.AndroidClientToken.Substring(DatadogConfig.AndroidClientToken.Length - 4)}"
                    : "***CONFIGURED***";

            System.Diagnostics.Debug.WriteLine($"[Datadog] - Client Token: {maskedToken}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] - RUM Application ID: {DatadogConfig.AndroidRumApplicationId}");

            // Build Datadog configuration
            var configBuilder = new DatadogConfiguration.Builder(
                DatadogConfig.AndroidClientToken,
                DatadogConfig.Environment,
                string.Empty, // variant
                DatadogConfig.ServiceName
            );

            configBuilder.UseSite(GetDatadogSite(DatadogConfig.Site));
            configBuilder.SetBatchSize(BatchSize.Small!);
            configBuilder.SetUploadFrequency(UploadFrequency.Frequent!);

            var configuration = configBuilder.Build();

            // Initialize Datadog SDK
            DatadogCore.Initialize(this.ApplicationContext!, configuration, TrackingConsent.Granted!);
            System.Diagnostics.Debug.WriteLine("[Datadog] Core SDK initialized");

            // Set verbosity level for debugging
            if (DatadogConfig.VerboseLogging)
            {
                DatadogCore.Verbosity = (int)Android.Util.LogPriority.Verbose;
            }

            // Enable Logs
            Logs.Enable(new LogsConfiguration.Builder().Build());
            System.Diagnostics.Debug.WriteLine("[Datadog] Logs enabled");

            // Enable RUM (Real User Monitoring)
            var rumConfigBuilder = new RumConfiguration.Builder(DatadogConfig.AndroidRumApplicationId);
            rumConfigBuilder.SetSessionSampleRate(DatadogConfig.SessionSampleRate);
            rumConfigBuilder.TrackUserInteractions();
            rumConfigBuilder.TrackLongTasks();
            rumConfigBuilder.SetVitalsUpdateFrequency(VitalsUpdateFrequency.Frequent!);

            Rum.Enable(rumConfigBuilder.Build());
            System.Diagnostics.Debug.WriteLine("[Datadog] RUM enabled");

            // Enable Session Replay
            try
            {
                var sessionReplayConfig = new SessionReplayConfiguration.Builder(DatadogConfig.SessionReplaySampleRate)
                    .Build();

                SessionReplay.Enable(sessionReplayConfig);
                System.Diagnostics.Debug.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            // Enable APM Tracing
            try
            {
                var traceConfig = new TraceConfiguration.Builder().Build();
                Trace.Enable(traceConfig);
                System.Diagnostics.Debug.WriteLine("[Datadog] APM Tracing enabled");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("[Datadog] Successfully initialized for Android");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Datadog] Initialization failed: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    private static DatadogSite GetDatadogSite(string site)
    {
        return site.ToUpperInvariant() switch
        {
            "US1" => DatadogSite.Us1!,
            "US3" => DatadogSite.Us3!,
            "US5" => DatadogSite.Us5!,
            "EU1" => DatadogSite.Eu1!,
            "AP1" => DatadogSite.Ap1!,
            "GOV" => DatadogSite.Us1Fed!,
            _ => DatadogSite.Us1!
        };
    }
}
