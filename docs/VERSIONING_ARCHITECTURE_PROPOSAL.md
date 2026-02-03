# Versioning Architecture Proposal: Independent Binding Versions

## Current State (Single Version for All Packages)

### Structure
```
/dd-sdk-maui/
├── Directory.Build.props
│   └── DatadogSdkVersion: 3.5.0 (applies to ALL packages)
├── Datadog.MAUI.Android.Binding/ (all packages: 3.5.0)
├── Datadog.MAUI.iOS.Binding/ (all packages: 3.5.0)
└── Datadog.MAUI.Plugin/ (version: 3.5.0)
```

### Limitations
- ❌ Can't release Android binding fix (3.5.0 → 3.5.0.1) without touching iOS
- ❌ Can't release plugin improvement (3.5.0 → 3.5.1) without new binding versions
- ❌ All packages must move in lockstep even for platform-specific changes
- ❌ Doesn't follow Microsoft's Xamarin.AndroidX pattern (independent versions)

### Current Advantage
- ✅ Simple: One version to track
- ✅ Easy validation: Git tag matches single version
- ✅ No user confusion about version combinations

---

## Proposed Architecture: Nested Directory.Build.props

### Goal
Enable independent versioning for Android bindings, iOS bindings, and the main plugin while maintaining version validation.

### Structure

```
/dd-sdk-maui/
├── Directory.Build.props (root)
│   ├── DatadogSdkVersion: 3.5.0 (base/fallback version)
│   ├── PluginVersion: 3.5.1 (plugin-specific)
│   └── Version: $(PluginVersion) (default for projects)
│
├── Datadog.MAUI.Android.Binding/
│   ├── Directory.Build.props (Android-specific)
│   │   └── AndroidBindingVersion: 3.5.0.1
│   │   └── Version: $(AndroidBindingVersion)
│   ├── dd-sdk-android-core/ (version: 3.5.0.1)
│   ├── dd-sdk-android-rum/ (version: 3.5.0.1)
│   └── ... (all inherit 3.5.0.1)
│
├── Datadog.MAUI.iOS.Binding/
│   ├── Directory.Build.props (iOS-specific)
│   │   └── iOSBindingVersion: 3.5.0.2
│   │   └── Version: $(iOSBindingVersion)
│   └── ... (all inherit 3.5.0.2)
│
└── Datadog.MAUI.Plugin/
    └── (inherits PluginVersion: 3.5.1 from root)
```

### How MSBuild Property Inheritance Works

MSBuild searches for `Directory.Build.props` files from the project directory upward:

1. **Project-level**: Check project directory
2. **Parent directories**: Walk up looking for `Directory.Build.props`
3. **Closest wins**: First one found is imported
4. **Import chain**: Can use `<Import>` to chain to parent

#### Example: dd-sdk-android-core.csproj

```
/dd-sdk-maui/Datadog.MAUI.Android.Binding/dd-sdk-android-core/dd-sdk-android-core.csproj
                                          ↑
                        Looks for Directory.Build.props here (not found)
                                          ↑
            /dd-sdk-maui/Datadog.MAUI.Android.Binding/Directory.Build.props
                                    FOUND! Uses this
                                          ↑
                        (Stops searching, doesn't go to root)
```

To chain to root:
```xml
<!-- Datadog.MAUI.Android.Binding/Directory.Build.props -->
<Project>
  <!-- Import root props first -->
  <Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))" />

  <!-- Override version for Android bindings -->
  <PropertyGroup>
    <AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
    <Version>$(AndroidBindingVersion)</Version>
  </PropertyGroup>
</Project>
```

---

## Implementation Options

### Option 1: Conditional Properties (Recommended for Now)

**Modify root Directory.Build.props only** - no nested files yet:

