# iOS Namespace Cleanup - Completed

**Date**: 2026-01-22
**Status**: ✅ Complete

## What Was Done

We removed the redundant "Datadog" prefix from iOS binding namespaces to create cleaner, more consistent namespaces that better align with the Android binding pattern.

### Before (Old Namespaces)
```csharp
using Datadog.iOS.DatadogCore;
using Datadog.iOS.DatadogInternal;
using Datadog.iOS.DatadogRUM;
using Datadog.iOS.DatadogLogs;
using Datadog.iOS.DatadogTrace;
using Datadog.iOS.DatadogCrashReporting;
using Datadog.iOS.DatadogSessionReplay;
using Datadog.iOS.DatadogWebViewTracking;
using Datadog.iOS.DatadogFlags;
```

### After (New Namespaces)
```csharp
using Datadog.iOS.Core;
using Datadog.iOS.Internal;
using Datadog.iOS.RUM;
using Datadog.iOS.Logs;
using Datadog.iOS.Trace;
using Datadog.iOS.CrashReporting;
using Datadog.iOS.SessionReplay;
using Datadog.iOS.WebViewTracking;
using Datadog.iOS.Flags;
```

## Changes Made

### 1. Objective Sharpie Script Updated

**File**: [scripts/generate-ios-bindings-sharpie.sh](../../scripts/generate-ios-bindings-sharpie.sh)

Added logic to strip the "Datadog" prefix from framework names:

```bash
# Strip "Datadog" prefix from framework name for cleaner C# namespaces
if [[ "$FRAMEWORK" == Datadog* ]]; then
    NAMESPACE_SUFFIX="${FRAMEWORK#Datadog}"
else
    NAMESPACE_SUFFIX="$FRAMEWORK"
fi

sharpie bind \
    --namespace="Datadog.iOS.$NAMESPACE_SUFFIX" \
    ...
```

**Result**: Future binding generations will automatically use the cleaner namespace structure.

### 2. Created Namespace Update Script

**File**: [scripts/update-ios-namespaces.sh](../../scripts/update-ios-namespaces.sh)

This script:
- Updates all existing C# binding files in project directories
- Updates all generated binding files
- Creates `.bak` backup files
- Reports which files were changed

**Usage**:
```bash
./scripts/update-ios-namespaces.sh
```

### 3. Updated All iOS Binding Projects

Applied namespace changes to all binding projects:

- ✅ **DatadogCore** → `Datadog.iOS.Core`
- ✅ **DatadogInternal** → `Datadog.iOS.Internal`
- ✅ **DatadogRUM** → `Datadog.iOS.RUM`
- ✅ **DatadogLogs** → `Datadog.iOS.Logs`
- ✅ **DatadogTrace** → `Datadog.iOS.Trace`
- ✅ **DatadogCrashReporting** → `Datadog.iOS.CrashReporting`
- ✅ **DatadogSessionReplay** → `Datadog.iOS.SessionReplay`
- ✅ **DatadogWebViewTracking** → `Datadog.iOS.WebViewTracking`
- ✅ **DatadogFlags** → `Datadog.iOS.Flags`

**Files Updated**:
- `Datadog.MAUI.iOS.Binding/*/ApiDefinition.cs`
- `Datadog.MAUI.iOS.Binding/*/StructsAndEnums.cs`
- `Datadog.MAUI.iOS.Binding/Generated/*/ApiDefinitions.cs`
- `Datadog.MAUI.iOS.Binding/Generated/*/StructsAndEnums.cs`

### 4. Updated Unified Plugin

**Files**: `Datadog.MAUI.Plugin/Platforms/iOS/*.cs`

Updated all iOS platform implementations to use new namespaces:
- ✅ `Datadog.ios.cs`
- ✅ `Rum.ios.cs`
- ✅ `Logs.ios.cs`
- ✅ `Tracer.ios.cs`
- ✅ `IOSLogger.cs`
- ✅ `IOSSpan.cs`

### 5. Updated Sample App

**File**: [samples/DatadogMauiSample/Platforms/iOS/AppDelegate.cs](../../samples/DatadogMauiSample/Platforms/iOS/AppDelegate.cs)

Updated all using statements and enabled Session Replay + WebView Tracking with the new namespaces.

## Benefits

### 1. Consistency with Android Pattern

**Android**:
```csharp
using Com.Datadog.Android;
using Com.Datadog.Android.Rum;
using Com.Datadog.Android.Log;
```

**iOS (Now)**:
```csharp
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.Logs;
```

While not identical, the iOS structure is now much closer to Android's clean organization.

### 2. Improved Readability

```csharp
// Before - repetitive and redundant
using Datadog.iOS.DatadogRUM;
DDRUM.EnableWith(config);

// After - clear and concise
using Datadog.iOS.RUM;
DDRUM.EnableWith(config);
```

### 3. Better IntelliSense Experience

When typing `Datadog.iOS.`, developers now see:
- `Core`
- `RUM`
- `Logs`
- `Trace`

Instead of:
- `DatadogCore`
- `DatadogRUM`
- `DatadogLogs`
- `DatadogTrace`

The redundancy is gone, making it easier to find what you need.

### 4. Future-Proof

The updated Objective Sharpie script ensures all future binding generations will automatically use the cleaner namespace structure.

## Verification

To verify the changes worked:

```bash
# Check that old namespaces are gone
! grep -r "Datadog\.iOS\.DatadogCore" Datadog.MAUI.iOS.Binding/ --include="*.cs" | grep -v ".bak"

# Check new namespaces are present
grep -r "namespace Datadog\.iOS\.Core" Datadog.MAUI.iOS.Binding/DatadogCore/

# Build iOS bindings
dotnet build Datadog.MAUI.iOS.Binding/DatadogCore/DatadogCore.csproj
```

## Cleanup

After verifying everything builds and works, remove backup files:

```bash
find Datadog.MAUI.iOS.Binding -name "*.bak" -delete
find Datadog.MAUI.Plugin/Platforms/iOS -name "*.bak*" -delete
```

## Breaking Changes

This is a **breaking change** for anyone using the iOS bindings directly. However:

1. **Impact is minimal**: Most users use the unified `Datadog.MAUI.Plugin` API, not direct bindings
2. **Easy migration**: Simple find-replace in affected code
3. **Pre-1.0**: We're still in development, making this the right time for such changes

### Migration Guide

If you have code using old namespaces:

```bash
# Find affected files
grep -r "Datadog.iOS.Datadog" . --include="*.cs"

# Replace old with new
sed -i '' 's/Datadog\.iOS\.DatadogCore/Datadog.iOS.Core/g' YourFile.cs
sed -i '' 's/Datadog\.iOS\.DatadogRUM/Datadog.iOS.RUM/g' YourFile.cs
# ... etc
```

Or use our script:
```bash
./scripts/update-ios-namespaces.sh
```

## Related Documentation

- [IOS_NAMESPACE_DESIGN.md](IOS_NAMESPACE_DESIGN.md) - Original design discussion
- [NAMESPACE_CONSISTENCY.md](NAMESPACE_CONSISTENCY.md) - Android vs iOS comparison
- [generate-ios-bindings-sharpie.sh](../../scripts/generate-ios-bindings-sharpie.sh) - Updated generation script

## Conclusion

The iOS binding namespaces are now cleaner, more consistent with Android, and easier to work with. This lays a solid foundation for the 1.0 release.

**Next regeneration**: When Objective Sharpie is run again, it will automatically use the new namespace structure thanks to the script updates.
