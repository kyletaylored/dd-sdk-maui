using DatadogMauiSample.Config;

namespace DatadogMauiSample;

public partial class DebugInfoPage : ContentPage
{
    public DebugInfoPage()
    {
        InitializeComponent();
        LoadDebugInfo();
    }

    private void LoadDebugInfo()
    {
        // Platform
#if ANDROID
        PlatformLabel.Text = "Android";
        ApplicationIdLabel.Text = MaskSensitiveData(DatadogConfig.AndroidRumApplicationId);
        ClientTokenLabel.Text = MaskToken(DatadogConfig.AndroidClientToken);
#elif IOS
        PlatformLabel.Text = "iOS";
        ApplicationIdLabel.Text = MaskSensitiveData(DatadogConfig.IosRumApplicationId);
        ClientTokenLabel.Text = MaskToken(DatadogConfig.IosClientToken);
#else
        PlatformLabel.Text = "Unknown";
        ApplicationIdLabel.Text = "N/A";
        ClientTokenLabel.Text = "N/A";
#endif

        // General configuration
        EnvironmentLabel.Text = DatadogConfig.Environment;
        ServiceNameLabel.Text = DatadogConfig.ServiceName;
        SessionSampleRateLabel.Text = $"{DatadogConfig.SessionSampleRate}%";
        SessionReplaySampleRateLabel.Text = $"{DatadogConfig.SessionReplaySampleRate}%";
        VerboseLoggingLabel.Text = DatadogConfig.VerboseLogging ? "Enabled" : "Disabled";

        // First party hosts
        if (DatadogConfig.FirstPartyHosts != null && DatadogConfig.FirstPartyHosts.Count > 0)
        {
            FirstPartyHostsLabel.Text = string.Join(", ", DatadogConfig.FirstPartyHosts);
        }
        else
        {
            FirstPartyHostsLabel.Text = "None configured";
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

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
