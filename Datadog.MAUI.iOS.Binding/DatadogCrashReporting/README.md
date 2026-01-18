# Datadog iOS Crash Reporting - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS Crash Reporting. Automatically capture and report iOS app crashes with full stack traces and context.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.CrashReporting
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.CrashReporting" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` and `Datadog.MAUI.iOS.RUM` to be initialized first.

## Overview

Crash Reporting provides:
- **Automatic crash detection** for unhandled exceptions
- **Full stack traces** with file names and line numbers
- **Device and OS information** (model, OS version, memory)
- **App state** at time of crash
- **User context** (if set via Core SDK)
- **RUM session correlation** (link crashes to user sessions)
- **Crash-free rate** metrics

## Quick Start

### Enable Crash Reporting

In your `AppDelegate.cs` after Core and RUM initialization:

```csharp
using DatadogCore;
using DatadogRUM;
using DatadogCrashReporting;

// Initialize Core first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Enable RUM (required)
DDRUM.Enable(rumConfig);

// Enable Crash Reporting
DDCrashReporting.Enable();

Console.WriteLine("[Datadog] Crash Reporting enabled");
```

That's it! Crashes are now automatically reported to Datadog.

## What Gets Reported

### Captured Information

When a crash occurs, the following data is automatically captured:

- **Crash type**: Exception type and reason
- **Stack trace**: Full call stack with symbols
- **Thread information**: All threads and their states
- **Device info**: Model, architecture, memory
- **OS info**: iOS version, build number
- **App info**: Version, build, bundle ID
- **User context**: User ID, email, name (if set)
- **Custom attributes**: Global RUM attributes
- **Session context**: RUM session ID, view ID
- **Memory**: Available memory at crash time
- **Disk space**: Available disk space

### Crash Report Example

```
Exception Type: NSInvalidArgumentException
Reason: -[__NSArrayI objectAtIndex:]: index 10 beyond bounds [0 .. 4]

Stack Trace:
0  MyApp  0x0000000100e23a4c  -[ViewController loadData] + 156 (ViewController.cs:42)
1  MyApp  0x0000000100e23b2c  -[ViewController viewDidLoad] + 88 (ViewController.cs:25)
2  UIKit  0x00000001a4e23a4c  -[UIViewController loadViewIfRequired] + 1234
...

Device: iPhone 14 Pro
OS: iOS 17.0.1
Memory: 4.2 GB free of 6 GB
Disk: 45.3 GB free
Session ID: abc-def-123
User: user_123 (john@example.com)
```

## How It Works

### Crash Detection Flow

```
1. App launches
2. Crash Reporting module initializes
3. Crash handler installed
4. App runs normally
5. Unhandled exception occurs
6. Crash handler captures context
7. Crash data written to disk
8. App terminates
9. App relaunches
10. SDK detects crash report on disk
11. Crash report sent to Datadog RUM
12. Crash appears in RUM Errors
```

### Why App Must Restart

iOS crashes terminate the app process immediately. The crash report is:
1. Written to disk during crash
2. Detected and sent on next app launch

This ensures crash data is not lost.

## RUM Integration

### Automatic RUM Correlation

Crashes are automatically linked to RUM sessions:

```csharp
// Enable RUM
DDRUM.Enable(rumConfig);

// Enable Crash Reporting
DDCrashReporting.Enable();

// Crashes now include:
// - session_id: RUM session ID
// - view.id: View where crash occurred
// - User journey leading to crash
// - Session replay (if enabled)
```

### Viewing Crashes in RUM

In Datadog RUM Explorer:

1. Navigate to **RUM → Errors**
2. Filter by: `@error.type:crash`
3. Click on crash to see full details
4. View:
   - Stack trace
   - User session context
   - Session replay (if available)
   - User journey before crash
   - Device and app information

### Crash-Free Rate

Monitor app stability:

```
Crash-Free Sessions = (Total Sessions - Sessions with Crashes) / Total Sessions
```

View in Datadog:
- **RUM → Overview** - Crash-free rate percentage
- **RUM → Crashes** - List of all crashes
- **Alerts** - Set up alerts for crash rate spikes

## Complete Example

