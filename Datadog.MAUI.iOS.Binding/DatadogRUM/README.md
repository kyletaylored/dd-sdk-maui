# Datadog iOS RUM (Real User Monitoring) - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS RUM. Track user sessions, views, actions, resources, and errors automatically in your iOS application.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.RUM
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.RUM" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` to be initialized first.

## Overview

RUM (Real User Monitoring) provides:
- **Automatic session tracking** across app launches
- **View tracking** for screens and view controllers
- **Action tracking** for user interactions (taps, swipes, scrolls)
- **Resource tracking** for network requests
- **Error tracking** with stack traces
- **Performance monitoring** (long tasks, frozen frames)
- **User journey analysis** with session replays

## Quick Start

### Enable RUM

In your `AppDelegate.cs` after Core SDK initialization:

```csharp
using Datadog.iOS.Core;
using Datadog.iOS.RUM;

// Initialize Core first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Create RUM configuration
var rumConfig = new DDRUMConfiguration(applicationId: "YOUR_RUM_APPLICATION_ID");
rumConfig.SessionSampleRate = 100.0f;  // Sample 100% of sessions

// Enable automatic tracking
rumConfig.TrackUIKitViews();           // Track view controllers automatically
rumConfig.TrackUIKitActions();         // Track taps, swipes automatically
rumConfig.TrackLongTasks(0.1);         // Track tasks longer than 100ms
rumConfig.TrackBackgroundEvents(true); // Track events in background

// Enable RUM
DDRUM.Enable(rumConfig);

Console.WriteLine("[Datadog] RUM enabled");
```

## Automatic Tracking

### View Tracking

Automatically track UIViewControllers:

```csharp
// In RUM configuration
rumConfig.TrackUIKitViews();

// Views are now tracked automatically when:
// - View controller appears
// - View controller disappears
// - View controller name changes
```

### Action Tracking

Automatically track user interactions:

```csharp
// In RUM configuration
rumConfig.TrackUIKitActions();

// Actions tracked automatically:
// - Taps on buttons, controls
// - Swipes, scrolls
// - Long presses
// - Text field editing
```

### Long Task Tracking

Track operations that block the main thread:

```csharp
// Track tasks longer than 100ms (0.1 seconds)
rumConfig.TrackLongTasks(0.1);

// Long tasks indicate performance issues:
// - Heavy computation on main thread
// - Synchronous I/O
// - Excessive rendering
```

### Background Events

Track events when app is in background:

```csharp
rumConfig.TrackBackgroundEvents(true);
```

### Frustration Signals

Track user frustration (error taps, rage clicks):

```csharp
rumConfig.TrackFrustrations(true);

// Detects:
// - Error taps (tap on element that causes error)
// - Rage taps (multiple rapid taps)
// - Dead taps (tap with no response)
```

## Manual Tracking

### RUM Monitor

Access the global RUM monitor:

```csharp
var monitor = DDRUMMonitor.Shared;
```

### View Tracking

Manually track views:

```csharp
// Start tracking a view
monitor.StartView(
    key: "HomeViewController",
    name: "Home Screen",
    attributes: null
);

// Stop tracking when view disappears
monitor.StopView(
    key: "HomeViewController",
    attributes: null
);
```

### Action Tracking

Manually track user actions:

```csharp
// Track a tap
monitor.AddAction(
    type: DDRUMActionType.Tap,
    name: "Submit Button",
    attributes: new NSDictionary(
        new NSString("button_id"), new NSString("submit")
    )
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

### Resource Tracking (Network Requests)

Manually track network requests:

```csharp
// Start tracking a network request
monitor.StartResource(
    resourceKey: "api-call-1",
    httpMethod: DDRUMHTTPMethod.Get,
    urlString: "https://api.example.com/users",
    attributes: null
);

// On success
monitor.StopResource(
    resourceKey: "api-call-1",
    statusCode: 200,
    kind: DDRUMResourceType.Xhr,
    size: 1024,
    attributes: null
);

// On failure
monitor.StopResourceWithError(
    resourceKey: "api-call-1",
    message: "Network timeout",
    source: DDRUMErrorSource.Network,
    attributes: null
);
```

### Error Tracking

Manually track errors:

```csharp
// Track an error
monitor.AddError(
    message: "Failed to load data",
    source: DDRUMErrorSource.Source,
    stack: exception.StackTrace,
    attributes: new NSDictionary(
        new NSString("error_code"), new NSString("500")
    )
);

// Track from NSError
try
{
    // ... code that might fail ...
}
catch (Exception ex)
{
    var nsError = new NSError(
        new NSString("ErrorDomain"),
        500,
        new NSDictionary(
            new NSString(NSError.LocalizedDescriptionKey),
            new NSString(ex.Message)
        )
    );
    monitor.AddError(nsError, DDRUMErrorSource.Source, null);
}
```

## Configuration Options

### Sample Rate

Control what percentage of sessions to track:

```csharp
// Track 100% of sessions (development)
rumConfig.SessionSampleRate = 100.0f;

