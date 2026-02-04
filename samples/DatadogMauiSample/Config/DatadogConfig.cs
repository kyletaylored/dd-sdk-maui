using Microsoft.Extensions.Configuration;

namespace DatadogMauiSample.Config;

/// <summary>
/// LEGACY: Configuration settings for Datadog RUM and logging.
/// This class is NO LONGER NEEDED with the new UseDatadogFromConfiguration() method.
/// Kept for reference only - can be used for accessing config values in application code if needed.
///
/// For Datadog initialization, use UseDatadogFromConfiguration() in MauiProgram.cs instead.
/// </summary>
[Obsolete("Use UseDatadogFromConfiguration() extension method for initialization. This class is kept for reference only.")]
public static class DatadogConfig
{
    private static IConfiguration? _configuration;

    /// <summary>
    /// Initialize the configuration with IConfiguration instance.
    /// Should be called from MauiProgram.cs after building the configuration.
    /// </summary>
    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the deployment environment (e.g., dev, staging, prod).
    /// </summary>
    public static string Environment => _configuration?["Datadog:Environment"] ?? "dev";

    /// <summary>
    /// Gets the service name for Datadog.
    /// </summary>
    public static string ServiceName => _configuration?["Datadog:ServiceName"] ?? "shopist-maui-demo";

    /// <summary>
    /// Datadog site to send data to. Options: US1 (default), US3, US5, EU1, AP1, GOV
    /// </summary>
    public static string Site => _configuration?["Datadog:Site"] ?? "US1";

    /// <summary>
    /// Gets whether verbose logging is enabled.
    /// </summary>
    public static bool VerboseLogging => _configuration?.GetValue<bool>("Datadog:VerboseLogging") ?? false;

    /// <summary>
    /// Gets the Android client token from configuration.
    /// </summary>
    public static string AndroidClientToken => _configuration?["Datadog:Android:ClientToken"] ?? "PLACEHOLDER_ANDROID_CLIENT_TOKEN";

    /// <summary>
    /// Gets the Android RUM application ID from configuration.
    /// </summary>
    public static string AndroidRumApplicationId => _configuration?["Datadog:Android:RumApplicationId"] ?? "PLACEHOLDER_ANDROID_APPLICATION_ID";

    /// <summary>
    /// Gets the iOS client token from configuration.
    /// </summary>
    public static string IosClientToken => _configuration?["Datadog:iOS:ClientToken"] ?? "PLACEHOLDER_IOS_CLIENT_TOKEN";

    /// <summary>
    /// Gets the iOS RUM application ID from configuration.
    /// </summary>
    public static string IosRumApplicationId => _configuration?["Datadog:iOS:RumApplicationId"] ?? "PLACEHOLDER_IOS_APPLICATION_ID";

    /// <summary>
    /// Gets the RUM session sample rate (0-100).
    /// </summary>
    public static float SessionSampleRate => _configuration?.GetValue<float>("Datadog:Rum:SessionSampleRate") ?? 100f;

    /// <summary>
    /// Gets the telemetry sample rate (0-100).
    /// </summary>
    public static float TelemetrySampleRate => _configuration?.GetValue<float>("Datadog:Rum:TelemetrySampleRate") ?? 100f;

    /// <summary>
    /// Gets the session replay sample rate (0-100).
    /// </summary>
    public static float SessionReplaySampleRate => _configuration?.GetValue<float>("Datadog:Rum:SessionReplaySampleRate") ?? 100f;

    /// <summary>
    /// Gets the list of first-party hosts for distributed tracing.
    /// </summary>
    public static List<string> FirstPartyHosts => _configuration?.GetSection("Datadog:FirstPartyHosts").Get<List<string>>() ?? new List<string> { "fakestoreapi.com" };
}
