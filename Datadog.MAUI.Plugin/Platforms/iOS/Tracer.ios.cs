using Datadog.iOS.Trace;
using Foundation;

namespace Datadog.Maui.Tracing;

public static partial class Tracer
{
    private static DDTracer? _nativeTracer;

    private static DDTracer NativeTracer
    {
        get
        {
            if (_nativeTracer == null)
            {
                _nativeTracer = DDTracer.Shared;
            }
            return _nativeTracer ?? throw new InvalidOperationException("Failed to initialize Datadog tracer");
        }
    }

    private static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime)
    {
        OTSpan nativeSpan;

        if (parent is Platforms.iOS.IOSSpan iosParent)
        {
            var parentContext = iosParent.NativeSpan.Context;
            var startDate = startTime?.UtcDateTime;

            nativeSpan = NativeTracer.StartSpan(
                operationName: operationName,
                parent: parentContext,
                tags: null,
                startTime: startDate != null ? (NSDate)startDate.Value : null
            );
        }
        else if (startTime.HasValue)
        {
            nativeSpan = NativeTracer.StartRootSpan(
                operationName: operationName,
                tags: null,
                startTime: (NSDate)startTime.Value.UtcDateTime,
                customSampleRate: null
            );
        }
        else
        {
            nativeSpan = NativeTracer.StartSpan(operationName: operationName);
        }

        return new Platforms.iOS.IOSSpan(nativeSpan);
    }

    private static partial ISpan? PlatformGetActiveSpan()
    {
        // iOS SDK doesn't expose an ActiveSpan property on DDTracer
        return null;
    }

    private static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span)
    {
        if (span is not Platforms.iOS.IOSSpan iosSpan)
            return;

        var carrier = new NSMutableDictionary<NSString, NSString>();
        var error = (NSError?)null;

        // Use "http_headers" format which is the standard OpenTracing format
        NativeTracer.Inject(
            spanContext: iosSpan.NativeSpan.Context,
            format: "http_headers",
            carrier: carrier,
            error: out error
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

    private static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        var carrier = new NSMutableDictionary<NSString, NSString>();
        foreach (var kvp in headers)
        {
            carrier[new NSString(kvp.Key)] = new NSString(kvp.Value);
        }

        var error = (NSError?)null;
        var success = NativeTracer.ExtractWithFormat(
            format: "http_headers",
            carrier: carrier,
            error: out error
        );

        if (!success || error != null)
            return null;

        // The extracted context is not exposed, so we can't create a span from it
        // This is a limitation of the iOS OpenTelemetry API
        return null;
    }
}
