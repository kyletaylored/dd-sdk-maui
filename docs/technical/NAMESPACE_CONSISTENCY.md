# Namespace Consistency Analysis

## Current State

### Android Bindings
The Android bindings follow Xamarin.Android conventions and preserve the original Java package structure:

```csharp
using Com.Datadog.Android;                    // com.datadog.android
using Com.Datadog.Android.Core.Configuration; // com.datadog.android.core.configuration
using Com.Datadog.Android.Log;                // com.datadog.android.log
using Com.Datadog.Android.Rum;                // com.datadog.android.rum
using Com.Datadog.Android.Trace;              // com.datadog.android.trace
using Com.Datadog.Android.Sessionreplay;      // com.datadog.android.sessionreplay
using Com.Datadog.Android.Webview;            // com.datadog.android.webview
using Com.Datadog.Android.Ndk;                // com.datadog.android.ndk
```

**Key characteristics:**
- Java package names → C# namespaces with `Com.` prefix
- Package structure preserved exactly (lowercase `android`, `sessionreplay`, etc.)
- Class names preserved (e.g., `Datadog`, `Logs`, `Rum`, `Trace`)
- This is standard Xamarin.Android binding practice

### iOS Bindings
The iOS bindings use a custom namespace structure:

```csharp
using Datadog.iOS.DatadogCore;          // DatadogCore framework
using Datadog.iOS.DatadogRUM;           // DatadogRUM framework
using Datadog.iOS.DatadogLogs;          // DatadogLogs framework
using Datadog.iOS.DatadogTrace;         // DatadogTrace framework
using Datadog.iOS.DatadogCrashReporting;    // DatadogCrashReporting framework
using Datadog.iOS.DatadogSessionReplay;     // DatadogSessionReplay framework
using Datadog.iOS.DatadogWebViewTracking;   // DatadogWebViewTracking framework
```

**Key characteristics:**
- Custom `Datadog.iOS.*` namespace prefix (we chose this)
- Framework names included in namespace (e.g., `DatadogCore`, `DatadogRUM`)
- Class names keep native `DD` prefix (e.g., `DDDatadog`, `DDRUM`, `DDLogs`)
- This is custom - not standard Xamarin.iOS practice

## The Inconsistency

### Why It Exists

**Android (Xamarin.Android standard):**
```csharp
Com.Datadog.Android.Datadog.Initialize(...)
Com.Datadog.Android.Rum.Rum.Enable(...)
Com.Datadog.Android.Log.Logs.Enable(...)
```

**iOS (Custom choice):**
```csharp
Datadog.iOS.DatadogCore.DDDatadog.InitializeWithConfiguration(...)
Datadog.iOS.DatadogRUM.DDRUM.EnableWith(...)
Datadog.iOS.DatadogLogs.DDLogs.EnableWith(...)
```

The Android bindings look "Java-like" because they **are** Java code being called from C#.
The iOS bindings look "Swift-like" because they **are** Swift/ObjC code being called from C#.

### Standard Xamarin.iOS Practice

Following Xamarin.iOS conventions more closely, the iOS namespaces could have been:

```csharp
// Option 1: Preserve Apple's framework structure exactly
using DatadogCore;
using DatadogRUM;
using DatadogLogs;
using DatadogTrace;

// Classes would be: DDDatadog, DDRUM, DDLogs, DDTrace
```

```csharp
// Option 2: Mirror Android structure more closely
using Com.Datadog.Ios;
using Com.Datadog.Ios.Core;
using Com.Datadog.Ios.Rum;
using Com.Datadog.Ios.Logs;
```

We chose `Datadog.iOS.*` which is a middle ground but creates inconsistency.

## Why This Matters

### For Users

**Android code:**
```csharp
Com.Datadog.Android.Datadog.Initialize(this, config, TrackingConsent.Granted);
Logs.Enable(logsConfig);
Rum.Enable(rumConfig);
```

**iOS code:**
```csharp
DDDatadog.InitializeWithConfiguration(configuration, DDTrackingConsent.Granted);
DDLogs.EnableWith(logsConfig);
DDRUM.EnableWith(rumConfig);
```

The API surface is completely different:
- Different namespace structure
- Different class naming conventions (`Logs` vs `DDLogs`)
- Different method names (`Enable` vs `EnableWith`)
- Different enum names (`TrackingConsent` vs `DDTrackingConsent`)

### Our Solution: Unified Plugin

This is **exactly why we created the unified `Datadog.MAUI.Plugin`**:

```csharp
using Datadog.Maui;
using Datadog.Maui.Configuration;

// Same API for both platforms!
Datadog.Initialize(config);
Logs.Log("message");
Rum.AddAction(RumActionType.Tap, "button");
```

The unified plugin abstracts away the platform differences and provides a consistent C# API.

## Recommendations

