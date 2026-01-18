# Datadog iOS Trace (APM) - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS Trace. Implement distributed tracing and Application Performance Monitoring (APM) in your iOS application.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Trace
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.Trace" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` to be initialized first.

## Overview

Datadog Trace (APM) provides:
- **Distributed tracing** across services
- **Performance monitoring** for operations
- **Automatic RUM correlation** (link traces to user sessions)
- **Custom span creation** for any operation
- **Parent-child span relationships** for nested operations
- **Tag-based filtering** and analysis

## Quick Start

### Enable Tracing

In your `AppDelegate.cs` after Core SDK initialization:

```csharp
using DatadogCore;
using DatadogTrace;

// Initialize Core first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Enable tracing
var traceConfig = new DDTraceConfiguration();
traceConfig.SampleRate = 100.0f;              // Sample 100% of traces
traceConfig.BundleWithRumEnabled = true;      // Link traces to RUM

DDTrace.Enable(traceConfig);

// Get the tracer
var tracer = DDTracer.Shared;

Console.WriteLine("[Datadog] APM Tracing enabled");
```

## Creating Spans

### Basic Span

```csharp
var tracer = DDTracer.Shared;

// Start a span
var span = tracer.StartSpan("database_query");

// Perform operation
var results = await database.QueryAsync("SELECT * FROM users");

// Finish the span
span.Finish();
```

### Span with Tags

```csharp
var span = tracer.StartSpan("api_request");

// Add tags
span.SetTag("http.method", "GET");
span.SetTag("http.url", "https://api.example.com/users");
span.SetTag("user.id", "123");

// Perform operation
var response = await httpClient.GetAsync(url);

// Add response info
span.SetTag("http.status_code", (int)response.StatusCode);
span.SetTag("response.size", response.Content.Headers.ContentLength ?? 0);

span.Finish();
```

### Nested Spans (Parent-Child)

```csharp
// Parent span
var parentSpan = tracer.StartSpan("process_order");
parentSpan.SetTag("order.id", "order-123");

try
{
    // Child span 1
    var validateSpan = tracer.StartSpan("validate_order", childOf: parentSpan.Context);
    ValidateOrder(order);
    validateSpan.Finish();

    // Child span 2
    var paymentSpan = tracer.StartSpan("process_payment", childOf: parentSpan.Context);
    ProcessPayment(order);
    paymentSpan.Finish();

    // Child span 3
    var shippingSpan = tracer.StartSpan("create_shipment", childOf: parentSpan.Context);
    CreateShipment(order);
    shippingSpan.Finish();

    parentSpan.SetTag("order.status", "completed");
}
catch (Exception ex)
{
    parentSpan.SetTag("error", true);
    parentSpan.SetTag("error.message", ex.Message);
    throw;
}
finally
{
    parentSpan.Finish();
}
```

## Span Operations

### Setting Tags

```csharp
// String tags
span.SetTag("user.name", "John Doe");
span.SetTag("environment", "production");

// Numeric tags
span.SetTag("item.count", 42);
span.SetTag("response.time", 125.5);

// Boolean tags
span.SetTag("cache.hit", true);
span.SetTag("error", false);

// Error tags
span.SetTag("error", true);
span.SetTag("error.type", "ValidationException");
span.SetTag("error.message", "Invalid email address");
span.SetTag("error.stack", exception.StackTrace);
```

### Logging Events

```csharp
var span = tracer.StartSpan("complex_operation");

// Log an event with fields
span.Log(new NSDictionary(
    new NSString("event"), new NSString("query_executed"),
    new NSString("rows_returned"), new NSNumber(42),
    new NSString("query_time_ms"), new NSNumber(125)
));

// Log another event
span.Log(new NSDictionary(
    new NSString("event"), new NSString("cache_updated"),
    new NSString("cache_key"), new NSString("user_123")
));

span.Finish();
```

### Span Context

Access span context for propagation:

```csharp
var span = tracer.StartSpan("parent_operation");

// Get span context
var context = span.Context;

// Context contains:
// - TraceId: Unique ID for the entire trace
// - SpanId: Unique ID for this span
// - Can be used to create child spans

var childSpan = tracer.StartSpan("child_operation", childOf: context);
```

## Configuration Options

### DDTraceConfiguration

```csharp
var traceConfig = new DDTraceConfiguration();

// Sample rate (0-100)
traceConfig.SampleRate = 100.0f;  // Trace 100% of operations

// Bundle with RUM
traceConfig.BundleWithRumEnabled = true;  // Link traces to RUM sessions

// Service name (optional, defaults to app service)
traceConfig.Service = "my-service";

// Network info
traceConfig.NetworkInfoEnabled = true;

// Tags to add to all spans
traceConfig.Tags = new NSDictionary(
    new NSString("env"), new NSString("production"),
    new NSString("version"), new NSString("1.0.0")
);

DDTrace.Enable(traceConfig);
```

### Sample Rate

Control what percentage of traces to record:

```csharp
// Trace all operations (development)
traceConfig.SampleRate = 100.0f;

// Trace 20% of operations (production - cost control)
traceConfig.SampleRate = 20.0f;

// Don't trace anything
traceConfig.SampleRate = 0.0f;
```

## RUM Integration

### Automatic RUM Correlation

When RUM is enabled, traces are automatically linked to RUM sessions:

```csharp
// Enable RUM bundling
traceConfig.BundleWithRumEnabled = true;

// Traces now include:
// - session_id: RUM session ID
// - view.id: Current RUM view ID
// - user information
```

### Viewing Traces in RUM

In Datadog RUM Explorer:
1. View a session
2. See associated traces in timeline
3. Click trace to see full span details

## Complete Example

```csharp
using Foundation;
using UIKit;
using DatadogCore;
using DatadogTrace;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private static DDTracer? _tracer;
    public static DDTracer Tracer => _tracer ?? throw new InvalidOperationException("Tracer not initialized");

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        InitializeDatadog();
        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        try
        {
            // Initialize Core SDK
            var config = new DDConfiguration("YOUR_CLIENT_TOKEN", "prod");
            config.Service = "com.example.myapp";
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            // Configure tracing
            var traceConfig = new DDTraceConfiguration();

            #if DEBUG
            traceConfig.SampleRate = 100.0f;  // All traces in debug
            #else
            traceConfig.SampleRate = 20.0f;   // 20% in production
            #endif

            traceConfig.BundleWithRumEnabled = true;
            traceConfig.NetworkInfoEnabled = true;
            traceConfig.Tags = new NSDictionary(
                new NSString("env"), new NSString("production"),
                new NSString("version"), new NSString("1.0.0")
            );

            DDTrace.Enable(traceConfig);

            _tracer = DDTracer.Shared;

            Console.WriteLine("[Datadog] APM Tracing enabled");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Tracing initialization failed: {ex.Message}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## Usage Patterns

### Database Operations

```csharp
public async Task<List<User>> GetUsersAsync()
{
    var tracer = AppDelegate.Tracer;
    var span = tracer.StartSpan("database.query");
    span.SetTag("db.type", "sqlite");
    span.SetTag("db.operation", "SELECT");
    span.SetTag("db.table", "users");

    try
    {
        var users = await database.QueryAsync<User>("SELECT * FROM users");

        span.SetTag("result.count", users.Count);
        span.Finish();

        return users;
    }
    catch (Exception ex)
    {
        span.SetTag("error", true);
        span.SetTag("error.message", ex.Message);
        span.Finish();
        throw;
    }
}
```

### HTTP Requests

```csharp
public async Task<T> FetchFromApiAsync<T>(string endpoint)
{
    var tracer = AppDelegate.Tracer;
    var span = tracer.StartSpan("http.request");
    span.SetTag("http.method", "GET");
    span.SetTag("http.url", endpoint);

    try
    {
        var response = await httpClient.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();

        span.SetTag("http.status_code", (int)response.StatusCode);
        span.SetTag("response.size", content.Length);

        span.Finish();

        return JsonSerializer.Deserialize<T>(content);
    }
    catch (HttpRequestException ex)
    {
        span.SetTag("error", true);
        span.SetTag("error.type", "network");
        span.SetTag("error.message", ex.Message);
        span.Finish();
        throw;
    }
}
```

### Business Operations

```csharp
public async Task ProcessCheckoutAsync(Cart cart)
{
    var tracer = AppDelegate.Tracer;
    var checkoutSpan = tracer.StartSpan("checkout.process");
    checkoutSpan.SetTag("cart.items", cart.Items.Count);
    checkoutSpan.SetTag("cart.total", cart.Total);

    try
    {
        // Validate cart
        var validateSpan = tracer.StartSpan("checkout.validate", childOf: checkoutSpan.Context);
        ValidateCart(cart);
        validateSpan.Finish();

        // Process payment
        var paymentSpan = tracer.StartSpan("payment.process", childOf: checkoutSpan.Context);
        paymentSpan.SetTag("payment.method", cart.PaymentMethod);
        paymentSpan.SetTag("payment.amount", cart.Total);
        await ProcessPaymentAsync(cart);
        paymentSpan.Finish();

        // Create order
        var orderSpan = tracer.StartSpan("order.create", childOf: checkoutSpan.Context);
        var order = await CreateOrderAsync(cart);
        orderSpan.SetTag("order.id", order.Id);
        orderSpan.Finish();

        checkoutSpan.SetTag("checkout.success", true);
        checkoutSpan.SetTag("order.id", order.Id);
        checkoutSpan.Finish();
    }
    catch (Exception ex)
    {
        checkoutSpan.SetTag("error", true);
        checkoutSpan.SetTag("error.type", ex.GetType().Name);
        checkoutSpan.SetTag("error.message", ex.Message);
        checkoutSpan.Finish();
        throw;
    }
}
```

### Background Tasks

```csharp
public async Task SyncDataAsync()
{
    var tracer = AppDelegate.Tracer;
    var syncSpan = tracer.StartSpan("background.sync");
    syncSpan.SetTag("sync.type", "full");

    try
    {
        var startTime = DateTime.UtcNow;

        // Download data
        var downloadSpan = tracer.StartSpan("sync.download", childOf: syncSpan.Context);
        var data = await DownloadDataAsync();
        downloadSpan.SetTag("download.size", data.Length);
        downloadSpan.Finish();

        // Process data
        var processSpan = tracer.StartSpan("sync.process", childOf: syncSpan.Context);
        var records = ProcessData(data);
        processSpan.SetTag("records.count", records.Count);
        processSpan.Finish();

        // Save to database
        var saveSpan = tracer.StartSpan("sync.save", childOf: syncSpan.Context);
        await SaveToDatabase(records);
        saveSpan.Finish();

        var duration = (DateTime.UtcNow - startTime).TotalSeconds;
        syncSpan.SetTag("sync.duration", duration);
        syncSpan.SetTag("sync.success", true);
        syncSpan.Finish();
    }
    catch (Exception ex)
    {
        syncSpan.SetTag("error", true);
        syncSpan.SetTag("error.message", ex.Message);
        syncSpan.Finish();
        throw;
    }
}
```

## Best Practices

### 1. Use Descriptive Span Names

```csharp
// Good - clear operation name
tracer.StartSpan("database.query.users");
tracer.StartSpan("api.fetch.products");
tracer.StartSpan("payment.process.stripe");

