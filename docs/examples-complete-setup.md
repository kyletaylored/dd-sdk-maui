---
layout: default
title: Complete Setup Example
parent: Code Examples
nav_order: 1
permalink: /examples/complete-setup
---

# Complete Setup Example

A comprehensive, single-file implementation showing how to initialize ALL Datadog features at app startup. This is a "hands-off" approach - configure everything once, and automatic instrumentation handles the rest.

{: .note }
> This example is based on the [DatadogMauiSample](https://github.com/kyletaylored/dd-sdk-maui/tree/main/samples/DatadogMauiSample) app in the repository.

---

## Table of Contents

- [Android Setup](#android-setup)
- [iOS Setup](#ios-setup)
- [Shared Configuration](#shared-configuration)
- [What Gets Tracked Automatically](#what-gets-tracked-automatically)
- [Environment Variables](#environment-variables)

---

## Android Setup

### Complete MainApplication.cs

Place this in `Platforms/Android/MainApplication.cs`:

```csharp
using Android.App;
using Android.Runtime;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Log;
using Com.Datadog.Android.Ndk;
using Com.Datadog.Android.Privacy;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Sessionreplay;
using Com.Datadog.Android.Trace;
using Com.Datadog.Android.Webview;

namespace YourApp;

[Application]
public class MainApplication : MauiApplication
{
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
            // =================================================================
            // STEP 1: Core SDK Configuration
            // =================================================================

            Console.WriteLine("[Datadog] Initializing Android SDK");

            var config = new Configuration.Builder(
                clientToken: "YOUR_CLIENT_TOKEN",      // Get from Datadog UI
                env: "production",                      // production, staging, dev, etc.
                variant: string.Empty,                  // Build variant (optional)
                serviceName: "my-maui-app"              // Your app name
            )
            .SetFirstPartyHosts(new[] {                // Enable distributed tracing
                "api.myapp.com",
                "backend.myapp.com"
            })
            .SetBatchSize(BatchSize.Medium)            // Small, Medium, Large
            .SetUploadFrequency(UploadFrequency.Average) // Frequent, Average, Rare
            .Build();

            // Initialize Core SDK with tracking consent
            Com.Datadog.Android.Datadog.Initialize(
                this,
                config,
                TrackingConsent.Granted                 // Granted, NotGranted, Pending
            );

            Console.WriteLine("[Datadog] ✓ Core SDK initialized");

            // =================================================================
            // STEP 2: Enable Verbose Logging (Optional - for debugging)
            // =================================================================

            // Uncomment to see detailed Datadog SDK logs
            // Com.Datadog.Android.Datadog.Verbosity = (int)Android.Util.LogPriority.Verbose;

            // =================================================================
            // STEP 3: Enable Log Collection
            // =================================================================

            var logsConfig = new LogsConfiguration.Builder()
                .SetEventMapper(null)                   // Optional: custom log mapper
                .Build();

            Logs.Enable(logsConfig);
            Console.WriteLine("[Datadog] ✓ Logs enabled");

            // =================================================================
            // STEP 4: Enable Native Crash Reporting (NDK)
            // =================================================================

            try
            {
                NdkCrashReports.Enable();
                Console.WriteLine("[Datadog] ✓ NDK crash reports enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ NDK crash reports failed: {ex.Message}");
            }

            // =================================================================
            // STEP 5: Enable Real User Monitoring (RUM)
            // =================================================================

            var rumConfiguration = new RumConfiguration.Builder("YOUR_RUM_APPLICATION_ID")

                // Automatic tracking features
                .TrackUserInteractions()                // Track taps, scrolls, swipes
                .TrackLongTasks(thresholdMs: 100)       // Track tasks > 100ms
                .TrackFrustrations(true)                // Track rage taps, frozen frames
                .TrackBackgroundEvents(true)            // Track events when app backgrounded
                .TrackNonFatalAnrs(true)                // Track ANRs (App Not Responding)

                // Sampling and telemetry
                .SetSessionSampleRate(100f)             // Sample 100% of sessions
                .SetTelemetrySampleRate(100f)           // Sample 100% of internal telemetry

                // Custom configuration
                .SetVitalsUpdateFrequency(               // Collect mobile vitals
                    VitalsUpdateFrequency.Average       // Frequent, Average, Rare, Never
                )
                .Build();

            Rum.Enable(rumConfiguration);
            Console.WriteLine("[Datadog] ✓ RUM enabled");

            // Initialize RUM Monitor (required for tracking)
            _ = GlobalRumMonitor.Instance;
            _ = GlobalRumMonitor.Get();

            // =================================================================
            // STEP 6: Enable Session Replay
            // =================================================================

            try
            {
                var sessionReplayConfig = new SessionReplayConfiguration.Builder(
                    replaySampleRate: 100f              // Sample 100% of sessions
                )
                // Privacy controls
                .SetTextAndInputPrivacy(
                    TextAndInputPrivacy.MaskSensitiveInputs  // Mask passwords, emails, etc.
                )
                .SetImagePrivacy(ImagePrivacy.MaskNone)      // MaskAll, MaskNone, MaskNonBundled
                .SetTouchPrivacy(TouchPrivacy.Show)          // Show, Hide
                .Build();

                SessionReplay.Enable(
                    sessionReplayConfig,
                    Com.Datadog.Android.Datadog.Instance
                );

                Console.WriteLine("[Datadog] ✓ Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ Session Replay failed: {ex.Message}");
            }

            // =================================================================
            // STEP 7: Enable APM Distributed Tracing
            // =================================================================

            try
            {
                var traceConfig = new TraceConfiguration.Builder()
                    .SetSampleRate(100f)                // Sample 100% of traces
                    .Build();

                Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);
                Console.WriteLine("[Datadog] ✓ APM Tracing enabled");

                // Note: Global tracer is automatically registered
                // HTTP requests will be automatically traced
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ APM Tracing failed: {ex.Message}");
            }

            // =================================================================
            // STEP 8: Enable WebView Tracking (Optional)
            // =================================================================

            // Note: WebView tracking requires a WebView instance
            // Enable it when you create a WebView in your app:
            //
            // WebViewTracking.Enable(
            //     webView,
            //     allowedHosts: new[] { "example.com", "myapp.com" }
            // );

            Console.WriteLine("[Datadog] ℹ WebView tracking: Call Enable() when WebView is created");

            // =================================================================
            // STEP 9: Set Global User Information (Optional)
            // =================================================================

            // Set user info after authentication
            // Com.Datadog.Android.Datadog.SetUserInfo(
            //     id: "user-123",
            //     name: "John Doe",
            //     email: "john@example.com",
            //     extraInfo: new Dictionary<string, Java.Lang.Object>
            //     {
            //         { "plan", "premium" },
            //         { "signup_date", "2024-01-15" }
            //     }
            // );

            // =================================================================
            // STEP 10: Add Global Attributes (Optional)
            // =================================================================

            // Add attributes that will be attached to ALL events
            Com.Datadog.Android.Datadog.AddUserProperties(
                new Dictionary<string, Java.Lang.Object>
                {
                    { "app_version", Android.App.Application.Context.PackageManager
                        .GetPackageInfo(Android.App.Application.Context.PackageName, 0).VersionName },
                    { "build_number", Android.App.Application.Context.PackageManager
                        .GetPackageInfo(Android.App.Application.Context.PackageName, 0).VersionCode.ToString() },
                    { "device_model", Android.OS.Build.Model },
                    { "os_version", Android.OS.Build.VERSION.Release }
                }
            );

            Console.WriteLine("[Datadog] ✓ Successfully initialized Android SDK with all features");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] ✗ Initialization failed: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

---

## iOS Setup

### Complete AppDelegate.cs

Place this in `Platforms/iOS/AppDelegate.cs`:

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.Logs;
using Datadog.iOS.Trace;
using Datadog.iOS.CrashReporting;
using Datadog.iOS.SessionReplay;
using Datadog.iOS.WebViewTracking;

namespace YourApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        InitializeDatadog();
        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        try
        {
            // =================================================================
            // STEP 1: Core SDK Configuration
            // =================================================================

            Console.WriteLine("[Datadog] Initializing iOS SDK");

            var configuration = new DDConfiguration(
                clientToken: "YOUR_CLIENT_TOKEN",       // Get from Datadog UI
                env: "production"                       // production, staging, dev, etc.
            );

            // Basic configuration
            configuration.Service = "my-maui-app";      // Your app name
            configuration.Site = DDSite.Us1;            // Us1, Us3, Us5, Eu1, Ap1, Us1_fed
            configuration.BatchSize = DDBatchSize.Medium;     // Small, Medium, Large
            configuration.UploadFrequency = DDUploadFrequency.Average; // Frequent, Average, Rare

            // First-party hosts for distributed tracing
            configuration.SetFirstPartyHostsWithHosts(new NSSet<NSString>(
                new NSString("api.myapp.com"),
                new NSString("backend.myapp.com")
            ));

            // Initialize Core SDK with tracking consent
            DDDatadog.InitializeWithConfiguration(
                configuration,
                DDTrackingConsent.Granted               // Granted, NotGranted, Pending
            );

            Console.WriteLine("[Datadog] ✓ Core SDK initialized");

            // =================================================================
            // STEP 2: Enable Verbose Logging (Optional - for debugging)
            // =================================================================

            // Uncomment to see detailed Datadog SDK logs
            // DDDatadog.VerbosityLevel = DDCoreLoggerLevel.Debug;

            // =================================================================
            // STEP 3: Enable Log Collection
            // =================================================================

            var logsConfiguration = new DDLogsConfiguration();
            // Optional: Configure log event mapper
            // logsConfiguration.EventMapper = ...;

            DDLogs.EnableWith(logsConfiguration);
            Console.WriteLine("[Datadog] ✓ Logs enabled");

            // =================================================================
            // STEP 4: Enable Crash Reporting
            // =================================================================

            try
            {
                DDCrashReporter.Enable();
                Console.WriteLine("[Datadog] ✓ Crash Reporting enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ Crash Reporting failed: {ex.Message}");
            }

            // =================================================================
            // STEP 5: Enable Real User Monitoring (RUM)
            // =================================================================

            var rumConfiguration = new DDRUMConfiguration(
                applicationID: "YOUR_RUM_APPLICATION_ID"
            );

            // Automatic tracking features
            rumConfiguration.UIKitViewsPredicate = new DDDefaultUIKitRUMViewsPredicate();
            rumConfiguration.UIKitActionsPredicate = new DDDefaultUIKitRUMActionsPredicate();
            rumConfiguration.URLSessionTracking = DDRUMURLSessionTracking.FirstPartyHostsWithTraces;

            // Advanced tracking
            rumConfiguration.TrackFrustrations = true;           // Track rage taps, frozen frames
            rumConfiguration.TrackBackgroundEvents = true;       // Track events when backgrounded
            rumConfiguration.LongTaskThreshold = 0.1;            // Track tasks > 100ms

            // Vitals monitoring
            rumConfiguration.VitalsUpdateFrequency = DDRUMVitalsFrequency.Average;

            // Sampling
            rumConfiguration.SessionSampleRate = 100.0f;         // Sample 100% of sessions
            rumConfiguration.TelemetrySampleRate = 100.0f;       // Sample 100% of internal telemetry

            DDRUM.EnableWith(rumConfiguration);
            Console.WriteLine("[Datadog] ✓ RUM enabled");

            // =================================================================
            // STEP 6: Enable Session Replay
            // =================================================================

            try
            {
                var sessionReplayConfig = new DDSessionReplayConfiguration(
                    replaySampleRate: 100.0f,            // Sample 100% of sessions

                    // Privacy controls
                    textAndInputPrivacyLevel: DDTextAndInputPrivacyLevel.SensitiveInputs,
                    imagePrivacyLevel: DDImagePrivacyLevel.None,
                    touchPrivacyLevel: DDTouchPrivacyLevel.Show
                );

                DDSessionReplay.EnableWith(sessionReplayConfig);
                Console.WriteLine("[Datadog] ✓ Session Replay enabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ Session Replay failed: {ex.Message}");
            }

            // =================================================================
            // STEP 7: Enable APM Distributed Tracing
            // =================================================================

            try
            {
                var traceConfig = new DDTraceConfiguration();
                traceConfig.SampleRate = 100.0f;         // Sample 100% of traces
                traceConfig.URLSessionTracking = DDTraceURLSessionTracking.FirstPartyHostsWithTraces;

                DDTrace.EnableWith(traceConfig);
                Console.WriteLine("[Datadog] ✓ APM Tracing enabled");

                // Note: HTTP requests to first-party hosts will be automatically traced
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Datadog] ⚠ APM Tracing failed: {ex.Message}");
            }

            // =================================================================
            // STEP 8: Enable WebView Tracking (Optional)
            // =================================================================

            // Note: WebView tracking requires a WKWebView instance
            // Enable it when you create a WebView in your app:
            //
            // DDWebViewTracking.EnableWithWebView(
            //     webView,
            //     hosts: new NSSet<NSString>(
            //         new NSString("example.com"),
            //         new NSString("myapp.com")
            //     )
            // );

            Console.WriteLine("[Datadog] ℹ WebView tracking: Call Enable() when WebView is created");

            // =================================================================
            // STEP 9: Set Global User Information (Optional)
            // =================================================================

            // Set user info after authentication
            // DDDatadog.SetUserInfoWithId(
            //     id: "user-123",
            //     name: "John Doe",
            //     email: "john@example.com",
            //     extraInfo: new NSDictionary<NSString, NSObject>(
            //         new NSString("plan"), new NSString("premium"),
            //         new NSString("signup_date"), new NSString("2024-01-15")
            //     )
            // );

            // =================================================================
            // STEP 10: Add Global Attributes (Optional)
            // =================================================================

            // Add attributes that will be attached to ALL events
            DDDatadog.AddUserExtraInfoWithExtraInfo(new NSDictionary<NSString, NSObject>(
                new NSString("app_version"),
                    new NSString(NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString()),
                new NSString("build_number"),
                    new NSString(NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString()),
                new NSString("device_model"),
                    new NSString(UIDevice.CurrentDevice.Model),
                new NSString("os_version"),
                    new NSString(UIDevice.CurrentDevice.SystemVersion)
            ));

            Console.WriteLine("[Datadog] ✓ Successfully initialized iOS SDK with all features");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] ✗ Initialization failed: {ex.Message}");
            Console.WriteLine($"[Datadog] Stack trace: {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

---

## Shared Configuration

### Configuration Helper Class (Optional)

Create a shared configuration class to manage environment-specific settings:

```csharp
// Shared/Config/DatadogConfig.cs
namespace YourApp.Config;

public static class DatadogConfig
{
    // Core configuration
    public static string Environment { get; private set; } = "production";
    public static string ServiceName { get; private set; } = "my-maui-app";
    public static string Site { get; private set; } = "US1";
    public static bool VerboseLogging { get; private set; } = false;

    // Android credentials
    public static string AndroidClientToken { get; private set; } = string.Empty;
    public static string AndroidRumApplicationId { get; private set; } = string.Empty;

    // iOS credentials
    public static string IosClientToken { get; private set; } = string.Empty;
    public static string IosRumApplicationId { get; private set; } = string.Empty;

    // First-party hosts for distributed tracing
    public static string[] FirstPartyHosts { get; private set; } = Array.Empty<string>();

    // Session replay sample rate (0-100)
    public static float SessionReplaySampleRate { get; private set; } = 100f;

    public static void LoadFromEnvironment()
    {
        // Load from environment variables
        Environment = GetEnvVar("DD_ENV", "production");
        ServiceName = GetEnvVar("DD_SERVICE", "my-maui-app");
        Site = GetEnvVar("DD_SITE", "US1");
        VerboseLogging = GetEnvVar("DD_VERBOSE", "false") == "true";

        // Android
        AndroidClientToken = GetEnvVar("DD_ANDROID_CLIENT_TOKEN", string.Empty);
        AndroidRumApplicationId = GetEnvVar("DD_ANDROID_RUM_APPLICATION_ID", string.Empty);

        // iOS
        IosClientToken = GetEnvVar("DD_IOS_CLIENT_TOKEN", string.Empty);
        IosRumApplicationId = GetEnvVar("DD_IOS_RUM_APPLICATION_ID", string.Empty);

        // First-party hosts
        var hostsStr = GetEnvVar("DD_FIRST_PARTY_HOSTS", string.Empty);
        FirstPartyHosts = string.IsNullOrEmpty(hostsStr)
            ? Array.Empty<string>()
            : hostsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);

        // Session replay
        var replayRateStr = GetEnvVar("DD_SESSION_REPLAY_SAMPLE_RATE", "100");
        SessionReplaySampleRate = float.TryParse(replayRateStr, out var rate) ? rate : 100f;
    }

    private static string GetEnvVar(string name, string defaultValue)
    {
        return System.Environment.GetEnvironmentVariable(name) ?? defaultValue;
    }
}
```

Then load it in both platform files:

```csharp
// At the top of OnCreate() or FinishedLaunching()
DatadogConfig.LoadFromEnvironment();

// Use in configuration
var config = new Configuration.Builder(
    clientToken: DatadogConfig.AndroidClientToken,
    env: DatadogConfig.Environment,
    variant: string.Empty,
    serviceName: DatadogConfig.ServiceName
)
// ... rest of config
```

---

## What Gets Tracked Automatically

Once you've completed the setup above, the following will be tracked **automatically** with no additional code required:

### ✅ RUM (Real User Monitoring)

| Feature | Android | iOS | Description |
|---------|---------|-----|-------------|
| **Views** | ✅ | ✅ | Activities/ViewControllers automatically tracked |
| **Actions** | ✅ | ✅ | Taps, scrolls, swipes on UI elements |
| **Resources** | ✅ | ✅ | HTTP requests (to first-party hosts) |
| **Errors** | ✅ | ✅ | Exceptions and crashes |
| **Long Tasks** | ✅ | ✅ | Main thread operations > threshold |
| **Frozen Frames** | ✅ | ✅ | UI freezes and janky animations |
| **Rage Taps** | ✅ | ✅ | Multiple rapid taps on same element |
| **ANRs** | ✅ | ⚠️ | Application Not Responding events |
| **Memory Usage** | ✅ | ✅ | Memory consumption metrics |
| **CPU Usage** | ✅ | ✅ | CPU utilization metrics |
| **Battery Usage** | ⚠️ | ⚠️ | Battery drain metrics |

### ✅ APM (Application Performance Monitoring)

| Feature | Android | iOS | Description |
|---------|---------|-----|-------------|
| **HTTP Tracing** | ✅ | ✅ | Automatic trace headers for first-party hosts |
| **Span Generation** | ✅ | ✅ | Automatic spans for HTTP requests |
| **Distributed Tracing** | ✅ | ✅ | End-to-end trace propagation |

### ✅ Logs

| Feature | Android | iOS | Description |
|---------|---------|-----|-------------|
| **Automatic Context** | ✅ | ✅ | Session ID, user info, view info attached |
| **Log Batching** | ✅ | ✅ | Efficient log upload |
| **Network Info** | ✅ | ✅ | Network status in logs |

### ✅ Crash Reporting

| Feature | Android | iOS | Description |
|---------|---------|-----|-------------|
| **Java/Kotlin Crashes** | ✅ | N/A | JVM crashes |
| **Swift/Obj-C Crashes** | N/A | ✅ | iOS crashes |
| **Native Crashes (C/C++)** | ✅ | ✅ | NDK/native code crashes |
| **Stack Traces** | ✅ | ✅ | Full stack traces |
| **Crash Grouping** | ✅ | ✅ | Automatic issue grouping |

### ✅ Session Replay

| Feature | Android | iOS | Description |
|---------|---------|-----|-------------|
| **Screen Recording** | ✅ | ✅ | Visual session recording |
| **Touch Recording** | ✅ | ✅ | User touch interactions |
| **Privacy Masking** | ✅ | ✅ | Automatic PII masking |
| **RUM Correlation** | ✅ | ✅ | Replay linked to RUM sessions |

---

## Environment Variables

### Development Setup (.env file)

Create a `.env` file in your project root (add to `.gitignore`!):

```bash
# Environment
DD_ENV=development
DD_SERVICE=my-maui-app
DD_SITE=US1
DD_VERBOSE=true

# Android Credentials
DD_ANDROID_CLIENT_TOKEN=pub1234567890abcdef1234567890abcd
DD_ANDROID_RUM_APPLICATION_ID=12345678-1234-1234-1234-123456789012

# iOS Credentials
DD_IOS_CLIENT_TOKEN=pub9876543210fedcba9876543210fedc
DD_IOS_RUM_APPLICATION_ID=87654321-4321-4321-4321-210987654321

# Distributed Tracing
DD_FIRST_PARTY_HOSTS=api.myapp.com,backend.myapp.com

# Session Replay
DD_SESSION_REPLAY_SAMPLE_RATE=100
```

### Production Setup (CI/CD)

Set these as environment variables in your CI/CD pipeline:

```bash
# GitHub Actions
- name: Set Datadog Environment Variables
  run: |
    echo "DD_ENV=production" >> $GITHUB_ENV
    echo "DD_SERVICE=my-maui-app" >> $GITHUB_ENV
    echo "DD_ANDROID_CLIENT_TOKEN=${{ secrets.DD_ANDROID_CLIENT_TOKEN }}" >> $GITHUB_ENV
    echo "DD_ANDROID_RUM_APPLICATION_ID=${{ secrets.DD_ANDROID_RUM_APP_ID }}" >> $GITHUB_ENV
    # ... etc
```

---

## Testing Your Setup

### 1. Run the App

```bash
# Android
dotnet build -t:Run -f net9.0-android

# iOS
dotnet build -t:Run -f net9.0-ios
```

### 2. Check Console Output

Look for initialization messages:

```
[Datadog] Initializing Android SDK
[Datadog] ✓ Core SDK initialized
[Datadog] ✓ Logs enabled
[Datadog] ✓ NDK crash reports enabled
[Datadog] ✓ RUM enabled
[Datadog] ✓ Session Replay enabled
[Datadog] ✓ APM Tracing enabled
[Datadog] ✓ Successfully initialized Android SDK with all features
```

### 3. Verify in Datadog UI

After 1-2 minutes, check the Datadog UI:

- **RUM**: [RUM Applications](https://app.datadoghq.com/rum/applications) → Your app → Sessions
- **Logs**: [Log Explorer](https://app.datadoghq.com/logs) → Filter by `service:my-maui-app`
- **APM**: [APM Services](https://app.datadoghq.com/apm/services) → Your service
- **Session Replay**: In RUM sessions, click "View Replay"

---

## Troubleshooting

### No Data Appearing

1. **Check credentials**: Ensure client tokens and application IDs are correct
2. **Check network**: Verify the device has internet connectivity
3. **Check sample rates**: Ensure sample rates are > 0
4. **Enable verbose logging**: Set verbosity level and check console output
5. **Wait 1-2 minutes**: Initial data may take time to appear

### Crashes on Startup

1. **Missing dependencies**: Ensure all NuGet packages are installed
2. **Invalid configuration**: Check for null or empty required fields
3. **Platform mismatch**: Ensure you're using platform-specific code in platform folders

### Session Replay Not Working

1. **Check sample rate**: Ensure `replaySampleRate > 0`
2. **Check privacy settings**: Overly restrictive privacy settings may hide content
3. **Android only**: Session Replay requires Android API 24+ (Android 7.0)

---

## Next Steps

Now that automatic instrumentation is set up, you can:

1. **Add custom logs**: Use `Logs.CreateLogger()` for component-specific logging
2. **Track custom events**: Use `Rum.AddAction()` for custom user actions
3. **Create custom traces**: Use `Tracer.StartSpan()` for custom distributed tracing
4. **Set user info**: Call `Datadog.SetUser()` after authentication

See the [Using the SDK](../getting-started/using-the-sdk) guide for more examples.

---

## Complete Sample App

The complete sample app with this implementation is available at:

[samples/DatadogMauiSample](https://github.com/kyletaylored/dd-sdk-maui/tree/main/samples/DatadogMauiSample)

Clone and run it to see everything in action:

```bash
git clone https://github.com/kyletaylored/dd-sdk-maui.git
cd dd-sdk-maui/samples/DatadogMauiSample

# Set your credentials
export DD_ANDROID_CLIENT_TOKEN="your-token"
export DD_ANDROID_RUM_APPLICATION_ID="your-app-id"

# Run
dotnet build -t:Run -f net9.0-android
```
