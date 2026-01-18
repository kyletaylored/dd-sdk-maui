# Datadog Android NDK Crash Reporting - .NET MAUI Binding

.NET MAUI bindings for Datadog Android NDK Crash Reporting. Capture and report native C/C++ crashes from Android NDK code.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.NDK
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.NDK" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.Android.Core` to be initialized first.

## Overview

The NDK Crash Reporting module provides:
- **Native crash detection** for C/C++ code
- **Automatic crash reporting** to Datadog RUM
- **Stack traces** from native code
- **Signal handling** (SIGSEGV, SIGABRT, etc.)
- **ANR detection** for native deadlocks

## Quick Start

### Enable NDK Crash Reporting

In your `MainApplication.cs` after Core SDK initialization:

```csharp
using Com.Datadog.Android.Ndk;

try
{
    // Enable NDK crash reports
    NdkCrashReports.Enable();

    Console.WriteLine("[Datadog] NDK crash reports enabled");
}
catch (Exception ex)
{
    Console.WriteLine($"[Datadog] NDK crash reports failed: {ex.Message}");
}
```

That's it! Native crashes are now automatically reported to Datadog.

## What Gets Reported

### Captured Information

- **Crash type**: Signal name (SIGSEGV, SIGABRT, SIGILL, etc.)
- **Stack trace**: Native call stack from crash point
- **Thread info**: Thread that crashed
- **Register state**: CPU register values at crash
- **Memory info**: Available/used memory
- **Device info**: Model, OS version, architecture
- **App info**: Version, build, process name

### Supported Signals

The following fatal signals are captured:

| Signal | Description |
|--------|-------------|
| `SIGABRT` | Abort signal (e.g., from `abort()`) |
| `SIGBUS` | Bus error (bad memory access) |
| `SIGFPE` | Floating point exception |
| `SIGILL` | Illegal instruction |
| `SIGSEGV` | Segmentation fault (invalid memory access) |
| `SIGSTKFLT` | Stack fault |
| `SIGTRAP` | Trace/breakpoint trap |

## Complete Example

```csharp
using Android.App;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Ndk;
using Com.Datadog.Android.Privacy;
using Com.Datadog.Android.Rum;

[Application]
public class MainApplication : MauiApplication
{
    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize in order: Core → RUM → NDK
        InitializeDatadogCore();
        InitializeRUM();
        InitializeNDKCrashReporting();
    }

    private void InitializeDatadogCore()
    {
        var config = new Configuration.Builder(
            "YOUR_CLIENT_TOKEN",
            "prod",
            string.Empty,
            "my-app"
        ).Build();

        Datadog.Initialize(this, config, TrackingConsent.Granted);
    }

    private void InitializeRUM()
    {
        var rumConfig = new RumConfiguration.Builder("YOUR_RUM_APP_ID")
            .Build();

        Rum.Enable(rumConfig);
    }

    private void InitializeNDKCrashReporting()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling NDK crash reports...");

            NdkCrashReports.Enable();

            Console.WriteLine("[Datadog] NDK crash reports enabled successfully");
        }
        catch (UnsatisfiedLinkError ex)
        {
            // Native library not found (expected on some devices/architectures)
            Console.WriteLine($"[Datadog] NDK crash reports not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] NDK crash reports failed: {ex.Message}");
        }
    }
}
```

## RUM Integration

When RUM is enabled, native crashes appear in:
- **RUM Error Explorer**: View/filter native crashes
- **Session Timeline**: See crash in context of user session
- **Error Details**: Full stack trace and crash info

### Viewing Native Crashes

1. Navigate to **RUM → Errors**
2. Filter by: `@error.source:native`
3. Click on error to see full details

### Crash Details Include

- Error message with signal name
- Native stack trace
- View name where crash occurred
- User session context
- Device and app information

## How It Works

### Crash Detection Flow

```
1. App loads native library
2. NDK module installs signal handlers
3. Native crash occurs (e.g., SIGSEGV)
4. Signal handler captures crash info
5. Crash data written to disk
6. App restarts
7. SDK detects crash report on disk
8. Report sent to Datadog RUM
9. Crash appears in RUM Errors
```

### Why App Must Restart

Native crashes terminate the process immediately. The crash report is:
1. Written to disk during crash
2. Detected and sent on next app start

This ensures crash data is not lost.

## Architecture Support

NDK Crash Reporting supports these CPU architectures:

