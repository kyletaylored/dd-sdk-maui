---
layout: default
title: Unified API Design
nav_order: 5
---

# Datadog MAUI Unified API Design

## Namespace Strategy

To avoid conflicts with the existing `Datadog.Trace` NuGet package (APM .NET tracer), we use the `Datadog.Maui` namespace hierarchy:

```
Datadog.Maui              // Main namespace - initialization and core types
Datadog.Maui.Logs         // Log collection
Datadog.Maui.Rum          // Real User Monitoring
Datadog.Maui.Tracing      // Mobile tracing (distinct from Datadog.Trace)
Datadog.Maui.Platforms    // Platform-specific implementations (internal)
```

**Rationale**:
- `Datadog.Maui` follows MAUI conventions (like `Microsoft.Maui.*`)
- Clear distinction from `Datadog.Trace` (APM tracer for backend .NET apps)
- Lowercase "Maui" matches Microsoft's convention for product names in namespaces

## API Design Philosophy

1. **Builder Pattern**: Similar to existing Datadog.Trace API
2. **Fluent Configuration**: Chainable configuration methods
3. **Platform Abstraction**: Hide iOS/Android differences
4. **MAUI Integration**: Use `MauiAppBuilder` extension for initialization

## Core API

### 1. Initialization (MauiProgram.cs)

```csharp
using Datadog.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Datadog initialization
        builder.UseDatadog(config =>
        {
            config.ClientToken = "YOUR_CLIENT_TOKEN";
            config.Environment = "production";
            config.ServiceName = "com.example.myapp";
            config.Site = DatadogSite.US1;  // US1, US3, US5, EU1, etc.

            // Optional: Override platform-specific settings
            config.ConfigurePlatform(platform =>
            {
#if ANDROID
                platform.ApplicationId = "YOUR_ANDROID_RUM_APP_ID";
                platform.TrackingConsent = TrackingConsent.Granted;
#elif IOS
                platform.ApplicationId = "YOUR_IOS_RUM_APP_ID";
                platform.TrackingConsent = TrackingConsent.Granted;
#endif
            });
        });

        return builder.Build();
    }
}
```

### 2. Alternative: Manual Initialization

```csharp
using Datadog.Maui;

// Create configuration
var config = new DatadogConfiguration.Builder("YOUR_CLIENT_TOKEN")
    .SetEnvironment("production")
    .SetServiceName("com.example.myapp")
    .SetSite(DatadogSite.US1)
    .SetTrackingConsent(TrackingConsent.Granted)
    .Build();

// Initialize Datadog
Datadog.Initialize(config);
```

### 3. Global Datadog Instance

```csharp
using Datadog.Maui;

// Access global instance
var datadog = Datadog.Instance;

// Check initialization status
if (Datadog.IsInitialized)
{
    // Perform operations
}

// Set user information
Datadog.SetUser(new UserInfo
{
    Id = "user-123",
    Name = "John Doe",
    Email = "john@example.com",
    ExtraInfo = new Dictionary<string, object>
    {
        ["subscription"] = "premium",
        ["role"] = "admin"
    }
});

// Set global tags
Datadog.SetTags(new Dictionary<string, string>
{
    ["version"] = "1.2.3",
    ["build"] = "456"
});

// Update tracking consent
Datadog.SetTrackingConsent(TrackingConsent.Granted);
```

## Logs API

### Basic Logging

```csharp
using Datadog.Maui.Logs;

// Get logger instance
var logger = Logs.CreateLogger("my-logger");

// Log messages with different levels
logger.Debug("Debug message", attributes: new { userId = "123" });
logger.Info("User logged in", attributes: new { userId = "123", source = "oauth" });
logger.Warn("API rate limit approaching");
logger.Error("Failed to fetch data", error: exception);

// Log with structured attributes
logger.Info("Purchase completed", attributes: new Dictionary<string, object>
{
    ["transaction_id"] = "txn_123456",
    ["amount"] = 49.99,
    ["currency"] = "USD",
    ["items_count"] = 3
});
```

### Logger Configuration

```csharp
using Datadog.Maui.Logs;

// Configure logs during initialization
builder.UseDatadog(config =>
{
    config.EnableLogs(logs =>
    {
        logs.SampleRate = 100; // Log 100% of events
        logs.NetworkInfoEnabled = true;
        logs.BundleWithRum = true; // Associate logs with RUM sessions
    });
});

// Or configure after initialization
Logs.Configure(config =>
{
    config.AddAttribute("app_version", "1.2.3");
    config.AddAttribute("environment", "production");
});
```

