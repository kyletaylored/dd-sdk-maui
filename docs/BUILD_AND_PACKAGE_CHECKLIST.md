# NuGet Package Building & Dependency Verification Checklist

This document outlines the complete process for building and packaging the Datadog.MAUI NuGet packages with full dependency verification.

## Pre-Build Checklist

- [ ] **Version Consistency**
  - [ ] Update `Directory.Build.props` with new version number in `<DatadogSdkVersion>`
  - [ ] Verify all binding csproj files use `$(DatadogSdkVersion)` for internal dependencies
  - [ ] Verify meta-package csproj files use consistent version references

- [ ] **Dependency Declaration**
  - [ ] Run validation script: `./scripts/validate-dependencies.sh`
  - [ ] All checks pass with no failures
  - [ ] Review any warnings in output

- [ ] **Project Structure**
  - [ ] All csproj files are well-formed XML
  - [ ] No circular ProjectReferences exist
  - [ ] All referenced projects exist in the repository

## Build Process

### Step 1: Clean Previous Artifacts
```bash
./scripts/build.sh Release
```

**Verification:**
- [ ] Build completes without errors
- [ ] No warnings about missing dependencies
- [ ] All target frameworks compile successfully

### Step 2: Verify Module Packages Have Dependencies

For each Android binding module:
```bash
cd Datadog.MAUI.Android.Binding/dd-sdk-android-<module>
grep -A 10 "<ItemGroup>" dd-sdk-android-<module>.csproj | grep -E "ProjectReference|PackageReference"
```

**Check:**
- [ ] ProjectReferences to other modules are present
- [ ] PackageReferences for .NET dependencies are present
- [ ] Version numbers use `$(DatadogSdkVersion)` where appropriate

For each iOS binding module:
```bash
cd Datadog.MAUI.iOS.Binding/<Module>
grep -A 10 "<ItemGroup>" <Module>.csproj | grep -E "ProjectReference|PackageReference"
```

**Check:**
- [ ] ProjectReferences to dependent modules are present (e.g., DatadogCore → DatadogInternal)
- [ ] Native frameworks are referenced, not as package dependencies

### Step 3: Verify Meta-Package Declarations

iOS Meta-Package:
```bash
grep "<PackageReference Include=\"Datadog.MAUI.iOS" Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj
```

**Should show:**
```
<PackageReference Include="Datadog.MAUI.iOS.Internal" ...
<PackageReference Include="Datadog.MAUI.iOS.Core" ...
<PackageReference Include="Datadog.MAUI.iOS.RUM" ...
<PackageReference Include="Datadog.MAUI.iOS.Logs" ...
<PackageReference Include="Datadog.MAUI.iOS.Trace" ...
<PackageReference Include="Datadog.MAUI.iOS.CrashReporting" ...
<PackageReference Include="Datadog.MAUI.iOS.SessionReplay" ...
<PackageReference Include="Datadog.MAUI.iOS.WebViewTracking" ...
<PackageReference Include="Datadog.MAUI.iOS.Flags" ...
<PackageReference Include="Datadog.MAUI.iOS.OpenTelemetryApi" ...
```

Android Meta-Package:
```bash
grep "<PackageReference Include=\"Datadog.MAUI.Android" Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj
```

**Should show:**
```
<PackageReference Include="Datadog.MAUI.Android.Internal" ...
<PackageReference Include="Datadog.MAUI.Android.Core" ...
<PackageReference Include="Datadog.MAUI.Android.RUM" ...
<PackageReference Include="Datadog.MAUI.Android.Logs" ...
<PackageReference Include="Datadog.MAUI.Android.Trace" ...
<PackageReference Include="Datadog.MAUI.Android.NDK" ...
<PackageReference Include="Datadog.MAUI.Android.SessionReplay" ...
<PackageReference Include="Datadog.MAUI.Android.WebView" ...
<PackageReference Include="Datadog.MAUI.Android.Flags" ...
<PackageReference Include="Datadog.MAUI.Android.OkHttp" ...
<PackageReference Include="Datadog.MAUI.Android.Trace.OpenTelemetry" ...
```

### Step 4: Verify Consumer Plugin Dependencies

```bash
grep -A 5 "TargetFramework.Contains('-ios')" Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj | grep PackageReference
grep -A 5 "TargetFramework.Contains('-android')" Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj | grep PackageReference
grep "Microsoft.Maui.Controls" Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj
```

**Should show:**
- [ ] `Datadog.MAUI.iOS.Binding` reference for iOS targets
- [ ] `Datadog.MAUI.Android.Binding` reference for Android targets
- [ ] `Microsoft.Maui.Controls` reference for all targets
- [ ] ProjectReferences marked with `PrivateAssets="All"`

## Packaging Process

### Step 1: Pack Module Bindings

Run the packing script:
```bash
./scripts/pack.sh Release ./artifacts
```

**Verification:**
- [ ] All module packages created successfully
- [ ] Android modules packed: Internal, Core, RUM, Logs, Trace, NDK, SessionReplay, WebView, Flags, OkHttp, Trace.OpenTelemetry
- [ ] iOS modules packed: Internal, Core, RUM, Logs, Trace, CrashReporting, SessionReplay, WebViewTracking, Flags, OpenTelemetryApi

### Step 2: Inspect Module Package Dependencies

For each created nupkg file:
```bash
# List package contents
unzip -p artifacts/Datadog.MAUI.Android.Core.3.5.0.nupkg "[Content_Types].xml" | xmllint --xpath '//Relationship[@Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata"]/@Target' -

# Or using nuget tools
dotnet package-diagnostics verify ./artifacts/Datadog.MAUI.Android.Core.3.5.0.nupkg
```

