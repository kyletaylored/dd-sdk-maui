---
layout: default
title: Identifying User-Facing APIs
nav_order: 32
---

# Methodology: Identifying User-Facing vs Internal APIs

This document explains **how to determine which binding methods should be exposed and documented for users** vs which are internal implementation details.

## Overview

When Objective Sharpie generates 684 API types from the iOS SDK, most of those are internal structures that users never interact with. We need clear criteria to identify the ~100-150 APIs that users actually call.

---

## Method 1: Official API Surface Document (PRIMARY)

**The most reliable method**: Datadog maintains an official `api-surface-swift` file that documents all public APIs.

**Location**: https://github.com/DataDog/dd-sdk-ios/blob/develop/api-surface-swift

### What This Tells Us

This file explicitly lists every public API organized by module. If it's in this file, **it's user-facing**. If it's not in this file, **it's internal**.

### Example: DatadogRUM Module

**User-Facing APIs (in api-surface-swift)**:
```swift
// Monitor access
RUM.shared(in:) -> RUMMonitorProtocol

// View tracking
monitor.startView(viewController:, name:, attributes:)
monitor.stopView(viewController:, attributes:)

// Resource tracking
monitor.startResource(resourceKey:, request:, attributes:)
monitor.stopResource(resourceKey:, response:, size:, attributes:)

// Error tracking
monitor.addError(message:, type:, stack:, source:, attributes:, file:, line:)

// Action tracking
monitor.addAction(type:, name:, attributes:)
```

**Internal APIs (NOT in api-surface-swift)**:
- Event structures: `DDRUMActionEvent`, `DDRUMViewEvent`, `DDRUMResourceEvent`
- Event properties: `DDRUMActionEventAction`, `DDRUMActionEventApplication`
- Nested event structures: `DDRUMActionEventActionCrash`, `DDRUMActionEventActionError`
- Internal predicates and filters
- Internal storage and threading utilities

**Key Insight**: Users call methods like `addAction()` which **internally create** `DDRUMActionEvent` objects. Users never construct these event objects directly, so we don't need to bind them.

### Complete Public API Count by Module

Based on the official api-surface-swift:

| Module | User-Facing APIs | Notes |
|--------|-----------------|-------|
| **DatadogCore** | ~15-20 methods | Initialization, user info, consent, SDK management |
| **DatadogLogs** | ~10 methods | Logger creation + 5 log levels |
| **DatadogRUM** | ~20 methods | View, resource, action, error tracking |
| **DatadogTrace** | ~10 methods | Tracer access, span operations |
| **DatadogSessionReplay** | ~5 methods | Enable, start/stop recording |
| **DatadogCrashReporting** | ~2 methods | Enable with optional plugin |
| **DatadogWebViewTracking** | ~2 methods | Enable/disable web view tracking |
| **DatadogFlags** | ~8 methods | Feature flags client (optional) |
| **Total** | **~70-80 methods** | vs 684 generated types! |

---

## Method 2: Cross-Reference with Android Implementation

**Principle**: iOS and Android SDKs have feature parity. If users call an API on Android, they need the equivalent on iOS.

### Example: Android Sample App Analysis

From `samples/DatadogMauiSample/Platforms/Android/MainApplication.cs`:

```csharp
// Core SDK initialization
var config = new Configuration.Builder(clientToken, env, variant, service)
    .SetFirstPartyHosts(hosts)
    .SetBatchSize(BatchSize.Small)
    .SetUploadFrequency(UploadFrequency.Frequent)
    .Build();
Datadog.Initialize(this, config, TrackingConsent.Granted);
Datadog.Verbosity = (int)LogPriority.Verbose;

// RUM
var rumConfig = new RumConfiguration.Builder(applicationId)
    .TrackUserInteractions()
    .TrackLongTasks()
    .TrackFrustrations(true)
    .TrackBackgroundEvents(true)
    .TrackNonFatalAnrs(true)
    .SetTelemetrySampleRate(100f)
    .Build();
Rum.Enable(rumConfig);

// Logs
var logsConfig = new LogsConfiguration.Builder().Build();
Logs.Enable(logsConfig);

// Trace
var traceConfig = new TraceConfiguration.Builder().Build();
Trace.Enable(traceConfig, Datadog.Instance);

// Session Replay
var replayConfig = new SessionReplayConfiguration.Builder(sampleRate)
    .SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)
    .SetImagePrivacy(ImagePrivacy.MaskNone)
    .SetTouchPrivacy(TouchPrivacy.Show)
    .Build();
SessionReplay.Enable(replayConfig, Datadog.Instance);

// WebView Tracking
WebViewTracking.Enable();
```

