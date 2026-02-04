---
layout: default
title: Configuration
nav_order: 2
parent: Symbol Upload Plugin
grand_parent: Guides
---

# Configuration Reference

Complete reference for all configuration options available in Datadog.MAUI.Symbols.

## MSBuild Properties

All configuration is done through MSBuild properties in your `.csproj` file. All properties are **namespaced with `DatadogSymbols*`** to avoid conflicts with other packages.

### Required Properties

#### Service Name

At least one service name property must be set:

```xml
<PropertyGroup>
  <!-- Option 1: Platform-specific (recommended) -->
  <DatadogSymbolsServiceNameAndroid>com.example.app.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.example.app.ios</DatadogSymbolsServiceNameiOS>

  <!-- Option 2: Global fallback -->
  <DatadogSymbolsServiceName>com.example.app</DatadogSymbolsServiceName>
</PropertyGroup>
```

**Service Name Hierarchy:**
1. Platform-specific (`DatadogSymbolsServiceNameAndroid` or `DatadogSymbolsServiceNameiOS`)
2. Global fallback (`DatadogSymbolsServiceName`)
3. Error if none are set

{: .important }
The service name must match exactly what you use in your Datadog RUM SDK initialization.

#### API Key

Provide via environment variable (recommended) or MSBuild property:

```xml
<!-- Option 1: Environment variable (recommended) -->
<!-- Set DD_API_KEY or DATADOG_API_KEY in your environment -->

<!-- Option 2: MSBuild property (not recommended for source control) -->
<DatadogSymbolsApiKey>your-api-key</DatadogSymbolsApiKey>
```

### Optional Properties

#### App Version

```xml
<!-- Defaults to ApplicationDisplayVersion -->
<DatadogSymbolsAppVersion>1.2.3</DatadogSymbolsAppVersion>
```

If not specified, uses `$(ApplicationDisplayVersion)` from your project, or `1.0.0` as final fallback.

{: .important }
**Do not** append build IDs to the version. The version must match exactly what the RUM SDK reports.

#### Build ID

```xml
<!-- Auto-generated per build when bundled CI is enabled -->
<!-- Can override if needed: -->
<DatadogSymbolsBuildId>abc123</DatadogSymbolsBuildId>
```

Build IDs are automatically generated and embedded in your app via the `DatadogBuildInfo` class. This allows the RUM SDK to associate crashes with the correct symbols.

**Default behavior:**
- Automatically generated: Yes (8-character GUID prefix)
- Passed to datadog-ci: Only when `DatadogSymbolsUseBundledCi=true`
- Available at runtime: Yes, via `Datadog.MAUI.Symbols.DatadogBuildInfo.BuildId`

#### Flavor/Variant

```xml
<!-- Defaults to "debug" or "release" based on Configuration -->
<DatadogSymbolsFlavor>staging</DatadogSymbolsFlavor>
```

Or set via environment variable:
```bash
export DD_BUILD_FLAVOR=production
```

Flavors allow you to upload symbols for different build variants (debug, release, staging, production) to the same service.

#### Datadog Site

```xml
<!-- Defaults to datadoghq.com (US1) -->
<DatadogSymbolsSite>us5.datadoghq.com</DatadogSymbolsSite>
```

**Available sites:**
- `datadoghq.com` - US1 (default)
- `us3.datadoghq.com` - US3
- `us5.datadoghq.com` - US5
- `datadoghq.eu` - EU1
- `ap1.datadoghq.com` - AP1

Or set via environment variable:
```bash
export DD_SITE="us5.datadoghq.com"
```

#### Dry Run Mode

```xml
<!-- Test without actually uploading (default: true for safety) -->
<DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
```

{: .warning }
**Default is `true`!** You must explicitly set to `false` to perform actual uploads.

#### Enable/Disable Upload

```xml
<!-- Disable upload completely -->
<DatadogSymbolsUploadEnabled>false</DatadogSymbolsUploadEnabled>

<!-- Enable upload in Debug configuration (default: false) -->
<DatadogSymbolsUploadInDebug>true</DatadogSymbolsUploadInDebug>
```

By default, upload only runs in Release configuration.

#### Bundled datadog-ci

```xml
<!-- Use bundled datadog-ci tarball (default: true) -->
<DatadogSymbolsUseBundledCi>true</DatadogSymbolsUseBundledCi>

<!-- Override tarball path (optional) -->
<DatadogSymbolsCiTgzPath>/path/to/custom/datadog-ci.tgz</DatadogSymbolsCiTgzPath>
```

The bundled CLI is enabled by default for deterministic behavior and build ID support. Set to `false` to use upstream `@datadog/datadog-ci` from npm registry.

## Configuration Examples

### Basic Configuration

