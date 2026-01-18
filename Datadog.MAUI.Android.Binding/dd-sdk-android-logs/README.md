# Datadog Android Logs SDK - .NET MAUI Binding

.NET MAUI bindings for Datadog Android Logs. Send logs from your Android application to Datadog for centralized logging, monitoring, and analysis.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Logs
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.Logs" Version="3.5.0" />
```

**Note**: This package requires `Datadog.MAUI.Android.Core` to be initialized first.

## Overview

The Logs SDK provides:
- **Structured logging** with multiple severity levels
- **Automatic context** (user info, network, device)
- **Custom attributes** for rich log data
- **RUM correlation** to link logs with user sessions
- **Network info** (connectivity status, carrier)
- **Batching and optimization** for performance

## Quick Start

### 1. Enable Logs

In your `MainApplication.cs` after Core SDK initialization:

```csharp
using Com.Datadog.Android.Log;

// Create logs configuration
var logsConfig = new LogsConfiguration.Builder().Build();

// Enable logging
Logs.Enable(logsConfig);

Console.WriteLine("[Datadog] Logs enabled");
```

### 2. Create a Logger

```csharp
// Create default logger
var logger = Logger.Builder()
    .SetNetworkInfoEnabled(true)
    .SetLogcatLogsEnabled(true)
    .SetBundleWithRumEnabled(true)
    .Build();

// Log messages
logger.V("Verbose message");
logger.D("Debug message");
logger.I("Info message");
logger.W("Warning message");
logger.E("Error message");
logger.Wtf("Critical failure");
```

That's it! Your logs will appear in the Datadog Logs Explorer.

## Configuration

### Logs Configuration Builder

```csharp
var logsConfig = new LogsConfiguration.Builder()
    // Custom endpoint (optional)
    .UseCustomEndpoint("https://custom-endpoint.example.com")

    // Event mapper to modify/filter logs before sending
    .SetEventMapper(logEvent => {
        // Modify or return null to drop the log
        return logEvent;
    })

    .Build();

Logs.Enable(logsConfig);
```

### Logger Builder

```csharp
var logger = Logger.Builder()
    // Service name (overrides global service)
    .SetService("my-service")

    // Logger name (appears in Datadog UI)
    .SetName("checkout-logger")

    // Include network info in logs
    .SetNetworkInfoEnabled(true)

    // Send logs to Android Logcat
    .SetLogcatLogsEnabled(true)

    // Bundle logs with RUM sessions
    .SetBundleWithRumEnabled(true)

    // Bundle logs with APM traces
    .SetBundleWithTraceEnabled(true)

    // Sample rate (0-100)
    .SetRemoteSampleRate(100f)

    // Log to Logcat only (not to Datadog)
    .SetRemoteLogThreshold(Android.Util.LogPriority.Assert)

    .Build();
```

## Logging Levels

### Log Methods

```csharp
// Verbose - detailed diagnostic info (lowest priority)
logger.V("Cache hit for key: user_123");

// Debug - diagnostic messages useful for debugging
logger.D("User authentication started");

// Info - general informational messages
logger.I("User logged in successfully");

// Warning - potentially harmful situations
logger.W("API rate limit approaching: 95/100 requests");

// Error - error events that allow app to continue
logger.E("Failed to load user preferences, using defaults");

// WTF (What a Terrible Failure) - critical failures
logger.Wtf("Database connection completely failed!");
```

### Log with Throwable/Exception

```csharp
try
{
    // ... code that might throw ...
}
catch (Exception ex)
{
    // Log C# exception
    logger.E("Payment processing failed", ex);

    // Or with custom message
    logger.E($"Payment failed for amount: {amount}", ex);
}

// Log Java throwable directly
catch (Java.Lang.Exception javaEx)
{
    logger.E("Java exception occurred", javaEx);
}
```

### Log with Attributes

```csharp
// Log with custom attributes
logger.I("User completed purchase", new Dictionary<string, Java.Lang.Object>
{
    { "user_id", "12345" },
    { "order_id", "ORD-789" },
    { "amount", 99.99 },
    { "currency", "USD" },
    { "payment_method", "credit_card" },
    { "items_count", 3 }
});

// Log error with context
logger.E("API request failed", new Dictionary<string, Java.Lang.Object>
{
    { "endpoint", "/api/users" },
    { "method", "POST" },
    { "status_code", 500 },
    { "retry_count", 3 }
}, exception);
```

## Advanced Features

### Global Logger Attributes

Add attributes that appear in all logs from a specific logger:

```csharp
// Create logger with permanent attributes
logger.AddAttribute("app_version", "1.2.3");
logger.AddAttribute("environment", "production");
logger.AddAttribute("feature_flag_checkout_v2", true);

