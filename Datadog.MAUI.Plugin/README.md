# Datadog SDK for .NET MAUI

[![NuGet](https://img.shields.io/nuget/v/Datadog.MAUI.svg)](https://www.nuget.org/packages/Datadog.MAUI/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/kyletaylored/dd-sdk-maui/blob/main/LICENSE)

Official Datadog monitoring SDK for .NET MAUI applications. Monitor your iOS and Android mobile apps with Real User Monitoring (RUM), log collection, and distributed tracing.

## Features

- **Real User Monitoring (RUM)** - Track user sessions, views, actions, resources, and errors
- **Session Replay** - Visual reproduction of user sessions with privacy controls
- **Log Collection** - Centralized logging with context enrichment
- **Distributed Tracing** - End-to-end visibility across mobile and backend services
- **Cross-Platform** - Single API for both iOS and Android
- **Easy Integration** - Simple setup with MauiAppBuilder extension

## Installation

Install via .NET CLI:

```bash
dotnet add package Datadog.MAUI
```

Or via Package Manager Console:

```powershell
Install-Package Datadog.MAUI
```

## Quick Start

### Option A: Configuration-Based (Simplest)

Initialize using `appsettings.json` - no custom classes needed:

**1. Create appsettings.json:**

```json
{
  "Datadog": {
    "Android": {
      "ClientToken": "pub...",
      "RumApplicationId": "..."
    },
    "iOS": {
      "ClientToken": "pub...",
      "RumApplicationId": "..."
    }
  }
}
```

**2. Initialize in one line:**

```csharp
builder.UseDatadogFromConfiguration();
```

### Option B: Programmatic

In your `MauiProgram.cs`:

```csharp
using Datadog.Maui;
using Datadog.Maui.Configuration;
using Datadog.Maui.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseDatadog(datadog =>
            {
                // Platform-specific client tokens
                datadog.SetClientToken(
                    android: "YOUR_ANDROID_CLIENT_TOKEN",
                    ios: "YOUR_IOS_CLIENT_TOKEN"
                );
                datadog.Environment = "production";
                datadog.ServiceName = "my-maui-app";
                datadog.Site = DatadogSite.US1;
                datadog.TrackingConsent = TrackingConsent.Granted;

                // Enable RUM
                datadog.EnableRum(rum =>
                {
                    rum.SetApplicationId(
                        android: "YOUR_ANDROID_RUM_ID",
                        ios: "YOUR_IOS_RUM_ID"
                    );
                });

                // Enable Logs (no configuration needed)
                datadog.EnableLogs();

                // Enable Tracing
                datadog.EnableTracing(tracing =>
                {
                    tracing.SetFirstPartyHosts(new[] { "api.example.com" });
                });

                // Enable Session Replay
                datadog.EnableSessionReplay(sessionReplay =>
                {
                    sessionReplay.SetSampleRate(20);
                    sessionReplay.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs);
                    sessionReplay.SetImagePrivacy(ImagePrivacy.MaskNonBundledOnly);
                    sessionReplay.SetTouchPrivacy(TouchPrivacy.Show);
                });
            });

        return builder.Build();
    }
}
```

### 2. Log Messages

```csharp
using Datadog.Maui.Logs;

var logger = Logger.Create("my-logger");

logger.Info("User logged in");
logger.Warn("Low memory warning", attributes: new Dictionary<string, object>
{
    { "available_memory", 256 },
    { "threshold", 512 }
});

try
{
    ProcessPayment();
}
catch (Exception ex)
{
    logger.Error("Payment failed", error: ex, attributes: new Dictionary<string, object>
    {
        { "amount", 99.99 },
        { "payment_method", "credit_card" }
    });
}
```

### 3. Track User Activity (RUM)

```csharp
using Datadog.Maui.Rum;

// Track views
Rum.StartView("home_screen", "Home");
// ... user interacts with the screen
Rum.StopView("home_screen");

// Track actions
Rum.AddAction(RumActionType.Tap, "login_button");

// Track errors
try
{
    await LoadData();
}
catch (Exception ex)
{
    Rum.AddError(ex);
}

// Track network requests
var resourceKey = $"api_users_{Guid.NewGuid()}";
Rum.StartResource(resourceKey, "GET", "https://api.myapp.com/users");

try
{
    var response = await httpClient.GetAsync(url);
    Rum.StopResource(resourceKey,
        statusCode: (int)response.StatusCode,
        size: content.Length,
        kind: RumResourceKind.Xhr);
}
catch (Exception ex)
{
    Rum.StopResourceWithError(resourceKey, ex);
}
```

### 4. Distributed Tracing

```csharp
using Datadog.Maui.Tracing;

// Create a span
using (var span = Tracer.StartSpan("process_order"))
{
    span.SetTag("order_id", orderId);
    span.SetTag("amount", totalAmount);

    try
    {
        await ProcessOrder();
    }
    catch (Exception ex)
    {
        span.SetError(ex);
        throw;
    }
    // Span automatically finishes when disposed
}

// Inject trace context into HTTP requests
var request = new HttpRequestMessage(HttpMethod.Get, url);
Tracer.Inject(request.Headers);
var response = await httpClient.SendAsync(request);
```

## Configuration

### Datadog Sites

Specify your Datadog site based on your account region:

| Site | Value | URL |
|------|-------|-----|
| US1 (default) | `DatadogSite.US1` | `https://app.datadoghq.com` |
| US3 | `DatadogSite.US3` | `https://us3.datadoghq.com` |
| US5 | `DatadogSite.US5` | `https://us5.datadoghq.com` |
| EU1 | `DatadogSite.EU1` | `https://app.datadoghq.eu` |
| US1-FED | `DatadogSite.US1_FED` | `https://app.ddog-gov.com` |
| AP1 | `DatadogSite.AP1` | `https://ap1.datadoghq.com` |

### User Information

```csharp
// Set user information
Datadog.SetUser(new UserInfo
{
    Id = "12345",
    Name = "John Doe",
    Email = "john@example.com",
    ExtraInfo = new Dictionary<string, object>
    {
        { "plan", "premium" }
    }
});

// Clear on logout
Datadog.ClearUser();
```

### Tracking Consent

```csharp
// Update consent status
Datadog.SetTrackingConsent(TrackingConsent.Granted);
Datadog.SetTrackingConsent(TrackingConsent.NotGranted);
Datadog.SetTrackingConsent(TrackingConsent.Pending);
```

### Global Tags

```csharp
Datadog.SetTags(new Dictionary<string, string>
{
    { "app_version", "1.0.0" },
    { "environment", "production" }
});
```

## Platform Requirements

### iOS
- iOS 12.0+
- .NET 8, 9, 10

### Android
- Android 5.0 (API 21)+
- .NET 9, 10

## Documentation

📚 **[Full Documentation](https://kyletaylored.github.io/dd-sdk-maui/)**

### Getting Started
- **[Using the SDK](https://kyletaylored.github.io/dd-sdk-maui/getting-started/using-the-sdk)** - Complete usage guide
- **[API Reference](https://kyletaylored.github.io/dd-sdk-maui/api-reference)** - Full API documentation
- **[Code Examples](https://kyletaylored.github.io/dd-sdk-maui/examples)** - Real-world examples

### Key Topics
- **[Logging](https://kyletaylored.github.io/dd-sdk-maui/api-reference#logs-api)** - Log collection and management
- **[RUM](https://kyletaylored.github.io/dd-sdk-maui/api-reference#rum-api)** - Real User Monitoring
- **[Tracing](https://kyletaylored.github.io/dd-sdk-maui/api-reference#tracing-api)** - Distributed tracing
- **[Configuration](https://kyletaylored.github.io/dd-sdk-maui/getting-started/using-the-sdk#configuration)** - All configuration options

### Datadog Resources
- [Datadog Documentation](https://docs.datadoghq.com/)
- [RUM Mobile Monitoring](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/)
- [Mobile Log Collection](https://docs.datadoghq.com/logs/log_collection/mobile/)
- [Mobile APM](https://docs.datadoghq.com/tracing/trace_collection/mobile/)

## Example: E-Commerce Checkout

```csharp
public class CheckoutService
{
    private readonly ILogger _logger = Logs.CreateLogger("checkout");

    public async Task ProcessCheckoutAsync(Cart cart)
    {
        using (var span = Tracer.StartSpan("checkout.process"))
        {
            span.SetTag("cart_id", cart.Id);
            span.SetTag("total_amount", cart.TotalAmount);

            Rum.StartView("checkout", "Checkout");

            try
            {
                // Validate cart
                _logger.Info("Validating cart");
                await ValidateCart(cart);
                Rum.AddTiming("cart_validated");

                // Process payment
                _logger.Info("Processing payment", new Dictionary<string, object>
                {
                    { "amount", cart.TotalAmount },
                    { "method", cart.PaymentMethod }
                });

                using (var paymentSpan = Tracer.StartSpan("checkout.payment", span))
                {
                    try
                    {
                        await ProcessPayment(cart);
                        Rum.AddAction(RumActionType.Custom, "payment_successful");
                    }
                    catch (PaymentException ex)
                    {
                        paymentSpan.SetError(ex);
                        Rum.AddError(ex, RumErrorSource.Source);
                        throw;
                    }
                }
                Rum.AddTiming("payment_processed");

                // Create order
                _logger.Info("Creating order");
                var order = await CreateOrder(cart);
                span.SetTag("order_id", order.Id);
                Rum.AddTiming("order_created");

                // Success
                Rum.AddAction(RumActionType.Custom, "checkout_completed");
                _logger.Info("Checkout completed successfully");

                Rum.StopView("checkout");
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                _logger.Error("Checkout failed", ex);
                Rum.AddError(ex);
                Rum.StopView("checkout");
                throw;
            }
        }
    }
}
```

## Dependency Injection

Use the interfaces for dependency injection and testing:

```csharp
// Register in MauiProgram.cs
builder.Services.AddSingleton<IDatadogSdk>(DatadogSdk.Instance);
builder.Services.AddSingleton<IDatadogLogger>(DatadogSdk.Logger);
builder.Services.AddSingleton<IDatadogRum>(DatadogSdk.Rum);
builder.Services.AddSingleton<IDatadogTrace>(DatadogSdk.Trace);

// Use in services
public class MyService
{
    private readonly ILogger _logger;
    private readonly IDatadogRum _rum;

    public MyService()
    {
        _logger = Logs.CreateLogger("my-service");
        _rum = DatadogSdk.Rum;
    }

    public async Task DoWork()
    {
        _logger.Info("Starting work");
        _rum.AddAction(RumActionType.Custom, "work_started");
        // ...
    }
}
```

## Best Practices

1. **Initialize Early** - Initialize Datadog as early as possible in your app lifecycle
2. **Use Named Loggers** - Create loggers for different components
3. **Add Context** - Include relevant attributes with logs and events
4. **Track View Lifecycle** - Properly start and stop views
5. **Use `using` for Spans** - Always use `using` statements for spans
6. **Respect Privacy** - Handle user consent properly

## Support

- **Documentation**: [https://kyletaylored.github.io/dd-sdk-maui/](https://kyletaylored.github.io/dd-sdk-maui/)
- **GitHub Issues**: [Report a bug](https://github.com/kyletaylored/dd-sdk-maui/issues)
- **Datadog Support**: [Contact support](https://docs.datadoghq.com/help/)

## License

Apache License 2.0 - See [LICENSE](https://github.com/kyletaylored/dd-sdk-maui/blob/main/LICENSE) for details.

## Contributing

Contributions are welcome! See the [developer documentation](https://kyletaylored.github.io/dd-sdk-maui/getting-started/installation) for setup instructions.
