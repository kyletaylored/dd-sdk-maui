using Datadog.iOS.SessionReplay;
using Datadog.iOS.Core;
using Datadog.Maui.Configuration;
using Foundation;

namespace Datadog.Maui;

internal static class SessionReplayInitializer
{
    internal static void Initialize(SessionReplayConfiguration config)
    {
        // Use the full 4-parameter constructor for better compatibility with iOS SDK
        var sessionReplayConfig = new DDSessionReplayConfiguration(
            replaySampleRate: config.SampleRate,
            textAndInputPrivacyLevel: MapTextAndInputPrivacy(config.TextAndInputPrivacy),
            imagePrivacyLevel: MapImagePrivacy(config.ImagePrivacy),
            touchPrivacyLevel: MapTouchPrivacy(config.TouchPrivacy)
        );

        DDSessionReplay.EnableWith(sessionReplayConfig);
    }

    private static DDTextAndInputPrivacyLevel MapTextAndInputPrivacy(TextAndInputPrivacy privacy)
    {
        return privacy switch
        {
            TextAndInputPrivacy.MaskAll => DDTextAndInputPrivacyLevel.All,
            TextAndInputPrivacy.MaskAllInputs => DDTextAndInputPrivacyLevel.AllInputs,
            TextAndInputPrivacy.MaskSensitiveInputs => DDTextAndInputPrivacyLevel.SensitiveInputs,
            _ => DDTextAndInputPrivacyLevel.SensitiveInputs
        };
    }

    private static DDImagePrivacyLevel MapImagePrivacy(ImagePrivacy privacy)
    {
        return privacy switch
        {
            ImagePrivacy.MaskAll => DDImagePrivacyLevel.All,
            ImagePrivacy.MaskNonBundledOnly => DDImagePrivacyLevel.NonBundledOnly,
            ImagePrivacy.MaskNone => DDImagePrivacyLevel.None,
            _ => DDImagePrivacyLevel.NonBundledOnly
        };
    }

    private static DDTouchPrivacyLevel MapTouchPrivacy(TouchPrivacy privacy)
    {
        return privacy switch
        {
            TouchPrivacy.Show => DDTouchPrivacyLevel.Show,
            TouchPrivacy.Hide => DDTouchPrivacyLevel.Hide,
            _ => DDTouchPrivacyLevel.Show
        };
    }
}
