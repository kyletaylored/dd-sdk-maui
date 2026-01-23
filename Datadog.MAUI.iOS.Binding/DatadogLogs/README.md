# Datadog iOS Logs - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS Logs. Send structured logs from your iOS application to Datadog with automatic RUM session correlation.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Logs
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.Logs" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` to be initialized first.

## Overview

Datadog Logs provides:
- **Structured logging** with custom attributes
- **Multiple log levels** (Debug, Info, Notice, Warn, Error, Critical)
- **Automatic RUM correlation** (link logs to RUM sessions)
- **Automatic trace correlation** (link logs to APM traces)
- **Network info** (cellular vs WiFi)
- **Custom logger instances** with different configurations

## Quick Start

### Enable Logging

In your `AppDelegate.cs` after Core SDK initialization:

```csharp
using Datadog.iOS.Core;
using Datadog.iOS.Logs;

// Initialize Core first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Enable logging
var logsConfig = new DDLogsConfiguration();
DDLogs.Enable(logsConfig);

// Create a logger
var logger = DDLogger.Create();

// Log messages
logger.Info("Application started", null);
logger.Debug("Debug information", null);
logger.Error("An error occurred", null);

Console.WriteLine("[Datadog] Logging enabled");
```

## Creating Loggers

### Default Logger

```csharp
// Create default logger
var logger = DDLogger.Create();

// Use it
logger.Info("Simple log message", null);
```

### Custom Logger

```csharp
// Create custom logger with configuration
var loggerConfig = new DDLoggerConfiguration
{
    Service = "my-service",
    Name = "my-logger",
    NetworkInfoEnabled = true,
    BundleWithRumEnabled = true,
    RemoteSampleRate = 100.0f,
    RemoteLogThreshold = DDLogLevel.Debug
};

var customLogger = DDLogger.Create(loggerConfig);
```

## Log Levels

### Available Levels

```csharp
// Debug - detailed diagnostic information
logger.Debug("Cache hit for key: user_123", null);

// Info - general informational messages
logger.Info("User logged in successfully", null);

// Notice - normal but significant events
logger.Notice("Configuration reloaded", null);

// Warn - warning messages
logger.Warn("API rate limit approaching", null);

// Error - error events
logger.Error("Failed to save user preferences", null);

// Critical - critical conditions
logger.Critical("Database connection failed", null);
```

### Log Level Enum

```csharp
public enum DDLogLevel
{
    Debug,     // Detailed diagnostic info
    Info,      // Informational messages
    Notice,    // Significant events
    Warn,      // Warning messages
    Error,     // Error conditions
    Critical   // Critical failures
}
```

## Log Attributes

### Adding Attributes

```csharp
// Log with custom attributes
var attributes = new NSDictionary(
    new NSString("user_id"), new NSString("123"),
    new NSString("action"), new NSString("purchase"),
    new NSString("amount"), new NSNumber(99.99),
    new NSString("currency"), new NSString("USD")
);

logger.Info("User completed purchase", attributes);
```

### Permanent Logger Attributes

Add attributes that appear in all logs from this logger:

```csharp
// Add permanent attributes
logger.AddAttribute(new NSString("app_version"), new NSString("1.2.3"));
logger.AddAttribute(new NSString("build"), new NSString("42"));
logger.AddAttribute(new NSString("environment"), new NSString("production"));

// All subsequent logs include these attributes
logger.Info("Event happened", null);  // Includes app_version, build, environment

// Remove attribute
logger.RemoveAttribute("build");
```

## Logger Configuration

### DDLogsConfiguration

Global logs configuration:

```csharp
var logsConfig = new DDLogsConfiguration
{
    EventMapper = (logEvent) =>
    {
        // Modify log event before sending
        // Return null to drop the event
        return logEvent;
    }
};

DDLogs.Enable(logsConfig);
```

### DDLoggerConfiguration

Per-logger configuration:

```csharp
var loggerConfig = new DDLoggerConfiguration();

// Service name
loggerConfig.Service = "my-service";

// Logger name
loggerConfig.Name = "checkout-logger";

// Include network info
loggerConfig.NetworkInfoEnabled = true;

// Bundle with RUM
loggerConfig.BundleWithRumEnabled = true;

// Bundle with Trace
loggerConfig.BundleWithTraceEnabled = true;

// Sample rate (0-100)
loggerConfig.RemoteSampleRate = 100.0f;

// Minimum level to send remotely
loggerConfig.RemoteLogThreshold = DDLogLevel.Debug;

// Enable console output
loggerConfig.ConsoleLogFormat = DDConsoleLogFormat.Short;

var logger = DDLogger.Create(loggerConfig);
```

## RUM Integration

### Automatic RUM Correlation

When RUM is enabled, logs are automatically linked to RUM sessions:

```csharp
// Enable RUM bundling
loggerConfig.BundleWithRumEnabled = true;

// Logs now include:
// - session_id: RUM session ID
// - view.id: Current RUM view ID
// - application.id: RUM application ID
```

