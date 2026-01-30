using System.Reflection;

namespace DatadogMauiSample.Config;

/// <summary>
/// Configuration settings for Datadog RUM and logging.
/// </summary>
public static class DatadogConfig
{
    /// <summary>
    /// Gets or sets the deployment environment (e.g., dev, staging, prod).
    /// </summary>
    public static string Environment { get; set; } = "dev";

    /// <summary>
    /// Gets or sets the service name for Datadog.
    /// </summary>
    public static string ServiceName { get; set; } = "shopist-maui-demo";

    /// <summary>
    /// Datadog site to send data to. Options: US1 (default), US3, US5, EU1, AP1, GOV
    /// </summary>
    public static string Site { get; set; } = "US1";

    /// <summary>
    /// Gets or sets a value indicating whether verbose logging is enabled.
    /// </summary>
    public static bool VerboseLogging { get; set; } = true;

    // Placeholder defaults (used if config file not found)
    private const string DefaultAndroidClientToken = "PLACEHOLDER_ANDROID_CLIENT_TOKEN";
    private const string DefaultAndroidApplicationId = "PLACEHOLDER_ANDROID_APPLICATION_ID";
    private const string DefaultIosClientToken = "PLACEHOLDER_IOS_CLIENT_TOKEN";
    private const string DefaultIosApplicationId = "PLACEHOLDER_IOS_APPLICATION_ID";

    // Lazy-loaded credentials from embedded config file
    private static readonly Lazy<Dictionary<string, string>> _credentials = new(LoadCredentials);

    /// <summary>
    /// Gets the Android client token (3-tier priority: Embedded Config > Environment Variable > Placeholder).
    /// </summary>
    public static string AndroidClientToken
    {
        get
        {
            // Priority 1: Embedded config file (build-time injected)
            if (_credentials.Value.TryGetValue("DD_RUM_ANDROID_CLIENT_TOKEN", out var configToken))
                return configToken;

            // Priority 2: Runtime environment variable
            var envToken = System.Environment.GetEnvironmentVariable("DD_RUM_ANDROID_CLIENT_TOKEN");
            if (!string.IsNullOrEmpty(envToken))
                return envToken;

            // Priority 3: Placeholder
            return DefaultAndroidClientToken;
        }
    }

    /// <summary>
    /// Gets the Android RUM application ID.
    /// </summary>
    public static string AndroidRumApplicationId
    {
        get
        {
            if (_credentials.Value.TryGetValue("DD_RUM_ANDROID_APPLICATION_ID", out var configId))
                return configId;

            var envId = System.Environment.GetEnvironmentVariable("DD_RUM_ANDROID_APPLICATION_ID");
            if (!string.IsNullOrEmpty(envId))
                return envId;

            return DefaultAndroidApplicationId;
        }
    }

    /// <summary>
    /// Gets the iOS client token (3-tier priority: Embedded Config > Environment Variable > Placeholder).
    /// </summary>
    public static string IosClientToken
    {
        get
        {
            if (_credentials.Value.TryGetValue("DD_RUM_IOS_CLIENT_TOKEN", out var configToken))
                return configToken;

            var envToken = System.Environment.GetEnvironmentVariable("DD_RUM_IOS_CLIENT_TOKEN");
            if (!string.IsNullOrEmpty(envToken))
                return envToken;

            return DefaultIosClientToken;
        }
    }

    /// <summary>
    /// Gets the iOS RUM application ID.
    /// </summary>
    public static string IosRumApplicationId
    {
        get
        {
            if (_credentials.Value.TryGetValue("DD_RUM_IOS_APPLICATION_ID", out var configId))
                return configId;

            var envId = System.Environment.GetEnvironmentVariable("DD_RUM_IOS_APPLICATION_ID");
            if (!string.IsNullOrEmpty(envId))
                return envId;

            return DefaultIosApplicationId;
        }
    }

    /// <summary>
    /// Gets or sets the RUM session sample rate (0-100).
    /// </summary>
    public static float SessionSampleRate { get; set; } = 100f;

    /// <summary>
    /// Gets or sets the session replay sample rate (0-100).
    /// </summary>
    public static float SessionReplaySampleRate { get; set; } = 100f;

    /// <summary>
    /// Gets or sets the list of first-party hosts for distributed tracing.
    /// </summary>
    public static List<string> FirstPartyHosts { get; set; } = new()
    {
        "api.shopist.io",
        "shopist.io"
    };

    /// <summary>
    /// Load credentials from embedded config file (generated at build time by MSBuild targets)
    /// </summary>
    private static Dictionary<string, string> LoadCredentials()
    {
        var creds = new Dictionary<string, string>();

        // Platform-specific resource name (determined at compile time)
#if ANDROID
        var resourceName = "DatadogMauiSample.Config.datadog-rum-android.config";
#elif IOS
        var resourceName = "DatadogMauiSample.Config.datadog-rum-ios.config";
#else
        var resourceName = "";
#endif

        if (string.IsNullOrEmpty(resourceName))
            return creds;

        try
        {
            // Load from embedded resource using reflection
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();

                // Parse KEY=VALUE format (semicolon-separated for multiple entries)
                foreach (var line in content.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = line.Trim().Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            creds[key] = value;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Datadog] Loaded {creds.Count} credentials from embedded config");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] No embedded config file found ({resourceName})");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Datadog] Failed to load embedded config: {ex.GetType().Name} - {ex.Message}");
        }

        return creds;
    }

    /// <summary>
    /// Optional: Load additional settings from environment variables
    /// (Credentials are loaded from embedded config, this is for other settings)
    /// </summary>
    public static void LoadFromEnvironment()
    {
        // Load general settings from environment variables
        Environment = GetEnvVar("DD_ENV", Environment);
        ServiceName = GetEnvVar("DD_SERVICE_NAME", ServiceName);
        Site = GetEnvVar("DD_SITE", Site);
        VerboseLogging = GetEnvVar("DD_VERBOSE_LOGGING", "true").ToLower() == "true";

        if (float.TryParse(GetEnvVar("DD_SESSION_SAMPLE_RATE", "100"), out var sessionRate))
        {
            SessionSampleRate = sessionRate;
        }

        if (float.TryParse(GetEnvVar("DD_SESSION_REPLAY_SAMPLE_RATE", "100"), out var replayRate))
        {
            SessionReplaySampleRate = replayRate;
        }
    }

    private static string GetEnvVar(string key, string defaultValue = "")
    {
        var value = System.Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }
}
