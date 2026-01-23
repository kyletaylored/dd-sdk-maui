# Datadog iOS WebView Tracking - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS WebView Tracking. Track user interactions, errors, and resources within WKWebViews integrated with your RUM sessions.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.WebViewTracking
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.WebViewTracking" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` and `Datadog.MAUI.iOS.RUM` to be initialized first.

## Overview

WebView Tracking provides:
- **Automatic RUM integration** for web content in WKWebViews
- **Resource tracking** for HTTP requests made from WebViews
- **Error tracking** for JavaScript errors
- **User action tracking** for interactions within web pages
- **Seamless correlation** between native and web sessions
- **Cross-platform consistency** with Android WebView tracking

## Quick Start

### Enable WebView Tracking

In your `AppDelegate.cs` after Core and RUM initialization:

```csharp
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.WebViewTracking;
using WebKit;

// Initialize Core and RUM first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);
DDRUM.Enable(rumConfig);

// Create WKWebView
var webView = new WKWebView();

// Enable tracking for this WebView
DDWebViewTracking.Enable(
    webView: webView,
    hosts: new[] { "example.com", "api.example.com" }
);

Console.WriteLine("[Datadog] WebView tracking enabled");
```

## What Gets Tracked

### Captured Information

When WebView tracking is enabled, the following data is automatically captured:

- **Views**: Page loads and navigation within WebViews
- **Resources**: HTTP requests (XHR, Fetch, images, scripts)
- **Actions**: User interactions (taps, form submissions)
- **Errors**: JavaScript errors and console errors
- **Long Tasks**: JavaScript blocking main thread >50ms
- **Session correlation**: Web events linked to native RUM session

### Integration with RUM

WebView events appear in your RUM session as:
- Native view → Web view → Native view (seamless transitions)
- All web events tagged with `source:browser`
- Full correlation between native and web performance

## Complete Example

```csharp
using Foundation;
using UIKit;
using WebKit;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.WebViewTracking;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private const string CLIENT_TOKEN = "YOUR_CLIENT_TOKEN";
    private const string RUM_APPLICATION_ID = "YOUR_RUM_APP_ID";

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
            var config = new DDConfiguration(CLIENT_TOKEN, "prod");
            config.Service = "com.example.myapp";
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            // Enable RUM
            var rumConfig = new DDRUMConfiguration(RUM_APPLICATION_ID);
            rumConfig.TrackUIKitViews();
            rumConfig.TrackUIKitActions();
            DDRUM.Enable(rumConfig);

            Console.WriteLine("[Datadog] Core and RUM initialized");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Initialization failed: {ex.Message}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

// In your view controller or page
public class WebViewController : UIViewController
{
    private WKWebView? webView;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Create WebView
        webView = new WKWebView(View.Bounds);
        View.AddSubview(webView);

        // Enable Datadog tracking
        DDWebViewTracking.Enable(
            webView: webView,
            hosts: new[] { "example.com", "*.example.com" }
        );

        // Load content
        var url = new NSUrl("https://example.com");
        webView.LoadRequest(new NSUrlRequest(url));
    }

    public override void ViewWillDisappear(bool animated)
    {
        base.ViewWillDisappear(animated);

        // Disable tracking when view disappears (optional)
        if (webView != null)
        {
            DDWebViewTracking.Disable(webView);
        }
    }
}
```

## Using WebViews

### Basic WebView Setup

```csharp
using WebKit;

// In your MAUI ContentPage
public class HybridPage : ContentPage
{
    public HybridPage()
    {
        var webView = new WebView
        {
            Source = "https://example.com"
        };

        Content = webView;

        // Access native WKWebView (iOS)
        #if IOS
        webView.HandlerChanged += (sender, e) =>
        {
            if (webView.Handler?.PlatformView is WKWebView wkWebView)
            {
                // Enable Datadog tracking
                DDWebViewTracking.Enable(
                    wkWebView,
                    new[] { "example.com" }
                );
            }
        };
        #endif
    }
}
```

### WebView with Custom Handler

```csharp
#if IOS
using WebKit;
using Microsoft.Maui.Handlers;

public class CustomWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(WKWebView platformView)
    {
        base.ConnectHandler(platformView);

        // Enable Datadog tracking
        DDWebViewTracking.Enable(
            platformView,
            new[] { "example.com", "api.example.com" }
        );
    }

    protected override void DisconnectHandler(WKWebView platformView)
    {
        // Disable tracking
        DDWebViewTracking.Disable(platformView);

        base.DisconnectHandler(platformView);
    }
}
#endif
```

