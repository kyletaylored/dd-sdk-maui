# Testing Datadog.MAUI.Symbols Locally

## Option 1: Local NuGet Source (Recommended)

### Step 1: Add a local NuGet source

```bash
# Add a local package source (one-time setup)
dotnet nuget add source /Users/kyle.taylor/server/nuget/dd-sdk-maui/Datadog.MAUI.Symbols/bin/Release --name DatadogMAUISymbolsLocal
```

### Step 2: Build the package

```bash
cd /Users/kyle.taylor/server/nuget/dd-sdk-maui/Datadog.MAUI.Symbols
dotnet pack -c Release
```

### Step 3: Reference it in your MAUI app

Add to your MAUI app's `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Datadog.MAUI.Symbols" Version="3.5.0" />
</ItemGroup>
```

### Step 4: Configure the plugin

Add these properties to your MAUI app's `.csproj`:

```xml
<PropertyGroup>
  <!-- Test with dry-run first -->
  <DatadogDryRun>true</DatadogDryRun>

  <DatadogServiceName>com.test.app</DatadogServiceName>
  <!-- Or platform-specific -->
  <DatadogServiceNameAndroid>com.test.app.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.test.app.ios</DatadogServiceNameiOS>

  <!-- API Key (or set DD_API_KEY environment variable) -->
  <DatadogApiKey>your-test-key</DatadogApiKey>
</PropertyGroup>
```

### Step 5: Test the build

```bash
# For Android
dotnet publish -f net9.0-android -c Release

# For iOS
dotnet publish -f net9.0-ios -c Release
```

---

## Option 2: Direct Project Reference

You can also reference the project directly, though this is less realistic:

Add to your MAUI app's `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="../Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj"
                    ReferenceOutputAssembly="false"
                    PrivateAssets="all" />
</ItemGroup>
```

**Note**: This approach may not work perfectly because MSBuild tasks need special packaging to be discovered.

---

## Option 3: Manual Package Installation

```bash
# After building the package
cd YourMAUIApp
dotnet add package Datadog.MAUI.Symbols --source /Users/kyle.taylor/server/nuget/dd-sdk-maui/Datadog.MAUI.Symbols/bin/Release
```

---

## Troubleshooting Local Testing

### Clear NuGet cache between builds

When making changes to the plugin, clear the cache to ensure you're using the latest version:

```bash
dotnet nuget locals all --clear
```

### Increment version for testing

If you make changes, increment the version in `Datadog.MAUI.Symbols.csproj` or the `DatadogSdkVersion` in `Directory.Build.props`:

```xml
<DatadogSdkVersion>3.5.1-local</DatadogSdkVersion>
```

### Enable verbose logging

To see what the plugin is doing:

```bash
dotnet publish -f net9.0-android -c Release -v detailed
```

### Verify the plugin is loaded

Check the build output for:
```
[Datadog] Uploading android symbols to Datadog...
```

---

## .NET Version Compatibility

The plugin targets `netstandard2.0`, which means it's compatible with:

- ✅ .NET 6 (net6.0-android, net6.0-ios)
- ✅ .NET 7 (net7.0-android, net7.0-ios)
- ✅ .NET 8 (net8.0-android, net8.0-ios)
- ✅ .NET 9 (net9.0-android, net9.0-ios)
- ✅ .NET 10+ (net10.0-android, net10.0-ios)

MSBuild tasks must target `netstandard2.0` to be compatible with all MSBuild versions, so this plugin will work with any .NET MAUI app regardless of the target framework version.
