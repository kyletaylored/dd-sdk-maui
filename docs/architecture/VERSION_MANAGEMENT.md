---
layout: default
title: Version Management
parent: Architecture
nav_order: 4
permalink: /architecture/version-management
---

# Version Management Guide

This document explains how versions are managed in the Datadog MAUI SDK project.

## Current State: Phase 1 Implementation

We use a **conditional properties** approach that allows for future independent versioning of platform bindings and the plugin, while currently maintaining a single version for simplicity.

### Version Properties

All version properties are defined in [Directory.Build.props](../Directory.Build.props):

```xml
<!-- Base version - default for all components when not individually specified -->
<DatadogSdkVersion>3.5.0</DatadogSdkVersion>

<!-- Native SDK versions (what we fetch from upstream) -->
<DatadogAndroidSdkVersion>3.5.0</DatadogAndroidSdkVersion>  <!-- defaults to DatadogSdkVersion -->
<DatadogiOSSdkVersion>3.5.0</DatadogiOSSdkVersion>          <!-- defaults to DatadogSdkVersion -->

<!-- Platform-specific binding versions (NuGet packages we publish) -->
<AndroidBindingVersion>3.5.0</AndroidBindingVersion>  <!-- defaults to DatadogAndroidSdkVersion -->
<iOSBindingVersion>3.5.0</iOSBindingVersion>          <!-- defaults to DatadogiOSSdkVersion -->

<!-- Plugin version (unified package) -->
<PluginVersion>3.5.0</PluginVersion>                   <!-- defaults to DatadogSdkVersion -->

<!-- Symbols package (independent) -->
<DatadogSymbolsVersion>0.0.2</DatadogSymbolsVersion>
```

### Current Behavior

**All packages share the same version** (`3.5.0`) because of cascading defaults:
- `DatadogAndroidSdkVersion` defaults to `$(DatadogSdkVersion)` → `3.5.0`
- `DatadogiOSSdkVersion` defaults to `$(DatadogSdkVersion)` → `3.5.0`
- `AndroidBindingVersion` defaults to `$(DatadogAndroidSdkVersion)` → `3.5.0`
- `iOSBindingVersion` defaults to `$(DatadogiOSSdkVersion)` → `3.5.0`
- `PluginVersion` defaults to `$(DatadogSdkVersion)` → `3.5.0`

This maintains the current single-version simplicity while providing two levels of version control:
1. **Native SDK versions** (what we fetch and bind)
2. **Binding package versions** (what we publish to NuGet)

## Package Types

### 1. Native SDK Bindings

Platform-specific C# bindings to Datadog's native iOS and Android SDKs:

**Android Binding Packages** (13 packages):
- `Datadog.MAUI.Android.Core` (version: `$(AndroidBindingVersion)`)
- `Datadog.MAUI.Android.RUM`
- `Datadog.MAUI.Android.Logs`
- ... (and 10 more)

**iOS Binding Packages** (10 packages):
- `Datadog.MAUI.iOS.Core` (version: `$(iOSBindingVersion)`)
- `Datadog.MAUI.iOS.RUM`
- `Datadog.MAUI.iOS.Logs`
- ... (and 7 more)

### 2. Unified Plugin

**Datadog.MAUI** (version: `$(PluginVersion)`):
- Cross-platform unified API
- References both Android and iOS bindings
- Provides single initialization point for apps

### 3. Build-Time Tooling

**Datadog.MAUI.Symbols** (version: `$(DatadogSymbolsVersion)`):
- Independent versioning (currently `0.0.2`)
- MSBuild task for uploading debug symbols
- Separate lifecycle from SDK packages

## Version Update Scenarios

### Scenario 1: Standard SDK Update (Current Behavior)

**When**: Updating to new versions of both iOS and Android native SDKs

**Action**: Update only `DatadogSdkVersion`

```bash
# Update base version
sed -i '' 's/<DatadogSdkVersion>3.5.0<\/DatadogSdkVersion>/<DatadogSdkVersion>3.6.0<\/DatadogSdkVersion>/' Directory.Build.props
```

**Result**: All packages move to `3.6.0`
- Android bindings: `3.6.0`
- iOS bindings: `3.6.0`
- Plugin: `3.6.0`

### Scenario 2: Different Native SDK Versions (Future)

**When**: iOS releases `3.5.1` with a critical fix, but Android is still at `3.5.0`

**Action**: Override `DatadogiOSSdkVersion` in the root Directory.Build.props

```xml
<DatadogiOSSdkVersion>3.5.1</DatadogiOSSdkVersion>
```

**Result**: Different native SDKs are fetched, bindings follow native versions
- Android bindings: `3.5.0` (wrapping Android SDK `3.5.0` from Maven)
- iOS bindings: `3.5.1` (wrapping iOS SDK `3.5.1` from GitHub)
- Plugin: `3.5.0` (still on base version)