// Track 20% of sessions (production - cost control)
rumConfig.SessionSampleRate = 20.0f;

// Don't track any sessions
rumConfig.SessionSampleRate = 0.0f;
```

### Telemetry Sample Rate

Control SDK telemetry (internal errors):

```csharp
// Report 100% of SDK telemetry
rumConfig.TelemetrySampleRate = 100.0f;

// Report 10% of SDK telemetry (reduce noise)
rumConfig.TelemetrySampleRate = 10.0f;
```

### Bundle with RUM

Link logs and traces to RUM sessions:

```csharp
rumConfig.BundleWithRumEnabled = true;
```

### Vitals Update Frequency

Control how often to collect performance vitals:

```csharp
rumConfig.VitalsUpdateFrequency = DDRUMVitalsFrequency.Average;  // Default
rumConfig.VitalsUpdateFrequency = DDRUMVitalsFrequency.Frequent; // More frequent
rumConfig.VitalsUpdateFrequency = DDRUMVitalsFrequency.Rare;     // Less frequent
rumConfig.VitalsUpdateFrequency = DDRUMVitalsFrequency.Never;    // Disable vitals
```

## Global Attributes

Add attributes to all RUM events:

```csharp
var monitor = DDRUMMonitor.Shared;

// Add global attributes
monitor.AddAttribute(new NSString("environment"), new NSString("production"));
monitor.AddAttribute(new NSString("version"), new NSString("1.2.3"));
monitor.AddAttribute(new NSString("tier"), new NSString("premium"));

