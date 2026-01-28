---
layout: default
title: Integration Packages
parent: Android
nav_order: 2
permalink: /guides/android/integrations
---

# Android Integration Packages

Guide to optional Android integration packages for the Datadog MAUI SDK.

---

## Overview

The Datadog MAUI SDK provides **13 Android packages**:
- **2 Core packages**: Internal, Core (required)
- **7 Feature packages**: Logs, RUM, Trace, NDK, SessionReplay, WebView, Flags
- **4 Integration packages**: OkHttp, Trace.OpenTelemetry, OkHttp.OpenTelemetry, GradlePlugin ✨

Integration packages are **optional** and provide connectivity with popular Android libraries and frameworks.

---

## Available Integration Packages

### 1. Datadog.MAUI.Android.OkHttp

**NuGet Package**: `Datadog.MAUI.Android.OkHttp`
**Maven Source**: `com.datadoghq:dd-sdk-android-okhttp:3.5.0`

**Purpose**: Automatic HTTP request tracing and RUM integration for OkHttp clients

**When to use**:
- Your MAUI app makes HTTP requests using OkHttp
- You want automatic network monitoring
- You need distributed tracing for HTTP requests

**Dependencies**:
- Datadog.MAUI.Android.Core
- Datadog.MAUI.Android.Rum
- Datadog.MAUI.Android.Trace
- Square.OkHttp3

**Features**:
- Automatic network request instrumentation
- RUM resource tracking
- Distributed tracing headers injection
- Request/response interception
- Error tracking

**Usage** (from Android platform code):
```csharp
using Com.Datadog.Android.Okhttp;

// Create OkHttpClient with Datadog interceptor
var client = new OkHttpClient.Builder()
    .AddInterceptor(DatadogInterceptor.Create())
    .Build();
```

---

### 2. Datadog.MAUI.Android.Trace.OpenTelemetry

**NuGet Package**: `Datadog.MAUI.Android.Trace.OpenTelemetry`
**Maven Source**: `com.datadoghq:dd-sdk-android-trace-otel:3.5.0`

**Purpose**: Bridge between Datadog tracing and OpenTelemetry API

**When to use**:
- Your app uses OpenTelemetry instrumentation
- You want to send OTel traces to Datadog
- You need compatibility with OTel ecosystem

**Dependencies**:
- Datadog.MAUI.Android.Core
- Datadog.MAUI.Android.Trace
- io.opentelemetry:opentelemetry-api (1.44.1)
- io.opentelemetry:opentelemetry-context (1.44.1)

**Features**:
- OpenTelemetry API compatibility
- Span context propagation
- Trace correlation between Datadog and OTel systems
- Distributed context support

**Usage**:
```csharp
using Com.Datadog.Android.Trace.Otel;

// Configure OpenTelemetry to use Datadog
var tracer = OpenTelemetry.GetTracer("my-app");
var span = tracer.SpanBuilder("operation").StartSpan();
```

---

### 3. Datadog.MAUI.Android.OkHttp.OpenTelemetry

**NuGet Package**: `Datadog.MAUI.Android.OkHttp.OpenTelemetry`
**Maven Source**: `com.datadoghq:dd-sdk-android-okhttp-otel:3.5.0`

**Purpose**: OkHttp integration using OpenTelemetry API

**When to use**:
- You use both OkHttp and OpenTelemetry
- You want unified tracing for HTTP requests
- You need OTel-compatible network tracing

**Dependencies**:
- Datadog.MAUI.Android.Trace
- Datadog.MAUI.Android.OkHttp
- Datadog.MAUI.Android.Trace.OpenTelemetry
- Square.OkHttp3

**Features**:
- Combines OkHttp instrumentation with OpenTelemetry
- Unified tracing for OkHttp requests using OTel API
- Compatible with OpenTelemetry ecosystem
- Distributed tracing support

