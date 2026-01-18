# Datadog Android WebView Tracking - .NET MAUI Binding

.NET MAUI bindings for Datadog Android WebView Tracking. Track user interactions, errors, and resources within WebViews integrated with your RUM sessions.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.WebView
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.WebView" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.Android.Core` and `Datadog.MAUI.Android.RUM` to be initialized first.

## Overview

The WebView Tracking module provides:
- **Automatic RUM integration** for web content in WebViews
- **Resource tracking** for HTTP requests made from WebViews
- **Error tracking** for JavaScript errors
- **User action tracking** for interactions within web pages
- **Seamless correlation** between native and web sessions

## Quick Start

### Enable WebView Tracking

In your `MainApplication.cs` after Core and RUM initialization:

```csharp
using Com.Datadog.Android.Webview;

try
{
    // Enable WebView tracking globally
    WebViewTracking.Enable();

    Console.WriteLine("[Datadog] WebView tracking enabled");
}
catch (Exception ex)
{
    Console.WriteLine($"[Datadog] WebView tracking failed: {ex.Message}");
}
```

That's it! All WebViews in your app will now be tracked automatically.

## What Gets Tracked

### Captured Information

When WebView tracking is enabled, the following data is automatically captured:

- **Views**: Page loads and navigation within WebViews
- **Resources**: HTTP requests (XHR, Fetch, images, scripts)
- **Actions**: User interactions (taps, swipes, form submissions)
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
using Android.App;
using Android.Webkit;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Privacy;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Webview;

[Application]
public class MainApplication : MauiApplication
{
    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize in order: Core → RUM → WebView
        InitializeDatadogCore();
        InitializeRUM();
        InitializeWebViewTracking();
    }

    private void InitializeDatadogCore()
    {
        var config = new Configuration.Builder(
            "YOUR_CLIENT_TOKEN",
            "prod",
            string.Empty,
            "my-app"
        )
        .SetFirstPartyHosts(new List<string> { "api.example.com", "example.com" })
        .Build();

        Datadog.Initialize(this, config, TrackingConsent.Granted);
    }

    private void InitializeRUM()
    {
        var rumConfig = new RumConfiguration.Builder("YOUR_RUM_APP_ID")
            .TrackUserInteractions()
            .Build();

        Rum.Enable(rumConfig);
    }

    private void InitializeWebViewTracking()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling WebView tracking...");

            // Enable tracking for all WebViews
            WebViewTracking.Enable();

            Console.WriteLine("[Datadog] WebView tracking enabled successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] WebView tracking failed: {ex.Message}");
        }
    }
}
```

## Using WebViews

### Basic WebView Setup

```csharp
using Android.Webkit;
using Microsoft.Maui.Handlers;

// In your MAUI view or page
var webView = new WebView
{
    Source = "https://example.com"
};

// WebView tracking is automatic - no additional code needed!
```

### WebView with Custom Settings

```csharp
#if ANDROID
using Android.Webkit;
using Microsoft.Maui.Platform;

// Customize WebView settings (optional)
public class CustomWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(Android.Webkit.WebView platformView)
    {
        base.ConnectHandler(platformView);

        var settings = platformView.Settings;
        settings.JavaScriptEnabled = true;
        settings.DomStorageEnabled = true;

        // Datadog tracking is automatic - already enabled
    }
}
#endif
```

### Hybrid App Pattern

For hybrid apps mixing native and web content:

```csharp
public class HybridPage : ContentPage
{
    private WebView webView;

    public HybridPage()
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

        // All interactions tracked automatically:
        // - Page loads in WebView
        // - User clicks/taps
        // - XHR/Fetch requests
        // - JavaScript errors
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Native view tracking (if using manual RUM)
        GlobalRumMonitor.Get().StartView("HybridPage", "Hybrid Page", null);

