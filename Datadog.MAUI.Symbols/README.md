# Datadog.MAUI.Symbols

An MSBuild SDK for .NET MAUI that automatically uploads debug symbols (iOS dSYMs and Android Proguard mapping files) to Datadog during the build/publish pipeline.

## Features

- **Automatic Uploads**: Triggers automatically during `dotnet publish` or `Release` builds
- **Platform Specificity**: Handles `mapping.txt` for Android and `.dSYM` for iOS
- **Granular Configuration**: Supports distinct Service Names for iOS vs. Android
- **CI/CD Friendly**: Reads configuration from MSBuild properties or Environment Variables

## Prerequisites

- **Node.js and npm**: Required to run `npx @datadog/datadog-ci`
- **Datadog API Key**: Must be set via `DD_API_KEY` environment variable or `DatadogApiKey` MSBuild property
- **Android**: ProGuard/R8 must be enabled for Release builds to generate `mapping.txt`

## Installation

Add the package to your .NET MAUI project:

```bash
dotnet add package Datadog.MAUI.Symbols
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Symbols" Version="1.0.0" />
```

## Quick Start

### Minimal Configuration

The simplest setup requires only API key and service name:

```xml
<PropertyGroup>
  <!-- Required: Service Name for both platforms -->
  <DatadogServiceName>com.company.myapp</DatadogServiceName>
</PropertyGroup>
```

Set your API key as an environment variable:

```bash
export DD_API_KEY=your-datadog-api-key
dotnet publish -f net9.0-android -c Release
```

Symbol files will automatically upload using:
- **Version**: Your app's `ApplicationDisplayVersion` (e.g., `1.0`)
- **Flavor**: `release` (or `debug` for Debug builds)
- **Service**: The service name you configured

### Recommended Configuration for Production

For production apps with multiple build variants (production, staging, development):

```xml
<PropertyGroup>
  <!-- Platform-specific service names -->
  <DatadogServiceNameAndroid>com.company.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.company.myapp.ios</DatadogServiceNameiOS>

  <!-- Datadog site (if not using US1) -->
  <DatadogSite>datadoghq.com</DatadogSite>

  <!-- Read flavor from environment variable -->
  <DatadogFlavor Condition="'$(DD_BUILD_FLAVOR)' != ''">$(DD_BUILD_FLAVOR)</DatadogFlavor>
</PropertyGroup>
```

Then differentiate builds using the `DD_BUILD_FLAVOR` environment variable:

```bash
# Production release
export DD_BUILD_FLAVOR=production
dotnet publish -f net9.0-android -c Release

# Staging release
export DD_BUILD_FLAVOR=staging
dotnet publish -f net9.0-android -c Release

# Developer builds (each dev gets unique symbols)
export DD_BUILD_FLAVOR=dev-kyle
dotnet publish -f net9.0-android -c Release
```

### Complete Configuration Options

```xml
<PropertyGroup>
  <!-- Required: Datadog API Key (or set DD_API_KEY environment variable) -->
  <DatadogApiKey>your-api-key</DatadogApiKey>

  <!-- Required: Service Name (global fallback) -->
  <DatadogServiceName>com.company.app</DatadogServiceName>

  <!-- Optional: Platform-specific service names (override global) -->
  <DatadogServiceNameAndroid>com.company.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.company.app.ios</DatadogServiceNameiOS>

  <!-- Optional: App Version (defaults to ApplicationDisplayVersion) -->
  <!-- Note: Should match version reported by RUM SDK for symbolication to work -->
  <DatadogAppVersion>1.2.3</DatadogAppVersion>

  <!-- Optional: Build Flavor/Variant -->
  <!-- Defaults to "debug" or "release" based on Configuration -->
  <!-- Read from DD_BUILD_FLAVOR environment variable if set -->
  <DatadogFlavor Condition="'$(DD_BUILD_FLAVOR)' != ''">$(DD_BUILD_FLAVOR)</DatadogFlavor>

  <!-- Optional: Datadog Site (defaults to datadoghq.com) -->
  <DatadogSite>datadoghq.com</DatadogSite>

  <!-- Optional: Dry run mode - simulates upload without sending data -->
  <DatadogDryRun>false</DatadogDryRun>

  <!-- Optional: Disable automatic upload entirely -->
  <DatadogUploadEnabled>true</DatadogUploadEnabled>

  <!-- Optional: Enable upload in Debug configuration (disabled by default) -->
  <DatadogUploadInDebug>false</DatadogUploadInDebug>
</PropertyGroup>
```

### Configuration Reference

