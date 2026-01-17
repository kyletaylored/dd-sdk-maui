---
layout: default
title: Dependency Quick Reference
nav_order: 6
description: "Quick lookup table for handling Datadog Android SDK dependencies in .NET MAUI bindings"
permalink: /dependency-quick-reference
---

# Android Dependency Quick Reference

Quick lookup table for handling Datadog Android SDK dependencies in .NET MAUI bindings.

## Decision Tree

```
Is this dependency needed?
├─ Yes → Where does it come from?
│  ├─ Maven artifact com.datadoghq:*
│  │  └─ Use <ProjectReference> to other binding project
│  │
│  ├─ Has Xamarin/AndroidX NuGet package?
│  │  ├─ Used by multiple modules?
│  │  │  ├─ Yes → Add to core as PackageReference
│  │  │  │        Add AndroidIgnoredJavaDependency in feature modules
│  │  │  └─ No  → Add PackageReference only where needed
│  │  │
│  │  └─ No NuGet package available
│  │     └─ Add AndroidMavenLibrary with Bind="false"
│  │
│  └─ Already in MAUI?
│     └─ Add AndroidIgnoredJavaDependency everywhere
│
└─ No (test/provided scope)
   └─ Don't include anywhere
```

## Common Dependencies Quick Lookup

| Maven Artifact | Action | Location | Version |
|----------------|--------|----------|---------|
| **Shared Dependencies (in core)** |
| `com.google.code.gson:gson` | NuGet | Core | 2.11.0 |
| `org.jetbrains:annotations` | NuGet | Core | 26.0.1.1 |
| `com.squareup.okhttp3:okhttp` | NuGet | Core | 4.12.0 |
| **MAUI Transitive (skip everywhere)** |
| `androidx.core:core` | Skip | - | - |
| `androidx.fragment:fragment` | Skip | - | - |
| `androidx.recyclerview:recyclerview` | Skip | - | - |
| `androidx.appcompat:appcompat` | Skip | - | - |
| `androidx.navigation:*` | Skip | - | - |
| `com.google.android.material:material` | Skip | - | - |
| `org.jetbrains.kotlin:kotlin-stdlib` | Skip | - | - |
| **Module-Specific (where needed)** |
| `androidx.work:work-runtime` | NuGet | Where needed | 2.10.0 |
| `com.lyft.kronos:*` | Maven | Core | 0.0.1-alpha11 |
| **Internal Cross-Refs** |
| `com.datadoghq:dd-sdk-android-*` | ProjectRef | - | - |

## File Templates

### Core Project (dd-sdk-android-core.csproj)

```xml
<ItemGroup>
  <!-- Shared dependencies as NuGet PackageReferences -->
  <PackageReference Include="GoogleGson" Version="2.11.0" />
  <PackageReference Include="Xamarin.Jetbrains.Annotations" Version="26.0.1.1" />
  <PackageReference Include="Square.OkHttp3" Version="4.12.0" />
</ItemGroup>

<ItemGroup>
  <!-- Maven dependencies for runtime (Bind="false" since NuGet provides bindings) -->
  <AndroidMavenLibrary Include="com.lyft.kronos:kronos-android" Version="0.0.1-alpha11" Bind="false" />
</ItemGroup>

<ItemGroup>
  <!-- Dependencies covered by MAUI or NuGet - ignore Maven warnings -->
  <AndroidIgnoredJavaDependency Include="org.jetbrains.kotlin:kotlin-stdlib:2.0.21" />
</ItemGroup>
```

### Feature Module (dd-sdk-android-rum.csproj, etc.)

```xml
<ItemGroup>
  <!-- Reference to core provides shared dependencies transitively -->
  <ProjectReference Include="..\dd-sdk-android-core\dd-sdk-android-core.csproj" />
</ItemGroup>

<ItemGroup>
  <!-- Maven dependencies for this module -->
  <AndroidMavenLibrary Include="com.datadoghq:dd-sdk-android-rum" Version="$(DatadogSdkVersion)" />
  <!-- Datadog cross-refs with Bind="false" (handled by ProjectReference) -->
  <AndroidMavenLibrary Include="com.datadoghq:dd-sdk-android-core" Version="3.5.0" Bind="false" />
</ItemGroup>

<ItemGroup>
  <!-- Shared dependencies provided by dd-sdk-android-core -->
  <AndroidIgnoredJavaDependency Include="com.google.code.gson:gson:2.10.1" />
  <AndroidIgnoredJavaDependency Include="org.jetbrains:annotations:13.0" />
  <AndroidIgnoredJavaDependency Include="com.squareup.okhttp3:okhttp:4.12.0" />

  <!-- Dependencies covered by MAUI -->
  <AndroidIgnoredJavaDependency Include="org.jetbrains.kotlin:kotlin-stdlib:2.0.21" />

  <!-- Test dependencies (always ignore) -->
  <AndroidIgnoredJavaDependency Include="org.junit.platform:junit-platform-launcher:1.9.3" />
</ItemGroup>

<ItemGroup>
  <!-- Module-specific NuGet dependencies (if any) -->
  <PackageReference Include="Xamarin.AndroidX.RecyclerView" Version="1.3.2.7" />
</ItemGroup>
```

## Error Code Cheat Sheet

| Error | Meaning | Fix |
|-------|---------|-----|
| `Type X is defined multiple times` | Duplicate dependency bound in multiple projects | Move to core + AndroidIgnoredJavaDependency |
| `Java dependency 'X' is not satisfied` | Missing dependency or wrong ignore | Add AndroidIgnoredJavaDependency if transitive, else add PackageReference |
| `Package downgrade: X from Y to Z` | Version conflict in transitive deps | Update to higher version in core |
| `does not implement inherited abstract member` | Binding issue (usually Gson) | Use GoogleGson NuGet, not Maven binding |

## Version Update Checklist

When updating Datadog SDK version:

- [ ] Run `./scripts/generate-android-dependencies.sh dd-sdk-android-core <VERSION>`
- [ ] Compare output with current .csproj files
- [ ] Check for new dependencies → Categorize and place appropriately
- [ ] Check for removed dependencies → Clean up references
- [ ] Update version numbers in all binding projects
- [ ] Build all projects: `dotnet build Datadog.MAUI.Android.Binding.sln`
- [ ] Build sample app: `dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj`
- [ ] Test app on device/emulator
- [ ] Update version in this document

## Useful Commands

```bash
# Generate dependency list from POM
./scripts/generate-android-dependencies.sh dd-sdk-android-core 3.5.0

# Check for duplicate dependencies
dotnet build samples/DatadogMauiSample/DatadogMauiSample.csproj 2>&1 | grep "defined multiple times"

# Search for NuGet package
# Visit: https://www.nuget.org/packages?q=<search-term>

# Check MAUI dependencies
dotnet list package --include-transitive

# Clean build
dotnet clean && dotnet build
```

## Common Patterns

### Pattern 1: New shared dependency discovered

1. Add `PackageReference` to core (if NuGet exists)
2. Add `AndroidIgnoredJavaDependency` to ALL feature modules
3. Build and test

### Pattern 2: Module-specific dependency

1. Add `PackageReference` or `AndroidMavenLibrary` ONLY to that module
2. Do NOT add `AndroidIgnoredJavaDependency` in other modules
3. Build and test

### Pattern 3: Datadog internal reference

1. Add `<ProjectReference>` from dependent to provider
2. Add `AndroidMavenLibrary` with `Bind="false"` for Maven artifact
3. Do NOT add `PackageReference` (provided by ProjectReference)
4. Build and test

---

**Last Updated**: 2026-01-16
**Datadog SDK Version**: 3.5.0
