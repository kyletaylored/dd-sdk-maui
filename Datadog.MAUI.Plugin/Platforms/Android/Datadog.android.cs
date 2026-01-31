using Android.App;
using Datadog.Maui.Configuration;
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
using GlobalRumMonitor = Datadog.Android.RUM.GlobalRumMonitor;

namespace Datadog.Maui;

public static partial class Datadog
{
    static partial void PlatformInitialize(global::Datadog.Maui.Configuration.DatadogConfiguration configuration)
    {
        var context = global::Android.App.Application.Context;

        // Build native configuration
        var configBuilder = new DatadogConfiguration.Builder(
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
        configBuilder.SetBatchSize(BatchSize.Medium!);
        configBuilder.SetUploadFrequency(UploadFrequency.Average!);

        var nativeConfig = configBuilder.Build();

        // Initialize Datadog SDK
        DatadogCore.Initialize(context, nativeConfig, MapTrackingConsent(configuration.TrackingConsent));

        // Set verbosity
        if (configuration.VerboseLogging)
        {
            DatadogCore.Verbosity = (int)global::Android.Util.LogPriority.Verbose;
        }

        // Apply global tags
        foreach (var tag in configuration.GlobalTags)
        {
            GlobalRumMonitor.Get().AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
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

    private static void InitializeRum(global::Datadog.Maui.Configuration.RumConfiguration rumConfig)
    {
        var rumConfigBuilder = new RumConfiguration.Builder(rumConfig.ApplicationId);

        rumConfigBuilder.SetSessionSampleRate((float)rumConfig.SessionSampleRate);
        rumConfigBuilder.SetTelemetrySampleRate((float)rumConfig.TelemetrySampleRate);

        if (rumConfig.TrackUserInteractions)
        {
            rumConfigBuilder.TrackUserInteractions();
        }

        rumConfigBuilder.TrackLongTasks();

        if (rumConfig.VitalsUpdateFrequency != global::Datadog.Maui.Configuration.VitalsUpdateFrequency.Never)
        {
            rumConfigBuilder.SetVitalsUpdateFrequency(MapVitalsFrequency(rumConfig.VitalsUpdateFrequency));
        }

        global::Datadog.Android.RUM.Rum.Enable(rumConfigBuilder.Build());
    }

    private static void InitializeLogs(global::Datadog.Maui.Configuration.LogsConfiguration logsConfig)
    {
        var logsConfigBuilder = new LogsConfiguration.Builder();

        // Note: NetworkInfoEnabled and BundleWithRum settings are not available in Android SDK v3.x
        // These features are enabled by default in the core configuration

        global::Datadog.Android.Logs.Logs.Enable(logsConfigBuilder.Build());
    }

    private static void InitializeTracing(TracingConfiguration tracingConfig)
    {
        var traceConfigBuilder = new TraceConfiguration.Builder();

        // Note: Sample rate is configured at tracer level, not at Trace configuration level
        // The tracer is registered separately with GlobalDatadogTracer

        global::Datadog.Android.Trace.Trace.Enable(traceConfigBuilder.Build());
    }

    static partial void PlatformSetUser(UserInfo userInfo)
    {
        DatadogCore.SetUserInfo(
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
            GlobalRumMonitor.Get().AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
        }
    }

    static partial void PlatformSetTrackingConsent(TrackingConsent consent)
    {
        var nativeConsent = MapTrackingConsent(consent);
        DatadogCore.SetTrackingConsent(nativeConsent);
    }

    static partial void PlatformClearUser()
    {
        DatadogCore.SetUserInfo(null, null, null, null);
    }

    // Helper methods to map enums
    private static global::Datadog.Android.DatadogSite MapSite(DatadogSite site)
    {
        return site switch
        {
            Maui.DatadogSite.US1 => global::Datadog.Android.DatadogSite.Us1!,
            Maui.DatadogSite.US3 => global::Datadog.Android.DatadogSite.Us3!,
            Maui.DatadogSite.US5 => global::Datadog.Android.DatadogSite.Us5!,
            Maui.DatadogSite.EU1 => global::Datadog.Android.DatadogSite.Eu1!,
            Maui.DatadogSite.US1_FED => global::Datadog.Android.DatadogSite.Us1Fed!,
            Maui.DatadogSite.AP1 => global::Datadog.Android.DatadogSite.Ap1!,
            _ => global::Datadog.Android.DatadogSite.Us1!
        };
    }

    private static global::Datadog.Android.Privacy.TrackingConsent MapTrackingConsent(Maui.TrackingConsent consent)
    {
        return consent switch
        {
            Maui.TrackingConsent.Granted => global::Datadog.Android.Privacy.TrackingConsent.Granted!,
            Maui.TrackingConsent.NotGranted => global::Datadog.Android.Privacy.TrackingConsent.NotGranted!,
            Maui.TrackingConsent.Pending => global::Datadog.Android.Privacy.TrackingConsent.Pending!,
            _ => global::Datadog.Android.Privacy.TrackingConsent.Pending!
        };
    }

    private static global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency MapVitalsFrequency(global::Datadog.Maui.Configuration.VitalsUpdateFrequency frequency)
    {
        return frequency switch
        {
            global::Datadog.Maui.Configuration.VitalsUpdateFrequency.Frequent => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Frequent!,
            global::Datadog.Maui.Configuration.VitalsUpdateFrequency.Average => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Average!,
            global::Datadog.Maui.Configuration.VitalsUpdateFrequency.Rare => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Rare!,
            _ => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Average!
        };
    }
}
