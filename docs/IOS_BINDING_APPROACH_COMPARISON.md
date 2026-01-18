# iOS Binding Approach Comparison

This document compares two approaches for completing the iOS bindings: fixing all generated bindings vs. creating minimal manual bindings.

## Option A: Fix All Generated Bindings (Complete API Coverage)

### What We Have

Objective Sharpie generated **684 API types** across all frameworks:

- **DatadogCore**: 22 interfaces (configuration, SDK initialization, networking)
- **DatadogInternal**: Internal utilities (loggers, storage, threading)
- **DatadogRUM**: RUM monitoring APIs (events, views, actions, resources)
- **DatadogLogs**: Logging APIs
- **DatadogTrace**: APM tracing APIs
- **DatadogSessionReplay**: Session replay configuration
- **DatadogCrashReporting**: Crash reporting
- **DatadogWebViewTracking**: WebView instrumentation

### Example of Generated Binding

```csharp
// From Generated/DatadogCore/ApiDefinitions.cs
namespace Datadog.iOS.DatadogCore
{
    // @interface DDConfiguration : NSObject
    [BaseType (typeof(NSObject))]
    [DisableDefaultCtor]
    interface DDConfiguration
    {
        [Export ("clientToken")]
        string ClientToken { get; set; }

        [Export ("env")]
        string Env { get; set; }

        [Export ("site", ArgumentSemantic.Strong)]
        DDSite Site { get; set; }

        [NullAllowed, Export ("service")]
        string Service { get; set; }

        // ... ~20 more properties for batch size, upload frequency, encryption, etc.
    }

    [BaseType (typeof(NSObject))]
    interface DDDatadog
    {
        [Static]
        [Export ("initializeWithConfiguration:trackingConsent:")]
        void InitializeWithConfiguration (DDConfiguration configuration, DDTrackingConsent trackingConsent);

        [Static]
        [Export ("verbosityLevel")]
        [Verify (MethodToProperty)]  // <-- NEEDS FIXING
        DDCoreLoggerLevel VerbosityLevel { get; set; }

        // ... ~15 more methods for SDK management
    }

    // ... 20 more interfaces with hundreds of methods/properties
}
```

### Issues to Fix (42 errors)

1. **[Verify] attributes** (~12 instances)
   - Sharpie flags methods that might be properties
   - Need manual review to decide method vs property
   - Example: `VerbosityLevel` should be a property

2. **Missing using directives** (~6 errors)
   - `using DatadogCore;` missing
   - `using Foundation;` NSURLSessionDataDelegate not imported

3. **Duplicate constructors** (~2 errors)
   - Some classes have multiple init methods that map to same C# constructor
   - Need to use `[Export("initWith...")]` with different selectors

4. **Internal dependencies** (~10 errors)
   - `DDCoreLoggerLevel` enum defined in Internal module
   - Need proper project references between binding modules

5. **Namespace consolidation**
   - Generated as `Datadog.iOS.DatadogCore`, `Datadog.iOS.DatadogRUM`, etc.
   - Should all be in `Datadog.iOS` namespace

### Pros

✅ **Complete API coverage** - All 684 types available to users
✅ **Future-proof** - Won't need to add bindings later
✅ **Auto-generated** - Most work already done by Sharpie
✅ **Matches native SDKs** - 1:1 mapping with iOS APIs

### Cons

❌ **Time-consuming** - Need to fix 42 compilation errors
❌ **Large surface area** - Users might get overwhelmed
❌ **Internal APIs exposed** - Some APIs aren't meant for public use
❌ **Maintenance burden** - More code to maintain and document

### Estimated Effort

- **Fixing errors**: 3-4 hours
- **Testing**: 1-2 hours
- **Documentation**: 4-6 hours (documenting 684 types!)
- **Total**: ~8-12 hours

---

## Option B: Minimal Manual Bindings (Core APIs Only)

### What Users Actually Need

Based on the Android sample app, users need these **core scenarios**:

#### 1. SDK Initialization (Required)
```csharp
// Configuration
var config = new DDConfiguration(clientToken, environment);
config.Service = "my-app";
config.Site = DDSite.US1;

// Initialize SDK
DDDatadog.Initialize(config, DDTrackingConsent.Granted);
DDDatadog.SetVerbosityLevel(DDCoreLoggerLevel.Debug);
```

#### 2. RUM Monitoring (Primary use case)
```csharp
// Enable RUM
var rumConfig = new DDRUMConfiguration(applicationId);
DDRUM.Enable(rumConfig);

// Track views, actions, errors
DDRUMMonitor.Shared.StartView("HomeView");
DDRUMMonitor.Shared.AddAction("ButtonTap");
DDRUMMonitor.Shared.AddError("Error message", error);
```

#### 3. Logging (Common)
```csharp
// Enable logs
var logsConfig = new DDLogsConfiguration();
DDLogs.Enable(logsConfig);

// Create logger
var logger = DDLogger.Create();
logger.Info("App started");
logger.Error("Something failed", error);
```

