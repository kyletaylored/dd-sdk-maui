using System.Net;
using Foundation;
using Datadog.iOS.Core;

namespace Datadog.Maui.Platforms.iOS;

/// <summary>
/// HTTP message handler that uses Datadog's native URLSession instrumentation for automatic RUM resource tracking.
/// This provides automatic network request tracking without manual span creation.
/// </summary>
/// <remarks>
/// **EXPERIMENTAL - NOT CURRENTLY WORKING**
///
/// This handler creates a dedicated NSUrlSession with a delegate that is registered with Datadog's
/// URLSessionInstrumentation. However, the DDURLSessionInstrumentation API in the current Datadog iOS SDK (3.5.0)
/// causes crashes when called with a custom delegate class.
///
/// The crash occurs at: DDURLSessionInstrumentation.EnableWithConfiguration() with error "Attempt to use unknown class"
///
/// **STATUS**: Waiting for next Datadog iOS SDK release which will include automatic swizzling support.
/// Until then, use manual RUM resource tracking with Rum.StartResource/StopResource instead.
///
/// **DO NOT USE THIS CLASS** until the Datadog iOS SDK is updated.
/// </remarks>
public class InstrumentedHttpMessageHandler : HttpMessageHandler
{
    private readonly NSUrlSession _session;
    private readonly InstrumentedSessionDelegate _delegate;
    private readonly bool _instrumentationEnabled;

    /// <summary>
    /// Creates a new InstrumentedHttpMessageHandler with Datadog URLSession instrumentation.
    /// </summary>
    /// <param name="firstPartyHosts">Array of first-party host names for distributed tracing.</param>
    /// <param name="tracingSampleRate">Sample rate for distributed tracing on first-party hosts (0-100). Default: 20.</param>
    public InstrumentedHttpMessageHandler(string[]? firstPartyHosts = null, int tracingSampleRate = 20)
    {
        _instrumentationEnabled = firstPartyHosts != null && firstPartyHosts.Length > 0;

        // Create custom delegate for the session
        _delegate = new InstrumentedSessionDelegate();

        // Create session configuration with reasonable defaults
        var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
        config.TimeoutIntervalForRequest = 60; // 60 seconds default timeout
        config.TimeoutIntervalForResource = 300; // 5 minutes for large downloads

        // Create NSUrlSession with our delegate
        _session = NSUrlSession.FromConfiguration(config, _delegate, null);

        // Register with Datadog's URLSessionInstrumentation if first-party hosts are provided
        if (_instrumentationEnabled)
        {
            EnableDatadogInstrumentation(firstPartyHosts!, tracingSampleRate);
        }

        System.Diagnostics.Debug.WriteLine($"[Datadog] InstrumentedHttpMessageHandler created (instrumentation: {_instrumentationEnabled})");
    }

    /// <summary>
    /// Enables Datadog URLSession instrumentation for the delegate.
    /// </summary>
    private void EnableDatadogInstrumentation(string[] firstPartyHosts, int tracingSampleRate)
    {
        try
        {
            // Create instrumentation configuration with our delegate
            var instrumentationConfig = new DDURLSessionInstrumentationConfiguration(_delegate);

            // Configure first-party hosts for distributed tracing
            // Note: DDURLSessionInstrumentationFirstPartyHostsTracing (Core binding) does not support
            // sample rate configuration like DDRUMFirstPartyHostsTracing (RUM binding) does.
            // The sample rate parameter is accepted but cannot be applied at this level.
            var hostsSet = new NSSet<NSString>(firstPartyHosts.Select(h => new NSString(h)).ToArray());
            var firstPartyHostsTracing = new DDURLSessionInstrumentationFirstPartyHostsTracing(hostsSet);
            instrumentationConfig.SetFirstPartyHostsTracing(firstPartyHostsTracing);

            // Enable the instrumentation
            DDURLSessionInstrumentation.EnableWithConfiguration(instrumentationConfig);

            System.Diagnostics.Debug.WriteLine($"[Datadog] URLSession instrumentation ENABLED for hosts: {string.Join(", ", firstPartyHosts)}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] NOTE: Sample rate configuration not available in URLSessionInstrumentation API");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Datadog] ERROR enabling URLSession instrumentation: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Sends an HTTP request using the instrumented NSUrlSession.
    /// Network requests are automatically tracked by Datadog RUM.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Convert HttpRequestMessage to NSUrlRequest
        var nsRequest = await CreateNSUrlRequestAsync(request);

        // Create TaskCompletionSource for async operation
        var tcs = new TaskCompletionSource<HttpResponseMessage>();

        // Register cancellation
        using var registration = cancellationToken.Register(() =>
        {
            tcs.TrySetCanceled(cancellationToken);
        });

        // Create and start the data task
        var dataTask = _session.CreateDataTask(nsRequest, (data, response, error) =>
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(cancellationToken);
                    return;
                }

                if (error != null)
                {
                    var exception = new HttpRequestException(error.LocalizedDescription, null, (HttpStatusCode)0);
                    tcs.TrySetException(exception);
                    return;
                }

                var httpResponse = CreateHttpResponseMessage(response as NSHttpUrlResponse, data, request);
                tcs.TrySetResult(httpResponse);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        dataTask.Resume();

        return await tcs.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Converts .NET HttpRequestMessage to NSUrlRequest.
    /// </summary>
    private async Task<NSMutableUrlRequest> CreateNSUrlRequestAsync(HttpRequestMessage request)
    {
        var url = new NSUrl(request.RequestUri!.AbsoluteUri);
        var nsRequest = new NSMutableUrlRequest(url)
        {
            HttpMethod = request.Method.Method
        };

        // Copy request headers
        foreach (var header in request.Headers)
        {
            var value = string.Join(", ", header.Value);
            nsRequest[header.Key] = value;
        }

        // Handle request content
        if (request.Content != null)
        {
            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                var value = string.Join(", ", header.Value);
                nsRequest[header.Key] = value;
            }

            // Set request body
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            nsRequest.Body = NSData.FromArray(contentBytes);
        }

        return nsRequest;
    }