// These attributes appear in all subsequent logs
logger.I("Event logged");  // Will include app_version, environment, feature_flag_checkout_v2

// Remove an attribute
logger.RemoveAttribute("feature_flag_checkout_v2");
```

### Global Logger Tags

Add tags for filtering in Datadog:

```csharp
// Add tags
logger.AddTag("team:mobile");
logger.AddTag("service:checkout");
logger.AddTag("version:1.2.3");

// Remove tag
logger.RemoveTag("version:1.2.3");
```

### Event Mapper (Filter/Transform Logs)

Modify or drop logs before they're sent:

```csharp
var logsConfig = new LogsConfiguration.Builder()
    .SetEventMapper(logEvent => {
        // Drop logs containing sensitive data
        if (logEvent.Message.Contains("password") ||
            logEvent.Message.Contains("credit_card"))
        {
            return null;  // Drop this log
        }

        // Add custom attribute to all logs
        logEvent.AdditionalAttributes["environment"] = "production";

        // Modify message
        if (logEvent.Message.Length > 1000)
        {
            logEvent.Message = logEvent.Message.Substring(0, 1000) + "... (truncated)";
        }

        return logEvent;  // Send modified log
    })
    .Build();
```

### Multiple Loggers

Create specialized loggers for different purposes:

```csharp
// Logger for authentication
var authLogger = Logger.Builder()
    .SetName("auth-logger")
    .SetService("authentication")
    .Build();

authLogger.I("User login attempt");

// Logger for payments
var paymentLogger = Logger.Builder()
    .SetName("payment-logger")
    .SetService("payment-processing")
    .Build();

paymentLogger.I("Payment initiated");

// Logger for analytics
var analyticsLogger = Logger.Builder()
    .SetName("analytics-logger")
    .SetRemoteSampleRate(10f)  // Sample 10% to reduce volume
    .Build();

analyticsLogger.I("Page view");
```

### Remote Log Threshold

Control which logs are sent to Datadog:

```csharp
var logger = Logger.Builder()
    // Only send Warn, Error, and Wtf to Datadog
    // Verbose, Debug, Info only go to Logcat
    .SetRemoteLogThreshold(Android.Util.LogPriority.Warn)
    .SetLogcatLogsEnabled(true)
    .Build();

logger.V("Verbose log");  // Logcat only
logger.D("Debug log");    // Logcat only
logger.I("Info log");     // Logcat only
logger.W("Warning log");  // Logcat + Datadog
logger.E("Error log");    // Logcat + Datadog
```

## RUM and Trace Integration

### Bundle with RUM

Link logs to RUM sessions:

```csharp
var logger = Logger.Builder()
    .SetBundleWithRumEnabled(true)
    .Build();

// Logs are now correlated with RUM session
logger.I("User action logged");
```

**Benefits**:
- See logs in context of user session in RUM Explorer
- Filter logs by RUM session ID
- Understand user journey leading to errors

### Bundle with APM Traces

Link logs to distributed traces:

```csharp
var logger = Logger.Builder()
    .SetBundleWithTraceEnabled(true)
    .Build();

// Logs include trace ID and span ID
logger.I("Database query executed");
```

**Benefits**:
- See logs within trace spans in APM
- Correlate application logs with backend traces
- Full request journey visibility

### Combined RUM + Trace + Logs

```csharp
var logger = Logger.Builder()
    .SetBundleWithRumEnabled(true)
    .SetBundleWithTraceEnabled(true)
    .Build();

// Logs correlated with both RUM and APM
logger.I("Payment processed", new Dictionary<string, Java.Lang.Object>
{
    { "amount", 99.99 },
    { "status", "success" }
});
```

## Complete Example

```csharp
using Android.App;
using Com.Datadog.Android.Log;

[Application]
public class MainApplication : MauiApplication
{
    private Logger _logger;

    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize Core SDK first
        InitializeDatadogCore();

