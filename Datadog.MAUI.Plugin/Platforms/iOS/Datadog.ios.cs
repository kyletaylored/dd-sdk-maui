using Datadog.Maui.Configuration;
using DatadogCore;
using Foundation;

namespace Datadog.Maui;

public static partial class Datadog
{
    static partial void PlatformInitialize(DatadogConfiguration configuration)
    {
        // Build native configuration
        var configBuilder = DatadogCore.Datadog.Configuration.BuilderWithClientToken(
            configuration.ClientToken,
            configuration.Environment
        );

        configBuilder.Set(site: MapSite(configuration.Site));
        configBuilder.Set(service: configuration.ServiceName);

        // Set first-party hosts
        if (configuration.FirstPartyHosts.Length > 0)
        {
            configBuilder.TrackURLSession(firstPartyHostsTracing: new NSSet<NSString>(
                configuration.FirstPartyHosts.Select(h => new NSString(h)).ToArray()
            ));
        }

        var nativeConfig = configBuilder.Build();

        // Initialize Datadog SDK
        DatadogCore.Datadog.Initialize(
            with: nativeConfig,
            trackingConsent: MapTrackingConsent(configuration.TrackingConsent)
        );

        // Set verbosity
        if (configuration.VerboseLogging)
        {
            DatadogCore.Datadog.VerbosityLevel = CoreLoggerLevel.Debug;
        }

        // Apply global tags
        foreach (var tag in configuration.GlobalTags)
        {
            DatadogCore.Datadog.SetTag(key: tag.Key, value: tag.Value);
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
        var rumConfigBuilder = DatadogRUM.RUM.Configuration.BuilderWithApplicationID(rumConfig.ApplicationId);

        rumConfigBuilder.Set(sessionSampleRate: rumConfig.SessionSampleRate);
        rumConfigBuilder.Set(telemetrySampleRate: rumConfig.TelemetrySampleRate);
        rumConfigBuilder.TrackUserInteractions(rumConfig.TrackUserInteractions);
        rumConfigBuilder.TrackLongTasks(true);

        if (rumConfig.VitalsUpdateFrequency != VitalsUpdateFrequency.Never)
        {
            rumConfigBuilder.TrackVitals(MapVitalsFrequency(rumConfig.VitalsUpdateFrequency));
        }

        DatadogRUM.RUM.Enable(with: rumConfigBuilder.Build());
    }

    private static void InitializeLogs(LogsConfiguration logsConfig)
    {
        var logsConfigBuilder = DatadogLogs.Logs.Configuration.BuilderWithNetworkInfoEnabled(
            logsConfig.NetworkInfoEnabled
        );

        logsConfigBuilder.Set(eventMapper: null); // Can be customized later

        DatadogLogs.Logs.Enable(with: logsConfigBuilder.Build());
    }

    private static void InitializeTracing(TracingConfiguration tracingConfig)
    {
        var traceConfigBuilder = DatadogTrace.Trace.Configuration.BuilderWithSampleRate(
            tracingConfig.SampleRate
        );

        DatadogTrace.Trace.Enable(with: traceConfigBuilder.Build());
    }

    static partial void PlatformSetUser(UserInfo userInfo)
    {
        var extraInfo = userInfo.ExtraInfo?.ToDictionary(
            kvp => kvp.Key,
            kvp => NSObject.FromObject(kvp.Value)
        );

        DatadogCore.Datadog.SetUserInfo(
            id: userInfo.Id,
            name: userInfo.Name,
            email: userInfo.Email,
            extraInfo: extraInfo != null ? NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
                extraInfo.Values.ToArray(),
                extraInfo.Keys.Select(k => new NSString(k)).ToArray()
            ) : null
        );
    }

    static partial void PlatformSetTags(Dictionary<string, string> tags)
    {
        foreach (var tag in tags)
        {
            DatadogCore.Datadog.SetTag(key: tag.Key, value: tag.Value);
        }
    }

    static partial void PlatformSetTrackingConsent(TrackingConsent consent)
    {
        DatadogCore.Datadog.Set(trackingConsent: MapTrackingConsent(consent));
    }

    static partial void PlatformClearUser()
    {
        DatadogCore.Datadog.ClearUserInfo();
    }

    // Helper methods to map enums
    private static DatadogSite MapSite(DatadogSite site)
    {
        return site switch
        {
            Maui.DatadogSite.US1 => DatadogCore.DatadogSite.Us1,
            Maui.DatadogSite.US3 => DatadogCore.DatadogSite.Us3,
            Maui.DatadogSite.US5 => DatadogCore.DatadogSite.Us5,
            Maui.DatadogSite.EU1 => DatadogCore.DatadogSite.Eu1,
            Maui.DatadogSite.US1_FED => DatadogCore.DatadogSite.Us1_FED,
            Maui.DatadogSite.AP1 => DatadogCore.DatadogSite.Ap1,
            _ => DatadogCore.DatadogSite.Us1
        };
    }

    private static TrackingConsent MapTrackingConsent(TrackingConsent consent)
    {
        return consent switch
        {
            Maui.TrackingConsent.Granted => DatadogCore.TrackingConsent.Granted,
            Maui.TrackingConsent.NotGranted => DatadogCore.TrackingConsent.NotGranted,
            Maui.TrackingConsent.Pending => DatadogCore.TrackingConsent.Pending,
            _ => DatadogCore.TrackingConsent.Pending
        };
    }

    private static DatadogRUM.VitalsFrequency MapVitalsFrequency(VitalsUpdateFrequency frequency)
    {
        return frequency switch
        {
            VitalsUpdateFrequency.Frequent => DatadogRUM.VitalsFrequency.Frequent,
            VitalsUpdateFrequency.Average => DatadogRUM.VitalsFrequency.Average,
            VitalsUpdateFrequency.Rare => DatadogRUM.VitalsFrequency.Rare,
            _ => DatadogRUM.VitalsFrequency.Average
        };
    }
}
