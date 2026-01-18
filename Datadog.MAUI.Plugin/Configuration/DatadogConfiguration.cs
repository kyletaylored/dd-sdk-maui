namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for Datadog SDK initialization.
/// </summary>
public class DatadogConfiguration
{
    /// <summary>
    /// Client token for authentication with Datadog.
    /// </summary>
    public required string ClientToken { get; init; }

    /// <summary>
    /// Environment name (e.g., "production", "staging", "development").
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    /// Service name to identify the application.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Datadog site (region) for data collection.
    /// </summary>
    public DatadogSite Site { get; init; } = DatadogSite.US1;

    /// <summary>
    /// User tracking consent status.
    /// </summary>
    public TrackingConsent TrackingConsent { get; init; } = TrackingConsent.Pending;

    /// <summary>
    /// Global tags to attach to all events.
    /// </summary>
    public Dictionary<string, string> GlobalTags { get; init; } = new();

    /// <summary>
    /// Enable verbose logging for debugging.
    /// </summary>
    public bool VerboseLogging { get; init; }

    /// <summary>
    /// First-party hosts for distributed tracing.
    /// </summary>
    public string[] FirstPartyHosts { get; init; } = Array.Empty<string>();

    /// <summary>
    /// RUM configuration.
    /// </summary>
    public RumConfiguration? Rum { get; init; }

    /// <summary>
    /// Logs configuration.
    /// </summary>
    public LogsConfiguration? Logs { get; init; }

    /// <summary>
    /// Tracing configuration.
    /// </summary>
    public TracingConfiguration? Tracing { get; init; }

    /// <summary>
    /// Builder for creating DatadogConfiguration instances.
    /// </summary>
    public class Builder
    {
        private readonly string _clientToken;
        private string _environment = "development";
        private string _serviceName = string.Empty;
        private DatadogSite _site = DatadogSite.US1;
        private TrackingConsent _trackingConsent = TrackingConsent.Pending;
        private readonly Dictionary<string, string> _globalTags = new();
        private bool _verboseLogging;
        private string[] _firstPartyHosts = Array.Empty<string>();
        private RumConfiguration? _rum;
        private LogsConfiguration? _logs;
        private TracingConfiguration? _tracing;

        /// <summary>
        /// Creates a new configuration builder.
        /// </summary>
        /// <param name="clientToken">Client token for authentication with Datadog.</param>
        public Builder(string clientToken)
        {
            if (string.IsNullOrWhiteSpace(clientToken))
                throw new ArgumentException("Client token cannot be null or empty", nameof(clientToken));

            _clientToken = clientToken;
        }

        /// <summary>
        /// Sets the environment name.
        /// </summary>
        public Builder SetEnvironment(string environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            return this;
        }

        /// <summary>
        /// Sets the service name.
        /// </summary>
        public Builder SetServiceName(string serviceName)
        {
            _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            return this;
        }

        /// <summary>
        /// Sets the Datadog site (region).
        /// </summary>
        public Builder SetSite(DatadogSite site)
        {
            _site = site;
            return this;
        }

        /// <summary>
        /// Sets the tracking consent status.
        /// </summary>
        public Builder SetTrackingConsent(TrackingConsent consent)
        {
            _trackingConsent = consent;
            return this;
        }

        /// <summary>
        /// Adds a global tag to all events.
        /// </summary>
        public Builder AddGlobalTag(string key, string value)
        {
            _globalTags[key] = value;
            return this;
        }

        /// <summary>
        /// Enables verbose logging for debugging.
        /// </summary>
        public Builder EnableVerboseLogging()
        {
            _verboseLogging = true;
            return this;
        }

        /// <summary>
        /// Sets first-party hosts for distributed tracing.
        /// </summary>
        public Builder SetFirstPartyHosts(params string[] hosts)
        {
            _firstPartyHosts = hosts ?? Array.Empty<string>();
            return this;
        }

        /// <summary>
        /// Configures RUM (Real User Monitoring).
        /// </summary>
        public Builder EnableRum(Action<RumConfiguration.Builder> configure)
        {
            var builder = new RumConfiguration.Builder();
            configure(builder);
            _rum = builder.Build();
            return this;
        }

        /// <summary>
        /// Configures Logs collection.
        /// </summary>
        public Builder EnableLogs(Action<LogsConfiguration.Builder> configure)
        {
            var builder = new LogsConfiguration.Builder();
            configure(builder);
            _logs = builder.Build();
            return this;
        }

        /// <summary>
        /// Configures Tracing.
        /// </summary>
        public Builder EnableTracing(Action<TracingConfiguration.Builder> configure)
        {
            var builder = new TracingConfiguration.Builder();
            configure(builder);
            _tracing = builder.Build();
            return this;
        }

        /// <summary>
        /// Builds the configuration.
        /// </summary>
        public DatadogConfiguration Build()
        {
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
}
