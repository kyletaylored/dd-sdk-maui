---
layout: default
title: Scripts Overview
nav_order: 12
---

# Build Scripts Overview

Comprehensive guide to all build and automation scripts for the Datadog MAUI SDK.

---

## Script Index

| Script | Purpose | When to Use |
|--------|---------|-------------|
| **pack.sh** | Build and pack all NuGet packages | Release builds, local testing |
| **map-maven-to-nuget.sh** | Map Maven coordinates to NuGet packages | Adding new Android dependencies |
| **validate-android-artifacts.sh** | Validate Android SDK artifacts | Security validation, verification |
| **check-nuget-versions.sh** | Check for available NuGet package updates | Version maintenance |
| **generate-android-dependencies.sh** | Extract Maven dependencies from POMs | Initial package setup |
| **setup-android-bindings.sh** | Setup new Android binding projects | Creating new bindings |
| **download-android-aar.sh** | Download AAR files from Maven Central | Manual artifact management |
| **analyze-android-dependencies.sh** | Analyze Maven dependency tree | Dependency troubleshooting |
| **update-sdk-version.sh** | Update Datadog SDK version | Version upgrades |
| **verify-ios-xcframework.sh** | Verify iOS XCFramework checksums | iOS artifact validation |

---

## Core Scripts

### pack.sh

**Purpose**: Master build and packaging script

**What it does**:
- Builds all Android and iOS binding packages in dependency order
- Packs them into NuGet packages (.nupkg)
- Places artifacts in `./artifacts/` directory
- Supports multi-framework targeting (net9.0/net10.0)

**Usage**:
```bash
./scripts/pack.sh
```

**Build Order**:
```
Android:
1. dd-sdk-android-internal
2. dd-sdk-android-core
3. Feature packages (logs, rum, trace, ndk, session-replay, webview, flags)
4. Integration packages (okhttp, trace-otel, okhttp-otel, gradle-plugin)
5. Meta-package (Datadog.MAUI.Android.Binding)

iOS:
1. Individual feature bindings
2. Meta-package (Datadog.MAUI.iOS.Binding)
```

**Tips**:
- Run from repository root
- Requires .NET 9/10 SDKs installed
- First build downloads Maven artifacts (takes longer)
- Subsequent builds use cached artifacts

---

### map-maven-to-nuget.sh

**Purpose**: Maps Maven package coordinates to NuGet package names and suggests compatible versions

**Why this exists**:
Maven POMs specify versions that may not support net9.0/net10.0. This script helps find compatible NuGet packages.

**Usage**:
```bash
# Basic mapping
./scripts/map-maven-to-nuget.sh "org.jetbrains.kotlin:kotlin-stdlib" "2.0.21"

# Output:
# Xamarin.Kotlin.StdLib|2.3.0.1|⚠️ Upgraded from 2.0.21 for net9.0/net10.0 support
```

**Common Mappings**:
```bash
# Kotlin
org.jetbrains.kotlin:kotlin-stdlib → Xamarin.Kotlin.StdLib

# AndroidX
androidx.core:core → Xamarin.AndroidX.Core
androidx.fragment:fragment → Xamarin.AndroidX.Fragment

# OkHttp
com.squareup.okhttp3:okhttp → Square.OkHttp3

# OpenTelemetry
io.opentelemetry:opentelemetry-api → OpenTelemetry.Api
```

**When to use**:
- Adding new Android binding packages
- Upgrading dependencies
- Resolving version compatibility issues
- Understanding why versions differ from Maven POMs

**See also**: [maven-nuget-version-mapping.md](../guides/android/ANDROID_DEPENDENCIES.html)

---

### validate-android-artifacts.sh

**Purpose**: Download and validate Datadog Android SDK artifacts

**What it does**:
- Downloads `verification-metadata.xml` from dd-sdk-android releases
- Lists all components and their SHA256 checksums
- Validates artifact integrity (planned feature)

**Usage**:
```bash
# Download metadata for specific version
./scripts/validate-android-artifacts.sh 3.5.0 --download-metadata

# Validate artifacts (future)
./scripts/validate-android-artifacts.sh 3.5.0 --validate
```

