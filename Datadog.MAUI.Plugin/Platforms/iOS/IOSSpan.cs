using Datadog.iOS.DatadogTrace;
using Datadog.iOS.DatadogInternal;
using Foundation;

namespace Datadog.Maui.Platforms.iOS;

internal class IOSSpan : Tracing.ISpan
{
    private readonly OTSpan _nativeSpan;

    public IOSSpan(OTSpan nativeSpan)
    {
        _nativeSpan = nativeSpan;
    }

    // Expose native span for tracer to use
    internal OTSpan NativeSpan => _nativeSpan;

    // Note: iOS OpenTelemetry API doesn't expose SpanId/TraceId directly
    public string SpanId => string.Empty;

    public string TraceId => string.Empty;

    public void SetTag(string key, string value)
    {
        _nativeSpan.SetTag(key: key, value: value);
    }

    public void SetTag(string key, object value)
    {
        // Convert object to appropriate type for iOS API
        switch (value)
        {
            case string strValue:
                _nativeSpan.SetTag(key: key, value: strValue);
                break;
            case bool boolValue:
                _nativeSpan.SetTag(key: key, boolValue: boolValue);
                break;
            case int intValue:
                _nativeSpan.SetTag(key: key, numberValue: new NSNumber(intValue));
                break;
            case long longValue:
                _nativeSpan.SetTag(key: key, numberValue: new NSNumber(longValue));
                break;
            case float floatValue:
                _nativeSpan.SetTag(key: key, numberValue: new NSNumber(floatValue));
                break;
            case double doubleValue:
                _nativeSpan.SetTag(key: key, numberValue: new NSNumber(doubleValue));
                break;
            default:
                _nativeSpan.SetTag(key: key, value: value.ToString() ?? string.Empty);
                break;
        }
    }

    public void SetError(Exception exception)
    {
        var nsError = NSError.FromDomain(
            new NSString("Exception"),
            0,
            NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(exception.ToString()),
                NSError.LocalizedDescriptionKey
            )
        );
        _nativeSpan.SetError(nsError);
    }

    public void SetError(string message)
    {
        _nativeSpan.SetTag(key: "error", boolValue: true);
        _nativeSpan.SetTag(key: "error.message", value: message);
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

    public void Finish()
    {
        _nativeSpan.Finish();
    }

    public void Dispose()
    {
        Finish();
    }

    private static NSDictionary<NSString, NSObject> ConvertAttributes(Dictionary<string, object> attributes)
    {
        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();
        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
