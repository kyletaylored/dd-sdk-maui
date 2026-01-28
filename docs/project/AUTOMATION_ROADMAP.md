---
layout: default
title: Automation Roadmap
parent: Project
nav_order: 4
description: "Path to full automation for Android dependency management in the Datadog MAUI SDK"
---

# Dependency Automation Roadmap

This document outlines the path to full automation for Android dependency management in the Datadog MAUI SDK bindings.

## Current State vs. Desired State

### Current Implementation (Basic)

**Script**: `scripts/generate-android-dependencies.sh`

**What it does**:
1. ✅ Fetches POM file from Maven Central
2. ✅ Parses `<dependencies>` section
3. ✅ Filters out test-scope dependencies
4. ✅ Sets `Bind="false"` for non-Datadog artifacts
5. ✅ Outputs `AndroidMavenLibrary` XML snippets

**What it does NOT do**:
- ❌ Check if NuGet packages exist for dependencies
- ❌ Query package versions from NuGet.org
- ❌ Handle transitive dependencies from MAUI
- ❌ Generate `AndroidIgnoredJavaDependency` entries
- ❌ Generate `PackageReference` entries
- ❌ Make intelligent placement decisions (core vs. feature module)
- ❌ Validate against known dependency patterns

### Future Configuration-Driven System

**Configuration-Driven Approach**:

```yaml
# Dependencies categorized by action
download:
  - groupId: com.lyft.kronos
    artifactId: kronos-android

nuget:
  - groupId: com.google.code.gson
    artifactId: gson
    nugetPackage: GoogleGson

skip:
  - groupId: androidx.core
    artifactId: core
    reason: Transitive from Material/Navigation packages
```

**Advantages**:
1. ✅ Human-readable configuration
2. ✅ Explicit categorization of each dependency
3. ✅ Documents the "why" (reason field)
4. ✅ Easy to version control and review changes
5. ✅ Can be used by scripts to generate project files

## Recommended Enhanced System

### Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  1. POM Fetcher (Maven Central API)                        │
│     - Fetches POM for each Datadog module                  │
│     - Extracts compile/runtime dependencies                 │
│                                                             │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  2. Dependency Categorizer                                  │
│     - Checks deps-config.yaml for known mappings           │
│     - Queries NuGet.org API for Xamarin packages           │
│     - Identifies MAUI transitive dependencies              │
│     - Determines: nuget / skip / download / internal       │
│                                                             │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  3. Dependency Deduplicator                                 │
│     - Analyzes all modules to find shared dependencies     │
│     - Identifies dependencies used by 2+ feature modules   │
│     - Marks for centralization in core                     │
│                                                             │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  4. Project File Generator                                  │
│     - Core: PackageReference for shared NuGet deps         │
│     - Core: AndroidMavenLibrary for shared downloads       │
│     - Features: AndroidIgnoredJavaDependency for shared    │
│     - Features: Module-specific deps where needed          │
│     - All: AndroidIgnoredJavaDependency for MAUI-provided  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Configuration File: deps-config.yaml

Enhanced configuration file format with comprehensive metadata:

