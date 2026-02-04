using Datadog.iOS.Trace;
using Foundation;

namespace Datadog.Maui.Tracing;

public static partial class Tracer
{
    private static DDTracer? _nativeTracer;
    private static bool _tracerInitializationFailed = false;

    // Internal flag set by Datadog.ios.cs to indicate if tracing was enabled
    internal static bool IsTracingEnabled { get; set; } = false;

    private static DDTracer? NativeTracer
    {
        get
        {
            if (!IsTracingEnabled || _tracerInitializationFailed)
            {
                return null;
            }

            if (_nativeTracer == null)
            {
                try
                {
                    _nativeTracer = DDTracer.Shared;
                }
                catch (Exception)
                {
                    _tracerInitializationFailed = true;
                    return null;
                }
            }
            return _nativeTracer;
        }
    }

    private static partial ISpan PlatformStartSpan(string operationName, ISpan? parent, DateTimeOffset? startTime)
    {
        // Return no-op span if tracer initialization failed
        if (NativeTracer == null)
        {
            return new Platforms.iOS.NoOpSpan();
        }

        try
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
        catch (Exception)
        {
            return new Platforms.iOS.NoOpSpan();
        }
    }

    private static partial ISpan? PlatformGetActiveSpan()
    {
        // iOS SDK doesn't expose an ActiveSpan property on DDTracer
        return null;
    }

    private static partial void PlatformInject(IDictionary<string, string> headers, ISpan? span)
    {
        if (NativeTracer == null)
            return;

        if (span is not Platforms.iOS.IOSSpan iosSpan)
            return;

        try
        {
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
        catch (Exception)
        {
            // Silently fail - distributed tracing will not work for this request
        }
    }

    private static partial ISpan? PlatformExtract(IDictionary<string, string> headers)
    {
        if (NativeTracer == null)
            return null;

        try
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
        catch (Exception)
        {
            return null;
        }
    }
}
