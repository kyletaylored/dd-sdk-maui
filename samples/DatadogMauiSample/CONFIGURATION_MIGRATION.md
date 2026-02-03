# Configuration Migration to .NET IConfiguration

This document explains the migration from the custom embedded resource configuration to the standard .NET `IConfiguration` pattern with `appsettings.json`.

## What Changed

### 1. Added Standard .NET Configuration Files

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development environment overrides

### 2. Configuration Structure

```json
{
  "Datadog": {
    "Environment": "dev",
    "ServiceName": "shopist-maui-demo",
    "Site": "US1",
    "VerboseLogging": true,
    "Android": {
      "ClientToken": "",
      "RumApplicationId": ""
    },
    "iOS": {
      "ClientToken": "",
      "RumApplicationId": ""
    },
    "FirstPartyHosts": [
      "fakestoreapi.com"
    ],
    "Rum": {
      "SessionSampleRate": 100,
      "TelemetrySampleRate": 100,
      "TrackUserInteractions": true
    },
    "Logs": {
      "Enabled": true
    },
    "Tracing": {
      "SampleRate": 100
    }
  }
}
```

### 3. Clean API Without #if Directives

The builder pattern now includes helper methods to eliminate `#if ANDROID / #elif IOS` blocks:

**Before:**
```csharp
#if ANDROID
    datadog.ClientToken = DatadogConfig.AndroidClientToken;
#elif IOS
    datadog.ClientToken = DatadogConfig.IosClientToken;
#endif
```

**After:**
```csharp
datadog.SetClientToken(
    android: datadogSettings.Android.ClientToken,
    ios: datadogSettings.iOS.ClientToken
);
```

### 4. MauiProgram.cs Implementation

```csharp
// Load configuration from appsettings.json
var assembly = Assembly.GetExecutingAssembly();
using var appsettingsStream = assembly.GetManifestResourceStream("DatadogMauiSample.appsettings.json");

if (appsettingsStream != null)
{
    var config = new ConfigurationBuilder()
        .AddJsonStream(appsettingsStream)
        .Build();

    builder.Configuration.AddConfiguration(config);
}

// Bind Datadog settings
var datadogSettings = new DatadogSettings();
builder.Configuration.GetSection("Datadog").Bind(datadogSettings);

// Use helper methods for clean platform-specific configuration
builder.UseDatadog(datadog =>
{
    // No #if blocks needed!
    datadog.SetClientToken(
        android: datadogSettings.Android.ClientToken ?? string.Empty,
        ios: datadogSettings.iOS.ClientToken ?? string.Empty
    );

    datadog.EnableRum(rum =>
    {
        rum.SetApplicationId(
            android: datadogSettings.Android.RumApplicationId ?? string.Empty,
            ios: datadogSettings.iOS.RumApplicationId ?? string.Empty
        );
    });
});
```

## Benefits

1. **Standard .NET Pattern**: Uses `IConfiguration` like all modern .NET applications
2. **Environment-Specific Overrides**: `appsettings.Development.json` automatically overrides base settings
3. **Cleaner Code**: No `#if` directives scattered throughout configuration code
4. **Type-Safe**: Configuration bound to strongly-typed `DatadogSettings` class
5. **Familiar**: Developers coming from ASP.NET Core will recognize this pattern immediately

## Setting Credentials

Set your Datadog credentials in `appsettings.json`:

```json
{
  "Datadog": {
    "Android": {
      "ClientToken": "your-android-client-token",
      "RumApplicationId": "your-android-rum-app-id"
    },
    "iOS": {
      "ClientToken": "your-ios-client-token",
      "RumApplicationId": "your-ios-rum-app-id"
    }
  }
}
```

**Security Note**: Never commit `appsettings.json` with real credentials to version control. Use environment-specific files or CI/CD secrets.

## Files Modified

- `DatadogMauiSample.csproj` - Added configuration packages and MauiAsset includes
- `MauiProgram.cs` - Updated to use IConfiguration and helper methods
- `Configuration/DatadogSettings.cs` (NEW) - Strongly-typed configuration model
- `Platforms/Android/MainApplication.cs` - Removed custom initialization (now handled by plugin)
- `appsettings.json` (NEW) - Base configuration
- `appsettings.Development.json` (NEW) - Development overrides

## NuGet Packages Added

- `Microsoft.Extensions.Configuration.Json` (9.0.0)
- `Microsoft.Extensions.Configuration.Binder` (9.0.0)

## Removed Dependencies

- `DatadogConfig` static class (old embedded resource approach)
- MSBuild targets that generated config files from environment variables