- ✅ **armeabi-v7a** (32-bit ARM)
- ✅ **arm64-v8a** (64-bit ARM)
- ✅ **x86** (32-bit Intel)
- ✅ **x86_64** (64-bit Intel)

The correct native library is automatically loaded for the device architecture.

## Troubleshooting

### NDK Crash Reports Not Working

1. **Check Core SDK initialized**:
```csharp
if (Datadog.Instance == null)
{
    Console.WriteLine("Core SDK not initialized!");
}
```

2. **Check RUM enabled** (required for crash reports):
```csharp
Rum.Enable(rumConfig);
```

3. **Check for UnsatisfiedLinkError**:
```csharp
try
{
    NdkCrashReports.Enable();
}
catch (UnsatisfiedLinkError ex)
{
    // Native library not available for this architecture
    Console.WriteLine($"NDK not supported: {ex.Message}");
}
```

4. **Verify architecture support**:
```csharp
Console.WriteLine($"Device ABI: {Android.OS.Build.SupportedAbis[0]}");
```

### Crashes Not Appearing in Datadog

1. **Wait for app restart**: Crash reports send on next app launch
2. **Check RUM session**: Crash linked to RUM session
3. **Verify network**: Ensure device can reach Datadog
4. **Check filters**: In RUM, filter by `@error.source:native`

### Missing Stack Traces

Native stack traces require symbols. For better stack traces:

1. **Keep debug symbols**: Don't strip too aggressively
2. **Use NDK symbolication**: Upload symbols to Datadog (coming soon)
3. **Ensure crash is in your native code**: System crashes may have limited traces

## Testing Native Crashes

### Trigger Test Crash

**⚠️ WARNING**: This will crash your app!

```csharp
// FOR TESTING ONLY - DO NOT USE IN PRODUCTION
[DllImport("libc")]
private static extern void abort();

public void TestNativeCrash()
{
    Console.WriteLine("[TEST] Triggering native crash...");

    // This will cause SIGABRT
    abort();
}
```

### Verify Crash Reporting

1. Trigger test crash
2. App will terminate
3. Restart app
4. Check logcat for upload message
5. View crash in Datadog RUM Errors

## Best Practices

### 1. Enable Early

Enable NDK crash reporting immediately after Core SDK:

```csharp
Datadog.Initialize(this, config, consent);
Rum.Enable(rumConfig);  // Required
NdkCrashReports.Enable();  // Enable right after
```

### 2. Handle Gracefully

Native library might not be available:

```csharp
try
{
    NdkCrashReports.Enable();
}
catch (UnsatisfiedLinkError)
{
    // Not available on this device - app continues normally
    Console.WriteLine("NDK crash reporting not available");
}
```

### 3. Always Enable RUM

NDK crash reports require RUM:

```csharp
// RUM must be enabled first
Rum.Enable(rumConfig);

// Then enable NDK
NdkCrashReports.Enable();
```

### 4. Test in Debug Builds

Verify crash reporting works:

```csharp
#if DEBUG
// Add test crash button during development
#endif
```

### 5. Don't Catch Fatal Signals

Let NDK module handle crashes - don't install your own signal handlers for fatal signals.

## Limitations

### What's NOT Captured

- **Managed C# exceptions**: Use RUM error tracking instead
- **Java exceptions**: Automatically captured by Core SDK
- **Handled signals**: Only fatal, unhandled signals
- **Crashes outside your app**: System crashes not reported

### Known Issues

- **Symbol resolution**: Stack traces may show memory addresses instead of function names (symbolication coming soon)
- **Emulator support**: May not work on all emulators (works on real devices)

## Performance Impact

NDK Crash Reporting has minimal performance impact:

- **Initialization**: <5ms
- **Runtime overhead**: None (handlers only run on crash)
- **Crash handling**: <100ms (writing crash data)
- **App size**: ~500KB per supported architecture

## API Reference

### NdkCrashReports

| Method | Description |
|--------|-------------|
| `Enable()` | Enable NDK crash reporting |

**That's it!** Simple one-method API.

### Exceptions

| Exception | When Thrown |
|-----------|-------------|
| `UnsatisfiedLinkError` | Native library not found (wrong architecture or not included) |
| `RuntimeException` | Initialization failed (check Core SDK) |

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Required: RUM tracking (crashes reported here)
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Optional: Additional logging

## Resources

- [Android Crash Reporting](https://docs.datadoghq.com/real_user_monitoring/error_tracking/android/)
- [RUM Error Tracking](https://docs.datadoghq.com/real_user_monitoring/error_tracking/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
