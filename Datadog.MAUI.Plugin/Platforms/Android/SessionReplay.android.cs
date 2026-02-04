namespace Datadog.Maui;

internal static class SessionReplayInitializer
{
    internal static void Initialize(Configuration.SessionReplayConfiguration config)
    {
        var sessionReplayConfig = new Android.SessionReplay.SessionReplayConfiguration.Builder(config.SampleRate)
            .SetTextAndInputPrivacy(MapTextAndInputPrivacy(config.TextAndInputPrivacy))
            .SetImagePrivacy(MapImagePrivacy(config.ImagePrivacy))
            .SetTouchPrivacy(MapTouchPrivacy(config.TouchPrivacy))
            .Build();

        Android.SessionReplay.SessionReplay.Enable(sessionReplayConfig, Android.Datadog.Instance);
    }

    private static Android.SessionReplay.TextAndInputPrivacy MapTextAndInputPrivacy(Configuration.TextAndInputPrivacy privacy)
    {
        return privacy switch
        {
            Configuration.TextAndInputPrivacy.MaskSensitiveInputs => Android.SessionReplay.TextAndInputPrivacy.MaskSensitiveInputs!,
            Configuration.TextAndInputPrivacy.MaskAllInputs => Android.SessionReplay.TextAndInputPrivacy.MaskAllInputs!,
            Configuration.TextAndInputPrivacy.MaskAll => Android.SessionReplay.TextAndInputPrivacy.MaskAll!,
            _ => Android.SessionReplay.TextAndInputPrivacy.MaskSensitiveInputs!
        };
    }

    private static Android.SessionReplay.ImagePrivacy MapImagePrivacy(Configuration.ImagePrivacy privacy)
    {
        return privacy switch
        {
            Configuration.ImagePrivacy.MaskNonBundledOnly => Android.SessionReplay.ImagePrivacy.MaskLargeOnly!,
            Configuration.ImagePrivacy.MaskAll => Android.SessionReplay.ImagePrivacy.MaskAll!,
            Configuration.ImagePrivacy.MaskNone => Android.SessionReplay.ImagePrivacy.MaskNone!,
            _ => Android.SessionReplay.ImagePrivacy.MaskLargeOnly!
        };
    }

    private static Android.SessionReplay.TouchPrivacy MapTouchPrivacy(Configuration.TouchPrivacy privacy)
    {
        return privacy switch
        {
            Configuration.TouchPrivacy.Show => Android.SessionReplay.TouchPrivacy.Show!,
            Configuration.TouchPrivacy.Hide => Android.SessionReplay.TouchPrivacy.Hide!,
            _ => Android.SessionReplay.TouchPrivacy.Show!
        };
    }
}
