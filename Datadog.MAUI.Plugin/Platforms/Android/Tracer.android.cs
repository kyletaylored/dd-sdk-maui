using IO.Opentracing.Propagation;

namespace Datadog.Maui.Tracing;

public static partial class Tracer
{
    private static Java.Lang.Object? _nativeTracer;

    private static Java.Lang.Object NativeTracer
    {
        get
        {
            if (_nativeTracer == null)
            {
                // Use reflection to call GlobalDatadogTracer.get()
                var globalTracerClass = Java.Lang.Class.ForName("com.datadog.android.trace.GlobalDatadogTracer");
                var getMethod = globalTracerClass?.GetMethod("get");
                _nativeTracer = getMethod?.Invoke(null) as Java.Lang.Object;

                if (_nativeTracer == null)
                {
                    throw new InvalidOperationException("Failed to get DatadogTracer from GlobalDatadogTracer.get()");
                }
            }
            return _nativeTracer;
        }
    }

    private static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime)
    {
        // Call tracer.buildSpan(operationName)
        var tracerClass = NativeTracer.Class;
        var buildSpanMethod = tracerClass?.GetMethod("buildSpan", Java.Lang.Class.FromType(typeof(Java.Lang.String)));
        var spanBuilder = buildSpanMethod?.Invoke(NativeTracer, new Java.Lang.String(operationName)) as Java.Lang.Object;

        if (spanBuilder == null)
        {
            throw new InvalidOperationException("Failed to create span builder");
        }

        // Set parent if provided
        if (parent is Platforms.Android.AndroidSpan androidParent)
        {
            var spanBuilderClass = spanBuilder.Class;
            var asChildOfMethod = spanBuilderClass?.GetMethod("asChildOf", Java.Lang.Class.FromType(typeof(Java.Lang.Object)));
            var parentContext = androidParent.GetContext();
            spanBuilder = asChildOfMethod?.Invoke(spanBuilder, parentContext) as Java.Lang.Object;
        }

        // Set start time if provided
        if (startTime.HasValue)
        {
            var microseconds = startTime.Value.ToUnixTimeMilliseconds() * 1000;
            var spanBuilderClass = spanBuilder.Class;
            var withStartTimestampMethod = spanBuilderClass?.GetMethod("withStartTimestamp", Java.Lang.Long.Type);
            spanBuilder = withStartTimestampMethod?.Invoke(spanBuilder, Java.Lang.Long.ValueOf(microseconds)) as Java.Lang.Object;
        }

        // Call start() to create the span
        var finalSpanBuilderClass = spanBuilder.Class;
        var startMethod = finalSpanBuilderClass?.GetMethod("start");
        var nativeSpan = startMethod?.Invoke(spanBuilder) as Java.Lang.Object;

        if (nativeSpan == null)
        {
            throw new InvalidOperationException("Failed to start span");
        }

        return new Platforms.Android.AndroidSpan(nativeSpan);
    }

    private static partial ISpan? PlatformGetActiveSpan()
    {
        // Call tracer.activeSpan()
        var tracerClass = NativeTracer.Class;
        var activeSpanMethod = tracerClass?.GetMethod("activeSpan");
        var activeSpan = activeSpanMethod?.Invoke(NativeTracer) as Java.Lang.Object;

        return activeSpan != null ? new Platforms.Android.AndroidSpan(activeSpan) : null;
    }

    private static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span)
    {
        if (span is not Platforms.Android.AndroidSpan androidSpan)
            return;

        var textMapInject = new TextMapInjectAdapter(headers);

        // Get BuiltinFormats.HTTP_HEADERS
        var builtinFormatsClass = Java.Lang.Class.ForName("io.opentracing.propagation.Format$Builtin");
        var httpHeadersField = builtinFormatsClass?.GetField("HTTP_HEADERS");
        var httpHeadersFormat = httpHeadersField?.Get(null) as Java.Lang.Object;

        // Call tracer.inject(spanContext, format, carrier)
        var tracerClass = NativeTracer.Class;
        var injectMethod = tracerClass?.GetMethod("inject",
            Java.Lang.Class.FromType(typeof(Java.Lang.Object)),
            Java.Lang.Class.FromType(typeof(Java.Lang.Object)),
            Java.Lang.Class.FromType(typeof(Java.Lang.Object)));

        var spanContext = androidSpan.GetContext();
        injectMethod?.Invoke(NativeTracer, spanContext, httpHeadersFormat, textMapInject);
    }

    private static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        var textMapExtract = new TextMapExtractAdapter(headers);

        // Get BuiltinFormats.HTTP_HEADERS
        var builtinFormatsClass = Java.Lang.Class.ForName("io.opentracing.propagation.Format$Builtin");
        var httpHeadersField = builtinFormatsClass?.GetField("HTTP_HEADERS");
        var httpHeadersFormat = httpHeadersField?.Get(null) as Java.Lang.Object;

        // Call tracer.extract(format, carrier)
        var tracerClass = NativeTracer.Class;
        var extractMethod = tracerClass?.GetMethod("extract",
            Java.Lang.Class.FromType(typeof(Java.Lang.Object)),
            Java.Lang.Class.FromType(typeof(Java.Lang.Object)));

        var spanContext = extractMethod?.Invoke(NativeTracer, httpHeadersFormat, textMapExtract) as Java.Lang.Object;

        if (spanContext == null)
            return null;

        // Build a new span with the extracted context as parent
        var buildSpanMethod = tracerClass?.GetMethod("buildSpan", Java.Lang.Class.FromType(typeof(Java.Lang.String)));
        var spanBuilder = buildSpanMethod?.Invoke(NativeTracer, new Java.Lang.String("extracted")) as Java.Lang.Object;

        if (spanBuilder == null)
            return null;

        var spanBuilderClass = spanBuilder.Class;
        var asChildOfMethod = spanBuilderClass?.GetMethod("asChildOf", Java.Lang.Class.FromType(typeof(Java.Lang.Object)));
        spanBuilder = asChildOfMethod?.Invoke(spanBuilder, spanContext) as Java.Lang.Object;

        var startMethod = spanBuilderClass?.GetMethod("start");
        var span = startMethod?.Invoke(spanBuilder) as Java.Lang.Object;

        return span != null ? new Platforms.Android.AndroidSpan(span) : null;
    }
}

// Adapter for injecting headers (write-only)
internal class TextMapInjectAdapter : Java.Lang.Object, ITextMap
{
    private readonly IDictionary<string, string> _headers;

    public TextMapInjectAdapter(IDictionary<string, string> headers)
    {
        _headers = headers;
    }

    public void Put(string? key, string? value)
    {
        if (key != null && value != null)
        {
            _headers[key] = value;
        }
    }

    public Java.Util.IIterator? Iterator()
    {
        // Not used for injection
        return null;
    }
}

// Adapter for extracting headers (read-only)
internal class TextMapExtractAdapter : Java.Lang.Object, ITextMapExtract
{
    private readonly IDictionary<string, string> _headers;

    public TextMapExtractAdapter(IDictionary<string, string> headers)
    {
        _headers = headers;
    }

    public Java.Util.IIterator? Iterator()
    {
        var entries = new Java.Util.ArrayList();
        foreach (var kvp in _headers)
        {
            entries.Add(new Java.Util.AbstractMap.SimpleEntry(kvp.Key, kvp.Value));
        }
        return entries.Iterator();
    }
}