**Note**: When native SDK versions diverge, you typically want to:
1. Update the plugin version to the higher version (e.g., `3.5.1`)
2. Tag the release with the plugin version

### Scenario 4: Android Binding Patch (Future)

**When**: Android binding needs a C# marshalling fix, but native SDK and iOS are unchanged

**Action**: Override `AndroidBindingVersion` in the root Directory.Build.props

```xml
<AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
```

**Result**: Android binding packages diverge from native SDK version
- Android native SDK: `3.5.0` (fetched from Maven)
- Android bindings: `3.5.0.1` (published to NuGet with binding fix)
- iOS bindings: `3.5.0`
- Plugin: `3.5.0`

### Scenario 6: iOS Binding Patch (Future)

**When**: iOS binding needs a C# marshalling fix, but native SDK and Android are unchanged

**Action**: Override `iOSBindingVersion` in the root Directory.Build.props

```xml
<iOSBindingVersion>3.5.0.2</iOSBindingVersion>
```

**Result**: iOS binding packages diverge from native SDK version
- iOS native SDK: `3.5.0` (fetched from GitHub)
- iOS bindings: `3.5.0.2` (published to NuGet with binding fix)
- Android bindings: `3.5.0`
- Plugin: `3.5.0`

### Scenario 7: Plugin-Only Enhancement (Future)

**When**: Adding new plugin API or enhancement without touching bindings

**Action**: Override `PluginVersion` in the root Directory.Build.props

```xml
<PluginVersion>3.5.1</PluginVersion>
```

**Result**: Plugin diverges
- Android bindings: `3.5.0`
- iOS bindings: `3.5.0`
- Plugin: `3.5.1`

### Scenario 8: Symbols Package Update

**When**: Updating the symbols upload tool

**Action**: Update `DatadogSymbolsVersion`

```bash
sed -i '' 's/<DatadogSymbolsVersion>0.0.2<\/DatadogSymbolsVersion>/<DatadogSymbolsVersion>0.0.3<\/DatadogSymbolsVersion>/' Directory.Build.props
```

**Result**: Only symbols package changes (no impact on SDK)

## Version Validation

### Git Tag Validation

When creating a release with tag `vX.Y.Z`:

**Current validation** (implemented in `.github/workflows/build-all.yml`):

1. Extracts version from git tag (e.g., `v3.5.0` → `3.5.0`)
2. Extracts all version properties from Directory.Build.props:
   - `DatadogSdkVersion` (base version)
   - `AndroidBindingVersion` (defaults to base if not explicitly set)
   - `iOSBindingVersion` (defaults to base if not explicitly set)
   - `PluginVersion` (defaults to base if not explicitly set)
3. Validates tag matches `DatadogSdkVersion` (for Phase 1)
4. Displays all versions in validation output
5. Notes if versions have diverged

**Example validation output**:
```
📦 Version Information
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🏷️  Git tag version: 3.5.0

📄 Directory.Build.props versions:
   Base SDK version:        3.5.0
   Android binding version: 3.5.0
   iOS binding version:     3.5.0
   Plugin version:          3.5.0
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✅ All versions are synchronized at 3.5.0

✅ Version validation passed
```

**Future (Phase 2+)**: When versions diverge, validation logic will be updated to compare tag against `PluginVersion` instead of `DatadogSdkVersion`.

### Example Future Release (Phase 2)

**Git tag**: `v3.5.1`

**Validation output**:
```
📦 Version Information
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🏷️  Git tag version: 3.5.1

📄 Directory.Build.props versions:
   Base SDK version:        3.5.0
   Android binding version: 3.5.0.1
   iOS binding version:     3.5.0.2
   Plugin version:          3.5.1
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ℹ️  Note: Platform versions have diverged from base version
   This is expected when making platform-specific or plugin-specific releases.

✅ Version validation passed
```

**Release notes** will include:
```markdown
## Version Information

- **Base SDK Version**: 3.5.0
- **Android Binding Version**: 3.5.0.1
- **iOS Binding Version**: 3.5.0.2
- **Plugin Version**: 3.5.1

> Note: When all versions are the same, packages are synchronized.
> Platform-specific versions indicate targeted fixes or updates.
```

**NuGet Packages**:
- Datadog.MAUI 3.5.1
- Datadog.MAUI.Android.* 3.5.0.1 (13 packages)
- Datadog.MAUI.iOS.* 3.5.0.2 (10 packages)

## Migration Phases

### ✅ Phase 1: Preparation (COMPLETED)

**Status**: ✅ Fully Implemented

