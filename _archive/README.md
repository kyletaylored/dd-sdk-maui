# Archived Android Bindings

This directory contains Android binding projects that are not included in the main MAUI SDK packages.

## dd-sdk-android-gradle-plugin

**Status**: Archived - Not compatible with MAUI

**Reason**: The Datadog Android Gradle Plugin is designed to work within Gradle's build system to upload mapping files (ProGuard/R8) and NDK symbol files to Datadog. This functionality is incompatible with MAUI's MSBuild-based build system.

**What it does**:
- Provides Gradle tasks like `uploadMappingRelease` and `uploadNdkSymbolFilesRelease`
- Integrates with Gradle build lifecycle
- Requires `datadog { }` configuration block in build.gradle

**Why it doesn't work for MAUI**:
1. MAUI uses MSBuild, not Gradle directly
2. Gradle plugin behavior (tasks, build hooks) cannot be bound to .NET
3. Configuration DSL is Gradle-specific
4. MAUI's Android build process wraps Gradle internally, not exposed to developers

**Alternatives for MAUI developers**:

For uploading mapping/symbol files from MAUI projects:

1. **datadog-ci CLI** (Recommended):
   ```bash
   npm install -g @datadog/datadog-ci
   datadog-ci flutter-symbols upload --service-name com.mycompany.myapp --flavor release --version 1.0.0 --android-mapping --android-mapping-location path/to/mapping.txt
   ```

2. **Datadog API** (Direct):
   - Use Datadog's Source Map API
   - Can be integrated as a custom MSBuild task

3. **Manual Upload**:
   - Via Datadog web UI under RUM → Error Tracking → Upload Mappings

**Why not create an MSBuild task for uploads?**

While technically possible to create an MSBuild task wrapper around `datadog-ci`, there are good reasons not to:

1. **Dependency management**: Would require Node.js/npm installation on build machines
2. **Cross-platform challenges**: Different Node.js installation paths on Windows/Mac/Linux
3. **CI/CD flexibility**: Most CI systems already have Node.js installed; developers prefer explicit upload steps they can control
4. **Separation of concerns**: Uploading symbols is a deployment/release concern, not a compilation concern
5. **Existing tooling**: `datadog-ci` is well-maintained and handles authentication, retries, and API versioning

**Recommended approach**: Use `datadog-ci android upload` as documented in `/docs/MAPPING_FILE_UPLOADS.md`

**Future consideration**:
If there's strong demand, a lightweight .NET global tool could be created that:
- Wraps Datadog's HTTP API directly (no Node.js dependency)
- Can be installed via `dotnet tool install`
- Provides MSBuild task integration
- Reads config from MSBuild properties or environment variables

However, this would duplicate effort since `datadog-ci` already provides this functionality robustly.