    /// <summary>
    /// Converts NSHttpUrlResponse to .NET HttpResponseMessage.
    /// </summary>
    private HttpResponseMessage CreateHttpResponseMessage(NSHttpUrlResponse? nsResponse, NSData? data, HttpRequestMessage request)
    {
        var statusCode = nsResponse != null ? (HttpStatusCode)(int)nsResponse.StatusCode : HttpStatusCode.OK;
        var response = new HttpResponseMessage(statusCode)
        {
            RequestMessage = request
        };

        // Copy response headers
        if (nsResponse?.AllHeaderFields != null)
        {
            foreach (var kvp in nsResponse.AllHeaderFields)
            {
                var key = kvp.Key.ToString();
                var value = kvp.Value.ToString();

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    // Try adding to response headers first, then content headers
                    if (!response.Headers.TryAddWithoutValidation(key, value))
                    {
                        // Some headers belong to content
                        if (response.Content != null)
                        {
                            response.Content.Headers.TryAddWithoutValidation(key, value);
                        }
                    }
                }
            }
        }

        // Set response content
        if (data != null && data.Length > 0)
        {
            var bytes = new byte[data.Length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
            response.Content = new ByteArrayContent(bytes);
        }
        else
        {
            response.Content = new ByteArrayContent(Array.Empty<byte>());
        }

        return response;
    }

    /// <summary>
    /// Disposes the handler and cleans up resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                // Invalidate and cancel all tasks in the session
                _session?.InvalidateAndCancel();

                // Disable Datadog instrumentation if it was enabled
                if (_instrumentationEnabled)
                {
                    try
                    {
                        DDURLSessionInstrumentation.DisableWithDelegateClass(_delegate);
                        System.Diagnostics.Debug.WriteLine("[Datadog] URLSession instrumentation disabled");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Datadog] WARNING: Failed to disable instrumentation: {ex.Message}");
                    }
                }

                _session?.Dispose();
                _delegate?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Datadog] Error during disposal: {ex.Message}");
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Custom NSUrlSessionDataDelegate that will be instrumented by Datadog.
    /// The delegate methods are intercepted by Datadog's instrumentation layer
    /// to automatically track network requests as RUM resources.
    /// </summary>
    [Foundation.Register("InstrumentedSessionDelegate")]
    private class InstrumentedSessionDelegate : NSUrlSessionDataDelegate
    {
        public InstrumentedSessionDelegate()
        {
            System.Diagnostics.Debug.WriteLine("[Datadog] InstrumentedSessionDelegate created");
        }

        public InstrumentedSessionDelegate(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        // Note: We don't need to implement delegate methods explicitly.
        // Datadog's URLSessionInstrumentation will intercept calls to this delegate
        // and automatically track network metrics, timing, and errors as RUM resources.
        //
        // The instrumentation handles:
        // - Request start/end timing
        // - Response status codes
        // - Error tracking
        // - Distributed trace header injection for first-party hosts
        // - RUM resource event creation
    }
}
