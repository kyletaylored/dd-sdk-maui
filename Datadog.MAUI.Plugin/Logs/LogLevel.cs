namespace Datadog.Maui.Logs;

/// <summary>
/// Log severity level.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Debug level - detailed information for diagnosing problems.
    /// </summary>
    Debug,

    /// <summary>
    /// Info level - informational messages.
    /// </summary>
    Info,

    /// <summary>
    /// Notice level - normal but significant events.
    /// </summary>
    Notice,

    /// <summary>
    /// Warn level - warning messages.
    /// </summary>
    Warn,

    /// <summary>
    /// Error level - error events.
    /// </summary>
    Error,

    /// <summary>
    /// Critical level - critical conditions.
    /// </summary>
    Critical,

    /// <summary>
    /// Alert level - action must be taken immediately.
    /// </summary>
    Alert,

    /// <summary>
    /// Emergency level - system is unusable.
    /// </summary>
    Emergency
}
