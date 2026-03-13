using Microsoft.Extensions.Configuration;

namespace DatadogMauiSample.Config;

public static class DatadogConfig
{
    // General settings
    public static string Environment { get; private set; } = "dev";
    public static string Site { get; private set; } = "US1";
    public static bool VerboseLogging { get; private set; } = true;
    public static float SessionSampleRate { get; private set; } = 100f;
    public static float SessionReplaySampleRate { get; private set; } = 100f;

    // First-party hosts for tracing
    public static List<string> FirstPartyHosts { get; private set; } = new()
    {
        "api.shopist.io",
        "shopist.io"
    };

    // Platform credentials
    public static string ServiceName { get; private set; } = "shopist-maui-demo";
    public static string AndroidClientToken { get; private set; } = "";
    public static string AndroidRumApplicationId { get; private set; } = "";
    public static string IosClientToken { get; private set; } = "";
    public static string IosRumApplicationId { get; private set; } = "";

    /// <summary>
    /// Load all Datadog settings from the app's IConfiguration (appsettings.json).
    /// </summary>
    public static void Load(IConfiguration configuration)
    {
        var dd = configuration.GetSection("Datadog");
        if (!dd.Exists())
        {
            Console.WriteLine("[Datadog] WARNING: No 'Datadog' section found in configuration");
            return;
        }

        Environment = dd["Environment"] ?? Environment;
        Site = dd["Site"] ?? Site;

        if (bool.TryParse(dd["VerboseLogging"], out var verbose))
            VerboseLogging = verbose;

        if (float.TryParse(dd["SessionSampleRate"], out var sessionRate))
            SessionSampleRate = sessionRate;

        if (float.TryParse(dd["SessionReplaySampleRate"], out var replayRate))
            SessionReplaySampleRate = replayRate;

        // Android credentials
        var android = dd.GetSection("Android");
        AndroidClientToken = android["ClientToken"] ?? "";
        AndroidRumApplicationId = android["ApplicationId"] ?? "";

        // iOS credentials
        var ios = dd.GetSection("iOS");
        IosClientToken = ios["ClientToken"] ?? "";
        IosRumApplicationId = ios["ApplicationId"] ?? "";

        // Platform-specific service name
#if ANDROID
        ServiceName = android["ServiceName"] ?? ServiceName;
#elif IOS
        ServiceName = ios["ServiceName"] ?? ServiceName;
#endif

        Console.WriteLine($"[Datadog] Configuration loaded from appsettings.json");
    }
}
