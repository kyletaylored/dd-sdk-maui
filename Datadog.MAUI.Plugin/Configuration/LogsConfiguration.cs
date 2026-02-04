namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for Logs collection.
/// </summary>
public class LogsConfiguration
{
    /// <summary>
    /// Builder for creating LogsConfiguration instances.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Builds the Logs configuration.
        /// </summary>
        public LogsConfiguration Build()
        {
            return new LogsConfiguration();
        }
    }
}
