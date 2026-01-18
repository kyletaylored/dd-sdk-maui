namespace Datadog.Maui.Rum;

/// <summary>
/// Type of RUM user action.
/// </summary>
public enum RumActionType
{
    /// <summary>
    /// Tap action.
    /// </summary>
    Tap,

    /// <summary>
    /// Scroll action.
    /// </summary>
    Scroll,

    /// <summary>
    /// Swipe action.
    /// </summary>
    Swipe,

    /// <summary>
    /// Click action.
    /// </summary>
    Click,

    /// <summary>
    /// Custom action.
    /// </summary>
    Custom
}
