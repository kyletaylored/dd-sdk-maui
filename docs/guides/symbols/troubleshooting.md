---
layout: default
title: Troubleshooting
nav_order: 4
parent: Symbol Upload Plugin
grand_parent: Guides
---

# Troubleshooting Guide

Common issues and solutions for Datadog.MAUI.Symbols.

## Installation Issues

### Package Not Found

**Error**: `error NU1101: Unable to find package Datadog.MAUI.Symbols`

**Causes**:
- Package not yet published to NuGet.org
- Using wrong package source
- Network/connectivity issues

**Solutions**:

1. Verify package exists:
   ```bash
   dotnet search Datadog.MAUI.Symbols
   ```

2. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

3. Check package sources:
   ```bash
   dotnet nuget list source
   ```

### Version Compatibility

**Error**: `Package Datadog.MAUI.Symbols 1.0.0 is not compatible with net6.0-android`

**Cause**: Wrong package version or corrupted download

**Solution**:
```bash
# Clear cache and restore
dotnet nuget locals all --clear
dotnet restore --force
```

The plugin supports all .NET versions (6.0+) as it targets `netstandard2.0`.

## Configuration Issues

### No Service Name Configured

**Error**: `Datadog Service Name is required. Set <DatadogServiceName> or <DatadogServiceNameAndroid>/<DatadogServiceNameiOS>.`

**Solution**:

Add to your `.csproj`:

```xml
<PropertyGroup>
  <DatadogServiceNameAndroid>com.yourapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.yourapp.ios</DatadogServiceNameiOS>
</PropertyGroup>
```

### API Key Not Found

**Warning**: `DD_API_KEY is not set. Upload may fail if not configured elsewhere.`

**Solutions**:

1. Set environment variable:
   ```bash
   export DD_API_KEY="your-api-key"
   ```

2. Or add to `.csproj` (not recommended for production):
   ```xml
   <DatadogApiKey>your-api-key</DatadogApiKey>
   ```

3. Verify it's set:
   ```bash
   echo $DD_API_KEY  # Should show your key
   ```

## Build Issues

### npx Command Not Found

**Error**: `npx: command not found` or `Failed to execute npx command. Ensure Node.js is installed.`

**Cause**: Node.js not installed or not in PATH

**Solutions**:

1. Install Node.js:
   - Download from [nodejs.org](https://nodejs.org/)
   - Or use package manager:
     ```bash
     # macOS
     brew install node

     # Ubuntu/Debian
     sudo apt install nodejs npm

     # Windows
     choco install nodejs
     ```

2. Verify installation:
   ```bash
   node --version  # Should show v14+
   npm --version   # Should show v6+
   npx --version   # Should show v6+
   ```

3. Add to PATH if needed (Windows):
   ```
   C:\Program Files\nodejs
   ```

### mapping.txt Not Found (Android)

**Warning**: `Android mapping.txt not found. Ensure ProGuard/R8 is enabled for Release builds.`

**Cause**: ProGuard/R8 not enabled or not running in Release configuration

**Solutions**:

1. Enable R8/ProGuard in `.csproj`:
   ```xml
   <PropertyGroup Condition="'$(Configuration)' == 'Release'">
     <AndroidLinkTool>r8</AndroidLinkTool>
     <AndroidEnableProguard>true</AndroidEnableProguard>
   </PropertyGroup>
   ```

2. Verify you're building in Release:
   ```bash
   dotnet publish -c Release -f net8.0-android
   ```

3. Check if file exists:
   ```bash
   find . -name "mapping.txt"
   ```

   Should show path like:
   ```
   ./bin/Release/net8.0-android/mapping.txt
   ```

4. If still missing, check for build errors:
   ```bash
   dotnet build -c Release -v detailed | grep -i proguard
   ```

### dSYM Folder Not Found (iOS)

**Warning**: `iOS dSYM folder not found at expected locations.`

**Cause**: Building in wrong configuration or dSYM generation disabled

**Solutions**:

1. Build in Release configuration:
   ```bash
   dotnet publish -c Release -f net8.0-ios
   ```

2. dSYMs are typically at:
   ```
   bin/Release/net8.0-ios/YourApp.app.dSYM
   ```

3. Verify dSYM exists:
   ```bash
   find . -name "*.dSYM" -type d
   ```

4. For Archive builds:
   ```bash
   # Create archive which includes dSYMs
   xcodebuild archive -scheme YourApp \
     -archivePath build/YourApp.xcarchive
   ```

## Upload Issues

### Upload Failed - 403 Forbidden

**Error**: `Datadog upload failed: 403 Forbidden`

**Causes**:
- Invalid API key
- API key doesn't have upload permissions
- Wrong Datadog site

**Solutions**:

1. Verify API key:
   - Log in to Datadog
   - Go to Organization Settings → API Keys
   - Verify key exists and is active

2. Check permissions:
   - API key needs upload/write permissions
   - Create a new key if needed

3. Verify Datadog site:
   ```xml
   <!-- Match your Datadog instance -->
   <DatadogSite>us5.datadoghq.com</DatadogSite>
   ```

### Upload Failed - Network Error

**Error**: `ENOTFOUND`, `ETIMEDOUT`, or `Network error`

**Causes**:
- No internet connection
- Corporate firewall
- Proxy configuration needed

**Solutions**:

1. Test connectivity:
   ```bash
   curl -I https://api.datadoghq.com
   ```

2. Configure proxy if needed:
   ```bash
   export HTTP_PROXY=http://proxy.company.com:8080
   export HTTPS_PROXY=http://proxy.company.com:8080
   ```

3. Check firewall rules for outbound HTTPS to:
   - `api.datadoghq.com` (US1)
   - `api.us5.datadoghq.com` (US5)
   - etc.

### Upload Succeeds But Symbols Not Working

**Problem**: Upload succeeds but crashes still show obfuscated stacks

**Causes**:
- Service name mismatch
- Wrong app version
- Symbols not yet indexed (takes a few minutes)

**Solutions**:

1. Verify service name matches RUM SDK exactly:

   ```csharp
   // RUM SDK initialization
   Datadog.Initialize(appContext, credentials, configuration, trackingConsent) {
       ServiceName = "com.yourapp.android"  // Must match exactly!
   };
   ```

   ```xml
   <!-- Symbols plugin -->
   <DatadogServiceNameAndroid>com.yourapp.android</DatadogServiceNameAndroid>
   ```

2. Check app version matches:
   ```xml
   <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
   ```

3. Wait 5-10 minutes for indexing

4. Verify in Datadog:
   - APM → Error Tracking → Settings → Symbol Files
   - Look for your service name and version

### datadog-ci Command Failed

**Error**: Various errors from `@datadog/datadog-ci`

**Solutions**:

1. Clear npm cache:
   ```bash
   npm cache clean --force
   ```

2. Update datadog-ci:
   ```bash
   npx clear-npx-cache
   ```

3. Run manually to debug:
   ```bash
   npx @datadog/datadog-ci flutter-symbols upload \
     --service-name com.yourapp.android \
     --version 1.0.0 \
     --android-mapping \
     --android-mapping-location ./bin/Release/mapping.txt
   ```

## CI/CD Issues

### Upload Works Locally But Not in CI

**Problem**: Upload succeeds on local machine but fails in CI/CD

**Solutions**:

1. Verify Node.js installed in CI:
   ```yaml
   - name: Setup Node.js
     uses: actions/setup-node@v4
     with:
       node-version: '18'
   ```

2. Check secrets are set:
   ```yaml
   - name: Debug
     run: |
       echo "DD_API_KEY is set: $([[ -n "$DD_API_KEY" ]] && echo 'yes' || echo 'no')"
   ```

3. Enable verbose logging:
   ```bash
   dotnet publish -c Release -v detailed
   ```

4. Check CI build logs for:
   ```
   [Datadog] Uploading android symbols to Datadog...
   ```

### Secrets Not Available

**Error**: API key not found in CI environment

**Solutions**:

1. **GitHub Actions**:
   - Settings → Secrets → Actions
   - Add `DATADOG_API_KEY`

2. **Azure DevOps**:
   - Pipelines → Library → Variable Groups
   - Create group with `DatadogApiKey` (mark as secret)

3. **GitLab CI**:
   - Settings → CI/CD → Variables
   - Add `DATADOG_API_KEY` (Protected, Masked)

4. **Jenkins**:
   - Manage Jenkins → Credentials
   - Add Secret text with ID `datadog-api-key`

## Performance Issues

### Upload Takes Too Long

**Problem**: Symbol upload adding significant time to build

**Solutions**:

1. Check symbol file size:
   ```bash
   # Android
   ls -lh bin/Release/*/mapping.txt

   # iOS
   du -sh bin/Release/*/*.dSYM
   ```

2. Use faster internet connection

3. Consider uploading in background job (post-build)

4. Check if dry-run is faster (indicates network issue):
   ```bash
   dotnet publish -c Release -p:DatadogDryRun=true
   ```

### Build Hangs During Upload

**Problem**: Build appears to hang at symbol upload step

**Solutions**:

1. Check for network connectivity

2. Set timeout in CI:
   ```yaml
   - name: Publish
     timeout-minutes: 10
     run: dotnet publish -c Release
   ```

3. Enable verbose output to see progress:
   ```bash
   dotnet publish -c Release -v detailed
   ```

## Debugging

### Enable Verbose Logging

Get detailed MSBuild output:

```bash
dotnet publish -c Release -v detailed 2>&1 | tee build.log
```

Look for:
```
[Datadog] Command: npx @datadog/datadog-ci ...
```

### Test Plugin Directly

Test without building entire app:

```bash
# Run plugin task manually
npx @datadog/datadog-ci flutter-symbols upload \
  --service-name com.test.app \
  --version 1.0.0 \
  --android-mapping \
  --android-mapping-location ./path/to/mapping.txt \
  --dry-run
```

### Verify Package Installation

Check if plugin is properly installed:

```bash
# List installed packages
dotnet list package | grep Datadog.MAUI.Symbols

# Should show:
# Datadog.MAUI.Symbols    1.0.0
```

### Check Target File

Verify the `.targets` file is being loaded:

```bash
dotnet build -c Release -v detailed 2>&1 | grep -i "Datadog.MAUI.Symbols.targets"
```

Should see:
```
Target "DatadogUploadSymbols" in file "...Datadog.MAUI.Symbols.targets"
```

## Getting Help

If you're still stuck:

1. **Check the documentation**:
   - [Getting Started](getting-started.html)
   - [Configuration Reference](configuration.html)
   - [CI/CD Integration](ci-cd.html)

2. **Search existing issues**:
   - [GitHub Issues](https://github.com/DataDog/dd-sdk-maui/issues)

3. **Create a new issue**:
   - Include full error message
   - Build output with `-v detailed`
   - Platform (iOS/Android)
   - .NET version
   - CI/CD platform (if applicable)

4. **Contact Datadog Support**:
   - [Datadog Support Portal](https://docs.datadoghq.com/help/)
   - Include reference to `Datadog.MAUI.Symbols` plugin

## Common Error Messages Reference

| Error | Quick Fix |
|-------|-----------|
| `npx: command not found` | Install Node.js |
| `mapping.txt not found` | Enable ProGuard/R8 for Release |
| `dSYM folder not found` | Build in Release configuration |
| `Service Name is required` | Add `DatadogServiceName*` properties |
| `DD_API_KEY is not set` | Set environment variable or MSBuild property |
| `403 Forbidden` | Check API key and permissions |
| `ENOTFOUND` / `ETIMEDOUT` | Check network connectivity |
| `Package not found` | Clear NuGet cache, restore |
