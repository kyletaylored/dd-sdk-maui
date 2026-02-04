using Datadog.Maui.Configuration;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.Logs;
using Datadog.iOS.Trace;
using Foundation;

namespace Datadog.Maui;

public static partial class Datadog
{
    static partial void PlatformInitialize(DatadogConfiguration configuration)
    {
        // Create native configuration
        var nativeConfig = new DDConfiguration(
            clientToken: configuration.ClientToken,
            env: configuration.Environment
        );

        nativeConfig.Site = MapSite(configuration.Site);
        nativeConfig.Service = configuration.ServiceName;

        // Initialize Datadog SDK
        DDDatadog.InitializeWithConfiguration(
            nativeConfig,
            MapTrackingConsent(configuration.TrackingConsent)
        );

        // Set verbosity
        if (configuration.VerboseLogging)
        {
            DDDatadog.VerbosityLevel = DDCoreLoggerLevel.Debug;
        }

        // Note: iOS SDK doesn't have a direct SetTag API like Android
        // Global tags need to be set via RUM/Logs configuration or per-event

        // Enable RUM if configured
        if (configuration.Rum != null)
        {
            InitializeRum(configuration.Rum);
        }

        // Enable Logs if configured
        if (configuration.Logs != null)
        {
            InitializeLogs(configuration.Logs);
        }

        // Enable Tracing if configured
        if (configuration.Tracing != null)
        {
            InitializeTracing(configuration.Tracing);
        }

        // Enable Session Replay if configured
        if (configuration.SessionReplay != null)
        {
            SessionReplayInitializer.Initialize(configuration.SessionReplay);
        }
    }

    private static void InitializeRum(RumConfiguration rumConfig)
    {
        var rumConfiguration = new DDRUMConfiguration(applicationID: rumConfig.ApplicationId);

        rumConfiguration.SessionSampleRate = rumConfig.SessionSampleRate;
        rumConfiguration.TrackFrustrations = rumConfig.TrackUserInteractions;
        rumConfiguration.TrackBackgroundEvents = true;
        rumConfiguration.VitalsUpdateFrequency = MapVitalsFrequency(rumConfig.VitalsUpdateFrequency);

        // Enable automatic UIKit view tracking with MAUI-aware filtering
        if (rumConfig.TrackViewsAutomatically)
        {
            rumConfiguration.UiKitViewsPredicate = new Platforms.iOS.MauiRumViewsPredicate();
        }

        // Enable automatic UIKit action tracking (taps, swipes, etc.)
        if (rumConfig.TrackUserInteractions)
        {
            rumConfiguration.UiKitActionsPredicate = new DDDefaultUIKitRUMActionsPredicate();
        }

        // Enable automatic URLSession tracking for HTTP resources (out-of-the-box from native SDK)
        var urlSessionTracking = new DDRUMURLSessionTracking();
        rumConfiguration.SetURLSessionTracking(urlSessionTracking);

        DDRUM.EnableWith(rumConfiguration);

        // Set variant and buildId as global RUM attributes
        var rumMonitor = DDRUMMonitor.Shared;
        if (!string.IsNullOrEmpty(rumConfig.Variant))
        {
            rumMonitor.AddAttribute("variant", new NSString(rumConfig.Variant));
        }
        if (!string.IsNullOrEmpty(rumConfig.BuildId))
        {
            rumMonitor.AddAttribute("build_id", new NSString(rumConfig.BuildId));
        }
    }

    private static void InitializeLogs(LogsConfiguration logsConfig)
    {
        var logsConfiguration = new DDLogsConfiguration(customEndpoint: null);
        DDLogs.EnableWith(logsConfiguration);
    }

    private static void InitializeTracing(TracingConfiguration tracingConfig)
    {
        // Signal to Tracer that tracing will be enabled
        Tracing.Tracer.IsTracingEnabled = true;

        var traceConfiguration = new DDTraceConfiguration();
        traceConfiguration.SampleRate = tracingConfig.SampleRate;

        DDTrace.EnableWith(traceConfiguration);

        // Note: URLSession tracking for resources is now handled automatically by RUM's URLSessionTracking
        // For distributed tracing headers on first-party hosts, the RUM URLSessionTracking needs to be configured
        // This is already done in InitializeRum() above
        System.Diagnostics.Debug.WriteLine("[Datadog] Tracing enabled (URLSession tracking handled by RUM)");
    }

    static partial void PlatformSetUser(UserInfo userInfo)
    {
        var extraInfo = userInfo.ExtraInfo != null && userInfo.ExtraInfo.Count > 0
            ? NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
                userInfo.ExtraInfo.Values.Select(v => NSObject.FromObject(v)).ToArray(),
                userInfo.ExtraInfo.Keys.Select(k => new NSString(k)).ToArray()
            )
            : new NSDictionary<NSString, NSObject>();

        DDDatadog.SetUserInfoWithUserId(
            userInfo.Id ?? string.Empty,
            userInfo.Name,
            userInfo.Email,
            extraInfo
        );
    }

    static partial void PlatformSetTags(Dictionary<string, string> tags)
    {
        // iOS SDK doesn't have a global SetTag API at the DDDatadog level
        // Tags would need to be set per-logger or per-RUM monitor
        // This is a known limitation - we'll document it
    }

    static partial void PlatformSetTrackingConsent(TrackingConsent consent)
    {
        DDDatadog.SetTrackingConsentWithConsent(MapTrackingConsent(consent));
    }

    static partial void PlatformClearUser()
    {
        DDDatadog.ClearUserInfo();
    }

    // Helper methods to map enums
    private static DDSite MapSite(DatadogSite site)
    {
        return site switch
        {
            Maui.DatadogSite.US1 => DDSite.Us1,
            Maui.DatadogSite.US3 => DDSite.Us3,
            Maui.DatadogSite.US5 => DDSite.Us5,
            Maui.DatadogSite.EU1 => DDSite.Eu1,
            Maui.DatadogSite.US1_FED => DDSite.Us1_fed,
            Maui.DatadogSite.AP1 => DDSite.Ap1,
            _ => DDSite.Us1
        };
    }

    private static DDTrackingConsent MapTrackingConsent(TrackingConsent consent)
    {
        return consent switch
        {
            Maui.TrackingConsent.Granted => DDTrackingConsent.Granted,
            Maui.TrackingConsent.NotGranted => DDTrackingConsent.NotGranted,
            Maui.TrackingConsent.Pending => DDTrackingConsent.Pending,
            _ => DDTrackingConsent.Pending
        };
    }

    private static DDRUMVitalsFrequency MapVitalsFrequency(VitalsUpdateFrequency frequency)
    {
        return frequency switch
        {
            VitalsUpdateFrequency.Frequent => DDRUMVitalsFrequency.Frequent,
            VitalsUpdateFrequency.Average => DDRUMVitalsFrequency.Average,
            VitalsUpdateFrequency.Rare => DDRUMVitalsFrequency.Rare,
            _ => DDRUMVitalsFrequency.Average
        };
    }
}
