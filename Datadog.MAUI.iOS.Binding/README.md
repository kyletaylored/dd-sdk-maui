# Datadog iOS Bindings for .NET MAUI

This directory contains separate .NET binding projects for each Datadog iOS SDK framework.

## Project Structure

Each Datadog iOS framework has its own binding project, named to match the xcframework:

- **DatadogInternal/** - Core internal framework (no dependencies)
- **DatadogCore/** - Core SDK framework (depends on DatadogInternal)
- **DatadogRUM/** - Real User Monitoring (depends on DatadogCore)
- **DatadogLogs/** - Log collection (depends on DatadogCore)
- **DatadogTrace/** - APM tracing (depends on DatadogCore)
- **DatadogCrashReporting/** - Crash reporting (depends on DatadogCore)
- **DatadogSessionReplay/** - Session replay (depends on DatadogCore + DatadogRUM)
- **DatadogWebViewTracking/** - WebView tracking (depends on DatadogCore + DatadogRUM)
- **DatadogFlags/** - Feature flags (depends on DatadogCore)
- **OpenTelemetryApi/** - OpenTelemetry API (no dependencies)
- **Datadog.MAUI.iOS.Binding.csproj** - Meta-package that references all individual packages

## XCFrameworks

The native XCFrameworks are stored in the `artifacts/` directory (git-ignored).

Download them using:
```bash
../scripts/download-ios-frameworks.sh 3.5.0 .
```

## Generating Bindings

Bindings are generated using Objective Sharpie and stored in the `Generated/` directory (git-ignored).

To regenerate all bindings:
```bash
../scripts/generate-ios-bindings.sh .
```

Generated bindings are then manually reviewed and copied into each project's `ApiDefinition.cs` and `StructsAndEnums.cs` files.

## Manual Cleanup Required

After Sharpie generation, the following manual cleanup is typically needed:

1. **Remove invalid using statements**: Remove `using Datadog*;` lines that reference native framework namespaces
2. **Add cross-project references**: Add `using DatadogMaui.iOS.DatadogInternal;` to DatadogCore, etc.
3. **Remove [Verify] attributes**: Review and remove all `[Verify(...)]` attributes after confirming correctness
4. **Fix duplicate definitions**: Resolve any duplicate constructors or members
5. **Add missing usings**: Add `using Foundation;`, `using WebKit;`, etc. as needed
6. **Resolve ambiguous types**: Disambiguate types that exist in multiple frameworks

## Building

Build individual projects:
```bash
dotnet build DatadogInternal/DatadogInternal.csproj
dotnet build DatadogCore/DatadogCore.csproj
dotnet build DatadogRUM/DatadogRUM.csproj
# etc.
```

Build all projects at once (meta-package):
```bash
dotnet build Datadog.MAUI.iOS.Binding.csproj
```

## NuGet Packages

Each project can be packaged individually:
```bash
dotnet pack DatadogCore/DatadogCore.csproj --configuration Release
```

This allows consumers to install only the frameworks they need:
```xml
<PackageReference Include="Datadog.MAUI.iOS.Core" Version="3.5.0" />
<PackageReference Include="Datadog.MAUI.iOS.RUM" Version="3.5.0" />
```

Or use the convenience meta-package:
```xml
<PackageReference Include="Datadog.MAUI.iOS.Binding" Version="3.5.0" />
```

## Current Status

- ✅ DatadogInternal: Built successfully
- ⚠️ DatadogCore: Needs manual cleanup (Verify attributes, duplicate constructors)
- ⚠️ DatadogRUM: Needs manual cleanup
- ⚠️ DatadogLogs: Needs manual cleanup
- ⚠️ DatadogTrace: Needs manual cleanup
- ⚠️ DatadogCrashReporting: Needs manual cleanup
- ⚠️ DatadogSessionReplay: Needs manual cleanup
- ⚠️ DatadogWebViewTracking: Needs manual cleanup
- ⚠️ DatadogFlags: Sharpie generation failed (needs manual binding or re-run with updated Sharpie)
- ⚠️ OpenTelemetryApi: Needs binding generation

## Next Steps

1. Manually clean up generated bindings for each framework
2. Test build each project individually
3. Create consolidated solution file
4. Set up CI/CD to automate building and packaging
5. Publish to NuGet.org or internal feed
