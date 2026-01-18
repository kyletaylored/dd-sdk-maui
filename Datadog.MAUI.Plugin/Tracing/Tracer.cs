namespace Datadog.Maui.Tracing;

/// <summary>
/// Static API for distributed tracing.
/// </summary>
public static partial class Tracer
{
    /// <summary>
    /// Starts a new span.
    /// </summary>
    /// <param name="operationName">Operation name.</param>
    /// <param name="parent">Parent span context (optional).</param>
    /// <param name="startTime">Start time (optional, defaults to now).</param>
    /// <returns>Span instance.</returns>
    public static ISpan StartSpan(string operationName, ISpan? parent = null, DateTimeOffset? startTime = null)
    {
        return PlatformStartSpan(operationName, parent, startTime);
    }

    /// <summary>
    /// Gets the active span.
    /// </summary>
    public static ISpan? ActiveSpan => PlatformGetActiveSpan();

    /// <summary>
    /// Injects trace context into HTTP headers.
    /// </summary>
    /// <param name="headers">Headers dictionary to inject into.</param>
    /// <param name="span">Span to inject (optional, uses active span if null).</param>
    public static void Inject(IDictionary<string, string> headers, ISpan? span = null)
    {
        PlatformInject(headers, span ?? ActiveSpan);
    }

    /// <summary>
    /// Injects trace context into HTTP headers.
    /// </summary>
    /// <param name="headers">HTTP headers collection.</param>
    /// <param name="span">Span to inject (optional, uses active span if null).</param>
    public static void Inject(System.Net.Http.Headers.HttpRequestHeaders headers, ISpan? span = null)
    {
        var dict = new Dictionary<string, string>();
        PlatformInject(dict, span ?? ActiveSpan);

        foreach (var kvp in dict)
        {
            headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Extracts trace context from headers.
    /// </summary>
    /// <param name="headers">Headers to extract from.</param>
    /// <returns>Extracted span context, or null if not found.</returns>
    public static ISpan? Extract(IDictionary<string, string> headers)
    {
        return PlatformExtract(headers);
    }

    // Platform-specific partial methods
    static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime);
    static partial ISpan? PlatformGetActiveSpan();
    static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span);
    static partial ISpan? PlatformExtract(IDictionary<string, string> headers);
}
