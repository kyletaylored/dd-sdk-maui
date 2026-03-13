# Datadog MAUI SDK Integration Guide

## Quick Start: Simplified MainApplication.cs

Replace your current low-level binding code with the recommended high-level MAUI API:

```csharp
using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;
using MG365Mobile.UI;
using Datadog.Maui;
using Datadog.Maui.Configuration;

namespace EcolabEveryDay;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
        SetAppDimensions();
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize Datadog FIRST - before any other setup
        InitializeDatadog();
    }

    private void InitializeDatadog()
    {
        try
        {
            Console.WriteLine("[Datadog] Initializing Datadog MAUI SDK");

            // Initialize with high-level MAUI API
            Datadog.Initialize(new DatadogConfiguration
            {
                ClientToken = "pub4f12c5c92f42bf602d0111434fd8e26b",
                Environment = "INT",
                ServiceName = "EcolabEveryday",
                Site = DatadogSite.Us1,
                TrackingConsent = TrackingConsent.Granted,
                // Enable verbose logging for debugging (remove for production)
                VerboseLogging = true,
                // Global tags applied to all events
                GlobalTags = new Dictionary<string, string>
                {
                    { "app_version", GetAppVersion() },
                    { "device_model", Android.OS.Build.Model },
                    { "os_version", Android.OS.Build.VERSION.Release }
                }
            });

            Console.WriteLine("[Datadog] ✓ Datadog MAUI SDK initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] ✗ Initialization failed: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    private string GetAppVersion()
    {
        try
        {
            var context = Android.App.Application.Context;
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            return packageInfo.VersionName ?? "unknown";
        }
        catch
        {
            return "unknown";
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private void SetAppDimensions()
    {
        App.ScreenHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;
        App.ScreenWidth = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density;
        App.SafeAreaTop = 0;
        App.SafeAreaBottom = 0;
    }
}
```

## Key Changes

### 1. **Simplified Imports**
- ❌ Remove: `using Datadog.Android.RUM;` and all low-level binding imports
- ✅ Add: `using Datadog.Maui;` and `using Datadog.Maui.Configuration;`

### 2. **Unified Initialization**
- ❌ Remove: Separate calls to `Logs.Enable()`, `Rum.Enable()`, `Trace.Enable()`, etc.
- ✅ Use: Single `Datadog.Initialize()` call with `DatadogConfiguration`

### 3. **Benefits**
- **Cross-platform**: Works on both iOS and Android with same code
- **Simpler**: No need to manage low-level Android APIs
- **Type-safe**: Uses .NET enums and classes instead of Android types
- **Maintainable**: Cleaner, more readable code

## DatadogConfiguration Options

```csharp
new DatadogConfiguration
{
    // Required
    ClientToken = "your-token",          // From Datadog UI
    Environment = "prod",                // prod, staging, dev, etc.
    ServiceName = "your-app",            // Your app name
    
    // Optional
    Site = DatadogSite.Us1,              // Us1, Us3, Eu1, etc.
    TrackingConsent = TrackingConsent.Granted,  // Granted, NotGranted, Pending
    VerboseLogging = false,              // Enable debug logs
    GlobalTags = new Dictionary<string, string>
    {
        { "key", "value" }               // Tags applied to all events
    },
    FirstPartyHosts = new[] 
    { 
        "api.myapp.com"                  // For distributed tracing
    }
}
```

## What the SDK Automatically Enables

When you call `Datadog.Initialize()`, the following features are automatically enabled:

✅ **Core SDK**: Configuration and initialization  
✅ **Logs**: Log collection and sending  
✅ **RUM**: Real User Monitoring with automatic interaction tracking  
✅ **Traces**: APM tracing for performance monitoring  
✅ **Crash Reporting**: NDK crash detection  
✅ **Session Replay**: User interaction recording  
✅ **WebView Tracking**: Web content monitoring  

All with sensible defaults optimized for production use.

## Setting User Information (After Authentication)

```csharp
// After user logs in
Datadog.SetUserInfo(
    id: "user-123",
    name: "John Doe",
    email: "john@example.com",
    extraInfo: new Dictionary<string, string>
    {
        { "plan", "premium" },
        { "role", "admin" }
    }
);
```

## Logging Events

```csharp
// In your app code
using Datadog.Maui;

Datadog.Debug("Debug message");
Datadog.Info("Informational message");
Datadog.Warn("Warning message");
Datadog.Error("Error occurred", new Dictionary<string, string>
{
    { "error_code", "404" },
    { "endpoint", "/api/users" }
});
```

## Setting Up the NuGet Source

Before building, ensure your local NuGet source is configured:

```bash
# Add local source pointing to artifacts folder
dotnet nuget add source /path/to/artifacts --name LocalDatadog

# Verify
dotnet nuget source list

# Clear cache and restore
dotnet nuget locals all --clear
dotnet restore
```

## Troubleshooting

**Error: "The type or namespace name 'Core' does not exist"**
- ❌ You're using binding types directly
- ✅ Use `Datadog.Initialize()` instead

**Error: "NuGet package not found"**
- Ensure the local NuGet source is added
- Run `dotnet restore` to pull packages
- Check that package folder contains `.nupkg` files

**Build fails on Android**
- Ensure `Datadog.MAUI` is referenced in `.csproj`
- Check that all binding packages are in the NuGet source
- Clear NuGet cache: `dotnet nuget locals all --clear`

## Project File Requirements

Your `.csproj` should have:

```xml
<ItemGroup>
    <PackageReference Include="Datadog.MAUI" Version="3.5.0" />
</ItemGroup>
```

That's it! The binding packages are pulled in as dependencies automatically.
