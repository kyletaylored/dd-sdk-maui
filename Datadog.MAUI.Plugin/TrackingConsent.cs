namespace Datadog.Maui;

/// <summary>
/// User tracking consent status.
/// </summary>
public enum TrackingConsent
{
    /// <summary>
    /// User has granted consent - tracking starts immediately.
    /// </summary>
    Granted,

    /// <summary>
    /// User has not granted consent - no tracking occurs.
    /// </summary>
    NotGranted,

    /// <summary>
    /// Consent is pending - events are stored locally until consent is granted or denied.
    /// </summary>
    Pending
}