        // Enable logs
        InitializeLogs();
    }

    private void InitializeLogs()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling Logs...");

            // Configure logs
            var logsConfig = new LogsConfiguration.Builder()
                .SetEventMapper(FilterSensitiveLogs)
                .Build();

            Logs.Enable(logsConfig);

            // Create application logger
            _logger = Logger.Builder()
                .SetService("my-mobile-app")
                .SetName("app-logger")
                .SetNetworkInfoEnabled(true)
                .SetLogcatLogsEnabled(true)
                .SetBundleWithRumEnabled(true)
                .SetBundleWithTraceEnabled(true)
                #if DEBUG
                .SetRemoteSampleRate(100f)
                .SetRemoteLogThreshold(Android.Util.LogPriority.Verbose)
                #else
                .SetRemoteSampleRate(100f)
                .SetRemoteLogThreshold(Android.Util.LogPriority.Info)
                #endif
                .Build();

            // Add global attributes
            _logger.AddAttribute("app_version", GetAppVersion());
            _logger.AddAttribute("device_model", Android.OS.Build.Model);
            _logger.AddAttribute("os_version", Android.OS.Build.VERSION.SdkInt.ToString());

            // Add tags
            _logger.AddTag("platform:android");
            _logger.AddTag("app:mobile");

            _logger.I("[Datadog] Logs enabled successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Logs initialization failed: {ex.Message}");
        }
    }

    private LogEvent FilterSensitiveLogs(LogEvent logEvent)
    {
        // Drop logs with sensitive keywords
        var sensitiveKeywords = new[] { "password", "credit_card", "ssn", "token" };

        foreach (var keyword in sensitiveKeywords)
        {
            if (logEvent.Message.ToLower().Contains(keyword))
            {
                return null;  // Drop this log
            }
        }

        return logEvent;
    }

    private string GetAppVersion()
    {
        try
        {
            var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
            return packageInfo.VersionName;
        }
        catch
        {
            return "unknown";
        }
    }

    public Logger GetLogger() => _logger;
}
```

## Usage in Activities/Services

```csharp
using Android.App;
using Com.Datadog.Android.Log;

[Activity(Label = "MainActivity")]
public class MainActivity : Activity
{
    private Logger _logger;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Get logger from Application
        var app = (MainApplication)Application;
        _logger = app.GetLogger();

        _logger.I("MainActivity created");
    }

    private void OnButtonClick()
    {
        _logger.D("Button clicked", new Dictionary<string, Java.Lang.Object>
        {
            { "button_id", "submit" },
            { "screen", "home" }
        });

        try
        {
            ProcessData();
        }
        catch (Exception ex)
        {
            _logger.E("Data processing failed", new Dictionary<string, Java.Lang.Object>
            {
                { "error_type", ex.GetType().Name },
                { "user_action", "button_click" }
            }, ex);
        }
    }

    private void ProcessData()
    {
        // ... business logic ...
    }

    protected override void OnDestroy()
    {
        _logger.I("MainActivity destroyed");
        base.OnDestroy();
    }
}
```

## Best Practices

### 1. Use Appropriate Log Levels

```csharp
// Good
logger.V("Cache lookup: key=user_123");  // Verbose for detailed tracing
logger.D("API request started");         // Debug for development
logger.I("User logged in");              // Info for important events
logger.W("Cache miss, fetching from API"); // Warn for suboptimal situations
logger.E("Payment failed", exception);   // Error for failures

// Avoid
logger.I("x = 5");  // Too verbose for Info level
logger.E("User clicked button");  // Not an error
```

### 2. Add Meaningful Attributes

```csharp
// Good - rich context
logger.E("Checkout failed", new Dictionary<string, Java.Lang.Object>
{
    { "user_id", userId },
    { "cart_value", 299.99 },
    { "payment_method", "credit_card" },
    { "error_code", "CARD_DECLINED" },
    { "retry_attempt", 2 }
});

// Avoid - missing context
logger.E("Checkout failed");
```

### 3. Use Structured Logging

```csharp
// Good - structured
logger.I("Order placed", new Dictionary<string, Java.Lang.Object>
{
    { "order_id", orderId },
    { "total_amount", 150.00 },
    { "item_count", 3 }
});

// Avoid - unstructured
logger.I($"Order {orderId} placed for ${150.00} with 3 items");
```

### 4. Filter Sensitive Data

```csharp
// Always filter PII and secrets
var logsConfig = new LogsConfiguration.Builder()
    .SetEventMapper(logEvent => {
        // Redact credit card numbers
        logEvent.Message = Regex.Replace(
            logEvent.Message,
            @"\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}",
            "****-****-****-****"
        );

        return logEvent;
    })
    .Build();
```

### 5. Use Multiple Loggers

Create specialized loggers for different components:

```csharp
var networkLogger = Logger.Builder()
    .SetName("network")
    .SetRemoteSampleRate(50f)  // Sample 50% of network logs
    .Build();

var businessLogger = Logger.Builder()
    .SetName("business-logic")
    .SetRemoteSampleRate(100f)  // Always send business logs
    .Build();