```xml
<Project>
  <PropertyGroup>
    <!-- Base version -->
    <DatadogSdkVersion>3.5.0</DatadogSdkVersion>

    <!-- Platform-specific versions (allow override) -->
    <AndroidBindingVersion Condition="'$(AndroidBindingVersion)' == ''">$(DatadogSdkVersion)</AndroidBindingVersion>
    <iOSBindingVersion Condition="'$(iOSBindingVersion)' == ''">$(DatadogSdkVersion)</iOSBindingVersion>

    <!-- Plugin version (can differ from bindings) -->
    <PluginVersion Condition="'$(PluginVersion)' == ''">$(DatadogSdkVersion)</PluginVersion>

    <!-- Default version for all projects -->
    <Version>$(PluginVersion)</Version>
  </PropertyGroup>
</Project>
```

**When ready to diverge**, add nested Directory.Build.props:

```xml
<!-- Datadog.MAUI.Android.Binding/Directory.Build.props -->
<Project>
  <Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))" />

  <PropertyGroup>
    <AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
    <Version>$(AndroidBindingVersion)</Version>
  </PropertyGroup>
</Project>
```

**Advantages**:
- ✅ No breaking changes now - still single version
- ✅ Easy migration path when needed
- ✅ Properties exist but all reference same value initially
- ✅ Can override via CLI: `dotnet pack /p:AndroidBindingVersion=3.5.0.2`

**Migration path**:
1. Phase 1: Add conditional properties (no behavior change)
2. Phase 2: Add nested Directory.Build.props when first divergence needed
3. Phase 3: Update validation workflows to handle multiple versions

### Option 2: Nested Directory.Build.props from Start

Create nested props files immediately with same versions:

```
/dd-sdk-maui/
├── Directory.Build.props (PluginVersion: 3.5.0)
├── Datadog.MAUI.Android.Binding/Directory.Build.props (3.5.0)
└── Datadog.MAUI.iOS.Binding/Directory.Build.props (3.5.0)
```

**Advantages**:
- ✅ Structure ready for future divergence
- ✅ Clear ownership of version properties

**Disadvantages**:
- ❌ Overhead of maintaining 3 files even when versions are same
- ❌ More validation complexity from day 1

### Option 3: Keep Current Single Version

Don't change anything - maintain single version for all packages.

**When to use**:
- Early in project lifecycle (✅ current state)
- Binding issues still being discovered
- No users yet depending on specific versions
- Team prefers simplicity over flexibility

**When to migrate away**:
- Frequent binding-only patch releases needed
- Platform versions regularly diverge
- Users complain about version churn
- Following Microsoft's AndroidX pattern becomes important

---

## Version Validation Changes

### Current Validation
```yaml
# Single git tag validation
- Extract version from git tag (v3.5.0)
- Compare to Directory.Build.props DatadogSdkVersion
- Fail if mismatch
```

### Proposed Validation (with Independent Versions)

#### SDK Release Tags (v3.5.x format)
```yaml
# Tag represents PLUGIN version
- Extract plugin version from git tag (v3.5.1)
- Compare to root Directory.Build.props PluginVersion
- Also extract binding versions from nested Directory.Build.props
- Document all three versions in release
```

#### Example Release with Divergent Versions

**Git tag**: `v3.5.1`

**Validation output**:
```
✅ Version Validation Passed

Plugin version: 3.5.1 (matches git tag)
Android binding version: 3.5.0.1
iOS binding version: 3.5.0.2

Release includes:
- Datadog.MAUI.Plugin 3.5.1
- Datadog.MAUI.Android.Binding 3.5.0.1 (and 13 module packages)
- Datadog.MAUI.iOS.Binding 3.5.0.2 (and 10 module packages)
```

---

## Package Dependency Management

### Current
```xml
<!-- Datadog.MAUI.Plugin.csproj -->
<ItemGroup>
  <!-- References binding meta-packages -->
  <ProjectReference Include="../Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj" />
  <ProjectReference Include="../Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj" />
</ItemGroup>
```

All packages have same version (3.5.0), so no version mismatch concerns.

### With Independent Versions

**Option A: Project References** (Current - No Change Needed)
```xml
<!-- Still use ProjectReference - NuGet handles version automatically -->
<ProjectReference Include="../Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj" />
```
- ✅ Versions automatically aligned during build
- ✅ No manual version updates needed
- ✅ Works for local development

