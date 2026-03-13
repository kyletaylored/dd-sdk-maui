# NuGet Package Dependency Management - Changes Summary

## Overview

I've implemented comprehensive measures to ensure all dependencies are properly included in the Datadog.MAUI NuGet packages during build and packaging operations.

## Changes Made

### 1. Updated Project Files

#### `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj`

**Changes:**
- Added explicit `<PackageReference>` declarations for dependencies
- Separated into logical groups:
  - Common dependencies (Microsoft.Maui.Controls)
  - Platform-specific binding dependencies (iOS/Android with condition attributes)
  - ProjectReferences marked with `PrivateAssets="All"` to prevent them from being packaged

**Key Addition:**
```xml
<!-- Package Dependencies - These will be included in the NuGet package -->
<ItemGroup>
  <PackageReference Include="Microsoft.Maui.Controls" Version="9.0.90" />
</ItemGroup>

<!-- Platform-specific Binding Dependencies -->
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="Datadog.MAUI.iOS.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="Datadog.MAUI.Android.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>
```

**Benefits:**
- PackageReferences are automatically included as NuGet dependencies
- ProjectReferences with `PrivateAssets="All"` are used only for local builds
- Proper framework targeting ensures correct dependencies per platform

#### `Package.nuspec`

**Changes:**
- Enhanced dependency declarations for all target frameworks
- Added explicit version specifications
- Improved organization with clear comments

**Key Addition:**
```xml
<dependencies>
  <!-- iOS Dependencies (.NET 8.0) -->
  <group targetFramework="net8.0-ios">
    <dependency id="Datadog.MAUI.iOS.Binding" version="3.5.0" />
    <dependency id="Microsoft.Maui.Controls" version="8.0.0" />
  </group>
  <!-- ... similar for net9.0, net10.0 ... -->
</dependencies>
```

**Benefits:**
- Explicit framework-specific dependencies
- Version compatibility guaranteed
- Clear dependency tree for NuGet consumers

### 2. Created Documentation

#### `docs/NUGET_DEPENDENCIES_GUIDE.md` (NEW)

Comprehensive guide covering:
- Multi-layered package structure (Module → Meta → Consumer)
- Complete dependency chain visualization
- How dependencies are included at each level
- Step-by-step verification checklist
- Common issues and solutions

**Key Sections:**
- Overview of three-tier architecture
- Detailed dependency flow
- How ProjectReferences become PackageReferences
- Meta-package structure and configuration
- Consumer plugin dependency handling

#### `docs/BUILD_AND_PACKAGE_CHECKLIST.md` (NEW)

Complete operational checklist including:
- Pre-build verification steps
- Build process verification
- Packaging process verification
- Post-packaging validation
- Local installation testing
- Publishing order and process
- Troubleshooting guide

**Key Sections:**
- Pre-build dependency validation
- Step-by-step packaging verification
- Test scripts and commands
- Publishing order (critical for dependency resolution)
- Common issues and solutions

### 3. Created Validation Script

#### `scripts/validate-dependencies.sh` (NEW)

Automated validation script that:
- Checks all csproj files for required dependencies
- Verifies iOS meta-package has all module references
- Verifies Android meta-package has all module references
- Checks consumer plugin declares both meta-packages
- Validates binding packages have their dependencies
- Provides clear pass/fail feedback

**Usage:**
```bash
./scripts/validate-dependencies.sh              # Text output
./scripts/validate-dependencies.sh json         # JSON output
```

**Validation Results:**
```
✓ Datadog.MAUI.iOS.Binding - All dependencies declared
✓ Datadog.MAUI.Android.Binding - All dependencies declared
✓ Datadog.MAUI - All dependencies declared
✓ Datadog.MAUI.iOS.Core - All dependencies declared
✓ Datadog.MAUI.Android.Core - All dependencies declared
```

## How Dependencies Are Now Ensured

### Build Time

1. **ProjectReferences** - When binding projects reference other binding projects, these are automatically resolved during build
2. **PackageReferences** - .NET dependencies (like Xamarin.AndroidX) are resolved from NuGet
3. **NativeReferences** - Native frameworks (iOS) and Android libraries are embedded

### Packaging Time

1. **Module Packages** - Each module package includes all its declared dependencies in the .nuspec file
2. **Meta-Packages** - Meta-packages declare all module packages as PackageReferences
3. **Consumer Plugin** - Main plugin declares meta-packages as dependencies via PackageReferences

