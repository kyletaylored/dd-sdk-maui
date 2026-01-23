# Datadog iOS Core SDK - .NET MAUI Binding

.NET MAUI bindings for the Datadog iOS Core SDK. This is the foundation module that provides SDK initialization, configuration, and core functionality for all Datadog iOS features.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Core
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.Core" Version="3.5.0" />
```

## Overview

The Core SDK provides:
- SDK initialization and configuration
- User information management
- Tracking consent management (GDPR/CCPA compliance)
- Core data collection infrastructure
- Batch upload management

All other Datadog features (RUM, Logs, Trace, SessionReplay, CrashReporting) depend on this core module.

## Quick Start

### 1. Initialize the SDK

In your iOS `AppDelegate.cs`:

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        InitializeDatadog();
        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        // Create configuration
        var config = new DDConfiguration(
            clientToken: "YOUR_CLIENT_TOKEN",
            env: "prod"
        );

        config.Service = "com.example.myapp";
        config.Site = DDSite.Us1;
        config.BatchSize = DDBatchSize.Medium;
        config.UploadFrequency = DDUploadFrequency.Average;

        // Initialize SDK
        DDDatadog.Initialize(config, DDTrackingConsent.Granted);

        // Enable verbose logging for debugging
        DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);

        Console.WriteLine("[Datadog] SDK initialized successfully");
    }
}
```

## Core APIs

### SDK Initialization

#### Configuration Builder

```csharp
// Minimum required configuration
var config = new DDConfiguration(
    clientToken: "YOUR_CLIENT_TOKEN",  // From Datadog dashboard
    env: "prod"                        // Environment name
);

// Set optional properties
config.Service = "my-app";             // Service name
config.Site = DDSite.Us1;              // Datadog site
config.BatchSize = DDBatchSize.Medium; // Batch size
config.UploadFrequency = DDUploadFrequency.Average;
```

#### Configuration Options

```csharp
// Set Datadog site
config.Site = DDSite.Us1;      // US (datadoghq.com) - default
config.Site = DDSite.Us3;      // US3 (us3.datadoghq.com)
config.Site = DDSite.Us5;      // US5 (us5.datadoghq.com)
config.Site = DDSite.Eu1;      // EU (datadoghq.eu)
config.Site = DDSite.Us1Fed;   // FedRAMP (ddog-gov.com)
config.Site = DDSite.Ap1;      // Asia Pacific (ap1.datadoghq.com)

// Batch size - how much data to collect before uploading
config.BatchSize = DDBatchSize.Small;   // Upload more frequently, less data
config.BatchSize = DDBatchSize.Medium;  // Balanced (default)
config.BatchSize = DDBatchSize.Large;   // Upload less frequently, more data

// Upload frequency
config.UploadFrequency = DDUploadFrequency.Frequent;  // Upload often
config.UploadFrequency = DDUploadFrequency.Average;   // Balanced (default)
config.UploadFrequency = DDUploadFrequency.Rare;      // Conserve battery/bandwidth

// Service name
config.Service = "my-app";

// Additional properties
config.BundleWithRumEnabled = true;  // Bundle logs/traces with RUM
```

#### Initialize SDK

```csharp
// Initialize with configuration
DDDatadog.Initialize(
    configuration: config,
    trackingConsent: DDTrackingConsent.Granted  // Initial consent level
);

// Check if initialized
if (DDDatadog.IsInitialized)
{
    Console.WriteLine("Datadog SDK is ready");
}
```

### Tracking Consent

Manage user privacy and consent (GDPR/CCPA compliance):

```csharp
// Grant consent - SDK collects and sends data
DDDatadog.SetTrackingConsent(DDTrackingConsent.Granted);

// Pending - SDK buffers data locally until consent is granted/denied
DDDatadog.SetTrackingConsent(DDTrackingConsent.Pending);

// Not granted - SDK doesn't collect or send data
DDDatadog.SetTrackingConsent(DDTrackingConsent.NotGranted);
```

**Best Practice**: Start with `Pending` until user makes a choice:

```csharp
// On app start
DDDatadog.Initialize(config, DDTrackingConsent.Pending);

// After user accepts/declines in privacy dialog
if (userAcceptedTracking)
{
    DDDatadog.SetTrackingConsent(DDTrackingConsent.Granted);
}
else
{
    DDDatadog.SetTrackingConsent(DDTrackingConsent.NotGranted);
}
```

