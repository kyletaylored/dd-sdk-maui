---
layout: default
title: Getting Started
nav_order: 3
---

---
layout: default
title: Getting Started
nav_order: 2
description: "Guide to developing and building the Datadog SDK for .NET MAUI"
permalink: /getting-started
---

# Getting Started with Datadog MAUI SDK Development

This guide will help you get started with developing and building the Datadog SDK for .NET MAUI.

## Prerequisites

Ensure you have the following installed:

- **macOS** (required for iOS development)
- **.NET 8+ SDK**: [Download](https://dotnet.microsoft.com/download)
- **Xcode 14+**: Install from App Store
- **Android SDK**: Installed via Visual Studio or standalone
- **Git**: For version control

Optional but recommended:
- **Visual Studio 2022 for Mac** or **VS Code** with C# extensions
- **Objective Sharpie**: `brew install objectivesharpie` (for iOS binding generation)
- **PowerShell 7+**: `brew install powershell` (for PowerShell scripts)

## Quick Start (5 minutes)

### 1. Clone and Setup

```bash
# Clone the repository
git clone https://github.com/DataDog/dd-sdk-maui.git
cd dd-sdk-maui

# Create artifacts directory
mkdir -p artifacts/packages

# Make scripts executable
chmod +x scripts/*.sh
```

### 2. Download Native Frameworks

```bash
# Download iOS XCFrameworks (macOS only)
./scripts/download-ios-frameworks.sh

# This will:
# - Fetch the latest Datadog iOS SDK release
# - Download all XCFrameworks
# - Extract them to Datadog.MAUI.iOS.Binding/
```

The script will download approximately 50-100 MB of frameworks.

### 3. Build the Project

```bash
# Build in Debug mode
./scripts/build.sh

# Or build in Release mode (creates NuGet packages)
./scripts/build.sh Release
```

## Detailed Setup

### iOS Binding Development

#### Step 1: Download Frameworks

```bash
# Download specific version
./scripts/download-ios-frameworks.sh 3.5.0

# Or use PowerShell
pwsh scripts/download-ios-frameworks.ps1 -Version 3.5.0
```

#### Step 2: Generate Bindings with Objective Sharpie

Install Objective Sharpie if you haven't already:
```bash
brew install objectivesharpie
```

Generate bindings for DatadogCore:
```bash
cd Datadog.MAUI.iOS.Binding

sharpie bind \
  --output=Generated/Core \
  --namespace=DatadogMaui.iOS \
  --sdk=iphoneos17.0 \
  DatadogCore.xcframework/ios-arm64/DatadogCore.framework/Headers/DatadogCore.h

# Review the generated files
ls -la Generated/Core/
```

Repeat for other frameworks (RUM, Logs, Trace, etc.).

#### Step 3: Integrate Generated Bindings

1. Review generated `ApiDefinition.cs` and `StructsAndEnums.cs`
2. Copy relevant definitions to the main binding files
3. Fix any warnings or errors
4. Remove platform-specific attributes that don't apply

#### Step 4: Build iOS Binding

```bash
dotnet build Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj -c Debug
```

### Android Binding Development

#### Step 1: Understand Dependencies

```bash
# Analyze Android dependencies
pwsh scripts/update-android-dependencies.ps1 -Version 3.5.0 -OutputFile deps.json

# Or download verification metadata
pwsh scripts/download-android-artifacts.ps1 -Version 3.5.0 -ParseArtifacts
```

This will show you all Maven dependencies that will be downloaded.

#### Step 2: Build Android Binding

```bash
dotnet build Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj -c Debug
```

The first build will:
- Download all Maven dependencies from Maven Central
- Generate C# bindings for Android libraries
- This may take 5-10 minutes

#### Step 3: Fix Binding Errors (if any)

If you encounter binding errors, fix them in `Transforms/Metadata.xml`:

```xml
<metadata>
  <!-- Example: Remove a problematic class -->
  <remove-node path="/api/package[@name='com.example']/class[@name='ProblematicClass']" />

  <!-- Example: Rename a method -->
  <attr path="/api/package[@name='com.example']/class[@name='MyClass']/method[@name='oldName']"
        name="managedName">NewName</attr>
</metadata>
```

Then rebuild:
```bash
dotnet build Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj -c Debug
```

### Plugin Development

#### Step 1: Implement Platform-Specific Code

Edit the implementation files:

**iOS**: `Datadog.MAUI.Plugin/Platforms/iOS/DatadogSdkImplementation.cs`
```csharp
public void Initialize(DatadogConfiguration configuration)
{
    // TODO: Replace with actual iOS SDK initialization
    // Example:
    // var config = new DDConfiguration(
    //     clientToken: configuration.ClientToken,
    //     env: configuration.Environment
    // );
    // DDDatadog.Initialize(config);
}
```

**Android**: `Datadog.MAUI.Plugin/Platforms/Android/DatadogSdkImplementation.cs`
```csharp
public void Initialize(DatadogConfiguration configuration)
{
    // TODO: Replace with actual Android SDK initialization
    // Example:
    // var credentials = new Credentials(...);
    // var config = new Configuration.Builder(...).Build();
    // Datadog.Initialize(Android.App.Application.Context, credentials, config);
}
```

#### Step 2: Build the Plugin

```bash
dotnet build Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj -c Debug
```

### Testing with Sample App

#### Step 1: Configure Sample App

Edit `samples/DatadogMauiSample/MauiProgram.cs` and uncomment the initialization:

```csharp
DatadogSdk.Initialize(new DatadogConfiguration
{
    ClientToken = "YOUR_ACTUAL_CLIENT_TOKEN",
    Environment = "dev",
    ApplicationId = "YOUR_ACTUAL_APP_ID",
    ServiceName = "datadog-maui-sample",
    Site = DatadogSite.US1,
});
```

Get your credentials from [Datadog](https://app.datadoghq.com/).

#### Step 2: Run Sample App

**iOS Simulator:**
```bash
dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj -f net9.0-ios -c Debug
# Then open in Xcode or use Visual Studio to run
```

**Android Emulator:**
```bash
dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj -f net9.0-android -c Debug
# Then deploy to emulator or device
```

## Common Tasks

### Update to New Datadog SDK Version

```bash
# 1. Update version in Directory.Build.props
sed -i '' 's/<DatadogSdkVersion>3.5.0<\/DatadogSdkVersion>/<DatadogSdkVersion>3.6.0<\/DatadogSdkVersion>/' Directory.Build.props

# 2. Download new frameworks
./scripts/download-ios-frameworks.sh 3.6.0

# 3. Rebuild everything
./scripts/build.sh
```

### Create NuGet Packages

```bash
# Build in Release mode
./scripts/build.sh Release

# Packages will be in artifacts/packages/
ls -lh artifacts/packages/*.nupkg
```

### Test Packages Locally

In your test project, add a NuGet.config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="local-dd-sdk" value="/path/to/dd-sdk-maui/artifacts/packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Then reference the package:
```bash
dotnet add package Datadog.MAUI --version 3.5.0
```

### Clean Build

```bash
# Clean all build artifacts
dotnet clean Datadog.MAUI.sln -c Debug
dotnet clean Datadog.MAUI.sln -c Release

# Remove downloaded frameworks (to start fresh)
rm -rf Datadog.MAUI.iOS.Binding/*.xcframework

# Remove packages
rm -rf artifacts/packages/*
```

### Run in CI/CD

The project includes a GitHub Actions workflow that automatically:
- Builds iOS and Android bindings
- Creates NuGet packages
- Runs on every push/PR

See `.github/workflows/build.yml` for details.

## Troubleshooting

### iOS Build Issues

**Problem**: "Framework not found DatadogCore"
```bash
# Solution: Download frameworks
./scripts/download-ios-frameworks.sh
```

**Problem**: "Binding generation failed"
```bash
# Solution: Install Objective Sharpie
brew install objectivesharpie

# Verify installation
sharpie --version
```

**Problem**: "Unsupported Xcode version"
```bash
# Solution: Update Xcode
# Check version
xcodebuild -version

# Should be 14.0 or later
```

### Android Build Issues

**Problem**: "Could not resolve com.datadoghq:dd-sdk-android-core"
```bash
# Solution: Check internet connection and Maven Central access
curl -I https://repo1.maven.org/maven2/

# Try cleaning and rebuilding
dotnet clean && dotnet build
```

**Problem**: "Java compilation failed"
```bash
# Solution: Ensure Java 17 is installed
java -version

# Install via Homebrew if needed
brew install openjdk@17
```

**Problem**: "Binding errors during build"
```bash
# Solution: Add metadata transforms
# Edit: Datadog.MAUI.Android.Binding/Transforms/Metadata.xml
# See CONTRIBUTING.md for examples
```

### General Issues

**Problem**: "SDK not initialized"
```csharp
// Solution: Call Initialize before using SDK
DatadogSdk.Initialize(new DatadogConfiguration { ... });
```

**Problem**: "Platform not supported"
```bash
# Solution: Ensure you're building for iOS or Android
dotnet build -f net9.0-ios
# or
dotnet build -f net9.0-android
```

## Next Steps

1. **Review Architecture**: Read [PROJECT_OVERVIEW.md](../project/PROJECT_GUIDE.html)
2. **Implement Features**: Start with core initialization
3. **Write Tests**: Add unit and integration tests
4. **Contribute**: See [CONTRIBUTING.md]()
5. **Ask Questions**: Open GitHub issues or discussions

## Useful Commands

```bash
# List all available .NET SDKs
dotnet --list-sdks

# List all available MAUI workloads
dotnet workload list

# Install MAUI workload if needed
dotnet workload install maui

# Check project structure
tree -L 3 -I 'bin|obj'

# Search for TODOs
grep -r "TODO" --include="*.cs"

# Count lines of code
find . -name "*.cs" -not -path "*/bin/*" -not -path "*/obj/*" | xargs wc -l

# View recent changes
git log --oneline --graph --decorate --all -10
```

## Resources

### Documentation
- [Documentation Index](../index.html)
- [Project Overview](../project/PROJECT_GUIDE.html)
- [Contributing Guide]()
- [Changelog]()

### External Resources
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [Datadog Documentation](https://docs.datadoghq.com/)
- [Datadog iOS SDK](https://github.com/DataDog/dd-sdk-ios)
- [Datadog Android SDK](https://github.com/DataDog/dd-sdk-android)

### Community
- [GitHub Issues](https://github.com/DataDog/dd-sdk-maui/issues)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/datadog)
- [Datadog Community](https://datadoghq.com/community/)

---

**Happy Coding!** 🚀

If you run into any issues, please check [CONTRIBUTING.md]() or open an issue on GitHub.
