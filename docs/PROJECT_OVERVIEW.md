---
layout: default
title: Project Overview
nav_order: 3
description: "Comprehensive overview of the Datadog SDK for .NET MAUI project structure, implementation approach, and current status"
permalink: /project-overview
---

# Datadog MAUI SDK - Project Overview

This document provides a comprehensive overview of the Datadog SDK for .NET MAUI project structure, implementation approach, and current status.

## Project Status (January 2026)

### ✅ Completed

- **Android Bindings**: 9/9 binding projects building successfully (0 errors)
  - Core + Internal modules
  - RUM, Logs, Trace feature modules
  - Session Replay, NDK, WebView, Flags modules
- **Dependency Management**: Centralized pattern established with comprehensive documentation
- **Sample Application**: Working Android app demonstrating SDK initialization
- **Documentation**: Complete dependency management guides and automation roadmap

### ⏳ In Progress

- **iOS Bindings**: Scaffolded, needs binding generation and MacCatalyst compatibility
- **CI/CD Pipeline**: GitHub Actions workflows (in development)

### 🎯 Next Steps

- Complete iOS binding generation
- Implement GitHub Actions workflows for automated builds
- Create cross-platform MAUI plugin wrapper
- Expand sample app to demonstrate all features
- NuGet package publishing

## Architecture

### High-Level Design

```
┌─────────────────────────────────────────────────────┐
│         Datadog.MAUI (Main Plugin - Future)         │
│  - Cross-platform interfaces (IDatadogSdk, etc.)    │
│  - Configuration (DatadogConfiguration)             │
│  - Static entry point (DatadogSdk)                  │
└────────────────┬──────────────────┬─────────────────┘
                 │                  │
        ┌────────▼──────┐  ┌────────▼─────────┐
        │  iOS Bindings │  │ Android Bindings │
        │  (Platform)   │  │   (Modular)      │
        └───────┬───────┘  └────────┬─────────┘
                │                   │
        ┌───────▼──────┐   ┌────────▼──────────┐
        │ XCFrameworks │   │ Maven Libraries   │
        │  (Native)    │   │    (Native)       │
        └──────────────┘   └───────────────────┘
```

### Component Breakdown

#### 1. **Datadog.MAUI.Android.Binding** (Modular Structure)

The Android bindings are organized as **9 independent NuGet packages**, each binding a specific Datadog Android SDK module:

##### Core Modules

- **dd-sdk-android-core**: Foundation module, centralizes shared dependencies
  - Provides: Gson, JetBrains Annotations, Kotlin stdlib, OkHttp via NuGet
  - All feature modules depend on this via ProjectReference
- **dd-sdk-android-internal**: Internal utilities (dependency of core)

##### Feature Modules (depend on core)

- **dd-sdk-android-rum**: Real User Monitoring
- **dd-sdk-android-logs**: Logging
- **dd-sdk-android-trace**: APM Tracing
- **dd-sdk-android-session-replay**: Session Replay
- **dd-sdk-android-ndk**: Native crash reporting
- **dd-sdk-android-webview**: WebView tracking
- **dd-sdk-android-flags**: Feature flags

**Technology**: `AndroidMavenLibrary` with automated binding generation

**Dependency Pattern**:

- Core provides shared dependencies as NuGet packages
- Feature modules consume transitively via ProjectReference
- `AndroidIgnoredJavaDependency` suppresses duplicate warnings
- See [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md) for details

**Target Frameworks**: net9.0-android, net10.0-android

**Status**: ✅ All 9 projects building with 0 errors

#### 2. **Datadog.MAUI.iOS.Binding**

- **Purpose**: Provides C# bindings for iOS XCFrameworks
- **Technology**: Objective-C bindings via `NativeReference`
- **Frameworks Included**:
  - DatadogCore.xcframework
  - DatadogInternal.xcframework
  - DatadogRUM.xcframework
  - DatadogLogs.xcframework
  - DatadogTrace.xcframework
  - DatadogCrashReporting.xcframework
  - DatadogSessionReplay.xcframework
  - DatadogWebViewTracking.xcframework
- **Target Frameworks**: net9.0-ios, net10.0-ios (MacCatalyst temporarily disabled)
- **Status**: ⏳ Scaffolded, needs binding generation

#### 3. **Datadog.MAUI Plugin** (Planned)

Cross-platform wrapper that will provide a unified API across iOS and Android:

