# Datadog iOS Bindings for .NET MAUI

.NET bindings for the Datadog iOS SDK, providing Real User Monitoring (RUM), logging, APM tracing, session replay, and crash reporting for iOS applications.

## Installation

Add the package via NuGet:

```bash
dotnet add package Datadog.MAUI.iOS.Binding
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.Binding" Version="3.5.0" />
```

## Modules

This meta-package includes all Datadog iOS binding modules:

| Module | Description | Package Name |
|--------|-------------|--------------|
| **DatadogCore** | Core SDK initialization and configuration | `Datadog.MAUI.iOS.Core` |
| **DatadogInternal** | Internal utilities and types | `Datadog.MAUI.iOS.Internal` |
| **DatadogRUM** | Real User Monitoring | `Datadog.MAUI.iOS.RUM` |
| **DatadogLogs** | Logging | `Datadog.MAUI.iOS.Logs` |
| **DatadogTrace** | APM Tracing | `Datadog.MAUI.iOS.Trace` |
| **DatadogCrashReporting** | Crash reports | `Datadog.MAUI.iOS.CrashReporting` |
| **DatadogSessionReplay** | Session recording | `Datadog.MAUI.iOS.SessionReplay` |
| **DatadogWebViewTracking** | WebView instrumentation | `Datadog.MAUI.iOS.WebViewTracking` |
| **DatadogFlags** | Feature flags | `Datadog.MAUI.iOS.Flags` |
| **OpenTelemetryApi** | OpenTelemetry support | `Datadog.MAUI.iOS.OpenTelemetryApi` |

## Quick Start

### 1. Initialize the SDK

In your iOS `AppDelegate.cs`:

```csharp
using Datadog.iOS;
using Foundation;
using UIKit;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Initialize Datadog
        InitializeDatadog();

        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        // Create configuration
        var config = new DDConfiguration("YOUR_CLIENT_TOKEN", "prod")
        {
            Site = DDSite.US1,
            Service = "com.example.myapp",
            BatchSize = DDBatchSize.Medium,
            UploadFrequency = DDUploadFrequency.Average
        };

        // Initialize SDK
        DDDatadog.Initialize(config, DDTrackingConsent.Granted);

        // Enable debug logging
        DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
    }
}
```

### 2. Enable RUM (Real User Monitoring)

```csharp
// Create RUM configuration
var rumConfig = new DDRUMConfiguration("YOUR_RUM_APPLICATION_ID")
{
    SessionSampleRate = 100.0f  // Sample 100% of sessions
};

// Enable automatic tracking
rumConfig.TrackUIKitViews();       // Track view controllers automatically
rumConfig.TrackUIKitActions();     // Track taps, swipes automatically
rumConfig.TrackLongTasks(0.1);     // Track tasks longer than 100ms
rumConfig.TrackBackgroundEvents(true);

// Enable RUM
DDRUM.Enable(rumConfig);
```

### 3. Enable Logging

```csharp
// Enable logging
var logsConfig = new DDLogsConfiguration();
DDLogs.Enable(logsConfig);

// Create a logger
var logger = DDLogger.Create();

// Log messages
logger.Info("Application started", null);
logger.Debug("Debug info", null);
logger.Error("An error occurred", null);
```

### 4. Enable APM Tracing

```csharp
// Enable tracing
var traceConfig = new DDTraceConfiguration
{
    SampleRate = 100.0f,
    BundleWithRumEnabled = true
};
DDTrace.Enable(traceConfig);

// Use the tracer
var tracer = DDTracer.Shared;
var span = tracer.StartSpan("database_query", null, null);
// ... perform operation ...
span.Finish();
```

### 5. Enable Session Replay

```csharp
var replayConfig = new DDSessionReplayConfiguration(100.0f)  // 100% sample rate
{
    TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs,
    ImagePrivacy = DDImagePrivacy.MaskNone,
    TouchPrivacy = DDTouchPrivacy.Show
};

DDSessionReplay.Enable(replayConfig);
```

### 6. Enable Crash Reporting

```csharp
// Enable crash reporting (integrates with RUM automatically)
DDCrashReporting.Enable();
```

## Core Features

### SDK Initialization

```csharp
// Check if SDK is initialized
if (DDDatadog.IsInitialized)
{
    Console.WriteLine("Datadog SDK is ready");
}

// Set user information
DDDatadog.SetUserInfo(
    id: "user123",
    name: "John Doe",
    email: "john@example.com",
    extraInfo: null
);

// Clear user information
DDDatadog.ClearUserInfo();

// Update tracking consent
DDDatadog.SetTrackingConsent(DDTrackingConsent.Granted);

// Clear all data
DDDatadog.ClearAllData();
```

### Configuration Options

#### Datadog Sites

```csharp
config.Site = DDSite.US1;      // US (datadoghq.com)
config.Site = DDSite.US3;      // US3 (us3.datadoghq.com)
config.Site = DDSite.US5;      // US5 (us5.datadoghq.com)
config.Site = DDSite.EU1;      // EU (datadoghq.eu)
config.Site = DDSite.US1_FED;  // FedRAMP (ddog-gov.com)
config.Site = DDSite.AP1;      // AP1 (ap1.datadoghq.com)
```