**Usage**:
```csharp
using Com.Datadog.Android.Okhttp.Otel;

// Create OkHttpClient with OTel tracing
var client = new OkHttpClient.Builder()
    .AddInterceptor(OtelTracingInterceptor.Create())
    .Build();
```

---

### 4. Datadog.MAUI.Android.GradlePlugin

**NuGet Package**: `Datadog.MAUI.Android.GradlePlugin`
**Maven Source**: `com.datadoghq:dd-sdk-android-gradle-plugin:1.9.0`

**Purpose**: Build-time plugin for source map upload and symbol mapping

**Note**: Uses separate versioning (1.9.0) from SDK (3.5.0)

**When to use**:
- You enable ProGuard/R8 obfuscation
- You need crash reports with unobfuscated stack traces
- You want to upload source maps automatically

**Dependencies**:
- org.json:json (20180813)
- Square.OkHttp3

**Features**:
- Source map upload for crash reporting
- Symbol mapping for obfuscated code
- Build-time integration
- Automatic mapping file upload

**Setup**:
This is a build-time tool, typically configured in Gradle. For MAUI apps, you may need custom MSBuild integration.

---

## Installation

### Add Individual Package

```bash
dotnet add package Datadog.MAUI.Android.OkHttp --version 3.5.0
```

### Add via PackageReference

```xml
<PackageReference Include="Datadog.MAUI.Android.OkHttp" Version="3.5.0" />
```

### Multiple Integrations

```xml
<ItemGroup>
  <!-- Core packages (required) -->
  <PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />

  <!-- Feature packages (as needed) -->
  <PackageReference Include="Datadog.MAUI.Android.Rum" Version="3.5.0" />
  <PackageReference Include="Datadog.MAUI.Android.Trace" Version="3.5.0" />

  <!-- Integration packages (optional) -->
  <PackageReference Include="Datadog.MAUI.Android.OkHttp" Version="3.5.0" />
  <PackageReference Include="Datadog.MAUI.Android.Trace.OpenTelemetry" Version="3.5.0" />
</ItemGroup>
```

---

## Implementation Details

### Opt-In Binding Approach

Following the **opt-in vs opt-out** philosophy, internal builder classes with complex generic signatures are removed from bindings:

**What was removed**:
```xml
<!-- Metadata.xml for dd-sdk-android-okhttp -->
<remove-node path="/api/package[@name='com.datadog.android.okhttp']/class[@name='DatadogInterceptor.Builder']" />
<remove-node path="/api/package[@name='com.datadog.android.okhttp.trace']/class[@name='TracingInterceptor.BaseBuilder']" />
<remove-node path="/api/package[@name='com.datadog.android.okhttp.trace']/class[@name='TracingInterceptor.Builder']" />
```

**Rationale**:
- Builder classes had Kotlin generic type signatures that conflicted with C# binding generator
- Users don't need direct access to builder patterns - they can use factory methods
- Cleaner API surface exposes only user-facing functionality

**User impact**:
Users construct interceptors using factory methods or direct constructors instead of builders. This is simpler and more idiomatic for C#.

---

## Build Information

### Multi-Framework Support

All integration packages support both target frameworks:
- ✅ net9.0-android
- ✅ net10.0-android

### Build Warnings (Expected)

These warnings are normal and don't affect functionality:

- **BG8605**: Missing Java types (Kotlin function types)
- **BG8606**: Some types could not be bound (intentionally removed)
- **BG8400/BG8401**: Kotlin Companion objects skipped

These warnings occur because we use the opt-in approach, intentionally excluding internal types.

---

## Package Selection Guide

### Do I need integration packages?

**You DON'T need integration packages if**:
- You only use standard .NET HTTP clients (HttpClient)
- You don't use OkHttp library
- You don't use OpenTelemetry
- You don't need build-time symbol upload

**You DO need integration packages if**:
- Your Android code uses OkHttp for networking
- You want OpenTelemetry compatibility
- You need distributed tracing across services
- You use ProGuard/R8 obfuscation

---

