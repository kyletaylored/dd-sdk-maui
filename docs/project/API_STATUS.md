---
layout: default
title: API Implementation Status
parent: Project
nav_order: 5
permalink: /project/api-status
---

# Datadog MAUI SDK - API Implementation Status

This document provides a comprehensive status of all Datadog features and their implementation in the .NET MAUI SDK.

## ✅ Fully Implemented Features

### Core SDK
- ✅ **SDK Initialization** - iOS & Android
- ✅ **Client Token Configuration** - Cross-platform with helper methods
- ✅ **Environment Configuration** - dev/staging/production
- ✅ **Service Name** - Application identifier
- ✅ **Site Configuration** - US1/US3/US5/EU1/AP1/US1_FED
- ✅ **Tracking Consent** - Granted/NotGranted/Pending
- ✅ **Global Tags** - Custom key-value pairs
- ✅ **Verbose Logging** - Debug output

### Real User Monitoring (RUM)
- ✅ **Application ID Configuration** - Platform-specific with helper methods
- ✅ **Session Sampling** - 0-100% sampling rate
- ✅ **Telemetry Sampling** - Internal SDK telemetry sampling
- ✅ **Automatic View Tracking** - MAUI ContentPage lifecycle (Android works, iOS uses MauiRumViewsPredicate)
- ✅ **User Interaction Tracking** - Tap/click events (MauiRumActionsPredicate on iOS, TrackUserInteractions on Android)
- ✅ **Frustration Tracking** - Rage taps, error taps, dead clicks (`TrackFrustrations`)
- ✅ **Background Event Tracking** - Track events when app is backgrounded (`TrackBackgroundEvents`)
- ✅ **Resource Tracking** - HTTP requests (automatic on Android, DatadogHttpMessageHandler on iOS)
- ✅ **Error Tracking** - Exceptions and crashes
- ✅ **Mobile Vitals** - CPU, memory, battery with configurable frequency
- ✅ **Manual View API** - `Rum.StartView()`, `Rum.StopView()`
- ✅ **Manual Action API** - `Rum.AddAction()`
- ✅ **Manual Resource API** - `Rum.StartResource()`, `Rum.StopResource()`
- ✅ **Manual Error API** - `Rum.AddError()`
- ✅ **Manual Timing API** - `Rum.AddTiming()`

### Logging
- ✅ **Logs Configuration** - Basic setup
- ✅ **Manual Logging API** - `Logs.CreateLogger()`
- ✅ **Log Levels** - Debug/Info/Notice/Warn/Error/Critical
- ✅ **Logger Attributes** - Custom attributes per log
- ✅ **Global Log Attributes** - `Logs.AddAttribute()`, `Logs.RemoveAttribute()`

### APM Tracing
- ✅ **Trace Configuration** - Sample rate, first-party hosts
- ✅ **URLSession Tracking** - iOS automatic (limited - see limitations)
- ✅ **OkHttp Interceptor** - Android automatic HTTP tracing
- ✅ **Manual Span API** - `Tracer.StartSpan()`
- ✅ **Distributed Tracing** - Parent-child spans
- ✅ **Trace Context Injection** - `Tracer.Inject()` for HTTP headers
- ✅ **Trace Context Extraction** - `Tracer.Extract()`
- ✅ **Active Span** - `Tracer.ActiveSpan`
- ✅ **Span Tags** - Custom metadata
- ✅ **Span Events** - Timeline markers
- ✅ **Span Errors** - Exception tracking

### User Management
- ✅ **Set User Info** - ID, name, email, custom attributes
- ✅ **Clear User** - Logout handling
- ✅ **User Extra Info** - Dictionary of custom user data

### Session Replay
- ✅ **Session Replay Configuration** - `EnableSessionReplay()` with sample rate, privacy settings
- ✅ **Text and Input Privacy** - MaskSensitiveInputs/Mask/Allow
- ✅ **Image Privacy** - MaskNonBundledOnly/MaskAll/MaskNone
- ✅ **Touch Privacy** - Show/Hide

### WebView Tracking
- ✅ **Android WebView Handler** - Custom handler for DatadogWebViewHandler
- ⚠️ **iOS WebView** - Native SDK supports it, needs MAUI integration

### iOS UIKit Predicates
- ✅ **UIKit Views Predicate** - `MauiRumViewsPredicate` (filters MAUI-internal controllers)
- ✅ **UIKit Actions Predicate** - `MauiRumActionsPredicate` (tracks UIControl taps, accessibility-labeled views)
- ❌ **SwiftUI Predicates** - Not applicable (MAUI does not use SwiftUI)

## ⚠️ Partially Implemented Features

### Crash Reporting
- ⚠️ **Android NDK Crashes** - Automatically enabled in native SDK
- ⚠️ **iOS Crash Reporting** - Automatically enabled in native SDK
- ⚠️ **Symbolication** - Upload tools exist but configuration not exposed
- **Status**: Works automatically, but configuration options (e.g., custom crash attributes) not exposed

