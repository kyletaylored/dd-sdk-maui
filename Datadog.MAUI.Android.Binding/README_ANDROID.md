# Datadog Android Bindings for .NET MAUI

Complete .NET bindings for the [Datadog Android SDK](https://docs.datadoghq.com/real_user_monitoring/android/) v2.21.0, providing Real User Monitoring (RUM), Log Collection, APM Tracing, Native Crash Reporting, Session Replay, and Feature Flags for Android applications built with .NET MAUI.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Binding
```

This includes all 13 Android binding modules (Core, Internal, RUM, Logs, Trace, NDK, SessionReplay, WebView, Flags, OkHttp, Trace.OpenTelemetry, OkHttp.OpenTelemetry, GradlePlugin).

## Quick Start

### 1. Initialize the SDK

```csharp
using Com.Datadog.Android;
using Com.Datadog.Android.Core.Configuration;

var credentials = new Credentials(
    clientToken: "YOUR_CLIENT_TOKEN",
    envName: "prod",
    variant: "release",
    rumApplicationId: "YOUR_RUM_APP_ID"
);

var configuration = new Configuration.Builder(
    clientToken: credentials.ClientToken,
    env: credentials.EnvName,
    variant: credentials.Variant
)
    .SetSite(DatadogSite.Us1)
    .Build();

Datadog.Initialize(Application.Context, credentials, configuration, TrackingConsent.Granted);
```

### 2. Enable RUM

```csharp
using Com.Datadog.Android.Rum;

var rumConfig = new RumConfiguration.Builder("YOUR_RUM_APPLICATION_ID")
    .SetSessionSampleRate(100.0f)
    .TrackUserInteractions()
    .Build();

Rum.Enable(rumConfig);

// Track views
GlobalRum.Get().StartView("home", "Home Screen", new Dictionary<string, object>());
```

### 3. Enable Logs

```csharp
using Com.Datadog.Android.Log;

var logsConfig = new LogsConfiguration.Builder()
    .SetNetworkInfoEnabled(true)
    .SetBundleWithRumEnabled(true)
    .Build();

Logs.Enable(logsConfig);

var logger = new Logger.Builder()
    .SetService("my-service")
    .SetLoggerName("MainLogger")
    .Build();

logger.I("Info message");
```

### 4. Enable Tracing

```csharp
using Com.Datadog.Android.Trace;

var traceConfig = new TraceConfiguration.Builder()
    .SetSampleRate(100.0f)
    .Build();

Trace.Enable(traceConfig);

var tracer = GlobalTracer.Get();
var span = tracer.BuildSpan("operation").Start();
span.Finish();
```

## Supported Platforms

- **Android 5.0+ (API 21+)** (net9.0-android, net10.0-android)

## Documentation

See complete API reference and examples in the main README.

- [Datadog Android Documentation](https://docs.datadoghq.com/real_user_monitoring/android/)
- [Integration Packages Guide](../docs/ANDROID_INTEGRATION_PACKAGES.md)

## Version

- Binding Version: 3.5.0
- Native Android SDK Version: 2.21.0