Minimal setup with environment variable for API key:

```xml
<PropertyGroup>
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
</PropertyGroup>
```

```bash
export DD_API_KEY="your-api-key"
```

### Platform-Specific Configuration

Different settings per platform:

```xml
<PropertyGroup>
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsAppVersion>$(ApplicationDisplayVersion)</DatadogSymbolsAppVersion>
  <DatadogSymbolsSite>us5.datadoghq.com</DatadogSymbolsSite>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
</PropertyGroup>
```

### Multi-Environment with Flavors

Different flavors for staging vs production:

```xml
<!-- Base configuration -->
<PropertyGroup>
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
</PropertyGroup>

<!-- Flavor from environment -->
<PropertyGroup Condition="'$(DD_BUILD_FLAVOR)' != ''">
  <DatadogSymbolsFlavor>$(DD_BUILD_FLAVOR)</DatadogSymbolsFlavor>
</PropertyGroup>
```

```bash
# Production build
export DD_BUILD_FLAVOR=production
dotnet publish -c Release

# Staging build
export DD_BUILD_FLAVOR=staging
dotnet publish -c Release
```

### Conditional Configuration

Enable only for specific configurations:

```xml
<!-- Upload only in Release builds -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
</PropertyGroup>

<!-- Dry run in Debug builds -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DatadogSymbolsServiceNameAndroid>com.myapp.android.debug</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios.debug</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>true</DatadogSymbolsDryRun>
  <DatadogSymbolsUploadInDebug>true</DatadogSymbolsUploadInDebug>
</PropertyGroup>
```

### CI/CD Configuration

Use environment variables for secrets:

```xml
<PropertyGroup>
  <!-- Service names in source control -->
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>

  <!-- API key and site from environment -->
  <!-- DD_API_KEY environment variable -->
  <!-- DD_SITE environment variable -->
</PropertyGroup>
```

{% raw %}
```yaml
# GitHub Actions
env:
  DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
  DD_SITE: us5.datadoghq.com
  DD_BUILD_FLAVOR: production
```
{% endraw %}

### Using with RUM SDK

Pass build metadata to the RUM SDK:

```xml
<PropertyGroup>
  <DatadogSymbolsServiceNameAndroid>com.myapp.android</DatadogSymbolsServiceNameAndroid>
  <DatadogSymbolsServiceNameiOS>com.myapp.ios</DatadogSymbolsServiceNameiOS>
  <DatadogSymbolsDryRun>false</DatadogSymbolsDryRun>
  <!-- Build ID and Variant are auto-generated -->
</PropertyGroup>
```

```csharp
using Datadog.MAUI.Symbols;

// In your RUM configuration
datadog.EnableRum(rum =>
{
    rum.SetApplicationId(
        android: "android-rum-app-id",
        ios: "ios-rum-app-id"
    );

    // Pass build metadata for symbolication
    rum.Variant = DatadogBuildInfo.Variant;  // e.g., "release", "staging"
    rum.BuildId = DatadogBuildInfo.BuildId;  // e.g., "a1b2c3d4"
});
```

## Environment Variables

The plugin respects these environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `DD_API_KEY` or `DATADOG_API_KEY` | Datadog API key | (required if not in .csproj) |
| `DD_SITE` | Datadog site | `datadoghq.com` |
| `DD_BUILD_FLAVOR` | Build flavor/variant | Configuration name (Debug/Release) |

Environment variables take precedence over `.csproj` values where applicable.

## Property Reference Table

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `DatadogSymbolsServiceName` | string | Conditional | - | Global service name fallback |
| `DatadogSymbolsServiceNameAndroid` | string | Conditional | - | Android-specific service name |
| `DatadogSymbolsServiceNameiOS` | string | Conditional | - | iOS-specific service name |
| `DatadogSymbolsApiKey` | string | Conditional | `DD_API_KEY` env var | Datadog API key |
| `DatadogSymbolsAppVersion` | string | No | `ApplicationDisplayVersion` | App version for symbols |
| `DatadogSymbolsBuildId` | string | No | Auto-generated | Unique build identifier |
| `DatadogSymbolsFlavor` | string | No | Configuration name | Build flavor/variant |
| `DatadogSymbolsSite` | string | No | `datadoghq.com` | Datadog site URL |
| `DatadogSymbolsDryRun` | boolean | No | `true` | Run without uploading |
| `DatadogSymbolsUploadEnabled` | boolean | No | `true` | Enable/disable plugin |
| `DatadogSymbolsUploadInDebug` | boolean | No | `false` | Upload in Debug builds |
| `DatadogSymbolsUseBundledCi` | boolean | No | `true` | Use bundled datadog-ci |
| `DatadogSymbolsCiTgzPath` | string | No | Auto-detected | Custom datadog-ci tarball path |

