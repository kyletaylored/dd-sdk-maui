# Datadog Android Bindings for .NET MAUI

Complete .NET bindings for the Datadog Android SDK. This meta-package provides all Datadog Android modules for Real User Monitoring (RUM), Logging, APM Tracing, Session Replay, Crash Reporting, and integrations.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Binding
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.Binding" Version="3.5.0" />
```

## What's Included

This meta-package references all individual Datadog Android binding packages:

### Core Modules
- **Datadog.MAUI.Android.Internal** - Internal SDK infrastructure
- **Datadog.MAUI.Android.Core** - Core SDK initialization and configuration
- **Datadog.MAUI.Android.OpenTracingApi** - OpenTracing API support

### Feature Modules
- **Datadog.MAUI.Android.RUM** - Real User Monitoring (track sessions, views, actions, resources, errors)
- **Datadog.MAUI.Android.Logs** - Log collection and management
- **Datadog.MAUI.Android.Trace** - APM distributed tracing
- **Datadog.MAUI.Android.NDK** - Native crash reporting (C/C++)
- **Datadog.MAUI.Android.SessionReplay** - Visual session recording
- **Datadog.MAUI.Android.WebView** - WebView tracking integration
- **Datadog.MAUI.Android.Flags** - Feature flags integration

### Integration Modules
- **Datadog.MAUI.Android.OkHttp** - OkHttp HTTP client instrumentation
- **Datadog.MAUI.Android.Trace.OpenTelemetry** - OpenTelemetry tracing integration
- **Datadog.MAUI.Android.OkHttp.OpenTelemetry** - OkHttp + OpenTelemetry integration

## Target Frameworks

Supports multi-targeting for:
- **net9.0-android** (Android API 21+)
- **net10.0-android** (Android API 21+)

## Quick Start

### 1. Initialize the SDK

In your Android `MainApplication.cs`:

```csharp
using Android.App;
using Android.Runtime;
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Privacy;
using Com.Datadog.Android.Rum;

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
        var config = new Configuration.Builder(
            clientToken: "YOUR_CLIENT_TOKEN",
            env: "prod",
            variant: ""
        )
        .UseSite(DatadogSite.Us1)
        .SetBatchSize(BatchSize.Medium)
        .SetUploadFrequency(UploadFrequency.Average)
        .TrackInteractions()
        .UseViewTrackingStrategy(new ActivityViewTrackingStrategy(true))
        .Build();

        var credentials = new Credentials(
            clientToken: "YOUR_CLIENT_TOKEN",
            envName: "prod",
            variant: "",
            rumApplicationId: "YOUR_RUM_APP_ID"
        );

        Datadog.Initialize(this, credentials, config, TrackingConsent.Granted);

        // Enable RUM
        var rumConfig = new RumConfiguration.Builder("YOUR_RUM_APP_ID")
            .TrackUserInteractions()
            .TrackLongTasks(100)
            .UseViewTrackingStrategy(new ActivityViewTrackingStrategy(true))
            .Build();
        Rum.Enable(rumConfig);

        // Enable Logs
        var logsConfig = new LogsConfiguration.Builder().Build();
        Logs.Enable(logsConfig);

        // Enable Tracing
        var traceConfig = new TraceConfiguration.Builder().Build();
        Trace.Enable(traceConfig);

        Console.WriteLine("[Datadog] Android SDK initialized");
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

### 2. Track Events

```csharp
// Log messages
var logger = new Logger.Builder()
    .SetNetworkInfoEnabled(true)
    .SetLogcatLogsEnabled(true)
    .SetBundleWithTraceEnabled(true)
    .SetName("MyApp")
    .Build();

logger.D("Debug message");
logger.I("Info message");
logger.W("Warning message");
logger.E("Error message");

// Track RUM events
GlobalRumMonitor.Get().StartView("key", "Screen Name", null);
GlobalRumMonitor.Get().AddAction(RumActionType.Tap, "Button Clicked", null);
GlobalRumMonitor.Get().StopView("key", null);

// Trace HTTP requests
var span = GlobalTracer.Get().BuildSpan("api.request").Start();
try
{
    // Make API call
    span.SetTag("http.status_code", 200);
}
finally
{
    span.Finish();
}
```

## Individual Package Documentation

For detailed documentation on specific modules, see the README files in each package directory:

- Core SDK
- RUM (Real User Monitoring)
- Logs
- Trace (APM)
- Session Replay
- NDK Crash Reporting
- WebView Tracking
- Feature Flags

## Using Individual Packages

If you only need specific features, you can reference individual packages instead:

```xml
<!-- Core only -->
<PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />

<!-- Core + RUM -->
<PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />
<PackageReference Include="Datadog.MAUI.Android.RUM" Version="3.5.0" />

<!-- Core + Logs + Trace -->
<PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />
<PackageReference Include="Datadog.MAUI.Android.Logs" Version="3.5.0" />
<PackageReference Include="Datadog.MAUI.Android.Trace" Version="3.5.0" />
```

## Platform Requirements

- **Android**: API 21 (Android 5.0 Lollipop) or higher
- **.NET**: 9.0+ with Android workload installed
- **Build Tools**: Android SDK Build-Tools 34.0.0+

## Dependencies

This binding depends on:
- Native Datadog Android SDK 3.5.0
- Xamarin.AndroidX libraries
- Xamarin.Kotlin.StdLib
- OkHttp 4.12.0 (for networking)
- Gson (for JSON serialization)

## Key Features

### Real User Monitoring (RUM)
- Automatic activity and fragment tracking
- User interaction tracking (taps, scrolls, swipes)
- Resource tracking for network requests
- Error and crash tracking
- Performance monitoring (long tasks, ANRs)

### Logging
- Structured logging with multiple severity levels
- Automatic context enrichment (user info, session ID)
- Log correlation with RUM and traces
- Custom attributes and tags
- Network status tracking

### APM Tracing
- Distributed tracing across services
- Automatic span generation
- Custom instrumentation support
- OpenTelemetry integration
- Trace correlation with RUM and logs

### Session Replay
- Visual recording of user sessions
- Privacy masking controls
- Automatic screenshot capture
- Touch interaction recording
- Synchronized with RUM events

### NDK Crash Reporting
- Native crash detection (C/C++)
- Stack trace collection
- Crash correlation with RUM sessions
- Symbol upload support

## Privacy and Compliance

Control data collection and user privacy:

```csharp
// Start with pending consent
Datadog.Initialize(this, credentials, config, TrackingConsent.Pending);

// Grant consent after user accepts
Datadog.SetTrackingConsent(TrackingConsent.Granted);

// Revoke consent if user declines
Datadog.SetTrackingConsent(TrackingConsent.NotGranted);
```

## Resources

- [Datadog Android Documentation](https://docs.datadoghq.com/real_user_monitoring/android/)
- [Android RUM Setup](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/setup/android/)
- [Android Log Collection](https://docs.datadoghq.com/logs/log_collection/android/)
- [Android APM Tracing](https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/android/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## Support

For issues and questions:
- [GitHub Issues](https://github.com/DataDog/dd-sdk-maui/issues)
- [Datadog Support](https://docs.datadoghq.com/help/)

## License

Apache 2.0. See [LICENSE](../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