**Option B: Package References with Version Properties**
```xml
<PackageReference Include="Datadog.MAUI.Android.Binding" Version="$(AndroidBindingVersion)" />
<PackageReference Include="Datadog.MAUI.iOS.Binding" Version="$(iOSBindingVersion)" />
```
- ✅ Explicit version control
- ❌ Requires properties defined in plugin's scope
- ❌ More complex

**Recommendation**: Keep ProjectReferences for development, let pack process generate correct PackageReferences.

---

## Release Scenarios

### Scenario 1: Binding-Only Android Fix

**Issue**: Android binding metadata needs fix, iOS is fine, plugin is fine

**Current Approach**:
```bash
# Must bump all packages
sed -i '' 's/3.5.0/3.5.1/' Directory.Build.props
# Result: Android 3.5.1, iOS 3.5.1, Plugin 3.5.1
# Problem: iOS and Plugin versions bumped unnecessarily
```

**With Independent Versions**:
```bash
# Edit only Android binding version
echo '<AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>' >> Datadog.MAUI.Android.Binding/Directory.Build.props
# Result: Android 3.5.0.1, iOS 3.5.0, Plugin 3.5.0
# Benefit: Precise versioning, no noise
```

### Scenario 2: Plugin API Enhancement

**Issue**: New plugin feature, bindings unchanged

**Current Approach**:
```bash
# Must bump all packages even though bindings unchanged
sed -i '' 's/3.5.0/3.6.0/' Directory.Build.props
```

**With Independent Versions**:
```bash
# Edit only plugin version
sed -i '' 's/<PluginVersion>3.5.0/3.6.0/' Directory.Build.props
# Result: Plugin 3.6.0, bindings stay 3.5.0
# Benefit: Clear signal - plugin changed, bindings didn't
```

### Scenario 3: Native SDK Update (Both Platforms)

**Issue**: Update to Android SDK 3.6.0 and iOS SDK 3.6.0

**Both approaches identical**:
```bash
# Update base version (current)
sed -i '' 's/3.5.0/3.6.0/' Directory.Build.props

# Or with independent versions
sed -i '' 's/DatadogSdkVersion>3.5.0/3.6.0/' Directory.Build.props
sed -i '' 's/AndroidBindingVersion>3.5.0/3.6.0/' Datadog.MAUI.Android.Binding/Directory.Build.props
sed -i '' 's/iOSBindingVersion>3.5.0/3.6.0/' Datadog.MAUI.iOS.Binding/Directory.Build.props
sed -i '' 's/PluginVersion>3.5.0/3.6.0/' Directory.Build.props
```

---

## Migration Path Recommendation

### Phase 1: Preparation (Current Sprint) ✅

**Add conditional properties** to root Directory.Build.props:

```xml
<PropertyGroup>
  <DatadogSdkVersion>3.5.0</DatadogSdkVersion>

  <!-- Future: Allow binding-specific overrides -->
  <AndroidBindingVersion Condition="'$(AndroidBindingVersion)' == ''">$(DatadogSdkVersion)</AndroidBindingVersion>
  <iOSBindingVersion Condition="'$(iOSBindingVersion)' == ''">$(DatadogSdkVersion)</iOSBindingVersion>
  <PluginVersion Condition="'$(PluginVersion)' == ''">$(DatadogSdkVersion)</PluginVersion>

  <Version>$(PluginVersion)</Version>
</PropertyGroup>
```

**Result**: No behavior change, but properties exist for future use.

**Documentation**: Update VERSION_MANAGEMENT.md to explain the properties and migration path.

### Phase 2: First Divergence (When Needed)

**Trigger**: First time you need Android 3.5.0.1 while iOS stays 3.5.0

**Action**: Create nested Directory.Build.props:

```bash
# Create Android-specific props
cat > Datadog.MAUI.Android.Binding/Directory.Build.props <<EOF
<Project>
  <Import Project="\$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '\$(MSBuildThisFileDirectory)../'))" />

  <PropertyGroup>
    <AndroidBindingVersion>3.5.0.1</AndroidBindingVersion>
    <Version>\$(AndroidBindingVersion)</Version>
  </PropertyGroup>
</Project>
EOF
```

**Validation Update**: Enhance validate-version.yml to extract all three versions.

