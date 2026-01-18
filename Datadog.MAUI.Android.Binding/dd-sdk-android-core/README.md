# Datadog Android Core SDK - .NET MAUI Binding

.NET MAUI bindings for the Datadog Android Core SDK. This is the foundation module that provides SDK initialization, configuration, and core functionality for all Datadog Android features.

## Installation

Add the package via NuGet:

```bash
dotnet add package Datadog.MAUI.Android.Core
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />
```

## Overview

The Core SDK provides:
- SDK initialization and configuration
- User information management
- Tracking consent management
- Network request instrumentation
- First-party host configuration

All other Datadog features (RUM, Logs, Trace, SessionReplay) depend on this core module.

## Quick Start

### 1. Initialize the SDK

In your Android `MainApplication.cs`:

```csharp
using Android.App;
using Android.Runtime;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Privacy;

[Application]
public class MainApplication : MauiApplication
{
    public override void OnCreate()
    {
        base.OnCreate();
        InitializeDatadog();
    }

    private void InitializeDatadog()
    {
        // Create configuration
        var config = new Configuration.Builder(
            clientToken: "YOUR_CLIENT_TOKEN",
            env: "prod",
            variant: string.Empty,  // Build variant, use empty for release
            serviceName: "com.example.myapp"
        )
        .SetFirstPartyHosts(new List<string> { "api.example.com", "example.com" })
        .SetBatchSize(BatchSize.Small)
        .SetUploadFrequency(UploadFrequency.Frequent)
        .Build();

        // Initialize SDK
        Com.Datadog.Android.Datadog.Initialize(this, config, TrackingConsent.Granted);

        // Enable verbose logging for debugging
        Com.Datadog.Android.Datadog.Verbosity = (int)Android.Util.LogPriority.Verbose;

        Console.WriteLine("[Datadog] SDK initialized successfully");
    }
}
```

## Core APIs

### SDK Initialization

#### Configuration Builder

```csharp
var config = new Configuration.Builder(
    clientToken: "YOUR_CLIENT_TOKEN",  // From Datadog dashboard
    env: "prod",                       // Environment name
    variant: "",                       // Build variant (optional)
    serviceName: "my-app"              // Service name
)
.Build();
```

#### Configuration Options

```csharp
// Set Datadog site
.SetSite(DatadogSite.Us1)   // US (datadoghq.com)
.SetSite(DatadogSite.Us3)   // US3
.SetSite(DatadogSite.Us5)   // US5
.SetSite(DatadogSite.Eu1)   // EU (datadoghq.eu)
.SetSite(DatadogSite.Us1Fed) // FedRAMP
.SetSite(DatadogSite.Ap1)   // Asia Pacific

// Configure first-party hosts for network tracing
.SetFirstPartyHosts(new List<string> {
    "api.example.com",
    "example.com",
    "*.example.com"  // Wildcard supported
})

// Batch size - how much data to collect before uploading
.SetBatchSize(BatchSize.Small)   // Upload more frequently, less data
.SetBatchSize(BatchSize.Medium)  // Balanced (default)
.SetBatchSize(BatchSize.Large)   // Upload less frequently, more data

// Upload frequency
.SetUploadFrequency(UploadFrequency.Frequent)  // Upload often
.SetUploadFrequency(UploadFrequency.Average)   // Balanced (default)
.SetUploadFrequency(UploadFrequency.Rare)      // Conserve battery/bandwidth

// Proxy configuration (optional)
.SetProxy(new Proxy(Proxy.Type.Http, new InetSocketAddress("proxy.example.com", 8080)))

// Custom endpoint (optional - for EU or custom domains)
.UseSite(DatadogSite.Eu1)
```

#### Initialize SDK

```csharp
// Initialize with configuration
Com.Datadog.Android.Datadog.Initialize(
    context: this,                           // Application context
    configuration: config,                   // Configuration object
    trackingConsent: TrackingConsent.Granted // Initial consent level
);
```

### Tracking Consent

Manage user privacy and consent:

```csharp
// Grant consent - SDK collects and sends data
Com.Datadog.Android.Datadog.SetTrackingConsent(TrackingConsent.Granted);

// Pending - SDK buffers data locally until consent is granted/denied
Com.Datadog.Android.Datadog.SetTrackingConsent(TrackingConsent.Pending);

// Not granted - SDK doesn't collect or send data
Com.Datadog.Android.Datadog.SetTrackingConsent(TrackingConsent.NotGranted);
```

**Best Practice**: Start with `Pending` until user makes a choice:

```csharp
// On app start
Datadog.Initialize(this, config, TrackingConsent.Pending);

// After user accepts/declines in privacy dialog
if (userAcceptedTracking)
{
    Datadog.SetTrackingConsent(TrackingConsent.Granted);
}
else
{
    Datadog.SetTrackingConsent(TrackingConsent.NotGranted);
}
```

### User Information

