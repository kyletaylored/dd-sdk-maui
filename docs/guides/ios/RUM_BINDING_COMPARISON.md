---
layout: default
title: RUM Example
parent: iOS
nav_order: 3
---

# RUM Binding Comparison: Generated vs Manual

This document shows the difference between what Objective Sharpie generated for RUM vs what users actually need.

## What Objective Sharpie Generated

**File**: `Generated/DatadogRUM/ApiDefinitions.cs`
- **Lines of code**: 7,199
- **Number of interfaces**: 294
- **Includes**:
  - All event data structures (DDRUMActionEvent, DDRUMViewEvent, DDRUMResourceEvent, etc.)
  - All internal event properties (timestamps, IDs, nested structures)
  - All predicates and filters
  - SwiftUI-specific bindings
  - UIKit-specific bindings
  - Internal instrumentation classes

### Example of Generated Event Structure

```csharp
// THIS IS AUTO-GENERATED - 7,199 LINES!

// Just ONE of many event types (there are ~50 similar interfaces for events)
interface DDRUMActionEvent
{
    [Export ("action", ArgumentSemantic.Strong)]
    DDRUMActionEventAction Action { get; set; }

    [Export ("application", ArgumentSemantic.Strong)]
    DDRUMActionEventApplication Application { get; set; }

    [Export ("ciTest", ArgumentSemantic.Strong)]
    DDRUMActionEventCITest CiTest { get; set; }

    [Export ("connectivity", ArgumentSemantic.Strong)]
    DDRUMActionEventConnectivity Connectivity { get; set; }

    [Export ("container", ArgumentSemantic.Strong)]
    DDRUMActionEventContainer Container { get; set; }

    [Export ("context", ArgumentSemantic.Strong)]
    DDRUMActionEventContext Context { get; set; }

    [Export ("date")]
    long Date { get; set; }

    [Export ("dd", ArgumentSemantic.Strong)]
    DDRUMDD DD { get; set; }

    [Export ("device", ArgumentSemantic.Strong)]
    DDRUMActionEventDevice Device { get; set; }

    [Export ("display", ArgumentSemantic.Strong)]
    DDRUMActionEventDisplay Display { get; set; }

    // ... ~30 more properties ...
}

// And then ALL the nested structures...
interface DDRUMActionEventAction
{
    [Export ("crash", ArgumentSemantic.Strong)]
    DDRUMActionEventActionCrash Crash { get; set; }

    [Export ("error", ArgumentSemantic.Strong)]
    DDRUMActionEventActionError Error { get; set; }

    [Export ("frustration", ArgumentSemantic.Strong)]
    DDRUMActionEventActionFrustration Frustration { get; set; }

    [Export ("id")]
    string Id { get; set; }

    [Export ("loadingTime")]
    long LoadingTime { get; set; }

    [Export ("longTask", ArgumentSemantic.Strong)]
    DDRUMActionEventActionLongTask LongTask { get; set; }

    [Export ("resource", ArgumentSemantic.Strong)]
    DDRUMActionEventActionResource Resource { get; set; }

    [Export ("target", ArgumentSemantic.Strong)]
    DDRUMActionEventActionTarget Target { get; set; }

    [Export ("type")]
    DDRUMActionEventActionType Type { get; set; }

    // ... continues for pages ...
}

// And it keeps nesting deeper...
interface DDRUMActionEventActionCrash
{
    [Export ("count")]
    long Count { get; set; }
}

interface DDRUMActionEventActionError
{
    [Export ("count")]
    long Count { get; set; }
}

interface DDRUMActionEventActionFrustration
{
    [Export ("type")]
    DDRUMActionEventActionFrustrationType[] Type { get; set; }
}

// ... this pattern repeats for EVERY event type (Action, View, Resource, Error, LongTask, etc.)
// ... and EVERY nested property in each event
// ... resulting in 294 interfaces total!
```

## What Users Actually Call

Looking at your Android sample, users call **only 5-10 RUM methods**:

```csharp
// From samples/DatadogMauiSample/Platforms/Android/MainApplication.cs

// 1. ENABLE RUM (once at startup)
var rumConfiguration = new RumConfiguration.Builder(applicationId)
    .TrackUserInteractions()        // Enable automatic tracking
    .TrackLongTasks()               // Enable performance monitoring
    .TrackFrustrations(true)        // Enable UX monitoring
    .TrackBackgroundEvents(true)    // Track background activity
    .TrackNonFatalAnrs(true)        // Track app not responding
    .SetTelemetrySampleRate(100f)   // Set sample rate
    .Build();

Rum.Enable(rumConfiguration);  // <-- ONE METHOD

// 2. GET MONITOR INSTANCE
var monitor = GlobalRumMonitor.Instance;  // <-- ONE PROPERTY

// That's it for automatic RUM! The SDK automatically tracks:
// - View changes
// - User taps
// - Network requests
// - Crashes
// - Performance metrics

// 3. MANUAL TRACKING (optional, for custom events)
// Track custom view
monitor.StartView("CustomViewController", "Custom View Name");
monitor.StopView("CustomViewController");

// Track custom action
monitor.AddAction("CustomButtonClick", new Dictionary<string, object>
{
    { "button_id", "submit" },
    { "screen", "checkout" }
});

// Track custom error
monitor.AddError("Custom error message", DDRUMErrorSource.Source, exception);

// Track custom resource (usually automatic)
monitor.StartResource("api-call", "GET", url);
monitor.StopResource("api-call", 200, responseSize);
```

### iOS Equivalent (What We Need to Bind)

```csharp
// OPTION B: MINIMAL MANUAL BINDINGS
// Only what users actually call

namespace Datadog.iOS
{
    // ================================================
    // RUM Configuration - ONE class users create
    // ================================================
    [BaseType(typeof(NSObject))]
    interface DDRUMConfiguration
    {
        // Constructor - users call this once
        [Export("initWithApplicationID:")]
        NativeHandle Constructor(string applicationId);

        // Builder-style methods users call for configuration
        [Export("trackUIKitViews")]
        DDRUMConfiguration TrackUIKitViews();

        [Export("trackUIKitActions")]
        DDRUMConfiguration TrackUIKitActions();

        [Export("trackLongTasks:")]
        DDRUMConfiguration TrackLongTasks(double threshold);

        [Export("trackBackgroundEvents:")]
        DDRUMConfiguration TrackBackgroundEvents(bool enabled);

        [Export("setSessionSampleRate:")]
        DDRUMConfiguration SetSessionSampleRate(float rate);

        [Export("setTelemetrySampleRate:")]
        DDRUMConfiguration SetTelemetrySampleRate(float rate);

        // ~10-15 configuration methods total
    }

    // ================================================
    // RUM Enable - ONE static method users call
    // ================================================
    [BaseType(typeof(NSObject))]
    interface DDRUM
    {
        // Enable RUM - users call this once at startup
        [Static]
        [Export("enable:")]
        void Enable(DDRUMConfiguration configuration);
    }

    // ================================================
    // RUM Monitor - ~10 methods for manual tracking
    // ================================================
    [BaseType(typeof(NSObject))]
    interface DDRUMMonitor
    {
        // Get shared instance - users call this to get monitor
        [Static]
        [Export("shared")]
        DDRUMMonitor Shared { get; }

        // Manual view tracking (~2 methods)
        [Export("startView:name:attributes:")]
        void StartView(string key, string name, [NullAllowed] NSDictionary attributes);

        [Export("stopView:attributes:")]
        void StopView(string key, [NullAllowed] NSDictionary attributes);

        // Manual action tracking (~1 method)
        [Export("addAction:name:attributes:")]
        void AddAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

        // Error tracking (~1-2 methods)
        [Export("addError:message:source:attributes:")]
        void AddError(NSError error, string message, DDRUMErrorSource source, [NullAllowed] NSDictionary attributes);

        // Resource tracking (~3 methods for manual network tracking)
        [Export("startResource:httpMethod:urlString:attributes:")]
        void StartResource(string key, string httpMethod, string url, [NullAllowed] NSDictionary attributes);

        [Export("stopResource:statusCode:size:attributes:")]
        void StopResource(string key, nint statusCode, long size, [NullAllowed] NSDictionary attributes);

        [Export("stopResourceWithError:error:attributes:")]
        void StopResourceWithError(string key, NSError error, [NullAllowed] NSDictionary attributes);

        // Attribute management (~2 methods)
        [Export("addAttribute:forKey:")]
        void AddAttribute(NSObject value, string key);

        [Export("removeAttribute:")]
        void RemoveAttribute(string key);

        // ~10-12 methods total
    }

    // ================================================
    // Enums - ~3-4 simple enums
    // ================================================
    [Native]
    public enum DDRUMActionType : long
    {
        Tap,
        Scroll,
        Swipe,
        Click,
        Custom
    }

    [Native]
    public enum DDRUMErrorSource : long
    {
        Source,
        Network,
        WebView,
        Console,
        Custom
    }

    [Native]
    public enum DDRUMResourceType : long
    {
        Image,
        Xhr,
        Beacon,
        Css,
        Document,
        Fetch,
        Font,
        Js,
        Media,
        Other,
        Native
    }
}

// TOTAL: ~3 classes, ~30 methods, ~3 enums
// LINES OF CODE: ~200-300 lines
// vs GENERATED: 294 interfaces, 7,199 lines!
```