### HTTP Tracing
- ✅ **Android** - Fully automatic via OkHttp interceptor
- ⚠️ **iOS** - URLSession tracking configured but `DDURLSessionInstrumentation` crashes with current SDK
  - **Workaround**: Use `DatadogHttpMessageHandler` for RUM resource tracking, or manual `Tracer.StartSpan()` for distributed tracing

## ❌ Not Applicable / Documented Limitations

### iOS URLSession Instrumentation
- ❌ **Automatic HttpClient Tracing** - `DDURLSessionInstrumentation.EnableWithConfiguration()` crashes with current SDK version
- **Alternative**:
  - Use `DatadogHttpMessageHandler` wrapper for RUM resource tracking
  - Use manual `Tracer.StartSpan()` around HTTP calls for APM distributed tracing
- **Documentation**: [HTTP Tracing Guide](../guides/http-tracing)

### Feature Flags
- ❌ **Not Implemented** - Not yet bound to native SDKs
- **Status**: Low priority, native SDK support exists

## 🚧 API Additions Needed

### 1. Crash Reporting Configuration

**What needs to be added:**

```csharp
public class CrashReportingConfiguration
{
    public bool Enabled { get; init; } = true;
    public Dictionary<string, object> CustomAttributes { get; init; } = new();

    public class Builder
    {
        public Builder Enable(bool enable) { }
        public Builder AddCustomAttribute(string key, object value) { }
        public CrashReportingConfiguration Build() { }
    }
}

// In DatadogConfigurationBuilder:
public void EnableCrashReporting(Action<CrashReportingConfiguration.Builder> configure) { }
```

**Priority**: Medium - Crashes are automatically captured, this just adds configuration

### 2. Feature Flags

**What needs to be added:**

```csharp
public static class FeatureFlags
{
    public static bool Evaluate(string featureName) { }
    public static bool Evaluate(string featureName, bool defaultValue) { }
    public static T Evaluate<T>(string featureName, T defaultValue) { }
}

public class FeatureFlagsConfiguration
{
    public class Builder
    {
        public Builder SetPollingInterval(TimeSpan interval) { }
        public FeatureFlagsConfiguration Build() { }
    }
}

// In DatadogConfigurationBuilder:
public void EnableFeatureFlags(Action<FeatureFlagsConfiguration.Builder> configure) { }
```

**Priority**: Low - Nice-to-have feature

## 📊 Platform Parity Matrix

| Feature | Android Status | iOS Status | Notes |
|---------|----------------|------------|-------|
| RUM Basic Config | ✅ Full | ✅ Full | Parity achieved |
| RUM Manual API | ✅ Full | ✅ Full | Parity achieved |
| UIKit Predicates | N/A | ✅ Full | MauiRumViewsPredicate + MauiRumActionsPredicate |
| Logs Config | ✅ Full | ✅ Full | Parity achieved |
| Logs Manual API | ✅ Full | ✅ Full | Parity achieved |
| Tracing Config | ✅ Full | ✅ Full | Parity achieved |
| Tracing Manual API | ✅ Full | ✅ Full | Parity achieved |
| Automatic HTTP | ✅ Full | ⚠️ Limited | iOS: use DatadogHttpMessageHandler |
| WebView Tracking | ✅ Full | ⚠️ Partial | iOS needs handler implementation |
| Session Replay | ✅ Full | ✅ Full | Both platforms configured via EnableSessionReplay() |
| Crash Reporting | ✅ Automatic | ✅ Automatic | Both work, config not exposed |
| User Info | ✅ Full | ✅ Full | Parity achieved |
| Global Tags | ✅ Full | ⚠️ Limited | iOS has SDK limitation |

## 🎯 Recommended Priorities

### High Priority
1. **iOS WebView Handler** - Complete cross-platform WebView support
2. **iOS HTTP Tracing** - Await next Datadog iOS SDK release with automatic swizzling

### Medium Priority
3. **Crash Reporting Configuration** - Expose custom attributes

### Low Priority
4. **Feature Flags** - Complete feature if customer demand exists

## 📝 Documentation Status

- ✅ **Quick Start Guide** - Complete
- ✅ **Builder Pattern API** - Complete
- ✅ **Manual RUM API** - Complete
- ✅ **Manual Logs API** - Complete
- ✅ **Manual Tracing API** - Complete
- ✅ **Platform-Specific Configuration** - Complete (with new helper methods)
- ✅ **iOS Limitations** - Complete
- ✅ **Session Replay** - Complete
- ✅ **UIKit Predicates** - Complete ([UIKIT_PREDICATES_ANALYSIS.md](../guides/ios/UIKIT_PREDICATES_ANALYSIS.md))
- ⚠️ **WebView Tracking** - Needs iOS handler documentation

## References

- [iOS UIKit Predicates Analysis](../guides/ios/UIKIT_PREDICATES_ANALYSIS.md)
- [HTTP Tracing Guide](../guides/http-tracing)
