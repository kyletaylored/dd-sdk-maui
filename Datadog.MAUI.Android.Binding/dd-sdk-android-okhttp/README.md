# Datadog Android OkHttp Instrumentation - .NET Binding

.NET MAUI binding for the Datadog OkHttp instrumentation library for Android. This binding provides automatic distributed tracing and RUM resource tracking for applications using OkHttp as their HTTP client.

## About

This package provides .NET bindings for the native Datadog OkHttp instrumentation (`com.datadoghq:dd-sdk-android-okhttp`). It automatically instruments OkHttp clients to track HTTP requests as RUM resources and create distributed tracing spans.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Binding.OkHttp --version 3.5.0
```

**Note:** This binding is automatically included when you install the main `Datadog.MAUI` package. You typically don't need to reference this directly unless you're using OkHttp directly in platform-specific code.

## Features

- **Automatic HTTP Tracking** - Monitor all OkHttp requests without manual instrumentation
- **RUM Resource Integration** - HTTP calls appear as resources in RUM
- **Distributed Tracing** - Propagate trace context to backend services
- **Error Tracking** - Failed requests are automatically reported
- **Performance Metrics** - Track request timing and data transfer

## When to Use This Package

Use this package when:
- You're making HTTP requests using OkHttp in Android platform-specific code
- You need fine-grained control over HTTP client configuration
- You're integrating with existing OkHttp-based libraries

For most MAUI applications using `HttpClient`, the main `Datadog.MAUI` plugin handles HTTP tracking automatically.

## Usage

### Prerequisites

Initialize the Datadog Core SDK and enable Tracing:

```csharp
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Datadog;
using Com.Datadog.Android.Trace;

var configuration = new Configuration.Builder(
    clientToken: "YOUR_CLIENT_TOKEN",
    env: "production",
    variant: ""
)
.UseSite(DatadogSite.Us1)
.Build();

Datadog.Initialize(Application.Context, configuration, TrackingConsent.Granted);

// Enable Tracing
Trace.Enable(new TraceConfiguration.Builder().Build());
```

### Instrument OkHttp Client

```csharp
using Com.Datadog.Android.Okhttp;
using Square.OkHttp3;

// Create OkHttp client with Datadog instrumentation
var client = new OkHttpClient.Builder()
    .AddInterceptor(new DatadogInterceptor())
    .Build();

// Use the instrumented client for requests
var request = new Request.Builder()
    .Url("https://api.example.com/data")
    .Build();

var response = client.NewCall(request).Execute();
```

### Advanced Configuration

```csharp
using Com.Datadog.Android.Okhttp;
using Com.Datadog.Android.Trace;
using Square.OkHttp3;
using System.Collections.Generic;

// Configure first-party hosts for distributed tracing
var firstPartyHosts = new List<string>
{
    "api.example.com",
    "backend.example.com"
};

var datadogInterceptor = new DatadogInterceptor(
    tracedHosts: firstPartyHosts,
    traceSampler: new RateBasedSampler(1.0f) // Sample 100% of traces
);

var client = new OkHttpClient.Builder()
    .AddInterceptor(datadogInterceptor)
    .Build();
```

### Event Listener for Advanced Tracking

```csharp
using Com.Datadog.Android.Okhttp;
using Square.OkHttp3;

// Use DatadogEventListener for additional connection metrics
var eventListener = new DatadogEventListener.Factory();

var client = new OkHttpClient.Builder()
    .AddInterceptor(new DatadogInterceptor())
    .EventListenerFactory(eventListener)
    .Build();
```

## API Reference

### DatadogInterceptor

OkHttp interceptor that tracks requests and creates tracing spans.

#### Constructors

```csharp
// Basic interceptor with default settings
public DatadogInterceptor();

// With traced hosts for distributed tracing
public DatadogInterceptor(IList<string> tracedHosts);

// With traced hosts and custom sampler
public DatadogInterceptor(IList<string> tracedHosts, TraceSampler traceSampler);
```

### DatadogEventListener.Factory

Event listener factory for tracking connection-level metrics.

```csharp
// Create factory
var factory = new DatadogEventListener.Factory();

// Use with OkHttpClient
client.EventListenerFactory(factory);
```

## Integration with MAUI Plugin

The main `Datadog.MAUI` plugin automatically instruments `HttpClient` on Android, which uses OkHttp under the hood. This package is already included and configured for you.

You only need to use this package directly if you're:
- Using OkHttp directly in Android platform code
- Implementing custom HTTP clients
- Integrating third-party libraries that use OkHttp

## Example: Custom API Client

```csharp
#if ANDROID
using Com.Datadog.Android.Okhttp;
using Square.OkHttp3;
using System.Threading.Tasks;

public class CustomApiClient
{
    private readonly OkHttpClient _client;

    public CustomApiClient()
    {
        _client = new OkHttpClient.Builder()
            .AddInterceptor(new DatadogInterceptor(
                tracedHosts: new[] { "api.myservice.com" }
            ))
            .Build();
    }

    public async Task<string> GetDataAsync(string endpoint)
    {
        var request = new Request.Builder()
            .Url($"https://api.myservice.com/{endpoint}")
            .Build();

        using var response = await _client.NewCall(request).ExecuteAsync();
        return await response.Body().StringAsync();
    }
}
#endif
```

## Tracked Metrics

The OkHttp instrumentation automatically tracks:

- **Request Duration** - Time from request start to response completion
- **Status Codes** - HTTP response status codes
- **Request/Response Size** - Data transfer volume
- **Errors** - Network failures and HTTP errors
- **Connection Metrics** - DNS lookup, TCP connection, TLS handshake times (with EventListener)

## Native Android Reference

For complete native SDK documentation, see:
- [Datadog OkHttp Instrumentation](https://docs.datadoghq.com/real_user_monitoring/android/advanced_configuration/#okhttp)

## Version Information

- **Native SDK Version**: 3.5.0
- **Maven Artifact**: `com.datadoghq:dd-sdk-android-okhttp:3.5.0`
- **OkHttp Version**: 4.x

## License

Apache 2.0 - See main repository LICENSE file.
