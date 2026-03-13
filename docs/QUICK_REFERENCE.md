# Quick Reference: NuGet Dependency Management

## Quick Commands

### Validate Dependencies
```bash
./scripts/validate-dependencies.sh
```

### Build Release
```bash
./scripts/build.sh Release
```

### Package All Projects
```bash
./scripts/pack.sh Release ./artifacts
```

### Test Local Installation
```bash
# Add local source
dotnet nuget add source ./artifacts --name LocalDD

# Create test project
dotnet new maui -n TestApp
cd TestApp

# Install package
dotnet add package Datadog.MAUI

# View dependencies
dotnet list package --include-transitive
```

## Dependency Hierarchy

```
Datadog.MAUI (consumer package)
├── Datadog.MAUI.iOS.Binding (meta-package)
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
└── Datadog.MAUI.Android.Binding (meta-package)
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

## Publishing Order

**CRITICAL:** Publish in this order:

1. All module packages (Datadog.MAUI.iOS.*, Datadog.MAUI.Android.*)
2. Meta-packages (Datadog.MAUI.iOS.Binding, Datadog.MAUI.Android.Binding)
3. Consumer plugin (Datadog.MAUI)

Incorrect order will cause "dependency not found" errors.

## Key Configuration

### Version Management
- Edit: `Directory.Build.props`
- Property: `<DatadogSdkVersion>3.5.0</DatadogSdkVersion>`
- All projects reference this property

### Package Dependencies
- **Plugin**: `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj`
  - Uses `PackageReference` for package dependencies
  - Uses `ProjectReference` with `PrivateAssets="All"` for local builds

- **Meta-Packages**: `Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj`, `Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj`
  - Uses `PackageReference` for all module packages
  - No assembly output (`IncludeBuildOutput=false`)

- **Module Packages**: Individual binding csproj files
  - Use `ProjectReference` for inter-module dependencies
  - Use `PackageReference` for .NET dependencies
  - Declarations become part of the .nuspec

## Files to Check/Update

### For Dependency Changes
1. `Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj` - Plugin dependencies
2. `Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj` - iOS meta-package
3. `Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj` - Android meta-package
4. Individual binding csproj files - Module dependencies

### For Version Management
- `Directory.Build.props` - Master version property
- `Package.nuspec` - Legacy spec file (optional, can be auto-generated)

### For Verification
- `scripts/validate-dependencies.sh` - Run to verify all dependencies
- `docs/NUGET_DEPENDENCIES_GUIDE.md` - Complete reference
- `docs/BUILD_AND_PACKAGE_CHECKLIST.md` - Pre-release checklist

## Common Tasks

### Add a New Dependency to Plugin

1. **If it's for all platforms:**
   ```xml
   <ItemGroup>
     <PackageReference Include="NewPackage" Version="1.0.0" />
   </ItemGroup>
   ```

2. **If it's platform-specific (iOS):**
   ```xml
   <ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
     <PackageReference Include="NewPackage" Version="1.0.0" />
   </ItemGroup>
   ```

3. **If it's platform-specific (Android):**
   ```xml
   <ItemGroup Condition="$(TargetFramework.Contains('-android'))">
     <PackageReference Include="NewPackage" Version="1.0.0" />
   </ItemGroup>
   ```

### Add a New Module to Meta-Package

In `Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj` or `Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Datadog.MAUI.iOS.NewModule" Version="$(DatadogSdkVersion)" />
</ItemGroup>
```

Then run validation:
```bash
./scripts/validate-dependencies.sh
```

### Update Version Number

1. Edit `Directory.Build.props`:
   ```xml
   <DatadogSdkVersion>3.6.0</DatadogSdkVersion>
   ```

2. Rebuild all projects:
   ```bash
   ./scripts/build.sh Release
   ```

3. Validate:
   ```bash
   ./scripts/validate-dependencies.sh
   ```

4. Package:
   ```bash
   ./scripts/pack.sh Release ./artifacts
   ```

## Troubleshooting

### Validation Fails
```bash
./scripts/validate-dependencies.sh
# Review output - missing dependencies are listed
# Add missing PackageReference to appropriate csproj
```

### Build Fails
```bash
./scripts/build.sh Release
# Check for ProjectReference or PackageReference issues
# Ensure all referenced projects exist
```

### Package Won't Install
```bash
# Check package contents
unzip -p artifacts/Datadog.MAUI.3.5.0.nupkg "Datadog.MAUI.nuspec"

# Verify dependencies are declared
# Ensure all dependency packages are published first
```

### Circular Dependencies
```bash
dotnet list package --include-transitive
# Look for cycles in the output
# Restructure packages to eliminate cycles
```

## Documentation

- **Full Guide**: `docs/NUGET_DEPENDENCIES_GUIDE.md`
- **Checklist**: `docs/BUILD_AND_PACKAGE_CHECKLIST.md`
- **Changes**: `docs/DEPENDENCY_CHANGES_SUMMARY.md`
- **This Guide**: `docs/QUICK_REFERENCE.md`

## Support

For detailed information:
1. Review the relevant documentation file
2. Run validation script with `-h` for help
3. Check script comments for additional options
4. Review build logs for specific errors
