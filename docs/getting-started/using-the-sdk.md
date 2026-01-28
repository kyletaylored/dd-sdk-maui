---
layout: default
title: Using the SDK
parent: Getting Started
nav_order: 2
permalink: /getting-started/using-the-sdk
---

# Using the Datadog MAUI SDK

Complete guide to using the Datadog SDK in your .NET MAUI application.

{: .note }
> **New to Datadog?** Sign up for a free account at [datadoghq.com](https://www.datadoghq.com/) to get your client token and application ID.

## Installation

Add the Datadog MAUI SDK to your project:

```bash
dotnet add package Datadog.MAUI
```

Or via Package Manager Console:
```powershell
Install-Package Datadog.MAUI
```

## Quick Start

### 1. Initialize the SDK

In your `MauiProgram.cs`:

```csharp
using Datadog.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDatadog(config =>
            {
                config.ClientToken = "YOUR_CLIENT_TOKEN";
                config.Environment = "production";
                config.ServiceName = "my-maui-app";
                config.Site = DatadogSite.US1;

                // Enable RUM
                config.EnableRum(rum =>
                {
                    rum.SetApplicationId("YOUR_APPLICATION_ID");
                    rum.SetSessionSampleRate(100);
                });

                // Enable Logs
                config.EnableLogs(logs =>
                {
                    logs.SetSampleRate(100);
                });

                // Enable Tracing
                config.EnableTracing(tracing =>
                {
                    tracing.SetSampleRate(100);
                });
            });

        return builder.Build();
    }
}
```

### 2. Start Logging

```csharp
using Datadog.Maui;

// Create a logger
var logger = Logs.CreateLogger("my-logger");

// Log messages
logger.Info("Application started");
logger.Warn("Low memory warning", new Dictionary<string, object>
{
    { "available_memory", 256 },
    { "threshold", 512 }
});

// Log errors
try
{
    DoSomethingRisky();
}
catch (Exception ex)
{
    logger.Error("Operation failed", ex);
}
```

### 3. Track User Interactions (RUM)

```csharp
using Datadog.Maui;

// Track a view
Rum.StartView("home_screen", "Home Screen");

// Track user actions
Rum.AddAction(RumActionType.Tap, "login_button_clicked");

// Track errors
try
{
    await LoadData();
}
catch (Exception ex)
{
    Rum.AddError(ex);
}

// Stop the view when done
Rum.StopView("home_screen");
```

### 4. Trace Operations

```csharp
using Datadog.Maui;

// Start a span
using (var span = Tracer.StartSpan("api.fetch_users"))
{
    span.SetTag("user_id", userId);

    try
    {
        var users = await FetchUsers();
        span.SetTag("user_count", users.Count);
    }
    catch (Exception ex)
    {
        span.SetError(ex);
        throw;
    }
    // Span automatically finishes when disposed
}
```

## Configuration

### Basic Configuration

```csharp
builder.UseDatadog(config =>
{
    // Required
    config.ClientToken = "YOUR_CLIENT_TOKEN";
    config.Environment = "production";
    config.ServiceName = "my-maui-app";

    // Optional
    config.Site = DatadogSite.US1; // Default: US1
    config.TrackingConsent = TrackingConsent.Granted; // Default: Granted
    config.VerboseLogging = true; // Default: false

    // Add global tags
    config.GlobalTags["app_version"] = "1.0.0";
    config.GlobalTags["build_number"] = "42";
});
```

### RUM Configuration

```csharp
config.EnableRum(rum =>
{
    rum.SetApplicationId("YOUR_APPLICATION_ID");

    // Sampling rates (0-100)
    rum.SetSessionSampleRate(100); // Sample 100% of sessions
    rum.SetTelemetrySampleRate(20); // Sample 20% of internal telemetry

    // Auto-tracking
    rum.TrackViewsAutomatically(true); // Default: true
    rum.TrackUserInteractions(true); // Default: true
    rum.TrackResources(true); // Default: true
    rum.TrackErrors(true); // Default: true

    // Vitals frequency
    rum.SetVitalsUpdateFrequency(VitalsUpdateFrequency.Average);
});
```

### Logs Configuration

```csharp
config.EnableLogs(logs =>
{
    logs.SetSampleRate(100); // Sample 100% of logs
    logs.EnableNetworkInfo(true); // Include network info in logs
    logs.BundleWithRum(true); // Bundle logs with RUM sessions
});
```

### Tracing Configuration

```csharp
config.EnableTracing(tracing =>
{
    tracing.SetSampleRate(100); // Sample 100% of traces
    tracing.EnableTraceIdGeneration(true); // Generate trace IDs
    tracing.SetFirstPartyHosts(new[]
    {
        "api.myapp.com",
        "backend.myapp.com"
    });
});
```

### Builder Pattern Alternative

If you prefer not using the extension method:

```csharp
var configuration = new DatadogConfiguration.Builder("YOUR_CLIENT_TOKEN")
    .SetEnvironment("production")
    .SetServiceName("my-maui-app")
    .SetSite(DatadogSite.US1)
    .AddGlobalTag("version", "1.0.0")
    .EnableRum(rum =>
    {
        rum.SetApplicationId("YOUR_APPLICATION_ID");
        rum.SetSessionSampleRate(100);
    })
    .EnableLogs(logs =>
    {
        logs.SetSampleRate(100);
    })
    .EnableTracing(tracing =>
    {
        tracing.SetSampleRate(100);
    })
    .Build();

Datadog.Initialize(configuration);
```

## Logging

### Creating Loggers

```csharp
// Create named loggers
var networkLogger = Logs.CreateLogger("network");
var databaseLogger = Logs.CreateLogger("database");
var analyticsLogger = Logs.CreateLogger("analytics");
```

### Log Levels

```csharp
logger.Debug("Debug message");
logger.Info("Info message");
logger.Notice("Notice message");
logger.Warn("Warning message");
logger.Error("Error message");
logger.Critical("Critical message");
```

### Adding Context

```csharp
// Add attributes to individual logs
logger.Info("User logged in", new Dictionary<string, object>
{
    { "user_id", "12345" },
    { "method", "oauth" },
    { "duration_ms", 230 }
});

// Add attributes to all logs from this logger
logger.AddAttribute("component", "authentication");
logger.AddTag("env", "production");

// Add global attributes to ALL loggers
Logs.AddAttribute("app_version", "1.0.0");
Logs.AddTag("platform", DeviceInfo.Platform.ToString());
```

### Logging Exceptions

```csharp
try
{
    await ProcessPayment();
}
catch (Exception ex)
{
    logger.Error("Payment processing failed", ex, new Dictionary<string, object>
    {
        { "amount", 99.99 },
        { "currency", "USD" },
        { "payment_method", "credit_card" }
    });
}
```

## Real User Monitoring (RUM)

### View Tracking

```csharp
// Manual view tracking
public class HomePage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Rum.StartView("home_page", "Home", new Dictionary<string, object>
        {
            { "user_type", "premium" }
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Rum.StopView("home_page");
    }
}
```

### Action Tracking

```csharp
// Track button taps
private void OnLoginButtonClicked(object sender, EventArgs e)
{
    Rum.AddAction(RumActionType.Tap, "login_button", new Dictionary<string, object>
    {
        { "screen", "home" }
    });

    await PerformLogin();
}

// Track custom actions
Rum.AddAction(RumActionType.Custom, "checkout_completed", new Dictionary<string, object>
{
    { "total_amount", 149.99 },
    { "item_count", 3 }
});
```

### Resource Tracking

```csharp
// Track HTTP requests
var resourceKey = "fetch_users_" + Guid.NewGuid();

Rum.StartResource(resourceKey, "GET", "https://api.myapp.com/users");

try
{
    var response = await httpClient.GetAsync("https://api.myapp.com/users");
    var content = await response.Content.ReadAsStringAsync();

    Rum.StopResource(
        resourceKey,
        statusCode: (int)response.StatusCode,
        size: content.Length,
        kind: RumResourceKind.Xhr
    );
}
catch (Exception ex)
{
    Rum.StopResourceWithError(resourceKey, ex);
}
```

### Error Tracking

```csharp
// Track errors with exceptions
try
{
    await LoadData();
}
catch (Exception ex)
{
    Rum.AddError(ex, RumErrorSource.Source, new Dictionary<string, object>
    {
        { "operation", "load_data" },
        { "retry_count", retryCount }
    });
}

// Track errors with messages
Rum.AddError("Invalid user input", RumErrorSource.Source, null, new Dictionary<string, object>
{
    { "field", "email" },
    { "validation_rule", "format" }
});

// Track network errors
Rum.AddError("API request failed", RumErrorSource.Network);
```

### Custom Timings

```csharp
// Add custom performance timings
Rum.StartView("checkout", "Checkout");

// ... load initial data
Rum.AddTiming("initial_load_complete");

// ... fetch payment methods
Rum.AddTiming("payment_methods_loaded");

// ... process checkout
Rum.AddTiming("checkout_complete");

Rum.StopView("checkout");
```

### Global Attributes

```csharp
// Add attributes to all RUM events
Rum.AddAttribute("user_tier", "premium");
Rum.AddAttribute("experiment_group", "B");

// Remove attributes
Rum.RemoveAttribute("experiment_group");
```

## Distributed Tracing

### Basic Tracing

```csharp
// Start a span
using (var span = Tracer.StartSpan("operation_name"))
{
    span.SetTag("user_id", userId);
    span.SetTag("operation_type", "query");

    try
    {
        await PerformOperation();
    }
    catch (Exception ex)
    {
        span.SetError(ex);
        throw;
    }
    // Span finishes automatically on dispose
}
```

### Nested Spans

```csharp
// Parent span
using (var parentSpan = Tracer.StartSpan("process_order"))
{
    parentSpan.SetTag("order_id", orderId);

    // Child span
    using (var childSpan = Tracer.StartSpan("validate_payment", parentSpan))
    {
        await ValidatePayment();
    }

    // Another child span
    using (var childSpan = Tracer.StartSpan("update_inventory", parentSpan))
    {
        await UpdateInventory();
    }
}
```

### HTTP Client Tracing

```csharp
// Inject trace context into HTTP requests
var request = new HttpRequestMessage(HttpMethod.Get, "https://api.myapp.com/users");

using (var span = Tracer.StartSpan("api.fetch_users"))
{
    // Inject trace context into headers
    Tracer.Inject(request.Headers, span);

    var response = await httpClient.SendAsync(request);

    span.SetTag("http.status_code", (int)response.StatusCode);
}
```

### Extract Trace Context

```csharp
// Extract trace context from incoming headers
var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
var parentSpan = Tracer.Extract(headers);

// Continue the trace
using (var span = Tracer.StartSpan("process_request", parentSpan))
{
    await ProcessRequest();
}
```

## User Information

### Set User Information

```csharp
// Set user information
Datadog.SetUser(new UserInfo
{
    Id = "12345",
    Name = "John Doe",
    Email = "john.doe@example.com",
    ExtraInfo = new Dictionary<string, object>
    {
        { "plan", "premium" },
        { "signup_date", "2024-01-15" }
    }
});
```

### Clear User Information

```csharp
// Clear user information (e.g., on logout)
Datadog.ClearUser();
```

## Tracking Consent

### Update Consent

```csharp
// User granted consent
Datadog.SetTrackingConsent(TrackingConsent.Granted);

// User denied consent
Datadog.SetTrackingConsent(TrackingConsent.NotGranted);

// Consent is pending
Datadog.SetTrackingConsent(TrackingConsent.Pending);
```

{: .note }
> When consent is `NotGranted` or `Pending`, data is stored locally and will be uploaded once consent is `Granted`.

## Global Tags

```csharp
// Set global tags
Datadog.SetTags(new Dictionary<string, string>
{
    { "app_version", "1.0.0" },
    { "environment", "production" },
    { "datacenter", "us-east-1" }
});
```

## Platform-Specific Considerations

### Android

```csharp
// In your MainActivity.cs, ensure proper initialization
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);

    // Datadog initialization happens in MauiProgram
    // No additional setup required
}
```

### iOS

```csharp
// In your AppDelegate.cs, ensure proper initialization
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    // Datadog initialization happens in MauiProgram
    // No additional setup required

    return base.FinishedLaunching(application, launchOptions);
}
```

## Best Practices

### 1. Initialize Early

Initialize Datadog as early as possible in your app lifecycle:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder.UseMauiApp<App>();

    // Initialize Datadog FIRST
    builder.UseDatadog(config => { /* ... */ });

    // Then configure other services
    builder.Services.AddSingleton<MyService>();

    return builder.Build();
}
```

### 2. Use Named Loggers

Create loggers for different components:

```csharp
public class NetworkService
{
    private readonly ILogger _logger = Logs.CreateLogger("network");

