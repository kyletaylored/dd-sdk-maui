# Mapping File Uploads for MAUI Android

When building MAUI Android apps with code obfuscation (ProGuard/R8) or native code (NDK), you'll need to upload mapping files to Datadog to get readable stack traces in RUM Error Tracking.

## Why Mapping Files are Needed

- **ProGuard/R8 Mapping**: Deobfuscates minified class/method names in crash reports
- **NDK Symbol Files**: Resolves native crash stack traces from C/C++ code

## Prerequisites: Enable R8 Code Shrinking

**R8 mapping files are only generated when code shrinking is enabled.** Add this to your MAUI Android project's `.csproj` file:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <!-- Enable R8 code shrinker -->
  <AndroidEnableR8>True</AndroidEnableR8>
  <AndroidLinkTool>r8</AndroidLinkTool>

  <!-- Link mode: SdkOnly (safer) or Full (more aggressive shrinking) -->
  <AndroidLinkMode>Full</AndroidLinkMode>

  <!-- Optional: Custom ProGuard rules file -->
  <!-- <AndroidProguardConfig>proguard.cfg</AndroidProguardConfig> -->
</PropertyGroup>
```

**Link Mode Options:**
- `SdkOnly`: Only shrinks SDK assemblies, leaves your code untouched (safer, less aggressive)
- `Full`: Shrinks both SDK and your app code (more aggressive, may require custom ProGuard rules)

**After enabling R8**, build your Release configuration:
```bash
dotnet build -c Release -f net9.0-android
```

The mapping file will be generated at one of these locations:

**Primary location** (standard MAUI builds):
```
bin/Release/net9.0-android/mapping.txt
bin/Release/net10.0-android/mapping.txt
```

**Alternative location** (intermediate build output):
```
obj/Release/net9.0-android/lp/map.cache/mapping.txt
obj/Release/net10.0-android/lp/map.cache/mapping.txt
```

**Note**:
- The `bin/` location is the final output directory and is typically more reliable
- Without R8 enabled, no mapping file is generated and stack traces will already be readable (but your APK will be larger)
- If publishing as an Android App Bundle (`.aab`) to Google Play, the mapping file is often uploaded automatically

### Custom ProGuard Rules (Optional)

If you encounter runtime crashes after enabling R8 (common with reflection-heavy code), you may need custom ProGuard rules.

Create a `proguard.cfg` file in your project root:
```
# Keep Datadog SDK classes
-keep class com.datadog.** { *; }

# Keep classes used via reflection
-keep class com.yourcompany.yourapp.models.** { *; }

# Keep MAUI generated code
-keep class crc64** { *; }
-keep class mono.** { *; }
```

Then reference it in your `.csproj`:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidProguardConfig>proguard.cfg</AndroidProguardConfig>
</PropertyGroup>
```

**Common issues requiring ProGuard rules:**
- JSON serialization classes being stripped
- Classes accessed via reflection (dependency injection, plugins)
- Third-party SDK classes that R8 incorrectly considers unused

## Why the Gradle Plugin Doesn't Work

The native Android `dd-sdk-android-gradle-plugin` is **not compatible** with MAUI projects because:
- MAUI uses MSBuild, not Gradle directly
- Gradle plugin tasks cannot be bound to .NET
- MAUI's build process wraps Gradle internally

## Recommended Approach: datadog-ci CLI

The **datadog-ci** command-line tool is the recommended way to upload mapping files from MAUI projects.

### Which datadog-ci command to use?

| Command | Use Case | MAUI Compatibility |
|---------|----------|-------------------|
| `datadog-ci flutter-symbols upload` | Android/iOS symbols (can handle Android mappings, NDK symbols, iOS dSYMs) | ✅ **Use for MAUI Android** |
| `datadog-ci dsyms upload` | iOS/macOS dSYM files only | ✅ Use for MAUI iOS |

**For MAUI projects, use**:
- `datadog-ci flutter-symbols upload --android-mapping` for Android R8/ProGuard mapping files
- `datadog-ci flutter-symbols upload --android-mapping --ndk-symbol-files` for combined mapping + NDK symbols
- `datadog-ci dsyms upload` for iOS dSYM files (separate documentation needed)

**Important**: While the command is named `flutter-symbols`, it works for any Android/iOS app. The key is providing explicit paths and ensuring `--service-name`, `--flavor`, and `--version` exactly match what your app sends to Datadog in RUM events.

## Critical: Parameter Matching

⚠️ **The upload will silently succeed but symbols won't be found if parameters don't match exactly.**

Because `datadog-ci flutter-symbols` can't automatically determine your app's metadata, you **must** ensure these values match exactly:

| Parameter | Must Match | How to Find in MAUI |
|-----------|-----------|---------------------|
| `--service-name` | The `serviceName` in your Datadog SDK configuration | Check your `Datadog.Initialize()` call or defaults to bundle ID |
| `--flavor` | Your build configuration | Usually `release` for Release builds, `debug` for Debug |
| `--version` | The version sent in RUM events | Usually your `ApplicationVersion` from .csproj or `CFBundleShortVersionString` |

**Example of checking your service name in code:**
```csharp
// In your MAUI app startup
Datadog.Initialize(
    configuration: new DatadogConfiguration(
        clientToken: "...",
        environment: "prod",
        serviceName: "com.mycompany.myapp" // ← This must match --service-name
    )
);
```

If not explicitly set, the default service name is your bundle identifier:
- Android: `com.mycompany.myapp` (from `ApplicationId` in .csproj)
- iOS: `com.mycompany.myapp` (from `CFBundleIdentifier` in Info.plist)

### 1. Install datadog-ci

```bash
npm install -g @datadog/datadog-ci
```

### 2. Set Environment Variables

```bash
export DATADOG_API_KEY=<your-api-key>
export DATADOG_SITE=datadoghq.com  # or datadoghq.eu, etc.
```

### 3. Upload ProGuard/R8 Mapping

After building your Release configuration:

```bash
datadog-ci flutter-symbols upload \
  --service-name <your-service-name> \
  --flavor release \
  --version <your-version> \
  --android-mapping \
  --android-mapping-location <path-to-mapping.txt>
```

**Example**:
```bash
datadog-ci flutter-symbols upload \
  --service-name com.mycompany.myapp \
  --flavor release \
  --version 1.0.0 \
  --android-mapping \
  --android-mapping-location bin/Release/net9.0-android/mapping.txt
```

**Mapping file locations** (MAUI Android Release builds):
```
<project>/bin/Release/net9.0-android/mapping.txt        (primary)
<project>/bin/Release/net10.0-android/mapping.txt       (primary)
<project>/obj/Release/net9.0-android/lp/map.cache/mapping.txt  (alternative)
<project>/obj/Release/net10.0-android/lp/map.cache/mapping.txt (alternative)
```

**Critical**: The `--service-name` must **exactly match** the service name configured in your Datadog MAUI SDK initialization. By default, this is your app's bundle identifier (e.g., `com.mycompany.myapp`).

### 4. Upload NDK Symbol Files

If your MAUI app includes native libraries (C/C++ code via P/Invoke or native bindings):

```bash
datadog-ci flutter-symbols upload \
  --service-name <your-service-name> \
  --flavor release \
  --version <your-version> \
  --ndk-symbol-files <path-to-symbol-directory>
```

**Example with specific architecture**:
```bash
datadog-ci flutter-symbols upload \
  --service-name com.mycompany.myapp \
  --flavor release \
  --version 1.0.0 \
  --ndk-symbol-files obj/Release/net9.0-android/android/assets/obj/local/arm64-v8a
```

**NDK symbols locations** in MAUI Android builds:

For native libraries compiled with debug symbols:
```
<project>/obj/Release/net9.0-android/android/assets/obj/local/arm64-v8a/
<project>/obj/Release/net9.0-android/android/assets/obj/local/armeabi-v7a/
<project>/obj/Release/net9.0-android/android/assets/obj/local/x86/
<project>/obj/Release/net9.0-android/android/assets/obj/local/x86_64/
```

Or for extracted .so files:
```
<project>/obj/Release/net9.0-android/android/libs/
```

**Note**: Most MAUI apps don't have NDK symbols unless you're:
- Using native libraries via P/Invoke
- Including third-party native SDKs
- Using Datadog's NDK crash reporting module

### 5. Combined Upload (Mapping + NDK)

You can upload both mapping files and NDK symbols in a single command:

```bash
datadog-ci flutter-symbols upload \
  --service-name com.mycompany.myapp \
  --flavor release \
  --version 1.0.0 \
  --android-mapping \
  --android-mapping-location bin/Release/net9.0-android/mapping.txt \
  --ndk-symbol-files obj/Release/net9.0-android/android/assets/obj/local/arm64-v8a \
  --ndk-symbol-files obj/Release/net9.0-android/android/assets/obj/local/armeabi-v7a
```

This is the most efficient approach for complete crash symbolication coverage.

**Note on multiple architectures**: You can specify `--ndk-symbol-files` multiple times to upload symbols for different ABIs (arm64-v8a, armeabi-v7a, x86, x86_64).

## Troubleshooting MAUI Build Output Paths