```yaml
# Shared dependencies centralized in core
shared_nuget:
  - maven:
      groupId: com.google.code.gson
      artifactId: gson
      version: 2.10.1
    nuget:
      packageId: GoogleGson
      version: 2.11.0
    notes: "Complex binding requirements - must use NuGet package"
    placement: core

  - maven:
      groupId: org.jetbrains
      artifactId: annotations
      version: 13.0
    nuget:
      packageId: Xamarin.Jetbrains.Annotations
      version: 26.0.1.1
      minVersion: 26.0.1.1
    notes: "Required version >= 26.0.1.1 due to Kotlin stdlib transitive deps"
    placement: core

  - maven:
      groupId: com.squareup.okhttp3
      artifactId: okhttp
      version: 4.12.0
    nuget:
      packageId: Square.OkHttp3
      version: 4.12.0
    placement: core

# Dependencies covered by MAUI (skip in all projects)
maui_transitive:
  - maven:
      groupId: androidx.core
      artifactId: core
    reason: "Transitive from Material/Navigation packages in MAUI"
    mauiPackage: "Microsoft.Maui.Controls includes androidx.core transitively"

  - maven:
      groupId: androidx.fragment
      artifactId: fragment
    reason: "Transitive from Material/Navigation packages"

  - maven:
      groupId: androidx.recyclerview
      artifactId: recyclerview
    reason: "Transitive from Material package"

  - maven:
      groupId: androidx.appcompat
      artifactId: appcompat
    reason: "Transitive from Material package"

  - maven:
      groupId: androidx.navigation
      artifactId: navigation-runtime
    reason: "Direct in MAUI Core (Xamarin.AndroidX.Navigation.Runtime)"

  - maven:
      groupId: androidx.navigation
      artifactId: navigation-fragment
    reason: "Direct in MAUI Core (Xamarin.AndroidX.Navigation.Fragment)"

  - maven:
      groupId: com.google.android.material
      artifactId: material
    reason: "Direct in MAUI Core (Xamarin.Google.Android.Material)"

  - maven:
      groupId: org.jetbrains.kotlin
      artifactId: kotlin-stdlib
    reason: "Embedded in SDK, also available as Xamarin.Kotlin.StdLib"

  - maven:
      groupId: org.jetbrains.kotlin
      artifactId: kotlin-reflect
    reason: "Embedded in SDK, also available as Xamarin.Kotlin.Reflect"

# Dependencies without NuGet bindings (download AAR/JAR)
download:
  - maven:
      groupId: com.lyft.kronos
      artifactId: kronos-android
    reason: "No NuGet binding available"
    placement: core

  - maven:
      groupId: com.lyft.kronos
      artifactId: kronos-java
    reason: "No NuGet binding available"
    placement: core

  - maven:
      groupId: org.jctools
      artifactId: jctools-core
    reason: "No NuGet binding available"
    placement: trace  # Used only by trace module

  - maven:
      groupId: com.google.re2j
      artifactId: re2j
    reason: "No NuGet binding available"
    placement: trace  # Used only by trace module

  - maven:
      groupId: io.opentracing
      artifactId: opentracing-api
    reason: "No NuGet binding available"
    notes: "Only needed for trace-otel integration"

  - maven:
      groupId: androidx.compose.runtime
      artifactId: runtime
    reason: "Compose support - check if MAUI includes it"
    placement: session-replay

  - maven:
      groupId: androidx.metrics
      artifactId: metrics-performance
    reason: "Not available on Maven Central (Google Maven only)"
    action: ignore  # Can't download, not critical

# Module-specific NuGet dependencies (not shared)
module_specific_nuget:
  - maven:
      groupId: androidx.work
      artifactId: work-runtime
    nuget:
      packageId: Xamarin.AndroidX.Work.Runtime
      version: 2.10.0
    usedBy: [core, rum, logs, trace, session-replay, ndk, webview, flags]
    notes: "Actually used by all modules - should be in shared?"

  - maven:
      groupId: androidx.recyclerview
      artifactId: recyclerview
    nuget:
      packageId: Xamarin.AndroidX.RecyclerView
      version: 1.3.2.7
    usedBy: [rum]
    notes: "Only RUM needs RecyclerView directly"

# Internal Datadog cross-references
internal:
  - maven:
      groupId: com.datadoghq
      artifactId: dd-sdk-android-core
    action: project_reference
    notes: "All feature modules reference core"

  - maven:
      groupId: com.datadoghq
      artifactId: dd-sdk-android-internal
    action: project_reference
    notes: "Core references internal"

# Test dependencies (always ignore)
test:
  - groupId: org.junit.platform
  - groupId: org.junit.jupiter
  - groupId: org.junit.vintage
  - groupId: org.mockito
  - groupId: org.mockito.kotlin
  - groupId: org.assertj
  - groupId: com.github.xgouchet.Elmyr
```

## Implementation Plan

### Phase 1: Enhanced Configuration (Week 1)

1. **Create comprehensive deps-config.yaml**
   - Add placement information
   - Add NuGet version mappings
   - Add usage patterns (shared vs. module-specific)

2. **Create validation script**
   ```bash
   ./scripts/validate-deps-config.sh
   ```
   - Validates YAML syntax
   - Checks for duplicate entries
   - Warns about missing required fields

### Phase 2: Dependency Analyzer (Week 2)

1. **Create analyzer script**
   ```bash
   ./scripts/analyze-dependencies.sh <version>
   ```
   - Fetches all POM files for Datadog modules
   - Extracts all dependencies with versions
   - Cross-references with deps-config.yaml
   - Identifies:
     - ✅ Known dependencies (in config)
     - ⚠️  Unknown dependencies (need categorization)
     - ❌ Conflicting versions

2. **Output format**:
   ```
   Analyzing Datadog Android SDK v3.5.0

   ✅ Known Dependencies (45)
   ⚠️  Unknown Dependencies (3):
      - androidx.new.library:new-component:1.0.0
        Action: Needs categorization
        Suggestion: Check NuGet.org for Xamarin.AndroidX.New.Library

   ❌ Version Conflicts (1):
      - androidx.lifecycle:lifecycle-runtime
        Core: 2.8.5 | RUM: 2.8.7
        Resolution: Update core to 2.8.7
   ```

### Phase 3: Project File Generator (Week 3)