### Hybrid App Pattern

```csharp
public class HybridAppPage : ContentPage
{
    private WebView? webView;

    public HybridAppPage()
    {
        // Create WebView
        webView = new WebView
        {
            Source = new UrlWebViewSource
            {
                Url = "https://example.com/webapp"
            }
        };

        Content = webView;

        // Enable tracking
        #if IOS
        webView.HandlerChanged += EnableTracking;
        #endif
    }

    #if IOS
    private void EnableTracking(object? sender, EventArgs e)
    {
        if (webView?.Handler?.PlatformView is WKWebView wkWebView)
        {
            DDWebViewTracking.Enable(
                wkWebView,
                new[] { "example.com" }
            );

            // All interactions tracked automatically:
            // - Page loads in WebView
            // - User clicks/taps
            // - XHR/Fetch requests
            // - JavaScript errors
        }
    }
    #endif

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Native view tracking (if using manual RUM)
        var monitor = DDRUMMonitor.Shared;
        monitor.StartView("HybridAppPage", "Hybrid App", null);

        // Web content tracking is automatic
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        var monitor = DDRUMMonitor.Shared;
        monitor.StopView("HybridAppPage", null);
    }
}
```

## Configuration

### Tracked Hosts

Specify which hosts to track in detail:

```csharp
// Single domain
DDWebViewTracking.Enable(webView, new[] { "example.com" });

// Multiple domains
DDWebViewTracking.Enable(webView, new[] {
    "example.com",
    "api.example.com",
    "cdn.example.com"
});

// Wildcard subdomains
DDWebViewTracking.Enable(webView, new[] {
    "*.example.com",  // All subdomains
    "example.com"     // Main domain
});
```

**Benefits of Tracked Hosts**:
- Detailed resource timing
- Request/response headers captured
- Distributed tracing correlation
- Spans created for backend correlation

### Disabling Tracking

```csharp
// Disable tracking for a WebView
DDWebViewTracking.Disable(webView);
```

## How It Works

### Tracking Flow

```
1. App creates WKWebView
2. DDWebViewTracking.Enable() injects Datadog JS
3. User loads web page
4. JavaScript SDK initialized in WebView
5. Web events captured (views, actions, resources)
6. Events forwarded to native SDK
7. Correlated with RUM session
8. Sent to Datadog backend
```

### JavaScript Injection

The WebView Tracking module automatically:
1. Injects Datadog Browser SDK into all tracked WebViews
2. Configures Browser SDK with same client token
3. Links web session to native RUM session
4. Forwards all web events through native SDK

**No manual JavaScript integration required!**

## RUM Integration

### Viewing WebView Data

In Datadog RUM Explorer:

1. **Filter by source**:
   - `@view.source:ios` - Native views only
   - `@view.source:browser` - Web views only

2. **Session Timeline**:
   - See seamless transitions: Native → Web → Native
   - Web views appear inline with native views

3. **Resource Timing**:
   - HTTP requests from WebView
   - XHR/Fetch calls
   - Image/script loads

4. **Errors**:
   - JavaScript errors
   - Console errors
   - Network failures

### Correlation

WebView events are correlated with:
- **Same RUM session**: Web and native share session ID
- **User context**: Same user info across web/native
- **Global attributes**: Attributes set in native apply to web
- **View hierarchy**: Web views nested under native views

## Troubleshooting

### WebView Tracking Not Working

1. **Check Core and RUM initialized**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check RUM enabled** (required for WebView tracking):
```csharp
DDRUM.Enable(rumConfig);
```

3. **Verify WebView is WKWebView**:
```csharp
if (platformView is WKWebView wkWebView)
{
    DDWebViewTracking.Enable(wkWebView, hosts);
}
```

4. **Check hosts array not empty**:
```csharp
DDWebViewTracking.Enable(webView, new[] { "example.com" });  // Not empty
```

### Web Events Not Appearing in Datadog

1. **Wait for data upload**: Events batch and upload periodically
2. **Check RUM session active**: Web events require active RUM session
3. **Verify tracked hosts**: Only specified hosts tracked in detail
4. **Check network**: Ensure WebView can reach Datadog endpoints
5. **Inspect WebView console**: Look for Datadog JS initialization messages

### Missing Resource Details

If resource timing is missing:
1. **Add hosts to tracked list**
2. **Enable CORS** on your backend for timing API
3. **Check CSP headers** allow resource timing

### JavaScript Errors Not Captured

