# Datadog Android Bindings for .NET MAUI

This directory contains separate .NET binding projects for each Datadog Android SDK module.

## Project Structure

Each Datadog Android module has its own binding project, named to match the Maven artifact:

- **dd-sdk-android-internal/** - Internal framework (no dependencies)
- **dd-sdk-android-core/** - Core SDK module (depends on Internal)
- **dd-sdk-android-rum/** - Real User Monitoring (depends on Core)
- **dd-sdk-android-logs/** - Log collection (depends on Core)
- **dd-sdk-android-trace/** - APM tracing (depends on Core)
- **dd-sdk-android-ndk/** - Native crash reporting (depends on Core)
- **dd-sdk-android-session-replay/** - Session replay (depends on Core)
- **dd-sdk-android-webview/** - WebView tracking (depends on Core)
- **dd-sdk-android-flags/** - Feature flags (depends on Core)
- **Datadog.MAUI.Android.Binding.csproj** - Meta-package that references all individual packages

## How Android Bindings Work

Unlike iOS which requires downloaded XCFrameworks, Android bindings use **AndroidMavenLibrary** to automatically download and bind Maven artifacts at build time.

Each binding project:
1. References the Maven artifact via `<AndroidMavenLibrary>`
2. Automatically downloads the `.aar` from Maven Central
3. Generates C# bindings from the Java/Kotlin code
4. Includes a `Transforms/Metadata.xml` for fixing binding issues

## Building

Build individual projects:
```bash
dotnet build dd-sdk-android-internal/dd-sdk-android-internal.csproj
dotnet build dd-sdk-android-core/dd-sdk-android-core.csproj
dotnet build dd-sdk-android-rum/dd-sdk-android-rum.csproj
# etc.
```

Build all projects at once (meta-package):
```bash
dotnet build Datadog.MAUI.Android.Binding.csproj
```

## Metadata Transforms

When bindings generate errors, use `Transforms/Metadata.xml` to fix them:

```xml
<metadata>
  <!-- Remove problematic classes -->
  <remove-node path="/api/package[@name='com.datadog.android.internal']/class[@name='ProblematicClass']" />

  <!-- Rename conflicts -->
  <attr path="/api/package[@name='com.datadog.android']/class[@name='Foo']" name="managedName">DatadogFoo</attr>
</metadata>
```

## NuGet Packages

Each project can be packaged individually:
```bash
dotnet pack dd-sdk-android-core/dd-sdk-android-core.csproj --configuration Release
```

This allows consumers to install only the modules they need:
```xml
<PackageReference Include="Datadog.MAUI.Android.Core" Version="3.5.0" />
<PackageReference Include="Datadog.MAUI.Android.RUM" Version="3.5.0" />
```

Or use the convenience meta-package:
```xml
<PackageReference Include="Datadog.MAUI.Android.Binding" Version="3.5.0" />
```

## Dependencies

Dependencies are automatically resolved by `AndroidMavenLibrary`:
- **Kotlin**: kotlin-stdlib 2.0.21
- **Network**: OkHttp 4.12.0
- **JSON**: Gson 2.10.1
- **Time Sync**: Kronos 0.0.1-alpha11
- **AndroidX**: annotation, collection, work-runtime

All transitive dependencies are marked with `Bind="false"` to avoid duplicate bindings.

## Current Status

All 9 core and feature binding projects created:
- ✅ dd-sdk-android-internal
- ✅ dd-sdk-android-core
- ✅ dd-sdk-android-rum
- ✅ dd-sdk-android-logs
- ✅ dd-sdk-android-trace
- ✅ dd-sdk-android-ndk
- ✅ dd-sdk-android-session-replay
- ✅ dd-sdk-android-webview
- ✅ dd-sdk-android-flags

## Integration Packages (Optional)

Additional integration packages are available but not included in the core binding:
- dd-sdk-android-okhttp - OkHttp interceptor
- dd-sdk-android-okhttp-otel - OkHttp + OpenTelemetry
- dd-sdk-android-rum-coroutines - Kotlin Coroutines support
- dd-sdk-android-trace-coroutines - Trace + Coroutines
- dd-sdk-android-compose - Jetpack Compose support
- dd-sdk-android-session-replay-compose - Session Replay + Compose
- dd-sdk-android-session-replay-material - Session Replay + Material Design
- And more...

These can be added as separate binding projects if needed.

## Next Steps

1. Test build each individual project
2. Resolve any binding errors using Metadata transforms
3. Test build the meta-package
4. Create integration tests
5. Publish to NuGet.org or internal feed
