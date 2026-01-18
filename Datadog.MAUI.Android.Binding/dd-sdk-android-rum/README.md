# Datadog Android RUM SDK - .NET MAUI Binding

.NET MAUI bindings for Datadog Android RUM (Real User Monitoring). Track user sessions, views, actions, resources, and errors in your Android applications.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.RUM
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.RUM" Version="3.5.0" />
```

**Note**: This package requires `Datadog.MAUI.Android.Core` to be initialized first.

## Overview

RUM provides:
- **Automatic tracking** of views, user actions, and network requests
- **Manual tracking** for custom events
- **Performance monitoring** (long tasks, ANRs, frame drops)
- **Error tracking** with stack traces
- **Session replay** integration
- **User frustration signals** (rage taps, error taps)

## Quick Start

### 1. Enable RUM

In your `MainApplication.cs` after Core SDK initialization:

```csharp
using Com.Datadog.Android.Rum;

// Create RUM configuration
var rumConfiguration = new RumConfiguration.Builder(
    applicationId: "YOUR_RUM_APPLICATION_ID"  // From Datadog dashboard
)
.TrackUserInteractions()           // Track taps, swipes automatically
.TrackLongTasks()                  // Track tasks blocking UI thread
.TrackFrustrations(true)           // Track rage taps, error taps
.TrackBackgroundEvents(true)       // Track events when app backgrounded
.TrackNonFatalAnrs(true)           // Track "Application Not Responding"
.SetTelemetrySampleRate(100f)      // Sample 100% of sessions
.Build();

// Enable RUM
Rum.Enable(rumConfiguration);

Console.WriteLine("[Datadog] RUM enabled");
```

### 2. Access RUM Monitor

```csharp
// Get global monitor instance
var monitor = GlobalRumMonitor.Instance;

// Or use Get() method
var monitor = GlobalRumMonitor.Get();
```

That's it! RUM will automatically track:
- ✅ Activity/Fragment views
- ✅ User taps and gestures
- ✅ Network requests (to first-party hosts)
- ✅ Crashes and ANRs
- ✅ Performance metrics

## Configuration

### RUM Configuration Builder

```csharp
var rumConfig = new RumConfiguration.Builder(applicationId)
    // Automatic tracking
    .TrackUserInteractions()              // Enable touch tracking
    .TrackUserInteractions(touchTargetExtraAttributesProviders)  // With custom attributes

    // Performance monitoring
    .TrackLongTasks()                     // Track long tasks (100ms+ threshold)
    .TrackLongTasks(durationThreshold)    // Custom threshold in milliseconds
    .TrackNonFatalAnrs(true)              // Track ANRs (Application Not Responding)

    // User experience
    .TrackFrustrations(true)              // Track rage taps, error taps
    .TrackBackgroundEvents(true)          // Track events when app backgrounded

    // Sampling
    .SetSessionSampleRate(100f)           // 0-100, percentage of sessions to track
    .SetTelemetrySampleRate(100f)         // 0-100, percentage of internal telemetry

    // Custom tracking
    .SetViewTrackingStrategy(strategy)    // Custom view tracking strategy
    .SetVitalUpdateFrequency(frequency)   // How often to sample performance vitals

    .Build();
```

### Tracking Strategies

#### Automatic Activity/Fragment Tracking

By default, RUM tracks Activities and Fragments. Customize with tracking strategies:

```csharp
using Com.Datadog.Android.Rum.Tracking;

// Track all activities and fragments
var strategy = new ActivityViewTrackingStrategy(true);
rumConfig.SetViewTrackingStrategy(strategy);

// Track activities only (no fragments)
var strategy = new ActivityViewTrackingStrategy(false);
rumConfig.SetViewTrackingStrategy(strategy);

// Track fragments only
var strategy = new FragmentViewTrackingStrategy(true);
rumConfig.SetViewTrackingStrategy(strategy);

// Custom tracking - implement your own
var strategy = new MyCustomViewTrackingStrategy();
rumConfig.SetViewTrackingStrategy(strategy);
```

### Sample Rates

Control data volume with sample rates:

```csharp
// Development - track everything
rumConfig.SetSessionSampleRate(100f);
rumConfig.SetTelemetrySampleRate(100f);

// Production - sample subset of sessions
rumConfig.SetSessionSampleRate(20f);  // 20% of sessions
rumConfig.SetTelemetrySampleRate(20f);
```

**Session Sample Rate**: Percentage of user sessions to track (0-100)
**Telemetry Sample Rate**: Percentage of SDK internal telemetry to send (0-100)

## Manual Tracking

### Views

Track custom views manually:

```csharp
var monitor = GlobalRumMonitor.Get();