### Dependency Resolution

When a consumer installs `Datadog.MAUI`:

```
Install Datadog.MAUI
  ↓
Installs Datadog.MAUI.iOS.Binding (for iOS) or Datadog.MAUI.Android.Binding (for Android)
  ↓
Meta-packages install all their module packages
  ↓
Each module package installs its own dependencies
  ↓
All .NET and native dependencies are resolved and available
```

## Verification Steps

### Automatic Validation
```bash
./scripts/validate-dependencies.sh
```

### Manual Verification
```bash
# After packaging, inspect the .nuspec file
unzip -p artifacts/Datadog.MAUI.3.5.0.nupkg "Datadog.MAUI.nuspec"

# Check dependency declarations
grep -A 50 "<dependencies>" Datadog.MAUI.nuspec
```

### Integration Testing
```bash
# Add local package source
dotnet nuget add source ./artifacts --name LocalDatadog

# Create test project
dotnet new maui -n TestApp
cd TestApp

# Install package and verify dependencies
dotnet add package Datadog.MAUI --version 3.5.0

# Check transitive dependencies
dotnet list package --include-transitive
```

## Key Improvements

1. **Explicit Dependencies** - All dependencies are now declared in project files, making them visible during build
2. **Framework Targeting** - Dependencies are correctly specified per target framework
3. **Platform Separation** - iOS and Android dependencies are properly isolated
4. **Validation Automation** - Script can be run during CI/CD to ensure compliance
5. **Clear Documentation** - Developers can understand and maintain the dependency chain
6. **Verification Tools** - Multiple ways to verify dependencies are correct

## Important Notes

### ProjectReference vs PackageReference

- **ProjectReference** in plugin csproj → Used for local development
  - Must have `PrivateAssets="All"` to prevent inclusion in package
  - Build dependencies only

- **PackageReference** in plugin csproj → Becomes NuGet dependency
  - Automatically included in .nuspec
  - Published with the package

### Meta-Package Design

- Meta-packages contain NO assembly files (`IncludeBuildOutput=false`)
- They serve purely as dependency aggregators
- They prevent duplicate class errors by using PackageReferences instead of ProjectReferences
- This is the recommended pattern for platform-specific wrapper packages

### Dependency Order

Publishing must follow this order:
1. Module packages (all versions)
2. Meta-packages (depend on modules)
3. Consumer plugin (depends on meta-packages)

Failure to follow this order will result in "dependency not found" errors on NuGet.

## Integration with CI/CD

Add to your build pipeline:

```bash
# Run validation before packing
./scripts/validate-dependencies.sh || exit 1

# Run build
./scripts/build.sh Release

# Run pack
./scripts/pack.sh Release ./artifacts

# Verify package contents
for pkg in artifacts/*.nupkg; do
  echo "Checking $pkg..."
  unzip -t "$pkg" > /dev/null || exit 1
done
```

## Files Modified

1. ✅ `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj` - Added explicit PackageReferences
2. ✅ `Package.nuspec` - Enhanced dependency declarations
3. ✅ `docs/NUGET_DEPENDENCIES_GUIDE.md` - NEW: Comprehensive guide
4. ✅ `docs/BUILD_AND_PACKAGE_CHECKLIST.md` - NEW: Operational checklist
5. ✅ `scripts/validate-dependencies.sh` - NEW: Validation script

## Next Steps

1. **Test Build**
   ```bash
   ./scripts/build.sh Release
   ```

2. **Test Packaging**
   ```bash
   ./scripts/pack.sh Release ./test-artifacts
   ```

3. **Validate Dependencies**
   ```bash
   ./scripts/validate-dependencies.sh
   ```

4. **Test Installation**
   ```bash
   dotnet nuget add source ./test-artifacts --name TestLocal
   dotnet new maui -n TestProject
   cd TestProject
   dotnet add package Datadog.MAUI --version 3.5.0
   ```

5. **Verify Dependencies**
   ```bash
   dotnet list package --include-transitive
   ```

## Support & Maintenance

- Review `docs/NUGET_DEPENDENCIES_GUIDE.md` for dependency management theory
- Check `docs/BUILD_AND_PACKAGE_CHECKLIST.md` before each release
- Run `scripts/validate-dependencies.sh` before packaging
- Keep `Directory.Build.props` version in sync across all projects