        // Web content tracking is automatic
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        GlobalRumMonitor.Get().StopView("HybridPage", null);
    }
}
```

## First-Party Hosts

Configure which hosts should be traced for detailed resource tracking:

```csharp
// In Core SDK initialization
var config = new Configuration.Builder(clientToken, env, variant, service)
    .SetFirstPartyHosts(new List<string>
    {
        "api.example.com",     // API domain
        "example.com",         // Main domain
        "*.example.com"        // All subdomains
    })
    .Build();
```

**Benefits of First-Party Hosts**:
- Detailed resource timing
- Request/response headers captured
- Distributed tracing correlation
- Spans created for backend correlation

## How It Works

### Tracking Flow

```
1. App creates WebView
2. WebViewTracking.Enable() injects Datadog JS
3. User loads web page
4. JavaScript SDK initialized in WebView
5. Web events captured (views, actions, resources)
6. Events forwarded to native SDK
7. Correlated with RUM session
8. Sent to Datadog backend
```

### JavaScript Injection

The WebView Tracking module automatically:
1. Injects Datadog Browser SDK into all WebViews
2. Configures Browser SDK with same client token
3. Links web session to native RUM session
4. Forwards all web events through native SDK

**No manual JavaScript integration required!**

## RUM Integration

### Viewing WebView Data

In Datadog RUM Explorer:

1. **Filter by source**:
   - `@view.source:browser` - Web views only
   - `@view.source:android` - Native views only

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
if (Datadog.Instance == null)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check RUM enabled** (required for WebView tracking):
```csharp
Rum.Enable(rumConfig);
```

3. **Verify JavaScript enabled**:
```csharp
#if ANDROID
var settings = platformWebView.Settings;
if (!settings.JavaScriptEnabled)
{
    settings.JavaScriptEnabled = true;
}
#endif
```

4. **Check WebView URL**:
   - Must be HTTP/HTTPS (not file://)
   - Must have network connectivity
   - Check Content Security Policy (CSP) allows Datadog

### Web Events Not Appearing in Datadog

1. **Wait for data upload**: Events batch and upload periodically
2. **Check RUM session active**: Web events require active RUM session
3. **Verify first-party hosts**: For detailed tracking, configure hosts
4. **Check network**: Ensure WebView can reach Datadog endpoints

### Missing Resource Details

If resource timing is missing:
1. **Configure first-party hosts** in Core SDK
2. **Enable CORS** on your backend for timing API
3. **Check CSP headers** allow resource timing

### JavaScript Errors Not Captured

1. **Verify JavaScript enabled** in WebView settings
2. **Check error occurs after SDK injection** (early errors may be missed)
3. **Enable verbose logging** to see injection status:
```csharp
Datadog.Verbosity = (int)Android.Util.LogPriority.Verbose;
```

## Best Practices

### 1. Enable Early

Enable WebView tracking immediately after RUM:

```csharp
Datadog.Initialize(this, config, consent);
Rum.Enable(rumConfig);
WebViewTracking.Enable();  // Enable right after RUM
```

### 2. Configure First-Party Hosts

For web apps making API calls:

```csharp
.SetFirstPartyHosts(new List<string>
{
    "api.example.com",
    "*.example.com"
})
```

### 3. Enable JavaScript

Ensure JavaScript is enabled:

```csharp
#if ANDROID
platformWebView.Settings.JavaScriptEnabled = true;
#endif
```

### 4. Use HTTPS

Always use HTTPS URLs for better security and tracking:

```csharp
// Good
webView.Source = "https://example.com";