// Start tracking a view
monitor.StartView(
    key: "ProductDetailView",  // Unique identifier
    name: "Product Details",   // Human-readable name
    attributes: new Dictionary<string, Java.Lang.Object>
    {
        { "product_id", "12345" },
        { "category", "electronics" }
    }
);

// Stop tracking when view closes
monitor.StopView(
    key: "ProductDetailView",
    attributes: new Dictionary<string, Java.Lang.Object>
    {
        { "time_spent_seconds", 45 }
    }
);
```

### Actions (User Interactions)

Track custom user actions:

```csharp
// Track a tap/click
monitor.AddAction(
    type: RumActionType.Tap,
    name: "Add to Cart",
    attributes: new Dictionary<string, Java.Lang.Object>
    {
        { "product_id", "12345" },
        { "quantity", 1 },
        { "price", 99.99 }
    }
);

// Track other action types
monitor.AddAction(RumActionType.Scroll, "Product List Scrolled", attributes);
monitor.AddAction(RumActionType.Swipe, "Image Gallery Swiped", attributes);
monitor.AddAction(RumActionType.Click, "Buy Now Clicked", attributes);
monitor.AddAction(RumActionType.Custom, "Checkout Completed", attributes);
```

### Errors

Track errors and exceptions:

```csharp
// Track from C# exception
try
{
    // ... code that might throw ...
}
catch (Exception ex)
{
    monitor.AddError(
        message: ex.Message,
        source: RumErrorSource.Source,  // Source code error
        stacktrace: ex.StackTrace,
        attributes: new Dictionary<string, Java.Lang.Object>
        {
            { "error_type", ex.GetType().Name },
            { "user_action", "checkout" }
        }
    );
}

// Track with Java throwable
monitor.AddError(
    message: "Payment processing failed",
    source: RumErrorSource.Network,  // Network error
    throwable: javaException,
    attributes: attributes
);

// Track with custom error details
monitor.AddErrorWithStacktrace(
    message: "Custom error occurred",
    source: RumErrorSource.Custom,
    stacktrace: "Custom stack trace...",
    attributes: attributes
);
```

**Error Sources**:
- `RumErrorSource.Source` - Application source code errors
- `RumErrorSource.Network` - Network request errors
- `RumErrorSource.WebView` - WebView errors
- `RumErrorSource.Console` - Console log errors
- `RumErrorSource.Custom` - Custom errors

### Resources (Network Requests)

Track network requests manually (automatic tracking is usually sufficient):

```csharp
// Start tracking a request
monitor.StartResource(
    key: "api-user-profile",        // Unique identifier
    method: RumResourceMethod.Get,  // HTTP method
    url: "https://api.example.com/user/profile",
    attributes: new Dictionary<string, Java.Lang.Object>
    {
        { "user_id", "12345" }
    }
);

// On successful response
monitor.StopResource(
    key: "api-user-profile",
    statusCode: 200,                // HTTP status code
    size: 2048,                     // Response size in bytes
    kind: RumResourceKind.Xhr,      // Resource type
    attributes: new Dictionary<string, Java.Lang.Object>
    {
        { "cache_hit", false }
    }
);

// On request failure
monitor.StopResourceWithError(
    key: "api-user-profile",
    statusCode: 500,
    message: "Internal Server Error",
    source: RumErrorSource.Network,
    throwable: exception
);

// On request error without HTTP response
monitor.StopResourceWithError(
    key: "api-user-profile",
    message: "Network timeout",
    source: RumErrorSource.Network,
    throwable: exception,
    attributes: attributes
);
```

**Resource Types (RumResourceKind)**:
- `Xhr` - XMLHttpRequest / Fetch API
- `Document` - HTML document
- `Image` - Image resource
- `Js` - JavaScript file
- `Font` - Font file
- `Css` - CSS stylesheet
- `Media` - Audio/video
- `Other` - Other resource types
- `Native` - Native platform resource

### Global Attributes

Add attributes to all RUM events:

```csharp
// Add global attribute
monitor.AddAttribute("app_version", "1.2.3");
monitor.AddAttribute("user_tier", "premium");
monitor.AddAttribute("ab_test_variant", "B");

// Remove global attribute
monitor.RemoveAttribute("ab_test_variant");
```

### Feature Flags

Track feature flag evaluations:

```csharp
monitor.AddFeatureFlagEvaluation(
    name: "new_checkout_flow",
    value: true
);

