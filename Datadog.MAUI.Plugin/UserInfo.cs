namespace Datadog.Maui;

/// <summary>
/// User information for tracking.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Unique user identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// User's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Additional user attributes.
    /// </summary>
    public Dictionary<string, object>? ExtraInfo { get; set; }
}