### iOS Equivalents Needed

For each Android API above, we need the iOS equivalent:

| Android API | iOS Equivalent | Module |
|------------|----------------|---------|
| `Datadog.Initialize(config, consent)` | `Datadog.initialize(with:trackingConsent:)` | DatadogCore |
| `Datadog.Verbosity = level` | `Datadog.verbosityLevel = level` | DatadogCore |
| `Datadog.SetUserInfo()` | `Datadog.setUserInfo(id:name:email:)` | DatadogCore |
| `Rum.Enable(config)` | `RUM.enable(with:in:)` | DatadogRUM |
| `GlobalRumMonitor.Instance` | `RUMMonitor.shared(in:)` | DatadogRUM |
| `monitor.StartView()` | `monitor.startView(viewController:name:)` | DatadogRUM |
| `monitor.AddAction()` | `monitor.addAction(type:name:)` | DatadogRUM |
| `monitor.AddError()` | `monitor.addError(message:type:)` | DatadogRUM |
| `Logs.Enable(config)` | `Logs.enable(with:in:)` | DatadogLogs |
| `Logger.Create()` | `Logger.create(with:in:)` | DatadogLogs |
| `logger.Info()` | `logger.info(_:error:attributes:)` | DatadogLogs |
| `Trace.Enable(config)` | `Trace.enable(with:in:)` | DatadogTrace |
| `Tracer.Shared` | `Tracer.shared(in:)` | DatadogTrace |
| `SessionReplay.Enable(config)` | `SessionReplay.enable(with:in:)` | DatadogSessionReplay |
| `WebViewTracking.Enable()` | `WebViewTracking.enable(webView:)` | DatadogWebViewTracking |

**Rule**: If Android users call it, iOS users need it.

---

## Method 3: Official Documentation Analysis

**Principle**: APIs that appear in "Getting Started" guides and tutorials are user-facing.

### Official Datadog Documentation

**iOS Setup Guide**: https://docs.datadoghq.com/real_user_monitoring/application_monitoring/ios/setup/

Key APIs mentioned:
1. `Datadog.initialize(with: configuration, trackingConsent: consent)`
2. `RUM.enable(with: rumConfiguration)`
3. `Logs.enable(with: logsConfiguration)`
4. `Trace.enable(with: traceConfiguration)`
5. `SessionReplay.enable(with: replayConfiguration)`

**Tracing Documentation**: https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/ios/

APIs mentioned:
1. `Tracer.shared(in: core)`
2. `tracer.startSpan(operationName: "operation")`
3. `span.finish()`

**Pattern**: Documentation shows **configuration builders** and **static enable methods**. It does NOT show internal event structures or implementation details.

---

## Method 4: API Naming Patterns

**Principle**: Certain naming patterns indicate user-facing vs internal APIs.

### User-Facing Patterns

✅ **Static factory methods**
- `Datadog.initialize()` - SDK entry point
- `Logger.create()` - Create instances
- `RUMMonitor.shared()` - Singleton access

✅ **Enable/disable methods**
- `RUM.enable(with: config)` - Turn on features
- `Logs.enable(with: config)`
- `SessionReplay.enable(with: config)`

✅ **Action verbs for tracking**
- `startView()`, `stopView()` - View lifecycle
- `addAction()` - Log user actions
- `addError()` - Report errors
- `startResource()`, `stopResource()` - Network tracking

✅ **Configuration builders**
- `Configuration` with properties: `clientToken`, `env`, `site`, `service`
- `RUM.Configuration`, `Logs.Configuration`, etc.

✅ **Simple parameter types**
- Strings: `viewName`, `actionName`, `errorMessage`
- Booleans: `trackFrustrations`, `enabled`
- Numbers: `sampleRate`, `threshold`
- Enums: `TrackingConsent`, `DatadogSite`, `RUMActionType`

### Internal Patterns

❌ **Event data structures**
- `DDRUMActionEvent` - Created internally, never by users
- `DDRUMViewEvent`, `DDRUMResourceEvent` - Same pattern
- Pattern: `DD*Event` with 50+ nested properties