monitor.AddFeatureFlagEvaluation(
    name: "discount_percentage",
    value: 15
);
```

## Performance Monitoring

### Long Tasks

Automatically tracked when enabled:

```csharp
// Track tasks taking longer than 100ms (default)
rumConfig.TrackLongTasks();

// Custom threshold (in milliseconds)
rumConfig.TrackLongTasks(durationThreshold: 200);  // 200ms
```

Long tasks indicate UI thread blocking and poor performance.

### ANRs (Application Not Responding)

Track non-fatal ANRs:

```csharp
rumConfig.TrackNonFatalAnrs(true);
```

ANRs occur when the UI thread is blocked for too long (typically 5+ seconds).

### User Frustrations

Track user frustration signals:

```csharp
rumConfig.TrackFrustrations(true);
```

Detects:
- **Rage Taps**: Multiple rapid taps in same area (user frustrated)
- **Error Taps**: Taps immediately followed by errors (broken UI)

## Complete Example

```csharp
using Android.App;
using Com.Datadog.Android.Rum;

[Application]
public class MainApplication : MauiApplication
{
    private const string RUM_APPLICATION_ID = "YOUR_RUM_APP_ID";

    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize Core SDK first
        InitializeDatadogCore();

        // Then enable RUM
        InitializeRUM();
    }

    private void InitializeRUM()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling RUM...");

            // Configure RUM
            var rumConfig = new RumConfiguration.Builder(RUM_APPLICATION_ID)
                // Automatic tracking
                .TrackUserInteractions()
                .TrackLongTasks(100)  // 100ms threshold
                .TrackFrustrations(true)
                .TrackBackgroundEvents(true)
                .TrackNonFatalAnrs(true)

                // Sampling
                #if DEBUG
                .SetSessionSampleRate(100f)  // Track all sessions in debug
                .SetTelemetrySampleRate(100f)
                #else
                .SetSessionSampleRate(20f)   // 20% in production
                .SetTelemetrySampleRate(20f)
                #endif

                .Build();

            // Enable RUM
            Rum.Enable(rumConfig, Com.Datadog.Android.Datadog.Instance);

            // Get monitor instance
            var monitor = GlobalRumMonitor.Get();

            // Add global attributes
            monitor.AddAttribute("app_version", GetAppVersion());
            monitor.AddAttribute("build_number", GetBuildNumber());

            Console.WriteLine("[Datadog] RUM enabled successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] RUM initialization failed: {ex.Message}");
        }
    }

    private string GetAppVersion()
    {
        var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
        return packageInfo.VersionName;
    }

    private int GetBuildNumber()
    {
        var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
        return (int)packageInfo.LongVersionCode;
    }
}
```

## Activity/Fragment Tracking

### Manual Activity Tracking

```csharp
using Android.App;
using Com.Datadog.Android.Rum;

[Activity(Label = "Product Details")]
public class ProductDetailActivity : Activity
{
    private IGlobalRumMonitor _rumMonitor;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _rumMonitor = GlobalRumMonitor.Get();

        // Start tracking this view
        _rumMonitor.StartView(
            key: this.GetType().Name,
            name: "Product Details",
            attributes: new Dictionary<string, Java.Lang.Object>
            {
                { "product_id", Intent.GetStringExtra("product_id") }
            }
        );
    }

    protected override void OnDestroy()
    {
        // Stop tracking when activity is destroyed
        _rumMonitor.StopView(this.GetType().Name, null);
        base.OnDestroy();
    }
}
```

### Custom View Names

Customize how views appear in RUM dashboard:

```csharp
// Instead of "MainActivity"
_rumMonitor.StartView("MainActivity", "Home Screen", attributes);

// Instead of "ProductDetailActivity"
_rumMonitor.StartView("ProductDetailActivity", "Product: iPhone 15", attributes);
```

## Best Practices

### 1. Enable Automatic Tracking

Use automatic tracking whenever possible:

```csharp
rumConfig.TrackUserInteractions()
    .TrackLongTasks()
    .TrackFrustrations(true);
```

### 2. Set Meaningful View Names

Use human-readable names:

```csharp
// Good
monitor.StartView("checkout", "Checkout - Step 2", attributes);

