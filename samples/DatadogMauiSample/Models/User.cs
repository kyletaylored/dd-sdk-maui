namespace DatadogMauiSample.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = "https://api.dicebear.com/7.x/avataaars/svg?seed=";

    public static User Guest => new()
    {
        Id = "guest",
        Name = "Guest",
        Email = "guest@example.com"
    };
}
