# OpenTracing API - .NET Binding for Android

.NET MAUI binding for the OpenTracing API used by the Datadog Android SDK. This binding provides the OpenTracing standard interface for distributed tracing.

## About

This package provides .NET bindings for the OpenTracing API (`io.opentracing:opentracing-api`), which is the standard interface used by the Datadog Android Trace SDK. OpenTracing defines a vendor-neutral API for distributed tracing.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Binding.OpenTracingApi --version 0.33.0
```

**Note:** This binding is automatically included as a dependency when you install `Datadog.MAUI`. You typically don't need to reference this directly unless you're building custom tracing integrations.

## What is OpenTracing?

OpenTracing is a vendor-neutral API specification for distributed tracing. It defines a common interface for:

- **Spans** - Representing units of work in a distributed system
- **Span Context** - Propagating trace information across process boundaries
- **Tracers** - Creating and managing spans
- **Scope Management** - Managing the active span

The Datadog Android SDK uses OpenTracing as its tracing interface, allowing compatibility with OpenTracing-compatible tools and libraries.

## Usage

### Basic Span Creation (via Datadog Tracer)

```csharp
#if ANDROID
using Com.Datadog.Android.Trace;
using Io.Opentracing;
using Io.Opentracing.Util;

// Get the global tracer (Datadog tracer)
ITracer tracer = GlobalTracer.Get();

// Create a span
ISpan span = tracer.BuildSpan("operation_name")
    .WithTag("custom.tag", "value")
    .Start();

try
{
    // Your operation here
}
finally
{
    span.Finish();
}
#endif
```

### Using Span Context for Distributed Tracing

```csharp
#if ANDROID
using Io.Opentracing;
using Io.Opentracing.Propagation;
using Io.Opentracing.Util;

ITracer tracer = GlobalTracer.Get();

// Extract context from incoming request headers
var headers = new Dictionary<string, string>();
ISpanContext parentContext = tracer.Extract(
    BuiltinFormats.HttpHeaders,
    new TextMapAdapter(headers)
);

// Create child span with parent context
ISpan span = tracer.BuildSpan("handle_request")
    .AsChildOf(parentContext)
    .Start();

// Do work...

span.Finish();
#endif
```

### Injecting Trace Context

```csharp
#if ANDROID
using Io.Opentracing;
using Io.Opentracing.Propagation;
using Io.Opentracing.Util;

ITracer tracer = GlobalTracer.Get();
ISpan span = tracer.ActiveSpan();

if (span != null)
{
    // Inject context into outgoing request headers
    var headers = new Dictionary<string, string>();
    tracer.Inject(
        span.Context,
        BuiltinFormats.HttpHeaders,
        new TextMapInjectAdapter(headers)
    );

    // Use headers in HTTP request
}
#endif
```

## Integration with Datadog MAUI Plugin

The `Datadog.MAUI` plugin provides a higher-level API that wraps OpenTracing functionality. For most use cases, use the plugin's `Tracer` class instead:

```csharp
using Datadog.Maui.Tracing;

// High-level API (works on all platforms)
using var span = Tracer.StartSpan("operation_name");
span.SetTag("key", "value");
```

The OpenTracing API is useful when you need:
- Direct access to OpenTracing-compatible APIs
- Integration with third-party OpenTracing libraries
- Advanced tracing features specific to OpenTracing
- Android platform-specific tracing code

## OpenTracing API Overview

### ITracer

Main interface for creating spans and managing trace context.

```csharp
// Create spans
ISpan BuildSpan(string operationName);

// Get active span
ISpan ActiveSpan();

// Extract context from headers
ISpanContext Extract(IFormat format, ICarrier carrier);

// Inject context into headers
void Inject(ISpanContext context, IFormat format, ICarrier carrier);
```

### ISpan

Represents a unit of work in a distributed trace.

```csharp
// Set tags
ISpan SetTag(string key, string value);
ISpan SetTag(string key, bool value);
ISpan SetTag(string key, double value);

// Log events
ISpan Log(string message);
ISpan Log(IDictionary<string, object> fields);

// Set baggage (cross-process key-value pairs)
ISpan SetBaggageItem(string key, string value);
string GetBaggageItem(string key);

// Finish span
void Finish();
void Finish(long finishMicros);
```

### ISpanContext

Trace context that can be propagated across process boundaries.

```csharp
// Get baggage items
IEnumerable<KeyValuePair<string, string>> GetBaggageItems();
```

## Example: Custom HTTP Instrumentation

```csharp
#if ANDROID
using Io.Opentracing;
using Io.Opentracing.Propagation;
using Io.Opentracing.Tag;
using Io.Opentracing.Util;
using System.Net.Http;

public class TracedHttpClient
{
    private readonly HttpClient _client;
    private readonly ITracer _tracer;

    public TracedHttpClient()
    {
        _client = new HttpClient();
        _tracer = GlobalTracer.Get();
    }

    public async Task<string> GetAsync(string url)
    {
        var span = _tracer.BuildSpan("http.request")
            .WithTag(Tags.SpanKind.Key, Tags.SpanKindClient)
            .WithTag(Tags.HttpMethod.Key, "GET")
            .WithTag(Tags.HttpUrl.Key, url)
            .Start();

        try
        {
            // Inject trace context into request headers
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var carrier = new HttpHeadersCarrier(request.Headers);
            _tracer.Inject(span.Context, BuiltinFormats.HttpHeaders, carrier);

            var response = await _client.SendAsync(request);
            span.SetTag(Tags.HttpStatus.Key, (int)response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            span.SetTag(Tags.Error.Key, true);
            span.Log(new Dictionary<string, object>
            {
                { "event", "error" },
                { "error.kind", ex.GetType().Name },
                { "message", ex.Message }
            });
            throw;
        }
        finally
        {
            span.Finish();
        }
    }
}
#endif
```

## OpenTracing Standards

This binding implements the [OpenTracing Specification v1.1](https://github.com/opentracing/specification/blob/master/specification.md).

### Semantic Conventions

OpenTracing defines standard tags for common scenarios:

| Tag | Description | Example |
|-----|-------------|---------|
| `span.kind` | Span type | `client`, `server`, `producer`, `consumer` |
| `http.method` | HTTP method | `GET`, `POST` |
| `http.url` | Full URL | `https://api.example.com/users` |
| `http.status_code` | HTTP status | `200`, `404` |
| `db.type` | Database type | `sql`, `redis` |
| `error` | Error flag | `true` (when error occurs) |

## Native References

- [OpenTracing Documentation](https://opentracing.io/)
- [OpenTracing Specification](https://github.com/opentracing/specification)
- [Datadog and OpenTracing](https://docs.datadoghq.com/tracing/trace_collection/open_standards/java/)

## Version Information

- **OpenTracing API Version**: 0.33.0
- **Maven Artifact**: `io.opentracing:opentracing-api:0.33.0`

## License

Apache 2.0 - See main repository LICENSE file.

OpenTracing API is licensed under Apache 2.0 by the OpenTracing Authors.
