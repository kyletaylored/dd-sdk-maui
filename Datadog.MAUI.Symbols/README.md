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

## Configuration

Configure the plugin using MSBuild properties in your `.csproj` file:

```xml
<PropertyGroup>
  <!-- Required: Datadog API Key (or set DD_API_KEY environment variable) -->
  <DatadogApiKey>your-api-key</DatadogApiKey>

  <!-- Required: Service Name (global fallback) -->
  <DatadogServiceName>com.company.app</DatadogServiceName>

  <!-- Optional: Platform-specific service names (override global) -->
  <DatadogServiceNameAndroid>com.company.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.company.app.ios</DatadogServiceNameiOS>

  <!-- Optional: App Version (defaults to ApplicationDisplayVersion or 1.0.0) -->
  <DatadogAppVersion>1.2.3</DatadogAppVersion>

  <!-- Optional: Datadog Site (defaults to datadoghq.com) -->
  <!-- Common values: datadoghq.com, us3.datadoghq.com, us5.datadoghq.com, datadoghq.eu, ap1.datadoghq.com -->
  <DatadogSite>datadoghq.com</DatadogSite>

  <!-- Optional: Dry run mode - simulates upload without sending data -->
  <DatadogDryRun>true</DatadogDryRun>

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
| `DatadogAppVersion` | No | `ApplicationDisplayVersion` or `1.0.0` | Version string for the uploaded symbols. Must match the version in your RUM configuration. |
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

For CI/CD pipelines, set the API key as an environment variable:

```yaml
# GitHub Actions example
name: Build and Upload Symbols
on: [push]

jobs:
  build:
    runs-on: macos-latest
    env:
      DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
      DD_SITE: datadoghq.com

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

### Disabling Symbol Upload in CI

To disable symbol upload for specific builds:

```bash
# Disable via environment variable
export DatadogUploadEnabled=false
dotnet publish -f net9.0-android -c Release

# Or via command line property
dotnet publish -f net9.0-android -c Release /p:DatadogUploadEnabled=false
```

## License

Apache-2.0
