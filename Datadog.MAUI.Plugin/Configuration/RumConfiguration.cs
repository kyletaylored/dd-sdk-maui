namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for RUM (Real User Monitoring).
/// </summary>
public class RumConfiguration
{
    /// <summary>
    /// RUM application ID.
    /// </summary>
    public required string ApplicationId { get; init; }

    /// <summary>
    /// Session sampling rate (0-100).
    /// </summary>
    public int SessionSampleRate { get; init; } = 100;

    /// <summary>
    /// Telemetry sampling rate (0-100).
    /// </summary>
    public int TelemetrySampleRate { get; init; } = 20;

    /// <summary>
    /// Automatically track view navigation.
    /// </summary>
    public bool TrackViewsAutomatically { get; init; } = true;

    /// <summary>
    /// Automatically track user interactions (taps, clicks).
    /// </summary>
    public bool TrackUserInteractions { get; init; } = true;

    /// <summary>
    /// Automatically track network resources.
    /// </summary>
    public bool TrackResources { get; init; } = true;

    /// <summary>
    /// Automatically track errors.
    /// </summary>
    public bool TrackErrors { get; init; } = true;

    /// <summary>
    /// Vitals update frequency.
    /// </summary>
    public VitalsUpdateFrequency VitalsUpdateFrequency { get; init; } = VitalsUpdateFrequency.Average;

    /// <summary>
    /// Builder for creating RumConfiguration instances.
    /// </summary>
    public class Builder
    {
        private string? _applicationId;
        private int _sessionSampleRate = 100;
        private int _telemetrySampleRate = 20;
        private bool _trackViewsAutomatically = true;
        private bool _trackUserInteractions = true;
        private bool _trackResources = true;
        private bool _trackErrors = true;
        private VitalsUpdateFrequency _vitalsUpdateFrequency = VitalsUpdateFrequency.Average;

        /// <summary>
        /// Sets the RUM application ID.
        /// </summary>
        public Builder SetApplicationId(string applicationId)
        {
            _applicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
            return this;
        }

        /// <summary>
        /// Sets the session sampling rate (0-100).
        /// </summary>
        public Builder SetSessionSampleRate(int rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentOutOfRangeException(nameof(rate), "Sample rate must be between 0 and 100");

            _sessionSampleRate = rate;
            return this;
        }

        /// <summary>
        /// Sets the telemetry sampling rate (0-100).
        /// </summary>
        public Builder SetTelemetrySampleRate(int rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentOutOfRangeException(nameof(rate), "Sample rate must be between 0 and 100");

            _telemetrySampleRate = rate;
            return this;
        }

        /// <summary>
        /// Enables or disables automatic view tracking.
        /// </summary>
        public Builder TrackViewsAutomatically(bool enable)
        {
            _trackViewsAutomatically = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables automatic user interaction tracking.
        /// </summary>
        public Builder TrackUserInteractions(bool enable)
        {
            _trackUserInteractions = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables automatic resource tracking.
        /// </summary>
        public Builder TrackResources(bool enable)
        {
            _trackResources = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables automatic error tracking.
        /// </summary>
        public Builder TrackErrors(bool enable)
        {
            _trackErrors = enable;
            return this;
        }

        /// <summary>
        /// Sets the vitals update frequency.
        /// </summary>
        public Builder SetVitalsUpdateFrequency(VitalsUpdateFrequency frequency)
        {
            _vitalsUpdateFrequency = frequency;
            return this;
        }

        /// <summary>
        /// Builds the RUM configuration.
        /// </summary>
        public RumConfiguration Build()
        {
            if (string.IsNullOrWhiteSpace(_applicationId))
                throw new InvalidOperationException("ApplicationId must be set for RUM configuration");

            return new RumConfiguration
            {
                ApplicationId = _applicationId,
                SessionSampleRate = _sessionSampleRate,
                TelemetrySampleRate = _telemetrySampleRate,
                TrackViewsAutomatically = _trackViewsAutomatically,
                TrackUserInteractions = _trackUserInteractions,
                TrackResources = _trackResources,
                TrackErrors = _trackErrors,
                VitalsUpdateFrequency = _vitalsUpdateFrequency
            };
        }
    }
}

/// <summary>
/// Frequency for updating mobile vitals.
/// </summary>
public enum VitalsUpdateFrequency
{
    /// <summary>
    /// Frequent updates (500ms).
    /// </summary>
    Frequent,

    /// <summary>
    /// Average updates (1s).
    /// </summary>
    Average,

    /// <summary>
    /// Rare updates (2s).
    /// </summary>
    Rare,

    /// <summary>
    /// No vitals tracking.
    /// </summary>
    Never
}
