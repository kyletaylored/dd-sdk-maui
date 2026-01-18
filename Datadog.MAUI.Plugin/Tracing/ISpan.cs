namespace Datadog.Maui.Tracing;

/// <summary>
/// Represents a span in distributed tracing.
/// </summary>
public interface ISpan : IDisposable
{
    /// <summary>
    /// Gets the span ID.
    /// </summary>
    string SpanId { get; }

    /// <summary>
    /// Gets the trace ID.
    /// </summary>
    string TraceId { get; }

    /// <summary>
    /// Sets a tag on the span.
    /// </summary>
    void SetTag(string key, string value);

    /// <summary>
    /// Sets a tag on the span.
    /// </summary>
    void SetTag(string key, object value);

    /// <summary>
    /// Sets an error on the span.
    /// </summary>
    void SetError(Exception exception);

    /// <summary>
    /// Sets an error on the span.
    /// </summary>
    void SetError(string message);

    /// <summary>
    /// Adds an event to the span.
    /// </summary>
    void AddEvent(string name, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Finishes the span.
    /// </summary>
    void Finish();
}
