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
        // TODO: Implement trace context injection
        // Currently disabled due to OpenTracing TextMap binding conflicts
        // The TextMapInjectAdapter class causes Java compilation errors with Iterable interface inheritance
        throw new NotImplementedException(
            "Trace context injection is not yet implemented on Android. " +
            "This feature requires refactoring the OpenTracing TextMap adapters to avoid Iterable conflicts.");
    }

    private static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        // TODO: Implement trace context extraction
        // Currently disabled due to OpenTracing TextMap binding conflicts
        // The TextMapExtractAdapter class causes Java compilation errors with Iterable interface inheritance
        throw new NotImplementedException(
            "Trace context extraction is not yet implemented on Android. " +
            "This feature requires refactoring the OpenTracing TextMap adapters to avoid Iterable conflicts.");
    }
}
