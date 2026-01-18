namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for Logs collection.
/// </summary>
public class LogsConfiguration
{
    /// <summary>
    /// Sampling rate for logs (0-100).
    /// </summary>
    public int SampleRate { get; init; } = 100;

    /// <summary>
    /// Include network information with logs.
    /// </summary>
    public bool NetworkInfoEnabled { get; init; } = true;

    /// <summary>
    /// Bundle logs with RUM sessions.
    /// </summary>
    public bool BundleWithRum { get; init; } = true;

    /// <summary>
    /// Builder for creating LogsConfiguration instances.
    /// </summary>
    public class Builder
    {
        private int _sampleRate = 100;
        private bool _networkInfoEnabled = true;
        private bool _bundleWithRum = true;

        /// <summary>
        /// Sets the sampling rate for logs (0-100).
        /// </summary>
        public Builder SetSampleRate(int rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentOutOfRangeException(nameof(rate), "Sample rate must be between 0 and 100");

            _sampleRate = rate;
            return this;
        }

        /// <summary>
        /// Enables or disables network information in logs.
        /// </summary>
        public Builder EnableNetworkInfo(bool enable)
        {
            _networkInfoEnabled = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables bundling logs with RUM sessions.
        /// </summary>
        public Builder BundleWithRum(bool enable)
        {
            _bundleWithRum = enable;
            return this;
        }

        /// <summary>
        /// Builds the Logs configuration.
        /// </summary>
        public LogsConfiguration Build()
        {
            return new LogsConfiguration
            {
                SampleRate = _sampleRate,
                NetworkInfoEnabled = _networkInfoEnabled,
                BundleWithRum = _bundleWithRum
            };
        }
    }
}
