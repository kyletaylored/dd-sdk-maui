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

All configuration is done through MSBuild properties in your `.csproj` file.

### Required Properties

#### Service Name

At least one service name property must be set:

```xml
<PropertyGroup>
  <!-- Option 1: Platform-specific (recommended) -->
  <DatadogServiceNameAndroid>com.example.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.example.app.ios</DatadogServiceNameiOS>

  <!-- Option 2: Global fallback -->
  <DatadogServiceName>com.example.app</DatadogServiceName>
</PropertyGroup>
```

**Service Name Hierarchy:**
1. Platform-specific (`DatadogServiceNameAndroid` or `DatadogServiceNameiOS`)
2. Global fallback (`DatadogServiceName`)
3. Error if none are set

{: .important }
The service name must match exactly what you use in your Datadog RUM SDK initialization.

#### API Key

Provide via environment variable (recommended) or MSBuild property:

```xml
<!-- Option 1: Environment variable (recommended) -->
<!-- Set DD_API_KEY in your environment -->

<!-- Option 2: MSBuild property (not recommended for source control) -->
<DatadogApiKey>your-api-key</DatadogApiKey>
```

### Optional Properties

#### App Version

```xml
<!-- Defaults to ApplicationDisplayVersion -->
<DatadogAppVersion>1.2.3</DatadogAppVersion>
```

If not specified, uses `$(ApplicationDisplayVersion)` from your project, or `1.0.0` as final fallback.

#### Datadog Site

```xml
<!-- Defaults to datadoghq.com (US1) -->
<DatadogSite>us5.datadoghq.com</DatadogSite>
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
<!-- Test without actually uploading -->
<DatadogDryRun>true</DatadogDryRun>
```

Useful for testing configuration without consuming API quotas.

#### Enable/Disable Upload

```xml
<!-- Disable upload completely -->
<DatadogUploadEnabled>false</DatadogUploadEnabled>

<!-- Enable upload in Debug configuration (default: false) -->
<DatadogUploadInDebug>true</DatadogUploadInDebug>
```

By default, upload only runs in Release configuration.

## Configuration Examples

### Basic Configuration

Minimal setup with environment variable for API key:

```xml
<PropertyGroup>
  <DatadogServiceNameAndroid>com.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios</DatadogServiceNameiOS>
</PropertyGroup>
```

```bash
export DD_API_KEY="your-api-key"
```

### Platform-Specific Configuration

Different settings per platform:

```xml
<PropertyGroup>
  <DatadogServiceNameAndroid>com.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios</DatadogServiceNameiOS>
  <DatadogAppVersion>$(ApplicationDisplayVersion)</DatadogAppVersion>
  <DatadogSite>us5.datadoghq.com</DatadogSite>
</PropertyGroup>
```

### Conditional Configuration

Enable only for specific configurations:

```xml
<!-- Upload only in Release builds -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DatadogServiceNameAndroid>com.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios</DatadogServiceNameiOS>
</PropertyGroup>

<!-- Dry run in Debug builds -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DatadogServiceNameAndroid>com.myapp.android.debug</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios.debug</DatadogServiceNameiOS>
  <DatadogDryRun>true</DatadogDryRun>
</PropertyGroup>
```

### CI/CD Configuration

Use environment variables for secrets:

```xml
<PropertyGroup>
  <!-- Service names in source control -->
  <DatadogServiceNameAndroid>com.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios</DatadogServiceNameiOS>

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
```
{% endraw %}

### Multi-Environment Configuration

Different service names per environment:

```xml
<!-- Production -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <DatadogServiceNameAndroid>com.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios</DatadogServiceNameiOS>
</PropertyGroup>

<!-- Staging -->
<PropertyGroup Condition="'$(Configuration)' == 'Staging'">
  <DatadogServiceNameAndroid>com.myapp.android.staging</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.myapp.ios.staging</DatadogServiceNameiOS>
</PropertyGroup>
```

## Environment Variables

The plugin respects these environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `DD_API_KEY` | Datadog API key | (required if not in .csproj) |
| `DD_SITE` | Datadog site | `datadoghq.com` |

Environment variables take precedence over `.csproj` values for `DD_SITE`.

## Property Reference Table

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `DatadogServiceName` | string | Conditional | - | Global service name fallback |
| `DatadogServiceNameAndroid` | string | Conditional | - | Android-specific service name |
| `DatadogServiceNameiOS` | string | Conditional | - | iOS-specific service name |
| `DatadogApiKey` | string | Conditional | `DD_API_KEY` env var | Datadog API key |
| `DatadogAppVersion` | string | No | `ApplicationDisplayVersion` | App version for symbols |
| `DatadogSite` | string | No | `datadoghq.com` | Datadog site URL |
| `DatadogDryRun` | boolean | No | `false` | Run without uploading |
| `DatadogUploadEnabled` | boolean | No | `true` | Enable/disable plugin |
| `DatadogUploadInDebug` | boolean | No | `false` | Upload in Debug builds |

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
  <DatadogUploadEnabled>false</DatadogUploadEnabled>
</PropertyGroup>
```

### Testing Configuration

Use dry-run to verify your configuration without uploading:

```bash
# Set dry-run in .csproj or via command line
dotnet publish -c Release -p:DatadogDryRun=true
```

Check the build output for:
```
[Datadog] Command: npx @datadog/datadog-ci flutter-symbols upload --dry-run ...
```

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

4. **Conditional Upload**
   - Only upload in Release builds by default
   - Use dry-run for testing in development

5. **Service Name Matching**
   - Ensure exact match with RUM SDK initialization
   - Case-sensitive!

## Validation

The plugin validates configuration at build time:

❌ **Missing Service Name:**
```
error: Datadog Service Name is required.
Set <DatadogServiceName> or <DatadogServiceNameAndroid>/<DatadogServiceNameiOS>.
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

## Next Steps

- [CI/CD Integration Guide](ci-cd.html)
- [Troubleshooting Common Issues](troubleshooting.html)
- [Getting Started Guide](getting-started.html)