## RUM (Real User Monitoring) API

### Basic RUM Tracking

```csharp
using Datadog.Maui.Rum;

// Start a view
Rum.StartView("ProductDetails", attributes: new { productId = "123" });

// Add actions
Rum.AddAction("Add to Cart", attributes: new { productId = "123", quantity = 2 });

// Track errors
try
{
    // Some operation
}
catch (Exception ex)
{
    Rum.AddError(ex, source: RumErrorSource.Source, attributes: new { context = "checkout" });
}

// Add custom timings
Rum.AddTiming("product_loaded");

// Track resources (API calls)
Rum.StartResource("https://api.example.com/products", method: "GET", attributes: new { page = 1 });
// ... after request completes
Rum.StopResource("https://api.example.com/products", kind: RumResourceKind.Native, status: 200, size: 1024);

// Stop view
Rum.StopView("ProductDetails", attributes: new { items_viewed = 5 });
```

### RUM Configuration

```csharp
using Datadog.Maui.Rum;

builder.UseDatadog(config =>
{
    config.EnableRum(rum =>
    {
        rum.ApplicationId = "YOUR_RUM_APP_ID";
        rum.SessionSampleRate = 100; // Monitor 100% of sessions
        rum.TelemetrySampleRate = 20; // Sample telemetry at 20%
        rum.TrackViewsAutomatically = true; // Auto-track page navigation
        rum.TrackUserInteractions = true; // Auto-track taps/clicks
        rum.TrackResources = true; // Auto-track network requests
        rum.TrackErrors = true; // Auto-track errors
        rum.VitalsUpdateFrequency = VitalsUpdateFrequency.Average;
    });
});

// Manual monitoring control
Rum.StartSession();
Rum.StopSession();
```

### MAUI Navigation Integration

```csharp
using Datadog.Maui.Rum;

// In your Shell or NavigationPage
protected override void OnNavigated(ShellNavigatedEventArgs args)
{
    base.OnNavigated(args);

    // Automatically track view changes
    Rum.StartView(args.Current.Location.ToString(), attributes: new
    {
        route = args.Current.Location.OriginalString,
        previous_route = args.Previous?.Location.OriginalString
    });
}
```

## Tracing API

### Span Creation

```csharp
using Datadog.Maui.Tracing;

// Create and start a span
using var span = Tracer.StartSpan("operation-name");
span.SetTag("user_id", "123");
span.SetTag("item_count", 5);

try
{
    // Perform operation
    await ProcessData();

    // Add events
    span.AddEvent("data_processed", attributes: new { records = 100 });
}
catch (Exception ex)
{
    span.SetError(ex);
    throw;
}
// Span automatically finishes when disposed

// Or manually manage span lifecycle
var span = Tracer.StartSpan("manual-operation");
// ... do work
span.Finish();
```

### Distributed Tracing

```csharp
using Datadog.Maui.Tracing;

// Inject trace context into HTTP headers
var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/data");
Tracer.Inject(request.Headers);

// Extract trace context from incoming data
var context = Tracer.Extract(headers);
using var span = Tracer.StartSpan("handle-request", parent: context);
```

### Trace Configuration

```csharp
builder.UseDatadog(config =>
{
    config.EnableTracing(tracing =>
    {
        tracing.SampleRate = 100;
        tracing.TraceIdGenerationEnabled = true;
        tracing.FirstPartyHosts = new[] { "api.example.com", "cdn.example.com" };
    });
});
```

## WebView Tracking

### Track WebView Content

```csharp
using Datadog.Maui.Rum;

// Enable WebView tracking for a specific WebView
var webView = new WebView
{
    Source = "https://example.com"
};

// Track WebView RUM events
Rum.TrackWebView(webView, allowedHosts: new[] { "example.com" });
```

## Configuration Types

### DatadogSite Enum

```csharp
public enum DatadogSite
{
    US1,  // datadoghq.com
    US3,  // us3.datadoghq.com
    US5,  // us5.datadoghq.com
    EU1,  // datadoghq.eu
    US1_FED,  // ddog-gov.com
    AP1   // ap1.datadoghq.com
}
```

### TrackingConsent Enum

```csharp
public enum TrackingConsent
{
    Granted,      // Start tracking immediately
    NotGranted,   // Do not track
    Pending       // Store events locally, wait for consent
}
```

### Comparison with Datadog.Trace

