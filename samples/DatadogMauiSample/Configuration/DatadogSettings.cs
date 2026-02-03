namespace DatadogMauiSample.Configuration;

/// <summary>
/// Datadog configuration settings loaded from appsettings.json.
/// </summary>
public class DatadogSettings
{
    public string Environment { get; set; } = "development";
    public string? ServiceName { get; set; }
    public string Site { get; set; } = "US1";
    public bool VerboseLogging { get; set; }
    public PlatformSettings Android { get; set; } = new();
    public PlatformSettings iOS { get; set; } = new();
    public string[] FirstPartyHosts { get; set; } = Array.Empty<string>();
    public RumSettings? Rum { get; set; }
    public LogsSettings? Logs { get; set; }
    public TracingSettings? Tracing { get; set; }
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
