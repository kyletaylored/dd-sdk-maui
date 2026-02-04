using Datadog.Maui.Configuration;
using Microsoft.Extensions.Configuration;

namespace Datadog.Maui.Extensions;

/// <summary>
/// Extension methods for configuring Datadog in a MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Adds Datadog SDK to the MAUI application.
    /// </summary>
    /// <param name="builder">The MAUI app builder.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The MAUI app builder for chaining.</returns>
    public static MauiAppBuilder UseDatadog(this MauiAppBuilder builder, Action<DatadogConfigurationBuilder> configure)
    {
        var configBuilder = new DatadogConfigurationBuilder();
        configure(configBuilder);

        try
        {
            var config = configBuilder.Build();

            // Initialize Datadog when the app starts
            Datadog.Initialize(config);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("ClientToken"))
        {
            // Log warning but don't crash the app
            System.Diagnostics.Debug.WriteLine("=============================================================");
            System.Diagnostics.Debug.WriteLine("[Datadog] WARNING: ClientToken not configured");
            System.Diagnostics.Debug.WriteLine("[Datadog] The app will run but telemetry will NOT be sent to Datadog");
            System.Diagnostics.Debug.WriteLine("[Datadog] To enable Datadog:");
            System.Diagnostics.Debug.WriteLine("[Datadog]   1. Get credentials from https://app.datadoghq.com/organization-settings/client-tokens");
            System.Diagnostics.Debug.WriteLine("[Datadog]   2. Configure ClientToken via appsettings.json or programmatically");
            System.Diagnostics.Debug.WriteLine("=============================================================");
            Console.WriteLine("[Datadog] WARNING: Running without Datadog telemetry - ClientToken not configured");
        }

        return builder;
    }

    /// <summary>
    /// Adds Datadog SDK to the MAUI application using configuration from appsettings.json.
    /// </summary>
    /// <param name="builder">The MAUI app builder.</param>
    /// <param name="sectionName">Configuration section name (default: "Datadog").</param>
    /// <param name="configure">Optional additional configuration action.</param>
    /// <param name="configureRum">Optional action to configure RUM settings (Variant, BuildId, etc.) without replacing the configuration from JSON.</param>
    /// <returns>The MAUI app builder for chaining.</returns>
    /// <remarks>
    /// Expects configuration structure:
    /// <code>
    /// {
    ///   "Datadog": {
    ///     "Environment": "production",
    ///     "ServiceName": "my-app",
    ///     "Site": "US1",
    ///     "FirstPartyHosts": ["api.example.com"],
    ///     "Android": {
    ///       "ClientToken": "pub...",
    ///       "RumApplicationId": "..."
    ///     },
    ///     "iOS": {
    ///       "ClientToken": "pub...",
    ///       "RumApplicationId": "..."
    ///     }
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public static MauiAppBuilder UseDatadogFromConfiguration(
        this MauiAppBuilder builder,
        string sectionName = "Datadog",
        Action<DatadogConfigurationBuilder>? configure = null,
        Action<RumConfiguration.Builder>? configureRum = null)
    {
        var config = builder.Configuration.GetSection(sectionName);

        return builder.UseDatadog(datadog =>
        {
            // Read platform-specific tokens
            var androidToken = config["Android:ClientToken"];
            var iosToken = config["iOS:ClientToken"];

            if (!string.IsNullOrWhiteSpace(androidToken) && !string.IsNullOrWhiteSpace(iosToken))
            {
                datadog.SetClientToken(
                    android: androidToken,
                    ios: iosToken
                );
            }

            // Read common settings
            var environment = config["Environment"];
            if (!string.IsNullOrWhiteSpace(environment))
            {
                datadog.Environment = environment;
            }

            var serviceName = config["ServiceName"];
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                datadog.ServiceName = serviceName;
            }

            var siteString = config["Site"];
            if (!string.IsNullOrWhiteSpace(siteString))
            {
                datadog.Site = ParseDatadogSite(siteString);
            }

            var firstPartyHosts = config.GetSection("FirstPartyHosts").Get<string[]>();
            if (firstPartyHosts?.Length > 0)
            {
                datadog.FirstPartyHosts = firstPartyHosts;
            }

            // Default to Granted if not specified
            var trackingConsent = config["TrackingConsent"];
            datadog.TrackingConsent = string.IsNullOrWhiteSpace(trackingConsent)
                ? TrackingConsent.Granted
                : ParseTrackingConsent(trackingConsent);

            // Enable RUM if application IDs are provided
            var androidRumId = config["Android:RumApplicationId"];
            var iosRumId = config["iOS:RumApplicationId"];

            if (!string.IsNullOrWhiteSpace(androidRumId) && !string.IsNullOrWhiteSpace(iosRumId))
            {
                datadog.EnableRum(rum =>
                {
                    rum.SetApplicationId(
                        android: androidRumId,
                        ios: iosRumId
                    );

                    // Optional RUM configuration
                    if (int.TryParse(config["Rum:SessionSampleRate"], out var sessionSampleRate))
                    {
                        rum.SetSessionSampleRate(sessionSampleRate);
                    }

                    // Apply additional RUM configuration (e.g., Variant, BuildId)
                    configureRum?.Invoke(rum);
                });
            }

            // Enable Logs if configured
            var logsEnabled = config.GetValue<bool?>("Logs:Enabled");
            if (logsEnabled != false) // Enable by default
            {
                datadog.EnableLogs();
            }

            // Enable Tracing if configured
            var tracingEnabled = config.GetValue<bool?>("Tracing:Enabled");
            if (tracingEnabled == true && firstPartyHosts?.Length > 0)
            {
                datadog.EnableTracing(tracing =>
                {
                    tracing.SetFirstPartyHosts(firstPartyHosts);

                    if (int.TryParse(config["Tracing:SampleRate"], out var traceSampleRate))
                    {
                        tracing.SetSampleRate(traceSampleRate);
                    }
                });
            }

            // Enable Session Replay if configured
            var sessionReplayEnabled = config.GetValue<bool?>("SessionReplay:Enabled");
            if (sessionReplayEnabled == true)
            {
                datadog.EnableSessionReplay(sessionReplay =>
                {
                    if (int.TryParse(config["SessionReplay:SampleRate"], out var sampleRate))
                    {
                        sessionReplay.SetSampleRate(sampleRate);
                    }

                    var textPrivacy = config["SessionReplay:TextAndInputPrivacy"];
                    if (!string.IsNullOrWhiteSpace(textPrivacy))
                    {
                        sessionReplay.SetTextAndInputPrivacy(ParseTextAndInputPrivacy(textPrivacy));
                    }

                    var imagePrivacy = config["SessionReplay:ImagePrivacy"];
                    if (!string.IsNullOrWhiteSpace(imagePrivacy))
                    {
                        sessionReplay.SetImagePrivacy(ParseImagePrivacy(imagePrivacy));
                    }

                    var touchPrivacy = config["SessionReplay:TouchPrivacy"];
                    if (!string.IsNullOrWhiteSpace(touchPrivacy))
                    {
                        sessionReplay.SetTouchPrivacy(ParseTouchPrivacy(touchPrivacy));
                    }
                });
            }

            // Allow programmatic overrides
            configure?.Invoke(datadog);
        });
    }

    private static DatadogSite ParseDatadogSite(string value)
    {
        return value.ToUpperInvariant() switch
        {
            "US1" => DatadogSite.US1,
            "US3" => DatadogSite.US3,
            "US5" => DatadogSite.US5,
            "EU1" => DatadogSite.EU1,
            "AP1" => DatadogSite.AP1,
            "US1_FED" => DatadogSite.US1_FED,
            _ => DatadogSite.US1
        };
    }

    private static TrackingConsent ParseTrackingConsent(string value)
    {
        return value.ToUpperInvariant() switch
        {
            "GRANTED" => TrackingConsent.Granted,
            "NOT_GRANTED" => TrackingConsent.NotGranted,
            "PENDING" => TrackingConsent.Pending,
            _ => TrackingConsent.Pending
        };
    }

    private static TextAndInputPrivacy ParseTextAndInputPrivacy(string value)
    {
        return value switch
        {
            "MaskAll" => TextAndInputPrivacy.MaskAll,
            "MaskAllInputs" => TextAndInputPrivacy.MaskAllInputs,
            "MaskSensitiveInputs" => TextAndInputPrivacy.MaskSensitiveInputs,
            _ => TextAndInputPrivacy.MaskSensitiveInputs
        };
    }

    private static ImagePrivacy ParseImagePrivacy(string value)
    {
        return value switch
        {
            "MaskAll" => ImagePrivacy.MaskAll,
            "MaskNone" => ImagePrivacy.MaskNone,
            "MaskNonBundledOnly" => ImagePrivacy.MaskNonBundledOnly,
            _ => ImagePrivacy.MaskNonBundledOnly
        };
    }

    private static TouchPrivacy ParseTouchPrivacy(string value)
    {
        return value switch
        {
            "Show" => TouchPrivacy.Show,
            "Hide" => TouchPrivacy.Hide,
            _ => TouchPrivacy.Show
        };
    }
}