// Avoid
monitor.StartView("activity_checkout_step2", "activity_checkout_step2", attributes);
```

### 3. Add Business Context

Include relevant business attributes:

```csharp
monitor.AddAction(RumActionType.Tap, "Add to Cart", new Dictionary<string, Java.Lang.Object>
{
    { "product_id", productId },
    { "product_name", productName },
    { "product_price", price },
    { "product_category", category },
    { "cart_total_items", cartItemCount }
});
```

### 4. Track Errors with Context

Provide context when tracking errors:

```csharp
catch (PaymentException ex)
{
    monitor.AddError(
        ex.Message,
        RumErrorSource.Source,
        ex.StackTrace,
        new Dictionary<string, Java.Lang.Object>
        {
            { "payment_method", "credit_card" },
            { "amount", orderTotal },
            { "currency", "USD" },
            { "error_code", ex.ErrorCode }
        }
    );
}
```

### 5. Use Lower Sample Rates in Production

Conserve data and costs:

```csharp
#if DEBUG
rumConfig.SetSessionSampleRate(100f);
#else
rumConfig.SetSessionSampleRate(10f);  // 10% of production sessions
#endif
```

### 6. Track Key User Journeys

Focus on important flows:

```csharp
// Track checkout funnel
monitor.AddAction(RumActionType.Custom, "Checkout Started", attributes);
monitor.AddAction(RumActionType.Custom, "Payment Info Entered", attributes);
monitor.AddAction(RumActionType.Custom, "Order Confirmed", attributes);
```

## Troubleshooting

### RUM Not Collecting Data

1. **Check Core SDK initialization**:
```csharp
if (Com.Datadog.Android.Datadog.Instance == null)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Verify RUM Application ID**:
```csharp
Console.WriteLine($"RUM App ID: {RUM_APPLICATION_ID}");
```

3. **Check sample rate**:
```csharp
// Temporarily set to 100%
rumConfig.SetSessionSampleRate(100f);
```

### Views Not Appearing

- Ensure you call `StartView()` and `StopView()`
- Check if automatic tracking is enabled
- Verify view names are not null/empty

### Actions Not Being Tracked

- Enable automatic tracking: `.TrackUserInteractions()`
- Check if views are being tracked (actions require an active view)
- Verify manual `AddAction()` calls are correct

### Performance Impact

If RUM causes performance issues:

```csharp
// Reduce sampling
rumConfig.SetSessionSampleRate(10f);

// Disable frustration tracking
rumConfig.TrackFrustrations(false);

// Increase long task threshold
rumConfig.TrackLongTasks(200);  // Only track tasks > 200ms
```

## API Reference

### RumConfiguration.Builder

| Method | Description |
|--------|-------------|
| `TrackUserInteractions()` | Enable automatic touch tracking |
| `TrackLongTasks()` | Track UI thread blocking (100ms+ default) |
| `TrackLongTasks(long)` | Track long tasks with custom threshold (ms) |
| `TrackFrustrations(bool)` | Track rage taps and error taps |
| `TrackBackgroundEvents(bool)` | Track events when app backgrounded |
| `TrackNonFatalAnrs(bool)` | Track Application Not Responding events |
| `SetSessionSampleRate(float)` | Sample rate for sessions (0-100) |
| `SetTelemetrySampleRate(float)` | Sample rate for internal telemetry (0-100) |
| `SetViewTrackingStrategy(ViewTrackingStrategy)` | Custom view tracking |
| `SetVitalUpdateFrequency(VitalUpdateFrequency)` | Performance sampling frequency |
| `Build()` | Create configuration |

### GlobalRumMonitor

| Method | Description |
|--------|-------------|
| `Get()` / `Instance` | Get monitor instance |
| `StartView(key, name, attributes)` | Start tracking a view |
| `StopView(key, attributes)` | Stop tracking a view |
| `AddAction(type, name, attributes)` | Track user action |
| `AddError(message, source, stacktrace, attributes)` | Track error |
| `StartResource(key, method, url, attributes)` | Start tracking network request |
| `StopResource(key, statusCode, size, kind, attributes)` | Stop tracking successful request |
| `StopResourceWithError(key, message, source, throwable)` | Stop tracking failed request |
| `AddAttribute(key, value)` | Add global attribute |
| `RemoveAttribute(key)` | Remove global attribute |
| `AddFeatureFlagEvaluation(name, value)` | Track feature flag |

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-session-replay](../dd-sdk-android-session-replay/README.md)** - Session recording
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Correlate logs with RUM
- **[dd-sdk-android-trace](../dd-sdk-android-trace/README.md)** - Correlate traces with RUM

## Resources

- [Datadog RUM Android Documentation](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/android/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [Mobile Vitals](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/android/mobile_vitals/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
