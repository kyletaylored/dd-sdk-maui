---
layout: default
title: iOS Binding Strategy
nav_order: 31
---

# iOS Binding Strategy

Complete guide to iOS bindings for the Datadog MAUI SDK, including approach comparison and implementation methodology.

---

## Table of Contents

- [Current Status](#current-status)
- [Approach Comparison](#approach-comparison)
- [Recommended Approach](#recommended-approach)
- [Implementation Methodology](#implementation-methodology)
- [Concrete Example: RUM Bindings](#concrete-example-rum-bindings)
- [Next Steps](#next-steps)

---

## Current Status

**Objective Sharpie has generated bindings for all Datadog iOS frameworks**, but they need work before being production-ready.

### What We Have

Generated bindings for 8 iOS frameworks:
- DatadogCore (22 interfaces) - Configuration, SDK initialization
- DatadogInternal - Internal utilities (loggers, storage, threading)
- DatadogRUM (294 interfaces) - RUM monitoring APIs
- DatadogLogs - Logging APIs
- DatadogTrace - APM tracing APIs
- DatadogSessionReplay - Session replay configuration
- DatadogCrashReporting - Crash reporting
- DatadogWebViewTracking - WebView instrumentation

**Total**: 684 API types generated
**Status**: 42 compilation errors need fixing

### Issues to Resolve

1. **[Verify] attributes** (~12 instances) - Manual review needed
2. **Missing using directives** (~6 errors) - Import statements
3. **Duplicate constructors** (~2 errors) - Selector conflicts
4. **Internal dependencies** (~10 errors) - Cross-module references
5. **Namespace consolidation** - Generated as separate namespaces
6. **Excessive API surface** - 7,199 lines for RUM alone

---

## Approach Comparison

### Option A: Fix All Generated Bindings

**What it means**: Fix all 42 compilation errors in Objective Sharpie generated code, expose all 684 API types.

**Pros**:
- ✅ Complete API coverage - all iOS APIs available
- ✅ Future-proof - won't need to add bindings later
- ✅ Auto-generated - most work done by Sharpie
- ✅ 1:1 mapping with native iOS SDKs

**Cons**:
- ❌ Time-consuming - 42 errors to fix manually
- ❌ Large surface area - overwhelming for users
- ❌ Internal APIs exposed - not meant for public use
- ❌ High documentation burden - 684 types to document
- ❌ Maintenance complexity - more code to maintain

**Effort Estimate**: 2-3 days to fix all errors + ongoing documentation

---

### Option B: Minimal Manual Bindings (Recommended)

**What it means**: Create clean, minimal bindings for only the user-facing APIs that developers actually need.

**Pros**:
- ✅ Clean API - Only expose what users need
- ✅ Less documentation - Fewer types to explain
- ✅ Faster to implement - Focus on essentials
- ✅ Easier to maintain - Smaller codebase
- ✅ Better UX - Users aren't overwhelmed
- ✅ Follows opt-in philosophy - Expose only necessary APIs

**Cons**:
- ❌ Might miss edge cases - Some users may need obscure APIs
- ❌ Requires analysis - Need to identify user-facing APIs
- ❌ Less complete - Not 1:1 with native SDKs

**Effort Estimate**: 1-2 days for essential bindings

---

### Hybrid Approach (Alternative)

**What it means**: Start with minimal manual bindings (Option B), keep generated bindings in `_reference/` folder for future expansion.

**Process**:
1. Create minimal bindings for 80% of use cases
2. Archive generated bindings to `_reference/Generated/`
3. When users request specific APIs, add them incrementally
4. Reference generated code as template

**Pros**:
- ✅ Best of both worlds
- ✅ Iterative improvement based on real needs
- ✅ Generated code not wasted - kept as reference

**Cons**:
- ❌ Reactive - Must wait for user requests
- ❌ Potential fragmentation - Mix of manual and generated code

---

## Recommended Approach

**We recommend Option B: Minimal Manual Bindings**

### Rationale

1. **User-focused**: Expose only what developers actually use
2. **Maintainable**: Smaller API surface is easier to document and maintain
3. **Proven pattern**: Android bindings use opt-in approach successfully
4. **Consistent philosophy**: Matches project's opt-in vs opt-out approach
5. **Practical**: Most users need ~5% of the APIs

### What to Expose

**Core APIs (Essential)**:
- SDK initialization and configuration
- Basic RUM monitoring (views, actions, resources, errors)
- Logging (info, warn, error, debug)
- Tracing (span creation, tags, baggage)
- Global context and user info

**Advanced APIs (Add Later)**:
- Custom event attributes
- Advanced RUM configuration
- Performance monitoring
- Session replay configuration
- WebView tracking
- Crash reporting customization

---

## Implementation Methodology

Use this systematic approach to identify which iOS APIs to bind.

### Method 1: Official API Surface (Recommended)

**What**: Use Datadog's official api-surface-swift files from dd-sdk-ios repository

**Process**:
```bash
# Clone dd-sdk-ios repo
git clone https://github.com/DataDog/dd-sdk-ios.git

# Check api-surface-swift files
find dd-sdk-ios -name "*.api-surface-swift"

# Example: DatadogCore/api-surface-swift
cat dd-sdk-ios/DatadogCore/api-surface-swift
```

**Output**:
```swift
// Official public API surface
public class Datadog {
    public static func initialize(configuration: Configuration, trackingConsent: TrackingConsent)
    public static var verbosityLevel: CoreLoggerLevel { get set }
    // ... only user-facing methods
}
```

**Why this works**: Datadog officially maintains these files showing what's public vs internal.

---

### Method 2: Android Cross-Reference

**What**: Compare with Android API to find equivalent iOS APIs

**Process**:
1. Identify Android public APIs (already exposed in Android bindings)
2. Find equivalent iOS APIs in Objective Sharpie output
3. Bind iOS equivalents

**Example**:
```
Android: com.datadog.android.rum.GlobalRum.get().addAttribute()
iOS: DDGlobalRUM.addAttribute(_:forKey:)
```

**Why this works**: Both SDKs have similar feature parity, public APIs should match.

---

### Method 3: Documentation Analysis

**What**: Review Datadog's official iOS SDK documentation

**Process**:
1. Read https://docs.datadoghq.com/mobile_app_testing/mobile_app_tests/ios/
2. Note which classes/methods are documented
3. Only documented APIs are meant for users
4. Bind only those APIs

**Why this works**: If Datadog doesn't document it, users shouldn't use it.

---

### Method 4: Naming Pattern Analysis

**What**: Identify internal types by naming conventions

**Patterns indicating INTERNAL types** (don't bind):
- Names starting with underscore: `_InternalClass`
- Names containing "Internal": `DDInternalStorage`
- Names containing "Private": `PrivateLogger`
- Complex nested event structures: `DDRUMActionEventActionCrashCount`

**Patterns indicating PUBLIC types** (do bind):
- Simple top-level classes: `DDConfiguration`, `DDDatadog`
- Monitor classes: `DDGlobalRUM`, `DDGlobalLogs`, `DDGlobalTracer`
- Config classes: `DDRUMConfiguration`, `DDLogsConfiguration`

**Why this works**: Apple conventions indicate public vs private APIs.

---

### Method 5: Compare Generated vs Needed

**What**: Show concrete example of generated bloat vs what users actually need

See [Concrete Example: RUM Bindings](#concrete-example-rum-bindings) below for detailed comparison.

**Why this works**: Demonstrates that 95% of generated code is unnecessary.

---

## Concrete Example: RUM Bindings

### What Objective Sharpie Generated

**File**: `Generated/DatadogRUM/ApiDefinitions.cs`
- **Lines of code**: 7,199
- **Number of interfaces**: 294
- **Includes**:
  - All event data structures (DDRUMActionEvent, DDRUMViewEvent, etc.)
  - All internal event properties (timestamps, IDs, nested structures)
  - All predicates and filters
  - SwiftUI-specific bindings
  - UIKit-specific bindings
  - Internal instrumentation classes

**Example of excessive nesting**:
```csharp
interface DDRUMActionEvent {
    DDRUMActionEventAction Action { get; set; }
    DDRUMActionEventApplication Application { get; set; }
    DDRUMActionEventCITest CiTest { get; set; }
    DDRUMActionEventConnectivity Connectivity { get; set; }
    // ... 30 more properties ...
}

interface DDRUMActionEventAction {
    DDRUMActionEventActionCrash Crash { get; set; }
    DDRUMActionEventActionError Error { get; set; }
    DDRUMActionEventActionFrustration Frustration { get; set; }
    // ... continues nesting deeper ...
}

interface DDRUMActionEventActionCrash {
    long Count { get; set; }
}

// ... 291 more interfaces like this!
```

### What Users Actually Need

**File**: `Manual/DatadogRUM/RUM.cs` (proposed)
- **Lines of code**: ~250
- **Number of interfaces**: 3
- **Includes**: Only user-facing monitoring APIs

**Example of clean minimal binding**:
```csharp
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS
{
    // @interface DDGlobalRUM : NSObject
    [BaseType(typeof(NSObject))]
    interface RUM
    {
        // Start view
        [Static, Export("startViewWithKey:name:attributes:")]
        void StartView(string key, string name, NSDictionary? attributes);

        // Stop view
        [Static, Export("stopViewWithKey:attributes:")]
        void StopView(string key, NSDictionary? attributes);

        // Add action
        [Static, Export("addActionWithType:name:attributes:")]
        void AddAction(RUMActionType type, string name, NSDictionary? attributes);

        // Add error
        [Static, Export("addErrorWithMessage:source:attributes:")]
        void AddError(string message, RUMErrorSource source, NSDictionary? attributes);

        // Start resource
        [Static, Export("startResourceWithKey:httpMethod:urlString:attributes:")]
        void StartResource(string key, string httpMethod, string url, NSDictionary? attributes);

        // Stop resource
        [Static, Export("stopResourceWithKey:statusCode:size:attributes:")]
        void StopResource(string key, int? statusCode, long? size, NSDictionary? attributes);

        // Global attributes
        [Static, Export("addAttributeForKey:value:")]
        void AddAttribute(string key, NSObject value);

        [Static, Export("removeAttributeForKey:")]
        void RemoveAttribute(string key);
    }

    // Supporting enums
    [Native]
    enum RUMActionType : long
    {
        Tap,
        Scroll,
        Swipe,
        Custom
    }

    [Native]
    enum RUMErrorSource : long
    {
        Network,
        Source,
        Console,
        Custom
    }
}
```

### Comparison

| Metric | Generated (Option A) | Manual (Option B) |
|--------|---------------------|-------------------|
| Lines of code | 7,199 | ~250 |
| Number of types | 294 | 3 |
| User-facing APIs | ~3% | 100% |
| Internal APIs | ~97% | 0% |
| Documentation burden | Very high | Low |
| Maintenance effort | High | Low |
| User confusion | High | Low |

**Conclusion**: Option B provides 97% less code while covering 100% of user needs.

---

## Decision Matrix

Use this matrix to decide which APIs to bind:

| Criteria | Bind It | Don't Bind It |
|----------|---------|---------------|
| **Documented by Datadog** | ✅ Yes | ❌ No |
| **Has Android equivalent** | ✅ Yes | ⚠️ Maybe (verify) |
| **In api-surface-swift** | ✅ Yes | ❌ No |
| **Top-level class** | ✅ Yes | ⚠️ Maybe |
| **Nested event structure** | ❌ No | ✅ Skip |
| **Starts with underscore** | ❌ No | ✅ Skip |
| **Contains "Internal"** | ❌ No | ✅ Skip |
| **Contains "Private"** | ❌ No | ✅ Skip |
| **Used in examples** | ✅ Yes | ❌ No |

---

## Next Steps

### Phase 1: Essential Bindings (1-2 days)

1. **Extract api-surface-swift files** from dd-sdk-ios repo
2. **Create manual bindings** for:
   - DatadogCore: Initialization, configuration
   - DatadogRUM: View/action/error/resource tracking
   - DatadogLogs: Basic logging methods
   - DatadogTrace: Span creation and management
3. **Test with sample app**
4. **Document usage examples**

### Phase 2: Validation (1 day)

1. **Cross-reference with Android** - Ensure feature parity
2. **Review with team** - Get feedback on API surface
3. **Test real-world scenarios** - Build sample integrations
4. **Update documentation**

### Phase 3: Expansion (Ongoing)

1. **Monitor user requests** - Track which APIs users ask for
2. **Add incrementally** - Bind additional APIs as needed
3. **Reference generated code** - Use as template when expanding
4. **Maintain consistency** - Keep clean, minimal philosophy

---

## Implementation Checklist

For each iOS framework:

- [ ] **Extract official API surface** from dd-sdk-ios repo
- [ ] **Identify user-facing classes** (top-level, documented)
- [ ] **Create manual ApiDefinitions.cs** with only essential APIs
- [ ] **Create StructsAndEnums.cs** for supporting types
- [ ] **Add proper namespaces** (all in `Datadog.iOS`)
- [ ] **Test compilation** - Ensure no errors
- [ ] **Create usage examples** - Document common scenarios
- [ ] **Add to sample app** - Real-world integration test

---

## Related Documentation

- [IDENTIFYING_USER_FACING_APIS.md]() - Detailed methodology
- [RUM_BINDING_COMPARISON.md]() - Concrete before/after example
- [UNIFIED_API_DESIGN.md]() - Cross-platform API design
- [PROJECT_OVERVIEW.md](../../project/PROJECT_GUIDE.html) - Overall project architecture

---

## Reference Materials

### Archive Generated Bindings

```bash
# Keep generated code for reference
mkdir -p _reference/ios-bindings-generated
mv Datadog.MAUI.iOS.Binding/*/Generated/ _reference/ios-bindings-generated/
```

### Official Resources

- [dd-sdk-ios GitHub](https://github.com/DataDog/dd-sdk-ios) - Source code
- [API Surface Files](https://github.com/DataDog/dd-sdk-ios/tree/main/DatadogCore) - Official public API
- [iOS SDK Docs](https://docs.datadoghq.com/mobile_app_testing/mobile_app_tests/ios/) - User documentation
- [Android Bindings](../Datadog.MAUI.Android.Binding/) - Reference implementation

---

## Summary

**Recommended Strategy**: Create minimal manual bindings (Option B)

**Key Principles**:
1. **Opt-in approach** - Expose only what users need
2. **Use official API surface** - Trust Datadog's public API definition
3. **Cross-reference with Android** - Ensure feature parity
4. **Clean, maintainable code** - Quality over quantity
5. **Iterative expansion** - Add APIs based on real user needs

**Expected Result**: ~250-500 lines of clean bindings per framework vs 7,000+ lines of generated bloat.

**Time Savings**: 1-2 days for manual bindings vs 2-3 days to fix generated code + ongoing maintenance burden.
