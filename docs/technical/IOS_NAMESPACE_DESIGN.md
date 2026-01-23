# iOS Binding Namespace Design

## The Current Problem

### What You're Seeing

```csharp
// Android - Works, consistent pattern
using Com.Datadog.Android;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Log;

// iOS - Inconsistent, framework names embedded in namespace
using Datadog.iOS.DatadogCore;      // Why "DatadogCore" in the namespace?
using Datadog.iOS.DatadogRUM;       // Why not just "Datadog.iOS.RUM"?
using Datadog.iOS.DatadogLogs;      // This is confusing!
```

**The question**: Why can't iOS bindings use a cleaner structure like `Datadog.iOS.Core`, `Datadog.iOS.RUM`, `Datadog.iOS.Logs`?

## Root Cause

The namespace is **hardcoded in the Objective Sharpie generation script**:

**File**: `scripts/generate-ios-bindings-sharpie.sh` (line 144)
```bash
sharpie bind \
    --output="$FRAMEWORK_OUTPUT" \
    --namespace="Datadog.iOS.$FRAMEWORK" \    # <-- HERE!
    --sdk="$SDK" \
    -scope "$HEADERS_PATH" \
    "$HEADER_TO_BIND"
```

The `$FRAMEWORK` variable contains the full framework name:
- `DatadogCore` → `Datadog.iOS.DatadogCore`
- `DatadogRUM` → `Datadog.iOS.DatadogRUM`
- `DatadogLogs` → `Datadog.iOS.DatadogLogs`

This causes the **redundant "Datadog" prefix** in the namespace.

## Why This Happened

### Apple's Framework Naming

Apple's Datadog iOS SDK uses these framework names:
```
DatadogCore.xcframework
DatadogRUM.xcframework
DatadogLogs.xcframework
DatadogTrace.xcframework
DatadogCrashReporting.xcframework
DatadogSessionReplay.xcframework
DatadogWebViewTracking.xcframework
```

These are the **actual framework names** - we didn't choose them. Apple requires the "Datadog" prefix to avoid naming conflicts with other SDKs.

### Sharpie Script Logic

The script iterates through frameworks and uses the framework name directly:
```bash
FRAMEWORKS=(
    "DatadogCore"
    "DatadogRUM"
    "DatadogLogs"
    # ...
)

for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    # Generates: Datadog.iOS.$FRAMEWORK
    sharpie bind --namespace="Datadog.iOS.$FRAMEWORK" ...
done
```

**Result**: We get `Datadog.iOS.DatadogCore` instead of `Datadog.iOS.Core`.

## Comparison with Android

### Android Approach
Android uses Java/Kotlin packages that follow standard naming:
```
com.datadog.android           → Com.Datadog.Android
com.datadog.android.rum       → Com.Datadog.Android.Rum
com.datadog.android.log       → Com.Datadog.Android.Log
```

The package names themselves are clean and logical. Xamarin.Android just translates them to C# with capital letters.

### iOS Approach (Current)
iOS framework names have the "Datadog" prefix baked in:
```
DatadogCore.xcframework       → Datadog.iOS.DatadogCore
DatadogRUM.xcframework        → Datadog.iOS.DatadogRUM
DatadogLogs.xcframework       → Datadog.iOS.DatadogLogs
```

We're preserving the framework names, which creates the redundancy.

## Proposed Solutions

### Option 1: Strip "Datadog" Prefix (Recommended)

Modify the Sharpie script to remove the "Datadog" prefix:

**Change in `scripts/generate-ios-bindings-sharpie.sh`**:
```bash
for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    # Strip "Datadog" prefix for cleaner namespace
    NAMESPACE_SUFFIX="${FRAMEWORK#Datadog}"  # Remove leading "Datadog"

    # If no prefix was removed (e.g., "OpenTelemetryApi"), use as-is
    if [ "$NAMESPACE_SUFFIX" = "$FRAMEWORK" ]; then
        NAMESPACE_SUFFIX="$FRAMEWORK"
    fi

    sharpie bind \
        --namespace="Datadog.iOS.$NAMESPACE_SUFFIX" \
        ...
done
```

**Result**:
```csharp
using Datadog.iOS.Core;             // Clean!
using Datadog.iOS.RUM;              // Better!
using Datadog.iOS.Logs;             // Consistent with Android pattern!
using Datadog.iOS.Trace;
using Datadog.iOS.CrashReporting;
using Datadog.iOS.SessionReplay;
using Datadog.iOS.WebViewTracking;
```

**Benefits**:
- Cleaner, more readable namespaces
- Closer alignment with Android pattern
- Less redundancy
- **Breaking change** - requires version bump

### Option 2: Use Simple Naming Like Android

Go even further and use simpler names:

```bash
# Map framework names to simple suffixes
declare -A NAMESPACE_MAP=(
    ["DatadogCore"]="Core"
    ["DatadogRUM"]="Rum"
    ["DatadogLogs"]="Logs"
    ["DatadogTrace"]="Trace"
    ["DatadogCrashReporting"]="CrashReporting"
    ["DatadogSessionReplay"]="SessionReplay"
    ["DatadogWebViewTracking"]="WebViewTracking"
)

NAMESPACE_SUFFIX="${NAMESPACE_MAP[$FRAMEWORK]}"
sharpie bind --namespace="Datadog.iOS.$NAMESPACE_SUFFIX" ...
```

