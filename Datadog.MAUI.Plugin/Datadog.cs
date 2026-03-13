using Datadog.Maui.Configuration;

namespace Datadog.Maui;

/// <summary>
/// Main entry point for Datadog SDK.
/// </summary>
public static partial class Datadog
{
    private static DatadogConfiguration? _configuration;
    private static bool _isInitialized;

    /// <summary>
    /// Gets whether Datadog SDK is initialized.
    /// </summary>
    public static bool IsInitialized => _isInitialized;

    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    public static DatadogConfiguration? Configuration => _configuration;

    /// <summary>
    /// Initializes the Datadog SDK.
    /// </summary>
    /// <param name="configuration">SDK configuration.</param>
    public static void Initialize(DatadogConfiguration configuration)
    {
        if (_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK already initialized");
            return;
        }

        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Call platform-specific initialization
        PlatformInitialize(configuration);

        _isInitialized = true;
        Console.WriteLine($"[Datadog] SDK initialized - Environment: {configuration.Environment}, Service: {configuration.ServiceName}");
    }

    /// <summary>
    /// Sets user information for tracking.
    /// </summary>
    /// <param name="userInfo">User information.</param>
    public static void SetUser(UserInfo userInfo)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        PlatformSetUser(userInfo);
    }

    /// <summary>
    /// Sets global tags for all events.
    /// </summary>
    /// <param name="tags">Tags to set.</param>
    public static void SetTags(Dictionary<string, string> tags)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        PlatformSetTags(tags);
    }

    /// <summary>
    /// Updates tracking consent.
    /// </summary>
    /// <param name="consent">New consent status.</param>
    public static void SetTrackingConsent(TrackingConsent consent)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        PlatformSetTrackingConsent(consent);
    }

    /// <summary>
    /// Clears all user information.
    /// </summary>
    public static void ClearUser()
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        PlatformClearUser();
    }

    /// <summary>
    /// Adds a custom global attribute to all RUM events.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="value">Attribute value (string, int, double, bool, etc.).</param>
    public static void AddAttribute(string key, object value)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        ArgumentNullException.ThrowIfNull(key);
        PlatformAddAttribute(key, value);
    }

    /// <summary>
    /// Sets (adds or replaces) a custom global attribute on all RUM events.
    /// Functionally equivalent to AddAttribute — overwrites any existing value for the key.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="value">Attribute value.</param>
    public static void SetAttribute(string key, object value)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        ArgumentNullException.ThrowIfNull(key);
        PlatformAddAttribute(key, value);
    }

    /// <summary>
    /// Removes a custom global attribute from all RUM events.
    /// </summary>
    /// <param name="key">Attribute key to remove.</param>
    public static void RemoveAttribute(string key)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("[Datadog] SDK not initialized");
            return;
        }

        ArgumentNullException.ThrowIfNull(key);
        PlatformRemoveAttribute(key);
    }

    // Platform-specific partial methods
    static partial void PlatformInitialize(DatadogConfiguration configuration);
    static partial void PlatformSetUser(UserInfo userInfo);
    static partial void PlatformSetTags(Dictionary<string, string> tags);
    static partial void PlatformSetTrackingConsent(TrackingConsent consent);
    static partial void PlatformClearUser();
    static partial void PlatformAddAttribute(string key, object value);
    static partial void PlatformRemoveAttribute(string key);
}
