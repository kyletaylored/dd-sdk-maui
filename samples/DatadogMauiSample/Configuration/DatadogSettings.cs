using Datadog.Maui;

namespace DatadogMauiSample.Configuration;

/// <summary>
/// LEGACY: Datadog configuration settings loaded from appsettings.json.
/// This class is NO LONGER NEEDED with the new UseDatadogFromConfiguration() method.
/// Kept for reference only.
///
/// See MauiProgram.cs for the simplified approach using UseDatadogFromConfiguration().
/// </summary>
[Obsolete("Use UseDatadogFromConfiguration() extension method instead. This class is kept for reference only.")]
public class DatadogSettings
{
    public string Environment { get; set; } = "development";
    public string? ServiceName { get; set; }
    public string SiteString { get; set; } = "US1";
    public bool VerboseLogging { get; set; }
    public PlatformSettings Android { get; set; } = new();
    public PlatformSettings iOS { get; set; } = new();
    public string[] FirstPartyHosts { get; set; } = Array.Empty<string>();
    public RumSettings? Rum { get; set; }
    public LogsSettings? Logs { get; set; }
    public TracingSettings? Tracing { get; set; }

    /// <summary>
    /// Converts the site string to DatadogSite enum.
    /// </summary>
    public DatadogSite Site => SiteString.ToUpperInvariant() switch
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

public class PlatformSettings
{
    public string? ClientToken { get; set; }
    public string? RumApplicationId { get; set; }
}

public class RumSettings
{
    public int SessionSampleRate { get; set; } = 100;
    public int TelemetrySampleRate { get; set; } = 100;
    public bool TrackLongTasks { get; set; } = true;
    public bool TrackUserInteractions { get; set; } = true;
    public bool TrackFrustrations { get; set; } = true;
}

public class LogsSettings
{
    public bool Enabled { get; set; } = true;
}

public class TracingSettings
{
    public int SampleRate { get; set; } = 100;
}