    public async Task<Data> FetchData()
    {
        _logger.Info("Fetching data");
        // ...
    }
}
```

### 3. Add Context to Events

Always include relevant context:

```csharp
logger.Error("Payment failed", exception, new Dictionary<string, object>
{
    { "payment_method", paymentMethod },
    { "amount", amount },
    { "currency", currency },
    { "retry_count", retryCount }
});
```

### 4. Track View Lifecycle

Properly track view lifecycle:

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();
    Rum.StartView(ViewKey, ViewName);
}

protected override void OnDisappearing()
{
    base.OnDisappearing();
    Rum.StopView(ViewKey);
}
```

### 5. Use Spans with `using` Statements

Always use `using` statements for spans:

```csharp
// Good
using (var span = Tracer.StartSpan("operation"))
{
    await DoWork();
}

// Bad - span may not finish on exception
var span = Tracer.StartSpan("operation");
await DoWork();
span.Finish();
```

### 6. Respect User Privacy

Always handle user consent properly:

```csharp
// On app start
if (!HasUserConsent())
{
    Datadog.SetTrackingConsent(TrackingConsent.Pending);
}

// When user grants consent
private void OnConsentGranted()
{
    Datadog.SetTrackingConsent(TrackingConsent.Granted);
    // Pending data will now be uploaded
}
```