#### Batch Size and Upload Frequency

```csharp
// Batch size - how much data to batch before uploading
config.BatchSize = DDBatchSize.Small;   // Upload more frequently, less data
config.BatchSize = DDBatchSize.Medium;  // Balanced (default)
config.BatchSize = DDBatchSize.Large;   // Upload less frequently, more data

// Upload frequency
config.UploadFrequency = DDUploadFrequency.Frequent;  // Upload more often
config.UploadFrequency = DDUploadFrequency.Average;   // Balanced (default)
config.UploadFrequency = DDUploadFrequency.Rare;      // Conserve battery/bandwidth
```

#### Tracking Consent

```csharp
// Grant consent - SDK collects and sends data
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Pending - SDK buffers data locally until consent is granted
DDDatadog.Initialize(config, DDTrackingConsent.Pending);

// Not granted - SDK doesn't collect or send data
DDDatadog.Initialize(config, DDTrackingConsent.NotGranted);

// Update consent later
DDDatadog.SetTrackingConsent(DDTrackingConsent.Granted);
```

## RUM (Real User Monitoring)

### Manual View Tracking

```csharp
var monitor = DDRUMMonitor.Shared;

// Start tracking a view
monitor.StartView(
    key: "HomeViewController",
    name: "Home Screen",
    attributes: null
);

// Stop tracking when view disappears
monitor.StopView("HomeViewController", null);
```

### Manual Action Tracking

```csharp
// Track a tap
monitor.AddAction(
    type: DDRUMActionType.Tap,
    name: "Submit Button",
    attributes: new NSDictionary("button_id", "submit")
);

// Track a custom action
monitor.AddAction(
    type: DDRUMActionType.Custom,
    name: "User Completed Onboarding",
    attributes: null
);

// Track a continuous action
monitor.StartAction(DDRUMActionType.Scroll, "Product List", null);
// ... user scrolls ...
monitor.StopAction(DDRUMActionType.Scroll, "Product List", null);
```

### Error Tracking

```csharp
// Track an error
monitor.AddError(
    message: "Failed to load data",
    source: DDRUMErrorSource.Network,
    stack: exception.StackTrace,
    attributes: null
);

// Track from NSError
try
{
    // ... code that might fail ...
}
catch (Exception ex)
{
    var nsError = new NSError(new NSString("ErrorDomain"), 500);
    monitor.AddError(nsError, DDRUMErrorSource.Source, null);
}
```

### Resource Tracking (Network Requests)

```csharp
// Start tracking a network request
monitor.StartResource(
    resourceKey: "api-call-1",
    httpMethod: "GET",
    urlString: "https://api.example.com/users",
    attributes: null
);

// On success
monitor.StopResource(
    resourceKey: "api-call-1",
    statusCode: 200,
    size: 1024,
    attributes: null
);

// On failure
monitor.StopResourceWithError(
    resourceKey: "api-call-1",
    error: new NSError(new NSString("Network"), 500),
    attributes: null
);
```

### Global Attributes

```csharp
// Add a global attribute to all RUM events
monitor.AddAttribute(new NSString("environment"), "production");
monitor.AddAttribute(new NSString("version"), "1.2.3");

// Remove a global attribute
monitor.RemoveAttribute("environment");
```

### Session Management

```csharp
// Get current session ID
monitor.GetCurrentSessionId((sessionId) =>
{
    if (sessionId != null)
    {
        Console.WriteLine($"Session ID: {sessionId}");
    }
});
```

## Logging

### Creating Loggers

```csharp
// Default logger
var logger = DDLogger.Create();

// Custom logger
var loggerConfig = new DDLoggerConfiguration
{
    Service = "my-service",
    Name = "my-logger",
    NetworkInfoEnabled = true,
    BundleWithRumEnabled = true,
    RemoteSampleRate = 100.0f
};
var customLogger = DDLogger.Create(loggerConfig);
```

### Log Levels

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

### Log Attributes

```csharp
// Log with custom attributes
var attributes = new NSDictionary(
    new NSString("user_id"), new NSString("123"),
    new NSString("action"), new NSString("purchase"),
    new NSString("amount"), new NSNumber(99.99)
);

logger.Info("User completed purchase", attributes);

// Add permanent attributes to logger
logger.AddAttribute(new NSString("app_version"), "1.2.3");
logger.AddAttribute(new NSString("build"), "42");

// Remove attribute
logger.RemoveAttribute("build");
```

## APM Tracing

### Creating Spans

```csharp
var tracer = DDTracer.Shared;

// Start a span
var span = tracer.StartSpan("database_query", null, null);

// Add tags
span.SetTag("db.type", new NSString("postgresql"));
span.SetTag("db.instance", new NSString("users_db"));

// Log events
var logFields = new NSDictionary(
    new NSString("event"), new NSString("query_executed"),
    new NSString("rows"), new NSNumber(42)
);
span.Log(logFields);

// Finish the span
span.Finish();
```

### Tracing Network Requests