// Avoid - vague names
tracer.StartSpan("operation");
tracer.StartSpan("fetch");
```

### 2. Add Meaningful Tags

```csharp
span.SetTag("operation", "checkout");
span.SetTag("user.id", userId);
span.SetTag("cart.total", total);
span.SetTag("payment.method", method);
```

### 3. Always Finish Spans

```csharp
// Use try-finally to ensure span finishes
var span = tracer.StartSpan("operation");
try
{
    PerformOperation();
}
finally
{
    span.Finish();
}
```

### 4. Tag Errors Properly

```csharp
catch (Exception ex)
{
    span.SetTag("error", true);
    span.SetTag("error.type", ex.GetType().Name);
    span.SetTag("error.message", ex.Message);
    span.SetTag("error.stack", ex.StackTrace);
    throw;
}
```

### 5. Use Parent-Child Relationships

```csharp
// Parent span for the overall operation
var parentSpan = tracer.StartSpan("process_order");

// Child spans for sub-operations
var childSpan = tracer.StartSpan("validate", childOf: parentSpan.Context);
```

### 6. Adjust Sample Rate for Production

```csharp
#if DEBUG
traceConfig.SampleRate = 100.0f;
#else
traceConfig.SampleRate = 20.0f;
#endif
```

### 7. Enable RUM Bundling

```csharp
traceConfig.BundleWithRumEnabled = true;
```

## Troubleshooting

### Traces Not Appearing

1. **Check Core SDK initialized**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Verify tracing enabled**:
```csharp
DDTrace.Enable(traceConfig);
```

3. **Check sample rate**:
```csharp
traceConfig.SampleRate = 100.0f;  // Track all traces
```

4. **Verify spans are finished**:
```csharp
span.Finish();  // Must call this!
```

### Traces Missing RUM Context

Enable RUM bundling:
```csharp
traceConfig.BundleWithRumEnabled = true;
```

Ensure RUM is enabled:
```csharp
DDRUM.Enable(rumConfig);
```

### High Trace Volume

Reduce sample rate:
```csharp
traceConfig.SampleRate = 10.0f;  // Only 10% of traces
```

### Missing Child Spans

Ensure parent context is passed:
```csharp
var childSpan = tracer.StartSpan("child", childOf: parentSpan.Context);
```

## API Reference

### DDTrace

| Method | Description |
|--------|-------------|
| `Enable(DDTraceConfiguration)` | Enable tracing with configuration |

### DDTracer

| Property | Description |
|----------|-------------|
| `Shared` | Get shared tracer instance |

| Method | Description |
|--------|-------------|
| `StartSpan(string)` | Start span with operation name |
| `StartSpan(string, childOf:)` | Start span as child of parent context |

### DDSpan

| Method | Description |
|--------|-------------|
| `SetTag(string, object)` | Add tag to span |
| `Log(NSDictionary)` | Log event with fields |
| `Finish()` | Finish span and send to Datadog |

| Property | Description |
|----------|-------------|
| `Context` | Get span context for creating child spans |

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogRUM](../DatadogRUM/README.md)** - Optional: Correlate traces with RUM sessions
- **[DatadogLogs](../DatadogLogs/README.md)** - Optional: Correlate traces with logs

## Resources

- [iOS APM Documentation](https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/ios/)
- [APM Trace Explorer](https://docs.datadoghq.com/tracing/trace_explorer/)
- [Distributed Tracing](https://docs.datadoghq.com/tracing/guide/distributed_tracing/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