// Remove attribute
monitor.RemoveAttribute("tier");
```

## Session Management

### Get Current Session ID

```csharp
monitor.GetCurrentSessionId((sessionId) =>
{
    if (sessionId != null)
    {
        Console.WriteLine($"Session ID: {sessionId}");
    }
    else
    {
        Console.WriteLine("No active RUM session");
    }
});
```

### Stop Session

```csharp
// Stop current session (new session starts on next event)
monitor.StopSession();
```

## Enums and Constants

### DDRUMActionType

Types of user actions:

```csharp
public enum DDRUMActionType
{
    Tap,        // User tapped on element
    Scroll,     // User scrolled
    Swipe,      // User swiped
    Click,      // Mouse click (iPad)
    Custom      // Custom action
}
```

### DDRUMErrorSource

Source of errors:

```csharp
public enum DDRUMErrorSource
{
    Source,     // Application code error
    Network,    // Network error
    WebView,    // WebView error
    Console,    // Console error (JavaScript)
    Custom      // Custom error
}
```

### DDRUMHTTPMethod

HTTP methods:

```csharp
public enum DDRUMHTTPMethod
{
    Get,
    Post,
    Put,
    Delete,
    Head,
    Patch
}
```

### DDRUMResourceType

Types of resources:

```csharp
public enum DDRUMResourceType
{
    Image,      // Image resource
    Xhr,        // XMLHttpRequest or Fetch
    Beacon,     // Beacon API
    Css,        // CSS file
    Document,   // HTML document
    Fetch,      // Fetch API
    Font,       // Font file
    Js,         // JavaScript file
    Media,      // Audio/video
    Other,      // Other resource
    Native      // Native network request
}
```

### DDRUMVitalsFrequency

Vitals collection frequency:

```csharp
public enum DDRUMVitalsFrequency
{
    Frequent,   // Collect vitals frequently
    Average,    // Balanced (default)
    Rare,       // Collect rarely
    Never       // Don't collect vitals
}
```

## Complete Example

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private const string CLIENT_TOKEN = "YOUR_CLIENT_TOKEN";
    private const string RUM_APPLICATION_ID = "YOUR_RUM_APP_ID";
    private const string ENVIRONMENT = "prod";

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
            var config = new DDConfiguration(CLIENT_TOKEN, ENVIRONMENT);
            config.Service = "com.example.myapp";
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            // Configure RUM
            var rumConfig = new DDRUMConfiguration(RUM_APPLICATION_ID);

            #if DEBUG
            rumConfig.SessionSampleRate = 100.0f;  // Track all sessions in debug
            #else
            rumConfig.SessionSampleRate = 20.0f;   // Track 20% in production
            #endif

            // Enable automatic tracking
            rumConfig.TrackUIKitViews();
            rumConfig.TrackUIKitActions();
            rumConfig.TrackLongTasks(0.1);
            rumConfig.TrackBackgroundEvents(true);
            rumConfig.TrackFrustrations(true);

            // Performance settings
            rumConfig.VitalsUpdateFrequency = DDRUMVitalsFrequency.Average;
            rumConfig.TelemetrySampleRate = 100.0f;
            rumConfig.BundleWithRumEnabled = true;

            // Enable RUM
            DDRUM.Enable(rumConfig);

            Console.WriteLine("[Datadog] RUM enabled");

            // Set global attributes
            var monitor = DDRUMMonitor.Shared;
            monitor.AddAttribute(new NSString("app_version"), new NSString("1.0.0"));
            monitor.AddAttribute(new NSString("build"), new NSString("42"));

            Console.WriteLine("[Datadog] RUM configured successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] RUM initialization failed: {ex.Message}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## MAUI Integration

### Track MAUI Pages

```csharp
public class BasePage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Track view
        var monitor = DDRUMMonitor.Shared;
        monitor.StartView(
            key: GetType().Name,
            name: Title ?? GetType().Name,
            attributes: null
        );
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop tracking view
        var monitor = DDRUMMonitor.Shared;
        monitor.StopView(
            key: GetType().Name,
            attributes: null
        );
    }
}
```

### Track Button Clicks

```csharp
var button = new Button { Text = "Submit" };
button.Clicked += (sender, e) =>
{
    // Track action
    var monitor = DDRUMMonitor.Shared;
    monitor.AddAction(
        type: DDRUMActionType.Tap,
        name: "Submit Button Clicked",
        attributes: null
    );

    // Your button logic
    SubmitForm();
};
```

### Track API Calls with HttpClient

```csharp
public async Task<string> FetchDataAsync(string url)
{
    var monitor = DDRUMMonitor.Shared;
    var resourceKey = $"api-{Guid.NewGuid()}";

    // Start tracking
    monitor.StartResource(
        resourceKey: resourceKey,
        httpMethod: DDRUMHTTPMethod.Get,
        urlString: url,
        attributes: null
    );

    try
    {
        var response = await httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        // Stop with success
        monitor.StopResource(
            resourceKey: resourceKey,
            statusCode: (int)response.StatusCode,
            kind: DDRUMResourceType.Xhr,
            size: content.Length,
            attributes: null
        );

        return content;
    }
    catch (Exception ex)
    {
        // Stop with error
        monitor.StopResourceWithError(
            resourceKey: resourceKey,
            message: ex.Message,
            source: DDRUMErrorSource.Network,
            attributes: null
        );

        throw;
    }
}
```

## Best Practices

### 1. Enable Automatic Tracking

Use automatic tracking to minimize manual instrumentation:

```csharp
rumConfig.TrackUIKitViews();
rumConfig.TrackUIKitActions();
```

### 2. Set Appropriate Sample Rates

Use different sample rates for dev vs production:

```csharp
#if DEBUG
rumConfig.SessionSampleRate = 100.0f;
#else
rumConfig.SessionSampleRate = 20.0f;
#endif
```

### 3. Use Global Attributes

Set attributes once instead of on every event:

```csharp
monitor.AddAttribute(new NSString("user_tier"), new NSString("premium"));
```

### 4. Bundle with Logs and Traces

Enable bundling for correlated debugging:

```csharp
rumConfig.BundleWithRumEnabled = true;
```

### 5. Track Critical User Journeys

Manually track important flows:

```csharp
// Purchase flow
monitor.StartView("CheckoutFlow", "Checkout", null);
monitor.AddAction(DDRUMActionType.Custom, "Begin Checkout", null);
// ... track each step ...
monitor.AddAction(DDRUMActionType.Custom, "Purchase Complete", null);
monitor.StopView("CheckoutFlow", null);
```

### 6. Handle Errors Gracefully

Always track errors with context:

```csharp
try
{
    await ProcessPayment();
}
catch (Exception ex)
{
    monitor.AddError(
        message: ex.Message,
        source: DDRUMErrorSource.Source,
        stack: ex.StackTrace,
        attributes: new NSDictionary(
            new NSString("payment_method"), new NSString("credit_card"),
            new NSString("amount"), new NSNumber(99.99)
        )
    );
    throw;
}
```

## Troubleshooting

### RUM Data Not Appearing

1. **Check Core SDK initialized**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Verify RUM application ID** matches Datadog dashboard

3. **Check sample rate**:
```csharp
rumConfig.SessionSampleRate = 100.0f;  // Track all sessions for testing
```

4. **Enable verbose logging**:
```csharp
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
```

### Views Not Tracked

1. **Enable automatic tracking**:
```csharp
rumConfig.TrackUIKitViews();
```

2. **Or track manually** in view lifecycle:
```csharp
override func viewDidAppear(_ animated: Bool) {
    super.viewDidAppear(animated)
    monitor.startView(key: "MyView", name: "My View", attributes: [:])
}
```

### Actions Not Tracked

1. **Enable automatic tracking**:
```csharp
rumConfig.TrackUIKitActions();
```

2. **Or track manually** on button clicks

### High Data Usage

Reduce sample rate in production:
```csharp
rumConfig.SessionSampleRate = 10.0f;  // Only 10% of sessions
```

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogLogs](../DatadogLogs/README.md)** - Optional: Log correlation with RUM
- **[DatadogTrace](../DatadogTrace/README.md)** - Optional: Trace correlation with RUM
- **[DatadogSessionReplay](../DatadogSessionReplay/README.md)** - Optional: Visual session replays
- **[DatadogCrashReporting](../DatadogCrashReporting/README.md)** - Optional: Crash reports in RUM

## Resources

- [iOS RUM Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [RUM Best Practices](https://docs.datadoghq.com/real_user_monitoring/guide/best-practices/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