### Short Term (Current State)

**Keep the inconsistency** because:
1. The bindings are already generated and used
2. Changing namespaces would be a breaking change
3. The unified plugin already solves the consistency problem for users
4. Platform-specific code (like sample app initialization) will naturally differ

**Document it clearly:**
- The Android bindings follow Xamarin.Android conventions (Java packages → C# namespaces)
- The iOS bindings follow our custom convention (`Datadog.iOS.*` + `DD` prefixes)
- Users should use the unified `Datadog.MAUI.Plugin` API, not platform-specific bindings
- Platform-specific initialization is only needed in `MainApplication.cs` (Android) and `AppDelegate.cs` (iOS)

### Long Term (Future Consideration)

If we want to improve consistency, we could:

1. **Regenerate iOS bindings** with namespace structure closer to Android:
   ```csharp
   // New iOS namespace structure (breaking change)
   using Com.Datadog.Ios;
   using Com.Datadog.Ios.Core;
   using Com.Datadog.Ios.Rum;
   ```

2. **Keep class names native** (they already are):
   - Android: `Datadog`, `Logs`, `Rum` (Java class names)
   - iOS: `DDDatadog`, `DDLogs`, `DDRUM` (ObjC/Swift class names)

3. **Version this as a breaking change** (v2.0.0)

However, this is **low priority** because:
- The unified plugin already provides consistency
- Most users won't touch platform-specific code
- Breaking changes are disruptive
- The current structure works fine

## Example: Side-by-Side Comparison

### Android (MainApplication.cs)
```csharp
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Log;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Trace;
using Com.Datadog.Android.Sessionreplay;

// Initialize Core SDK
var config = new Configuration.Builder(clientToken, env, variant, service).Build();
Com.Datadog.Android.Datadog.Initialize(this, config, TrackingConsent.Granted);

// Enable features
var logsConfig = new LogsConfiguration.Builder().Build();
Logs.Enable(logsConfig);

var rumConfig = new RumConfiguration.Builder(appId).Build();
Rum.Enable(rumConfig);

var traceConfig = new TraceConfiguration.Builder().Build();
Trace.Enable(traceConfig, Com.Datadog.Android.Datadog.Instance);

var sessionReplayConfig = new SessionReplayConfiguration.Builder(sampleRate).Build();
SessionReplay.Enable(sessionReplayConfig, Com.Datadog.Android.Datadog.Instance);
```

### iOS (AppDelegate.cs)
```csharp
using Datadog.iOS.DatadogCore;
using Datadog.iOS.DatadogRUM;
using Datadog.iOS.DatadogLogs;
using Datadog.iOS.DatadogTrace;
using Datadog.iOS.DatadogCrashReporting;
using Datadog.iOS.DatadogSessionReplay;

// Initialize Core SDK
var configuration = new DDConfiguration(clientToken: token, env: env);
configuration.Service = service;
DDDatadog.InitializeWithConfiguration(configuration, DDTrackingConsent.Granted);

// Enable features
DDLogs.EnableWith(new DDLogsConfiguration());

DDCrashReporting.EnableWith(DDCrashReportingConfiguration.Create());

var rumConfiguration = new DDRUMConfiguration(applicationID: appId);
DDRUM.EnableWith(rumConfiguration);

var traceConfig = new DDTraceConfiguration();
DDTrace.EnableWith(traceConfig);

var sessionReplayConfig = new DDSessionReplayConfiguration(replaySampleRate: sampleRate);
DDSessionReplay.EnableWith(sessionReplayConfig);
```

### With Unified Plugin (Recommended for Users)
```csharp
using Datadog.Maui;
using Datadog.Maui.Extensions;
using Datadog.Maui.Configuration;

builder.UseDatadog(config =>
{
    config.ClientToken = clientToken;
    config.Environment = environment;
    config.ServiceName = serviceName;
    config.TrackingConsent = TrackingConsent.Granted;

    config.EnableRum(rum =>
    {
        rum.ApplicationId = applicationId;
        rum.SessionSampleRate = 100.0f;
    });

    config.EnableLogs(logs => { });
    config.EnableTracing(tracing => { });
});
```

## Conclusion

The namespace inconsistency exists because:
1. **Android bindings** follow standard Xamarin.Android practice (Java packages → `Com.*` namespaces)
2. **iOS bindings** use our custom structure (`Datadog.iOS.*` namespaces)
3. **Both preserve native naming** (Java class names vs ObjC/Swift `DD` prefixes)

This is **acceptable** because:
- The unified plugin provides a consistent API for users
- Platform-specific initialization code is minimal and isolated
- Changing it would be a breaking change with little benefit
- Standard binding practices differ between Xamarin.Android and Xamarin.iOS anyway

**Recommendation:** Document this clearly but don't change it. Focus on the unified plugin as the primary user-facing API.