```csharp
// Trace an API call
var span = tracer.StartSpan("api_request", null, null);
span.SetTag("http.method", new NSString("GET"));
span.SetTag("http.url", new NSString("https://api.example.com/users"));

try
{
    // ... make HTTP request ...
    span.SetTag("http.status_code", new NSNumber(200));
}
catch (Exception ex)
{
    span.SetTag("error", new NSNumber(true));
    span.SetTag("error.message", new NSString(ex.Message));
}
finally
{
    span.Finish();
}
```

## Session Replay

### Privacy Levels

```csharp
// Text and Input Privacy
config.SetTextAndInputPrivacy(DDTextAndInputPrivacy.MaskAll);              // Mask all text
config.SetTextAndInputPrivacy(DDTextAndInputPrivacy.MaskSensitiveInputs);  // Mask passwords, etc.
config.SetTextAndInputPrivacy(DDTextAndInputPrivacy.AllowAll);             // Show all text

// Image Privacy
config.SetImagePrivacy(DDImagePrivacy.MaskAll);          // Mask all images
config.SetImagePrivacy(DDImagePrivacy.MaskNonBundled);   // Mask external images only
config.SetImagePrivacy(DDImagePrivacy.MaskNone);         // Show all images

// Touch Privacy
config.SetTouchPrivacy(DDTouchPrivacy.Hide);  // Hide touch indicators
config.SetTouchPrivacy(DDTouchPrivacy.Show);  // Show touch indicators
```

## WebView Tracking

### Enable WebView Tracking

```csharp
using WebKit;

// Enable tracking for a WebView
var webView = new WKWebView();
DDWebViewTracking.Enable(
    webView: webView,
    hosts: new string[] { "example.com", "api.example.com" }
);

// Disable tracking
DDWebViewTracking.Disable(webView);
```

## Best Practices

### 1. Initialize Early

Initialize Datadog in `AppDelegate.FinishedLaunching` before any other SDK calls:

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    InitializeDatadog();  // First thing
    return base.FinishedLaunching(app, options);
}
```

### 2. Use Automatic Tracking

Enable automatic tracking for views and actions to minimize manual instrumentation:

```csharp
rumConfig.TrackUIKitViews();
rumConfig.TrackUIKitActions();
```

### 3. Set User Information

Set user info after authentication for better debugging:

```csharp
DDDatadog.SetUserInfo(
    id: userId,
    name: userName,
    email: userEmail,
    extraInfo: null
);
```

### 4. Bundle RUM with Logs and Traces

Enable `BundleWithRumEnabled` to correlate logs and traces with RUM sessions:

```csharp
loggerConfig.BundleWithRumEnabled = true;
traceConfig.BundleWithRumEnabled = true;
```

### 5. Sample Rate for Production

Use 100% sample rate for development, lower for production:

```csharp
#if DEBUG
rumConfig.SessionSampleRate = 100.0f;  // All sessions
#else
rumConfig.SessionSampleRate = 20.0f;   // 20% of sessions
#endif
```

### 6. Handle Privacy Correctly

Set appropriate privacy levels for session replay:

```csharp
replayConfig.SetTextAndInputPrivacy(DDTextAndInputPrivacy.MaskSensitiveInputs);
replayConfig.SetImagePrivacy(DDImagePrivacy.MaskNonBundled);
```

## Troubleshooting

### SDK Not Sending Data

1. **Check initialization**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("SDK not initialized!");
}
```

2. **Enable verbose logging**:
```csharp
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
```

3. **Verify tracking consent**:
```csharp
DDDatadog.SetTrackingConsent(DDTrackingConsent.Granted);
```

### Data Not Appearing in Datadog

- Verify client token and application ID are correct
- Check the Datadog site matches your account (US1, EU1, etc.)
- Ensure network connectivity
- Check for firewall/proxy blocking requests to Datadog

### High Data Usage

- Reduce sample rates:
```csharp
rumConfig.SessionSampleRate = 20.0f;  // Sample 20% instead of 100%
traceConfig.SampleRate = 20.0f;
```

- Adjust batch size and upload frequency:
```csharp
config.BatchSize = DDBatchSize.Large;
config.UploadFrequency = DDUploadFrequency.Rare;
```

## API Reference

For detailed API documentation, see the individual module READMEs:

- [DatadogCore README](DatadogCore/README.md) - Core SDK APIs
- [DatadogRUM README](DatadogRUM/README.md) - RUM monitoring APIs
- [DatadogLogs README](DatadogLogs/README.md) - Logging APIs
- [DatadogTrace README](DatadogTrace/README.md) - APM tracing APIs
- [DatadogSessionReplay README](DatadogSessionReplay/README.md) - Session replay APIs
- [DatadogCrashReporting README](DatadogCrashReporting/README.md) - Crash reporting APIs
- [DatadogWebViewTracking README](DatadogWebViewTracking/README.md) - WebView tracking APIs

## Resources

- [Datadog iOS SDK Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [Datadog RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)
- [Issue Tracker](https://github.com/DataDog/dd-sdk-maui/issues)

## License

This binding library is licensed under Apache 2.0. See [LICENSE](../LICENSE) for details.

The Datadog iOS SDK is copyright Datadog, Inc.
