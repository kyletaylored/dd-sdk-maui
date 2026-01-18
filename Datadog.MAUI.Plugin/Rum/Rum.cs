namespace Datadog.Maui.Rum;

/// <summary>
/// Static API for Real User Monitoring (RUM).
/// </summary>
public static partial class Rum
{
    /// <summary>
    /// Starts tracking a view.
    /// </summary>
    /// <param name="key">Unique view identifier.</param>
    /// <param name="name">View name (optional, defaults to key).</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void StartView(string key, string? name = null, Dictionary<string, object>? attributes = null)
    {
        PlatformStartView(key, name ?? key, attributes);
    }

    /// <summary>
    /// Stops tracking the current view.
    /// </summary>
    /// <param name="key">View identifier.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void StopView(string key, Dictionary<string, object>? attributes = null)
    {
        PlatformStopView(key, attributes);
    }

    /// <summary>
    /// Adds a user action.
    /// </summary>
    /// <param name="type">Action type.</param>
    /// <param name="name">Action name.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void AddAction(RumActionType type, string name, Dictionary<string, object>? attributes = null)
    {
        PlatformAddAction(type, name, attributes);
    }

    /// <summary>
    /// Starts tracking a resource.
    /// </summary>
    /// <param name="key">Unique resource identifier.</param>
    /// <param name="method">HTTP method (GET, POST, etc).</param>
    /// <param name="url">Resource URL.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void StartResource(string key, string method, string url, Dictionary<string, object>? attributes = null)
    {
        PlatformStartResource(key, method, url, attributes);
    }

    /// <summary>
    /// Stops tracking a resource.
    /// </summary>
    /// <param name="key">Resource identifier.</param>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="size">Response size in bytes.</param>
    /// <param name="kind">Resource type.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void StopResource(string key, int? statusCode = null, long? size = null, RumResourceKind kind = RumResourceKind.Native, Dictionary<string, object>? attributes = null)
    {
        PlatformStopResource(key, statusCode, size, kind, attributes);
    }

    /// <summary>
    /// Stops tracking a resource with an error.
    /// </summary>
    /// <param name="key">Resource identifier.</param>
    /// <param name="error">Error that occurred.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void StopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes = null)
    {
        PlatformStopResourceWithError(key, error, attributes);
    }

    /// <summary>
    /// Adds an error.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="source">Error source.</param>
    /// <param name="exception">Exception object.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void AddError(string message, RumErrorSource source = RumErrorSource.Source, Exception? exception = null, Dictionary<string, object>? attributes = null)
    {
        PlatformAddError(message, source, exception, attributes);
    }

    /// <summary>
    /// Adds an error from an exception.
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="source">Error source.</param>
    /// <param name="attributes">Custom attributes.</param>
    public static void AddError(Exception exception, RumErrorSource source = RumErrorSource.Source, Dictionary<string, object>? attributes = null)
    {
        PlatformAddError(exception.Message, source, exception, attributes);
    }

    /// <summary>
    /// Adds a custom timing.
    /// </summary>
    /// <param name="name">Timing name.</param>
    public static void AddTiming(string name)
    {
        PlatformAddTiming(name);
    }

    /// <summary>
    /// Adds a global attribute to all RUM events.
    /// </summary>
    public static void AddAttribute(string key, object value)
    {
        PlatformAddAttribute(key, value);
    }

    /// <summary>
    /// Removes a global attribute.
    /// </summary>
    public static void RemoveAttribute(string key)
    {
        PlatformRemoveAttribute(key);
    }

    /// <summary>
    /// Starts a new RUM session.
    /// </summary>
    public static void StartSession()
    {
        PlatformStartSession();
    }

    /// <summary>
    /// Stops the current RUM session.
    /// </summary>
    public static void StopSession()
    {
        PlatformStopSession();
    }

    // Platform-specific partial methods
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes);
    static partial void PlatformStopView(string key, Dictionary<string, object>? attributes);
    static partial void PlatformAddAction(RumActionType type, string name, Dictionary<string, object>? attributes);
    static partial void PlatformStartResource(string key, string method, string url, Dictionary<string, object>? attributes);
    static partial void PlatformStopResource(string key, int? statusCode, long? size, RumResourceKind kind, Dictionary<string, object>? attributes);
    static partial void PlatformStopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes);
    static partial void PlatformAddError(string message, RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes);
    static partial void PlatformAddTiming(string name);
    static partial void PlatformAddAttribute(string key, object value);
    static partial void PlatformRemoveAttribute(string key);
    static partial void PlatformStartSession();
    static partial void PlatformStopSession();
}
