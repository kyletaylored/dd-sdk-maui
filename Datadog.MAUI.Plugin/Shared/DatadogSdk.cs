namespace Datadog.MAUI;

/// <summary>
/// Main entry point for the Datadog SDK.
/// Provides access to platform-specific implementations of Datadog features.
/// </summary>
public static class DatadogSdk
{
    private static IDatadogSdk? _instance;
    private static IDatadogLogger? _logger;
    private static IDatadogRum? _rum;
    private static IDatadogTrace? _trace;

    /// <summary>
    /// Gets the current Datadog SDK instance.
    /// </summary>
    public static IDatadogSdk Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets the Datadog logger instance.
    /// </summary>
    public static IDatadogLogger Logger
    {
        get
        {
            if (_logger == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _logger;
        }
    }

    /// <summary>
    /// Gets the Datadog RUM instance.
    /// </summary>
    public static IDatadogRum Rum
    {
        get
        {
            if (_rum == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _rum;
        }
    }

    /// <summary>
    /// Gets the Datadog Trace instance.
    /// </summary>
    public static IDatadogTrace Trace
    {
        get
        {
            if (_trace == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _trace;
        }
    }

    /// <summary>
    /// Initializes the Datadog SDK with the provided configuration.
    /// Must be called once during application startup.
    /// </summary>
    /// <param name="configuration">The SDK configuration</param>
    public static void Initialize(DatadogConfiguration configuration)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("Datadog SDK has already been initialized.");
        }

#if ANDROID
        _instance = new Platforms.Android.DatadogSdkImplementation();
        _logger = new Platforms.Android.DatadogLoggerImplementation();
        _rum = new Platforms.Android.DatadogRumImplementation();
        _trace = new Platforms.Android.DatadogTraceImplementation();
#elif IOS
        _instance = new Platforms.iOS.DatadogSdkImplementation();
        _logger = new Platforms.iOS.DatadogLoggerImplementation();
        _rum = new Platforms.iOS.DatadogRumImplementation();
        _trace = new Platforms.iOS.DatadogTraceImplementation();
#else
        throw new PlatformNotSupportedException("Datadog SDK is only supported on iOS and Android.");
#endif

        _instance.Initialize(configuration);
    }

    /// <summary>
    /// Resets the SDK (for testing purposes only).
    /// </summary>
    internal static void Reset()
    {
        _instance = null;
        _logger = null;
        _rum = null;
        _trace = null;
    }
}