**Result**:
```csharp
using Datadog.iOS.Core;
using Datadog.iOS.Rum;              // Lowercase to match Android
using Datadog.iOS.Logs;
```

**Benefits**:
- Even cleaner
- Matches Android capitalization pattern
- **Breaking change**

### Option 3: Keep Current, Document It

Keep `Datadog.iOS.DatadogCore` but document why it exists:
- Preserves Apple's framework naming exactly
- No breaking changes
- Less confusing for developers familiar with native iOS SDK
- Continue using unified plugin to abstract away the difference

## Impact Analysis

### Files That Would Change

If we implement Option 1 or 2, these files need updates:

1. **Generation Script**:
   - `scripts/generate-ios-bindings-sharpie.sh`

2. **All Generated Bindings**:
   - `Datadog.MAUI.iOS.Binding/Generated/*/ApiDefinitions.cs`
   - `Datadog.MAUI.iOS.Binding/Generated/*/StructsAndEnums.cs`

3. **Unified Plugin**:
   - `Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs`
   - `Datadog.MAUI.Plugin/Platforms/iOS/Rum.ios.cs`
   - `Datadog.MAUI.Plugin/Platforms/iOS/Logs.ios.cs`
   - `Datadog.MAUI.Plugin/Platforms/iOS/Tracer.ios.cs`

4. **Sample App**:
   - `samples/DatadogMauiSample/Platforms/iOS/AppDelegate.cs`

5. **Documentation**:
   - All docs that reference iOS namespaces

### Breaking Change Implications

This is a **breaking change** for anyone using platform-specific iOS bindings directly:

**Before**:
```csharp
using Datadog.iOS.DatadogCore;
DDDatadog.InitializeWithConfiguration(config, consent);
```

**After**:
```csharp
using Datadog.iOS.Core;
DDDatadog.InitializeWithConfiguration(config, consent);
```

**Mitigation**:
- Most users use the unified `Datadog.MAUI.Plugin` API, not direct bindings
- Only platform-specific initialization code (AppDelegate.cs) is affected
- Version bump to v2.0.0 to signal breaking change
- Migration guide in docs

## Recommendation

### Short Term: Keep Current Structure (Option 3)

**Why**:
- iOS bindings are still in development
- Sample app is the only consumer
- Breaking changes before v1.0 release are acceptable but unnecessary churn
- Focus should be on completing functionality, not bikeshedding namespaces

**Actions**:
1. Document the namespace pattern in `docs/technical/NAMESPACE_CONSISTENCY.md` ✅
2. Note that it differs from Android and why
3. Emphasize the unified plugin as the primary API
4. Add TODOs for future cleanup

### Long Term: Implement Option 1 (Before v1.0)

**When**: Before declaring the iOS bindings stable (v1.0 release)

**Why**:
- Better developer experience
- Closer alignment with Android
- Now is the time for breaking changes (pre-1.0)
- Clean foundation for future

**Implementation Steps**:
1. Update `scripts/generate-ios-bindings-sharpie.sh` to strip "Datadog" prefix
2. Regenerate all bindings: `./scripts/generate-ios-bindings-sharpie.sh`
3. Update consolidated bindings in each project folder
4. Update unified plugin iOS platform implementations
5. Update sample app AppDelegate.cs
6. Update all documentation
7. Run full test suite
8. Update CHANGELOG.md with breaking changes
9. Commit with message: "BREAKING: Simplify iOS binding namespaces"

## Example Implementation

Here's the exact change needed in `scripts/generate-ios-bindings-sharpie.sh`:

```bash
for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    echo -e "${YELLOW}Processing $FRAMEWORK...${NC}"

    # ... existing framework path setup ...

    # Strip "Datadog" prefix for cleaner C# namespaces
    # DatadogCore → Core, DatadogRUM → RUM, etc.
    if [[ "$FRAMEWORK" == Datadog* ]]; then
        NAMESPACE_SUFFIX="${FRAMEWORK#Datadog}"
    else
        # Frameworks like "OpenTelemetryApi" keep their name
        NAMESPACE_SUFFIX="$FRAMEWORK"
    fi

    # Create framework-specific output directory
    FRAMEWORK_OUTPUT="$OUTPUT_DIR/$FRAMEWORK"
    mkdir -p "$FRAMEWORK_OUTPUT"

    # Run Objective Sharpie with cleaned namespace
    echo -e "  Generating bindings with namespace: Datadog.iOS.$NAMESPACE_SUFFIX"
    sharpie bind \
        --output="$FRAMEWORK_OUTPUT" \
        --namespace="Datadog.iOS.$NAMESPACE_SUFFIX" \
        --sdk="$SDK" \
        -scope "$HEADERS_PATH" \
        "$HEADER_TO_BIND" \
        2>&1 | grep -v "warning:" || true

    # ... rest of script ...
done
```

## Decision

**Current Status**: Using Option 3 (keep current structure)

**Next Steps**:
1. Document this decision ✅
2. Complete iOS binding functionality first
3. Revisit namespace structure before v1.0
4. Get user feedback on whether this is confusing

**Decision Date**: 2026-01-22

**Rationale**: Premature to make breaking changes while bindings are still incomplete. Focus on functionality first, polish later.
