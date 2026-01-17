namespace Datadog.MAUI;

/// <summary>
/// Configuration for initializing the Datadog SDK.
/// </summary>
public class DatadogConfiguration
{
    /// <summary>
    /// Your Datadog client token (required).
    /// </summary>
    public required string ClientToken { get; set; }

    /// <summary>
    /// Your environment name (e.g., "prod", "staging", "dev").
    /// </summary>
    public required string Environment { get; set; }

    /// <summary>
    /// Your application ID for RUM (optional).
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Service name for your application (optional).
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Datadog site (e.g., "US1", "EU1", "US3", "US5"). Defaults to "US1".
    /// </summary>
    public DatadogSite Site { get; set; } = DatadogSite.US1;

    /// <summary>
    /// Whether to enable crash reporting. Defaults to true.
    /// </summary>
    public bool EnableCrashReporting { get; set; } = true;

    /// <summary>
    /// Whether to track user interactions automatically. Defaults to true.
    /// </summary>
    public bool TrackUserInteractions { get; set; } = true;

    /// <summary>
    /// Whether to track network requests automatically. Defaults to true.
    /// </summary>
    public bool TrackNetworkRequests { get; set; } = true;

    /// <summary>
    /// Whether to track view lifecycle automatically. Defaults to true.
    /// </summary>
    public bool TrackViewLifecycle { get; set; } = true;

    /// <summary>
    /// Sample rate for RUM sessions (0.0 to 100.0). Defaults to 100.0 (all sessions).
    /// </summary>
    public float SessionSampleRate { get; set; } = 100.0f;

    /// <summary>
    /// Sample rate for traces (0.0 to 100.0). Defaults to 100.0 (all traces).
    /// </summary>
    public float TraceSampleRate { get; set; } = 100.0f;

    /// <summary>
    /// Additional custom attributes to attach to all events.
    /// </summary>
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}

/// <summary>
/// Datadog site locations.
/// </summary>
public enum DatadogSite
{
    /// <summary>
    /// US1 (datadoghq.com)
    /// </summary>
    US1,

    /// <summary>
    /// EU1 (datadoghq.eu)
    /// </summary>
    EU1,

    /// <summary>
    /// US3 (us3.datadoghq.com)
    /// </summary>
    US3,

    /// <summary>
    /// US5 (us5.datadoghq.com)
    /// </summary>
    US5,

    /// <summary>
    /// US1_FED (ddog-gov.com)
    /// </summary>
    US1_FED,

    /// <summary>
    /// AP1 (ap1.datadoghq.com)
    /// </summary>
    AP1
}