**Changes Made**:
1. ✅ Added conditional version properties to [Directory.Build.props](../Directory.Build.props):
   - `AndroidBindingVersion` (defaults to `DatadogSdkVersion`)
   - `iOSBindingVersion` (defaults to `DatadogSdkVersion`)
   - `PluginVersion` (defaults to `DatadogSdkVersion`)
2. ✅ Updated [Makefile](../Makefile) to pass version to iOS framework download script
3. ✅ Enhanced `.github/workflows/build-all.yml` version validation:
   - Extracts and displays all version properties
   - Detects when versions have diverged
   - Includes version info in release notes
4. ✅ Created comprehensive documentation (this file)

**Behavior**: All properties default to same value (single version) - no breaking changes

**Result**: Future-ready for independent versioning with zero impact on current workflow

### Phase 2: First Divergence (Ready When Needed)

**Trigger**: First time we need platform-specific or plugin-specific patch release

**Status**: ⏳ Waiting for need (infrastructure ready)

**Required Actions**:
1. ✅ **Edit version property** in Directory.Build.props (just change one line!)
2. ✅ **Workflow handles it automatically** - no changes needed (already implemented)
3. ⚠️ **Update validation logic** - Change tag comparison from `DatadogSdkVersion` to `PluginVersion`

**Example**: Android binding needs fix at `3.5.0.1`:
```xml
<!-- In Directory.Build.props - just override the property -->
<AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
```

That's it! The build system will automatically:
- Build Android bindings with version `3.5.0.1`
- Keep iOS bindings at `3.5.0`
- Keep plugin at `3.5.0`
- Display all versions in validation output
- Include version info in release notes

**Phase 2 Workflow Update**:
When you're ready to let tags represent plugin version instead of base version, update line 639 in `.github/workflows/build-all.yml`:

```bash
# Change from:
if [ "$TAG_VERSION" != "$BASE_VERSION" ]; then

# To:
if [ "$TAG_VERSION" != "$PLUGIN_VERSION" ]; then
```

### Phase 3: Nested Directory.Build.props (Optional)

**When**: If we frequently update platform versions independently

**Action**: Create platform-specific Directory.Build.props files:

```bash
# Create Android-specific props
Datadog.MAUI.Android.Binding/Directory.Build.props
```

```xml
<Project>
  <Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))"/>

  <PropertyGroup>
    <AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
    <Version>$(AndroidBindingVersion)</Version>
  </PropertyGroup>
</Project>
```

This approach:
- Isolates platform version changes to platform directories
- Imports root props for common settings
- Overrides only the version

## Command-Line Overrides

You can override any version property via command-line:

```bash
# Build with specific Android binding version
dotnet build /p:AndroidBindingVersion=3.5.0.2

# Pack with specific plugin version
dotnet pack /p:PluginVersion=3.6.0

# Build everything with custom base version
dotnet build /p:DatadogSdkVersion=3.7.0
```

## Best Practices

### 1. Minimize Version Divergence

Keep versions synchronized when possible:
- ✅ Update all together for native SDK updates
- ✅ Use patch versions (x.y.z.1) for binding-specific fixes
- ⚠️ Only diverge when absolutely necessary

### 2. Follow Semantic Versioning

- **Major.Minor.Patch** for plugin and base versions
- **Major.Minor.Patch.Revision** for binding patches
- Match native SDK major.minor when possible

### 3. Document Version Combinations

In release notes, always show:
- Plugin version (git tag)
- Android binding version
- iOS binding version
- What changed in each

### 4. Version Property Hierarchy

The version system has two layers:

**Layer 1: Native SDK Versions (what we fetch)**
```
DatadogSdkVersion (base)
    ↓
├── DatadogAndroidSdkVersion → Controls Maven fetch
└── DatadogiOSSdkVersion → Controls GitHub download
```

**Layer 2: Binding Package Versions (what we publish)**
```
DatadogAndroidSdkVersion
    ↓
    AndroidBindingVersion → NuGet package version

DatadogiOSSdkVersion
    ↓
    iOSBindingVersion → NuGet package version

DatadogSdkVersion
    ↓
    PluginVersion → Unified package version
```

## Comparison to Microsoft's Xamarin.AndroidX

Microsoft uses **fully independent versioning** for each binding package:
- `Xamarin.AndroidX.Navigation.UI`: `2.9.6.1`
- `Xamarin.AndroidX.Navigation.Fragment`: `2.9.6.3`
- `Xamarin.AndroidX.Core`: `1.10.1.4`

**Our approach**:
- More conservative (platform-level synchronization)
- Simpler for users (fewer version combinations)
- Still allows platform-specific patches
- Plugin provides unified entry point

## Related Documentation

- [Directory.Build.props](../Directory.Build.props) - Version property definitions
- [.github/workflows/build-all.yml](../.github/workflows/build-all.yml) - Version validation logic
- [Packaging Architecture](packaging) - NuGet package structure
