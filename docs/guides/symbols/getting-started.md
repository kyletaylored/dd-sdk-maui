---
layout: default
title: Getting Started
nav_order: 1
parent: Symbol Upload Plugin
grand_parent: Guides
---

# Getting Started with Datadog.MAUI.Symbols

This guide walks you through installing and using the Datadog.MAUI.Symbols plugin for the first time.

## Prerequisites

Before you begin, ensure you have:

1. **.NET MAUI Application** (.NET 6 or later)
2. **Node.js and npm** installed ([Download](https://nodejs.org/))
3. **Datadog Account** with an API key
4. **Release Configuration** set up for your app

### Verify Node.js Installation

```bash
node --version  # Should show v14+ or higher
npm --version   # Should show v6+ or higher
```

If not installed, download from [nodejs.org](https://nodejs.org/).

## Installation

### Step 1: Install the NuGet Package

Add the package to your MAUI project:

```bash
dotnet add package Datadog.MAUI.Symbols
```

Or manually add to your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Datadog.MAUI.Symbols" Version="1.0.0" />
</ItemGroup>
```

### Step 2: Get Your Datadog API Key

1. Log in to [Datadog](https://app.datadoghq.com/)
2. Go to **Organization Settings** → **API Keys**
3. Create a new API key or copy an existing one
4. Save it securely - you'll need it for configuration

## Basic Configuration

### Option 1: Environment Variable (Recommended)

Set your API key as an environment variable:

**macOS/Linux:**
```bash
export DD_API_KEY="your-api-key-here"
```

**Windows (PowerShell):**
```powershell
$env:DD_API_KEY="your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set DD_API_KEY=your-api-key-here
```

### Option 2: In Project File

Add to your `.csproj` (not recommended for production):

```xml
<PropertyGroup>
  <DatadogApiKey>your-api-key-here</DatadogApiKey>
</PropertyGroup>
```

{: .warning }
> Don't commit API keys to source control! Use environment variables or CI/CD secrets.

### Step 3: Configure Service Names

Add service name configuration to your `.csproj`:

```xml
<PropertyGroup>
  <!-- Use your app's bundle identifier -->
  <DatadogServiceNameAndroid>com.yourcompany.yourapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.yourcompany.yourapp.ios</DatadogServiceNameiOS>
</PropertyGroup>
```

{: .note }
The service name should match what you configured in the Datadog RUM SDK initialization.

## Platform-Specific Setup

### Android Setup

Enable ProGuard/R8 for Release builds. Add to your `.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidLinkTool>r8</AndroidLinkTool>
  <AndroidEnableProguard>true</AndroidEnableProguard>
</PropertyGroup>
```

This generates the `mapping.txt` file needed for Android symbol upload.

### iOS Setup

dSYM files are automatically generated during Release builds. No additional configuration needed!

## First Upload Test

### Test with Dry Run

Before doing a real upload, test with dry-run mode:

```xml
<PropertyGroup>
  <DatadogDryRun>true</DatadogDryRun>
</PropertyGroup>
```

Then publish:

```bash
dotnet publish -f net8.0-android -c Release
```

You should see output like:

```
[Datadog] Uploading android symbols to Datadog...
[Datadog] Command: npx @datadog/datadog-ci flutter-symbols upload --dry-run ...
[Datadog] Successfully uploaded android symbols.
```

### Real Upload

Remove the dry-run setting (or set to `false`) and publish again:

```bash
# Make sure API key is set
export DD_API_KEY="your-api-key"

# Publish for Android
dotnet publish -f net8.0-android -c Release

# Publish for iOS
dotnet publish -f net8.0-ios -c Release
```

## Verify Upload

### Check Build Output

Look for success messages in your build output:

```
[Datadog] Uploading android symbols to Datadog...
[Datadog] Successfully uploaded android symbols.
```

### Check Datadog Dashboard

1. Go to **APM** → **Error Tracking** or **RUM** → **Errors**
2. Navigate to **Settings** → **Symbol Files**
3. You should see your uploaded symbols listed by service name and version

## Complete Example

Here's a complete `.csproj` configuration:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net8.0-android;net8.0-ios</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <UseMaui>true</UseMaui>
    <ApplicationId>com.yourcompany.yourapp</ApplicationId>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>

    <!-- Datadog Symbols Configuration -->
    <DatadogServiceNameAndroid>com.yourcompany.yourapp.android</DatadogServiceNameAndroid>
    <DatadogServiceNameiOS>com.yourcompany.yourapp.ios</DatadogServiceNameiOS>
    <!-- DatadogApiKey set via DD_API_KEY environment variable -->
  </PropertyGroup>

  <!-- Android Release Configuration -->
  <PropertyGroup Condition="'$(Configuration)' == 'Release' and '$(TargetFramework)' == 'net8.0-android'">
    <AndroidLinkTool>r8</AndroidLinkTool>
    <AndroidEnableProguard>true</AndroidEnableProguard>
  </PropertyGroup>

  <ItemGroup>
    <!-- Datadog RUM SDK -->
    <PackageReference Include="Datadog.MAUI" Version="3.5.0" />

    <!-- Datadog Symbols Plugin -->
    <PackageReference Include="Datadog.MAUI.Symbols" Version="1.0.0" />
  </ItemGroup>

</Project>
```

## Next Steps

Now that you have basic symbol upload working:

- **[Configuration Guide](configuration.html)** - Learn about all available options
- **[CI/CD Integration](ci-cd.html)** - Set up automated uploads
- **[Troubleshooting](troubleshooting.html)** - Common issues and solutions

## Common First-Time Issues

### "npx: command not found"

**Problem**: Node.js is not installed or not in PATH.

**Solution**: Install Node.js from [nodejs.org](https://nodejs.org/) and restart your terminal.

### "mapping.txt not found" (Android)

**Problem**: ProGuard/R8 is not enabled.

**Solution**: Add the Android Release configuration shown above.

### "No service name configured"

**Problem**: Missing service name properties.

**Solution**: Add `DatadogServiceNameAndroid` and `DatadogServiceNameiOS` to your `.csproj`.

### Upload succeeds but symbols don't work

**Problem**: Service name mismatch between RUM SDK and symbols.

**Solution**: Ensure the service name in symbols config matches your RUM SDK initialization exactly.

## Getting Help

If you're stuck:

1. Check the [Troubleshooting Guide](troubleshooting.html)
2. Review the [Configuration Reference](configuration.html)
3. [Open an issue](https://github.com/DataDog/dd-sdk-maui/issues) on GitHub