### Viewing Logs in RUM

In Datadog RUM Explorer:
1. View a session
2. See associated logs in timeline
3. Filter logs by level, message, attributes

## Trace Integration

### Automatic Trace Correlation

When APM Tracing is enabled, logs are linked to traces:

```csharp
// Enable trace bundling
loggerConfig.BundleWithTraceEnabled = true;

// Logs now include:
// - trace_id: APM trace ID
// - span_id: Current span ID
```

## Console Output

### Console Log Formats

```csharp
// No console output
loggerConfig.ConsoleLogFormat = null;

// Short format: [LEVEL] message
loggerConfig.ConsoleLogFormat = DDConsoleLogFormat.Short;

// Short with prefix: [DD][LEVEL] message
loggerConfig.ConsoleLogFormat = DDConsoleLogFormat.ShortWith(prefix: "[DD]");

// JSON format (for log aggregators)
loggerConfig.ConsoleLogFormat = DDConsoleLogFormat.Json;
```

## Event Mapper

Filter or modify logs before sending:

```csharp
var logsConfig = new DDLogsConfiguration
{
    EventMapper = (logEvent) =>
    {
        // Drop debug logs in production
        if (logEvent.Status == DDLogStatus.Debug && IsProduction)
        {
            return null;  // Drop this log
        }

        // Redact sensitive info
        if (logEvent.Message.Contains("password"))
        {
            logEvent.Message = "*** REDACTED ***";
        }

        // Add custom attribute
        logEvent.Attributes["processed"] = new NSString("true");

        return logEvent;
    }
};

DDLogs.Enable(logsConfig);
```

## Complete Example

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;
using Datadog.iOS.Logs;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private static DDLogger? _logger;
    public static DDLogger Logger => _logger ?? throw new InvalidOperationException("Logger not initialized");

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

            // Configure logging
            var logsConfig = new DDLogsConfiguration
            {
                EventMapper = (logEvent) =>
                {
                    // Redact sensitive data
                    if (logEvent.Message.Contains("token"))
                    {
                        logEvent.Message = Regex.Replace(
                            logEvent.Message,
                            @"token=[\w-]+",
                            "token=***"
                        );
                    }
                    return logEvent;
                }
            };

            DDLogs.Enable(logsConfig);

            // Create logger
            var loggerConfig = new DDLoggerConfiguration
            {
                Service = "com.example.myapp",
                Name = "app-logger",
                NetworkInfoEnabled = true,
                BundleWithRumEnabled = true,
                BundleWithTraceEnabled = true,
                RemoteSampleRate = 100.0f
            };

            #if DEBUG
            loggerConfig.ConsoleLogFormat = DDConsoleLogFormat.ShortWith(prefix: "[DD]");
            loggerConfig.RemoteLogThreshold = DDLogLevel.Debug;
            #else
            loggerConfig.ConsoleLogFormat = null;
            loggerConfig.RemoteLogThreshold = DDLogLevel.Info;
            #endif

            _logger = DDLogger.Create(loggerConfig);

            // Add permanent attributes
            _logger.AddAttribute(new NSString("app_version"), new NSString("1.0.0"));
            _logger.AddAttribute(new NSString("device_model"), new NSString(UIDevice.CurrentDevice.Model));

            _logger.Info("Datadog logging initialized", null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Logging initialization failed: {ex.Message}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## Usage Patterns

### Application Lifecycle

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    AppDelegate.Logger.Info("Application launching", null);
    return base.FinishedLaunching(app, options);
}

public override void OnActivated(UIApplication application)
{
    AppDelegate.Logger.Info("Application activated", null);
}

public override void OnResignActivation(UIApplication application)
{
    AppDelegate.Logger.Info("Application resigned activation", null);
}
```

### Error Logging with Context

```csharp
public async Task<User> LoadUserAsync(string userId)
{
    try
    {
        var user = await userService.GetUserAsync(userId);
        logger.Info("User loaded successfully", new NSDictionary(
            new NSString("user_id"), new NSString(userId),
            new NSString("user_role"), new NSString(user.Role)
        ));
        return user;
    }
    catch (Exception ex)
    {
        logger.Error($"Failed to load user: {ex.Message}", new NSDictionary(
            new NSString("user_id"), new NSString(userId),
            new NSString("error_type"), new NSString(ex.GetType().Name),
            new NSString("stack_trace"), new NSString(ex.StackTrace)
        ));
        throw;
    }
}
```

### Feature Flag Logging

```csharp
public void CheckFeatureFlag(string featureName, bool isEnabled)
{
    logger.Info($"Feature flag evaluated: {featureName}", new NSDictionary(
        new NSString("feature_name"), new NSString(featureName),
        new NSString("is_enabled"), new NSNumber(isEnabled),
        new NSString("user_id"), new NSString(currentUserId)
    ));
}
```

### Performance Logging

```csharp
public async Task<Data> LoadDataAsync()
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var data = await dataService.FetchAsync();

    stopwatch.Stop();

    logger.Info("Data loaded", new NSDictionary(
        new NSString("duration_ms"), new NSNumber(stopwatch.ElapsedMilliseconds),
        new NSString("item_count"), new NSNumber(data.Count),
        new NSString("cache_hit"), new NSNumber(data.FromCache)
    ));

    if (stopwatch.ElapsedMilliseconds > 1000)
    {
        logger.Warn("Slow data load detected", new NSDictionary(
            new NSString("duration_ms"), new NSNumber(stopwatch.ElapsedMilliseconds)
        ));
    }

    return data;
}
```

## Best Practices

### 1. Use Appropriate Log Levels

```csharp
// Debug - development only
logger.Debug("Cache key: user_123_profile", null);