## Direct Comparison

| Aspect | Generated (Option A) | Manual (Option B) |
|--------|---------------------|-------------------|
| **RUM File Size** | 7,199 lines | ~200-300 lines |
| **Interfaces/Classes** | 294 interfaces | 3 classes |
| **What's Included** | Every event structure, every property, every internal class | Only APIs users call |
| **Event Structures** | Full `DDRUMActionEvent` with 50+ properties | Not needed - SDK creates internally |
| **Predicates** | 10+ predicate interfaces | Use defaults, expose if needed |
| **Configuration** | Every possible option | Common options (~10 methods) |
| **Monitor Methods** | Everything | Core tracking methods (~10) |

## The Key Insight

**Users DON'T create event objects!**

All those 294 interfaces for event structures (`DDRUMActionEvent`, `DDRUMViewEvent`, etc.) are **created internally by the SDK**. Users never instantiate them.

### What Users Do:

```csharp
// Users call this:
monitor.AddAction("Button Click");

// SDK internally creates this (users never see it):
// var event = new DDRUMActionEvent {
//     Action = new DDRUMActionEventAction {
//         Type = DDRUMActionEventActionType.Tap,
//         Target = new DDRUMActionEventActionTarget { Name = "Button Click" },
//         ...
//     },
//     Date = timestamp,
//     Application = new DDRUMActionEventApplication { Id = appId },
//     ... 50 more properties ...
// };
```

Users only need the **input APIs** (methods they call), not the **output structures** (internal event objects).

## Recommendation

For RUM specifically, **Option B (Manual)** makes even more sense:

1. **99% reduction in code** - From 7,199 lines to ~250 lines
2. **99% reduction in documentation** - From 294 types to 3 classes
3. **Same functionality** - Users can do everything they need
4. **Better maintainability** - Only maintain what's actually used
5. **Clearer purpose** - Obvious which APIs are for users

### Documentation Benefit

**Option A would require documenting**:
- 294 interfaces
- ~1,000+ properties
- Internal event structures users never create
- Result: Overwhelming, confusing documentation

**Option B would document**:
- 3 classes
- ~30 methods
- Only APIs users call
- Result: Clear, focused "Getting Started" guide

---

## What About Advanced Users?

If someone needs an advanced API we didn't bind:

1. **Check if it's really needed** - Most "advanced" APIs are internal
2. **Add it to manual bindings** - Takes 5-10 minutes per API
3. **Reference generated bindings** - We keep them for reference
4. **Future binding** - Add in next version if commonly requested

In practice, 95% of users only need the core APIs. The remaining 5% who need something custom can request it, and we add it in the next release.

---

## Conclusion

For RUM (and similar for Logs, Trace, etc.), **Manual bindings are clearly better**:

- ✅ **200 lines vs 7,199 lines**
- ✅ **3 classes to document vs 294 interfaces**
- ✅ **Clear user-facing API vs internal structures**
- ✅ **Faster to write, test, and maintain**
- ✅ **Better documentation experience**

The generated bindings are great as a **reference** and for **understanding the native SDK**, but not ideal for what users actually need to call.
