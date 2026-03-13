using global::Android.App;
using Datadog.Maui.Configuration;
using global::Datadog.Android;
using global::Datadog.Android.Core.Configuration;
using global::Datadog.Android.Privacy;

namespace Datadog.Maui;

public static partial class Datadog
{
    static partial void PlatformInitialize(DatadogConfiguration configuration)
    {
        var context = global::Android.App.Application.Context;

        // Build native configuration
        var configBuilder = new global::Datadog.Android.Core.Configuration.Configuration.Builder(
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
        configBuilder.SetBatchSize(global::Datadog.Android.Core.Configuration.BatchSize.Medium);
        configBuilder.SetUploadFrequency(global::Datadog.Android.Core.Configuration.UploadFrequency.Average);

        var nativeConfig = configBuilder.Build();

        // Initialize Datadog SDK
        global::Datadog.Android.Datadog.Initialize(context, nativeConfig, MapTrackingConsent(configuration.TrackingConsent));

        // Set verbosity
        if (configuration.VerboseLogging)
        {
            global::Datadog.Android.Datadog.Verbosity = (int)global::Android.Util.LogPriority.Verbose;
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

        // Apply global tags (must be after RUM is enabled so GlobalRumMonitor is active)
        if (configuration.GlobalTags.Count > 0)
        {
            var rumMonitor = global::Datadog.Android.RUM.GlobalRumMonitor.Get();
            foreach (var tag in configuration.GlobalTags)
            {
                rumMonitor?.AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
            }
        }
    }

    private static void InitializeRum(RumConfiguration rumConfig)
    {
        var rumConfigBuilder = new global::Datadog.Android.RUM.RumConfiguration.Builder(rumConfig.ApplicationId);

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

        global::Datadog.Android.RUM.Rum.Enable(rumConfigBuilder.Build());
    }

    private static void InitializeLogs(LogsConfiguration logsConfig)
    {
        var logsConfigBuilder = new global::Datadog.Android.Logs.LogsConfiguration.Builder();

        // Note: NetworkInfoEnabled and BundleWithRum settings are not available in Android SDK v3.x
        // These features are enabled by default in the core configuration

        global::Datadog.Android.Logs.Logs.Enable(logsConfigBuilder.Build());
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
        var extraInfo = userInfo.ExtraInfo?.ToDictionary(
            kvp => kvp.Key,
            kvp => ToJavaObject(kvp.Value)
        );

        global::Datadog.Android.Datadog.SetUserInfo(
            userInfo.Id,
            userInfo.Name,
            userInfo.Email,
            extraInfo
        );
    }

    static partial void PlatformSetTags(Dictionary<string, string> tags)
    {
        var rumMonitor = global::Datadog.Android.RUM.GlobalRumMonitor.Get();
        foreach (var tag in tags)
        {
            rumMonitor?.AddAttribute(tag.Key, new Java.Lang.String(tag.Value));
        }
    }

    static partial void PlatformSetTrackingConsent(TrackingConsent consent)
    {
        var nativeConsent = MapTrackingConsent(consent);
        global::Datadog.Android.Datadog.SetTrackingConsent(nativeConsent);
    }

    static partial void PlatformClearUser()
    {
        global::Datadog.Android.Datadog.SetUserInfo(null, null, null, null);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        global::Datadog.Android.RUM.GlobalRumMonitor.Get()?.AddAttribute(key, ToJavaObject(value));
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        global::Datadog.Android.RUM.GlobalRumMonitor.Get()?.RemoveAttribute(key);
    }

    /// <summary>
    /// Converts a .NET object to a Java.Lang.Object, preserving numeric and boolean types.
    /// </summary>
    private static Java.Lang.Object ToJavaObject(object? value)
    {
        return value switch
        {
            null => new Java.Lang.String(""),
            Java.Lang.Object jObj => jObj,
            string s => new Java.Lang.String(s),
            bool b => new Java.Lang.Boolean(b),
            int i => new Java.Lang.Integer(i),
            long l => new Java.Lang.Long(l),
            float f => new Java.Lang.Float(f),
            double d => new Java.Lang.Double(d),
            _ => new Java.Lang.String(value.ToString() ?? "")
        };
    }

    // Helper methods to map enums
    private static global::Datadog.Android.DatadogSite MapSite(DatadogSite site)
    {
        return site switch
        {
            Maui.DatadogSite.US1 => global::Datadog.Android.DatadogSite.Us1,
            Maui.DatadogSite.US3 => global::Datadog.Android.DatadogSite.Us3,
            Maui.DatadogSite.US5 => global::Datadog.Android.DatadogSite.Us5,
            Maui.DatadogSite.EU1 => global::Datadog.Android.DatadogSite.Eu1,
            Maui.DatadogSite.US1_FED => global::Datadog.Android.DatadogSite.Us1Fed,
            Maui.DatadogSite.AP1 => global::Datadog.Android.DatadogSite.Ap1,
            _ => global::Datadog.Android.DatadogSite.Us1
        };
    }

    private static global::Datadog.Android.Privacy.TrackingConsent MapTrackingConsent(Maui.TrackingConsent consent)
    {
        return consent switch
        {
            Maui.TrackingConsent.Granted => global::Datadog.Android.Privacy.TrackingConsent.Granted,
            Maui.TrackingConsent.NotGranted => global::Datadog.Android.Privacy.TrackingConsent.NotGranted,
            Maui.TrackingConsent.Pending => global::Datadog.Android.Privacy.TrackingConsent.Pending,
            _ => global::Datadog.Android.Privacy.TrackingConsent.Pending
        };
    }

    private static global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency MapVitalsFrequency(VitalsUpdateFrequency frequency)
    {
        return frequency switch
        {
            VitalsUpdateFrequency.Frequent => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Frequent,
            VitalsUpdateFrequency.Average => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Average,
            VitalsUpdateFrequency.Rare => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Rare,
            _ => global::Datadog.Android.RUM.Configuration.VitalsUpdateFrequency.Average
        };
    }
}