### User Information

Set user context for better debugging and analytics:

```csharp
// Set user information
DDDatadog.SetUserInfo(
    id: "user_123",
    name: "John Doe",
    email: "john@example.com",
    extraInfo: new NSDictionary(
        new NSString("plan"), new NSString("premium"),
        new NSString("account_age"), new NSNumber(365)
    )
);

// Clear user information (e.g., on logout)
DDDatadog.ClearUserInfo();
```

### Verbosity / Debug Logging

Control SDK logging for debugging:

```csharp
// Debug - see important operations
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);

// Info - see high-level operations only
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Info);

// Warn - see warnings and errors only
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Warn);

// Error - see errors only
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Error);

// Critical - critical errors only
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Critical);
```

**Recommendation**: Use `Debug` during development, `Warn` or higher in production.

### Clear All Data

Clear all locally stored data:

```csharp
// Clear all Datadog data (e.g., on logout or data privacy request)
DDDatadog.ClearAllData();
```

### SDK Instance Check

```csharp
// Check if SDK is initialized
if (DDDatadog.IsInitialized)
{
    Console.WriteLine("Datadog SDK is initialized");
}
else
{
    Console.WriteLine("Datadog SDK not initialized");
}
```

## Enums and Constants

### DDSite

Datadog intake sites:

```csharp
public enum DDSite
{
    Us1,     // US - datadoghq.com (default)
    Us3,     // US3 - us3.datadoghq.com
    Us5,     // US5 - us5.datadoghq.com
    Eu1,     // EU - datadoghq.eu
    Us1Fed,  // US FedRAMP - ddog-gov.com
    Ap1      // Asia Pacific - ap1.datadoghq.com
}
```

### DDTrackingConsent

User consent levels:

```csharp
public enum DDTrackingConsent
{
    Granted,     // User granted consent - collect and send data
    NotGranted,  // User declined consent - don't collect data
    Pending      // Waiting for user decision - buffer data locally
}
```

### DDBatchSize

Data batch size before upload:

```csharp
public enum DDBatchSize
{
    Small,   // Upload more frequently with less data
    Medium,  // Balanced (default)
    Large    // Upload less frequently with more data
}
```

### DDUploadFrequency

How often to upload data:

```csharp
public enum DDUploadFrequency
{
    Frequent,  // Upload often (more battery usage)
    Average,   // Balanced (default)
    Rare       // Upload infrequently (conserve battery)
}
```

### DDCoreLoggerLevel

SDK logging levels:

```csharp
public enum DDCoreLoggerLevel
{
    Debug,     // Detailed debug information
    Info,      // Informational messages
    Warn,      // Warning messages
    Error,     // Error messages
    Critical   // Critical errors only
}
```

## Complete Initialization Example

Here's a complete example from a real MAUI app:

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;

namespace DatadogMauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private const string CLIENT_TOKEN = "YOUR_CLIENT_TOKEN";
    private const string ENVIRONMENT = "prod";
    private const string SERVICE_NAME = "com.example.myapp";

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        InitializeDatadog();
        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        try
        {
            Console.WriteLine("[Datadog] Initializing for iOS");
            Console.WriteLine($"[Datadog] - Environment: {ENVIRONMENT}");
            Console.WriteLine($"[Datadog] - Service: {SERVICE_NAME}");

            // Create configuration
            var config = new DDConfiguration(CLIENT_TOKEN, ENVIRONMENT);
            config.Service = SERVICE_NAME;
            config.Site = DDSite.Us1;
            config.BatchSize = DDBatchSize.Small;
            config.UploadFrequency = DDUploadFrequency.Frequent;
            config.BundleWithRumEnabled = true;

            // Initialize SDK
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            // Enable verbose logging in debug builds
            #if DEBUG
            DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
            #else
            DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Warn);
            #endif

            // Set user info if available
            SetUserInfoIfAvailable();

            Console.WriteLine("[Datadog] Successfully initialized for iOS");
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
            DDDatadog.SetUserInfo(
                id: userId,
                name: GetStoredUserName(),
                email: GetStoredUserEmail(),
                extraInfo: new NSDictionary(
                    new NSString("app_version"), new NSString("1.0.0"),
                    new NSString("device_model"), new NSString(UIDevice.CurrentDevice.Model)
                )
            );
        }
    }

    private string GetStoredUserId() => ""; // Implement based on your app
    private string GetStoredUserName() => ""; // Implement based on your app
    private string GetStoredUserEmail() => ""; // Implement based on your app

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## Best Practices