| Property | Required | Default | Description |
|----------|----------|---------|-------------|
| `DatadogApiKey` | Yes* | - | Datadog API key for authentication. Can also be set via `DD_API_KEY` environment variable. |
| `DatadogServiceName` | Yes* | - | Global service name used for both platforms unless overridden. |
| `DatadogServiceNameAndroid` | No | - | Android-specific service name. Overrides `DatadogServiceName` for Android builds. |
| `DatadogServiceNameiOS` | No | - | iOS-specific service name. Overrides `DatadogServiceName` for iOS builds. |
| `DatadogAppVersion` | No | `ApplicationDisplayVersion` or `1.0.0` | Version string for the uploaded symbols. **Important**: Must match the version reported by your RUM SDK for crash symbolication to work. We recommend omitting this property to automatically use `ApplicationDisplayVersion`. |
| `DatadogFlavor` | No | `debug` or `release` (based on Configuration) | Build flavor/variant. Combined with service and version to create unique symbol upload. Use `DD_BUILD_FLAVOR` environment variable to differentiate builds (e.g., `production`, `staging`, `dev-username`). |
| `DatadogSite` | No | `datadoghq.com` | Datadog site for your organization. Can also be set via `DD_SITE` environment variable. |
| `DatadogDryRun` | No | `false` | When `true`, simulates the upload without sending data. Useful for testing. |
| `DatadogUploadEnabled` | No | `true` | Set to `false` to completely disable symbol uploads. |
| `DatadogUploadInDebug` | No | `false` | Set to `true` to enable uploads in Debug configuration (by default only runs in Release). |

\* Required unless overridden by platform-specific properties or environment variables.

### Service Name Hierarchy

The plugin determines the service name using the following precedence:

1. **Platform Specific**: `DatadogServiceNameAndroid` or `DatadogServiceNameiOS`
2. **Global**: `DatadogServiceName`
3. **Error**: If none are set, the build will fail

### Android Setup

For Android, ensure ProGuard/R8 is enabled in your Release configuration:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidLinkTool>r8</AndroidLinkTool>
  <AndroidEnableProguard>true</AndroidEnableProguard>
</PropertyGroup>
```

## Usage

Once configured, the plugin automatically runs during the `Publish` target:

```bash
# Publish for Android (generates mapping.txt with R8 enabled)
dotnet publish -f net9.0-android -c Release

# Publish for iOS (generates .dSYM folder)
dotnet publish -f net9.0-ios -c Release
```

The plugin will:

1. Detect the platform (Android/iOS)
2. Locate the debug symbols (`mapping.txt` or `.dSYM`)
3. Upload them to Datadog using `npx @datadog/datadog-ci`

### Viewing Upload Output

The symbol upload output appears mid-build, not at the end. To view only Datadog-related messages:

```bash
# Filter output to show only Datadog messages
dotnet publish -f net9.0-android -c Release 2>&1 | grep "\[Datadog\]"
```

Expected output:
```
[Datadog] Symbol upload target triggered for android
[Datadog] Found Android mapping file: /path/to/mapping.txt
[Datadog] Uploading android symbols to Datadog...
[Datadog]   Service: com.company.app.android
[Datadog]   Version: 1.0.0
[Datadog]   Variant: release
[Datadog] datadog-ci output:
  ℹ DRY RUN MODE ENABLED. WILL NOT UPLOAD SYMBOLS
  ✅ Uploaded 1 mapping file to Datadog
```

## Environment Variables

Instead of hardcoding the API key in your `.csproj`, you can use environment variables:

```bash
export DD_API_KEY="your-api-key"
export DD_SITE="us5.datadoghq.com"
```

## Local Development with ProjectReference

If you're developing the Symbols package locally and want to test it without creating NuGet packages:

```xml
<!-- In your test app's .csproj -->
<ItemGroup>
  <ProjectReference Include="..\..\Datadog.MAUI.Symbols\Datadog.MAUI.Symbols.csproj" ReferenceOutputAssembly="false" />
</ItemGroup>

<!-- Manually load the MSBuild task from local build output -->
<UsingTask TaskName="Datadog.MAUI.Symbols.UploadSymbolsTask"
           AssemblyFile="$(MSBuildProjectDirectory)\..\..\Datadog.MAUI.Symbols\bin\$(Configuration)\netstandard2.0\Datadog.MAUI.Symbols.dll" />

<!-- Import the .targets file -->
<Import Project="..\..\Datadog.MAUI.Symbols\build\Datadog.MAUI.Symbols.targets" />
```

This setup automatically rebuilds the Symbols package when you run `dotnet publish` on your test app.

## Troubleshooting

### "npx not found" Error

Ensure Node.js and npm are installed and available in your PATH.

```bash
# Verify installation
npx --version
```

If not installed, download from [nodejs.org](https://nodejs.org/).

### "mapping.txt not found" (Android)

Verify that ProGuard/R8 is enabled for Release builds. The mapping file is only generated when code shrinking is enabled.

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidEnableR8>true</AndroidEnableR8>
  <AndroidLinkTool>r8</AndroidLinkTool>
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
</PropertyGroup>
```

The plugin searches these locations in order:
1. `$(OutputPath)\mapping.txt`
2. `$(OutputPath)\..\mapping.txt`
3. `$(IntermediateOutputPath)\mapping.txt`
4. `$(ProjectDir)\obj\$(Configuration)\$(TargetFramework)\android-arm64\mapping.txt`