| Feature | Datadog.Trace (.NET APM) | Datadog.Maui (Mobile SDK) |
|---------|-------------------------|---------------------------|
| **Purpose** | Backend .NET application tracing | Mobile app monitoring (iOS/Android) |
| **Namespace** | `Datadog.Trace` | `Datadog.Maui` |
| **Initialization** | `Tracer.Configure(settings)` | `builder.UseDatadog(config)` |
| **Primary Use** | APM, distributed tracing | RUM, logs, mobile-specific monitoring |
| **Target** | ASP.NET, Console apps, services | .NET MAUI mobile apps |
| **Conflict** | ❌ No conflict - different namespaces | |

## Example: Complete Integration

```csharp
// MauiProgram.cs
using Datadog.Maui;
using Datadog.Maui.Logs;
using Datadog.Maui.Rum;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();

        // Initialize Datadog
        builder.UseDatadog(config =>
        {
            config.ClientToken = Environment.GetEnvironmentVariable("DD_CLIENT_TOKEN")!;
            config.Environment = "production";
            config.ServiceName = "com.example.myapp";
            config.Site = DatadogSite.US1;

            // Enable RUM
            config.EnableRum(rum =>
            {
                rum.ApplicationId = Environment.GetEnvironmentVariable("DD_RUM_APP_ID")!;
                rum.SessionSampleRate = 100;
                rum.TrackViewsAutomatically = true;
                rum.TrackUserInteractions = true;
                rum.TrackResources = true;
            });

            // Enable Logs
            config.EnableLogs(logs =>
            {
                logs.BundleWithRum = true;
                logs.NetworkInfoEnabled = true;
            });

            // Enable Tracing
            config.EnableTracing(tracing =>
            {
                tracing.SampleRate = 100;
                tracing.FirstPartyHosts = new[] { "api.example.com" };
            });

            // Set global attributes
            config.GlobalTags.Add("app_version", "1.2.3");
            config.GlobalTags.Add("platform", DeviceInfo.Platform.ToString());
        });

        return builder.Build();
    }
}

// In your pages/views
public partial class ProductDetailsPage : ContentPage
{
    private readonly ILogger _logger;

    public ProductDetailsPage()
    {
        InitializeComponent();
        _logger = Logs.CreateLogger("ProductDetailsPage");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Track view
        Rum.StartView("ProductDetails", attributes: new { productId = ProductId });
        _logger.Info("Viewed product details", new { productId = ProductId });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Rum.StopView("ProductDetails");
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        using var span = Tracer.StartSpan("add_to_cart");
        span.SetTag("product_id", ProductId);

        try
        {
            // Track user action
            Rum.AddAction("AddToCart", attributes: new { productId = ProductId });

            await CartService.AddItem(ProductId);

            _logger.Info("Added to cart", new { productId = ProductId });
        }
        catch (Exception ex)
        {
            Rum.AddError(ex, source: RumErrorSource.Source);
            _logger.Error("Failed to add to cart", ex, new { productId = ProductId });
            span.SetError(ex);
        }
    }
}
```

## Migration Path from Native SDKs

For users migrating from native Datadog iOS/Android SDKs:

| Native iOS/Android | Datadog.Maui |
|-------------------|--------------|
| `Datadog.initialize()` | `builder.UseDatadog()` or `Datadog.Initialize()` |
| `Logs.enable()` | `config.EnableLogs()` |
| `RUM.enable()` | `config.EnableRum()` |
| `Trace.enable()` | `config.EnableTracing()` |
| `RUMMonitor.shared()` | `Rum` static class |
| `Logger.create()` | `Logs.CreateLogger()` |
| `GlobalRUM.addAttribute()` | `Rum.AddAttribute()` |

## Implementation Notes

1. **Platform-specific code** lives in `Datadog.Maui.Platforms` (internal)
2. **Partial classes** for platform-specific implementations:
   ```csharp
   // Datadog.cs
   public static partial class Datadog
   {
       public static partial void Initialize(DatadogConfiguration config);
   }

   // Platforms/Android/Datadog.android.cs
   public static partial class Datadog
   {
       public static partial void Initialize(DatadogConfiguration config)
       {
           // Android-specific implementation
       }
   }
   ```

3. **Dependency Injection** support:
   ```csharp
   builder.Services.AddSingleton<ILogger>(Logs.CreateLogger("app"));
   ```

4. **Crash Reporting** automatic on both platforms (NDK for Android, native iOS crash reporting)

## Next Steps

1. Implement core types (`DatadogConfiguration`, `Datadog` static class)
2. Implement `MauiAppBuilderExtensions.UseDatadog()`
3. Implement Logs API with platform-specific bridges
4. Implement RUM API with MAUI navigation integration
5. Implement Tracing API
6. Add XML documentation comments
7. Create sample project demonstrating all features