### 1. Initialize Early

Initialize Datadog in `AppDelegate.FinishedLaunching` before any features are used:

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    InitializeDatadog();  // First thing
    return base.FinishedLaunching(app, options);
}
```

### 2. Handle Initialization Errors

Wrap initialization in try-catch to prevent app crashes:

```csharp
try
{
    DDDatadog.Initialize(config, DDTrackingConsent.Granted);
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
    DDDatadog.SetUserInfo(
        id: user.Id,
        name: user.Name,
        email: user.Email,
        extraInfo: new NSDictionary(
            new NSString("subscription_tier"), new NSString(user.SubscriptionTier)
        )
    );
}
```

### 5. Clear Data on Logout

Clear user data when logging out:

```csharp
private void OnUserLoggedOut()
{
    DDDatadog.ClearUserInfo();
    // Or clear all data
    DDDatadog.ClearAllData();
}
```

### 6. Adjust Settings for Production

Use conservative settings in production to save battery and data:

```csharp
#if DEBUG
config.BatchSize = DDBatchSize.Small;
config.UploadFrequency = DDUploadFrequency.Frequent;
#else
config.BatchSize = DDBatchSize.Large;
config.UploadFrequency = DDUploadFrequency.Average;
#endif
```

## Troubleshooting

### SDK Not Initializing

**Check console output**:
```bash
# In Xcode console or device logs
grep Datadog
```

**Common issues**:
- Invalid client token
- Network connectivity issues
- Incorrect site configuration
- Missing required dependencies

### No Data Appearing in Datadog

1. **Verify client token**: Check it matches your Datadog dashboard
2. **Check site**: Ensure site matches your account region (US1, EU1, etc.)
3. **Verify consent**: Must be `DDTrackingConsent.Granted`
4. **Check network**: Ensure app can reach Datadog servers

```csharp
// Add debug logging
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
```

### Build Errors

If you encounter build errors:

1. **Check .NET version**: Requires .NET 8.0+ with iOS workload
2. **Clean build**: `dotnet clean && dotnet build`
3. **Check dependencies**: Ensure all Datadog.MAUI.iOS.* packages are same version

## Platform Requirements

- **iOS**: 12.0+
- **.NET**: 8.0+ (net8.0-ios, net9.0-ios, or net10.0-ios)
- **Xcode**: 14.0+

## Dependencies

This binding depends on:

- **.NET iOS**: net8.0-ios or later
- **Foundation**: iOS Foundation framework
- **UIKit**: iOS UIKit framework
- **Native Datadog SDK**: DatadogCore 2.x (embedded)

## Related Modules

After initializing Core, enable additional features:

- **[DatadogRUM](../DatadogRUM/README.md)** - Real User Monitoring
- **[DatadogLogs](../DatadogLogs/README.md)** - Logging
- **[DatadogTrace](../DatadogTrace/README.md)** - APM Tracing
- **[DatadogSessionReplay](../DatadogSessionReplay/README.md)** - Session Recording
- **[DatadogCrashReporting](../DatadogCrashReporting/README.md)** - Crash Reports
- **[DatadogWebViewTracking](../DatadogWebViewTracking/README.md)** - WebView Tracking

## Resources

- [Official Datadog iOS Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [iOS Troubleshooting Guide](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/troubleshooting/ios/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)
- [Issue Tracker](https://github.com/DataDog/dd-sdk-maui/issues)

## Sources

- [GitHub - DataDog/dd-sdk-ios](https://github.com/DataDog/dd-sdk-ios)
- [Datadog iOS SDK Releases](https://github.com/DataDog/dd-sdk-ios/releases)
- [iOS RUM Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [iOS Log Collection](https://docs.datadoghq.com/logs/log_collection/ios/)

## License

This binding library is licensed under Apache 2.0. See [LICENSE](../../LICENSE) for details.

The Datadog iOS SDK is copyright Datadog, Inc.