## Advanced Configuration

### Custom Symbol Paths

The plugin automatically detects symbol files. If you have a custom build setup, the plugin searches these locations:

**Android:**
- `$(OutputPath)/mapping.txt`
- `$(OutputPath)/../mapping.txt`
- `$(IntermediateOutputPath)/mapping.txt`
- `$(ProjectDir)/obj/$(Configuration)/$(TargetFramework)/android-arm64/mapping.txt`

**iOS:**
- `$(OutputPath)/$(AssemblyName).app.dSYM`
- `$(OutputPath)/../$(AssemblyName).app.dSYM`
- `$(AppBundleDir).dSYM`

### Disable for Specific Platforms

```xml
<!-- Upload only iOS symbols -->
<PropertyGroup Condition="'$(TargetFramework)' == 'net8.0-android'">
  <DatadogSymbolsUploadEnabled>false</DatadogSymbolsUploadEnabled>
</PropertyGroup>
```

### Testing Configuration

Use dry-run to verify your configuration without uploading:

```bash
# Set dry-run in .csproj or via command line
dotnet publish -c Release -p:DatadogSymbolsDryRun=true
```

Check the build output for:
```
[Datadog] Command: npx @datadog/datadog-ci flutter-symbols upload --dry-run ...
```

### Using Upstream datadog-ci

To use the official npm registry version instead of bundled:

```xml
<PropertyGroup>
  <DatadogSymbolsUseBundledCi>false</DatadogSymbolsUseBundledCi>
</PropertyGroup>
```

Note: Build ID support may not be available in upstream CLI yet.

## Best Practices

1. **Use Environment Variables for Secrets**
   - Never commit API keys to source control
   - Use CI/CD secrets or local `.env` files

2. **Platform-Specific Service Names**
   - Use separate names for iOS and Android
   - Helps distinguish platform-specific issues in Datadog

3. **Version Consistency**
   - Use `$(ApplicationDisplayVersion)` for automatic sync
   - Or manage version centrally in `Directory.Build.props`
   - **Never** append build IDs to version strings

4. **Conditional Upload**
   - Only upload in Release builds by default
   - Use dry-run for testing in development
   - Remember to set `DatadogSymbolsDryRun=false` for real uploads

5. **Service Name Matching**
   - Ensure exact match with RUM SDK initialization
   - Case-sensitive!

6. **Flavor/Variant Usage**
   - Use flavors to differentiate environments (staging, production)
   - Pass variant to RUM SDK via `DatadogBuildInfo.Variant`
   - Each flavor uploads symbols independently

7. **Build ID Integration**
   - Let the plugin auto-generate build IDs
   - Pass to RUM SDK via `DatadogBuildInfo.BuildId`
   - Ensures crashes are symbolicated with correct symbols

## Validation

The plugin validates configuration at build time:

❌ **Missing Service Name:**
```
error: Datadog Service Name is required.
Set <DatadogSymbolsServiceName> or <DatadogSymbolsServiceNameAndroid>/<DatadogSymbolsServiceNameiOS>.
```

⚠️ **Missing Symbol Files:**
```
warning: Android mapping.txt not found.
Ensure ProGuard/R8 is enabled for Release builds.
```

⚠️ **Missing DD_API_KEY:**
```
warning: DD_API_KEY is not set.
Upload may fail if not configured elsewhere.
```

⚠️ **Dry-run enabled:**
```
[Datadog] Dry Run: true
```

## Migration from Old Property Names

If you were using older versions of the plugin, update your property names:

| Old Property | New Property |
|-------------|-------------|
| `DatadogServiceName` | `DatadogSymbolsServiceName` |
| `DatadogServiceNameAndroid` | `DatadogSymbolsServiceNameAndroid` |
| `DatadogServiceNameiOS` | `DatadogSymbolsServiceNameiOS` |
| `DatadogApiKey` | `DatadogSymbolsApiKey` |
| `DatadogAppVersion` | `DatadogSymbolsAppVersion` |
| `DatadogFlavor` | `DatadogSymbolsFlavor` |
| `DatadogSite` | `DatadogSymbolsSite` |
| `DatadogDryRun` | `DatadogSymbolsDryRun` |
| `DatadogUploadEnabled` | `DatadogSymbolsUploadEnabled` |
| `DatadogUploadInDebug` | `DatadogSymbolsUploadInDebug` |
| `DatadogUseBundledCi` | `DatadogSymbolsUseBundledCi` |

## Next Steps

- [CI/CD Integration Guide](ci-cd.html)
- [Troubleshooting Common Issues](troubleshooting.html)
- [Getting Started Guide](getting-started.html)
