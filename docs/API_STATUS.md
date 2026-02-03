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
- ✅ **Automatic View Tracking** - MAUI ContentPage lifecycle (Android works, iOS limited)
- ✅ **User Interaction Tracking** - Tap/click events (TrackFrustrations on iOS, TrackUserInteractions on Android)
- ✅ **Resource Tracking** - HTTP requests (automatic on Android, limited on iOS)
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

### WebView Tracking
- ✅ **Android WebView Handler** - Custom handler for DatadogWebViewHandler
- ⚠️ **iOS WebView** - Native SDK supports it, needs MAUI integration

## ⚠️ Partially Implemented Features

### Session Replay
- ⚠️ **Android Session Replay** - Native SDK supports it, configuration NOT exposed in API
- ⚠️ **iOS Session Replay** - Native SDK supports it, configuration NOT exposed in API
- **Status**: Native SDKs have full support, need to add configuration builders

### Crash Reporting
- ⚠️ **Android NDK Crashes** - Automatically enabled in native SDK
- ⚠️ **iOS Crash Reporting** - Automatically enabled in native SDK
- ⚠️ **Symbolication** - Upload tools exist but configuration not exposed
- **Status**: Works automatically, but configuration options (e.g., custom crash attributes) not exposed

### HTTP Tracing
- ✅ **Android** - Fully automatic via OkHttp interceptor
- ⚠️ **iOS** - URLSession tracking configured but HttpClient doesn't use URLSession
  - **Workaround**: Use `DatadogHttpMessageHandler` or manual `Tracer.StartSpan()`

## ❌ Not Applicable / Documented Limitations

### iOS UIKit Predicates
- ❌ **UIKit Views Predicate** - Not exposed (intentional)
- ❌ **UIKit Actions Predicate** - Not exposed (intentional)
- ❌ **SwiftUI Predicates** - Not exposed (intentional)
- **Reason**: MAUI apps use MAUI abstractions, not UIKit/SwiftUI directly
- **Alternative**: Use manual RUM API (`Rum.StartView()`, `Rum.AddAction()`)
- **Documentation**: [UIKIT_PREDICATES_ANALYSIS.md](guides/ios/UIKIT_PREDICATES_ANALYSIS.md)

### iOS URLSession Instrumentation
- ❌ **Automatic HttpClient Tracing** - .NET HttpClient doesn't use native URLSession
- **Alternative**:
  - Use `DatadogHttpMessageHandler` wrapper
  - Use manual `Tracer.StartSpan()` around HTTP calls

### Feature Flags
- ❌ **Not Implemented** - Not yet bound to native SDKs
- **Status**: Low priority, native SDK support exists

## 🚧 API Additions Needed

### 1. Session Replay Configuration

**What needs to be added:**

```csharp
public class SessionReplayConfiguration
{
    public int SampleRate { get; init; } = 100;
    public TextAndInputPrivacy TextPrivacy { get; init; } = TextAndInputPrivacy.MaskSensitiveInputs;
    public ImagePrivacy ImagePrivacy { get; init; } = ImagePrivacy.MaskNone;
    public TouchPrivacy TouchPrivacy { get; init; } = TouchPrivacy.Show;

    public class Builder
    {
        public Builder SetSampleRate(int rate) { }
        public Builder SetTextAndInputPrivacy(TextAndInputPrivacy privacy) { }
        public Builder SetImagePrivacy(ImagePrivacy privacy) { }
        public Builder SetTouchPrivacy(TouchPrivacy privacy) { }
        public SessionReplayConfiguration Build() { }
    }
}

// In DatadogConfigurationBuilder:
public void EnableSessionReplay(Action<SessionReplayConfiguration.Builder> configure) { }
```

**Priority**: High - Many customers want visual session replay

### 2. Crash Reporting Configuration

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

### 3. Feature Flags

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
| Logs Config | ✅ Full | ✅ Full | Parity achieved |
| Logs Manual API | ✅ Full | ✅ Full | Parity achieved |
| Tracing Config | ✅ Full | ✅ Full | Parity achieved |
| Tracing Manual API | ✅ Full | ✅ Full | Parity achieved |
| Automatic HTTP | ✅ Full | ⚠️ Limited | iOS HttpClient doesn't use URLSession |
| WebView Tracking | ✅ Full | ⚠️ Partial | iOS needs handler implementation |
| Session Replay | ⚠️ Config Missing | ⚠️ Config Missing | Both platforms need API exposure |
| Crash Reporting | ✅ Automatic | ✅ Automatic | Both work, config not exposed |
| User Info | ✅ Full | ✅ Full | Parity achieved |
| Global Tags | ✅ Full | ⚠️ Limited | iOS has SDK limitation |

## 🎯 Recommended Priorities

### High Priority
1. **Session Replay Configuration** - High customer demand
2. **iOS WebView Handler** - Complete cross-platform WebView support
3. **iOS HTTP Tracing Guide** - Document workarounds clearly

### Medium Priority
4. **Crash Reporting Configuration** - Expose custom attributes
5. **Improve iOS Global Tags** - Find workaround or document limitation

### Low Priority
6. **Feature Flags** - Complete feature if customer demand exists

## 📝 Documentation Status

- ✅ **Quick Start Guide** - Complete
- ✅ **Builder Pattern API** - Complete
- ✅ **Manual RUM API** - Complete
- ✅ **Manual Logs API** - Complete
- ✅ **Manual Tracing API** - Complete
- ✅ **Platform-Specific Configuration** - Complete (with new helper methods)
- ✅ **iOS Limitations** - Complete
- ⚠️ **Session Replay** - Needs updating when API added
- ⚠️ **WebView Tracking** - Needs iOS handler documentation

## References

- [Feature Testing Checklist](../FEATURE_TESTING_CHECKLIST.md)
- [iOS UIKit Predicates Analysis](guides/ios/UIKIT_PREDICATES_ANALYSIS.md)
- [HTTP Tracing Guide](https://kyletaylored.github.io/dd-sdk-maui/guides/http-tracing)