```csharp
using Foundation;
using UIKit;
using DatadogCore;
using DatadogRUM;
using DatadogCrashReporting;

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
            Console.WriteLine("[Datadog] Initializing for iOS");

            // Initialize Core SDK
            var config = new DDConfiguration(CLIENT_TOKEN, "prod");
            config.Service = "com.example.myapp";
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            Console.WriteLine("[Datadog] Core SDK initialized");

            // Enable RUM (required for crash reporting)
            var rumConfig = new DDRUMConfiguration(RUM_APPLICATION_ID);
            rumConfig.SessionSampleRate = 100.0f;
            rumConfig.TrackUIKitViews();
            rumConfig.TrackUIKitActions();
            DDRUM.Enable(rumConfig);

            Console.WriteLine("[Datadog] RUM enabled");

            // Enable Crash Reporting
            try
            {
                DDCrashReporting.Enable();
                Console.WriteLine("[Datadog] Crash Reporting enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] Crash Reporting failed: {ex.Message}");
            }

            // Set user info for better crash context
            DDDatadog.SetUserInfo(
                id: "user_123",
                name: "John Doe",
                email: "john@example.com",
                extraInfo: null
            );

            Console.WriteLine("[Datadog] Successfully initialized for iOS");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Failed to initialize: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## Testing Crash Reporting

### Trigger Test Crash

**⚠️ WARNING**: This will crash your app!

```csharp
// FOR TESTING ONLY - DO NOT USE IN PRODUCTION
public void TestCrash()
{
    Console.WriteLine("[TEST] Triggering crash...");

    // Method 1: Null reference
    string? nullString = null;
    var length = nullString.Length;  // Crash!

    // Method 2: Index out of bounds
    var array = new int[] { 1, 2, 3 };
    var value = array[10];  // Crash!

    // Method 3: Divide by zero
    var result = 42 / 0;  // Crash!

    // Method 4: Force unwrap
    throw new InvalidOperationException("Test crash");
}
```

### Verify Crash Reporting

1. Trigger test crash
2. App will terminate
3. Restart app
4. Check console for upload message:
   ```
   [Datadog] Crash report detected
   [Datadog] Uploading crash report...
   [Datadog] Crash report uploaded successfully
   ```
5. View crash in Datadog RUM Errors (may take 1-2 minutes)

## Best Practices

### 1. Enable Early

Enable crash reporting immediately after Core and RUM:

```csharp
DDDatadog.Initialize(config, consent);
DDRUM.Enable(rumConfig);          // Required
DDCrashReporting.Enable();         // Enable right after
```

### 2. Set User Context

Set user info for better crash attribution:

```csharp
DDDatadog.SetUserInfo(
    id: userId,
    name: userName,
    email: userEmail,
    extraInfo: null
);
```

### 3. Add Global Attributes

Add context to all crash reports:

```csharp
var monitor = DDRUMMonitor.Shared;
monitor.AddAttribute(new NSString("app_version"), new NSString("1.0.0"));
monitor.AddAttribute(new NSString("environment"), new NSString("production"));
monitor.AddAttribute(new NSString("feature_flags"), new NSString("new_checkout,beta_ui"));
```

### 4. Handle Gracefully

Crash reporting might not be available:

```csharp
try
{
    DDCrashReporting.Enable();
}
catch (Exception ex)
{
    // App continues normally without crash reporting
    Console.WriteLine($"Crash reporting not available: {ex.Message}");
}
```

### 5. Monitor Crash-Free Rate

Set up alerts for crash rate:

```
Alert: Crash-free rate < 99.5%
Action: Notify team immediately
```

### 6. Enable Session Replay

Combine with Session Replay to see exactly what led to crash:

```csharp
DDRUM.Enable(rumConfig);
DDSessionReplay.Enable(replayConfig);
DDCrashReporting.Enable();

// Now crashes include video replay of what happened before crash
```

## Symbolication

### Automatic Symbolication

For .NET MAUI apps, stack traces include:
- ✅ **C# method names** - Managed code methods
- ✅ **File names** - .cs file paths
- ✅ **Line numbers** - Exact line where crash occurred
- ✅ **Native frames** - iOS framework calls

### Upload Symbols (Optional)

For better symbolication of native iOS code:

1. **Build with debug symbols**:
   ```xml
   <PropertyGroup>
     <DebugType>full</DebugType>
   </PropertyGroup>
   ```

2. **Archive includes dSYM files** automatically

3. **Symbols uploaded** with app to App Store Connect

## Limitations

### What's NOT Captured

- **Handled exceptions**: Only unhandled crashes (use RUM error tracking for handled errors)
- **Crashes in iOS frameworks**: System crashes outside your app
- **Memory warnings**: Not crashes, use RUM vitals instead
- **Intentional exits**: `exit()` calls not reported as crashes

### Known Issues

- **First crash after install**: May not be reported (SDK not initialized yet)
- **Crashes during launch**: May have limited context
- **Background crashes**: May have different stack traces

## Performance Impact

Crash Reporting has minimal performance impact:

- **Initialization**: <10ms
- **Runtime overhead**: None (handlers only run on crash)
- **Crash handling**: <100ms (writing crash data)
- **App size**: <500KB

## Troubleshooting

### Crashes Not Reported

1. **Check Core SDK initialized**:
```csharp
if (!DDDatadog.IsInitialized)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check RUM enabled** (required for crash reports):
```csharp
DDRUM.Enable(rumConfig);
```

3. **Verify crash reporting enabled**:
```csharp
DDCrashReporting.Enable();
```

4. **Wait for next launch**: Crashes are sent after app restarts

5. **Check network connectivity**: Ensure device can reach Datadog

### Crashes Not Appearing in Datadog

1. **Wait 1-2 minutes**: Processing takes time
2. **Check RUM Errors**: Navigate to RUM → Errors
3. **Filter by crash**: `@error.type:crash`
4. **Verify client token**: Matches Datadog dashboard
5. **Check tracking consent**: Must be `Granted`

### Missing Stack Traces

1. **Enable debug symbols**: Ensure `DebugType=full`
2. **Check .NET version**: Use .NET 8.0+ for best symbolication
3. **Verify not stripped**: Don't use aggressive linker settings

### Duplicate Crash Reports

This is normal:
- Same crash may appear multiple times if app relaunches repeatedly
- Filter by unique crash signature

## API Reference

### DDCrashReporting

| Method | Description |
|--------|-------------|
| `Enable()` | Enable crash reporting |

**That's it!** Simple one-method API.

### Requirements

- `Datadog.MAUI.iOS.Core` must be initialized
- `Datadog.MAUI.iOS.RUM` must be enabled
- App must relaunch after crash to send report

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogRUM](../DatadogRUM/README.md)** - Required: RUM tracking (crashes reported here)
- **[DatadogSessionReplay](../DatadogSessionReplay/README.md)** - Optional: See replay leading to crash
- **[DatadogLogs](../DatadogLogs/README.md)** - Optional: Log correlation

## Resources

- [iOS Crash Reporting](https://docs.datadoghq.com/real_user_monitoring/error_tracking/ios/)
- [RUM Error Tracking](https://docs.datadoghq.com/real_user_monitoring/error_tracking/)
- [Crash Analysis](https://docs.datadoghq.com/real_user_monitoring/error_tracking/crash_reporting/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