Set user context for better debugging and analytics:

```csharp
// Set user information
Com.Datadog.Android.Datadog.SetUserInfo(
    id: "user_123",
    name: "John Doe",
    email: "john@example.com",
    extraInfo: new Dictionary<string, Java.Lang.Object>
    {
        { "plan", "premium" },
        { "account_age", 365 }
    }
);

// Clear user information (e.g., on logout)
Com.Datadog.Android.Datadog.ClearAllData();
```

### Verbosity / Debug Logging

Control SDK logging for debugging:

```csharp
using Android.Util;

// Verbose - see all SDK operations
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Verbose;

// Debug - see important operations
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Debug;

// Info - see high-level operations only
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Info;

// Warn - see warnings and errors only
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Warn;

// Error - see errors only
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Error;

// Assert - silent mode
Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Assert;
```

**Recommendation**: Use `Verbose` or `Debug` during development, `Warn` or higher in production.

### SDK Instance

Access the SDK instance for advanced operations:

```csharp
// Get SDK instance
var sdkInstance = Com.Datadog.Android.Datadog.Instance;

// Check if SDK is initialized
if (sdkInstance != null)
{
    Console.WriteLine("Datadog SDK is initialized");
}
```

## Enums and Constants

### DatadogSite

Datadog intake sites:

```csharp
public enum DatadogSite
{
    Us1,     // US - datadoghq.com (default)
    Us3,     // US3 - us3.datadoghq.com
    Us5,     // US5 - us5.datadoghq.com
    Eu1,     // EU - datadoghq.eu
    Us1Fed,  // US FedRAMP - ddog-gov.com
    Ap1,     // Asia Pacific - ap1.datadoghq.com
    Staging  // Datadog staging environment
}
```

### TrackingConsent

User consent levels:

```csharp
public enum TrackingConsent
{
    Granted,     // User granted consent - collect and send data
    NotGranted,  // User declined consent - don't collect data
    Pending      // Waiting for user decision - buffer data locally
}
```

### BatchSize

Data batch size before upload:

```csharp
public enum BatchSize
{
    Small,   // Upload more frequently with less data
    Medium,  // Balanced (default)
    Large    // Upload less frequently with more data
}
```

### UploadFrequency

How often to upload data:

```csharp
public enum UploadFrequency
{
    Frequent,  // Upload often (more battery usage)
    Average,   // Balanced (default)
    Rare       // Upload infrequently (conserve battery)
}
```

## Complete Initialization Example

Here's a complete example from a real MAUI app:

```csharp
using Android.App;
using Android.Runtime;
using Android.Util;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Privacy;

namespace DatadogMauiSample;

[Application]
public class MainApplication : MauiApplication
{
    private const string CLIENT_TOKEN = "YOUR_CLIENT_TOKEN";
    private const string ENVIRONMENT = "prod";
    private const string SERVICE_NAME = "com.example.myapp";

    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        InitializeDatadog();
    }

    private void InitializeDatadog()
    {
        try
        {
            Console.WriteLine("[Datadog] Initializing for Android");
            Console.WriteLine($"[Datadog] - Environment: {ENVIRONMENT}");
            Console.WriteLine($"[Datadog] - Service: {SERVICE_NAME}");

            // Create configuration
            var config = new Configuration.Builder(
                CLIENT_TOKEN,
                ENVIRONMENT,
                string.Empty,  // variant - use empty for release
                SERVICE_NAME
            )
            .SetFirstPartyHosts(new List<string> { "api.example.com" })
            .SetBatchSize(BatchSize.Small)
            .SetUploadFrequency(UploadFrequency.Frequent)
            .Build();

            // Initialize SDK
            Com.Datadog.Android.Datadog.Initialize(this, config, TrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            // Enable verbose logging in debug builds
            #if DEBUG
            Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Verbose;
            #else
            Com.Datadog.Android.Datadog.Verbosity = (int)LogPriority.Warn;
            #endif

            // Set user info if available
            SetUserInfoIfAvailable();

            Console.WriteLine("[Datadog] Successfully initialized for Android");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Failed to initialize: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    private void SetUserInfoIfAvailable()
    {
        // Example: Get user from secure storage or preferences
        var userId = GetStoredUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            Com.Datadog.Android.Datadog.SetUserInfo(
                id: userId,
                name: GetStoredUserName(),
                email: GetStoredUserEmail(),
                extraInfo: new Dictionary<string, Java.Lang.Object>
                {
                    { "app_version", Android.Content.PM.PackageInfoFlags.MatchAll },
                    { "device_model", Android.OS.Build.Model }
                }
            );
        }
    }

    private string GetStoredUserId() => ""; // Implement based on your app
    private string GetStoredUserName() => ""; // Implement based on your app
    private string GetStoredUserEmail() => ""; // Implement based on your app

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## First-Party Hosts

Configure which hosts should be traced for network requests:

```csharp
// Simple domain
.SetFirstPartyHosts(new List<string> { "api.example.com" })