#### 4. APM Tracing (Common)
```csharp
// Enable tracing
var traceConfig = new DDTraceConfiguration();
DDTrace.Enable(traceConfig);

// Create spans
var span = DDTracer.Shared.StartSpan("operation");
span.Finish();
```

#### 5. Session Replay (Optional)
```csharp
var replayConfig = new DDSessionReplayConfiguration(sampleRate);
replayConfig.SetPrivacy(DDTextAndInputPrivacy.MaskSensitiveInputs);
DDSessionReplay.Enable(replayConfig);
```

### Manual Binding Example

```csharp
// ApiDefinition.cs - MANUALLY WRITTEN
using System;
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS
{
    // ================================================
    // CORE SDK - Initialization and Configuration
    // ================================================

    [BaseType(typeof(NSObject))]
    interface DDConfiguration
    {
        // Constructor
        [Export("initWithClientToken:env:")]
        NativeHandle Constructor(string clientToken, string env);

        // Essential properties users need
        [Export("service")]
        string Service { get; set; }

        [Export("site", ArgumentSemantic.Strong)]
        DDSite Site { get; set; }

        [NullAllowed, Export("version")]
        string Version { get; set; }

        // Advanced configuration (only what users actually use)
        [Export("batchSize", ArgumentSemantic.Assign)]
        DDBatchSize BatchSize { get; set; }

        [Export("uploadFrequency", ArgumentSemantic.Assign)]
        DDUploadFrequency UploadFrequency { get; set; }
    }

    [BaseType(typeof(NSObject))]
    interface DDDatadog
    {
        // Initialize SDK - most important API
        [Static]
        [Export("initializeWithConfiguration:trackingConsent:")]
        void Initialize(DDConfiguration configuration, DDTrackingConsent trackingConsent);

        // Set verbosity for debugging
        [Static]
        [Export("setVerbosityLevel:")]
        void SetVerbosityLevel(DDCoreLoggerLevel level);

        // User info
        [Static]
        [Export("setUserInfo:")]
        void SetUserInfo([NullAllowed] DDUserInfo userInfo);

        // Tracking consent
        [Static]
        [Export("setTrackingConsent:")]
        void SetTrackingConsent(DDTrackingConsent consent);
    }

    // ================================================
    // RUM - Real User Monitoring
    // ================================================

    [BaseType(typeof(NSObject))]
    interface DDRUMConfiguration
    {
        [Export("initWithApplicationID:")]
        NativeHandle Constructor(string applicationId);

        [Export("uiKitViewsPredicate", ArgumentSemantic.Strong)]
        IDDUIKitRUMViewsPredicate UIKitViewsPredicate { get; set; }

        [Export("uiKitActionsPredicate", ArgumentSemantic.Strong)]
        IDDUIKitRUMActionsPredicate UIKitActionsPredicate { get; set; }

        [Export("longTaskThreshold", ArgumentSemantic.Assign)]
        double LongTaskThreshold { get; set; }

        [Export("sessionSampleRate", ArgumentSemantic.Assign)]
        float SessionSampleRate { get; set; }
    }

    [BaseType(typeof(NSObject))]
    interface DDRUM
    {
        [Static]
        [Export("enable:")]
        void Enable(DDRUMConfiguration configuration);
    }

    [BaseType(typeof(NSObject))]
    interface DDRUMMonitor
    {
        [Static]
        [Export("shared")]
        DDRUMMonitor Shared { get; }

        // View tracking
        [Export("startView:name:attributes:")]
        void StartView(string viewController, string name, [NullAllowed] NSDictionary attributes);

        [Export("stopView:attributes:")]
        void StopView(string viewController, [NullAllowed] NSDictionary attributes);

        // Action tracking
        [Export("addAction:attributes:")]
        void AddAction(string name, [NullAllowed] NSDictionary attributes);

        // Error tracking
        [Export("addError:message:source:stack:attributes:file:line:")]
        void AddError(NSError error, string message, DDRUMErrorSource source, [NullAllowed] string stack, [NullAllowed] NSDictionary attributes, [NullAllowed] string file, nuint line);

        // Resource tracking
        [Export("startResource:httpMethod:url:attributes:")]
        void StartResource(string resourceKey, string httpMethod, NSUrl url, [NullAllowed] NSDictionary attributes);

        [Export("stopResource:statusCode:size:attributes:")]
        void StopResource(string resourceKey, nint statusCode, long size, [NullAllowed] NSDictionary attributes);

        [Export("stopResourceWithError:error:response:attributes:")]
        void StopResourceWithError(string resourceKey, NSError error, [NullAllowed] NSUrlResponse response, [NullAllowed] NSDictionary attributes);
    }

    // ================================================
    // LOGS - Logging
    // ================================================

    [BaseType(typeof(NSObject))]
    interface DDLogsConfiguration
    {
        [Export("init")]
        NativeHandle Constructor();

        [Export("eventMapper", ArgumentSemantic.Strong)]
        Func<DDLogEvent, DDLogEvent> EventMapper { get; set; }
    }

    [BaseType(typeof(NSObject))]
    interface DDLogs
    {
        [Static]
        [Export("enable:")]
        void Enable(DDLogsConfiguration configuration);
    }

    [BaseType(typeof(NSObject))]
    interface DDLogger
    {
        [Static]
        [Export("create")]
        DDLogger Create();

        [Static]
        [Export("createWithConfiguration:")]
        DDLogger Create(DDLoggerConfiguration configuration);

        [Export("debug:attributes:")]
        void Debug(string message, [NullAllowed] NSDictionary attributes);

        [Export("info:attributes:")]
        void Info(string message, [NullAllowed] NSDictionary attributes);

        [Export("warn:attributes:")]
        void Warn(string message, [NullAllowed] NSDictionary attributes);

        [Export("error:attributes:")]
        void Error(string message, [NullAllowed] NSDictionary attributes);

        [Export("critical:attributes:")]
        void Critical(string message, [NullAllowed] NSDictionary attributes);
    }

    // ... Similar minimal bindings for Trace, SessionReplay, etc.
}
```

