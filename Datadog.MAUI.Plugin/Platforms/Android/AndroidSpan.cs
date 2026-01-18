using Datadog.Maui.Tracing;
using Com.Datadog.Android.Trace;

namespace Datadog.Maui.Platforms.Android;

internal class AndroidSpan : ISpan
{
    private readonly AndroidX.Tracing.Span _nativeSpan;

    public AndroidSpan(AndroidX.Tracing.Span nativeSpan)
    {
        _nativeSpan = nativeSpan;
    }

    public string SpanId => _nativeSpan.Context.SpanId;

    public string TraceId => _nativeSpan.Context.TraceId;

    public void SetTag(string key, string value)
    {
        _nativeSpan.SetTag(key, value);
    }

    public void SetError(Exception exception)
    {
        _nativeSpan.SetError(true);
        _nativeSpan.SetTag("error.type", exception.GetType().Name);
        _nativeSpan.SetTag("error.message", exception.Message);
        _nativeSpan.SetTag("error.stack", exception.StackTrace ?? string.Empty);
    }

    public void AddEvent(string name, Dictionary<string, object>? attributes = null)
    {
        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(
                kvp => kvp.Key,
                kvp => (Java.Lang.Object)kvp.Value
            );
            _nativeSpan.Log(javaAttributes);
        }
        else
        {
            _nativeSpan.Log(new Dictionary<string, Java.Lang.Object>
            {
                { "event", new Java.Lang.String(name) }
            });
        }
    }

    public void Finish(DateTimeOffset? finishTime = null)
    {
        if (finishTime.HasValue)
        {
            var microseconds = finishTime.Value.ToUnixTimeMilliseconds() * 1000;
            _nativeSpan.Finish(microseconds);
        }
        else
        {
            _nativeSpan.Finish();
        }
    }

    internal AndroidX.Tracing.Span NativeSpan => _nativeSpan;
}
