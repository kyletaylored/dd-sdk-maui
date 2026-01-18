namespace Datadog.Maui.Logs;

/// <summary>
/// Static API for log management.
/// </summary>
public static partial class Logs
{
    private static readonly Dictionary<string, ILogger> _loggers = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Creates or retrieves a logger with the specified name.
    /// </summary>
    /// <param name="name">Logger name.</param>
    /// <returns>Logger instance.</returns>
    public static ILogger CreateLogger(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Logger name cannot be null or empty", nameof(name));

        lock (_lock)
        {
            if (_loggers.TryGetValue(name, out var existingLogger))
                return existingLogger;

            var logger = PlatformCreateLogger(name);
            _loggers[name] = logger;
            return logger;
        }
    }

    /// <summary>
    /// Adds a global attribute to all logs.
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
    /// Adds a global tag to all logs.
    /// </summary>
    public static void AddTag(string key, string value)
    {
        PlatformAddTag(key, value);
    }

    /// <summary>
    /// Removes a global tag.
    /// </summary>
    public static void RemoveTag(string key)
    {
        PlatformRemoveTag(key);
    }

    // Platform-specific partial methods
    static partial ILogger PlatformCreateLogger(string name);
    static partial void PlatformAddAttribute(string key, object value);
    static partial void PlatformRemoveAttribute(string key);
    static partial void PlatformAddTag(string key, string value);
    static partial void PlatformRemoveTag(string key);
}
