---
layout: default
title: Overview
parent: Project
nav_order: 1
permalink: /project/overview
---

# Project Guide

Complete guide to the Datadog MAUI SDK project - architecture, structure, and current status.

---

## Quick Navigation

- **New to the project?** Start with [Current Status](#current-status)
- **Need architecture details?** See [Architecture](#architecture)
- **Looking for files?** Check [Directory Structure](#directory-structure)
- **Want to build?** Go to [Build System](#build-system)

---

## Current Status

**Project Created**: January 15, 2026
**SDK Version**: 3.5.0
**.NET Targets**: 8, 9, 10
**Platforms**: iOS (12.0+), Android (API 21+)

### ✅ Completed

**Android Bindings**: 13/13 packages building successfully
- **Core**: dd-sdk-android-internal, dd-sdk-android-core
- **Features**: logs, rum, trace, ndk, session-replay, webview, flags
- **Integrations**: okhttp, trace-otel, okhttp-otel, gradle-plugin

**Build System**: Complete automation
- Multi-framework targeting (net9.0/net10.0-android)
- GitHub Actions workflows
- Package combination scripts
- Version management tools

**Documentation**: Comprehensive guides
- Android dependency management
- Build scripts and workflows
- iOS binding strategy
- Integration packages

### 🚧 In Progress

**iOS Bindings**: Scaffolded, implementing minimal manual bindings
- Objective Sharpie generated 684 API types
- Creating clean user-facing bindings (~250 lines vs 7,199)
- Following opt-in approach

**Unified API**: Cross-platform MAUI plugin
- Design complete ()
- Implementation pending iOS bindings

### 🎯 Next Steps

1. Complete iOS minimal bindings
2. Implement cross-platform MAUI plugin wrapper
3. Expand sample app to demonstrate all features
4. NuGet package publishing

---

## Architecture

### High-Level Design

```
┌───────────────────────────────────────────────────┐
│         Datadog.MAUI (Main Plugin - Future)       │
│  - Cross-platform interfaces (IDatadogSdk)        │
│  - Configuration (DatadogConfiguration)           │
│  - Static entry point (DatadogSdk)                │
└──────────────┬─────────────────┬──────────────────┘
               │                 │
      ┌────────▼───────┐  ┌──────▼────────┐
      │  Android       │  │  iOS          │
      │  Platform      │  │  Platform     │
      │  (13 packages) │  │  (In progress)│
      └────────────────┘  └───────────────┘
```

### Component Breakdown

#### 1. Android Bindings (Datadog.MAUI.Android.Binding)

**Modular Architecture**: 13 separate NuGet packages

**Core Packages** (2):
- `Datadog.MAUI.Android.Internal` - Internal utilities
- `Datadog.MAUI.Android.Core` - Core SDK functionality, initialization

**Feature Packages** (7):
- `Datadog.MAUI.Android.Logs` - Logging functionality
- `Datadog.MAUI.Android.RUM` - Real User Monitoring
- `Datadog.MAUI.Android.Trace` - APM tracing
- `Datadog.MAUI.Android.NDK` - Native crash reporting
- `Datadog.MAUI.Android.SessionReplay` - Session replay
- `Datadog.MAUI.Android.WebView` - WebView tracking
- `Datadog.MAUI.Android.Flags` - Feature flags

**Integration Packages** (4):
- `Datadog.MAUI.Android.OkHttp` - OkHttp instrumentation
- `Datadog.MAUI.Android.Trace.OpenTelemetry` - OTel integration
- `Datadog.MAUI.Android.OkHttp.OpenTelemetry` - OkHttp + OTel
- `Datadog.MAUI.Android.GradlePlugin` - Build-time tools

**Dependency Pattern**: Centralized core
- Core provides shared dependencies (Gson, Kotlin, AndroidX)
- Features reference core via `ProjectReference`
- Features declare `AndroidIgnoredJavaDependency` for shared deps

#### 2. iOS Bindings (Datadog.MAUI.iOS.Binding)

**Status**: In progress - implementing minimal manual bindings

**Approach**: Opt-in (expose only user-facing APIs)
- Generated bindings: 7,199 lines (294 interfaces)
- Manual bindings: ~250 lines (3 essential interfaces)
- Covers 100% of user needs with 97% less code

**Frameworks** (8):
- DatadogCore - SDK initialization
- DatadogRUM - RUM monitoring
- DatadogLogs - Logging
- DatadogTrace - Tracing
- DatadogSessionReplay - Session replay
- DatadogCrashReporting - Crash reports
- DatadogWebViewTracking - WebView tracking
- DatadogInternal - Internal utilities

#### 3. Main Plugin (Datadog.MAUI.Plugin)

**Status**: Design complete, implementation pending

**Purpose**: Cross-platform abstraction layer

**Features**:
- Unified API across iOS and Android
- Dependency injection support
- Configuration builder pattern
- Platform-specific implementations

---

## Directory Structure

```
dd-sdk-maui/
├── Datadog.MAUI.Android.Binding/   # Android native bindings (13 packages)
│   ├── dd-sdk-android-internal/
│   ├── dd-sdk-android-core/
│   ├── dd-sdk-android-logs/
│   ├── dd-sdk-android-rum/
│   ├── dd-sdk-android-trace/
│   ├── dd-sdk-android-ndk/
│   ├── dd-sdk-android-session-replay/
│   ├── dd-sdk-android-webview/
│   ├── dd-sdk-android-flags/
│   ├── dd-sdk-android-okhttp/        # Integration packages
│   ├── dd-sdk-android-trace-otel/
│   ├── dd-sdk-android-okhttp-otel/
│   ├── dd-sdk-android-gradle-plugin/
│   └── Datadog.MAUI.Android.Binding.csproj  # Meta-package
│
├── Datadog.MAUI.iOS.Binding/       # iOS native bindings (in progress)
│   ├── DatadogCore/
│   ├── DatadogRUM/
│   ├── DatadogLogs/
│   ├── DatadogTrace/
│   ├── DatadogSessionReplay/
│   ├── DatadogCrashReporting/
│   └── DatadogWebViewTracking/
│
├── Datadog.MAUI.Plugin/            # Cross-platform plugin (planned)
│   ├── Datadog.MAUI.Plugin.csproj
│   ├── IDatadogSdk.cs
│   ├── DatadogConfiguration.cs
│   ├── Platforms/
│   │   ├── Android/
│   │   └── iOS/
│   └── README.md
│
├── samples/DatadogMauiSample/      # Sample application
│   ├── DatadogMauiSample.csproj
│   ├── MauiProgram.cs
│   ├── Platforms/
│   │   ├── Android/
│   │   └── iOS/
│   └── README.md
│
├── scripts/                        # Build and automation scripts
│   ├── pack.sh                    # Master build script
│   ├── map-maven-to-nuget.sh     # Dependency mapping
│   ├── validate-android-artifacts.sh
│   ├── check-nuget-versions.sh
│   └── update-sdk-version.sh
│
├── .github/workflows/              # CI/CD pipelines
│   ├── build-all.yml              # Master workflow
│   ├── build-android.yml          # Android-specific
│   ├── build-ios.yml              # iOS-specific
│   ├── publish-to-nuget.yml       # Package publishing
│   └── check-sdk-updates.yml      # Version monitoring
│
├── docs/                           # Documentation
│   ├── README.md                  # Documentation index
│   ├── PROJECT_GUIDE.md           # This file
│   ├── ANDROID_DEPENDENCIES.md    # Android dependency guide
│   ├── ANDROID_INTEGRATION_PACKAGES.md
│   ├── IOS_BINDING_STRATEGY.md
│   ├── SCRIPTS_OVERVIEW.md
│   ├── WORKFLOW_ARCHITECTURE.md
│   ├── PACKAGING_ARCHITECTURE.md
│   └── _reference/                # Historical docs
│
├── Directory.Build.props           # Centralized MSBuild properties
├── Directory.Packages.props        # Centralized NuGet versions
├── global.json                     # .NET SDK version pinning
├── NuGet.Config                    # NuGet configuration
└── README.md                       # Main project README
```

---

## Build System

### Package Structure

**3-Tier Architecture**:
1. **Module Packages** - Individual binding packages
2. **Meta Packages** - Dependency-only packages (Datadog.MAUI.Android.Binding)
3. **Main Plugin** - Cross-platform wrapper (future)

### Build Process

**Local Build**:
```bash
./scripts/pack.sh
```

**What happens**:
1. Builds all Android modules in dependency order
2. Packs into NuGet packages (.nupkg)
3. Places in `./artifacts/` directory
4. Supports multi-framework targeting

**CI/CD Pipeline**:
1. **build-android.yml** - Builds Android packages
   - Separate builds for net9.0-android and net10.0-android
   - Combines into multi-framework packages
   - Tests with sample app
2. **build-ios.yml** - Builds iOS packages (planned)
3. **build-all.yml** - Master orchestrator
4. **publish-to-nuget.yml** - Publishes to NuGet.org

### Version Management

**Centralized in `Directory.Build.props`**:
```xml
<DatadogSdkVersion>3.5.0</DatadogSdkVersion>
<PackageVersion>3.5.0</PackageVersion>
```

**Centralized in `Directory.Packages.props`**:
```xml
<PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.3.0.1" />
<PackageVersion Include="GoogleGson" Version="2.11.0" />
```

**Update Process**:
```bash
./scripts/update-sdk-version.sh 3.6.0
```

---

## Technical Decisions

### 1. Modular Package Architecture

**Decision**: Create separate NuGet packages for each feature

**Rationale**:
- Users opt-in to features they need
- Smaller app size (no unused code)
- Matches upstream SDK structure
- Easier to maintain and update

**Alternative Considered**: Single "fat" package with everything

**Why Not**: Bloats app size, forces users to include unused features

---

### 2. AndroidMavenLibrary vs Manual AARs

**Decision**: Use `AndroidMavenLibrary` for automatic Maven downloads

**Rationale**:
- Automatic transitive dependency resolution
- No manual AAR management
- Easier to update versions
- Standard .NET Android approach

**Alternative Considered**: Manually download and embed AARs

**Why Not**: Manual process, harder to maintain, no dependency resolution

---

### 3. Centralized Dependency Management

**Decision**: Core provides shared dependencies, features consume

**Rationale**:
- Prevents duplicate Java classes
- Single source of truth for versions
- Transitive via ProjectReference
- Avoids D8/R8 compilation errors

**Alternative Considered**: Each module manages its own dependencies

**Why Not**: Causes duplicate class errors, version conflicts

---

### 4. Directory.Packages.props for Versions

**Decision**: Central Package Management (CPM) for all NuGet versions

**Rationale**:
- Single file for all version numbers
- Prevents version conflicts
- Easier to audit and update
- MSBuild best practice

**Alternative Considered**: Versions in each .csproj

**Why Not**: Hard to maintain consistency, prone to conflicts

---

### 5. iOS Minimal Manual Bindings

**Decision**: Create clean user-facing bindings, not fix all generated code

**Rationale**:
- 97% less code (250 lines vs 7,199)
- Exposes only what users need
- Easier to document and maintain
- Follows opt-in philosophy

**Alternative Considered**: Fix all 42 errors in Objective Sharpie output

**Why Not**: Exposes 97% internal APIs, high maintenance burden

---

## Key Metrics

### Android Bindings

- **Packages**: 13 (2 core + 7 features + 4 integrations)
- **Target Frameworks**: net9.0-android, net10.0-android
- **Build Status**: 0 errors, 0 warnings (critical)
- **Maven Artifacts**: Automatically downloaded
- **Shared Dependencies**: 3 (Gson, Kotlin, Annotations)

### iOS Bindings

- **Frameworks**: 8 (Core, RUM, Logs, Trace, SessionReplay, CrashReporting, WebView, Internal)
- **Target Frameworks**: net8.0-ios, net9.0-ios, net10.0-ios (planned)
- **Generated Code**: 684 types (7,199 lines) → Manual: 3 types (~250 lines)
- **Coverage**: 100% of user-facing APIs

### Build System

- **Scripts**: 10 automation scripts
- **Workflows**: 6 GitHub Actions workflows
- **Build Time**: ~5 minutes (first), ~2 minutes (cached)
- **Artifacts**: .nupkg packages in `./artifacts/`

---

## Related Documentation

- [Android Dependencies]() - Complete dependency guide
- [Android Integration Packages]() - Optional integrations
- [iOS Binding Strategy]() - iOS implementation approach
- [Scripts Overview]() - Build automation details
- [Workflow Architecture]() - CI/CD pipeline
- [Packaging Architecture]() - NuGet structure
- [Unified API Design]() - Cross-platform API spec

---

**Last Updated**: 2026-01-20
