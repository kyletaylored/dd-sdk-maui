using System.Diagnostics;
using Datadog.iOS.Trace;
using Foundation;

namespace Datadog.Maui.Http;

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
        if (requestUri == null || !ShouldTrace(requestUri.Host))
        {
            // Not a first-party host, don't trace
            return await base.SendAsync(request, cancellationToken);
        }

        // Get the Datadog tracer
        // Note: DDTracer.Shared can throw if tracing not enabled yet
        OTTracer? tracer = null;
        try
        {
            tracer = DDTracer.Shared;
        }
        catch (InvalidCastException ex)
        {
            Debug.WriteLine($"[Datadog HTTP] Tracer cast error (likely not initialized): {ex.Message}");
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Datadog HTTP] Failed to get tracer: {ex.Message}");
            return await base.SendAsync(request, cancellationToken);
        }

        if (tracer == null)
        {
            Debug.WriteLine("[Datadog HTTP] Tracer not available, skipping instrumentation");
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

        Debug.WriteLine($"[Datadog HTTP] Started span: {operationName}");

        // Inject trace headers
        try
        {
            var headerWriter = new DDHTTPHeadersWriter(DDTraceContextInjection.All);
            var traceHeaders = headerWriter.TraceHeaderFields;

            foreach (var header in traceHeaders)
            {
                request.Headers.TryAddWithoutValidation(header.Key.ToString(), header.Value.ToString());
                Debug.WriteLine($"[Datadog HTTP] Injected header: {header.Key} = {header.Value}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Datadog HTTP] Failed to inject headers: {ex.Message}");
        }

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

            Debug.WriteLine($"[Datadog HTTP] Completed: {operationName} - {response.StatusCode}");

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

            Debug.WriteLine($"[Datadog HTTP] Error: {operationName} - {ex.Message}");

            throw;
        }
        finally
        {
            // Finish the span
            span.Finish();
            Debug.WriteLine($"[Datadog HTTP] Finished span: {operationName}");
        }
    }

    private bool ShouldTrace(string host)
    {
        return _firstPartyHosts.Contains(host.ToLowerInvariant());
    }
}