1. **Verify tracking enabled** before page loads
2. **Check error occurs after SDK injection** (early errors may be missed)
3. **Enable verbose logging** to see injection status:
```csharp
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
```

## Best Practices

### 1. Enable Before Loading Content

```csharp
// Create WebView
var webView = new WKWebView();

// Enable tracking BEFORE loading
DDWebViewTracking.Enable(webView, hosts);

// Then load content
webView.LoadRequest(new NSUrlRequest(url));
```

### 2. Specify Tracked Hosts

```csharp
// Track your domains for detailed monitoring
DDWebViewTracking.Enable(webView, new[] {
    "example.com",
    "api.example.com",
    "*.example.com"
});
```

### 3. Use HTTPS

Always use HTTPS URLs for better security and tracking:

```csharp
// Good
webView.LoadRequest(new NSUrlRequest(new NSUrl("https://example.com")));

// Avoid
webView.LoadRequest(new NSUrlRequest(new NSUrl("http://example.com")));
```

### 4. Handle Gracefully

WebView tracking might not be available:

```csharp
try
{
    DDWebViewTracking.Enable(webView, hosts);
}
catch (Exception ex)
{
    // App continues normally without WebView tracking
    Console.WriteLine($"WebView tracking not available: {ex.Message}");
}
```

### 5. Clean Up

Disable tracking when WebView is disposed:

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing && webView != null)
    {
        DDWebViewTracking.Disable(webView);
    }
    base.Dispose(disposing);
}
```

### 6. Test Hybrid Flows

Test full user journeys:

```csharp
// Native view → Web view → Native view
// Ensure smooth session continuity
```

## Privacy Considerations

### User Data in Web Content

WebView tracking captures:
- ✅ **Page URLs** - Use query parameter masking if needed
- ✅ **User actions** - Clicks, taps on web elements
- ✅ **Resources** - URLs of HTTP requests
- ✅ **JavaScript errors** - May include user input in error messages

### Privacy Best Practices

1. **Review tracked domains** - Only track necessary hosts
2. **Avoid loading sensitive content** in tracked WebViews
3. **Configure CSP headers** to control what web content can do
4. **Test privacy settings** before production

## Performance Impact

WebView Tracking has minimal performance impact:

- **Initialization**: <10ms per WebView
- **Runtime overhead**: <1% (JavaScript injection and forwarding)
- **Network**: Shares native SDK upload (no extra requests)
- **WebView memory**: +~2MB for Browser SDK

## API Reference

### DDWebViewTracking

| Method | Description |
|--------|-------------|
| `Enable(WKWebView, string[])` | Enable tracking for WebView with tracked hosts |
| `Disable(WKWebView)` | Disable tracking for WebView |

### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `webView` | `WKWebView` | WebView instance to track |
| `hosts` | `string[]` | Array of hosts to track in detail |

## Examples

### E-commerce Hybrid App

```csharp
public class ProductPage : ContentPage
{
    public ProductPage(string productId)
    {
        var webView = new WebView
        {
            Source = $"https://example.com/products/{productId}"
        };

        Content = webView;

        #if IOS
        webView.HandlerChanged += (s, e) =>
        {
            if (webView.Handler?.PlatformView is WKWebView wkWebView)
            {
                DDWebViewTracking.Enable(wkWebView, new[] { "example.com" });
            }
        };
        #endif

        // Tracking automatic:
        // - Page load tracked
        // - User clicks "Add to Cart" tracked
        // - API calls to add to cart tracked
        // - JavaScript errors tracked
    }
}
```

### Web Dashboard

```csharp
public class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        var webView = new WebView
        {
            Source = "https://dashboard.example.com"
        };

        Content = webView;

        #if IOS
        webView.HandlerChanged += (s, e) =>
        {
            if (webView.Handler?.PlatformView is WKWebView wkWebView)
            {
                DDWebViewTracking.Enable(
                    wkWebView,
                    new[] { "dashboard.example.com", "api.example.com" }
                );
            }
        };
        #endif

        // All dashboard interactions tracked:
        // - Chart loads
        // - Filter changes
        // - API calls for data
        // - Export actions
    }
}
```

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogRUM](../DatadogRUM/README.md)** - Required: RUM tracking (WebView events appear here)
- **[DatadogLogs](../DatadogLogs/README.md)** - Optional: Log correlation with web logs
- **[DatadogTrace](../DatadogTrace/README.md)** - Optional: Trace web API calls

## Resources

- [iOS WebView Tracking](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/web_view_tracking/?tab=ios)
- [Browser SDK](https://docs.datadoghq.com/real_user_monitoring/browser/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