### Phase 3: Full Independence (If Beneficial)

**When**: After 3-6 months if independent versioning proves valuable

**Action**:
1. Add iOS nested Directory.Build.props
2. Update all workflows to handle three version properties
3. Update release notes templates
4. Update VERSIONING.md

---

## Comparison to Microsoft's Xamarin.AndroidX

### How Microsoft Does It

**Package**: `Xamarin.AndroidX.Navigation.UI`
- **Version**: 2.9.6.1
- **Binds**: androidx.navigation:navigation-ui:2.9.6

**Other packages in same family**:
- `Xamarin.AndroidX.Navigation.Fragment`: 2.9.6.3
- `Xamarin.AndroidX.Navigation.Runtime`: 2.9.6.1
- `Xamarin.AndroidX.Core`: 1.10.1.4

**Key observations**:
1. Each binding package has **independent 4-part versioning**
2. First 3 parts match native library version
3. 4th part is binding-specific revision
4. Packages don't move in lockstep
5. No "meta" package - users reference individual packages

### Our Approach Comparison

| Aspect | Microsoft AndroidX | Our Current | Our Proposed |
|--------|-------------------|-------------|--------------|
| Binding versions | Independent | Lockstep | Independent (future) |
| Format | 4-part (2.9.6.1) | 3-part (3.5.0) | 4-part (3.5.0.1) |
| Meta-package | None | Yes (Plugin) | Yes (Plugin, 3-part) |
| Binding revision | 4th digit | N/A | 4th digit |
| Version files | Per-package | Single | Per-platform (future) |

**Recommendation**: Adopt 4-part versioning for bindings when we implement independent versioning, keeping 3-part SemVer for the plugin.

---

## Decision Matrix

### Should You Implement Independent Versioning Now?

| Factor | Current State | Threshold to Switch |
|--------|---------------|-------------------|
| Public users | None (pre-release) | >100 users or first GA release |
| Binding patches | 0 releases | >2 binding-only patches per quarter |
| Platform divergence | Rare | Happens monthly |
| Team bandwidth | Limited | Dedicated DevEx resources |
| Complexity tolerance | Prefer simple | Team comfortable with 3 versions |

**Current Recommendation**: **Implement Phase 1 (conditional properties) but don't create nested files yet.**

**Revisit Decision**: After first GA release or when first binding-only patch is needed.

---

## Action Items

### Immediate (This Sprint)
- [ ] Add conditional version properties to root Directory.Build.props
- [ ] Document properties in VERSION_MANAGEMENT.md
- [ ] No workflow changes needed yet

### When First Divergence Needed
- [ ] Create nested Directory.Build.props in affected platform directory
- [ ] Update validate-version.yml to check all three version properties
- [ ] Update release notes template to show all versions
- [ ] Add version combination testing to CI

### Future Consideration
- [ ] After 3-6 months: Evaluate if independent versioning is beneficial
- [ ] If yes: Add nested props for all platforms
- [ ] If no: Remove conditional properties and keep simple single version

---

## Questions for Team Discussion

1. **Do we expect frequent binding-only patches?** If yes, independent versioning is valuable.
2. **How often do Android/iOS SDK versions diverge?** If often, independent versioning is necessary.
3. **What's our tolerance for version management complexity?** Simple single version vs. flexible independent versions.
4. **When is our first GA release?** Might want structure ready before going public.
5. **Do users care about binding version granularity?** Or do they just track plugin version?

---

## Recommendation Summary

**For v3.5.0 (Pre-Release)**:
- ✅ Implement Phase 1: Add conditional properties
- ❌ Don't create nested Directory.Build.props yet
- ✅ Document migration path
- ✅ Keep current single-version simplicity

**Migrate to independent versions when**:
- First binding-only patch release needed (e.g., Android 3.5.0 → 3.5.0.1)
- Platform versions diverge (e.g., Android 3.6.0, iOS 3.6.1)
- Users request more granular version control
- Team is comfortable with added complexity

**Long-term vision**: Follow Microsoft's Xamarin.AndroidX pattern with fully independent binding package versions and 3-part plugin version.