**When to use**:
- Security compliance requirements
- Verifying artifact authenticity
- Understanding available packages
- Supply chain security validation

---

## Utility Scripts

### check-nuget-versions.sh

**Purpose**: Check for newer NuGet package versions

**Usage**:
```bash
./scripts/check-nuget-versions.sh
```

**What it checks**:
- All packages in `Directory.Packages.props`
- Queries NuGet.org for latest stable versions
- Reports available updates

**Output format**:
```
Xamarin.Kotlin.StdLib: 2.3.0.1 → 2.4.0.1 (update available)
Xamarin.AndroidX.Core: 1.17.0.1 (up to date)
```

---

### generate-android-dependencies.sh

**Purpose**: Extract Maven dependencies from POM files

**Usage**:
```bash
./scripts/generate-android-dependencies.sh com.datadoghq:dd-sdk-android-rum:3.5.0
```

**What it does**:
- Downloads POM file from Maven Central
- Extracts dependencies
- Maps to NuGet package names
- Generates .csproj PackageReference entries

**When to use**:
- Creating new Android binding packages
- Understanding dependency requirements
- Initial package setup

---

### setup-android-bindings.sh

**Purpose**: Scaffold new Android binding project

**Usage**:
```bash
./scripts/setup-android-bindings.sh dd-sdk-android-example 3.5.0
```

**What it creates**:
- Directory structure
- .csproj file with AndroidMavenLibrary reference
- Transforms/Metadata.xml template
- Basic README

**When to use**:
- Adding new integration packages
- Creating custom bindings

---

## Maintenance Scripts

### update-sdk-version.sh

**Purpose**: Update Datadog SDK version across all projects

**Usage**:
```bash
./scripts/update-sdk-version.sh 3.6.0
```

**What it updates**:
- `Directory.Build.props` (DatadogSdkVersion)
- AndroidMavenLibrary references in all .csproj files
- Documentation

**When to use**:
- Upgrading to new Datadog SDK release
- Syncing all packages to same version

---

### verify-ios-xcframework.sh

**Purpose**: Verify iOS XCFramework checksums

**Usage**:
```bash
./scripts/verify-ios-xcframework.sh
```

**What it does**:
- Calculates SHA256 checksums of XCFramework files
- Compares against known good values
- Reports integrity status

**When to use**:
- Verifying manually downloaded frameworks
- Security validation
- After downloading iOS dependencies

---

## Common Workflows

### Adding a New Android Integration Package

```bash
# 1. Generate project structure
./scripts/setup-android-bindings.sh dd-sdk-android-newpackage 3.5.0

# 2. Find dependencies
./scripts/generate-android-dependencies.sh com.datadoghq:dd-sdk-android-newpackage:3.5.0

# 3. Map Maven dependencies to NuGet
./scripts/map-maven-to-nuget.sh "com.example:dependency" "1.0.0"

# 4. Update Directory.Packages.props with NuGet versions
# Edit Directory.Packages.props

# 5. Add to solution file
# Edit Datadog.MAUI.Android.Binding.sln

# 6. Add to pack.sh ANDROID_MODULES array
# Edit scripts/pack.sh

# 7. Build and test
./scripts/pack.sh
```

---

### Upgrading Datadog SDK Version

```bash
# 1. Update version
./scripts/update-sdk-version.sh 3.6.0

# 2. Download new verification metadata
./scripts/validate-android-artifacts.sh 3.6.0 --download-metadata

# 3. Check for dependency updates
./scripts/check-nuget-versions.sh

# 4. Update Directory.Packages.props if needed
# Edit Directory.Packages.props

# 5. Build all packages
./scripts/pack.sh

# 6. Test with sample app
cd samples/DatadogMauiSample
dotnet restore
dotnet build
```

---

## Architecture Notes

### Why AndroidMavenLibrary?

Your project uses `AndroidMavenLibrary` instead of manually downloading AARs:

**Advantages**:
- Automatic Maven artifact download
- Transitive dependency resolution
- Framework-specific version selection
- Cleaner project files

**How it works**:
```xml
<AndroidMavenLibrary Include="com.datadoghq:dd-sdk-android-rum" Version="3.5.0" />
```