## Dependency Injection

Use the interfaces for dependency injection and testing:

```csharp
public class MyService
{
    private readonly ILogger _logger;
    private readonly IDatadogRum _rum;

    public MyService(ILogger logger, IDatadogRum rum)
    {
        _logger = logger ?? Logs.CreateLogger("my-service");
        _rum = rum ?? DatadogSdk.Rum;
    }

    public async Task DoWork()
    {
        _logger.Info("Starting work");
        _rum.AddAction(RumActionType.Custom, "work_started");

        // ...
    }
}
```

## Troubleshooting

### SDK Not Initialized

```csharp
// Check if SDK is initialized
if (!Datadog.IsInitialized)
{
    // Initialize or log error
    Console.WriteLine("Datadog SDK not initialized!");
}
```

### Events Not Appearing

1. **Check your client token and application ID**
2. **Verify tracking consent**: Ensure consent is `Granted`
3. **Check sample rates**: Sample rates below 100% may drop events
4. **Review network connectivity**: Events require network access
5. **Enable verbose logging**: Set `VerboseLogging = true` in configuration

### Performance Concerns

1. **Adjust sample rates**: Lower sample rates reduce data volume
2. **Disable auto-tracking**: Disable features you don't need
3. **Use async operations**: All SDK operations are non-blocking

## Next Steps

- [API Reference](../api-reference) - Complete API documentation
- [Code Examples](../examples) - More code examples
- [Advanced Configuration](../advanced-configuration) - Advanced scenarios
- [Performance](../performance) - Performance optimization guide

## Resources

- [Datadog RUM Documentation](https://docs.datadoghq.com/real_user_monitoring/)
- [Datadog Logs Documentation](https://docs.datadoghq.com/logs/)
- [Datadog APM Documentation](https://docs.datadoghq.com/tracing/)
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
