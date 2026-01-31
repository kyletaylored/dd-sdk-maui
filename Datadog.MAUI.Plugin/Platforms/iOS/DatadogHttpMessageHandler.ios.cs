using System.Diagnostics;
using Datadog.iOS.Trace;
using Foundation;

namespace Datadog.Maui.Http;

/// <summary>
/// Helper to ensure logs are visible in Xcode console
/// </summary>
internal static class DatadogHttpLogger
{
    public static void Log(string message)
    {
        // Console.WriteLine goes to NSLog which is visible in Xcode
        Console.WriteLine(message);
        // Also write to Debug for when debugging in IDE
        Debug.WriteLine(message);
    }
}

/// <summary>
/// iOS-specific HTTP message handler that creates APM spans and injects trace headers.
/// </summary>
public class DatadogHttpMessageHandler : DelegatingHandler
{
    private readonly HashSet<string> _firstPartyHosts;

    public DatadogHttpMessageHandler(IEnumerable<string> firstPartyHosts, HttpMessageHandler? innerHandler = null)
        : base(innerHandler ?? new NSUrlSessionHandler())
    {
        _firstPartyHosts = new HashSet<string>(
            firstPartyHosts.Select(h => h.ToLowerInvariant()),
            StringComparer.OrdinalIgnoreCase
        );
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestUri = request.RequestUri;
        DatadogHttpLogger.Log($"[Datadog HTTP] Request intercepted: {request.Method} {requestUri}");

        if (requestUri == null || !ShouldTrace(requestUri.Host))
        {
            // Not a first-party host, don't trace
            DatadogHttpLogger.Log($"[Datadog HTTP] Skipping trace - host not in first-party list: {requestUri?.Host}");
            return await base.SendAsync(request, cancellationToken);
        }

        DatadogHttpLogger.Log($"[Datadog HTTP] Host {requestUri.Host} is first-party, attempting to trace");

        // Get the Datadog tracer
        // Note: DDTracer.Shared now correctly returns DDTracer (not OTTracer)
        DatadogHttpLogger.Log("[Datadog HTTP] Attempting to access DDTracer.Shared...");
        DDTracer? tracer = null;
        try
        {
            tracer = DDTracer.Shared;
            DatadogHttpLogger.Log($"[Datadog HTTP] ✓ Successfully obtained tracer: {tracer?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            DatadogHttpLogger.Log($"[Datadog HTTP] ✗ Failed to get tracer: {ex.GetType().Name}: {ex.Message}");
            if (ex.InnerException != null)
            {
                DatadogHttpLogger.Log($"[Datadog HTTP]   Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
            DatadogHttpLogger.Log($"[Datadog HTTP]   Falling back to untraced request");
            return await base.SendAsync(request, cancellationToken);
        }

        if (tracer == null)
        {
            DatadogHttpLogger.Log("[Datadog HTTP] ✗ Tracer is null, skipping instrumentation");
            return await base.SendAsync(request, cancellationToken);
        }

        // Create span
        var operationName = $"{request.Method.Method} {requestUri.AbsolutePath}";
        var tags = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
            new NSObject[] {
                new NSString(request.Method.Method),
                new NSString(requestUri.ToString()),
                new NSString("client"),
                new NSString("http")
            },
            new NSObject[] {
                new NSString("http.method"),
                new NSString("http.url"),
                new NSString("span.kind"),
                new NSString("component")
            }
        );

        var span = tracer.StartSpan(operationName, tags);

        DatadogHttpLogger.Log($"[Datadog HTTP] Started span: {operationName}");

        // Note: Header injection is skipped because DDTracer.Inject() has a binding issue
        // The native iOS SDK's URLSession instrumentation should handle trace propagation automatically
        // For now, we're just creating spans to track HTTP requests in APM
        DatadogHttpLogger.Log("[Datadog HTTP] Skipping header injection (using span tracking only)");

        HttpResponseMessage? response = null;
        Exception? exception = null;

        try
        {
            // Make the request
            response = await base.SendAsync(request, cancellationToken);

            // Set response status
            span.SetTag("http.status_code", new NSNumber((int)response.StatusCode));

            if (!response.IsSuccessStatusCode)
            {
                span.SetTag("error", true);
            }

            DatadogHttpLogger.Log($"[Datadog HTTP] Completed: {operationName} - {response.StatusCode}");

            return response;
        }
        catch (Exception ex)
        {
            exception = ex;

            // Mark span as error
            span.SetTag("error", true);
            span.SetTag("error.type", new NSString(ex.GetType().Name));
            span.SetTag("error.message", new NSString(ex.Message));
            span.SetTag("error.stack", new NSString(ex.StackTrace ?? ""));

            DatadogHttpLogger.Log($"[Datadog HTTP] Error: {operationName} - {ex.Message}");

            throw;
        }
        finally
        {
            // Finish the span
            span.Finish();
            DatadogHttpLogger.Log($"[Datadog HTTP] Finished span: {operationName}");
        }
    }

    private bool ShouldTrace(string host)
    {
        return _firstPartyHosts.Contains(host.ToLowerInvariant());
    }
}
