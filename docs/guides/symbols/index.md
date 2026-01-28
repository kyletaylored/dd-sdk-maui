---
layout: default
title: Symbol Upload Plugin
nav_order: 4
parent: Guides
has_children: true
---

# Datadog.MAUI.Symbols Plugin

The **Datadog.MAUI.Symbols** plugin is an MSBuild SDK that automatically uploads debug symbols to Datadog during your build/publish process, enabling proper symbolication of crash reports and error stack traces.

## What It Does

This plugin integrates seamlessly into your .NET MAUI build pipeline to:

- **iOS**: Upload `.dSYM` files automatically
- **Android**: Upload Proguard/R8 `mapping.txt` files automatically
- **CI/CD**: Works in any build environment with Node.js installed
- **Zero-touch**: Runs automatically on `dotnet publish` in Release configuration

## Why You Need It

Without debug symbols, crash reports and error stack traces in Datadog show obfuscated code:

```
at a.b.c (Unknown Source:0)
at d.e.f (Unknown Source:0)
```

With symbols uploaded, you get readable stack traces:

```
at MyApp.MainActivity.OnCreate (MainActivity.cs:42)
at MyApp.Services.DataService.FetchData (DataService.cs:156)
```

## Quick Start

### 1. Install the Package

```bash
dotnet add package Datadog.MAUI.Symbols
```

### 2. Configure Your Project

Add to your `.csproj`:

```xml
<PropertyGroup>
  <!-- Required: Service name -->
  <DatadogServiceNameAndroid>com.example.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.example.app.ios</DatadogServiceNameiOS>

  <!-- Optional: Set via environment variable instead -->
  <!-- <DatadogApiKey>your-api-key</DatadogApiKey> -->
</PropertyGroup>
```

### 3. Set Your API Key

In your environment (recommended):

```bash
export DD_API_KEY="your-datadog-api-key"
```

### 4. Build and Publish

```bash
# Symbols are automatically uploaded during publish
dotnet publish -f net8.0-android -c Release
dotnet publish -f net8.0-ios -c Release
```

That's it! Your symbols are now automatically uploaded to Datadog.

## Features

- ✅ **Automatic Upload**: No manual steps required
- ✅ **Platform Detection**: Automatically detects iOS vs Android
- ✅ **Flexible Configuration**: Platform-specific or global service names
- ✅ **CI/CD Ready**: Environment variable support
- ✅ **Dry-run Mode**: Test without uploading
- ✅ **Smart Defaults**: Works with standard MAUI project structures

## How It Works

1. **Build Phase**: You run `dotnet publish -c Release`
2. **Symbol Detection**: Plugin locates `.dSYM` or `mapping.txt`
3. **Upload Phase**: Uses `datadog-ci` to upload symbols
4. **Completion**: Symbols available in Datadog within minutes

## Next Steps

<div class="code-example" markdown="1">

**New to the plugin?**
→ [Getting Started](getting-started.html)

**Ready to configure?**
→ [Configuration Guide](configuration.html)

**Setting up CI/CD?**
→ [CI/CD Integration](ci-cd.html)

**Having issues?**
→ [Troubleshooting](troubleshooting.html)

</div>

## Requirements

- **.NET MAUI** app (net6.0+)
- **Node.js & npm** (for `datadog-ci`)
- **Datadog API Key**
- **Android**: ProGuard/R8 enabled for Release builds
- **iOS**: Building with dSYM generation enabled

## Version Compatibility

| Plugin Version | .NET MAUI Versions | Notes |
|----------------|-------------------|-------|
| 1.0.x | net6.0+, net7.0+, net8.0+, net9.0+, net10.0+ | All versions supported |

The plugin uses `netstandard2.0` for MSBuild compatibility, making it work with all .NET versions.

## Support

- [GitHub Issues](https://github.com/DataDog/dd-sdk-maui/issues)
- [Datadog Support](https://docs.datadoghq.com/help/)
- [NuGet Package](https://www.nuget.org/packages/Datadog.MAUI.Symbols)
