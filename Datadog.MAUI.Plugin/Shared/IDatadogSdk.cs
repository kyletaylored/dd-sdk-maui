namespace Datadog.MAUI;

/// <summary>
/// Main interface for interacting with the Datadog SDK across platforms.
/// </summary>
public interface IDatadogSdk
{
    /// <summary>
    /// Initializes the Datadog SDK with the provided configuration.
    /// Must be called before using any other Datadog features.
    /// </summary>
    /// <param name="configuration">The SDK configuration</param>
    void Initialize(DatadogConfiguration configuration);

    /// <summary>
    /// Sets the user information for tracking.
    /// </summary>
    /// <param name="id">Unique user identifier</param>
    /// <param name="name">User name (optional)</param>
    /// <param name="email">User email (optional)</param>
    void SetUser(string id, string? name = null, string? email = null);

    /// <summary>
    /// Clears the current user information.
    /// </summary>
    void ClearUser();

    /// <summary>
    /// Adds a custom attribute to all future events.
    /// </summary>
    /// <param name="key">Attribute key</param>
    /// <param name="value">Attribute value</param>
    void AddAttribute(string key, object value);

    /// <summary>
    /// Removes a custom attribute.
    /// </summary>
    /// <param name="key">Attribute key to remove</param>
    void RemoveAttribute(string key);
}

/// <summary>
/// Logging interface for the Datadog SDK.
/// </summary>
public interface IDatadogLogger
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void Debug(string message, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs an info message.
    /// </summary>
    void Info(string message, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void Warn(string message, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void Error(string message, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    void Critical(string message, Dictionary<string, object>? attributes = null);
}

/// <summary>
/// RUM (Real User Monitoring) interface for the Datadog SDK.
/// </summary>
public interface IDatadogRum
{
    /// <summary>
    /// Starts a view with the given key and name.
    /// </summary>
    void StartView(string key, string name, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Stops the current view.
    /// </summary>
    void StopView(string key, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Adds a custom action.
    /// </summary>
    void AddAction(string type, string name, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Adds a custom error.
    /// </summary>
    void AddError(string message, string source, Exception? exception = null, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Starts a resource loading.
    /// </summary>
    void StartResource(string key, string url, string method);

    /// <summary>
    /// Stops a resource loading with success.
    /// </summary>
    void StopResource(string key, int statusCode, long size, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Stops a resource loading with error.
    /// </summary>
    void StopResourceWithError(string key, string message, string source);
}

/// <summary>
/// Tracing interface for the Datadog SDK.
/// </summary>
public interface IDatadogTrace
{
    /// <summary>
    /// Starts a span with the given operation name.
    /// </summary>
    IDatadogSpan StartSpan(string operationName, Dictionary<string, object>? tags = null);
}

/// <summary>
/// Represents a distributed tracing span.
/// </summary>
public interface IDatadogSpan : IDisposable
{
    /// <summary>
    /// Sets a tag on the span.
    /// </summary>
    void SetTag(string key, string value);

    /// <summary>
    /// Sets an error on the span.
    /// </summary>
    void SetError(Exception exception);

    /// <summary>
    /// Finishes the span.
    /// </summary>
    void Finish();
}