### Pros

✅ **Fast to implement** - Only ~100-150 APIs to write
✅ **User-focused** - Only expose what users actually need
✅ **Clean documentation** - Easy to document core scenarios
✅ **No fixing errors** - Hand-written, no Sharpie cleanup
✅ **Better API design** - Can improve names, add helpers

### Cons

❌ **Incomplete coverage** - Missing advanced/internal APIs
❌ **Manual work** - Have to write bindings by hand
❌ **Might miss APIs** - Users might need something we didn't include
❌ **Future additions** - Need to add bindings as users request them

### Estimated Effort

- **Writing core bindings**: 4-6 hours
- **Testing**: 1-2 hours
- **Documentation**: 2-3 hours (only core scenarios!)
- **Total**: ~7-11 hours

---

## Comparison Matrix

| Aspect | Option A (Fix Generated) | Option B (Manual Core) |
|--------|--------------------------|------------------------|
| **API Count** | 684 types | ~20 classes, ~100-150 methods |
| **Coverage** | 100% of native SDK | ~80% of common use cases |
| **Build Errors** | 42 to fix | 0 (clean from start) |
| **Documentation Effort** | Very high (684 types) | Low (20 classes) |
| **User Experience** | Overwhelming choice | Focused, clear path |
| **Maintenance** | High (more code) | Low (less code) |
| **Extensibility** | Already done | Add later as needed |
| **Time to Complete** | 8-12 hours | 7-11 hours |
| **Time to Document** | 6-8 hours | 2-3 hours |
| **Risk** | Low (auto-generated) | Medium (might miss APIs) |

---

## Recommendation Based on Your Goal

Since you said:
> "for both android and ios, we need to document each binding methods that are available and how to use them for users who are using these packages"

### I recommend **Option B (Minimal Manual Bindings)** because:

1. **Easier to document** - 20 classes vs 684 types
2. **User-focused** - Document only what users need to know
3. **Matches Android pattern** - Android bindings also expose focused API
4. **Faster to market** - Can publish sooner with core functionality
5. **Better UX** - Users aren't overwhelmed by internal APIs

### Documentation Structure for Option B

```markdown
# Datadog iOS SDK API Reference

## Core SDK

### DDDatadog
Primary SDK initialization class.

**Initialize SDK**
```csharp
DDDatadog.Initialize(configuration, trackingConsent);
```

### DDConfiguration
SDK configuration builder.

**Create Configuration**
```csharp
var config = new DDConfiguration(clientToken, environment);
config.Service = "my-mobile-app";
config.Site = DDSite.US1;
```

## RUM (Real User Monitoring)

### DDRUM
Enable and configure RUM tracking.

### DDRUMMonitor
Track user interactions, views, and errors.

**Track a View**
```csharp
DDRUMMonitor.Shared.StartView("HomeViewController", "Home");
```

**Track an Action**
```csharp
DDRUMMonitor.Shared.AddAction("ButtonTap");
```

... etc.
```

### Hybrid Approach (Best of Both)

We could also do:
1. **Start with Option B** - Get core APIs working and documented
2. **Keep Option A bindings** - Store them in `Generated/` folder
3. **Add on demand** - When users need advanced APIs, copy from generated bindings

This way you get:
- ✅ Fast time to market
- ✅ Focused documentation
- ✅ Complete bindings available for reference
- ✅ Easy to extend later

---

## What Determines "User-Facing" vs "Internal"?

Looking at the Android sample, user-facing APIs are:

### Configuration & Initialization
- Creating configuration objects
- Setting basic properties (service, environment, site)
- Initialize SDK with configuration

### RUM APIs Users Call
- `Enable(config)` - Turn on feature
- `StartView()`, `StopView()` - Manual view tracking
- `AddAction()` - Track user actions
- `AddError()` - Report errors
- `StartResource()`, `StopResource()` - Track network calls

### Logging APIs Users Call
- `Logger.Create()` - Create logger instance
- `Logger.Debug/Info/Warn/Error()` - Log messages

### Internal/Advanced APIs to Skip
- Thread pool management
- Internal storage APIs
- Network retry logic
- Data encryption internals
- Performance instrumentation internals

The rule of thumb: **If it's in a "Getting Started" guide or common tutorial, it's user-facing**.
