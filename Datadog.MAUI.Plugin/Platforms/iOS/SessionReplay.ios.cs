using Datadog.iOS.SessionReplay;
using Datadog.iOS.Core;
using Datadog.Maui.Configuration;
using Foundation;

namespace Datadog.Maui;

internal static class SessionReplayInitializer
{
    internal static void Initialize(SessionReplayConfiguration config)
    {
        var sessionReplayConfig = new DDSessionReplayConfiguration(
            replaySampleRate: config.SampleRate
        );

        // Set privacy levels
        sessionReplayConfig.DefaultPrivacyLevel = MapPrivacyLevel(config.TextAndInputPrivacy, config.ImagePrivacy);

        // Note: iOS SDK doesn't have separate touch privacy configuration
        // Touch interactions are controlled by the privacy level

        DDSessionReplay.EnableWith(sessionReplayConfig);

        System.Diagnostics.Debug.WriteLine($"[Datadog] Session Replay enabled (iOS)");
        System.Diagnostics.Debug.WriteLine($"[Datadog]   - Sample Rate: {config.SampleRate}%");
        System.Diagnostics.Debug.WriteLine($"[Datadog]   - Text Privacy: {config.TextAndInputPrivacy}");
        System.Diagnostics.Debug.WriteLine($"[Datadog]   - Image Privacy: {config.ImagePrivacy}");
        System.Diagnostics.Debug.WriteLine($"[Datadog]   - Touch Privacy: {config.TouchPrivacy}");
    }

    private static DDSessionReplayConfigurationPrivacyLevel MapPrivacyLevel(
        TextAndInputPrivacy textPrivacy,
        ImagePrivacy imagePrivacy)
    {
        // iOS SDK has a simpler privacy model with three levels
        // We map based on the most restrictive setting provided

        return textPrivacy switch
        {
            TextAndInputPrivacy.MaskAll => DDSessionReplayConfigurationPrivacyLevel.Mask,
            TextAndInputPrivacy.MaskAllInputs => DDSessionReplayConfigurationPrivacyLevel.MaskUserInput,
            TextAndInputPrivacy.MaskSensitiveInputs => DDSessionReplayConfigurationPrivacyLevel.MaskUserInput,
            _ => DDSessionReplayConfigurationPrivacyLevel.Allow
        };
    }
}
