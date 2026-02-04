using Microsoft.Extensions.Configuration;

namespace DatadogMauiSample.Views;

/// <summary>
/// Page for displaying debug information about the Datadog SDK.
/// </summary>
public partial class DebugInfoPage : ContentPage
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugInfoPage"/> class.
    /// </summary>
    public DebugInfoPage()
    {
        InitializeComponent();

        // Get configuration from the application
        _configuration = Application.Current?.Handler?.MauiContext?.Services.GetService<IConfiguration>()
            ?? throw new InvalidOperationException("Configuration service not available");

        LoadDebugInfo();
    }

    private void LoadDebugInfo()
    {
        // Check if Datadog SDK is initialized using the SDK's initialization flag
        // This works cross-platform (Android, iOS, etc.)
        if (Datadog.Maui.Datadog.IsInitialized)
        {
            SdkStatusLabel.Text = "✅ SDK Initialized";
            SdkStatusLabel.TextColor = Colors.Green;
        }
        else
        {
            SdkStatusLabel.Text = "❌ NOT INITIALIZED\nDatadog.Initialize() was not called";
            SdkStatusLabel.TextColor = Colors.Red;
        }

        // Load app version information
        LoadVersionInfo();

        // Platform
#if ANDROID
        PlatformLabel.Text = "Android";
        ApplicationIdLabel.Text = MaskSensitiveData(_configuration["Datadog:Android:RumApplicationId"] ?? "");
        ClientTokenLabel.Text = MaskToken(_configuration["Datadog:Android:ClientToken"] ?? "");
#elif IOS
        PlatformLabel.Text = "iOS";
        ApplicationIdLabel.Text = MaskSensitiveData(_configuration["Datadog:iOS:RumApplicationId"] ?? "");
        ClientTokenLabel.Text = MaskToken(_configuration["Datadog:iOS:ClientToken"] ?? "");
#else
        PlatformLabel.Text = "Unknown";
        ApplicationIdLabel.Text = "N/A";
        ClientTokenLabel.Text = "N/A";
#endif

        // General configuration
        EnvironmentLabel.Text = _configuration["Datadog:Environment"] ?? "N/A";
        ServiceNameLabel.Text = _configuration["Datadog:ServiceName"] ?? "N/A";

        var sessionSampleRate = _configuration["Datadog:Rum:SessionSampleRate"] ?? "100";
        SessionSampleRateLabel.Text = $"{sessionSampleRate}%";

        var sessionReplaySampleRate = _configuration["Datadog:SessionReplay:SampleRate"] ?? "20";
        SessionReplaySampleRateLabel.Text = $"{sessionReplaySampleRate}%";

        VerboseLoggingLabel.Text = "N/A"; // VerboseLogging not in appsettings.json

        // First party hosts
        var firstPartyHosts = _configuration.GetSection("Datadog:FirstPartyHosts").Get<string[]>();
        if (firstPartyHosts != null && firstPartyHosts.Length > 0)
        {
            FirstPartyHostsLabel.Text = string.Join(", ", firstPartyHosts);
        }
        else
        {
            FirstPartyHostsLabel.Text = "None configured";
        }
    }

    private void LoadVersionInfo()
    {
        try
        {
            // Track the version (must be called before accessing properties)
            VersionTracking.Track();

            // Current version and build
            AppVersionLabel.Text = VersionTracking.CurrentVersion;
            AppBuildLabel.Text = VersionTracking.CurrentBuild;

            // First launch info
            if (VersionTracking.IsFirstLaunchEver)
            {
                FirstLaunchLabel.Text = "Yes (first time ever)";
                FirstLaunchLabel.TextColor = Colors.Green;
            }
            else if (VersionTracking.IsFirstLaunchForCurrentVersion)
            {
                FirstLaunchLabel.Text = $"Yes (for v{VersionTracking.CurrentVersion})";
                FirstLaunchLabel.TextColor = Colors.Orange;
            }
            else
            {
                FirstLaunchLabel.Text = "No";
                FirstLaunchLabel.TextColor = Colors.Gray;
            }

            // When this version was first installed
            if (VersionTracking.IsFirstLaunchForCurrentVersion)
            {
                VersionInstalledLabel.Text = "Just now";
                VersionInstalledLabel.TextColor = Colors.Green;
            }
            else
            {
                // Show previous version if available
                var previousVersion = VersionTracking.PreviousVersion;
                if (!string.IsNullOrEmpty(previousVersion))
                {
                    VersionInstalledLabel.Text = $"Upgraded from v{previousVersion}";
                    VersionInstalledLabel.TextColor = Colors.Blue;
                }
                else
                {
                    VersionInstalledLabel.Text = "Current install";
                    VersionInstalledLabel.TextColor = Colors.Gray;
                }
            }
        }
        catch (Exception ex)
        {
            AppVersionLabel.Text = "Error loading version info";
            AppBuildLabel.Text = ex.Message;
            FirstLaunchLabel.Text = "N/A";
            VersionInstalledLabel.Text = "N/A";
        }
    }

    private string MaskSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "Not configured";

        // Show first 8 and last 4 characters
        if (data.Length <= 12)
            return data;

        return $"{data.Substring(0, 8)}...{data.Substring(data.Length - 4)}";
    }

    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return "Not configured";

        // Show only first 4 characters for tokens
        if (token.Length <= 8)
            return token.Substring(0, Math.Min(4, token.Length)) + "****";

        return $"{token.Substring(0, 4)}{'*'.ToString().PadLeft(token.Length - 8, '*')}{token.Substring(token.Length - 4)}";
    }
}
