using Datadog.Maui.Tracing;

namespace Datadog.Maui.Platforms.Android;

internal class AndroidSpan : ISpan
{
    private readonly Java.Lang.Object _nativeSpan;

    public AndroidSpan(Java.Lang.Object nativeSpan)
    {
        _nativeSpan = nativeSpan;
    }

    public string SpanId
    {
        get
        {
            var context = GetContext();
            if (context == null) return string.Empty;

            var contextClass = context.Class;
            var toSpanIdMethod = contextClass?.GetMethod("toSpanId");
            var spanId = toSpanIdMethod?.Invoke(context) as Java.Lang.String;
            return spanId?.ToString() ?? string.Empty;
        }
    }

    public string TraceId
    {
        get
        {
            var context = GetContext();
            if (context == null) return string.Empty;

            var contextClass = context.Class;
            var toTraceIdMethod = contextClass?.GetMethod("toTraceId");
            var traceId = toTraceIdMethod?.Invoke(context) as Java.Lang.String;
            return traceId?.ToString() ?? string.Empty;
        }
    }

    public void SetTag(string key, string value)
    {
        var spanClass = _nativeSpan.Class;
        var setTagMethod = spanClass?.GetMethod("setTag",
            Java.Lang.Class.FromType(typeof(Java.Lang.String)),
            Java.Lang.Class.FromType(typeof(Java.Lang.String)));
        setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), new Java.Lang.String(value));
    }

    public void SetTag(string key, object value)
    {
        var spanClass = _nativeSpan.Class;

        if (value is string s)
        {
            var setTagMethod = spanClass?.GetMethod("setTag",
                Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                Java.Lang.Class.FromType(typeof(Java.Lang.String)));
            setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), new Java.Lang.String(s));
        }
        else if (value is bool b)
        {
            var setTagMethod = spanClass?.GetMethod("setTag",
                Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                Java.Lang.Boolean.Type);
            setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), Java.Lang.Boolean.ValueOf(b));
        }
        else if (value is int i)
        {
            var setTagMethod = spanClass?.GetMethod("setTag",
                Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                Java.Lang.Integer.Type);
            setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), Java.Lang.Integer.ValueOf(i));
        }
        else if (value is double d)
        {
            var setTagMethod = spanClass?.GetMethod("setTag",
                Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                Java.Lang.Class.FromType(typeof(Java.Lang.String)));
            setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), new Java.Lang.String(d.ToString()));
        }
        else
        {
            var setTagMethod = spanClass?.GetMethod("setTag",
                Java.Lang.Class.FromType(typeof(Java.Lang.String)),
                Java.Lang.Class.FromType(typeof(Java.Lang.String)));
            setTagMethod?.Invoke(_nativeSpan, new Java.Lang.String(key), new Java.Lang.String(value?.ToString() ?? ""));
        }
    }

    public void SetError(Exception exception)
    {
        SetTag("error", true);
        SetTag("error.type", exception.GetType().Name);
        SetTag("error.message", exception.Message);
        SetTag("error.stack", exception.StackTrace ?? string.Empty);
    }

    public void SetError(string message)
    {
        SetTag("error", true);
        SetTag("error.message", message);
    }

    public void AddEvent(string name, Dictionary<string, object>? attributes = null)
    {
        var spanClass = _nativeSpan.Class;

        if (attributes != null && attributes.Count > 0)
        {
            var javaMap = new Java.Util.HashMap();
            javaMap.Put("event", name);
            foreach (var kvp in attributes)
            {
                javaMap.Put(kvp.Key, ConvertToJavaObject(kvp.Value));
            }

            var logMethod = spanClass?.GetMethod("log", Java.Lang.Class.FromType(typeof(Java.Util.IMap)));
            logMethod?.Invoke(_nativeSpan, javaMap);
        }
        else
        {
            var logMethod = spanClass?.GetMethod("log", Java.Lang.Class.FromType(typeof(Java.Lang.String)));
            logMethod?.Invoke(_nativeSpan, new Java.Lang.String(name));
        }
    }

    public void Finish()
    {
        var spanClass = _nativeSpan.Class;
        var finishMethod = spanClass?.GetMethod("finish");
        finishMethod?.Invoke(_nativeSpan);
    }

    public void Dispose()
    {
        Finish();
    }

    internal Java.Lang.Object? GetContext()
    {
        var spanClass = _nativeSpan.Class;
        var contextMethod = spanClass?.GetMethod("context");
        return contextMethod?.Invoke(_nativeSpan) as Java.Lang.Object;
    }

    private static Java.Lang.Object ConvertToJavaObject(object value)
    {
        return value switch
        {
            string s => new Java.Lang.String(s),
            int i => Java.Lang.Integer.ValueOf(i),
            long l => Java.Lang.Long.ValueOf(l),
            float f => Java.Lang.Float.ValueOf(f),
            double d => Java.Lang.Double.ValueOf(d),
            bool b => Java.Lang.Boolean.ValueOf(b),
            _ => new Java.Lang.String(value?.ToString() ?? "")
        };
    }
}