❌ **Nested event properties**
- `DDRUMActionEventAction` - Sub-structure of event
- `DDRUMActionEventApplication` - More sub-structure
- Pattern: `DD*Event*` (double nesting indicates internal)

❌ **Protocol implementations**
- `URLSessionDataDelegate` implementations
- Custom predicates and filters (unless for configuration)
- Storage and threading utilities

❌ **Internal utilities**
- `DataUploadScheduler`
- `NetworkRetryer`
- `StorageEngine`
- Pattern: Implementation detail classes

❌ **Complex type parameters**
- Methods accepting internal event structures
- Methods returning internal storage references
- Pattern: If the parameter type is also internal, method is internal

---

## Method 5: Comparing Generated Output

**Principle**: Look at what Objective Sharpie generates for each framework and identify patterns.

### Example: DatadogRUM Generated Output

**Generated: 294 interfaces, 7,199 lines**

#### User-Facing (3 classes, ~20 methods)

```csharp
// Configuration class - users create this
interface DDRUMConfiguration
{
    [Export("initWithApplicationID:")]
    NativeHandle Constructor(string applicationId);

    [Export("trackUIKitViews")]
    DDRUMConfiguration TrackUIKitViews();

    [Export("sessionSampleRate")]
    float SessionSampleRate { get; set; }
}

// Static enable class - users call this once
interface DDRUM
{
    [Static]
    [Export("enable:")]
    void Enable(DDRUMConfiguration configuration);
}

// Monitor class - users call tracking methods
interface DDRUMMonitor
{
    [Static]
    [Export("shared")]
    DDRUMMonitor Shared { get; }

    [Export("startView:name:attributes:")]
    void StartView(string key, string name, NSDictionary attributes);

    [Export("addAction:name:attributes:")]
    void AddAction(DDRUMActionType type, string name, NSDictionary attributes);

    [Export("addError:message:source:attributes:")]
    void AddError(NSError error, string message, DDRUMErrorSource source, NSDictionary attributes);
}
```

#### Internal (291 interfaces, 7,100+ lines)

```csharp
// Event structure - SDK creates internally, users never touch
interface DDRUMActionEvent
{
    [Export("action", ArgumentSemantic.Strong)]
    DDRUMActionEventAction Action { get; set; }

    [Export("application", ArgumentSemantic.Strong)]
    DDRUMActionEventApplication Application { get; set; }

    [Export("ciTest", ArgumentSemantic.Strong)]
    DDRUMActionEventCITest CiTest { get; set; }

    // ... 50+ more properties
}

// Nested event structure - internal implementation
interface DDRUMActionEventAction
{
    [Export("crash", ArgumentSemantic.Strong)]
    DDRUMActionEventActionCrash Crash { get; set; }

    [Export("error", ArgumentSemantic.Strong)]
    DDRUMActionEventActionError Error { get; set; }

    // ... 20+ more properties
}

// Deep nested structure - internal implementation
interface DDRUMActionEventActionCrash
{
    [Export("count")]
    long Count { get; set; }
}

// This pattern repeats for:
// - DDRUMViewEvent (50+ properties)
// - DDRUMResourceEvent (50+ properties)
// - DDRUMErrorEvent (50+ properties)
// - DDRUMLongTaskEvent (30+ properties)
// And all their nested structures...
```

### Size Comparison

| Category | Interfaces | Lines | User Impact |
|----------|-----------|-------|-------------|
| **User-Facing** | 3 | ~200 | Users call these directly |
| **Internal** | 291 | 7,000+ | SDK uses internally, users never see |
| **Reduction** | **99%** | **97%** | Massive documentation savings |

---

## Decision Matrix

Use this table to determine if an API should be bound and documented:

| Question | User-Facing | Internal |
|----------|------------|----------|
| In official `api-surface-swift` file? | ✅ Yes | ❌ No |
| Called in Android sample app? | ✅ Yes | ❌ No |
| Appears in "Getting Started" docs? | ✅ Yes | ❌ No |
| Is it a static factory/enable method? | ✅ Yes | ❌ No |
| Has simple parameters (string, bool, enum)? | ✅ Likely | ❌ Unlikely |
| Is it an event data structure? | ❌ No | ✅ Yes |
| Name pattern `DD*Event*`? | ❌ No | ✅ Yes |
| Does it configure or track behavior? | ✅ Yes | ❌ No |
| Is it a utility/storage/threading class? | ❌ No | ✅ Yes |

