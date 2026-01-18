namespace Datadog.Maui.Rum;

/// <summary>
/// Source of an RUM error.
/// </summary>
public enum RumErrorSource
{
    /// <summary>
    /// Error from source code.
    /// </summary>
    Source,

    /// <summary>
    /// Network error.
    /// </summary>
    Network,

    /// <summary>
    /// WebView error.
    /// </summary>
    WebView,

    /// <summary>
    /// Custom error.
    /// </summary>
    Custom
}