/// <summary>
/// Fluent builder for Datadog configuration in MAUI apps.
/// </summary>
public class DatadogConfigurationBuilder
{
    private string? _clientToken;
    private string _environment = "development";
    private string? _serviceName;
    private DatadogSite _site = DatadogSite.US1;
    private TrackingConsent _trackingConsent = TrackingConsent.Pending;
    private readonly Dictionary<string, string> _globalTags = new();
    private bool _verboseLogging;
    private string[] _firstPartyHosts = Array.Empty<string>();
    private RumConfiguration? _rum;
    private LogsConfiguration? _logs;
    private TracingConfiguration? _tracing;
    private SessionReplayConfiguration? _sessionReplay;

    /// <summary>
    /// Sets the client token for authentication with Datadog.
    /// </summary>
    public string ClientToken
    {
        set => _clientToken = value;
    }

    /// <summary>
    /// Sets platform-specific client tokens for authentication with Datadog.
    /// </summary>
    /// <param name="android">Client token for Android.</param>
    /// <param name="ios">Client token for iOS.</param>
    public void SetClientToken(string android, string ios)
    {
#if ANDROID
        _clientToken = android;
#elif IOS
        _clientToken = ios;
#else
        _clientToken = android; // Default to Android for other platforms
#endif
    }

    /// <summary>
    /// Sets the environment name.
    /// </summary>
    public string Environment
    {
        set => _environment = value;
    }

