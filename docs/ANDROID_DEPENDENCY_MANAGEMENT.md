---
layout: default
title: Android Dependency Management
nav_order: 5
description: "How dependencies are managed in the Datadog MAUI Android binding projects"
permalink: /android-dependency-management
---

# Android Dependency Management for .NET MAUI Bindings

This document explains how dependencies are managed in the Datadog MAUI Android binding projects, based on hard-won lessons from resolving duplicate dependency issues.

## Table of Contents
- [Overview](#overview)
- [The Dependency Problem](#the-dependency-problem)
- [The Solution Pattern](#the-solution-pattern)
- [Dependency Categories](#dependency-categories)
- [Implementation Guide](#implementation-guide)
- [Automation Strategy](#automation-strategy)
- [Troubleshooting](#troubleshooting)
- [Future Updates](#future-updates)

## Overview

The Datadog Android SDK consists of multiple modules (core, rum, logs, trace, session-replay, etc.), each with its own set of Java/Kotlin dependencies. When creating .NET bindings for these modules, we must carefully manage these dependencies to avoid:

1. **Duplicate class definitions** - Same Java classes bound multiple times
2. **Version conflicts** - Different versions of the same library
3. **Missing dependencies** - Required classes not available at runtime
4. **Build errors** - D8/R8 compilation failures

## The Dependency Problem

### What We Discovered

When binding multiple Datadog SDK modules, we initially included dependencies in each binding project independently:

```xml
<!-- In dd-sdk-android-core.csproj -->
<AndroidMavenLibrary Include="com.google.code.gson:gson" Version="2.10.1" Bind="false" />

<!-- In dd-sdk-android-rum.csproj -->
<PackageReference Include="GoogleGson" Version="2.11.0" />

<!-- In dd-sdk-android-logs.csproj -->
<PackageReference Include="GoogleGson" Version="2.11.0" />
```

**Result**: At D8/R8 compilation time, the same Java classes existed multiple times, causing build failures:

```
MSBUILD : java error JAVA0000: Type com.google.gson.ExclusionStrategy is defined multiple times
```

### Root Cause

1. **Core** had gson as `AndroidMavenLibrary` with `Bind="false"` (runtime only, no C# bindings)
2. **Feature modules** independently included `GoogleGson` NuGet package (with C# bindings)
3. Both approaches package the Java classes into the final APK
4. D8/R8 fails when seeing duplicate class definitions

## The Solution Pattern

### Centralize Shared Dependencies in Core

Since all feature modules already have `<ProjectReference>` to core, we centralize shared dependencies:

**In [dd-sdk-android-core.csproj](../Datadog.MAUI.Android.Binding/dd-sdk-android-core/dd-sdk-android-core.csproj):**

```xml
<!-- Shared dependencies as NuGet PackageReferences -->
<ItemGroup>
  <PackageReference Include="GoogleGson" Version="2.11.0" />
  <PackageReference Include="Xamarin.Jetbrains.Annotations" Version="26.0.1.1" />
  <PackageReference Include="Square.OkHttp3" Version="4.12.0" />
</ItemGroup>
```

**In all feature modules (rum, logs, trace, etc.):**

```xml
<!-- Tell the build system these are provided transitively by core -->
<ItemGroup>
  <!-- Shared dependencies provided by dd-sdk-android-core -->
  <AndroidIgnoredJavaDependency Include="com.google.code.gson:gson:2.10.1" />
  <AndroidIgnoredJavaDependency Include="org.jetbrains:annotations:13.0" />
  <AndroidIgnoredJavaDependency Include="com.squareup.okhttp3:okhttp:4.12.0" />
</ItemGroup>
```

### Why This Works

1. **Single source of truth**: Core provides C# bindings via NuGet
2. **Transitive dependencies**: Feature modules get bindings through `ProjectReference` to core
3. **No duplicates**: `AndroidIgnoredJavaDependency` tells MSBuild to skip the Maven dependency
4. **Clean compilation**: D8/R8 sees only one copy of each Java class

## Dependency Categories

Based on analyzing the Datadog SDK dependencies, we classify them into four categories:

### 1. NuGet Package Available (Preferred)

Dependencies that have well-maintained Xamarin/MAUI NuGet bindings:

| Maven Artifact | NuGet Package | Notes |
|----------------|---------------|-------|
| `com.google.code.gson:gson` | `GoogleGson` | Gson has complex binding requirements - always use NuGet |
| `org.jetbrains:annotations` | `Xamarin.Jetbrains.Annotations` | Required version ≥ 26.0.1.1 |
| `com.squareup.okhttp3:okhttp` | `Square.OkHttp3` | HTTP client |
| `androidx.work:work-runtime` | `Xamarin.AndroidX.Work.Runtime` | WorkManager |
| `androidx.annotation:annotation` | `Xamarin.AndroidX.Annotation` | AndroidX annotations |
| `androidx.collection:collection` | `Xamarin.AndroidX.Collection` | AndroidX collections |
| `androidx.lifecycle:*` | `Xamarin.AndroidX.Lifecycle.*` | Lifecycle components |

**Action**: Include as `PackageReference` in core, add `AndroidIgnoredJavaDependency` in feature modules.

### 2. Covered by MAUI Core (Skip)

Dependencies that are transitively provided by MAUI's built-in packages:

| Maven Artifact | Reason to Skip |
|----------------|----------------|
| `androidx.core:core` | Transitive from Material/Navigation packages |
| `androidx.fragment:fragment` | Transitive from Material/Navigation packages |
| `androidx.recyclerview:recyclerview` | Transitive from Material package |
| `androidx.appcompat:appcompat` | Transitive from Material package |
| `androidx.navigation:navigation-*` | Direct in MAUI Core |
| `com.google.android.material:material` | Direct in MAUI Core |
| `org.jetbrains.kotlin:kotlin-stdlib` | Embedded in SDK, also available as Xamarin.Kotlin.StdLib |
| `org.jetbrains.kotlin:kotlin-reflect` | Embedded in SDK, also available as Xamarin.Kotlin.Reflect |

**Action**: Add `AndroidIgnoredJavaDependency` - do not include as Maven or NuGet.

### 3. No NuGet Available (Download & Bind)

Dependencies without existing NuGet bindings that must be downloaded and bound:

| Maven Artifact | Purpose |
|----------------|---------|
| `com.lyft.kronos:kronos-android` | NTP time synchronization |
| `com.lyft.kronos:kronos-java` | NTP time synchronization (Java) |
| `org.jctools:jctools-core` | Java concurrency tools |
| `com.google.re2j:re2j` | Regular expression engine |
| `io.opentracing:opentracing-*` | Distributed tracing API |
| `androidx.compose.runtime:runtime` | Compose runtime (if not in MAUI) |
| `androidx.metrics:metrics-performance` | Performance metrics |

**Action**: Include as `AndroidMavenLibrary` with `Bind="false"` for runtime inclusion only.

### 4. Internal Cross-References (ProjectReference)

Datadog SDK modules that reference each other:

| Maven Artifact | Action |
|----------------|--------|
| `com.datadoghq:dd-sdk-android-*` | Use `ProjectReference` between binding projects |

**Action**: Add `<ProjectReference>` to the corresponding binding project, include Maven artifact with `Bind="false"`.

## Implementation Guide

### Step 1: Analyze POM Files

Use the provided script to fetch and parse POM files:

```bash
./scripts/generate-android-dependencies.sh dd-sdk-android-core 3.5.0
```

This outputs `AndroidMavenLibrary` entries for all runtime dependencies.

### Step 2: Categorize Dependencies

For each dependency, determine its category:

1. **Check for NuGet package**: Search https://www.nuget.org for Xamarin/AndroidX bindings
2. **Check MAUI transitive**: See if MAUI already includes it (check MAUI's package dependencies)
3. **Check Datadog internal**: Is it another dd-sdk-android module?
4. **Default to download**: If no NuGet exists and not covered by MAUI

### Step 3: Update Core Project

For shared dependencies (category 1 - NuGet available):

```xml
<!-- dd-sdk-android-core.csproj -->
<ItemGroup>
  <!-- Shared dependencies as NuGet PackageReferences -->
  <PackageReference Include="GoogleGson" Version="2.11.0" />
  <PackageReference Include="Xamarin.Jetbrains.Annotations" Version="26.0.1.1" />
  <PackageReference Include="Square.OkHttp3" Version="4.12.0" />
</ItemGroup>
```

### Step 4: Update Feature Modules

For dependencies provided by core:

```xml
<!-- dd-sdk-android-rum.csproj, dd-sdk-android-logs.csproj, etc. -->
<ItemGroup>
  <!-- Shared dependencies provided by dd-sdk-android-core -->
  <AndroidIgnoredJavaDependency Include="com.google.code.gson:gson:2.10.1" />
  <AndroidIgnoredJavaDependency Include="org.jetbrains:annotations:13.0" />
  <AndroidIgnoredJavaDependency Include="com.squareup.okhttp3:okhttp:4.12.0" />
</ItemGroup>
```

### Step 5: Test Build

```bash
dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj -f net9.0-android
```

Look for:
- ✅ `Build succeeded` with 0 errors
- ❌ `Type X is defined multiple times` = duplicate dependency issue
- ❌ `Java dependency 'X' is not satisfied` = missing dependency

## Automation Strategy

### Current State

The current `generate-android-dependencies.sh` script is **basic** - it:
- ✅ Fetches POM files from Maven Central
- ✅ Parses compile/runtime dependencies
- ✅ Filters out test dependencies
- ✅ Sets `Bind="false"` for non-Datadog packages

But it **doesn't**:
- ❌ Check if NuGet packages exist
- ❌ Handle transitive dependencies from MAUI
- ❌ Make intelligent decisions about which strategy to use
- ❌ Generate `AndroidIgnoredJavaDependency` entries

### Recommended Enhancement

Create an enhanced script that:

1. **Fetches POM files** from Maven Central
2. **For each dependency**:
   - Query NuGet.org API to check if a Xamarin/AndroidX binding exists
   - Check against a configuration file (like `deps-config.yaml`) for known mappings
   - Categorize as: nuget / skip / download / internal
3. **Generate output** for each binding project:
   - Core: `PackageReference` entries for shared NuGet dependencies
   - Feature modules: `AndroidIgnoredJavaDependency` for shared dependencies
   - All: Appropriate `AndroidMavenLibrary` entries with `Bind="false"`

### Configuration File Structure

Enhance `scripts/deps-config.yaml` with:

```yaml
# Shared dependencies to be provided by core
shared:
  - maven: com.google.code.gson:gson
    nuget: GoogleGson
    version: 2.11.0
    notes: "Complex binding requirements - always use NuGet"

  - maven: org.jetbrains:annotations
    nuget: Xamarin.Jetbrains.Annotations
    version: 26.0.1.1
    notes: "Required version >= 26.0.1.1 due to transitive deps"

# Dependencies covered by MAUI (skip entirely)
maui_transitive:
  - maven: androidx.core:core
    reason: "Transitive from Material/Navigation packages"

  - maven: org.jetbrains.kotlin:kotlin-stdlib
    reason: "Embedded in SDK + available as Xamarin.Kotlin.StdLib"

# Dependencies requiring AAR/JAR download
download:
  - maven: com.lyft.kronos:kronos-android
    reason: "No NuGet binding available"
```

## Troubleshooting

### Error: "Type X is defined multiple times"

**Cause**: Duplicate dependency - same Java classes bound in multiple projects.

**Solution**:
1. Identify which dependency contains class X
2. Move it to core as a `PackageReference` (if NuGet available)
3. Add `AndroidIgnoredJavaDependency` in all feature modules
4. Remove any duplicate `PackageReference` from feature modules

### Error: "Java dependency 'X' is not satisfied"

**Cause**: Missing dependency - build system expects it but can't find it.

**Solution**:
1. Check if it's supposed to come from core transitively
2. If yes: Add `AndroidIgnoredJavaDependency` to tell build system it's satisfied
3. If no: Add as `PackageReference` (NuGet) or `AndroidMavenLibrary` (Maven)

### Error: "Package downgrade: X from Y to Z"

**Cause**: Version conflict - transitive dependency requires newer version.

**Solution**:
1. Update the package version in core to match the required version
2. Example: Update `Xamarin.Jetbrains.Annotations` from 24.1.0 to 26.0.1.1

### Build succeeds but app crashes at runtime

**Cause**: Dependency is ignored but not actually provided transitively.

**Solution**:
1. Check that core actually includes the dependency
2. Verify feature module has `<ProjectReference>` to core
3. Remove `AndroidIgnoredJavaDependency` if dependency isn't actually shared

## Future Updates

### When Datadog SDK Updates

1. **Fetch new POM files**:
   ```bash
   ./scripts/generate-android-dependencies.sh dd-sdk-android-core 3.6.0
   ```

2. **Compare dependencies**:
   - New dependencies? → Categorize and add to appropriate project
   - Removed dependencies? → Clean up references
   - Version changes? → Update version numbers

3. **Test thoroughly**:
   ```bash
   dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj
   ```

4. **Update this document** with any new learnings

### When NuGet Packages Change

1. **New Xamarin/AndroidX binding released**?
   - Move from `AndroidMavenLibrary` to `PackageReference`
   - Update shared dependencies in core
   - Add to `AndroidIgnoredJavaDependency` in feature modules

2. **NuGet package deprecated**?
   - Fall back to `AndroidMavenLibrary` approach
   - Remove from shared dependencies
   - Remove `AndroidIgnoredJavaDependency` from feature modules

### When MAUI Updates

MAUI updates may change which dependencies are transitively included:

1. **Check MAUI release notes** for dependency changes
2. **Update "Covered by MAUI" list** in this document
3. **Adjust `AndroidIgnoredJavaDependency` entries** accordingly
4. **Test build** to ensure no missing dependencies

## Key Insights

### 1. AndroidMavenLibrary with Bind="false" is Runtime-Only

```xml
<AndroidMavenLibrary Include="com.google.code.gson:gson" Version="2.10.1" Bind="false" />
```

This includes the Java classes at runtime but **does not generate C# bindings**. Use this for:
- Dependencies where NuGet bindings already exist (to avoid duplication)
- Transitive dependencies that don't need C# API exposure

### 2. GoogleGson Must Always Use NuGet Package

Gson has complex Java generics and edge cases that require extensive metadata transforms. The `GoogleGson` NuGet package has these transforms built-in. Attempting to bind gson directly from Maven will result in compilation errors:

```
error CS0534: 'DateTypeAdapter' does not implement inherited abstract member 'TypeAdapter.Write(JsonWriter?, Object?)'
```

**Always** use `PackageReference Include="GoogleGson"`, never bind from Maven.

### 3. Version Mismatches Cascade

A single version mismatch can cascade through transitive dependencies:

```
error NU1605: Package downgrade: Xamarin.Jetbrains.Annotations from 26.0.1.1 to 24.1.0
```

When updating shared dependencies, check all transitive requirements and update to the **highest required version**.

### 4. ProjectReference Provides Transitive Dependencies

When a feature module has:

```xml
<ProjectReference Include="..\dd-sdk-android-core\dd-sdk-android-core.csproj" />
```

It automatically gets:
- All `PackageReference` dependencies from core
- All `AndroidMavenLibrary` dependencies from core
- All generated C# bindings from core

This is the foundation of the centralized dependency pattern.

### 5. AndroidIgnoredJavaDependency is Declaration, Not Filter

```xml
<AndroidIgnoredJavaDependency Include="com.google.code.gson:gson:2.10.1" />
```

This tells MSBuild: "I know this dependency exists in the Maven POM, but I'm handling it another way (via NuGet or transitive from core), so don't error about it being missing."

It's a **declaration of intent**, not a filter that removes the dependency.

## Summary

**The Core Principle**: Centralize shared dependencies in the core binding project and let transitive dependencies flow to feature modules through ProjectReferences.

**The Three Rules**:
1. **Shared dependencies** → Core as `PackageReference`, feature modules add `AndroidIgnoredJavaDependency`
2. **MAUI-provided dependencies** → All modules add `AndroidIgnoredJavaDependency`, no one includes them
3. **Module-specific dependencies** → Include only where needed, as `PackageReference` or `AndroidMavenLibrary`

This pattern prevents duplicate class definitions while ensuring all required dependencies are available at runtime.

---

**Document Version**: 1.0
**Last Updated**: 2026-01-16
**Datadog SDK Version**: 3.5.0