// Multiple domains
.SetFirstPartyHosts(new List<string> {
    "api.example.com",
    "cdn.example.com",
    "analytics.example.com"
})

// Wildcard subdomain matching
.SetFirstPartyHosts(new List<string> {
    "*.example.com",  // Matches all subdomains
    "example.com"     // Matches exact domain
})
```

**Why First-Party Hosts Matter:**
- Network requests to these hosts are automatically traced
- Request/response details are captured in RUM
- Distributed tracing headers are added
- Allows correlation between frontend and backend traces

## Best Practices

### 1. Initialize Early

Initialize Datadog in `Application.OnCreate()` before any features are used:

```csharp
public override void OnCreate()
{
    base.OnCreate();
    InitializeDatadog();  // First thing
}
```

### 2. Handle Initialization Errors

Wrap initialization in try-catch to prevent app crashes:

```csharp
try
{
    Datadog.Initialize(this, config, TrackingConsent.Granted);
}
catch (Exception ex)
{
    Console.WriteLine($"[Datadog] Init failed: {ex.Message}");
    // App continues to work even if Datadog fails
}
```

### 3. Use Environment Variables

Don't hardcode tokens - use build configurations:

```csharp
#if DEBUG
private const string CLIENT_TOKEN = "debug_token";
private const string ENVIRONMENT = "dev";
#else
private const string CLIENT_TOKEN = "prod_token";
private const string ENVIRONMENT = "prod";
#endif
```

### 4. Set User Info After Login

Set user context after authentication for better tracking:

```csharp
private void OnUserLoggedIn(User user)
{
    Datadog.SetUserInfo(
        id: user.Id,
        name: user.Name,
        email: user.Email,
        extraInfo: new Dictionary<string, Java.Lang.Object>
        {
            { "subscription_tier", user.SubscriptionTier }
        }
    );
}
```

### 5. Clear Data on Logout

Clear user data when logging out:

```csharp
private void OnUserLoggedOut()
{
    Com.Datadog.Android.Datadog.ClearAllData();
}
```

### 6. Adjust Settings for Production

Use conservative settings in production to save battery and data:

```csharp
#if DEBUG
.SetBatchSize(BatchSize.Small)
.SetUploadFrequency(UploadFrequency.Frequent)
#else
.SetBatchSize(BatchSize.Large)
.SetUploadFrequency(UploadFrequency.Average)
#endif
```

## Troubleshooting

### SDK Not Initializing

**Check logcat output**:
```bash
adb logcat | grep Datadog
```

**Common issues**:
- Invalid client token
- Network connectivity issues
- Incorrect site configuration
- ProGuard/R8 rules missing

### No Data Appearing in Datadog

1. **Verify client token**: Check it matches your Datadog dashboard
2. **Check site**: Ensure site matches your account region (US1, EU1, etc.)
3. **Verify consent**: Must be `TrackingConsent.Granted`
4. **Check network**: Ensure app can reach Datadog servers

```csharp
// Add debug logging
Datadog.Verbosity = (int)LogPriority.Verbose;
```

### ProGuard/R8 Configuration

If using code shrinking, add these rules to `proguard-rules.pro`:

```proguard
-keep class com.datadog.android.** { *; }
-keep interface com.datadog.android.** { *; }
-keep enum com.datadog.android.** { *; }
```

## Dependencies

This binding depends on:

- **.NET Android**: net9.0-android or net10.0-android
- **Kotlin Standard Library**: Automatically included
- **AndroidX Libraries**: Automatically resolved
- **OkHttp**: For network requests (via NuGet)
- **Gson**: For JSON serialization (via NuGet)

## Related Modules

After initializing Core, enable additional features:

- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Real User Monitoring
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Logging
- **[dd-sdk-android-trace](../dd-sdk-android-trace/README.md)** - APM Tracing
- **[dd-sdk-android-session-replay](../dd-sdk-android-session-replay/README.md)** - Session Recording
- **[dd-sdk-android-ndk](../dd-sdk-android-ndk/README.md)** - Native Crash Reporting

## Resources

- [Official Datadog Android Documentation](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/android/)
- [Android Troubleshooting Guide](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/troubleshooting/android/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)
- [Issue Tracker](https://github.com/DataDog/dd-sdk-maui/issues)

## Sources

- [GitHub - DataDog/dd-sdk-android](https://github.com/DataDog/dd-sdk-android)
- [Datadog Android SDK Releases](https://github.com/DataDog/dd-sdk-android/releases)
- [Tracing Android Applications](https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/android/)
- [Android Log Collection](https://docs.datadoghq.com/logs/log_collection/android/)

## License

This binding library is licensed under Apache 2.0. See [LICENSE](../../LICENSE) for details.

The Datadog Android SDK is copyright Datadog, Inc.
