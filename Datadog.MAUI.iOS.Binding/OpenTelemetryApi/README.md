# OpenTelemetry API - .NET Binding for iOS

.NET MAUI binding for the OpenTelemetry API used by the Datadog iOS SDK. This binding provides the OpenTelemetry standard interface for distributed tracing.

## About

This package provides .NET bindings for the OpenTelemetry API framework, which is the vendor-neutral standard interface used by the Datadog iOS Trace SDK. OpenTelemetry defines a common API for observability including tracing, metrics, and logging.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Binding.OpenTelemetryApi --version 1.0.0
```

**Note:** This binding is automatically included as a dependency when you install `Datadog.MAUI`. You typically don't need to reference this directly unless you're building custom tracing integrations.

## What is OpenTelemetry?

OpenTelemetry is a vendor-neutral observability framework that provides:

- **Spans** - Represent units of work in distributed systems
- **Tracer** - Creates and manages spans
- **Context Propagation** - Carries trace information across process boundaries
- **Semantic Conventions** - Standard naming for common operations

The Datadog iOS SDK uses OpenTelemetry as its tracing interface, enabling compatibility with OpenTelemetry-compatible tools.

## Usage

### Basic Span Creation

```csharp
#if IOS
using OpenTelemetryApi;
using Datadog.iOS.DatadogTrace;

// Get the tracer provider (configured by Datadog)
var tracerProvider = DDTrace.TracerProvider;
var tracer = tracerProvider.Get("my-service");

// Create a span
var span = tracer.SpanBuilder("operation_name")
    .SetAttribute("custom.tag", "value")
    .StartSpan();

try
{
    // Your operation here
}
finally
{
    span.End();
}
#endif
```

### Span with Parent Context

```csharp
#if IOS
using OpenTelemetryApi;

var tracer = DDTrace.TracerProvider.Get("my-service");

// Create child span
var span = tracer.SpanBuilder("child_operation")
    .SetParent(Context.Current)
    .StartSpan();

// Do work
span.End();
#endif
```

## Integration with Datadog MAUI Plugin

The `Datadog.MAUI` plugin provides a higher-level cross-platform API:

```csharp
using Datadog.Maui.Tracing;

// Cross-platform API (recommended)
using var span = Tracer.StartSpan("operation_name");
span.SetTag("key", "value");
```

Use OpenTelemetry API directly when you need:
- Platform-specific tracing features
- Integration with OpenTelemetry libraries
- Fine-grained control over span attributes
- iOS-specific instrumentation

## OpenTelemetry API Overview

### Tracer

Creates and manages spans.

```csharp
// Get tracer from provider
var tracer = tracerProvider.Get("instrumentation-name");

// Build span
var spanBuilder = tracer.SpanBuilder("operation_name");
```

### Span

Represents a unit of work.

```csharp
// Set attributes
span.SetAttribute("http.method", "GET");
span.SetAttribute("http.status_code", 200);

// Add event
span.AddEvent("cache_hit");

// Set status
span.SetStatus(StatusCode.Ok);

// End span
span.End();
```

### SpanBuilder

Configures span before creation.

```csharp
var span = tracer.SpanBuilder("operation")
    .SetParent(parentContext)
    .SetSpanKind(SpanKind.Client)
    .SetAttribute("key", "value")
    .StartSpan();
```

## Example: HTTP Request Tracing

```csharp
#if IOS
using OpenTelemetryApi;
using Datadog.iOS.DatadogTrace;
using System.Net.Http;

public class TracedHttpClient
{
    private readonly HttpClient _client;
    private readonly Tracer _tracer;

    public TracedHttpClient()
    {
        _client = new HttpClient();
        _tracer = DDTrace.TracerProvider.Get("http-client");
    }

    public async Task<string> GetAsync(string url)
    {
        var span = _tracer.SpanBuilder("http.request")
            .SetSpanKind(SpanKind.Client)
            .SetAttribute("http.method", "GET")
            .SetAttribute("http.url", url)
            .StartSpan();

        try
        {
            var response = await _client.GetAsync(url);
            span.SetAttribute("http.status_code", (int)response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            span.SetStatus(StatusCode.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
        finally
        {
            span.End();
        }
    }
}
#endif
```

## Semantic Conventions

OpenTelemetry defines standard attributes for common operations:

| Attribute | Description | Example |
|-----------|-------------|---------|
| `http.method` | HTTP request method | `GET`, `POST` |
| `http.url` | Full URL | `https://api.example.com/users` |
| `http.status_code` | HTTP status code | `200`, `404` |
| `db.system` | Database type | `postgresql`, `redis` |
| `messaging.system` | Messaging system | `rabbitmq`, `kafka` |
| `rpc.service` | RPC service name | `MyService` |

## Span Kinds

```csharp
SpanKind.Internal  // Internal operation
SpanKind.Server    // Server handling request
SpanKind.Client    // Client making request
SpanKind.Producer  // Message producer
SpanKind.Consumer  // Message consumer
```

## Status Codes

```csharp
StatusCode.Unset   // Default, no explicit status
StatusCode.Ok      // Operation succeeded
StatusCode.Error   // Operation failed
```

## Native References

- [OpenTelemetry Documentation](https://opentelemetry.io/)
- [OpenTelemetry Specification](https://github.com/open-telemetry/opentelemetry-specification)
- [Datadog and OpenTelemetry](https://docs.datadoghq.com/tracing/trace_collection/open_standards/ios/)

## Version Information

- **OpenTelemetry API Version**: 1.0.0+
- **Framework**: `OpenTelemetryApi.xcframework`
- **Supported iOS**: 12.0+

## Differences from OpenTracing

OpenTelemetry is the successor to OpenTracing and OpenCensus:

- **Unified API** - Combines tracing, metrics, and logging
- **Context Propagation** - Improved cross-cutting context management
- **Semantic Conventions** - Standardized attribute naming
- **Actively Maintained** - OpenTracing is now archived

If you're familiar with OpenTracing (used in Android SDK), the concepts are similar but the API differs slightly.

## License

Apache 2.0 - See main repository LICENSE file.

OpenTelemetry API is licensed under Apache 2.0 by the OpenTelemetry Authors.
