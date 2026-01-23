# GitHub Actions Workflow Best Practices

This document outlines the best practices used in our CI/CD workflows for building Datadog MAUI SDK bindings.

## iOS Build Workflow

### Runner Configuration

**Runner**: `macos-latest` (macOS 15 Sequoia ARM64)

- Provides multiple Xcode versions
- ARM64 architecture for better performance
- See available versions: https://github.com/actions/runner-images/blob/main/images/macos/macos-15-arm64-Readme.md

**Xcode Version**: `16.2`

- Explicitly selected via `sudo xcode-select -s /Applications/Xcode_16.2.app/Contents/Developer`
- Must match one of the available versions in the runner image
- Update `env.XCODE_VERSION` when upgrading

### Multi-Target Framework Build Strategy

#### Why Separate Jobs?

We build different .NET target frameworks (net8.0-ios, net9.0-ios, net10.0-ios) in separate jobs because:

1. **SDK Isolation**: .NET SDK 8 requires specific isolation to prevent conflicts with newer SDKs
2. **global.json Management**: Each SDK version needs its own `global.json` configuration
3. **Workload Compatibility**: Different SDK versions have different iOS workload requirements
4. **Xcode Compatibility**: net8.0-ios requires Xcode 15.4, while net9.0-ios+ can use Xcode 16.2

#### Job Structure

```yaml
jobs:
  # Job 1: Build with .NET SDK 8 for net8.0-ios
  build-ios-net8:
    steps:
      - name: Setup .NET SDK 8
        uses: actions/setup-dotnet@v5
        with:
          dotnet-version: 8.0.x

      - name: Create temporary global.json for SDK 8
        run: echo '{"sdk":{"version":"${{ steps.setup-dotnet-8.outputs.dotnet-version }}","rollForward":"latestPatch"}}' > ./global.json

      - name: Build iOS bindings (net8.0-ios)
        run: dotnet build ... -p:TargetFrameworks=net8.0-ios

  # Job 2: Build with .NET SDK 9 for net9.0-ios
  build-ios-net9:
    steps:
      - name: Setup .NET SDK 9
        uses: actions/setup-dotnet@v5
        with:
          dotnet-version: 9.0.x

      - name: Create temporary global.json for SDK 9
        run: echo '{"sdk":{"version":"${{ steps.setup-dotnet-9.outputs.dotnet-version }}","rollForward":"latestPatch"}}' > ./global.json

      - name: Build iOS bindings (net9.0-ios)
        run: dotnet build ... -p:TargetFrameworks=net9.0-ios

  # Job 3: Build with .NET SDK 10 for net10.0-ios
  build-ios-net10:
    steps:
      - name: Remove global.json to allow SDK 10+
        run: rm -f global.json

      - name: Setup .NET SDK 10
        uses: actions/setup-dotnet@v5
        with:
          dotnet-version: 10.0.x

      - name: Build iOS bindings (net10.0-ios)
        run: dotnet build ... -p:TargetFrameworks=net10.0-ios
```

### XCFramework Management

**Strategy**: Download once, share across jobs

```yaml
jobs:
  download-xcframeworks:
    runs-on: macos-latest
    steps:
      - name: Cache XCFrameworks
        uses: actions/cache@v5
        with:
          key: ${{ runner.os }}-xcframeworks-${{ steps.sdk-version.outputs.version }}

      - name: Upload XCFrameworks
        uses: actions/upload-artifact@v6
        with:
          name: ios-xcframeworks
          retention-days: 1

  build-ios-net8:
    needs: download-xcframeworks
    steps:
      - name: Download XCFrameworks
        uses: actions/download-artifact@v7
        with:
          name: ios-xcframeworks
```

### Package Combining

**Why**: NuGet packages must contain all target frameworks (net8.0-ios, net9.0-ios, net10.0-ios) in a single package