**Rule of thumb**: If 3+ questions in the "User-Facing" column are Yes, **bind and document it**. If 3+ in "Internal" are Yes, **skip it**.

---

## Practical Application

### Step 1: Extract from api-surface-swift

Start with the official list. These are **guaranteed user-facing**:

```
DatadogCore:
- Datadog.initialize(with:trackingConsent:instanceName:)
- Datadog.setUserInfo(id:name:email:extraInfo:in:)
- Datadog.set(trackingConsent:in:)

DatadogRUM:
- RUM.enable(with:in:)
- RUMMonitor.shared(in:)
- monitor.startView(viewController:name:attributes:)
- monitor.addAction(type:name:attributes:)
- monitor.addError(message:type:stack:source:attributes:)

DatadogLogs:
- Logs.enable(with:in:)
- Logger.create(with:in:)
- logger.debug/info/warn/error/critical(_:error:attributes:)

... etc for all modules
```

### Step 2: Cross-check with Android

For each Android API in your sample, ensure iOS equivalent exists:

```
Android: Rum.Enable(config)
iOS: ✅ RUM.enable(with:in:) - already in list

Android: GlobalRumMonitor.Instance
iOS: ✅ RUMMonitor.shared(in:) - already in list

Android: Datadog.Initialize(config, consent)
iOS: ✅ Datadog.initialize(with:trackingConsent:) - already in list
```

### Step 3: Verify Against Documentation

Check that initialization examples in docs match your list:

```
Docs show: Datadog.initialize(with: config, trackingConsent: .granted)
Your list: ✅ Has Datadog.initialize()

Docs show: RUM.enable(with: rumConfig)
Your list: ✅ Has RUM.enable()

Docs show: monitor.startView()
Your list: ✅ Has RUMMonitor.startView()
```

### Step 4: Filter Generated Bindings

From Objective Sharpie's 684 types:
- Keep: Types in api-surface-swift
- Keep: Configuration classes
- Keep: Static factory classes
- Keep: Enums used as parameters
- **Skip**: `DD*Event*` structures
- **Skip**: Internal predicates not in api-surface
- **Skip**: Storage/threading utilities

**Result**: ~70-80 methods across 15-20 classes

---

## Recommended Approach

Based on this methodology:

### ✅ **Option B: Manual Bindings** (Recommended)

**Why**:
1. **Authoritative source exists**: api-surface-swift tells us exactly what to bind
2. **99% reduction**: From 684 types to ~70-80 methods
3. **Better UX**: Users aren't overwhelmed by internal structures
4. **Easier documentation**: Document 70 methods vs 684 types
5. **Matches Android**: Can mirror the Android sample patterns

**Process**:
1. Copy method signatures from api-surface-swift
2. Write C# bindings for each (15-20 classes total)
3. Cross-check with Android sample for completeness
4. Test with iOS sample app matching Android functionality
5. Document with clear examples

**Time**: ~6-8 hours for binding + 2-3 hours for documentation = **8-11 hours total**

### ❌ Option A: Fix All Generated (Not Recommended)

**Why Not**:
- 97% of generated code is internal structures users never call
- Would require documenting 684 types (most irrelevant)
- Users would be confused by internal event structures
- Maintenance burden for code that's never used

---

## Summary

**How we know which methods are user-facing**:

1. ✅ **PRIMARY**: Check official `api-surface-swift` file - if it's there, it's user-facing
2. ✅ **SECONDARY**: Cross-reference with Android implementation - users need equivalent APIs
3. ✅ **VALIDATION**: Verify against official documentation - user-facing APIs appear in tutorials
4. ✅ **PATTERN MATCHING**: Static factories, enable methods, simple parameters = user-facing
5. ✅ **EXCLUSION**: Event structures (`DD*Event*`) and utilities = internal

**Result**: Bind ~70-80 user-facing methods across 15-20 classes, skip the 600+ internal structures.

---

## Sources

- [Datadog iOS SDK GitHub Repository](https://github.com/DataDog/dd-sdk-ios)
- [Official API Surface Documentation](https://github.com/DataDog/dd-sdk-ios/blob/develop/api-surface-swift)
- [iOS and tvOS Monitoring Setup](https://docs.datadoghq.com/real_user_monitoring/application_monitoring/ios/setup/)
- [Tracing iOS Applications](https://docs.datadoghq.com/tracing/trace_collection/automatic_instrumentation/dd_libraries/ios/)
