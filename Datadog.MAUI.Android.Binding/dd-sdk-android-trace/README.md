# Datadog Android Trace SDK - .NET MAUI Binding

.NET MAUI bindings for Datadog Android APM (Application Performance Monitoring) Tracing. Monitor application performance, track distributed traces, and correlate frontend with backend services.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Trace
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.Trace" Version="3.5.0" />
```

**Note**: This package requires `Datadog.MAUI.Android.Core` to be initialized first.

## Overview

The Trace SDK provides:
- **Distributed tracing** across mobile and backend services
- **Automatic instrumentation** for OkHttp network requests
- **Custom spans** for tracking operations
- **RUM integration** to link traces with user sessions
- **Performance metrics** (operation duration, throughput)
- **Error tracking** within traces

## Quick Start

### 1. Enable Tracing

In your `MainApplication.cs` after Core SDK initialization:

```csharp
using Com.Datadog.Android.Trace;

// Create trace configuration
var traceConfig = new TraceConfiguration.Builder()
    .Build();

// Enable tracing
Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);

Console.WriteLine("[Datadog] APM Tracing enabled");
```

### 2. Get Tracer Instance

```csharp
// Get the global tracer
var tracer = AndroidTracer.Builder().Build();

// Or use OpenTracing API
var tracer = GlobalTracer.Get();
```

### 3. Create Spans

```csharp
// Start a span
var span = tracer.BuildSpan("database_query").Start();

try
{
    // Perform operation
    var result = await FetchDataFromDatabase();

    // Add tags
    span.SetTag("db.type", "postgresql");
    span.SetTag("rows.count", result.Count);
}
catch (Exception ex)
{
    // Mark span as error
    span.SetTag("error", true);
    span.SetTag("error.message", ex.Message);
    throw;
}
finally
{
    // Always finish the span
    span.Finish();
}
```

## Configuration

### Trace Configuration Builder

```csharp
var traceConfig = new TraceConfiguration.Builder()
    // Service name (overrides global service)
    .SetService("my-mobile-service")

    // Sample rate (0.0 to 100.0)
    .SetSampleRate(100.0)

    // Bundle traces with RUM sessions
    .SetBundleWithRumEnabled(true)

    // Include network info in traces
    .SetNetworkInfoEnabled(true)

    // Custom sampling rules
    .SetTraceSamplingRules(new List<TraceSamplingRule>
    {
        new TraceSamplingRule(serviceName: "critical-service", sampleRate: 100.0),
        new TraceSamplingRule(serviceName: "analytics", sampleRate: 10.0)
    })

    // Event mapper to modify traces
    .SetEventMapper(spanEvent => {
        // Modify or return null to drop
        return spanEvent;
    })

    .Build();

Trace.Enable(traceConfig, Datadog.Instance);
```

### Configuration Options Explained

#### Sample Rate
```csharp
// Development - trace everything
.SetSampleRate(100.0)

// Production - sample 20% of traces
.SetSampleRate(20.0)
```

**Sample Rate**: Percentage of traces to send (0-100). Lower values reduce data costs.

#### RUM Integration
```csharp
.SetBundleWithRumEnabled(true)
```

**Benefits**:
- See traces within RUM sessions
- Understand which API calls happened during user actions
- Full end-to-end visibility

#### Network Info
```csharp
.SetNetworkInfoEnabled(true)
```

Adds network context (WiFi, cellular, connection type) to all spans.

## Creating Spans

### Basic Span

```csharp
var span = tracer.BuildSpan("operation_name").Start();
try
{
    // ... perform operation ...
}
finally
{
    span.Finish();
}
```

### Span with Tags

Tags add metadata to spans:

```csharp
var span = tracer.BuildSpan("api_request")
    .WithTag("http.method", "GET")
    .WithTag("http.url", "https://api.example.com/users")
    .WithTag("user.id", "12345")
    .Start();

// Or set tags after creation
span.SetTag("http.status_code", 200);
span.SetTag("response.size", 2048);
span.SetTag("cache.hit", true);

span.Finish();
```

### Span with Parent (Nested Spans)

Create child spans to represent sub-operations:

```csharp
// Parent span
var parentSpan = tracer.BuildSpan("process_order").Start();

