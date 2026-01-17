using DotNetEnv;

namespace DatadogMauiSample.Config;

public static class DatadogConfig
{
    // General settings
    public static string Environment { get; set; } = "dev";
    public static string ServiceName { get; set; } = "shopist-maui-demo";
    public static bool VerboseLogging { get; set; } = true;

    // Android settings
    public static string AndroidClientToken { get; set; } = string.Empty;
    public static string AndroidRumApplicationId { get; set; } = string.Empty;

    // iOS settings
    public static string IosClientToken { get; set; } = string.Empty;
    public static string IosRumApplicationId { get; set; } = string.Empty;

    // Sample rates
    public static float SessionSampleRate { get; set; } = 100f;
    public static float SessionReplaySampleRate { get; set; } = 100f;

    // First-party hosts for tracing
    public static List<string> FirstPartyHosts { get; set; } = new()
    {
        "api.shopist.io",
        "shopist.io"
    };

    public static void LoadFromEnvironment()
    {
        // Load .env file from app package
        try
        {
            // Try to read .env file as a MAUI asset
            using var stream = Microsoft.Maui.Storage.FileSystem.OpenAppPackageFileAsync(".env").Result;
            using var reader = new StreamReader(stream);
            var envContent = reader.ReadToEnd();

            // Parse .env content manually since DotNetEnv expects a file path
            foreach (var line in envContent.Split('\n'))
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    System.Environment.SetEnvironmentVariable(key, value);
                }
            }

            Console.WriteLine("[Datadog] Loaded .env file from app package");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Failed to load .env file: {ex.Message}");
        }

        // Load from environment variables
        Environment = GetEnvVar("DD_ENV", Environment);
        ServiceName = GetEnvVar("DD_SERVICE_NAME", ServiceName);

        AndroidClientToken = GetEnvVar("DD_RUM_ANDROID_CLIENT_TOKEN", AndroidClientToken);
        AndroidRumApplicationId = GetEnvVar("DD_RUM_ANDROID_APPLICATION_ID", AndroidRumApplicationId);

        IosClientToken = GetEnvVar("DD_RUM_IOS_CLIENT_TOKEN", IosClientToken);
        IosRumApplicationId = GetEnvVar("DD_RUM_IOS_APPLICATION_ID", IosRumApplicationId);

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
