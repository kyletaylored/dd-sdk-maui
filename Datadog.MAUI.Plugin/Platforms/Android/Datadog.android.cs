using Android.App;
using Datadog.Maui.Configuration;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Privacy;

namespace Datadog.Maui;

public static partial class Datadog
{
    static partial void PlatformInitialize(DatadogConfiguration configuration)
    {
        var context = Android.App.Application.Context;

        // Build native configuration
        var configBuilder = new Com.Datadog.Android.Core.Configuration.Configuration.Builder(
            configuration.ClientToken,
            configuration.Environment,
            string.Empty, // variant
            configuration.ServiceName
        );

        // Set site
        configBuilder.UseSite(MapSite(configuration.Site));

        // Set first-party hosts
        if (configuration.FirstPartyHosts.Length > 0)
        {
            configBuilder.SetFirstPartyHosts(configuration.FirstPartyHosts.ToList());
        }

        // Set batch upload configuration
        configBuilder.SetBatchSize(Com.Datadog.Android.Core.Configuration.BatchSize.Medium);
        configBuilder.SetUploadFrequency(Com.Datadog.Android.Core.Configuration.UploadFrequency.Average);

        var nativeConfig = configBuilder.Build();

        // Initialize Datadog SDK
        Com.Datadog.Android.Datadog.Initialize(context, nativeConfig, MapTrackingConsent(configuration.TrackingConsent));

        // Set verbosity
        if (configuration.VerboseLogging)
        {
            Com.Datadog.Android.Datadog.Verbosity = (int)Android.Util.LogPriority.Verbose;
        }

        // Apply global tags
        foreach (var tag in configuration.GlobalTags)
        {
            Com.Datadog.Android.Rum.GlobalRumMonitor.Get().AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
        }

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
    }

    private static void InitializeRum(RumConfiguration rumConfig)
    {
        var rumConfigBuilder = new Com.Datadog.Android.Rum.RumConfiguration.Builder(rumConfig.ApplicationId);

        rumConfigBuilder.SetSessionSampleRate((float)rumConfig.SessionSampleRate);
        rumConfigBuilder.SetTelemetrySampleRate((float)rumConfig.TelemetrySampleRate);

        if (rumConfig.TrackUserInteractions)
        {
            rumConfigBuilder.TrackUserInteractions();
        }

        rumConfigBuilder.TrackLongTasks();

        if (rumConfig.VitalsUpdateFrequency != VitalsUpdateFrequency.Never)
        {
            rumConfigBuilder.SetVitalsUpdateFrequency(MapVitalsFrequency(rumConfig.VitalsUpdateFrequency));
        }

        Com.Datadog.Android.Rum.Rum.Enable(rumConfigBuilder.Build());
    }

    private static void InitializeLogs(LogsConfiguration logsConfig)
    {
        var logsConfigBuilder = new Com.Datadog.Android.Log.LogsConfiguration.Builder();

        // Note: NetworkInfoEnabled and BundleWithRum settings are not available in Android SDK v3.x
        // These features are enabled by default in the core configuration

        Com.Datadog.Android.Log.Logs.Enable(logsConfigBuilder.Build());
    }

    private static void InitializeTracing(TracingConfiguration tracingConfig)
    {
        var traceConfigBuilder = new Com.Datadog.Android.Trace.TraceConfiguration.Builder();

        // Note: Sample rate is configured at tracer level, not at Trace configuration level
        // The tracer is registered separately with GlobalDatadogTracer

        Com.Datadog.Android.Trace.Trace.Enable(traceConfigBuilder.Build());
    }

    static partial void PlatformSetUser(UserInfo userInfo)
    {
        Com.Datadog.Android.Datadog.SetUserInfo(
            userInfo.Id,
            userInfo.Name,
            userInfo.Email,
            userInfo.ExtraInfo?.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value)
        );
    }

    static partial void PlatformSetTags(Dictionary<string, string> tags)
    {
        foreach (var tag in tags)
        {
            Com.Datadog.Android.Rum.GlobalRumMonitor.Get().AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
        }
    }

    static partial void PlatformSetTrackingConsent(TrackingConsent consent)
    {
        var nativeConsent = MapTrackingConsent(consent);
        Com.Datadog.Android.Datadog.SetTrackingConsent(nativeConsent);
    }

    static partial void PlatformClearUser()
    {
        Com.Datadog.Android.Datadog.SetUserInfo(null, null, null, null);
    }

    // Helper methods to map enums
    private static Com.Datadog.Android.DatadogSite MapSite(DatadogSite site)
    {
        return site switch
        {
            Maui.DatadogSite.US1 => Com.Datadog.Android.DatadogSite.Us1,
            Maui.DatadogSite.US3 => Com.Datadog.Android.DatadogSite.Us3,
            Maui.DatadogSite.US5 => Com.Datadog.Android.DatadogSite.Us5,
            Maui.DatadogSite.EU1 => Com.Datadog.Android.DatadogSite.Eu1,
            Maui.DatadogSite.US1_FED => Com.Datadog.Android.DatadogSite.Us1Fed,
            Maui.DatadogSite.AP1 => Com.Datadog.Android.DatadogSite.Ap1,
            _ => Com.Datadog.Android.DatadogSite.Us1
        };
    }

    private static Com.Datadog.Android.Privacy.TrackingConsent MapTrackingConsent(Maui.TrackingConsent consent)
    {
        return consent switch
        {
            Maui.TrackingConsent.Granted => Com.Datadog.Android.Privacy.TrackingConsent.Granted,
            Maui.TrackingConsent.NotGranted => Com.Datadog.Android.Privacy.TrackingConsent.NotGranted,
            Maui.TrackingConsent.Pending => Com.Datadog.Android.Privacy.TrackingConsent.Pending,
            _ => Com.Datadog.Android.Privacy.TrackingConsent.Pending
        };
    }

    private static Com.Datadog.Android.Rum.Configuration.VitalsUpdateFrequency MapVitalsFrequency(VitalsUpdateFrequency frequency)
    {
        return frequency switch
        {
            VitalsUpdateFrequency.Frequent => Com.Datadog.Android.Rum.Configuration.VitalsUpdateFrequency.Frequent,
            VitalsUpdateFrequency.Average => Com.Datadog.Android.Rum.Configuration.VitalsUpdateFrequency.Average,
            VitalsUpdateFrequency.Rare => Com.Datadog.Android.Rum.Configuration.VitalsUpdateFrequency.Rare,
            _ => Com.Datadog.Android.Rum.Configuration.VitalsUpdateFrequency.Average
        };
    }
}