try
{
    // Child span 1 - validate
    var validateSpan = tracer.BuildSpan("validate_order")
        .AsChildOf(parentSpan.Context())
        .Start();
    ValidateOrder();
    validateSpan.Finish();

    // Child span 2 - charge payment
    var paymentSpan = tracer.BuildSpan("charge_payment")
        .AsChildOf(parentSpan.Context())
        .Start();
    ChargePayment();
    paymentSpan.Finish();

    // Child span 3 - send confirmation
    var emailSpan = tracer.BuildSpan("send_confirmation")
        .AsChildOf(parentSpan.Context())
        .Start();
    SendConfirmationEmail();
    emailSpan.Finish();
}
finally
{
    parentSpan.Finish();
}
```

### Error Handling in Spans

```csharp
var span = tracer.BuildSpan("payment_processing").Start();

try
{
    ProcessPayment();
}
catch (PaymentException ex)
{
    // Mark span as error
    span.SetTag(Tags.Error, true);
    span.SetTag("error.type", ex.GetType().Name);
    span.SetTag("error.message", ex.Message);
    span.SetTag("error.stack", ex.StackTrace);

    // Log error to span
    span.Log(new Dictionary<string, object>
    {
        { "event", "error" },
        { "error.kind", ex.GetType().Name },
        { "message", ex.Message },
        { "stack", ex.StackTrace }
    });

    throw;
}
finally
{
    span.Finish();
}
```

### Span Timing

```csharp
// Let SDK calculate duration (recommended)
var span = tracer.BuildSpan("operation").Start();
// ... operation ...
span.Finish();

// Or specify custom start/finish times
var startTime = DateTimeOffset.UtcNow;
var span = tracer.BuildSpan("operation")
    .WithStartTimestamp(startTime)
    .Start();
// ... operation ...
span.Finish(DateTimeOffset.UtcNow);
```

## Using Scopes (Active Span)

Scopes manage the current active span:

```csharp
using (var scope = tracer.BuildSpan("process_request")
    .StartActive(finishSpanOnDispose: true))
{
    // scope.Span is automatically the active span
    var span = scope.Span;

    span.SetTag("request.id", requestId);

    // Child spans automatically use active span as parent
    using (var childScope = tracer.BuildSpan("database_query")
        .StartActive(finishSpanOnDispose: true))
    {
        // This span is automatically a child of process_request
        childScope.Span.SetTag("query", "SELECT * FROM users");
        ExecuteQuery();
    }  // Child span finishes here

    ProcessResponse();
}  // Parent span finishes here
```

## Async Operations

For async methods, use scopes carefully:

```csharp
public async Task<string> FetchDataAsync()
{
    using (var scope = tracer.BuildSpan("fetch_data")
        .StartActive(finishSpanOnDispose: true))
    {
        var span = scope.Span;
        span.SetTag("operation.type", "async");

        try
        {
            var data = await httpClient.GetStringAsync("https://api.example.com/data");
            span.SetTag("data.size", data.Length);
            return data;
        }
        catch (Exception ex)
        {
            span.SetTag(Tags.Error, true);
            span.SetTag("error.message", ex.Message);
            throw;
        }
    }  // Span finishes after await completes
}
```

## Automatic OkHttp Instrumentation

Datadog automatically traces OkHttp requests to first-party hosts:

### Setup

```csharp
// In Core SDK configuration, specify first-party hosts
var config = new Configuration.Builder(clientToken, env, variant, service)
    .SetFirstPartyHosts(new List<string>
    {
        "api.example.com",
        "*.example.com"
    })
    .Build();

// Enable tracing
Trace.Enable(new TraceConfiguration.Builder().Build(), Datadog.Instance);

// OkHttp requests to these hosts are automatically traced!
using (var client = new OkHttpClient())
{
    var request = new Request.Builder()
        .Url("https://api.example.com/users")
        .Build();

    var response = await client.NewCall(request).ExecuteAsync();
    // Trace automatically created with request/response details
}
```

### What Gets Traced

Automatic instrumentation captures:
- HTTP method (GET, POST, etc.)
- URL
- Status code
- Request/response size
- Duration
- Errors

### Custom Attributes for HTTP Requests

Add custom attributes to automatic traces:

```csharp
var span = tracer.ActiveSpan;
if (span != null)
{
    span.SetTag("user.id", userId);
    span.SetTag("api.version", "v2");
}
```

## Complete Example

```csharp
using Android.App;
using Com.Datadog.Android.Trace;
using IO.Opentracing;
using IO.Opentracing.Util;

[Application]
public class MainApplication : MauiApplication
{
    private ITracer _tracer;

    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize Core SDK first
        InitializeDatadogCore();