    /// <summary>
    /// Sets the service name.
    /// </summary>
    public string ServiceName
    {
        set => _serviceName = value;
    }

    /// <summary>
    /// Sets the Datadog site (region).
    /// </summary>
    public DatadogSite Site
    {
        set => _site = value;
    }

    /// <summary>
    /// Sets the tracking consent status.
    /// </summary>
    public TrackingConsent TrackingConsent
    {
        set => _trackingConsent = value;
    }

    /// <summary>
    /// Gets the global tags dictionary.
    /// </summary>
    public Dictionary<string, string> GlobalTags => _globalTags;

    /// <summary>
    /// Sets verbose logging.
    /// </summary>
    public bool VerboseLogging
    {
        set => _verboseLogging = value;
    }

    /// <summary>
    /// Sets first-party hosts for distributed tracing.
    /// </summary>
    public string[] FirstPartyHosts
    {
        set => _firstPartyHosts = value ?? Array.Empty<string>();
    }

    /// <summary>
    /// Configures RUM (Real User Monitoring).
    /// </summary>
    public void EnableRum(Action<RumConfiguration.Builder> configure)
    {
        var builder = new RumConfiguration.Builder();
        configure(builder);
        _rum = builder.Build();
    }

    /// <summary>
    /// Configures Logs collection.
    /// </summary>
    public void EnableLogs(Action<LogsConfiguration.Builder>? configure = null)
    {
        var builder = new LogsConfiguration.Builder();
        configure?.Invoke(builder);
        _logs = builder.Build();
    }

    /// <summary>
    /// Configures Tracing.
    /// </summary>
    public void EnableTracing(Action<TracingConfiguration.Builder> configure)
    {
        var builder = new TracingConfiguration.Builder();
        configure(builder);
        _tracing = builder.Build();
    }

    /// <summary>
    /// Configures Session Replay.
    /// </summary>
    public void EnableSessionReplay(Action<SessionReplayConfiguration.Builder> configure)
    {
        var builder = new SessionReplayConfiguration.Builder();
        configure(builder);
        _sessionReplay = builder.Build();
    }

    internal DatadogConfiguration Build()
    {
        if (string.IsNullOrWhiteSpace(_clientToken))
        {
            var platform =
#if ANDROID
                "Android";
#elif IOS
                "iOS";
#else
                "Unknown";
#endif
            throw new InvalidOperationException(
                $"ClientToken must be set for {platform}. " +
                "Update your appsettings.json with valid Datadog credentials. " +
                "Get tokens from: https://app.datadoghq.com/organization-settings/client-tokens");
        }

        if (string.IsNullOrWhiteSpace(_serviceName))
        {
            // Default service name to app package name or assembly name
            _serviceName = AppInfo.PackageName ?? AppInfo.Name ?? "unknown";
        }

        return new DatadogConfiguration
        {
            ClientToken = _clientToken,
            Environment = _environment,
            ServiceName = _serviceName,
            Site = _site,
            TrackingConsent = _trackingConsent,
            GlobalTags = _globalTags,
            VerboseLogging = _verboseLogging,
            FirstPartyHosts = _firstPartyHosts,
            Rum = _rum,
            Logs = _logs,
            Tracing = _tracing,
            SessionReplay = _sessionReplay
        };
    }
}