**Check:**
- [ ] Each package's .nuspec file declares all dependencies
- [ ] Dependency versions are correct
- [ ] No circular dependencies detected

### Step 3: Verify Meta-Package Creation

```bash
unzip -p artifacts/Datadog.MAUI.iOS.Binding.3.5.0.nupkg "Datadog.MAUI.iOS.Binding.nuspec" | grep -A 50 "<dependencies>"
unzip -p artifacts/Datadog.MAUI.Android.Binding.3.5.0.nupkg "Datadog.MAUI.Android.Binding.nuspec" | grep -A 50 "<dependencies>"
```

**Check:**
- [ ] iOS meta-package lists all 10 iOS module packages as dependencies
- [ ] Android meta-package lists all 11 Android module packages as dependencies
- [ ] Versions match the `$(DatadogSdkVersion)` property (3.5.0 or current version)
- [ ] No other dependencies are declared (should be pure aggregators)

### Step 4: Verify Consumer Plugin Package

```bash
unzip -p artifacts/Datadog.MAUI.3.5.0.nupkg "Datadog.MAUI.nuspec" | grep -A 50 "<dependencies>"
```

**Check:**
- [ ] Declares `Datadog.MAUI.iOS.Binding` for iOS targets
- [ ] Declares `Datadog.MAUI.Android.Binding` for Android targets
- [ ] Declares `Microsoft.Maui.Controls` for all targets
- [ ] Versions are correct
- [ ] No transitive dependencies listed (those come from meta-packages)

## Post-Packaging Verification

### Test Local Installation

1. Add local NuGet source:
```bash
dotnet nuget add source ./artifacts --name LocalDatadog
```

2. Create test project:
```bash
dotnet new maui -n TestApp
cd TestApp
dotnet add package Datadog.MAUI --version 3.5.0 --source LocalDatadog
```

**Check:**
- [ ] Package restores successfully
- [ ] All transitive dependencies resolve
- [ ] No NuGet errors or warnings about missing dependencies
- [ ] All platform-specific bindings are available

3. Verify dependency chain:
```bash
dotnet list package --include-transitive
```

**Should show:**
- [ ] Datadog.MAUI depends on Datadog.MAUI.iOS.Binding (on iOS)
- [ ] Datadog.MAUI depends on Datadog.MAUI.Android.Binding (on Android)
- [ ] Meta-packages depend on all their module packages
- [ ] No missing or unresolved dependencies

### Inspect Package Contents

```bash
# List all files in package
unzip -l artifacts/Datadog.MAUI.3.5.0.nupkg

# Check for required .nuspec declarations
unzip -p artifacts/Datadog.MAUI.3.5.0.nupkg | grep -A 20 "<dependencies>"
```

**Check:**
- [ ] Package contains plugin DLL for all target frameworks
- [ ] No unexpected files or assemblies
- [ ] .nuspec file is well-formed and complete
- [ ] All required metadata is present

## Pre-Publishing Checklist

Before publishing to nuget.org:

- [ ] All validation checks pass
- [ ] Local installation tests succeed
- [ ] Dependency chain is complete
- [ ] No unresolved transitive dependencies
- [ ] Version numbers are consistent across all packages
- [ ] Release notes are updated in `Package.nuspec`
- [ ] README.md is referenced in package metadata
- [ ] License information is correct

## Publishing Order

When publishing to NuGet, follow this order:

1. **Module Packages** (all versions of each module)
   ```bash
   dotnet nuget push artifacts/Datadog.MAUI.Android.Internal*.nupkg -k $API_KEY -s https://api.nuget.org/v3/index.json
   dotnet nuget push artifacts/Datadog.MAUI.Android.Core*.nupkg -k $API_KEY -s https://api.nuget.org/v3/index.json
   # ... continue for all modules
   ```

2. **Meta-Packages** (only after all modules are published)
   ```bash
   dotnet nuget push artifacts/Datadog.MAUI.Android.Binding*.nupkg -k $API_KEY -s https://api.nuget.org/v3/index.json
   dotnet nuget push artifacts/Datadog.MAUI.iOS.Binding*.nupkg -k $API_KEY -s https://api.nuget.org/v3/index.json
   ```

3. **Consumer Plugin** (only after all dependencies are published)
   ```bash
   dotnet nuget push artifacts/Datadog.MAUI*.nupkg -k $API_KEY -s https://api.nuget.org/v3/index.json
   ```

## Troubleshooting

### Issue: "Missing dependency" error during restore

**Solution:**
1. Verify all module packages are published
2. Check version numbers match exactly
3. Run `./scripts/validate-dependencies.sh` to identify issues
4. Ensure ProjectReferences in csproj files reference correct project paths

### Issue: "Duplicate assembly" error

**Solution:**
1. Verify ProjectReferences in consumer packages have `PrivateAssets="All"`
2. Ensure meta-packages don't include assembly output (`IncludeBuildOutput=false`)
3. Check that module packages don't have other modules' assemblies

### Issue: "Circular dependency detected"

**Solution:**
1. Check ProjectReferences for cycles
2. Use `dotnet list package --include-transitive` to identify the cycle
3. Consider restructuring packages to break the cycle
4. Move circular dependencies to shared package if possible

## References

- **Validation Script**: `scripts/validate-dependencies.sh`
- **Build Script**: `scripts/build.sh`
- **Pack Script**: `scripts/pack.sh`
- **Dependency Guide**: `docs/NUGET_DEPENDENCIES_GUIDE.md`
- **Configuration**: `Directory.Build.props`
- **Main Plugin**: `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj`
- **Package Spec**: `Package.nuspec`