        // Enable tracing
        InitializeTracing();
    }

    private void InitializeTracing()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling APM Tracing...");

            // Configure tracing
            var traceConfig = new TraceConfiguration.Builder()
                .SetService("my-mobile-app")
                #if DEBUG
                .SetSampleRate(100.0)  // Trace everything in debug
                #else
                .SetSampleRate(20.0)   // Sample 20% in production
                #endif
                .SetBundleWithRumEnabled(true)
                .SetNetworkInfoEnabled(true)
                .Build();

            // Enable tracing
            Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);

            // Create tracer
            _tracer = AndroidTracer.Builder()
                .SetService("my-mobile-app")
                .Build();

            // Register as global tracer
            GlobalTracer.RegisterIfAbsent(_tracer);

            Console.WriteLine("[Datadog] APM Tracing enabled successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Tracing initialization failed: {ex.Message}");
        }
    }

    public ITracer GetTracer() => _tracer;
}
```

## Usage Examples

### Database Query

```csharp
public async Task<List<User>> GetUsersAsync()
{
    var tracer = GlobalTracer.Get();

    using (var scope = tracer.BuildSpan("database.query")
        .WithTag("db.type", "sqlite")
        .WithTag("db.instance", "app.db")
        .WithTag("db.statement", "SELECT * FROM users")
        .StartActive(finishSpanOnDispose: true))
    {
        var span = scope.Span;

        try
        {
            var users = await _database.Query<User>("SELECT * FROM users");

            span.SetTag("rows.count", users.Count);
            span.SetTag("query.duration_ms", span.Duration.TotalMilliseconds);

            return users;
        }
        catch (Exception ex)
        {
            span.SetTag(Tags.Error, true);
            span.SetTag("error.message", ex.Message);
            throw;
        }
    }
}
```

### API Request

```csharp
public async Task<UserProfile> GetUserProfileAsync(string userId)
{
    var tracer = GlobalTracer.Get();

    using (var scope = tracer.BuildSpan("api.get_user_profile")
        .WithTag("http.method", "GET")
        .WithTag("user.id", userId)
        .StartActive(finishSpanOnDispose: true))
    {
        var span = scope.Span;

        var url = $"https://api.example.com/users/{userId}";
        span.SetTag("http.url", url);

        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                span.SetTag("http.status_code", (int)response.StatusCode);
                span.SetTag("response.size", response.Content.Headers.ContentLength ?? 0);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<UserProfile>(json);

                return profile;
            }
        }
        catch (HttpRequestException ex)
        {
            span.SetTag(Tags.Error, true);
            span.SetTag("error.type", "network");
            span.SetTag("error.message", ex.Message);
            throw;
        }
    }
}
```

### Background Job

```csharp
public async Task ProcessOrdersJob()
{
    var tracer = GlobalTracer.Get();

    using (var scope = tracer.BuildSpan("job.process_orders")
        .WithTag("job.type", "background")
        .WithTag("job.schedule", "hourly")
        .StartActive(finishSpanOnDispose: true))
    {
        var span = scope.Span;

        var processedCount = 0;
        var errorCount = 0;

        try
        {
            var orders = await GetPendingOrders();
            span.SetTag("orders.pending", orders.Count);

            foreach (var order in orders)
            {
                try
                {
                    await ProcessOrder(order);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    span.Log(new Dictionary<string, object>
                    {
                        { "event", "order_failed" },
                        { "order.id", order.Id },
                        { "error", ex.Message }
                    });
                }
            }

            span.SetTag("orders.processed", processedCount);
            span.SetTag("orders.failed", errorCount);
        }
        catch (Exception ex)
        {
            span.SetTag(Tags.Error, true);
            span.SetTag("error.message", ex.Message);
            throw;
        }
    }
}
```

## Best Practices

### 1. Use Meaningful Span Names

```csharp
// Good - describes the operation
tracer.BuildSpan("checkout.process_payment")
tracer.BuildSpan("database.insert_user")
tracer.BuildSpan("api.get_product_details")

// Avoid - too generic
tracer.BuildSpan("operation")
tracer.BuildSpan("execute")
tracer.BuildSpan("process")
```

### 2. Add Rich Tags

```csharp
// Good - provides context
span.SetTag("user.id", userId);
span.SetTag("product.sku", productSku);
span.SetTag("order.total", 299.99);
span.SetTag("payment.method", "credit_card");

// Avoid - missing context
span.SetTag("id", "12345");
```

### 3. Always Finish Spans

```csharp
// Good - using statement ensures finish
using (var scope = tracer.BuildSpan("operation")
    .StartActive(finishSpanOnDispose: true))
{
    // ... operation ...
}