```

### 6. Optimize for Production

```csharp
#if DEBUG
var logger = Logger.Builder()
    .SetRemoteSampleRate(100f)
    .SetRemoteLogThreshold(Android.Util.LogPriority.Verbose)
    .Build();
#else
var logger = Logger.Builder()
    .SetRemoteSampleRate(100f)
    .SetRemoteLogThreshold(Android.Util.LogPriority.Info)  // Don't send V/D logs
    .Build();
#endif
```

### 7. Bundle with RUM and Traces

Enable correlation for better debugging:

```csharp
var logger = Logger.Builder()
    .SetBundleWithRumEnabled(true)   // See logs in RUM sessions
    .SetBundleWithTraceEnabled(true) // See logs in APM traces
    .Build();
```

## Troubleshooting

### Logs Not Appearing in Datadog

1. **Check Core SDK initialization**:
```csharp
if (Com.Datadog.Android.Datadog.Instance == null)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check Logs enabled**:
```csharp
// Should see no errors
Logs.Enable(new LogsConfiguration.Builder().Build());
```

3. **Check remote log threshold**:
```csharp
// Make sure threshold allows your log level
.SetRemoteLogThreshold(Android.Util.LogPriority.Verbose)
```

4. **Check sample rate**:
```csharp
.SetRemoteSampleRate(100f)  // Ensure 100% during testing
```

### Logs Only in Logcat

If logs appear in Logcat but not Datadog:

```csharp
// Check that Logcat-only mode is not enabled
var logger = Logger.Builder()
    .SetLogcatLogsEnabled(true)  // OK - also logs to Logcat
    .SetRemoteLogThreshold(Android.Util.LogPriority.Verbose)  // Also send to Datadog
    .Build();
```

### High Log Volume

Reduce log volume in production:

```csharp
// Sample logs
.SetRemoteSampleRate(10f)  // Send 10% of logs

// Only send warnings and errors
.SetRemoteLogThreshold(Android.Util.LogPriority.Warn)

// Use event mapper to drop verbose logs
.SetEventMapper(logEvent => {
    if (logEvent.Level == LogLevel.Verbose || logEvent.Level == LogLevel.Debug)
    {
        return null;  // Drop V/D logs
    }
    return logEvent;
})
```

## API Reference

### LogsConfiguration.Builder

| Method | Description |
|--------|-------------|
| `UseCustomEndpoint(string)` | Custom logs intake endpoint |
| `SetEventMapper(Func<LogEvent, LogEvent>)` | Transform/filter logs before sending |
| `Build()` | Create configuration |

### Logger.Builder

| Method | Description |
|--------|-------------|
| `SetService(string)` | Service name for this logger |
| `SetName(string)` | Logger name (appears in UI) |
| `SetNetworkInfoEnabled(bool)` | Include network info in logs |
| `SetLogcatLogsEnabled(bool)` | Also send logs to Android Logcat |
| `SetBundleWithRumEnabled(bool)` | Correlate logs with RUM sessions |
| `SetBundleWithTraceEnabled(bool)` | Correlate logs with APM traces |
| `SetRemoteSampleRate(float)` | Sample rate 0-100 |
| `SetRemoteLogThreshold(LogPriority)` | Minimum level to send to Datadog |
| `Build()` | Create logger |

### Logger Methods

| Method | Description |
|--------|-------------|
| `V(message)` | Verbose log |
| `D(message)` | Debug log |
| `I(message)` | Info log |
| `W(message)` | Warning log |
| `E(message)` | Error log |
| `Wtf(message)` | Critical failure log |
| `V/D/I/W/E/Wtf(message, attributes)` | Log with custom attributes |
| `V/D/I/W/E/Wtf(message, throwable)` | Log with exception |
| `V/D/I/W/E/Wtf(message, attributes, throwable)` | Log with attributes and exception |
| `AddAttribute(key, value)` | Add global attribute to all logs |
| `RemoveAttribute(key)` | Remove global attribute |
| `AddTag(tag)` | Add tag (e.g., "team:mobile") |
| `RemoveTag(tag)` | Remove tag |

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Correlate logs with RUM
- **[dd-sdk-android-trace](../dd-sdk-android-trace/README.md)** - Correlate logs with traces

## Resources

- [Datadog Android Log Collection](https://docs.datadoghq.com/logs/log_collection/android/)
- [Log Explorer](https://docs.datadoghq.com/logs/explorer/)
- [Log Processing](https://docs.datadoghq.com/logs/processing/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
