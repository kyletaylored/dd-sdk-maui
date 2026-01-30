namespace DatadogMauiSample.Models;

/// <summary>
/// Represents a user in the application.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the avatar URL.
    /// </summary>
    public string AvatarUrl { get; set; } = "https://api.dicebear.com/7.x/avataaars/svg?seed=";

    /// <summary>
    /// Gets a default guest user instance.
    /// </summary>
    public static User Guest => new()
    {
        Id = "guest",
        Name = "Guest",
        Email = "guest@example.com"
    };
}
