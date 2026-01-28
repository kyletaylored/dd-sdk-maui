---
layout: default
title: Dependencies
parent: Android
nav_order: 1
---

# Android Dependencies Guide

Complete guide to managing Android dependencies in the Datadog MAUI SDK bindings.

---

## Quick Start

**New to Android bindings?** Read [The Problem & Solution](#the-problem-and-solution) first.

**Need a quick answer?** Jump to [Quick Reference](#quick-reference).

**Upgrading SDK version?** See [Version Updates](#version-updates).

---

## Table of Contents

- [The Problem and Solution](#the-problem-and-solution)
- [Quick Reference](#quick-reference)
- [Dependency Categories](#dependency-categories)
- [Maven to NuGet Mapping](#maven-to-nuget-mapping)
- [Implementation Guide](#implementation-guide)
- [Version Updates](#version-updates)
- [Troubleshooting](#troubleshooting)

---

## The Problem and Solution

### The Problem

When binding multiple Datadog SDK modules, dependencies appear in multiple places:

```xml
<!-- dd-sdk-android-core needs gson -->
<AndroidMavenLibrary Include="com.google.code.gson:gson" Version="2.10.1" />

<!-- dd-sdk-android-rum also needs gson -->
<PackageReference Include="GoogleGson" Version="2.11.0" />

<!-- dd-sdk-android-logs also needs gson -->
<PackageReference Include="GoogleGson" Version="2.11.0" />
```

**Result**: Duplicate Java classes at compile time → Build fails

### The Solution

**Centralized Core Pattern**: Shared dependencies live in **core**, feature modules reference core.

```xml
<!-- dd-sdk-android-core.csproj - Provides shared dependencies -->
<ItemGroup>
  <PackageReference Include="GoogleGson" Version="2.11.0" />
  <PackageReference Include="Xamarin.Jetbrains.Annotations" Version="26.0.1.1" />
</ItemGroup>

<!-- dd-sdk-android-rum.csproj - Consumes from core -->
<ItemGroup>
  <ProjectReference Include="../dd-sdk-android-core/dd-sdk-android-core.csproj" />
  <AndroidIgnoredJavaDependency Include="com.google.code.gson:gson" />
  <AndroidIgnoredJavaDependency Include="org.jetbrains:annotations" />
</ItemGroup>
```

**Key insight**: `AndroidIgnoredJavaDependency` tells MSBuild "I know this is missing from Maven, but I've got it via ProjectReference"

---

## Quick Reference

### Decision Tree

```
Is this dependency needed?
├─ Datadog internal (com.datadoghq:*)?
│  └─ Use <ProjectReference> to other binding project
│
├─ Has Xamarin/AndroidX NuGet package?
│  ├─ Shared by multiple modules?
│  │  ├─ Add to core as <PackageReference>
│  │  └─ Add <AndroidIgnoredJavaDependency> in consumers
│  └─ Used by one module?
│     └─ Add <PackageReference> where needed
│
├─ Already in MAUI (androidx.* common)?
│  └─ Add <AndroidIgnoredJavaDependency> everywhere
│
└─ No NuGet package exists?
   └─ Use <AndroidMavenLibrary Bind="false">
```

### Common Dependencies

| Maven Coordinate | NuGet Package | Where | Version |
|------------------|---------------|-------|---------|
| **In Core (shared)** |
| `com.google.code.gson:gson` | `GoogleGson` | Core | 2.11.0 |
| `org.jetbrains:annotations` | `Xamarin.Jetbrains.Annotations` | Core | 26.0.1.1 |
| `com.squareup.okhttp3:okhttp` | `Square.OkHttp3` | Core | 4.12.0 |
| **MAUI Transitive (skip)** |
| `androidx.core:core` | (MAUI has it) | Skip | - |
| `androidx.fragment:fragment` | (MAUI has it) | Skip | - |
| `androidx.appcompat:appcompat` | (MAUI has it) | Skip | - |
| **Datadog Modules** |
| `com.datadoghq:dd-sdk-android-*` | ProjectReference | Variable | - |

---

## Dependency Categories

### 1. Datadog Internal Dependencies

**Maven**: `com.datadoghq:dd-sdk-android-*`

**Action**: Use `<ProjectReference>`

**Example**:
```xml
<!-- dd-sdk-android-rum depends on dd-sdk-android-core -->
<ProjectReference Include="../dd-sdk-android-core/dd-sdk-android-core.csproj" />
```

**Why**: We're binding these ourselves, so reference the binding project.

---

### 2. Shared External Dependencies

**Maven**: `com.google.code.gson:gson`, `org.jetbrains:annotations`

**Action**:
- Add NuGet package to **core**
- Add `<AndroidIgnoredJavaDependency>` in **consumers**

**Example**:
```xml
<!-- In core -->
<PackageReference Include="GoogleGson" Version="2.11.0" />

<!-- In rum, logs, trace, etc. -->
<AndroidIgnoredJavaDependency Include="com.google.code.gson:gson" />
```

**Why**: One package provides it, others consume via transitive reference.

---

### 3. MAUI Transitive Dependencies

**Maven**: `androidx.core:core`, `androidx.fragment:fragment`, etc.

**Action**: Add `<AndroidIgnoredJavaDependency>` everywhere

**Example**:
```xml
<AndroidIgnoredJavaDependency Include="androidx.core:core" />
<AndroidIgnoredJavaDependency Include="androidx.fragment:fragment" />
```

**Why**: MAUI already includes these. We don't need to provide them.

**How to check if MAUI has it**:
```bash
dotnet list package --include-transitive | grep -i androidx.core
```

---

### 4. Module-Specific Dependencies

**Maven**: Unique to one module only

**Action**: Add `<PackageReference>` or `<AndroidMavenLibrary>` where needed

**Example**:
```xml
<!-- Only dd-sdk-android-okhttp needs OkHttp -->
<PackageReference Include="Square.OkHttp3" Version="4.12.0" />
```

**Why**: No other module needs it, so keep it local.

---

## Maven to NuGet Mapping

### The Version Problem

Maven POMs specify exact versions that may not support modern .NET:

**Maven POM**:
```xml
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib</artifactId>
  <version>2.0.21</version>  <!-- Only supports net8.0-android -->
</dependency>
```

**NuGet Solution**:
```xml
<PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.3.0.1" />
<!-- Supports net9.0-android + net10.0-android -->
```

### Package Name Mapping

| Maven Coordinate | NuGet Package |
|------------------|---------------|
| **Kotlin** |
| `org.jetbrains.kotlin:kotlin-stdlib` | `Xamarin.Kotlin.StdLib` |
| `org.jetbrains.kotlin:kotlin-reflect` | `Xamarin.Kotlin.Reflect` |
| **AndroidX** |
| `androidx.core:core` | `Xamarin.AndroidX.Core` |
| `androidx.collection:collection` | `Xamarin.AndroidX.Collection` |
| `androidx.fragment:fragment` | `Xamarin.AndroidX.Fragment` |
| **Google** |
| `com.google.code.gson:gson` | `GoogleGson` |
| **Square** |
| `com.squareup.okhttp3:okhttp` | `Square.OkHttp3` |
| **OpenTelemetry** |
| `io.opentelemetry:opentelemetry-api` | `OpenTelemetry.Api` |

### Finding Compatible Versions

Use the mapping script:
```bash
./scripts/map-maven-to-nuget.sh "org.jetbrains.kotlin:kotlin-stdlib" "2.0.21"

# Output:
# Xamarin.Kotlin.StdLib|2.3.0.1|⚠️ Upgraded for net9.0/net10.0 support
```

### Version Override Strategy

1. **Check Maven POM version** - What does upstream specify?
2. **Check framework support** - Does NuGet package support net9.0/net10.0?
3. **Find compatible version** - Use script or search NuGet.org
4. **Override in Directory.Packages.props**:
```xml
<!-- Override with compatible version -->
<PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.3.0.1" />
```
5. **Document why** - Add comment explaining override

---

## Implementation Guide

### Adding a New Binding Package

**Step 1**: Identify dependencies from POM file

```bash
./scripts/generate-android-dependencies.sh com.datadoghq:dd-sdk-android-newmodule:3.5.0
```

**Step 2**: Categorize each dependency

For each dependency, determine:
- Is it a Datadog module? → ProjectReference
- Is it shared? → Add to core
- Is it in MAUI? → AndroidIgnoredJavaDependency
- Is it unique? → PackageReference here

**Step 3**: Update .csproj

```xml
<ItemGroup>
  <!-- Project references (Datadog modules) -->
  <ProjectReference Include="../dd-sdk-android-core/dd-sdk-android-core.csproj" />

  <!-- Ignored (handled by core or MAUI) -->
  <AndroidIgnoredJavaDependency Include="com.google.code.gson:gson" />
  <AndroidIgnoredJavaDependency Include="androidx.core:core" />

  <!-- Unique to this module -->
  <PackageReference Include="Some.Unique.Package" Version="1.0.0" />
</ItemGroup>
```

**Step 4**: Build and test

```bash
dotnet build Datadog.MAUI.Android.Binding/dd-sdk-android-newmodule/dd-sdk-android-newmodule.csproj
```

---

## Version Updates

### Upgrading Datadog SDK

```bash
# 1. Update SDK version
./scripts/update-sdk-version.sh 3.6.0

# 2. Check for new dependencies
./scripts/generate-android-dependencies.sh com.datadoghq:dd-sdk-android-core:3.6.0

# 3. Check NuGet package updates
./scripts/check-nuget-versions.sh

# 4. Update Directory.Packages.props with new versions

# 5. Build all packages
./scripts/pack.sh
```

### Updating Shared Dependency

**Example: Upgrading Kotlin**

1. Find compatible version:
```bash
./scripts/map-maven-to-nuget.sh "org.jetbrains.kotlin:kotlin-stdlib" "2.1.0"
```

2. Update `Directory.Packages.props`:
```xml
<PackageVersion Include="Xamarin.Kotlin.StdLib" Version="2.4.0.1" />
```

3. Build and test all packages:
```bash
./scripts/pack.sh
```

---

## Troubleshooting

### Error: Duplicate class definitions

**Symptom**:
```
Duplicate class com.google.gson.Gson found in...
```

**Cause**: Same dependency included multiple times

**Fix**: Move to core, add `AndroidIgnoredJavaDependency` in consumers

---

### Error: Missing required class

**Symptom**:
```
error : java.lang.ClassNotFoundException: com.google.gson.Gson
```

**Cause**: Dependency not included anywhere

**Fix**: Add `<PackageReference>` to core

---

### Error: Package does not support target framework

**Symptom**:
```
Package Xamarin.Kotlin.StdLib 2.0.21 is not compatible with net10.0-android
```

**Cause**: Old NuGet package version

**Fix**: Find newer version supporting net9.0/net10.0:
```bash
./scripts/map-maven-to-nuget.sh "org.jetbrains.kotlin:kotlin-stdlib" "2.0.21"
```

---

### Error: Version conflict

**Symptom**:
```
NU1605: Detected package downgrade: GoogleGson from 2.11.0 to 2.10.1
```

**Cause**: Different versions specified in different projects

**Fix**: Use `Directory.Packages.props` for centralized version control:
```xml
<PackageVersion Include="GoogleGson" Version="2.11.0" />
```

---

## Key Insights

### 1. GoogleGson Must Use NuGet

**Never bind from Maven**. Gson has complex Java generics that fail with CS0534 errors. Always use the `GoogleGson` NuGet package.

### 2. Kotlin Versions Matter

Maven Kotlin versions often don't support net9.0/net10.0. Always check and upgrade to compatible NuGet versions.

### 3. MAUI Already Has AndroidX

Most common AndroidX packages (core, fragment, appcompat) are already in MAUI. Always add `AndroidIgnoredJavaDependency` for these.

### 4. ProjectReference Is Transitive

When you reference core, you automatically get its dependencies. This is the foundation of the centralized pattern.

### 5. Directory.Packages.props Is Your Friend

Centralize all version numbers here. Single source of truth prevents version conflicts.

---

## Related Documentation

- [Android Integration Packages]() - Adding optional packages
- [Scripts Overview]() - Build script details
- [Packaging Architecture]() - NuGet package structure

---

**Last Updated**: 2026-01-20
