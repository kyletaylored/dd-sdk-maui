# Setup

Get started with Datadog Real User Monitoring (RUM) for .NET MAUI applications to visualize and analyze the real-time performance and user journeys of your mobile applications.

## Prerequisites

- **.NET 9.0 or 10.0** MAUI application
- **Android 21+** (Android 5.0 Lollipop)
- **iOS 17.0+**
- **Datadog account** with RUM enabled
- **Client tokens and Application IDs** from your [Datadog Organization Settings](https://app.datadoghq.com/organization-settings/client-tokens)

## Installation

Install the Datadog MAUI NuGet package:

```bash
dotnet add package Datadog.MAUI
```

Or via Package Manager Console:

```powershell
Install-Package Datadog.MAUI
```

## Basic Integration (Crawl)

The simplest way to get started requires just two steps:

### Step 1: Create Configuration File

Create `appsettings.json` in your project root with **Build Action: MauiAsset**:

```json
{
  "Datadog": {
    "Android": {
      "ClientToken": "YOUR_ANDROID_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_ANDROID_APPLICATION_ID"
    },
    "iOS": {
      "ClientToken": "YOUR_IOS_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_IOS_APPLICATION_ID"
    }
  }
}
```

> **Get your credentials**: Navigate to [Datadog Organization Settings → Client Tokens](https://app.datadoghq.com/organization-settings/client-tokens) and [RUM Applications](https://app.datadoghq.com/rum/list) to get your tokens and application IDs.

### Step 2: Initialize in MauiProgram.cs

Add the following to your `MauiProgram.cs` **before** `.UseMauiApp<App>()`:

```csharp
using Datadog.Maui.Extensions;
using Microsoft.Extensions.Configuration;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Load appsettings.json
        LoadAppSettings(builder);

        builder
            .UseDatadogFromConfiguration()  // ← Add this line
            .UseMauiApp<App>()
            // ... rest of your configuration

        return builder.Build();
    }

    private static void LoadAppSettings(MauiAppBuilder builder)
    {
        var configBuilder = new ConfigurationBuilder();

        try
        {
            // Load base configuration
            using var baseStream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter().GetResult();
            var baseMemoryStream = new MemoryStream();
            baseStream.CopyTo(baseMemoryStream);
            baseMemoryStream.Position = 0;
            configBuilder.AddJsonStream(baseMemoryStream);

            var config = configBuilder.Build();
            builder.Configuration.AddConfiguration(config);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Config] ERROR: {ex.Message}");
        }
    }
}
```

**That's it!** Your application now sends RUM data to Datadog.

### What's Tracked Automatically

With basic integration, Datadog automatically tracks:

- **Views**: All page navigations in your application
- **Actions**: User interactions (taps, swipes, scrolls)
- **Resources**: Network requests (HTTP/HTTPS)
- **Errors**: Unhandled exceptions and crashes
- **App Launches**: Cold and warm start times

## Standard Configuration (Walk)

Add common configuration options to tailor data collection to your needs:

```json
{
  "Datadog": {
    "Environment": "production",
    "ServiceName": "my-mobile-app",
    "Site": "US1",
    "Android": {
      "ClientToken": "YOUR_ANDROID_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_ANDROID_APPLICATION_ID"
    },
    "iOS": {
      "ClientToken": "YOUR_IOS_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_IOS_APPLICATION_ID"
    },
    "Logs": {
      "Enabled": true
    },
    "Tracing": {
      "Enabled": true,
      "SampleRate": 100
    },
    "Rum": {
      "SessionSampleRate": 100
    },
    "SessionReplay": {
      "Enabled": true,
      "SampleRate": 20,
      "TextAndInputPrivacy": "MaskSensitiveInputs",
      "ImagePrivacy": "MaskNonBundledOnly",
      "TouchPrivacy": "Show"
    },
    "FirstPartyHosts": [
      "api.example.com",
      "cdn.example.com"
    ]
  }
}
```

### Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Environment` | string | `"dev"` | Deployment environment (dev, staging, production) |
| `ServiceName` | string | App name | Service name appearing in Datadog |
| `Site` | string | `"US1"` | Datadog site: US1, US3, US5, EU1, AP1, US1_FED |
| `Logs.Enabled` | bool | `true` | Enable log collection |
| `Tracing.Enabled` | bool | `false` | Enable distributed tracing |
| `Tracing.SampleRate` | int | `100` | Percentage of traces to sample (0-100) |
| `Rum.SessionSampleRate` | int | `100` | Percentage of sessions to track (0-100) |
| `SessionReplay.Enabled` | bool | `false` | Enable Session Replay |
| `SessionReplay.SampleRate` | int | `0` | Percentage of sessions to replay (0-100) |
| `FirstPartyHosts` | string[] | `[]` | Hosts to enable distributed tracing |

### Privacy Controls

Session Replay supports three privacy levels for each data type:

**Text and Input Privacy:**
- `Allow`: Records all text and input values
- `Mask`: Masks all text with "X" characters
- `MaskSensitiveInputs`: Masks only password/email fields (recommended)

**Image Privacy:**
- `MaskAll`: Masks all images
- `MaskNonBundledOnly`: Masks only dynamic images (recommended)
- `MaskNone`: Shows all images

**Touch Privacy:**
- `Show`: Shows all touch interactions (recommended)
- `Hide`: Hides all touch interactions

## Advanced Configuration (Run)

For advanced scenarios, programmatically override configuration:

```csharp
using Datadog.Maui.Extensions;

builder.UseDatadogFromConfiguration(configure: datadog =>
{
    // Override settings programmatically
    datadog.EnableRum(rum =>
    {
        // Set build information for crash symbolication
        rum.Variant = Datadog.MAUI.Symbols.DatadogBuildInfo.Variant;
        rum.BuildId = Datadog.MAUI.Symbols.DatadogBuildInfo.BuildId;

        // Override session sample rate based on build type
#if DEBUG
        rum.SetSessionSampleRate(100); // Track all debug sessions
#else
        rum.SetSessionSampleRate(20);  // Sample 20% of production sessions
#endif
    });

    // Conditionally enable features
    if (DeviceInfo.DeviceType == DeviceType.Physical)
    {
        datadog.EnableSessionReplay(sessionReplay =>
        {
            sessionReplay.SetSampleRate(10);
            sessionReplay.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs);
        });
    }

    // Custom first-party hosts based on environment
    var apiHost = datadog.Environment == "production"
        ? "api.example.com"
        : "api-staging.example.com";

    datadog.EnableTracing(tracing =>
    {
        tracing.SetFirstPartyHosts(new[] { apiHost });
    });
});
```

### Using the SDK

After initialization, use the SDK throughout your application:

**Track Custom Views:**
```csharp
using Datadog.Maui.Rum;

Rum.StartView("checkout", "Checkout Screen", new Dictionary<string, object>
{
    { "cart_value", 99.99 },
    { "item_count", 3 }
});
```

**Track Custom Actions:**
```csharp
Rum.AddAction(RumActionType.Custom, "purchase_completed", new Dictionary<string, object>
{
    { "order_id", "12345" },
    { "total", 99.99 }
});
```

**Send Logs:**
```csharp
using Datadog.Maui.Logs;

var logger = Logs.CreateLogger("MyFeature");
logger.Info("User completed checkout", attributes: new Dictionary<string, object>
{
    { "order_id", "12345" }
});
```

**Set User Information:**
```csharp
using Datadog.Maui;

Datadog.SetUser(new UserInfo
{
    Id = "user-123",
    Name = "John Doe",
    Email = "john@example.com",
    ExtraInfo = new Dictionary<string, object>
    {
        { "plan", "premium" },
        { "account_age_days", 365 }
    }
});
```

**Distributed Tracing:**
```csharp
using Datadog.Maui.Tracing;

using (var span = Tracer.StartSpan("database_query"))
{
    span.SetTag("query_type", "SELECT");
    span.SetTag("table", "users");

    try
    {
        // Your database operation
        var result = await database.QueryAsync("SELECT * FROM users");
        span.SetTag("row_count", result.Count);
    }
    catch (Exception ex)
    {
        span.SetError(ex);
        throw;
    }
}
```

## Verify Installation

Build and run your application. You should see:

1. **Console output** (Debug builds):
   ```
   [Datadog] SDK initialized - Environment: production, Service: my-mobile-app
   ```

2. **Datadog RUM Dashboard**: Navigate to [RUM Applications](https://app.datadoghq.com/rum/list) and select your application. You should see sessions, views, and actions appearing within 1-2 minutes.

3. **Test Session Replay** (if enabled): Interact with your app for 30 seconds, then check the Session Replay tab in Datadog to see the recording.

## Next Steps

- **[Advanced Configuration](./advanced-configuration.md)**: Deep dive into all configuration options
- **[Crash Reporting](./crash-reporting.md)**: Symbolicate and debug crashes
- **[Mobile Vitals](./mobile-vitals.md)**: Monitor app performance metrics
- **[Web View Tracking](./webview-tracking.md)**: Track web content in your app
- **[Troubleshooting](./troubleshooting.md)**: Common issues and solutions

## Minimal Example

Here's a complete minimal example:

**appsettings.json:**
```json
{
  "Datadog": {
    "Android": {
      "ClientToken": "pub1234567890abcdef",
      "RumApplicationId": "12345678-1234-1234-1234-123456789012"
    },
    "iOS": {
      "ClientToken": "pub0987654321fedcba",
      "RumApplicationId": "87654321-4321-4321-4321-210987654321"
    }
  }
}
```

**MauiProgram.cs:**
```csharp
using Datadog.Maui.Extensions;
using Microsoft.Extensions.Configuration;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        LoadAppSettings(builder);

        builder
            .UseDatadogFromConfiguration()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }

    private static void LoadAppSettings(MauiAppBuilder builder)
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter().GetResult();
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var config = new ConfigurationBuilder()
                .AddJsonStream(memoryStream)
                .Build();

            builder.Configuration.AddConfiguration(config);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Config] Error: {ex.Message}");
        }
    }
}
```

That's all you need to get started with Datadog RUM in your .NET MAUI application!