At build time:
1. Downloads AAR from Maven Central
2. Extracts Java/Kotlin classes
3. Generates C# bindings
4. Downloads transitive dependencies
5. Resolves to NuGet package versions

---

### Central Package Management

All NuGet versions controlled in `Directory.Packages.props`:

```xml
<ItemGroup>
  <PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.3.0.1" />
  <PackageVersion Include="Xamarin.AndroidX.Core" Version="1.17.0.1" />
</ItemGroup>
```

**Why**:
- Single source of truth
- Consistent versions across all packages
- Easier to audit and update
- Framework compatibility control

---

### Version Override Strategy

Maven POMs often specify older versions that don't support net9.0/net10.0. Your override strategy:

1. **Check framework support**: Does NuGet package support net9.0/net10.0?
2. **Find compatible version**: Use `map-maven-to-nuget.sh` to find newer version
3. **Override in Directory.Packages.props**: Pin compatible version
4. **Document why**: Comment explains the override reason

**Example**:
```xml
<!-- Kotlin 2.0.21 from Maven doesn't support net10.0, use 2.3.0.1 -->
<PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.3.0.1" />
```

---

## Integration with CI/CD

Scripts are integrated into GitHub Actions workflows:

### build-android.yml
- Calls `dotnet pack` (invokes MSBuild which uses AndroidMavenLibrary)
- Builds separate framework targets
- Combines into multi-framework packages

### publish-to-nuget.yml
- Downloads packages from GitHub releases
- Validates structure
- Publishes to NuGet.org

### check-sdk-updates.yml
- Monitors Datadog SDK releases
- Alerts when new versions available
- Uses `validate-android-artifacts.sh`

---

## Troubleshooting

### Build Fails with "Unable to find package"

**Cause**: NuGet package doesn't exist for specified version

**Solution**:
```bash
# Find correct version
./scripts/map-maven-to-nuget.sh "package:name" "version"

# Update Directory.Packages.props with suggested version
```

---

### Maven Artifact Download Fails

**Cause**: Maven Central connection issue or artifact not found

**Solution**:
```bash
# Verify artifact exists
curl -I https://repo1.maven.org/maven2/com/datadoghq/dd-sdk-android-rum/3.5.0/dd-sdk-android-rum-3.5.0.aar

# Check version
./scripts/validate-android-artifacts.sh 3.5.0 --download-metadata
```

---

### Framework Not Supported Error

**Cause**: NuGet package doesn't support net9.0/net10.0

**Solution**:
```bash
# Find compatible version
./scripts/check-nuget-versions.sh

# Override in Directory.Packages.props
```

---

## Best Practices

### ✅ Do This

1. **Use pack.sh for builds**: Handles correct dependency order
2. **Use map-maven-to-nuget.sh**: Finds compatible versions automatically
3. **Document version overrides**: Explain why in Directory.Packages.props
4. **Test after adding packages**: Run pack.sh and build sample app
5. **Update all at once**: Use update-sdk-version.sh for consistency

### ❌ Don't Do This

1. ❌ Don't manually download AARs (use AndroidMavenLibrary)
2. ❌ Don't blindly use Maven POM versions (check framework support)
3. ❌ Don't skip Metadata.xml transforms (causes binding issues)
4. ❌ Don't add all integration packages (only what users need)
5. ❌ Don't modify generated code (use Metadata.xml instead)

---

## Related Documentation

- [maven-nuget-version-mapping.md](../guides/android/ANDROID_DEPENDENCIES.html) - Version compatibility guide
- [PACKAGING_ARCHITECTURE.md]() - NuGet package structure
- [WORKFLOW_ARCHITECTURE.md]() - CI/CD pipeline details
- [PROJECT_OVERVIEW.md](../project/PROJECT_GUIDE.html) - Overall architecture

---

## Future Automation

Planned improvements:

1. **Automated version checking**: Bot that monitors NuGet for updates
2. **Script consolidation**: Merge generate + setup scripts
3. **Validation integration**: Run validate-android-artifacts in CI
4. **Dependency graph visualization**: Tool to show package relationships
5. **iOS automation**: Objective Sharpie integration in workflows

For current status, see [AUTOMATION_ROADMAP.md]()
