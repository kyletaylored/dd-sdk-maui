# NuGet Package Dependencies Guide

This document ensures all dependencies are properly included in the Datadog.MAUI NuGet packages.

## Overview

The Datadog MAUI SDK uses a multi-layered package structure with three levels:

1. **Module Binding Packages** - Individual platform-specific bindings
2. **Platform Meta-Packages** - Convenience packages that bundle all modules
3. **Consumer Plugin Package** - The main `Datadog.MAUI` package

## Dependency Chain

```
Datadog.MAUI (Consumer)
├── Datadog.MAUI.iOS.Binding (Meta)
│   ├── Datadog.MAUI.iOS.Internal
│   ├── Datadog.MAUI.iOS.Core
│   ├── Datadog.MAUI.iOS.RUM
│   ├── Datadog.MAUI.iOS.Logs
│   ├── Datadog.MAUI.iOS.Trace
│   ├── Datadog.MAUI.iOS.CrashReporting
│   ├── Datadog.MAUI.iOS.SessionReplay
│   ├── Datadog.MAUI.iOS.WebViewTracking
│   ├── Datadog.MAUI.iOS.Flags
│   └── Datadog.MAUI.iOS.OpenTelemetryApi
│
└── Datadog.MAUI.Android.Binding (Meta)
    ├── Datadog.MAUI.Android.Internal
    ├── Datadog.MAUI.Android.Core
    ├── Datadog.MAUI.Android.RUM
    ├── Datadog.MAUI.Android.Logs
    ├── Datadog.MAUI.Android.Trace
    ├── Datadog.MAUI.Android.NDK
    ├── Datadog.MAUI.Android.SessionReplay
    ├── Datadog.MAUI.Android.WebView
    ├── Datadog.MAUI.Android.Flags
    ├── Datadog.MAUI.Android.OkHttp
    └── Datadog.MAUI.Android.Trace.OpenTelemetry
```

## How Dependencies Are Included

### 1. Module Binding Packages

Each individual binding package (e.g., `Datadog.MAUI.iOS.Core`) declares its own dependencies:

- **ProjectReferences**: Reference other binding projects in the same solution
  - Automatically converted to PackageReferences in the NuGet package
  - Ensures proper dependency ordering

- **NativeReferences**: Reference native frameworks (iOS) or Android libraries
  - Embedded in the binding assemblies
  - Not declared as NuGet dependencies (part of the binding)

- **PackageReferences**: Reference .NET packages
  - Automatically included in the NuGet package dependencies
  - Examples: Xamarin.AndroidX, GoogleGson, etc.

**Key Files:**
- `Datadog.MAUI.iOS.Binding/DatadogCore/DatadogCore.csproj` - iOS Core binding
- `Datadog.MAUI.Android.Binding/dd-sdk-android-core/dd-sdk-android-core.csproj` - Android Core binding

### 2. Platform Meta-Packages

The meta-packages (`Datadog.MAUI.iOS.Binding` and `Datadog.MAUI.Android.Binding`) bundle all module packages:

```xml
<ItemGroup>
  <PackageReference Include="Datadog.MAUI.iOS.Internal" Version="$(DatadogSdkVersion)" />
  <PackageReference Include="Datadog.MAUI.iOS.Core" Version="$(DatadogSdkVersion)" />
  <!-- ... more iOS packages ... -->
</ItemGroup>
```

**Key Properties:**
- `IncludeBuildOutput>false</IncludeBuildOutput>` - Prevents inclusion of assembly files
- `IsPackable>true</IsPackable>` - Allows packing as NuGet package
- `ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>` - Allows explicit version control

**Key Files:**
- `Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj`
- `Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj`

### 3. Consumer Plugin Package

The main `Datadog.MAUI` package references the platform meta-packages:

```xml
<!-- iOS Binding dependencies -->
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="Datadog.MAUI.iOS.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>

<!-- Android Binding dependencies -->
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="Datadog.MAUI.Android.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>
```

**Key Files:**
- `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj` - Main plugin with PackageReferences
- `Package.nuspec` - Legacy spec file (can be generated from csproj)

## Packaging Process

The `scripts/pack.sh` script ensures proper dependency handling:

### Step A: Pack Module Binding Packages
1. Packs all individual binding modules
2. Each module's dependencies are resolved from its csproj file
3. Packages are created in the output directory

### Step B: Pack Platform Meta-Packages
1. Uses `--source "$OUTPUT_DIR"` to find module packages from Step A
2. Meta-packages declare PackageReferences to all modules
3. Dependencies are correctly linked in the .nupkg