### "dSYM folder not found" (iOS)

The dSYM folder is generated during Release builds with debug symbol generation enabled.

The plugin searches these locations:
1. `$(OutputPath)\$(AssemblyName).app.dSYM`
2. `$(OutputPath)\..\$(AssemblyName).app.dSYM`
3. `$(AppBundleDir).dSYM`

### No Output Visible

Symbol upload messages appear mid-build, not at the end. Use grep to filter:

```bash
dotnet publish -f net9.0-android -c Release 2>&1 | grep "\[Datadog\]"
```

### Upload Runs in Debug Configuration

By default, symbol upload only runs in Release configuration. To test in Debug:

```xml
<DatadogUploadInDebug>true</DatadogUploadInDebug>
```

### Testing Without Uploading

Use dry-run mode to test the configuration without uploading symbols:

```xml
<DatadogDryRun>true</DatadogDryRun>
```

This will simulate the upload and show what would be sent.

## CI/CD Integration

For CI/CD pipelines, use environment variables to configure the API key and build flavor:

```yaml
# GitHub Actions example - Production build
name: Build and Upload Symbols (Production)
on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: macos-latest
    env:
      DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
      DD_SITE: datadoghq.com
      DD_BUILD_FLAVOR: production

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'

      - name: Publish Android
        run: dotnet publish -f net9.0-android -c Release

      - name: Publish iOS
        run: dotnet publish -f net9.0-ios -c Release
```

For staging builds, use a different flavor:

```yaml
# GitHub Actions example - Staging build
name: Build and Upload Symbols (Staging)
on:
  push:
    branches: [develop]

jobs:
  build:
    runs-on: macos-latest
    env:
      DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
      DD_SITE: datadoghq.com
      DD_BUILD_FLAVOR: staging  # Different flavor for staging

    steps:
      # ... same as above
```

### Disabling Symbol Upload in CI

To disable symbol upload for specific builds:

```bash
# Disable via environment variable
export DatadogUploadEnabled=false
dotnet publish -f net9.0-android -c Release

# Or via command line property
dotnet publish -f net9.0-android -c Release /p:DatadogUploadEnabled=false
```

## Managing Symbol Upload Uniqueness

Datadog identifies symbol files using the combination of `(service, version, flavor)`. Uploads with the same combination are ignored.

**Important**: The version in your symbol upload must match the version reported by your RUM SDK for crash symbolication to work correctly. Therefore, we recommend using the app's actual version (`ApplicationDisplayVersion`) and differentiating builds using the **flavor** parameter.

### Recommended Approach: Use Flavors for Build Variants

The cleanest approach is to use different flavors for different build types while keeping the version consistent with your RUM SDK:

```xml
<PropertyGroup>
  <!-- Read flavor from environment variable or use configuration name -->
  <DatadogFlavor Condition="'$(DD_BUILD_FLAVOR)' != ''">$(DD_BUILD_FLAVOR)</DatadogFlavor>
  <!-- If not set, DatadogFlavor will default to "debug" or "release" based on Configuration -->
</PropertyGroup>
```

Then set the flavor when building:

```bash
# Production release
export DD_BUILD_FLAVOR=production
dotnet publish -f net9.0-android -c Release

# Staging release
export DD_BUILD_FLAVOR=staging
dotnet publish -f net9.0-android -c Release

# Development builds (unique per developer)
export DD_BUILD_FLAVOR=dev-kyle
dotnet publish -f net9.0-android -c Release
```

This generates symbol uploads like:
- Production: `(datadog-maui-android, 1.0, production)`
- Staging: `(datadog-maui-android, 1.0, staging)`
- Developer: `(datadog-maui-android, 1.0, dev-kyle)`

**Benefits of the flavor-based approach**:
- Version automatically matches between RUM SDK and symbol upload (no configuration needed)
- Clear separation between production, staging, and development builds
- Multiple developers can work simultaneously without conflicts
- No runtime version passing required
- Simple to use with environment variables or CI/CD pipelines

### Alternative: Version with Build Metadata (Not Recommended)

If you absolutely need unique versions for each build, you can append build metadata to the version. However, this approach is **not recommended** because:

- You **must** pass the build-time generated version to your RUM SDK initialization at runtime
- Requires code changes in your MainActivity/AppDelegate to read and set the version
- Creates maintenance burden and complexity
- Easy to misconfigure, breaking crash symbolication

If you still want to use this approach:

```xml
<PropertyGroup>
  <BuildTimestamp>$([System.DateTime]::UtcNow.ToString('yyyyMMddHHmmss'))</BuildTimestamp>
  <DatadogAppVersion>$(ApplicationDisplayVersion).$(BuildTimestamp)</DatadogAppVersion>
</PropertyGroup>
```

This generates versions like `1.0.20260130143022`, but **requires passing the same version to the RUM SDK at runtime for symbolication to work**. You'll need to embed this version in your app and configure the RUM SDK accordingly.

## License

Apache-2.0
