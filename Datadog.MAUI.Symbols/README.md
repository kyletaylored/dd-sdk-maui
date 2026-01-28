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

  <!-- Required: Service Name -->
  <DatadogServiceName>com.company.app</DatadogServiceName>

  <!-- Optional: Platform-specific service names (take precedence over global) -->
  <DatadogServiceNameAndroid>com.company.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.company.app.ios</DatadogServiceNameiOS>

  <!-- Optional: App Version (defaults to ApplicationDisplayVersion) -->
  <DatadogAppVersion>1.2.3</DatadogAppVersion>

  <!-- Optional: Datadog Site (defaults to datadoghq.com) -->
  <DatadogSite>us5.datadoghq.com</DatadogSite>

  <!-- Optional: Dry run mode (test without uploading) -->
  <DatadogDryRun>false</DatadogDryRun>

  <!-- Optional: Disable automatic upload -->
  <DatadogUploadEnabled>true</DatadogUploadEnabled>

  <!-- Optional: Enable upload in Debug configuration -->
  <DatadogUploadInDebug>false</DatadogUploadInDebug>
</PropertyGroup>
```

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

Once configured, the plugin automatically runs during publish:

```bash
# Publish for Android
dotnet publish -f net8.0-android -c Release

# Publish for iOS
dotnet publish -f net8.0-ios -c Release
```

The plugin will:

1. Detect the platform (Android/iOS)
2. Locate the debug symbols (`mapping.txt` or `.dSYM`)
3. Upload them to Datadog using `datadog-ci`

## Environment Variables

Instead of hardcoding the API key in your `.csproj`, you can use environment variables:

```bash
export DD_API_KEY="your-api-key"
export DD_SITE="us5.datadoghq.com"
```

## Troubleshooting

### "npx not found" Error.

Ensure Node.js and npm are installed and available in your PATH.

### "mapping.txt not found" (Android).

Verify that ProGuard/R8 is enabled for Release builds. The mapping file is only generated when obfuscation is enabled.

### "dSYM folder not found" (iOS).

The dSYM folder is typically generated during Archive builds. Ensure you're building in Release configuration.

## CI/CD Integration.

For CI/CD pipelines, set the API key as an environment variable:

```yaml
# GitHub Actions example
env:
  DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
```

## License

Apache-2.0
