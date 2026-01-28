using System.Threading;
using DatadogMauiSample.Config;
using Microsoft.Maui.Devices;

namespace DatadogMauiSample.Services;

public interface IDatadogInitializer
{
    void Initialize();
}

public sealed class DatadogInitializer : IDatadogInitializer
{
    private static int _initialized;

    public void Initialize()
    {
        if (Interlocked.Exchange(ref _initialized, 1) == 1)
        {
            return;
        }

        try
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
#if ANDROID
                InitializeAndroid();
#else
                Console.WriteLine("[Datadog] Android initialization skipped (platform not available).");
#endif
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
#if IOS
                InitializeIos();
#else
                Console.WriteLine("[Datadog] iOS initialization skipped (platform not available).");
#endif
            }
            else
            {
                Console.WriteLine($"[Datadog] Initialization skipped for platform: {DeviceInfo.Platform}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Initialization failed: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

#if ANDROID
    private void InitializeAndroid()
    {
        try
        {
            Console.WriteLine("[Datadog] Initializing for Android");
            Console.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            Console.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

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

            var config = new Com.Datadog.Android.Core.Configuration.Configuration.Builder(
                DatadogConfig.AndroidClientToken,
                DatadogConfig.Environment,
                string.Empty,
                DatadogConfig.ServiceName
            )
            .SetFirstPartyHosts(DatadogConfig.FirstPartyHosts)
            .SetBatchSize(Com.Datadog.Android.Core.Configuration.BatchSize.Small)
            .SetUploadFrequency(Com.Datadog.Android.Core.Configuration.UploadFrequency.Frequent)
            .Build();

            Com.Datadog.Android.Datadog.Initialize(Android.App.Application.Context, config, Com.Datadog.Android.Privacy.TrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            if (DatadogConfig.VerboseLogging)
            {
                Com.Datadog.Android.Datadog.Verbosity = (int)Android.Util.LogPriority.Verbose;
            }

            var logsConfig = new Com.Datadog.Android.Log.LogsConfiguration.Builder().Build();
            Com.Datadog.Android.Log.Logs.Enable(logsConfig);

            Console.WriteLine("[Datadog] Logs enabled");

            try
            {
                Com.Datadog.Android.Ndk.NdkCrashReports.Enable();
                Console.WriteLine("[Datadog] NDK crash reports enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] NDK crash reports failed: {ex.Message}");
            }

            var rumConfiguration = new Com.Datadog.Android.Rum.RumConfiguration.Builder(DatadogConfig.AndroidRumApplicationId)
                .TrackUserInteractions()
                .TrackLongTasks()
                .TrackFrustrations(true)
                .TrackBackgroundEvents(true)
                .TrackNonFatalAnrs(true)
                .SetTelemetrySampleRate(100f)
                .Build();

            Com.Datadog.Android.Rum.Rum.Enable(rumConfiguration);

            Console.WriteLine("[Datadog] RUM enabled");

            _ = Com.Datadog.Android.Rum.GlobalRumMonitor.Instance;
            _ = Com.Datadog.Android.Rum.GlobalRumMonitor.Get();

            try
            {
                var sessionReplayConfig = new Com.Datadog.Android.Sessionreplay.SessionReplayConfiguration.Builder(
                    DatadogConfig.SessionReplaySampleRate
                )
                .SetTextAndInputPrivacy(Com.Datadog.Android.Sessionreplay.TextAndInputPrivacy.MaskSensitiveInputs)
                .SetImagePrivacy(Com.Datadog.Android.Sessionreplay.ImagePrivacy.MaskNone)
                .SetTouchPrivacy(Com.Datadog.Android.Sessionreplay.TouchPrivacy.Show)
                .Build();

                Com.Datadog.Android.Sessionreplay.SessionReplay.Enable(sessionReplayConfig, Com.Datadog.Android.Datadog.Instance);
                Console.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            try
            {
                var traceConfig = new Com.Datadog.Android.Trace.TraceConfiguration.Builder().Build();
                Com.Datadog.Android.Trace.Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);
                Console.WriteLine("[Datadog] APM Tracing enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            try
            {
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
#endif

#if IOS
    private void InitializeIos()
    {
        try
        {
            Console.WriteLine("[Datadog] Initializing for iOS");
            Console.WriteLine($"[Datadog] - Environment: {DatadogConfig.Environment}");
            Console.WriteLine($"[Datadog] - Service: {DatadogConfig.ServiceName}");

            var maskedToken = string.IsNullOrEmpty(DatadogConfig.IosClientToken)
                ? "NOT_CONFIGURED"
                : DatadogConfig.IosClientToken.Length > 14
                    ? $"{DatadogConfig.IosClientToken.Substring(0, 10)}...{DatadogConfig.IosClientToken.Substring(DatadogConfig.IosClientToken.Length - 4)}"
                    : "***CONFIGURED***";

            Console.WriteLine($"[Datadog] - Client Token: {maskedToken}");
            Console.WriteLine($"[Datadog] - RUM Application ID: {DatadogConfig.IosRumApplicationId}");

            var configuration = new Datadog.iOS.Core.DDConfiguration(
                clientToken: DatadogConfig.IosClientToken,
                env: DatadogConfig.Environment
            );
            configuration.Service = DatadogConfig.ServiceName;
            configuration.Site = GetDatadogSite(DatadogConfig.Site);
            configuration.BatchSize = Datadog.iOS.Core.DDBatchSize.Small;
            configuration.UploadFrequency = Datadog.iOS.Core.DDUploadFrequency.Frequent;

            Datadog.iOS.Core.DDDatadog.InitializeWithConfiguration(configuration, Datadog.iOS.Core.DDTrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            if (DatadogConfig.VerboseLogging)
            {
                Datadog.iOS.Core.DDDatadog.VerbosityLevel = Datadog.iOS.Core.DDCoreLoggerLevel.Debug;
            }

            Datadog.iOS.Logs.DDLogs.EnableWith(new Datadog.iOS.Logs.DDLogsConfiguration());
            Console.WriteLine("[Datadog] Logs enabled");

            try
            {
                Datadog.iOS.CrashReporting.DDCrashReporter.Enable();
                Console.WriteLine("[Datadog] Crash Reporting enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Crash Reporting failed: {ex.Message}");
            }

            var rumConfiguration = new Datadog.iOS.RUM.DDRUMConfiguration(applicationID: DatadogConfig.IosRumApplicationId);
            rumConfiguration.TrackFrustrations = true;
            rumConfiguration.TrackBackgroundEvents = true;
            rumConfiguration.VitalsUpdateFrequency = Datadog.iOS.RUM.DDRUMVitalsFrequency.Frequent;
            rumConfiguration.SessionSampleRate = 100.0f;

            Datadog.iOS.RUM.DDRUM.EnableWith(rumConfiguration);
            Console.WriteLine("[Datadog] RUM enabled");

            try
            {
                var sessionReplayConfig = new Datadog.iOS.SessionReplay.DDSessionReplayConfiguration(
                    replaySampleRate: DatadogConfig.SessionReplaySampleRate,
                    textAndInputPrivacyLevel: Datadog.iOS.SessionReplay.DDTextAndInputPrivacyLevel.SensitiveInputs,
                    imagePrivacyLevel: Datadog.iOS.SessionReplay.DDImagePrivacyLevel.None,
                    touchPrivacyLevel: Datadog.iOS.SessionReplay.DDTouchPrivacyLevel.Show
                );

                Datadog.iOS.SessionReplay.DDSessionReplay.EnableWith(sessionReplayConfig);
                Console.WriteLine("[Datadog] Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
            }

            try
            {
                var traceConfig = new Datadog.iOS.Trace.DDTraceConfiguration();
                Datadog.iOS.Trace.DDTrace.EnableWith(traceConfig);
                Console.WriteLine("[Datadog] APM Tracing enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] APM Tracing failed: {ex.Message}");
            }

            try
            {
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

    private static Datadog.iOS.Core.DDSite GetDatadogSite(string site)
    {
        return site.ToUpperInvariant() switch
        {
            "US1" => Datadog.iOS.Core.DDSite.Us1,
            "US3" => Datadog.iOS.Core.DDSite.Us3,
            "US5" => Datadog.iOS.Core.DDSite.Us5,
            "EU1" => Datadog.iOS.Core.DDSite.Eu1,
            "AP1" => Datadog.iOS.Core.DDSite.Ap1,
            "GOV" => Datadog.iOS.Core.DDSite.Us1_fed,
            _ => Datadog.iOS.Core.DDSite.Us1
        };
    }
#endif
}