// Avoid
webView.Source = "http://example.com";  // Less secure
```

### 5. Handle Gracefully

WebView tracking might not be available:

```csharp
try
{
    WebViewTracking.Enable();
}
catch (Exception ex)
{
    // App continues normally without WebView tracking
    Console.WriteLine($"WebView tracking not available: {ex.Message}");
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

1. **Mask sensitive URLs**:
```csharp
// Configure in RUM
rumConfig.SetResourceEventMapper(resourceEvent =>
{
    // Mask URL parameters containing PII
    var url = resourceEvent.Resource.Url;
    if (url.Contains("token="))
    {
        url = Regex.Replace(url, @"token=[^&]+", "token=***");
    }
    resourceEvent.Resource.Url = url;
    return resourceEvent;
});
```

2. **Avoid loading sensitive content** in tracked WebViews:
```csharp
// For highly sensitive content, use separate WebView without tracking
// or disable tracking temporarily
```

3. **Review CSP headers** to control what web content can do:
```html
<!-- In your web pages -->
<meta http-equiv="Content-Security-Policy"
      content="default-src 'self' https://datadoghq.com">
```

## Performance Impact

WebView Tracking has minimal performance impact:

- **Initialization**: <10ms
- **Runtime overhead**: <1% (JavaScript injection and forwarding)
- **App size**: ~50KB (JavaScript SDK injection)
- **Network**: Shares native SDK upload (no extra requests)

## API Reference

### WebViewTracking

| Method | Description |
|--------|-------------|
| `Enable()` | Enable WebView tracking for all WebViews in the app |

**That's it!** Simple one-method API.

### Requirements

- `Datadog.MAUI.Android.Core` must be initialized
- `Datadog.MAUI.Android.RUM` must be enabled
- WebView must have JavaScript enabled
- WebView must load HTTP/HTTPS content (not file://)

## Advanced Configuration

### Custom WebView Client

If using custom WebViewClient:

```csharp
#if ANDROID
using Android.Webkit;

public class CustomWebViewClient : WebViewClient
{
    public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
    {
        base.OnPageStarted(view, url, favicon);

        // Datadog tracking automatic - no manual calls needed
        Console.WriteLine($"Loading: {url}");
    }

    public override void OnPageFinished(WebView view, string url)
    {
        base.OnPageFinished(view, url);

        // Tracking already handled
        Console.WriteLine($"Loaded: {url}");
    }
}
#endif
```

### JavaScript Bridge

If using JavaScript bridge for native/web communication:

```csharp
#if ANDROID
using Android.Webkit;

public class JavaScriptInterface : Java.Lang.Object
{
    [JavascriptInterface]
    [Export("callNative")]
    public void CallNative(string message)
    {
        // Log custom action in RUM
        GlobalRumMonitor.Get().AddAction(
            RumActionType.Custom,
            $"WebView Bridge: {message}",
            null
        );
    }
}

// Add to WebView
platformWebView.AddJavascriptInterface(
    new JavaScriptInterface(),
    "NativeBridge"
);
```

Then in web JavaScript:
```javascript
// Call from web page
window.NativeBridge.callNative("user_completed_form");
```

## Limitations

### What's NOT Tracked

- **file:// URLs** - Local HTML files not tracked (no network)
- **about:blank** - Blank pages not tracked
- **Data URLs** - Inline data: URLs not tracked
- **Pre-injection errors** - Errors before SDK injection may be missed
- **WebView screenshots** - Session Replay doesn't capture WebView content

### Known Issues

- **CSP restrictions** - Strict CSP may block Datadog injection
- **Single-page apps** - Route changes may not create new views (depends on web framework)
- **iframes** - Nested iframes may have limited tracking

## Examples

### E-commerce Hybrid App

```csharp
public class ProductPage : ContentPage
{
    private WebView productWebView;

    public ProductPage(string productId)
    {
        // Native header
        var header = new Label { Text = "Product Details" };

        // Web content
        productWebView = new WebView
        {
            Source = $"https://example.com/products/{productId}"
        };

        Content = new StackLayout
        {
            Children = { header, productWebView }
        };

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

        // All dashboard interactions tracked:
        // - Chart loads
        // - Filter changes
        // - API calls for data
        // - Export actions
    }
}
```

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Required: RUM tracking (WebView events appear here)
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Optional: Log correlation with web logs
- **[dd-sdk-android-trace](../dd-sdk-android-trace/README.md)** - Optional: Trace web API calls

## Resources

- [Android WebView Tracking](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/android/web_view_tracking/)
- [Browser SDK](https://docs.datadoghq.com/real_user_monitoring/browser/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
