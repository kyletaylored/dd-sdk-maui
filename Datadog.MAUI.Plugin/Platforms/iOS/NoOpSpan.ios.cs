namespace Datadog.Maui.Platforms.iOS;

/// <summary>
/// No-op implementation of ISpan for when Datadog tracing is not initialized on iOS.
/// </summary>
internal class NoOpSpan : Tracing.ISpan
{
    public string SpanId => "0";
    public string TraceId => "0";

    public void SetTag(string key, string value) { }
    public void SetTag(string key, object value) { }
    public void SetError(Exception exception) { }
    public void SetError(string message) { }
    public void AddEvent(string name, Dictionary<string, object>? attributes = null) { }
    public void Finish() { }
    public void Dispose() { }
}
