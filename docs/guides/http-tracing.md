---
layout: default
title: HTTP Request Tracing
parent: Guides
nav_order: 6
permalink: /guides/http-tracing
---

# HTTP Request Tracing in .NET MAUI

How to trace HTTP requests and enable distributed tracing in your .NET MAUI application.

{: .warning }
> **Current Limitation**: Automatic HTTP tracing for iOS `HttpClient` is not yet implemented in the unified Datadog.MAUI plugin. Manual instrumentation is required. See [workarounds](#workarounds) below.

---

## Table of Contents

- [How HTTP Tracing Works](#how-http-tracing-works)
- [Android Implementation](#android-implementation)
- [iOS Implementation](#ios-implementation)
- [Manual HTTP Tracing](#manual-http-tracing)
- [Workarounds](#workarounds)
- [Future Improvements](#future-improvements)

---

## How HTTP Tracing Works

HTTP request tracing enables **distributed tracing** across your mobile app and backend services by:

1. **Injecting trace headers** - Adding `x-datadog-*` headers to outgoing HTTP requests
2. **Creating spans** - Automatically generating APM spans for HTTP calls
3. **Correlating with RUM** - Linking HTTP requests to RUM sessions and views
4. **Propagating context** - Carrying trace IDs across service boundaries

### First-Party vs Third-Party Hosts

- **First-Party Hosts**: Your own backend services (e.g., `api.myapp.com`)
  - **Traced with headers** - Distributed tracing enabled
  - **Full instrumentation** - Request/response metrics, status codes, errors

- **Third-Party Hosts**: External APIs (e.g., `api.stripe.com`)
  - **Tracked in RUM** - Appear as resources, but no trace headers injected
  - **Privacy** - Prevents leaking trace context to third parties

---

## Android Implementation

### ✅ Automatic HTTP Tracing (Works)

On Android, HTTP tracing works automatically when you configure first-party hosts:

```csharp
// In MauiProgram.cs
builder.UseDatadog(config =>
{
    config.ClientToken = "YOUR_CLIENT_TOKEN";
    config.Environment = "production";
    config.ServiceName = "my-app";

    // Configure first-party hosts for distributed tracing
    config.FirstPartyHosts = new[]
    {
        "api.myapp.com",
        "backend.myapp.com"
    };

    // Enable tracing
    config.EnableTracing(tracing =>
    {
        tracing.SetSampleRate(100);
        tracing.SetFirstPartyHosts(new[]
        {
            "api.myapp.com",
            "backend.myapp.com"
        });
    });
});
```

### What Gets Traced Automatically

All `HttpClient` requests to first-party hosts will automatically:
- Generate APM spans
- Inject `x-datadog-trace-id`, `x-datadog-parent-id`, `x-datadog-origin`, `x-datadog-sampling-priority` headers
- Track request timing, status codes, and errors
- Correlate with RUM sessions

### Example

```csharp
// Your normal HttpClient code - no changes needed!
public class ApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.myapp.com")
    };

    public async Task<User> GetUserAsync(string userId)
    {
        // This request is automatically traced on Android!
        var response = await _httpClient.GetAsync($"/users/{userId}");
        return await response.Content.ReadFromJsonAsync<User>();
    }
}
```

---

## iOS Implementation

### ❌ Automatic HTTP Tracing (Not Yet Implemented)

{: .warning }
> **Known Issue**: The Datadog MAUI plugin does not currently configure iOS URLSession instrumentation, so `HttpClient` requests are **not automatically traced** on iOS.

### Why Doesn't It Work?

In the native iOS Datadog SDK, HTTP tracing requires:

1. **URLSession Instrumentation** - Swizzling `NSURLSession` delegate methods
2. **Delegate Class Configuration** - Specifying which delegate class to instrument
3. **First-Party Hosts** - Configuring which hosts to trace

The Swift equivalent:

```swift
import DatadogTrace

// Enable URLSession instrumentation
URLSessionInstrumentation.enable(
    with: .init(
        delegateClass: SessionDelegate.self
    )
)

// Configure trace with first-party hosts
Trace.enable(
    with: Trace.Configuration(
        urlSessionTracking: .trace(
            hosts: ["api.myapp.com", "backend.myapp.com"]
        )
    )
)
```

### Current iOS Implementation Gap

The iOS platform code in `Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs` does NOT:
- Enable `URLSessionInstrumentation`
- Configure first-party hosts on `DDTraceConfiguration`
- Set `URLSessionTracking` property

**What's implemented:**
```csharp
// Current iOS implementation - incomplete!
private static void InitializeTracing(TracingConfiguration tracingConfig)
{
    var traceConfiguration = new DDTraceConfiguration();
    traceConfiguration.SampleRate = tracingConfig.SampleRate;
    DDTrace.EnableWith(traceConfiguration);
    // ⚠️ FirstPartyHosts NOT configured
    // ⚠️ URLSession instrumentation NOT enabled
}
```

**What's needed:**
```csharp
// What the implementation should do
private static void InitializeTracing(TracingConfiguration tracingConfig)
{
    var traceConfiguration = new DDTraceConfiguration();
    traceConfiguration.SampleRate = tracingConfig.SampleRate;

    // Configure first-party hosts for tracing
    if (tracingConfig.FirstPartyHosts.Length > 0)
    {
        var hosts = new NSSet<NSString>(
            tracingConfig.FirstPartyHosts.Select(h => new NSString(h)).ToArray()
        );

        var firstPartyHostsTracing = new DDTraceFirstPartyHostsTracing(hosts);
        var urlSessionTracking = new DDTraceURLSessionTracking(firstPartyHostsTracing);

        traceConfiguration.SetURLSessionTracking(urlSessionTracking);
    }

    DDTrace.EnableWith(traceConfiguration);

    // Enable URLSession instrumentation
    // Note: Requires delegate class - needs investigation
    // DDURLSessionInstrumentation.EnableWithConfiguration(...);
}
```

---

## Manual HTTP Tracing

Until automatic tracing is implemented on iOS, you can manually instrument HTTP requests using the `Tracer` API.

### Using the Tracer API

```csharp
using Datadog.Maui;

public class ApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.myapp.com")
    };

    public async Task<User> GetUserAsync(string userId)
    {
        // Start a span for the HTTP request
        using (var span = Tracer.StartSpan("http.request"))
        {
            span.SetTag("http.method", "GET");
            span.SetTag("http.url", $"/users/{userId}");
            span.SetTag("resource.name", $"GET /users/{userId}");

            try
            {
                // Create request
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"/users/{userId}"
                );

                // Inject trace context into headers
                Tracer.Inject(request.Headers, span);

                // Make the request
                var response = await _httpClient.SendAsync(request);

                // Tag the span with response info
                span.SetTag("http.status_code", (int)response.StatusCode);

                // Handle response
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                throw;
            }
        }
    }
}
```

### Creating a TracedHttpClient Wrapper

To avoid repeating tracing code, create a wrapper:

```csharp
using Datadog.Maui;

public class TracedHttpClient
{
    private readonly HttpClient _httpClient;

    public TracedHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string url)
    {
        return await SendAsync<T>(HttpMethod.Get, url);
    }

    public async Task<T> PostAsync<T>(string url, object body)
    {
        return await SendAsync<T>(HttpMethod.Post, url, body);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        using (var span = Tracer.StartSpan("http.request"))
        {
            span.SetTag("http.method", method.Method);
            span.SetTag("http.url", url);
            span.SetTag("resource.name", $"{method.Method} {url}");

            try
            {
                var request = new HttpRequestMessage(method, url);

                if (body != null)
                {
                    request.Content = JsonContent.Create(body);
                }

                // Inject trace headers
                Tracer.Inject(request.Headers, span);

                var response = await _httpClient.SendAsync(request);

                span.SetTag("http.status_code", (int)response.StatusCode);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                span.SetTag("error", true);
                throw;
            }
        }
    }
}

// Usage
public class ApiService
{
    private readonly TracedHttpClient _client;

    public ApiService()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.myapp.com")
        };
        _client = new TracedHttpClient(httpClient);
    }

    public async Task<User> GetUserAsync(string userId)
    {
        // Automatic tracing!
        return await _client.GetAsync<User>($"/users/{userId}");
    }
}
```

---

## Workarounds

### Option 1: Platform-Specific HTTP Clients

Use platform-specific implementations that work with native instrumentation:

**Android** (works automatically):
```csharp
// Platforms/Android/HttpClientFactory.cs
public static class HttpClientFactory
{
    public static HttpClient Create()
    {
        // Standard HttpClient - automatically traced!
        return new HttpClient();
    }
}
```

**iOS** (use native URLSession):
```csharp
// Platforms/iOS/HttpClientFactory.cs
using Foundation;

public static class HttpClientFactory
{
    public static HttpClient Create()
    {
        // Use NSUrlSessionHandler for better iOS integration
        // This handler uses NSURLSession under the hood
        var handler = new NSUrlSessionHandler();
        return new HttpClient(handler);
    }
}
```

Then use in shared code:
```csharp
// Shared code
#if ANDROID
using HttpClientFactory = YourApp.Platforms.Android.HttpClientFactory;
#elif IOS
using HttpClientFactory = YourApp.Platforms.iOS.HttpClientFactory;
#endif

public class ApiService
{
    private readonly HttpClient _httpClient = HttpClientFactory.Create();
}
```

### Option 2: Direct Native SDK Usage (iOS Only)

For iOS, you can bypass the MAUI plugin and use the native SDK directly:

```csharp
// Platforms/iOS/Services/TracedApiService.cs
using Foundation;
using Datadog.iOS.Trace;

public class TracedApiService
{
    public async Task<User> GetUserAsync(string userId)
    {
        // Create native tracer span
        var tracer = DDTracer.Shared;
        var span = tracer.BuildSpan("http.request").Start();
        span.SetTag("http.method", "GET");
        span.SetTag("http.url", $"/users/{userId}");

        try
        {
            // Use NSURLSession with instrumentation enabled
            var url = new NSUrl($"https://api.myapp.com/users/{userId}");
            var request = new NSUrlRequest(url);

            var (data, response) = await NSUrlSession.SharedSession.DataAsync(request);

            span.SetTag("http.status_code", (int)((NSHttpUrlResponse)response).StatusCode);
            span.Finish();

            // Parse response
            var json = NSJsonSerialization.JsonObject(data, 0, out _);
            return ParseUser(json);
        }
        catch (Exception ex)
        {
            span.SetTag("error", true);
            span.SetTag("error.message", ex.Message);
            span.Finish();
            throw;
        }
    }
}
```

### Option 3: Use RUM Resource Tracking

Even without APM tracing, you can still track HTTP requests in RUM:

```csharp
using Datadog.Maui;

public class ApiService
{
    private readonly HttpClient _httpClient = new();

    public async Task<User> GetUserAsync(string userId)
    {
        var resourceKey = $"get_user_{Guid.NewGuid()}";
        var url = $"https://api.myapp.com/users/{userId}";

        // Start RUM resource tracking
        Rum.StartResource(resourceKey, "GET", url);

        try
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Stop resource tracking with success
            Rum.StopResource(
                resourceKey,
                statusCode: (int)response.StatusCode,
                size: content.Length,
                kind: RumResourceKind.Xhr
            );

            return JsonSerializer.Deserialize<User>(content);
        }
        catch (Exception ex)
        {
            // Stop resource tracking with error
            Rum.StopResourceWithError(resourceKey, ex);
            throw;
        }
    }
}
```

{: .note }
> This tracks requests in RUM but does NOT enable distributed tracing (no trace headers injected).

---

## Future Improvements

The Datadog MAUI SDK team is working on implementing automatic HTTP tracing for iOS. The planned improvements include:

### Phase 1: Basic URLSession Instrumentation
- ✅ iOS bindings for `DDURLSessionInstrumentation` (already exists)
- ⏳ Configure URLSession instrumentation in iOS platform code
- ⏳ Apply first-party hosts configuration
- ⏳ Test with standard `HttpClient` using `NSUrlSessionHandler`

### Phase 2: HttpClient Interception
- ⏳ Create custom `HttpMessageHandler` for tracing
- ⏳ Automatic span creation for HTTP requests
- ⏳ Automatic header injection
- ⏳ Error and status code tracking

### Phase 3: Unified API
- ⏳ Single configuration for both platforms
- ⏳ Automatic platform detection and setup
- ⏳ Zero-code instrumentation

### How You Can Help

If you'd like to contribute to implementing automatic HTTP tracing:

1. **Test the bindings** - Verify iOS URLSession instrumentation works with native code
2. **Prototype solutions** - Try different approaches for `HttpClient` instrumentation
3. **Share feedback** - Report what works and what doesn't in your apps
4. **Submit PRs** - Contribute implementations to the [GitHub repository](https://github.com/kyletaylored/dd-sdk-maui)

---

## Related Documentation

- [Distributed Tracing Guide](https://docs.datadoghq.com/tracing/) - Official Datadog APM docs
- [iOS SDK - Network Tracking](https://docs.datadoghq.com/real_user_monitoring/ios/advanced_configuration/#automatically-track-network-requests) - Native iOS SDK docs
- [Android SDK - Network Tracking](https://docs.datadoghq.com/real_user_monitoring/android/advanced_configuration/#automatically-track-network-requests) - Native Android SDK docs
- [Tracing API Reference](../api-reference#tracing-api) - MAUI SDK tracing API

---

## Summary

| Platform | Automatic Tracing | Status | Workaround |
|----------|------------------|--------|------------|
| **Android** | ✅ Yes | Works automatically | None needed |
| **iOS** | ❌ No | Not yet implemented | Manual `Tracer` API or RUM tracking |

For production apps requiring distributed tracing on iOS, use the [manual tracing approach](#manual-http-tracing) or create a [TracedHttpClient wrapper](#creating-a-tracedhttpclient-wrapper) until automatic instrumentation is implemented.