## Adding More Integration Packages

Want to add additional integration packages? Follow this process:

### 1. Check Available Packages

See [_reference/ANDROID_PACKAGES_ANALYSIS.md](../_reference/ANDROID_PACKAGES_ANALYSIS.html) for complete list of available Datadog Android SDK packages.

**High-priority candidates**:
- `dd-sdk-android-rum-coroutines` - Kotlin coroutines RUM support
- `dd-sdk-android-trace-coroutines` - Kotlin coroutines tracing
- `dd-sdk-android-compose` - Jetpack Compose instrumentation
- `dd-sdk-android-timber` - Timber logging integration

### 2. Create New Package

```bash
# Generate project structure
./scripts/setup-android-bindings.sh dd-sdk-android-newpackage 3.5.0

# Find dependencies
./scripts/generate-android-dependencies.sh com.datadoghq:dd-sdk-android-newpackage:3.5.0

# Map Maven dependencies to NuGet
./scripts/map-maven-to-nuget.sh "dependency:name" "version"
```

### 3. Handle Binding Issues

If you encounter build errors with complex types:

1. **Identify problematic classes**: Look for `error CS0534` (abstract member not implemented)
2. **Remove from bindings**: Add `<remove-node>` to Transforms/Metadata.xml
3. **Apply opt-in philosophy**: Only expose user-facing APIs
4. **Test functionality**: Ensure users can still accomplish their goals

### 4. Add to Build System

```bash
# Add to solution
# Edit Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.sln

# Add to pack script
# Edit scripts/pack.sh ANDROID_MODULES array

# Build and test
./scripts/pack.sh
```

### 5. Document Usage

Create package-specific documentation showing:
- When to use the package
- Dependencies required
- Code examples
- Common scenarios

---

## Build Commands

### Build Individual Package

```bash
# Build for specific target
dotnet build Datadog.MAUI.Android.Binding/dd-sdk-android-okhttp/dd-sdk-android-okhttp.csproj -f net9.0-android

# Build for both targets
dotnet build Datadog.MAUI.Android.Binding/dd-sdk-android-okhttp/dd-sdk-android-okhttp.csproj
```

### Build All Packages

```bash
# Using pack script (recommended)
./scripts/pack.sh

# Or build solution
dotnet build Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.sln
```

---

## Maven Coordinates Reference

For adding more packages:

```bash
# Check Maven Central for available packages
curl -s https://repo1.maven.org/maven2/com/datadoghq/ | grep -o 'href="dd-sdk-android-[^"]*"'

# Get POM file for dependencies
curl -s https://repo1.maven.org/maven2/com/datadoghq/dd-sdk-android-okhttp/3.5.0/dd-sdk-android-okhttp-3.5.0.pom

# Map to NuGet packages
./scripts/map-maven-to-nuget.sh "com.squareup.okhttp3:okhttp" "4.12.0"
```

---

## Related Documentation

- [SCRIPTS_OVERVIEW.md]() - Build scripts guide
- [maven-nuget-version-mapping.md](../../guides/android/dependencies) - Version compatibility
- [PACKAGING_ARCHITECTURE.md]() - Package structure
- [_reference/ANDROID_PACKAGES_ANALYSIS.md](../_reference/ANDROID_PACKAGES_ANALYSIS.html) - All available packages

---

## Success Metrics

✅ **All 4 packages building successfully**
✅ **Multi-framework support** (net9.0-android + net10.0-android)
✅ **Proper dependency references**
✅ **Integrated into meta-package**
✅ **Added to build scripts**
✅ **Maven→NuGet mappings updated**
✅ **Opt-in approach** (clean API surface)

---

## Support and Issues

If you encounter issues with integration packages:

1. Check [Build Information](#build-information) for expected warnings
2. Review [Troubleshooting](#troubleshooting) in scripts overview
3. Verify dependencies using `./scripts/map-maven-to-nuget.sh`
4. File an issue with build logs and package versions