```yaml
jobs:
  combine-ios-packages:
    needs: [build-ios-net8, build-ios-net9, build-ios-net10]
    runs-on: ubuntu-latest # Package manipulation doesn't need macOS
    steps:
      - name: Combine packages for all target frameworks
        run: |
          # Extract net8 package as base
          unzip -q "$net8_pkg" -d "./temp-extract/base"

          # Copy net9.0-ios frameworks
          unzip -q "$net9_pkg" -d "./temp-extract/net9"
          cp -r "./temp-extract/net9/lib/net9.0-ios" "./temp-extract/base/lib/"

          # Copy net10.0-ios frameworks
          unzip -q "$net10_pkg" -d "./temp-extract/net10"
          cp -r "./temp-extract/net10/lib/net10.0-ios" "./temp-extract/base/lib/"

          # Repackage
          cd "./temp-extract/base"
          zip -q -r "../../artifacts-combined/${pkg_name}.${version}.nupkg" *
```

## Android Build Workflow

### Runner Configuration

**Runner**: `ubuntu-latest`

- Android builds don't require macOS
- Linux runners are faster and cheaper
- Ubuntu provides all necessary Android SDK tools

### Multi-Target Framework Build

Android uses a similar strategy but with fewer complications:

- net9.0-android and net10.0-android can be built together
- No need for strict SDK isolation like iOS
- Can use latest .NET SDK for both targets

## Common Best Practices

### Caching Strategy

**NuGet Packages**:

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v5
  with:
    path: ./artifacts-net8/*.nupkg
    key: ${{ runner.os }}-packages-${{ steps.sdk-version.outputs.version }}-${{ hashFiles('**/*.csproj') }}
```

**Workloads**:

```yaml
- name: Cache .NET workloads
  uses: actions/cache@v5
  with:
    path: |
      ~/.dotnet/sdk-manifests
      ~/.dotnet/metadata
      ~/.dotnet/workloadsets
      ~/.nuget/packages
    key: ${{ runner.os }}-dotnet-workload-${{ steps.setup-dotnet.outputs.dotnet-version }}
```

### Artifact Retention

- **XCFrameworks**: 1 day (only needed during build)
- **Intermediate packages**: 7 days (for debugging)
- **Combined packages**: 30 days (final artifacts)
- **Build logs**: 3 days (debugging failed builds)

### Error Handling

```yaml
- name: Build iOS bindings
  run: dotnet build ...
  continue-on-error: true # Only for incomplete bindings

- name: Upload build logs on failure
  if: failure()
  uses: actions/upload-artifact@v6
  with:
    name: build-logs
    path: |
      **/*.binlog
      **/bin/**/*.log
```

## Version Management

### Datadog SDK Version

Stored in `Directory.Build.props`:

```xml
<PropertyGroup>
  <DatadogSdkVersion>3.5.0</DatadogSdkVersion>
</PropertyGroup>
```

Retrieved in workflows:

```bash
VERSION=$(grep -E '<DatadogSdkVersion>.*</DatadogSdkVersion>' Directory.Build.props | sed 's/.*<DatadogSdkVersion>\(.*\)<\/DatadogSdkVersion>.*/\1/')
```

### Package Versioning

Format: `{MajorVersion}.{MinorVersion}.{PatchVersion}`

- Matches Datadog SDK version
- Example: `3.5.0`

## Troubleshooting

### iOS Build Failures

1. **Check Xcode version**: Ensure `XCODE_VERSION` matches available version in runner image
2. **Verify global.json**: Check if temporary `global.json` was created correctly
3. **Review workload installation**: Ensure iOS workload installed for correct SDK version
4. **Check XCFramework download**: Verify frameworks were downloaded and cached

### Android Build Failures

1. **Check Kotlin dependencies**: Verify `AndroidIgnoredJavaDependency` entries are correct
2. **Review androidx.tracing**: Ensure `Xamarin.AndroidX.Tracing.Tracing.Ktx 1.3.0.2` is referenced
3. **Validate Directory.Build.props**: Check `DatadogSdkVersion` and `AndroidVerifyJavaDependencies` settings

### Package Combining Failures

1. **Verify all source packages exist**: Check that net8, net9, and net10 packages were created
2. **Inspect package structure**: Use `unzip -l` to verify framework folders
3. **Check zip/unzip**: Ensure zip utility is available (usually pre-installed)

## Resources

- [GitHub Actions Runner Images - macOS 15](https://github.com/actions/runner-images/blob/main/images/macos/macos-15-arm64-Readme.md)
- [.NET SDK global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json)
- [.NET MAUI iOS Workloads](https://learn.microsoft.com/en-us/dotnet/maui/ios/deployment/)
- [NuGet Package Structure](https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package)
