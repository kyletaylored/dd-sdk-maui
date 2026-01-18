using DatadogTrace;
using Foundation;

namespace Datadog.Maui.Platforms.iOS;

internal class IOSSpan : Tracing.ISpan
{
    private readonly OTSpan _nativeSpan;

    public IOSSpan(OTSpan nativeSpan)
    {
        _nativeSpan = nativeSpan;
    }

    public string SpanId => _nativeSpan.Context.SpanId;

    public string TraceId => _nativeSpan.Context.TraceId;

    public void SetTag(string key, string value)
    {
        _nativeSpan.SetTag(key: key, value: value);
    }

    public void SetError(Exception exception)
    {
        _nativeSpan.SetError(new DatadogInternal.DDError(exception));
    }

    public void AddEvent(string name, Dictionary<string, object>? attributes = null)
    {
        if (attributes != null && attributes.Count > 0)
        {
            var nsAttributes = ConvertAttributes(attributes);
            _nativeSpan.Log(fields: nsAttributes);
        }
        else
        {
            _nativeSpan.Log(fields: new NSDictionary<NSString, NSObject>(
                new NSString("event"),
                new NSString(name)
            ));
        }
    }

    public void Finish(DateTimeOffset? finishTime = null)
    {
        if (finishTime.HasValue)
        {
            _nativeSpan.Finish(at: finishTime.Value.DateTime);
        }
        else
        {
            _nativeSpan.Finish();
        }
    }

    private static NSDictionary<NSString, NSObject> ConvertAttributes(Dictionary<string, object> attributes)
    {
        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();
        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
