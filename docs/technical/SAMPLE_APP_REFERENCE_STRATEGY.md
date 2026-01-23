# Sample App Reference Strategy

## Problem

The sample app needs to reference the Datadog.MAUI plugin differently depending on the build scenario:

1. **Local Debug**: Fast iteration - rebuild plugin on every change
2. **Local Release**: Test actual NuGet packages before publishing
3. **CI Pipeline**: Validate the packages that will be published

## Solution

Use **conditional references** based on `$(Configuration)`:

### Debug Configuration
```xml
<ItemGroup Condition="'$(Configuration)' == 'Debug'">
  <ProjectReference Include="..\..\Datadog.MAUI.Plugin\Datadog.MAUI.Plugin.csproj" />
</ItemGroup>
```

**Behavior**: Direct project reference → fast F5 debugging, no NuGet package needed

### Release Configuration
```xml
<ItemGroup Condition="'$(Configuration)' == 'Release'">
  <PackageReference Include="Datadog.MAUI" Version="$(DatadogSdkVersion)" />
</ItemGroup>

<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <RestoreAdditionalProjectSources>$(MSBuildProjectDirectory)/../../artifacts;$(RestoreAdditionalProjectSources)</RestoreAdditionalProjectSources>
</PropertyGroup>
```

**Behavior**: Uses NuGet package from `./artifacts` folder

## How It Works

### Local Debug Build
```bash
# Fast development iteration
dotnet build samples/DatadogMauiSample -c Debug -f net10.0-android

# Uses ProjectReference → plugin rebuilt automatically
# No need for NuGet packages
```

### Local Release Build
```bash
# Pack the plugin first
dotnet pack Datadog.MAUI.Plugin -c Release -o ./artifacts

# Build sample app using the packed NuGet
dotnet build samples/DatadogMauiSample -c Release -f net10.0-android

# Restores from ./artifacts (via NuGet.Config)
# Uses actual NuGet package → tests what users will get
```

### CI Pipeline Build

In CI, the workflow:
1. Builds and packs all binding projects → `./artifacts/*.nupkg`
2. Builds sample app in Release mode
3. Sample app automatically finds packages in `./artifacts` (via `NuGet.Config`)
4. Validates the actual packages that will be published

**No special CI configuration needed!** The `NuGet.Config` at the repo root handles it:

```xml
<packageSources>
  <clear />
  <add key="local" value="./artifacts" />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```

## Benefits

### For Developers
- **Debug**: Fast iteration with `ProjectReference`
- **Release**: Test actual NuGet packages locally before pushing

### For CI
- **Validates real packages**: Sample app uses the exact NuGet packages that will be published
- **No special config**: Works automatically with existing `NuGet.Config`
- **Catches packaging issues**: If NuGet packaging is broken, sample app build fails

### For Testing
- **Local smoke test**: `dotnet build -c Release` validates packages work
- **CI validation**: Pipeline ensures packages are consumable
- **Pre-release testing**: Can test packages before publishing to nuget.org

## Usage Examples

### Developer Workflow (Debug)
```bash
# Edit plugin code
vim Datadog.MAUI.Plugin/Platforms/Android/Datadog.android.cs

# Build and run sample - uses ProjectReference
dotnet build samples/DatadogMauiSample -c Debug -f net10.0-android
dotnet run --project samples/DatadogMauiSample -f net10.0-android
```

### Testing Packages Locally (Release)
```bash
# Pack all packages
./scripts/pack.sh

# Build sample app with packages
dotnet build samples/DatadogMauiSample -c Release -f net10.0-android

# If build succeeds, packages are good!
```

### CI Pipeline
```yaml
# Build and pack bindings
- name: Build and pack Android bindings
  run: dotnet pack Datadog.MAUI.Android.Binding/*.csproj -o ./artifacts

- name: Build and pack iOS bindings
  run: dotnet pack Datadog.MAUI.iOS.Binding/*.csproj -o ./artifacts

- name: Build and pack unified plugin
  run: dotnet pack Datadog.MAUI.Plugin -o ./artifacts

# Build sample app (automatically uses ./artifacts via NuGet.Config)
- name: Build sample app
  run: dotnet build samples/DatadogMauiSample -c Release -f net10.0-android

# If this succeeds, packages are valid!
```

## Troubleshooting

### "Package 'Datadog.MAUI' not found" in Release build
**Cause**: `./artifacts` folder doesn't have the NuGet package

**Fix**: Pack the plugin first:
```bash
dotnet pack Datadog.MAUI.Plugin -c Release -o ./artifacts
```

### Sample app still uses old package version
**Cause**: NuGet cache has old version

**Fix**: Clear local NuGet cache:
```bash
dotnet nuget locals all --clear
rm -rf ~/.nuget/packages/datadog.maui
```

### CI build can't find packages
**Cause**: Packages weren't built/packed before sample app build

**Fix**: Ensure CI workflow packs packages before building sample:
```yaml
- name: Pack packages
  run: dotnet pack -o ./artifacts

- name: Build sample  # Must come AFTER pack
  run: dotnet build samples/DatadogMauiSample -c Release
```

## Related Files

- [DatadogMauiSample.csproj](../../samples/DatadogMauiSample/DatadogMauiSample.csproj#95-116) - Conditional reference logic
- [NuGet.Config](../../NuGet.Config) - Local artifacts source
- [build-android.yml](../../.github/workflows/build-android.yml) - CI Android build
- [build-ios.yml](../../.github/workflows/build-ios.yml) - CI iOS build

## Design Rationale

### Why not always use ProjectReference?
- Doesn't test the actual NuGet packaging
- Doesn't catch issues with package dependencies
- Can't validate what users will consume

### Why not always use PackageReference?
- Slow local development (need to pack every time)
- Harder to debug into plugin code
- Extra step for quick iteration

### Why condition on Configuration instead of custom property?
- Standard MSBuild convention
- Works with IDE build configurations
- No custom environment variables needed
- Familiar to all .NET developers

## Conclusion

This strategy gives us the best of both worlds:
- **Fast Debug development** with ProjectReference
- **Validated Release builds** with PackageReference from `./artifacts`
- **Automatic CI validation** of publishable packages

The sample app becomes a **smoke test** for the NuGet packages - if it builds in Release mode, the packages are good!
