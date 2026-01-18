namespace Datadog.Maui.Logs;

/// <summary>
/// Logger interface for sending logs to Datadog.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Gets the logger name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void Debug(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs an info message.
    /// </summary>
    void Info(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a notice message.
    /// </summary>
    void Notice(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void Warn(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void Error(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    void Critical(string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a message at the specified level.
    /// </summary>
    void Log(LogLevel level, string message, Exception? error = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Adds a custom attribute to all logs from this logger.
    /// </summary>
    void AddAttribute(string key, object value);

    /// <summary>
    /// Removes a custom attribute.
    /// </summary>
    void RemoveAttribute(string key);

    /// <summary>
    /// Adds a tag to all logs from this logger.
    /// </summary>
    void AddTag(string key, string value);

    /// <summary>
    /// Removes a tag.
    /// </summary>
    void RemoveTag(string key);
}