1. **Create generator script**
   ```bash
   ./scripts/generate-binding-projects.sh <version>
   ```
   - Reads deps-config.yaml
   - Reads POM files for all modules
   - Generates complete .csproj files:
     - Core with shared dependencies
     - Feature modules with appropriate references
     - Correct placement of all dependency types

2. **Dry-run mode**:
   ```bash
   ./scripts/generate-binding-projects.sh --dry-run 3.5.0
   ```
   - Shows what would be generated
   - Allows review before applying

3. **Diff mode**:
   ```bash
   ./scripts/generate-binding-projects.sh --diff 3.5.0
   ```
   - Shows changes from current .csproj files
   - Useful for updates

### Phase 4: NuGet Discovery (Week 4)

1. **Create NuGet API client**
   ```bash
   ./scripts/find-nuget-package.sh androidx.example:example-lib
   ```
   - Queries NuGet.org API
   - Searches for Xamarin.AndroidX.*, Xamarin.*, GoogleGson, etc.
   - Returns package ID and latest version

2. **Integrate into analyzer**
   - Automatically suggest NuGet packages for unknown dependencies
   - Check if newer versions available

### Phase 5: MAUI Dependency Checker (Week 5)

1. **Create MAUI analyzer**
   ```bash
   ./scripts/check-maui-dependencies.sh
   ```
   - Analyzes MAUI's transitive dependencies
   - Compares with Datadog SDK dependencies
   - Identifies overlaps

2. **Output**:
   ```
   MAUI Transitive Dependencies:
   ✅ androidx.core:core (1.13.0) - Already in skip list
   ⚠️  androidx.compose.ui:ui (1.5.0) - Not in config, used by session-replay
       Suggestion: Add to maui_transitive or verify if needed
   ```

## Success Metrics

The automated system should achieve:

1. **Correctness**: 100% of dependencies correctly categorized
2. **Completeness**: Zero build errors related to missing/duplicate dependencies
3. **Maintainability**: SDK version updates take < 1 hour (vs. current manual process)
4. **Documentation**: All decisions captured in deps-config.yaml with rationale
5. **Validation**: Automated checks prevent misconfigurations

## Migration Path

### From Current State

1. **Keep existing scripts** for backward compatibility
2. **Run new analyzer** against current configuration
3. **Validate** that analyzer matches current .csproj files
4. **Document discrepancies** and resolve them
5. **Switch to generator** for next SDK version update

### For Next SDK Update (3.6.0)

1. Run analyzer with new version
2. Review unknown dependencies
3. Update deps-config.yaml with categorizations
4. Run generator with --diff to review changes
5. Apply changes and test
6. Document any new patterns learned

## Open Questions

### 1. How to handle version conflicts?

**Options**:
- A) Always use highest version (safest)
- B) Allow configuration to pin versions
- C) Warn and require manual resolution

**Recommendation**: A + C (use highest, but warn about significant version jumps)

### 2. When to check for newer NuGet packages?

**Options**:
- A) Every run (slow, but always current)
- B) Only on SDK version updates
- C) Manual trigger with --check-updates flag

**Recommendation**: C (manual trigger to avoid slow runs)

### 3. How to handle breaking changes in NuGet packages?

**Example**: Xamarin.AndroidX.* packages change namespaces

**Solution**: Add to deps-config.yaml:
```yaml
shared_nuget:
  - maven:
      groupId: androidx.core
      artifactId: core
    nuget:
      packageId: Xamarin.AndroidX.Core
      version: 1.15.0.2
      breaking_changes:
        - version: 1.14.0
          note: "Namespace changed from AndroidX.Core to Xamarin.AndroidX.Core"
```

### 4. Should we auto-commit generated files?

**Options**:
- A) Yes, with descriptive commit message
- B) No, always require manual review
- C) Dry-run by default, --apply flag to commit

**Recommendation**: C (safe default, easy to apply when ready)

## Comparison: Manual vs. Automated

| Task | Manual Time | Automated Time | Savings |
|------|-------------|----------------|---------|
| Fetch POMs | 10 min | Instant | 10 min |
| Categorize deps | 30 min | Instant | 30 min |
| Check NuGet | 20 min | Instant | 20 min |
| Generate XML | 40 min | Instant | 40 min |
| Validate & test | 30 min | 30 min | 0 min |
| **Total** | **2h 10min** | **30 min** | **1h 40min** |

**ROI**: After 3 SDK updates, the automation effort pays for itself.

## Next Steps

1. ✅ Document current patterns (DONE - this document)
2. ⏭️  Create enhanced deps-config.yaml
3. ⏭️  Build dependency analyzer
4. ⏭️  Build project file generator
5. ⏭️  Test on next SDK version update
6. ⏭️  Iterate based on learnings

---

**Document Version**: 1.0
**Last Updated**: 2026-01-16
**Status**: Roadmap (Not Yet Implemented)
