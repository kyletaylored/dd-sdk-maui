# Datadog MAUI SDK - Developer Guide

Complete guide for developers working on the Datadog MAUI SDK.

## Table of Contents

- [Prerequisites](#prerequisites)
- [First-Time Setup](#first-time-setup)
- [Common Development Tasks](#common-development-tasks)
- [Building the SDK](#building-the-sdk)
- [Packaging](#packaging)
- [Testing](#testing)
- [Sample Apps](#sample-apps)
- [Android Binding Development](#android-binding-development)
- [iOS Binding Development](#ios-binding-development)
- [Troubleshooting](#troubleshooting)
- [Architecture Documentation](#architecture-documentation)

## Prerequisites

### Required Tools

- **.NET SDK** - Version 9.0 or higher
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Android Workload** - For Android development
- **iOS Workload** - For iOS development (macOS only)
- **Xcode** - Latest version (macOS only, for iOS builds)

### Check Prerequisites

```bash
make check-prereqs
```

This verifies that all required tools are installed.

## First-Time Setup

### 1. Clone the Repository

```bash
git clone https://github.com/DataDog/dd-sdk-maui.git
cd dd-sdk-maui
```

### 2. Run Development Setup

```bash
make dev-setup
```

This command will:
- Install Android and iOS workloads
- Restore all NuGet packages
- Verify prerequisites

### 3. View Available Commands

```bash
make help
```

Or view the quick reference:

```bash
make readme
```

## Common Development Tasks

### Check Project Status

```bash
make status
```

Shows:
- .NET SDK version
- Current git status

### Restore NuGet Packages

```bash
make restore
```

Restores packages for:
- Main solution
- Android bindings
- iOS bindings (macOS only)
- Sample app

### Format Code

```bash
make format
```

Runs `dotnet format` on the entire solution.

### Clean Build Artifacts

```bash
# Clean bin/obj and local packages
make clean

# Deep clean including generated files
make clean-all
```

## Building the SDK

### Build Everything

```bash
make build
# or
make build-all
```

Builds all projects in the correct order:
1. Android binding modules
2. iOS binding modules (macOS only)
3. MAUI Plugin

**Note**: Meta-packages (Android/iOS bindings) are only built during `make pack`.

### Build Specific Platforms

#### Android Only

```bash
# Build Release configuration
make build-android

# Build Debug configuration
make build-android-debug

# Show only errors (hide warnings)
make build-android-errors
```

**Android modules built:**
- `dd-sdk-android-internal`
- `dd-sdk-android-core`
- `dd-sdk-android-logs`
- `dd-sdk-android-rum`
- `dd-sdk-android-trace`
- `dd-sdk-android-ndk`
- `dd-sdk-android-session-replay`
- `dd-sdk-android-webview`
- `dd-sdk-android-flags`
- `opentracing-api` (added via GitHub workflow)

#### iOS Only (macOS required)

```bash
# Build Release configuration
make build-ios

# Build Debug configuration
make build-ios-debug

# Show only errors (hide warnings)
make build-ios-errors
```

**iOS modules built:**
- `DatadogInternal`
- `DatadogCore`
- `DatadogLogs`
- `DatadogRUM`
- `DatadogTrace`
- `DatadogCrashReporting`
- `DatadogSessionReplay`
- `DatadogWebViewTracking`
- `DatadogFlags`
- `OpenTelemetryApi`

#### Plugin Only

```bash
make build-plugin
```

Builds the cross-platform MAUI plugin that wraps Android and iOS bindings.

### Show Only Errors

To see only errors without warnings during build:

```bash
make build-errors
```

Runs both `build-android-errors` and `build-ios-errors`.

## Packaging

### Create All NuGet Packages

```bash
make pack
```

This follows the proper dependency order documented in [docs/PACKAGING_ARCHITECTURE.md](PACKAGING_ARCHITECTURE.md):

1. Builds all binding modules
2. Packs individual Android modules (net9.0-android and net10.0-android)
3. Combines Android packages into multi-target packages
4. Packs individual iOS modules
5. Packs Android and iOS meta-packages
6. Packs the MAUI Plugin

**Output**: All packages are created in `./artifacts/`

### Pack in Debug Configuration

```bash
make pack-debug
```

Creates packages in Debug configuration, output to `./artifacts-debug/`

### List Generated Packages

```bash
make list-packages
```

Shows all `.nupkg` files in `./local-packages/`.

## Testing

### Run Unit Tests

```bash
make test
```

Runs all tests in `Datadog.MAUI.Plugin.Tests/`.

### Run Tests with Verbose Output

```bash
make test-verbose
```

Shows detailed test output for debugging.

## Sample Apps

### Build and Run Android Sample

```bash
# Build and launch on connected device/emulator
make sample-android

# Build only (don't run)
make sample-build-android
```

Requirements:
- Android emulator running or device connected
- Android SDK installed

### Build and Run iOS Sample (macOS only)

```bash
# Build and launch on default simulator
make sample-ios

# Build only (don't run)
make sample-build-ios

# Launch on specific simulator (iPhone 15 Pro)
make sample-ios-simulator
```

### List Available iOS Simulators

```bash
make list-simulators
```

Shows all available iPhone and iPad simulators.

## Android Binding Development

### Generate Android Dependencies

When adding a new Android SDK module, analyze its Maven POM to get dependency information:

```bash
make generate-android-deps MODULE=dd-sdk-android-rum VERSION=3.5.0
```

This script:
1. Downloads the module's POM file from Maven Central
2. Extracts all dependencies
3. Generates `<AndroidMavenLibrary>` entries for your `.csproj`

See [docs/ANDROID_DEPENDENCIES.md](ANDROID_DEPENDENCIES.md) for details.

### Setup Android Binding from Build Errors

When you have build errors in an Android binding project:

```bash
make setup-android-binding PROJECT=Datadog.MAUI.Android.Binding/dd-sdk-android-rum VERSION=3.5.0
```

This script:
1. Attempts to build the project
2. Analyzes error output
3. Suggests missing dependencies
4. Generates dependency entries

See [docs/SCRIPTS_OVERVIEW.md](SCRIPTS_OVERVIEW.md) for details.

### Watch Android Bindings for Changes

Auto-rebuild on file changes:

```bash
make watch-android
```

Press `Ctrl+C` to stop.

## iOS Binding Development

### Generate iOS Bindings with Objective Sharpie

```bash
make generate-ios-bindings
```

Uses Objective Sharpie to generate C# bindings from iOS XCFrameworks.

Requirements:
- macOS
- Objective Sharpie installed
- XCFrameworks in `Datadog.MAUI.iOS.Binding/artifacts/`

See [docs/IOS_BINDING_STRATEGY.md](IOS_BINDING_STRATEGY.md) for details.

### Watch iOS Bindings for Changes

Auto-rebuild on file changes:

```bash
make watch-ios
```

Press `Ctrl+C` to stop.

## Troubleshooting

### Sample App Fails to Build

**Problem**: Duplicate class errors (XA4215) when building Android sample.

**Solution**:
1. Rebuild binding packages: `make pack`
2. Clean sample: `cd samples/DatadogMauiSample && dotnet clean`
3. Rebuild sample: `make sample-build-android`

### Missing XCFrameworks for iOS

**Problem**: `make build-ios` fails with "No XCFrameworks found"

**Solution**:
1. Download XCFrameworks from Datadog iOS SDK releases
2. Place them in `Datadog.MAUI.iOS.Binding/artifacts/`
3. Run the GitHub workflow `build-ios.yml` which handles this automatically

### OpenTracingApi Package Missing

**Problem**: Android builds fail with `NU1101: Unable to find package Datadog.MAUI.Android.OpenTracingApi`

**Solution**:
The `opentracing-api` module is now included in the build loops. Re-run the GitHub workflow or:

```bash
make clean
make pack
```

### Gradle Plugin Versioning Mismatch

**Problem**: `dd-sdk-android-gradle-plugin` version doesn't match SDK version.

**Note**: The gradle plugin has been archived (moved to `_archive/`) because it's incompatible with MAUI's MSBuild system. For mapping file uploads, see [docs/MAPPING_FILE_UPLOADS.md](MAPPING_FILE_UPLOADS.md).

### R8 Mapping Files Not Generated

**Problem**: `mapping.txt` file not found after Release build.

**Solution**: Enable R8 in your `.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidEnableR8>True</AndroidEnableR8>
  <AndroidLinkTool>r8</AndroidLinkTool>
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
</PropertyGroup>
```

See [docs/MAPPING_FILE_UPLOADS.md](MAPPING_FILE_UPLOADS.md) for complete guide.

## Architecture Documentation

### Core Documentation

- **[README.md](../README.md)** - Project overview and quick start
- **[docs/README.md](README.md)** - Documentation index
- **[GETTING_STARTED.md](GETTING_STARTED.md)** - User-facing setup guide
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Contribution guidelines

### Build & Packaging

- **[PACKAGING_ARCHITECTURE.md](PACKAGING_ARCHITECTURE.md)** - NuGet packaging strategy
- **[WORKFLOW_ARCHITECTURE.md](WORKFLOW_ARCHITECTURE.md)** - GitHub Actions workflows
- **[SCRIPTS_OVERVIEW.md](SCRIPTS_OVERVIEW.md)** - Build script documentation

### Binding Development

- **[ANDROID_DEPENDENCIES.md](ANDROID_DEPENDENCIES.md)** - Android Maven dependency management
- **[ANDROID_INTEGRATION_PACKAGES.md](ANDROID_INTEGRATION_PACKAGES.md)** - Android integration modules
- **[IOS_BINDING_STRATEGY.md](IOS_BINDING_STRATEGY.md)** - iOS binding approach
- **[IDENTIFYING_USER_FACING_APIS.md](IDENTIFYING_USER_FACING_APIS.md)** - API surface analysis

### API Design

- **[UNIFIED_API_DESIGN.md](UNIFIED_API_DESIGN.md)** - Cross-platform API design
- **[RUM_BINDING_COMPARISON.md](RUM_BINDING_COMPARISON.md)** - RUM API comparison
- **[PROJECT_GUIDE.md](PROJECT_GUIDE.md)** - Project structure guide

### Deployment

- **[MAPPING_FILE_UPLOADS.md](MAPPING_FILE_UPLOADS.md)** - R8/ProGuard mapping file uploads for crash symbolication

### Automation

- **[AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md)** - CI/CD automation roadmap

## Quick Command Reference

| Task | Command |
|------|---------|
| First-time setup | `make dev-setup` |
| Build everything | `make build` or `make build-all` |
| Build Android only | `make build-android` |
| Build iOS only | `make build-ios` |
| Build plugin only | `make build-plugin` |
| Create all packages | `make pack` |
| Run tests | `make test` |
| Clean artifacts | `make clean` |
| Restore packages | `make restore` |
| Format code | `make format` |
| Run Android sample | `make sample-android` |
| Run iOS sample | `make sample-ios` |
| Check prerequisites | `make check-prereqs` |
| View all commands | `make help` |

## Development Workflow

### Adding a New Feature

1. Create a feature branch:
   ```bash
   git checkout -b feature/my-feature
   ```

2. Make your changes to the appropriate projects:
   - Android bindings: `Datadog.MAUI.Android.Binding/`
   - iOS bindings: `Datadog.MAUI.iOS.Binding/`
   - Plugin: `Datadog.MAUI.Plugin/`

3. Build and test:
   ```bash
   make build
   make test
   ```

4. Format code:
   ```bash
   make format
   ```

5. Test with sample app:
   ```bash
   make pack
   make sample-android  # or sample-ios
   ```

6. Commit and push:
   ```bash
   git add .
   git commit -m "feat: add my feature"
   git push origin feature/my-feature
   ```

### Fixing a Binding Issue

1. Identify which binding module has the issue

2. Build just that module to see errors:
   ```bash
   cd Datadog.MAUI.Android.Binding/dd-sdk-android-rum
   dotnet build
   ```

3. Fix issues (usually in `Transforms/Metadata.xml`)

4. Rebuild:
   ```bash
   make build-android
   ```

5. Test:
   ```bash
   make pack
   make sample-android
   ```

### Updating SDK Versions

When Datadog releases a new Android or iOS SDK version:

1. Update version in `Directory.Build.props`:
   ```xml
   <DatadogSdkVersion>3.6.0</DatadogSdkVersion>
   ```

2. For Android, regenerate dependencies if needed:
   ```bash
   make generate-android-deps MODULE=dd-sdk-android-core VERSION=3.6.0
   ```

3. For iOS, download new XCFrameworks and place in `Datadog.MAUI.iOS.Binding/artifacts/`

4. Build and test:
   ```bash
   make clean-all
   make pack
   make test
   ```

5. Test sample apps on both platforms

## CI/CD Integration

The project uses GitHub Actions for automated builds. See [WORKFLOW_ARCHITECTURE.md](WORKFLOW_ARCHITECTURE.md) for details.

### Workflows

- **build-all.yml** - Builds Android and iOS in parallel
- **build-android.yml** - Android-specific build
- **build-ios.yml** - iOS-specific build (downloads XCFrameworks automatically)

### Local Testing Before Push

Before pushing changes, ensure everything builds:

```bash
make clean
make build-all
make test
make pack
```

## Getting Help

- **View all commands**: `make help`
- **Quick reference**: `make readme`
- **Check status**: `make status`
- **Documentation**: Browse `docs/` directory

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed contribution guidelines.
