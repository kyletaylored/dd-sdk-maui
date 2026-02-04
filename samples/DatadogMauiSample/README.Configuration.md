# Configuration-Based Initialization

This guide shows how to use the simplified `UseDatadogFromConfiguration()` method to initialize Datadog using `appsettings.json` without creating custom configuration classes.

## Why Use Configuration-Based Initialization?

**Traditional Approach** (current MauiProgram.cs):
- ❌ Requires custom `DatadogSettings.cs` class
- ❌ 30+ lines of manual file loading code
- ❌ Nested POCOs (PlatformSettings, RumSettings, etc.)
- ❌ String-to-enum conversion logic
- ❌ Complex configuration binding

**Configuration-Based Approach** (simplified):
- ✅ Single line: `builder.UseDatadogFromConfiguration();`
- ✅ No custom classes needed
- ✅ Automatic configuration loading
- ✅ Built-in type conversion
- ✅ Clean, maintainable code

## Quick Start

### 1. Create appsettings.json

Add `appsettings.json` to your MAUI project root:

```json
{
  "Datadog": {
    "Environment": "production",
    "ServiceName": "my-maui-app",
    "Site": "US1",
    "FirstPartyHosts": ["api.example.com"],
    "Android": {
      "ClientToken": "YOUR_ANDROID_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_ANDROID_RUM_ID"
    },
    "iOS": {
      "ClientToken": "YOUR_IOS_CLIENT_TOKEN",
      "RumApplicationId": "YOUR_IOS_RUM_ID"
    }
  }
}
```

**Important**: Set the file's Build Action to `MauiAsset`.

### 2. Initialize in MauiProgram.cs

```csharp
using Datadog.Maui.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseDatadogFromConfiguration(); // That's it!

        return builder.Build();
    }
}
```

## Configuration Options

### Required Settings

```json
{
  "Datadog": {
    "Android": {
      "ClientToken": "pub...",        // Required for Android
      "RumApplicationId": "..."       // Required for RUM on Android
    },
    "iOS": {
      "ClientToken": "pub...",        // Required for iOS
      "RumApplicationId": "..."       // Required for RUM on iOS
    }
  }
}
```

### Optional Settings

```json
{
  "Datadog": {
    "Environment": "production",      // Default: "development"
    "ServiceName": "my-app",          // Default: null
    "Site": "US1",                    // Default: "US1" (US1, US3, US5, EU1, AP1, US1_FED)
    "TrackingConsent": "Granted",     // Default: "Granted" (Granted, NotGranted, Pending)
    "FirstPartyHosts": [              // Default: empty array
      "api.example.com",
      "backend.example.com"
    ]
  }
}
```

### Feature Configuration

#### Logs (Enabled by default)

```json
{
  "Datadog": {
    "Logs": {
      "Enabled": true                 // Default: true
    }
  }
}
```

#### Tracing

```json
{
  "Datadog": {
    "Tracing": {
      "Enabled": true,                // Default: false
      "SampleRate": 100               // Optional: 0-100 (percentage)
    },
    "FirstPartyHosts": [              // Required for tracing
      "api.example.com"
    ]
  }
}
```

#### RUM (Real User Monitoring)

```json
{
  "Datadog": {
    "Rum": {
      "SessionSampleRate": 100        // Optional: 0-100 (percentage)
    }
  }
}
```

#### Session Replay

```json
{
  "Datadog": {
    "SessionReplay": {
      "Enabled": true,                            // Default: false
      "SampleRate": 20,                           // Optional: 0-100 (percentage)
      "TextAndInputPrivacy": "MaskSensitiveInputs", // MaskAll, MaskAllInputs, MaskSensitiveInputs
      "ImagePrivacy": "MaskNonBundledOnly",       // MaskAll, MaskNone, MaskNonBundledOnly
      "TouchPrivacy": "Show"                      // Show, Hide
    }
  }
}
```

## Programmatic Overrides

You can override or extend configuration programmatically:

```csharp
builder.UseDatadogFromConfiguration(configure: datadog =>
{
    // Override settings from appsettings.json
    datadog.Environment = "staging";

    // Add global tags
    datadog.GlobalTags["team"] = "mobile";

    // Enable additional features
    datadog.VerboseLogging = true;
});
```

## Custom Configuration Section

Use a different section name:

