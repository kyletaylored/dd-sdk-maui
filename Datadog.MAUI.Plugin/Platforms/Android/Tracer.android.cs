using Com.Datadog.Android.Trace;
using Com.Datadog.Android.Tracing;

namespace Datadog.Maui.Tracing;

public static partial class Tracer
{
    private static AndroidX.Tracing.Tracer? _nativeTracer;

    private static AndroidX.Tracing.Tracer NativeTracer
    {
        get
        {
            if (_nativeTracer == null)
            {
                _nativeTracer = AndroidX.Tracing.GlobalTracer.Get();
            }
            return _nativeTracer;
        }
    }

    static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime)
    {
        var spanBuilder = NativeTracer.BuildSpan(operationName);

        if (parent is Platforms.Android.AndroidSpan androidParent)
        {
            spanBuilder.AsChildOf(androidParent.NativeSpan.Context);
        }

        if (startTime.HasValue)
        {
            var microseconds = startTime.Value.ToUnixTimeMilliseconds() * 1000;
            spanBuilder.WithStartTimestamp(microseconds);
        }

        var nativeSpan = spanBuilder.Start();
        return new Platforms.Android.AndroidSpan(nativeSpan);
    }

    static partial ISpan? PlatformGetActiveSpan()
    {
        var activeSpan = NativeTracer.ActiveSpan();
        return activeSpan != null ? new Platforms.Android.AndroidSpan(activeSpan) : null;
    }

    static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span)
    {
        if (span is not Platforms.Android.AndroidSpan androidSpan)
            return;

        var textMapAdapter = new TextMapAdapter(headers);
        NativeTracer.Inject(
            androidSpan.NativeSpan.Context,
            AndroidX.Tracing.Format.Builtin.HttpHeaders,
            textMapAdapter
        );
    }

    static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        var textMapAdapter = new TextMapAdapter(headers);
        var spanContext = NativeTracer.Extract(
            AndroidX.Tracing.Format.Builtin.HttpHeaders,
            textMapAdapter
        );

        if (spanContext == null)
            return null;

        var span = NativeTracer.BuildSpan("extracted")
            .AsChildOf(spanContext)
            .Start();

        return new Platforms.Android.AndroidSpan(span);
    }
}

internal class TextMapAdapter : Java.Lang.Object, AndroidX.Tracing.Propagation.ITextMap
{
    private readonly IDictionary<string, string> _headers;

    public TextMapAdapter(IDictionary<string, string> headers)
    {
        _headers = headers;
    }

    public void Put(string key, string value)
    {
        _headers[key] = value;
    }

    public Java.Util.IIterator Iterator()
    {
        var entries = _headers.Select(kvp =>
            new Java.Util.AbstractMap.SimpleEntry(kvp.Key, kvp.Value)
        ).ToList();
        return entries.Iterator();
    }
}
