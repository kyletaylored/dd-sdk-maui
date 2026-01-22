---
layout: default
title: Packaging Architecture
nav_order: 11
---

# Packaging and Release Guide: Datadog .NET MAUI Bindings (Android + iOS)

This repository produces .NET MAUI bindings for the Datadog native SDKs. The deliverables are published as NuGet packages and are designed for stable consumption in MAUI applications targeting Android and iOS.

This document explains:

1. The packaging architecture and rationale
2. The role of `Directory.*` MSBuild files in versioning and builds
3. Local build and packaging steps (including local feed usage)
4. A recommended publish workflow to nuget.org

---

## 1. Packaging Architecture

### 1.1 Goals

- Provide a **single “installable” package** for MAUI consumers (`Datadog.MAUI`)
- Maintain **modularity** across features (Core, Logs, RUM, Trace, etc.)
- Avoid Android and iOS toolchain issues caused by building multiple binding projects together
- Keep dependency management predictable across `net9` and `net10` platform TFMs

---

## 2. Package Types

### 2.1 Module Binding Packages (platform-specific)

These are the “real” binding packages. Each module corresponds to a native SDK module and produces assemblies and platform assets (AAR/JAR for Android, frameworks for iOS).

Examples:

- Android:
  - `Datadog.MAUI.Android.Internal`
  - `Datadog.MAUI.Android.Core`
  - `Datadog.MAUI.Android.Logs`
  - `Datadog.MAUI.Android.Rum`
  - etc.

- iOS:
  - `Datadog.MAUI.iOS.Core`
  - `Datadog.MAUI.iOS.RUM`
  - etc.

These packages target platform TFMs such as:

- Android: `net9.0-android`, `net10.0-android`
- iOS: `net9.0-ios`, `net10.0-ios`

---

### 2.2 Platform “Meta” Packages (dependency-only)

These packages exist to provide a convenient dependency bundle per platform. They contain **no assemblies** and do not run Android/iOS compilation steps. They only declare dependencies on the module binding packages.

- `Datadog.MAUI.Android.Binding` (dependency-only)
- `Datadog.MAUI.iOS.Binding` (dependency-only)

Key properties of platform meta packages:

- Target a **non-platform framework** (e.g., `net8.0` or `netstandard2.0`)
- Set `IncludeBuildOutput=false`
- Use **PackageReference** (not ProjectReference) to depend on module packages

**Rationale:**
Building/aggregating multiple Android binding projects in a single Android compilation unit causes Java build failures (e.g., duplicate generated `R.java` classes from AndroidX dependencies). A dependency-only meta-package avoids this by not invoking Android compilation. Dependency resolution occurs at NuGet restore time rather than MSBuild build time.

---

### 2.3 Consumer Plugin Package (`Datadog.MAUI`)

`Datadog.MAUI` is the consumer-facing package intended to be installed by MAUI applications. It multi-targets platform TFMs and conditionally references the platform meta packages.

- Targets: `net9.0-android`, `net10.0-android`, `net9.0-ios`, `net10.0-ios`
- Uses conditional `PackageReference`:
  - Android TFMs reference `Datadog.MAUI.Android.Binding`
  - iOS TFMs reference `Datadog.MAUI.iOS.Binding`

**Rationale:**
This mirrors standard MAUI plugin packaging patterns: the plugin package is the single point of entry, while platform-specific native dependencies are pulled in only for the relevant TFMs.

---

## 3. MSBuild Repository Files and Their Roles

### 3.1 `Directory.Packages.props` (NuGet Central Package Management)

- Declares centrally-managed NuGet versions via `<PackageVersion />`
- Keeps AndroidX/Kotlin/etc. aligned across projects
- Prevents inconsistent transitive graphs across sibling binding projects

Rule: With CPM enabled, any direct `<PackageReference Include="X" />` must have a matching `<PackageVersion Include="X" Version="..." />`.

---

### 3.2 `Directory.Build.props` (shared build properties)

- Defines shared MSBuild properties such as:
  - `DatadogSdkVersion`
  - `Version` (package version)

- Can define repository-wide defaults (nullable, warnings, etc.)

**Recommended pattern:**

- Maintain `DatadogSdkVersion` once
- Set `Version` to `$(DatadogSdkVersion)` so all packages produce consistent versions

---

### 3.3 `Directory.Build.targets` (shared build items and targets)

- Centralizes `<AndroidIgnoredJavaDependency />` entries that are present in AAR metadata but not required at runtime (JUnit, Mockito, etc.)
- Avoids duplicate declarations across projects
- (Optionally) can include dedupe targets to prevent duplicate-key failures

---

## 4. Java Dependency Verification (JDV)

For certain binding projects, Java dependency verification is disabled per AAR using:

```xml
VerifyDependencies="false"
```

This avoids known fragility with dependency verification in complex binding graphs (especially when used in combination with centralized versioning and imported build logic). Disabling verification removes one automated check; therefore, correctness should be validated via:

- sample app builds
- runtime smoke tests
- CI integration builds prior to publishing

---

## 5. Versioning Policy

All produced packages should share a single version, defined at the repository root.

Recommended in `Directory.Build.props`:

```xml
<Project>
  <PropertyGroup>
    <DatadogSdkVersion>3.5.0</DatadogSdkVersion>
    <Version>$(DatadogSdkVersion)</Version>
  </PropertyGroup>
</Project>
```

For pre-release builds:

```xml
<PropertyGroup>
  <DatadogSdkVersion>3.5.0</DatadogSdkVersion>
  <Version>$(DatadogSdkVersion)-beta.1</Version>
</PropertyGroup>
```

---

## 6. Build and Pack (Local)

### 6.1 Clean and Restore

From repository root:

```bash
dotnet clean
dotnet restore
```

---

### 6.2 Pack to a Local Output Folder

Use a local folder feed (recommended):

```bash
rm -rf ./artifacts
mkdir -p ./artifacts
```

#### Step A: Pack all module binding packages (Android + iOS)

Example (repeat for each project):

```bash
dotnet pack dd-sdk-android-internal/dd-sdk-android-internal.csproj -c Release -o ./artifacts
dotnet pack dd-sdk-android-core/dd-sdk-android-core.csproj -c Release -o ./artifacts
# ... pack all Android module projects ...

dotnet pack DatadogInternal/DatadogInternal.csproj -c Release -o ./artifacts
dotnet pack DatadogCore/DatadogCore.csproj -c Release -o ./artifacts
# ... pack all iOS module projects ...
```

#### Step B: Pack platform meta packages

```bash
dotnet pack Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj -c Release -o ./artifacts --source ./artifacts
dotnet pack Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj -c Release -o ./artifacts --source ./artifacts
```

#### Step C: Pack consumer plugin package

```bash
dotnet pack Datadog.MAUI/Datadog.MAUI.csproj -c Release -o ./artifacts --source ./artifacts
```

---

### 6.3 Verify output

```bash
ls ./artifacts
```

You should see `.nupkg` files for:

- module packages
- platform meta packages
- `Datadog.MAUI` (consumer plugin)

---

## 7. Publishing to nuget.org

### 7.1 Preconditions

- You must have a NuGet API key with permission to publish under the package IDs
- You should run a final verification build and/or sample app integration test before publishing

---

### 7.2 Recommended publish order

Publish in dependency order:

1. Module packages (platform binding packages)
2. Platform meta packages
3. Consumer plugin package (`Datadog.MAUI`)

This ensures that when meta/plugin packages are uploaded, their dependencies already exist on nuget.org.

---

### 7.3 Publish commands

Set API key:

```bash
export NUGET_API_KEY="..."
```

Push packages:

```bash
# 1) Push all module packages first
dotnet nuget push ./artifacts/Datadog.MAUI.Android.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
dotnet nuget push ./artifacts/Datadog.MAUI.iOS.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json

# 2) Push platform meta packages
dotnet nuget push ./artifacts/Datadog.MAUI.Android.Binding.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
dotnet nuget push ./artifacts/Datadog.MAUI.iOS.Binding.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json

# 3) Push consumer plugin package last
dotnet nuget push ./artifacts/Datadog.MAUI.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
```

Recommended flags:

- Add `--skip-duplicate` for CI re-runs
- Consider publishing to a staging feed first (Azure Artifacts/GitHub Packages) before nuget.org

---

## 8. Consumer Usage

MAUI app developers should install only:

- `Datadog.MAUI`

NuGet will resolve platform dependencies automatically based on the target frameworks.

---

## 9. Common Failure Modes

### 9.1 Duplicate Java classes (Android `R.java` duplicates)

Cause: building multiple binding projects together in one Android compilation unit (usually via `ProjectReference` aggregation).

Fix: ensure platform meta packages are dependency-only and use `PackageReference`, not `ProjectReference`, and ensure the plugin references packages, not projects.

---

### 9.2 Incorrect package version (defaults to 1.0.0)

Cause: no `Version`/`PackageVersion` set.

Fix: set version once in `Directory.Build.props`:

```xml
<Version>$(DatadogSdkVersion)</Version>
```

---

## 10. Summary

This repository intentionally separates:

- **module binding packages** (real bindings)
- **platform meta packages** (dependency-only)
- **plugin package** (single install for consumers)

This design avoids platform toolchain conflicts, improves maintainability, and provides a clean developer experience for MAUI applications.
