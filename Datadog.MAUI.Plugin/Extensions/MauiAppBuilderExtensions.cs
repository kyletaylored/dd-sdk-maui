using Datadog.Maui.Configuration;

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

        var config = configBuilder.Build();

        // Initialize Datadog when the app starts
        Datadog.Initialize(config);

        return builder;
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

    /// <summary>
    /// Sets the client token for authentication with Datadog.
    /// </summary>
    public string ClientToken
    {
        set => _clientToken = value;
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
    public void EnableLogs(Action<LogsConfiguration.Builder> configure)
    {
        var builder = new LogsConfiguration.Builder();
        configure(builder);
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

    internal DatadogConfiguration Build()
    {
        if (string.IsNullOrWhiteSpace(_clientToken))
            throw new InvalidOperationException("ClientToken must be set");

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
            Tracing = _tracing
        };
    }
}
