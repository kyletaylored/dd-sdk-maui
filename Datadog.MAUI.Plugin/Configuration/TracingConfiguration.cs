namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for Tracing.
/// </summary>
public class TracingConfiguration
{
    /// <summary>
    /// Sampling rate for traces (0-100).
    /// </summary>
    public int SampleRate { get; init; } = 100;

    /// <summary>
    /// Enable trace ID generation.
    /// </summary>
    public bool TraceIdGenerationEnabled { get; init; } = true;

    /// <summary>
    /// First-party hosts for distributed tracing.
    /// </summary>
    public string[] FirstPartyHosts { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Builder for creating TracingConfiguration instances.
    /// </summary>
    public class Builder
    {
        private int _sampleRate = 100;
        private bool _traceIdGenerationEnabled = true;
        private string[] _firstPartyHosts = Array.Empty<string>();

        /// <summary>
        /// Sets the sampling rate for traces (0-100).
        /// </summary>
        public Builder SetSampleRate(int rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentOutOfRangeException(nameof(rate), "Sample rate must be between 0 and 100");

            _sampleRate = rate;
            return this;
        }

        /// <summary>
        /// Enables or disables trace ID generation.
        /// </summary>
        public Builder EnableTraceIdGeneration(bool enable)
        {
            _traceIdGenerationEnabled = enable;
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
        /// Builds the Tracing configuration.
        /// </summary>
        public TracingConfiguration Build()
        {
            return new TracingConfiguration
            {
                SampleRate = _sampleRate,
                TraceIdGenerationEnabled = _traceIdGenerationEnabled,
                FirstPartyHosts = _firstPartyHosts
            };
        }
    }
}