- **Shared/**:
  - `IDatadogSdk.cs` - Core SDK interface
  - `IDatadogLogger.cs` - Logging interface
  - `IDatadogRum.cs` - RUM interface
  - `IDatadogTrace.cs` - Tracing interface
  - `DatadogConfiguration.cs` - Configuration model
  - `DatadogSdk.cs` - Static entry point
- **Platforms/iOS/** - iOS-specific implementations
- **Platforms/Android/** - Android-specific implementations
- **Target Frameworks**: net9.0-ios, net10.0-ios, net9.0-android, net10.0-android

## Build System

### Scripts (All Bash)

Located in `scripts/` directory:

1. **setup-android-bindings.sh**

   - Main setup script for Android bindings
   - Downloads AARs from Maven Central
   - Generates dependency declarations
   - Usage: `./scripts/setup-android-bindings.sh [version]`

2. **generate-android-dependencies.sh**

   - Parses POM files for dependency information
   - Generates AndroidMavenLibrary and AndroidIgnoredJavaDependency entries
   - Adds appropriate `Bind="false"` attributes
   - Usage: `./scripts/generate-android-dependencies.sh`

3. **analyze-android-dependencies.sh**

   - Analyzes transitive dependency chains
   - Identifies which dependencies should be centralized
   - Helps categorize dependencies (NuGet vs Maven)
   - Usage: `./scripts/analyze-android-dependencies.sh [module]`

4. **download-ios-frameworks.sh**

   - Downloads XCFrameworks from Datadog iOS SDK releases
   - Uses GitHub API to find latest or specific version
   - Extracts to iOS binding directory
   - Usage: `./scripts/download-ios-frameworks.sh [version]`

5. **generate-ios-bindings.sh**

   - Prepares iOS binding project structure
   - Sets up framework references
   - Usage: `./scripts/generate-ios-bindings.sh`

6. **build.sh**
   - Main build script for all projects
   - Supports Debug and Release configurations
   - Creates NuGet packages in Release mode
   - Usage: `./scripts/build.sh [Debug|Release]`

### CI/CD Pipeline

**GitHub Actions Workflows**: `.github/workflows/`

Planned workflow structure:

1. **build-android.yml**: Android binding builds

   - Downloads/caches AAR files
   - Builds net9.0-android and net10.0-android separately
   - Combines into multi-framework NuGet packages
   - Validates with test app

2. **build-ios.yml**: iOS binding builds

   - Downloads/caches XCFrameworks (macOS runner)
   - Builds net9.0-ios and net10.0-ios
   - Combines into multi-framework NuGet packages
   - Validates with test app (macOS runner)

3. **build-all.yml**: Main orchestrator
   - Calls Android and iOS workflows
   - Validates all builds
   - Combines artifacts
   - Triggers on push, PR, schedule

Triggers:

- Push to main/develop branches
- Pull requests
- Manual workflow dispatch
- Weekly schedule

## Version Management

### Centralized Version Control

All versions are managed in `Directory.Build.props`:

```xml
<DatadogSdkVersion>3.5.0</DatadogSdkVersion>
<Version>$(DatadogSdkVersion)</Version>
<PackageVersion>$(DatadogSdkVersion)</PackageVersion>
```

This ensures:

- All Android binding packages have matching versions
- iOS binding version matches SDK version
- NuGet packages are versioned consistently
- Easy version updates via single property

### Updating Versions

To update to a new Datadog SDK version:

1. Update `<DatadogSdkVersion>` in Directory.Build.props
2. Run: `./scripts/setup-android-bindings.sh NEW_VERSION`
3. Run: `./scripts/download-ios-frameworks.sh NEW_VERSION`
4. Build and test both platforms
5. Review dependency changes (see [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md#version-update-checklist))
6. Update CHANGELOG.md
7. Create release tag

## File Structure

```
dd-sdk-maui/
├── .github/
│   └── workflows/
│       ├── build-all.yml              # Main CI/CD orchestrator
│       ├── build-android.yml          # Android binding pipeline
│       └── build-ios.yml              # iOS binding pipeline
├── Datadog.MAUI.Android.Binding/      # Android bindings (9 projects)
│   ├── dd-sdk-android-core/           # Core binding (centralizes deps)
│   ├── dd-sdk-android-internal/       # Internal utilities
│   ├── dd-sdk-android-rum/            # RUM feature binding
│   ├── dd-sdk-android-logs/           # Logs feature binding
│   ├── dd-sdk-android-trace/          # Trace feature binding
│   ├── dd-sdk-android-session-replay/ # Session Replay binding
│   ├── dd-sdk-android-ndk/            # NDK crash reporting
│   ├── dd-sdk-android-webview/        # WebView tracking
│   └── dd-sdk-android-flags/          # Feature flags
├── Datadog.MAUI.iOS.Binding/
│   ├── Datadog.MAUI.iOS.Binding.csproj
│   ├── ApiDefinition.cs               # iOS API definitions (TODO)
│   ├── StructsAndEnums.cs             # iOS enums/structs (TODO)
│   └── Libs/*.xcframework             # Downloaded frameworks (git-ignored)
├── samples/
│   └── DatadogMauiSample/             # Working Android sample
│       ├── DatadogMauiSample.csproj
│       └── Platforms/Android/MainApplication.cs
├── scripts/
│   ├── setup-android-bindings.sh      # Main Android setup
│   ├── generate-android-dependencies.sh # Generate dep entries
│   ├── analyze-android-dependencies.sh # Analyze dep chains
│   ├── download-ios-frameworks.sh     # Download iOS frameworks
│   ├── generate-ios-bindings.sh       # Setup iOS bindings
│   ├── build.sh                       # Build all projects
│   └── create-android-binding-projects.sh # Project scaffolding
├── docs/
│   ├── README.md                      # Docs index
│   ├── PROJECT_OVERVIEW.md            # This file
│   ├── ANDROID_DEPENDENCY_MANAGEMENT.md # Comprehensive Android dep guide
│   ├── DEPENDENCY_QUICK_REFERENCE.md  # Quick lookup tables
│   ├── AUTOMATION_ROADMAP.md          # Future automation plans
│   ├── GETTING_STARTED.md             # Getting started guide
│   ├── CONTRIBUTING.md                # Contribution guidelines
│   └── CHANGELOG.md                   # Version history
├── Directory.Build.props              # Centralized build properties
├── NuGet.Config                       # NuGet sources
├── README.md                          # Main documentation
├── LICENSE                            # Apache 2.0 license
└── .gitignore                         # Git ignore rules
```

## Key Technical Decisions

### Android: Modular Binding Architecture

**Decision**: Create 9 separate binding projects instead of one monolithic binding.

**Rationale**:

- **Flexibility**: Users can reference only the modules they need
- **NuGet Best Practices**: Mirrors Datadog's Android SDK package structure
- **Dependency Management**: Enables centralized core pattern (see docs)
- **Maintainability**: Easier to update individual modules

**Trade-offs**:

- More .csproj files to maintain
- Requires careful dependency coordination
- More complex build process

### Android: Centralized Dependency Pattern

**Decision**: Core project provides shared dependencies via NuGet, feature modules consume transitively.

**Rationale**:

- **Eliminates Duplicates**: Prevents duplicate class errors from multiple bindings
- **Single Source of Truth**: Shared deps (Gson, Kotlin, AndroidX) managed in one place
- **Transitive Benefits**: Feature modules automatically get correct versions
- **NuGet Compatibility**: Uses well-maintained Xamarin/AndroidX packages

**Implementation**: See [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md)

### iOS: XCFramework Direct Binding

**Decision**: Use NativeReference to directly bind XCFrameworks.

**Rationale**:

- **Official Apple Distribution**: XCFrameworks are Apple's standard
- **Multi-Architecture**: Single framework supports device + simulator
- **Minimal Overhead**: Direct framework linking is fast and efficient
- **Future-Proof**: Matches Apple's recommended distribution format

**Status**: Frameworks downloaded, binding generation pending

### Version Synchronization

**Decision**: MAUI SDK versions match native SDK versions (e.g., 3.5.0).

**Rationale**:

- **Clear Feature Mapping**: Users know which native features are available
- **Bug Tracking**: Easier to correlate issues with upstream SDK
- **Documentation**: Can reference native SDK docs by version
- **Transparency**: Version number communicates native SDK dependency

## Current Limitations

1. **iOS Bindings**: Not yet generated (scaffolding complete)
2. **MacCatalyst**: Temporarily disabled due to iOS binding incompatibility
3. **Cross-Platform Plugin**: Not yet implemented (bindings first)
4. **Automated Tests**: No unit or integration tests yet
5. **CI/CD**: Workflows in development
6. **NuGet Publishing**: Manual process, needs automation

## Implementation Roadmap

### Phase 1: iOS Bindings (Current)

- [ ] Generate Objective-C bindings using Objective Sharpie
- [ ] Build iOS binding project successfully
- [ ] Create iOS sample app

### Phase 2: CI/CD (Next)

- [ ] Implement GitHub Actions workflows
- [ ] Automated building and testing
- [ ] Multi-framework NuGet package generation
- [ ] Artifact publishing

### Phase 3: Cross-Platform Plugin

- [ ] Design unified API surface
- [ ] Implement platform-specific wrappers
- [ ] Configuration abstraction
- [ ] Cross-platform sample app

### Phase 4: Production Readiness

- [ ] Comprehensive testing
- [ ] Performance optimization
- [ ] Documentation site
- [ ] NuGet.org publishing

### Phase 5: Advanced Features

- [ ] Automated dependency management
- [ ] Enhanced developer tooling
- [ ] Community ecosystem

## Resources

### Documentation

- **Dependency Management**: [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md)
- **Quick Reference**: [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md)
- **Automation Plans**: [AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md)
- **Getting Started**: [GETTING_STARTED.md](GETTING_STARTED.md)

### Official Datadog

- [Datadog Mobile RUM](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/)
- [iOS SDK](https://github.com/DataDog/dd-sdk-ios)
- [Android SDK](https://github.com/DataDog/dd-sdk-android)

### .NET Documentation

- [.NET MAUI](https://docs.microsoft.com/en-us/dotnet/maui/)
- [iOS Bindings](https://docs.microsoft.com/en-us/xamarin/ios/platform/binding-objective-c/)
- [Android Bindings](https://docs.microsoft.com/en-us/xamarin/android/platform/binding-java-library/)

### Tools

- [Objective Sharpie](https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/binding/objective-sharpie/)
- [Android Binding Metadata](https://docs.microsoft.com/en-us/xamarin/android/platform/binding-java-library/customizing-bindings/java-bindings-metadata)

---

**Document Version**: 2.0
**Last Updated**: 2026-01-16
**SDK Version**: 3.5.0
**Status**: Android bindings complete, iOS bindings in progress
