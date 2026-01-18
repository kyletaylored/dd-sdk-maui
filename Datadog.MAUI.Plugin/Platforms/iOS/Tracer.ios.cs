using DatadogTrace;
using Foundation;

namespace Datadog.Maui.Tracing;

public static partial class Tracer
{
    private static OTTracer? _nativeTracer;

    private static OTTracer NativeTracer
    {
        get
        {
            if (_nativeTracer == null)
            {
                _nativeTracer = DatadogTrace.Tracer.Shared();
            }
            return _nativeTracer;
        }
    }

    static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime)
    {
        var spanBuilder = NativeTracer.BuildSpan(operationName: operationName);

        if (parent is Platforms.iOS.IOSSpan iosParent)
        {
            spanBuilder = spanBuilder.AsChildOf(iosParent);
        }

        if (startTime.HasValue)
        {
            spanBuilder = spanBuilder.WithStartTime(startTime.Value.DateTime);
        }

        var nativeSpan = spanBuilder.Start();
        return new Platforms.iOS.IOSSpan(nativeSpan);
    }

    static partial ISpan? PlatformGetActiveSpan()
    {
        var activeSpan = NativeTracer.ActiveSpan;
        return activeSpan != null ? new Platforms.iOS.IOSSpan(activeSpan) : null;
    }

    static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span)
    {
        if (span is not Platforms.iOS.IOSSpan iosSpan)
            return;

        var carrier = new NSMutableDictionary<NSString, NSString>();
        NativeTracer.Inject(
            spanContext: iosSpan.Context,
            format: BuiltinFormats.HttpHeaders,
            carrier: carrier
        );

        foreach (var key in carrier.Keys)
        {
            var value = carrier[key];
            if (key != null && value != null)
            {
                headers[key.ToString()] = value.ToString();
            }
        }
    }

    static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        var carrier = new NSMutableDictionary<NSString, NSString>();
        foreach (var kvp in headers)
        {
            carrier[new NSString(kvp.Key)] = new NSString(kvp.Value);
        }

        var spanContext = NativeTracer.Extract(
            format: BuiltinFormats.HttpHeaders,
            carrier: carrier
        );

        if (spanContext == null)
            return null;

        var span = NativeTracer.BuildSpan(operationName: "extracted")
            .AsChildOf(spanContext)
            .Start();

        return new Platforms.iOS.IOSSpan(span);
    }
}