### If mapping.txt doesn't exist

**Problem**: `mapping.txt` file is not generated after building.

**Solutions**:

1. **Verify R8 is enabled** in your `.csproj`:
   ```bash
   grep -A3 "AndroidEnableR8" YourProject.csproj
   # Should show: <AndroidEnableR8>True</AndroidEnableR8>
   ```

2. **Build in Release configuration**:
   ```bash
   dotnet build -c Release -f net9.0-android
   ```
   Debug builds typically don't enable code shrinking.

3. **Check build output** for R8 execution:
   ```bash
   dotnet build -c Release -f net9.0-android -v detailed | grep -i "r8"
   # Should show R8 task execution
   ```

4. **Verify AndroidLinkMode**:
   - `AndroidLinkMode=None` → No mapping file generated
   - `AndroidLinkMode=SdkOnly` or `Full` → Mapping file generated

If the default paths don't work, here's how to find your files:

### Finding R8/ProGuard Mapping Files

Search for `mapping.txt` in your build output:
```bash
find . -name "mapping.txt" -type f | grep -E "(Release|release)"
```

Common MAUI Android locations (in order of preference):
1. `bin/Release/net9.0-android/mapping.txt` (primary, final build output)
2. `bin/Release/net10.0-android/mapping.txt` (primary, final build output)
3. `obj/Release/net9.0-android/lp/map.cache/mapping.txt` (intermediate)
4. `obj/Release/net10.0-android/lp/map.cache/mapping.txt` (intermediate)

### Finding NDK Symbol Files

Search for `.so` files with debug symbols:
```bash
find . -name "*.so" -type f | grep -E "(obj|libs)"
```

### Verify Files Are Valid

Check mapping file has content:
```bash
head -5 mapping.txt
# Should show obfuscation mappings like:
# com.myapp.MainActivity -> a:
#     void onCreate(android.os.Bundle) -> a
```

Check NDK symbols have debug info:
```bash
file libmylib.so
# Should show: ELF 64-bit LSB shared object, ARM aarch64, not stripped
```

## Alternative: MSBuild Integration

You can integrate the upload into your build process by adding a custom MSBuild target:

```xml
<!-- In your .csproj file -->
<Target Name="UploadMappingToDatadog" AfterTargets="Build" Condition="'$(Configuration)' == 'Release'">
  <Exec Command="datadog-ci flutter-symbols upload --service-name com.mycompany.myapp --flavor release --version $(ApplicationVersion) --android-mapping --android-mapping-location $(OutputPath)mapping.txt" />
</Target>
```

**Note**: `$(OutputPath)` resolves to `bin/Release/net9.0-android/` or similar, depending on the target framework.

**Prerequisites:**
- `datadog-ci` must be installed globally or in your CI environment
- Set `DATADOG_API_KEY` and `DATADOG_SITE` environment variables

## CI/CD Integration Examples

### GitHub Actions

```yaml
- name: Upload Mapping Files to Datadog
  env:
    DATADOG_API_KEY: ${{ secrets.DATADOG_API_KEY }}
    DATADOG_SITE: datadoghq.com
  run: |
    npm install -g @datadog/datadog-ci
    datadog-ci flutter-symbols upload \
      --service-name com.mycompany.myapp \
      --flavor release \
      --version ${{ github.ref_name }} \
      --android-mapping \
      --android-mapping-location MyApp/obj/Release/net9.0-android/lp/map.cache/mapping.txt
```

### Azure Pipelines

```yaml
- task: Npm@1
  inputs:
    command: 'custom'
    customCommand: 'install -g @datadog/datadog-ci'

- script: |
    datadog-ci flutter-symbols upload \
      --service-name com.mycompany.myapp \
      --flavor release \
      --version $(Build.BuildNumber) \
      --android-mapping \
      --android-mapping-location $(Build.SourcesDirectory)/MyApp/obj/Release/net9.0-android/lp/map.cache/mapping.txt
  env:
    DATADOG_API_KEY: $(DATADOG_API_KEY)
    DATADOG_SITE: datadoghq.com
```

## Manual Upload via Datadog UI

If automation isn't possible, you can manually upload mapping files:

1. Go to Datadog RUM → Error Tracking
2. Navigate to Source Maps/Mappings section
3. Upload your mapping.txt file with the corresponding version

## Resources

- [datadog-ci Documentation](https://github.com/DataDog/datadog-ci)
- [Datadog RUM Error Tracking](https://docs.datadoghq.com/real_user_monitoring/error_tracking/)
- [Android Source Code Mapping](https://docs.datadoghq.com/real_user_monitoring/error_tracking/android/)