// Info - important events
logger.Info("User logged in", null);

// Warn - potential issues
logger.Warn("API response time > 2s", null);

// Error - errors that need attention
logger.Error("Payment processing failed", null);

// Critical - system failures
logger.Critical("Database unavailable", null);
```

### 2. Add Structured Attributes

```csharp
// Good - structured data
logger.Info("Purchase completed", new NSDictionary(
    new NSString("user_id"), new NSString("123"),
    new NSString("amount"), new NSNumber(99.99),
    new NSString("product_id"), new NSString("prod-456")
));

// Avoid - unstructured string concatenation
logger.Info($"User 123 purchased prod-456 for $99.99", null);
```

### 3. Use Logger Attributes for Context

```csharp
// Set once
logger.AddAttribute(new NSString("user_tier"), new NSString("premium"));

// All logs include user_tier automatically
logger.Info("Action performed", null);
```

### 4. Enable RUM/Trace Bundling

```csharp
loggerConfig.BundleWithRumEnabled = true;
loggerConfig.BundleWithTraceEnabled = true;
```

### 5. Filter Sensitive Data

```csharp
var logsConfig = new DDLogsConfiguration
{
    EventMapper = (logEvent) =>
    {
        // Redact credit card numbers
        logEvent.Message = Regex.Replace(
            logEvent.Message,
            @"\d{4}-\d{4}-\d{4}-\d{4}",
            "****-****-****-****"
        );
        return logEvent;
    }
};
```

### 6. Adjust Sample Rate for Production

```csharp
#if DEBUG
loggerConfig.RemoteSampleRate = 100.0f;  // All logs
#else
loggerConfig.RemoteSampleRate = 50.0f;   // 50% of logs
#endif
```

## Troubleshooting

### Logs Not Appearing in Datadog

1. **Check Core SDK initialized**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Verify logs enabled**:
```csharp
DDLogs.Enable(logsConfig);
```

3. **Check sample rate**:
```csharp
loggerConfig.RemoteSampleRate = 100.0f;  // Track all logs
```

4. **Verify log level threshold**:
```csharp
loggerConfig.RemoteLogThreshold = DDLogLevel.Debug;  // Send all levels
```

### Logs Missing RUM Context

Enable RUM bundling:
```csharp
loggerConfig.BundleWithRumEnabled = true;
```

Ensure RUM is enabled:
```csharp
DDRUM.Enable(rumConfig);
```

### High Log Volume

Reduce sample rate:
```csharp
loggerConfig.RemoteSampleRate = 10.0f;  // Only 10% of logs
```

Or increase threshold:
```csharp
loggerConfig.RemoteLogThreshold = DDLogLevel.Warn;  // Only warnings and above
```

## API Reference

### DDLogs

| Method | Description |
|--------|-------------|
| `Enable(DDLogsConfiguration)` | Enable logging with configuration |

### DDLogger

| Method | Description |
|--------|-------------|
| `Create()` | Create logger with default configuration |
| `Create(DDLoggerConfiguration)` | Create logger with custom configuration |
| `Debug(string, NSDictionary?)` | Log debug message |
| `Info(string, NSDictionary?)` | Log info message |
| `Notice(string, NSDictionary?)` | Log notice message |
| `Warn(string, NSDictionary?)` | Log warning message |
| `Error(string, NSDictionary?)` | Log error message |
| `Critical(string, NSDictionary?)` | Log critical message |
| `AddAttribute(NSString, NSObject)` | Add permanent attribute |
| `RemoveAttribute(string)` | Remove permanent attribute |

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogRUM](../DatadogRUM/README.md)** - Optional: Correlate logs with RUM sessions
- **[DatadogTrace](../DatadogTrace/README.md)** - Optional: Correlate logs with APM traces

## Resources

- [iOS Log Collection](https://docs.datadoghq.com/logs/log_collection/ios/)
- [Log Explorer](https://docs.datadoghq.com/logs/explorer/)
- [Log Attributes](https://docs.datadoghq.com/logs/log_configuration/attributes_naming_convention/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