// Also good - try/finally
var span = tracer.BuildSpan("operation").Start();
try
{
    // ... operation ...
}
finally
{
    span.Finish();  // Always called
}
```

### 4. Handle Errors Properly

```csharp
var span = tracer.BuildSpan("operation").Start();
try
{
    // ... operation ...
}
catch (Exception ex)
{
    span.SetTag(Tags.Error, true);
    span.SetTag("error.type", ex.GetType().Name);
    span.SetTag("error.message", ex.Message);
    span.SetTag("error.stack", ex.StackTrace);
    throw;
}
finally
{
    span.Finish();
}
```

### 5. Use Scopes for Context Propagation

```csharp
// Scope automatically propagates context to child spans
using (var scope = tracer.BuildSpan("parent").StartActive(true))
{
    // Child spans automatically reference parent
    using (var childScope = tracer.BuildSpan("child").StartActive(true))
    {
        // No need to manually specify parent
    }
}
```

### 6. Sample Appropriately

```csharp
// Development
.SetSampleRate(100.0)

// Production - high-traffic app
.SetSampleRate(10.0)  // 10% of traces

// Production - low-traffic app
.SetSampleRate(100.0)  // All traces (volume is manageable)
```

### 7. Bundle with RUM

Enable RUM correlation:

```csharp
.SetBundleWithRumEnabled(true)
```

Allows you to see:
- Which API calls happened during a user session
- Performance impact of backend calls on user experience

## Troubleshooting

### Traces Not Appearing

1. **Check Core SDK**:
```csharp
if (Datadog.Instance == null)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check Trace enabled**:
```csharp
Trace.Enable(config, Datadog.Instance);
```

3. **Check sample rate**:
```csharp
.SetSampleRate(100.0)  // Ensure 100% during testing
```

4. **Check service name**:
```csharp
// Verify service name is set
.SetService("my-service")
```

### Spans Not Connected (Orphaned)

If child spans don't appear under parents:

```csharp
// Use AsChildOf or scopes
var child = tracer.BuildSpan("child")
    .AsChildOf(parentSpan.Context())
    .Start();

// Or use active span
using (var parentScope = tracer.BuildSpan("parent").StartActive(true))
{
    // Children automatically connected
    using (var childScope = tracer.BuildSpan("child").StartActive(true))
    {
        // ...
    }
}
```

### High Trace Volume

Reduce volume in production:

```csharp
// Lower sample rate
.SetSampleRate(10.0)  // 10% of traces

// Use sampling rules for selective tracing
.SetTraceSamplingRules(new List<TraceSamplingRule>
{
    new TraceSamplingRule("critical-service", 100.0),  // Always trace
    new TraceSamplingRule("analytics", 5.0)            // Sample 5%
})
```

## API Reference

### TraceConfiguration.Builder

| Method | Description |
|--------|-------------|
| `SetService(string)` | Service name for traces |
| `SetSampleRate(double)` | Sample rate 0-100 |
| `SetBundleWithRumEnabled(bool)` | Correlate with RUM |
| `SetNetworkInfoEnabled(bool)` | Include network info |
| `SetTraceSamplingRules(List)` | Custom sampling rules |
| `SetEventMapper(Func)` | Transform/filter spans |
| `Build()` | Create configuration |

### ITracer Methods

| Method | Description |
|--------|-------------|
| `BuildSpan(operationName)` | Create span builder |
| `ActiveSpan` | Get current active span |
| `ScopeManager` | Get scope manager |

### ISpanBuilder Methods

| Method | Description |
|--------|-------------|
| `WithTag(key, value)` | Add tag to span |
| `AsChildOf(spanContext)` | Set parent span |
| `Start()` | Create and start span |
| `StartActive(finishOnDispose)` | Create scope with span |

### ISpan Methods

| Method | Description |
|--------|-------------|
| `SetTag(key, value)` | Add/update tag |
| `Log(fields)` | Log structured data |
| `SetOperationName(name)` | Change operation name |
| `Finish()` | Complete span |
| `Finish(finishTimestamp)` | Complete with custom time |

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Correlate traces with RUM
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Correlate traces with logs

## Resources

- [Datadog Android Tracing](https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/android/)
- [APM Trace Explorer](https://docs.datadoghq.com/tracing/trace_explorer/)
- [Distributed Tracing](https://docs.datadoghq.com/tracing/trace_collection/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