### Step C: Pack Consumer Plugin Package
1. Uses `--source "$OUTPUT_DIR"` to find meta-packages from Step B
2. Consumer package declares PackageReferences to platform meta-packages
3. Final package includes complete dependency chain

## Ensuring All Dependencies Are Included

### For Binding Packages

1. **Check ProjectReferences** - All project-to-project dependencies must be listed
2. **Check PackageReferences** - All NuGet dependencies must be declared
3. **Verify NativeReferences** - Native frameworks are embedded, not as dependencies

Example:
```xml
<ItemGroup>
  <!-- Project reference: will become package dependency -->
  <ProjectReference Include="../DatadogInternal/DatadogInternal.csproj" />
  
  <!-- Package reference: will become package dependency -->
  <PackageReference Include="Xamarin.AndroidX.Collection" />
  
  <!-- Native reference: embedded, not a package dependency -->
  <NativeReference Include="$(XCFrameworkPath)" />
</ItemGroup>
```

### For Meta-Packages

1. Ensure ALL module packages are declared as PackageReferences
2. Use consistent version numbers via `$(DatadogSdkVersion)` property
3. Avoid other dependencies (meta-packages should be pure aggregators)

Example:
```xml
<PropertyGroup>
  <DatadogSdkVersion Condition="'$(DatadogSdkVersion)' == ''">3.5.0</DatadogSdkVersion>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Datadog.MAUI.iOS.Internal" Version="$(DatadogSdkVersion)" />
  <PackageReference Include="Datadog.MAUI.iOS.Core" Version="$(DatadogSdkVersion)" />
  <!-- ... more iOS packages ... -->
</ItemGroup>
```

### For Consumer Plugin Package

1. Declare PackageReferences for platform meta-packages (platform-conditional)
2. Declare PackageReferences for shared dependencies (Microsoft.Maui.Controls)
3. Mark ProjectReferences as `PrivateAssets="All"` to prevent them from becoming dependencies

Example:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Maui.Controls" Version="9.0.90" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="Datadog.MAUI.iOS.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="Datadog.MAUI.Android.Binding" Version="$(DatadogSdkVersion)" />
</ItemGroup>

<!-- ProjectReferences marked as PrivateAssets for local builds only -->
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <ProjectReference Include="../Datadog.MAUI.Android.Binding/dd-sdk-android-core/dd-sdk-android-core.csproj" PrivateAssets="All" />
</ItemGroup>
```

## Verification Checklist

Before publishing NuGet packages, verify:

1. **Module Packages**
   - [ ] Each module declares all its ProjectReferences
   - [ ] Each module declares all its PackageReferences
   - [ ] No circular dependencies exist
   - [ ] `dotnet pack` produces valid .nupkg files

2. **Meta-Packages**
   - [ ] All module packages are declared as PackageReferences
   - [ ] Version numbers are consistent
   - [ ] `IncludeBuildOutput` is set to `false`
   - [ ] `ManagePackageVersionsCentrally` is set to `false` (for explicit versions)

3. **Consumer Plugin Package**
   - [ ] Platform meta-packages are declared with condition attributes
   - [ ] Microsoft.Maui.Controls dependency is included
   - [ ] ProjectReferences have `PrivateAssets="All"`
   - [ ] All target frameworks are supported

4. **Package Contents**
   - [ ] Use `nuget list-package-contents <package.nupkg>` to verify contents
   - [ ] Verify .nuspec files have correct dependency declarations
   - [ ] No missing transitive dependencies

## Common Issues and Solutions

### Issue: "NuGet package dependency not found"
**Solution**: 
- Ensure the dependency module package is published before the dependent package
- Use `--source` flag in pack.sh to point to local packages
- Check pack.sh Step B and Step C for proper ordering

### Issue: "Duplicate assembly in package"
**Solution**:
- Mark ProjectReferences as `PrivateAssets="All"` in consumer packages
- Ensure ProjectReferences are only used during build, not packaged
- Use PackageReferences instead of ProjectReferences in final packages

### Issue: "Missing transitive dependency"
**Solution**:
- Check all binding packages declare their dependencies
- Verify meta-packages reference all module packages
- Use `dotnet list package --outdated` to verify dependency tree

## References

- **Build/Pack Scripts**: `scripts/build.sh`, `scripts/pack.sh`
- **Project Files**: 
  - Plugin: `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj`
  - iOS Meta: `Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj`
  - Android Meta: `Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj`
  - Binding Modules: Individual csproj files in binding directories
- **Configuration**: `Directory.Build.props` (version management)
- **Legacy Spec**: `Package.nuspec` (can be auto-generated from csproj)