```json
{
  "Monitoring": {
    "Android": {
      "ClientToken": "..."
    }
  }
}
```

```csharp
builder.UseDatadogFromConfiguration(sectionName: "Monitoring");
```

## Environment-Specific Configuration

### Development vs. Production

Create `appsettings.Development.json`:

```json
{
  "Datadog": {
    "Environment": "development",
    "Android": {
      "ClientToken": "DEV_ANDROID_TOKEN",
      "RumApplicationId": "DEV_ANDROID_RUM_ID"
    },
    "iOS": {
      "ClientToken": "DEV_IOS_TOKEN",
      "RumApplicationId": "DEV_IOS_RUM_ID"
    }
  }
}
```

And `appsettings.Production.json`:

```json
{
  "Datadog": {
    "Environment": "production",
    "Android": {
      "ClientToken": "PROD_ANDROID_TOKEN",
      "RumApplicationId": "PROD_ANDROID_RUM_ID"
    },
    "iOS": {
      "ClientToken": "PROD_IOS_TOKEN",
      "RumApplicationId": "PROD_IOS_RUM_ID"
    }
  }
}
```

### Loading Environment-Specific Config

```csharp
var builder = MauiApp.CreateBuilder();

// Load base configuration
var configBuilder = new ConfigurationBuilder();
using (var stream = await FileSystem.OpenAppPackageFileAsync("appsettings.json"))
{
    configBuilder.AddJsonStream(stream);
}

// Load environment-specific configuration
#if DEBUG
using (var devStream = await FileSystem.OpenAppPackageFileAsync("appsettings.Development.json"))
{
    configBuilder.AddJsonStream(devStream);
}
#else
using (var prodStream = await FileSystem.OpenAppPackageFileAsync("appsettings.Production.json"))
{
    configBuilder.AddJsonStream(prodStream);
}
#endif

builder.Configuration.AddConfiguration(configBuilder.Build());

// Now use configuration-based initialization
builder.UseDatadogFromConfiguration();
```

## Complete Example

See [appsettings.Simple.json](appsettings.Simple.json) and [MauiProgram.Simple.cs](MauiProgram.Simple.cs) for a complete working example.

## Migration from Custom DatadogSettings

If you're currently using custom `DatadogSettings` classes:

### Before (Complex)

```csharp
// Requires DatadogSettings.cs, Config.cs, and manual loading
var datadogSettings = new DatadogSettings();
builder.Configuration.GetSection("Datadog").Bind(datadogSettings);

builder.UseDatadog(datadog =>
{
    datadog.SetClientToken(
        android: datadogSettings.Android.ClientToken ?? string.Empty,
        ios: datadogSettings.iOS.ClientToken ?? string.Empty
    );
    datadog.Environment = datadogSettings.Environment;
    datadog.Site = datadogSettings.Site; // Requires custom enum conversion
    // ... many more lines
});
```

### After (Simple)

```csharp
// No custom classes needed
builder.UseDatadogFromConfiguration();
```

The new method automatically:
- Reads configuration from `appsettings.json`
- Handles platform-specific values (Android/iOS)
- Converts strings to enums (Site, TrackingConsent, privacy settings)
- Enables features based on configuration
- Provides sensible defaults

## Troubleshooting

### Configuration Not Loading

Ensure `appsettings.json` has Build Action set to `MauiAsset`:

1. Right-click `appsettings.json` in Solution Explorer
2. Properties → Build Action → MauiAsset

### Missing Client Tokens

The SDK will log a warning but won't crash:

```
[Datadog] WARNING: ClientToken not configured
[Datadog] The app will run but telemetry will NOT be sent to Datadog
```

### Platform-Specific Tokens

Both Android and iOS tokens must be provided. If only one platform is provided, the other platform will fail to initialize.

## Benefits Summary

| Aspect | Traditional | Configuration-Based |
|--------|------------|---------------------|
| Lines of code | 80+ | 1 |
| Custom classes | Required | None |
| Type safety | Manual conversion | Automatic |
| Maintenance | High | Low |
| Readability | Complex | Simple |
| Error-prone | Yes | No |

## See Also

- [Main README](README.md) - Complete sample app documentation
- [Datadog.MAUI Plugin README](../../Datadog.MAUI.Plugin/README.md) - Full SDK documentation
