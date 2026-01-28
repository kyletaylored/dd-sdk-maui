# Release Process for Datadog.MAUI.Symbols

This document describes how to release the Datadog.MAUI.Symbols package in a monorepo alongside other Datadog MAUI packages.

## Overview

The Symbols package has **independent versioning** from the main Datadog MAUI SDK packages. This allows:

- ✅ Independent releases without coordinating with SDK updates
- ✅ Faster iteration on symbol upload features
- ✅ Semantic versioning that reflects changes to the Symbols package only

## Version Management

### Main SDK Packages
- **Version Source**: `Directory.Build.props` → `<DatadogSdkVersion>3.5.0</DatadogSdkVersion>`
- **Tag Format**: `v3.5.0`
- **Packages**: Datadog.MAUI.Android.*, Datadog.MAUI.iOS.*, Datadog.MAUI.Plugin

### Symbols Package
- **Version Source**: `Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj` → `<Version>1.0.0</Version>`
- **Tag Format**: `symbols-v1.0.0`
- **Package**: Datadog.MAUI.Symbols only

## Release Process

### 1. Update Version

Edit `Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj`:

```xml
<PropertyGroup>
  <Version>1.1.0</Version>  <!-- Update this -->
</PropertyGroup>
```

Follow [Semantic Versioning](https://semver.org/):
- **Patch** (1.0.x): Bug fixes, documentation updates
- **Minor** (1.x.0): New features, backward compatible
- **Major** (x.0.0): Breaking changes

### 2. Update CHANGELOG

If you have a changelog, document the changes.

### 3. Commit and Push

```bash
git add Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj
git commit -m "chore(symbols): bump version to 1.1.0"
git push origin main
```

This triggers the **build-symbols.yml** workflow automatically due to path filters.

### 4. Create Release Tag

Once the build succeeds, create a tag:

```bash
git tag symbols-v1.1.0
git push origin symbols-v1.1.0
```

This triggers:
1. **build-symbols.yml** (builds the package)
2. **create-release** job (creates GitHub Release)
3. **publish-symbols-nuget.yml** (publishes to NuGet.org - if configured)

### 5. Verify Release

- Check GitHub Releases for `symbols-v1.1.0`
- Verify package appears on NuGet.org: https://www.nuget.org/packages/Datadog.MAUI.Symbols
- Test installation: `dotnet add package Datadog.MAUI.Symbols --version 1.1.0`

## Manual Publishing (Alternative)

If automatic publishing is not set up or you need to publish manually:

```bash
# Build the package
cd Datadog.MAUI.Symbols
dotnet pack -c Release

# Publish to NuGet.org
dotnet nuget push bin/Release/Datadog.MAUI.Symbols.1.1.0.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

Or use the GitHub Actions workflow dispatch:

1. Go to Actions → "Publish Datadog.MAUI.Symbols to NuGet"
2. Click "Run workflow"
3. Enter version: `1.1.0`
4. Select environment: `production`
5. Click "Run workflow"

## CI/CD Workflows

### build-symbols.yml

**Triggers:**
- Push to `main`/`develop` with changes in `Datadog.MAUI.Symbols/**`
- Pull requests with changes in `Datadog.MAUI.Symbols/**`
- Manual trigger via workflow_dispatch

**Actions:**
- Builds the package
- Validates package structure
- Uploads artifact
- Creates GitHub Release (if tagged with `symbols-v*`)

### publish-symbols-nuget.yml

**Triggers:**
- GitHub Release published with tag `symbols-v*`
- Manual trigger via workflow_dispatch

**Actions:**
- Publishes to NuGet.org (requires `NUGET_API_KEY` secret)
- Supports test environment for dry-run

## Setting Up NuGet API Key

To enable automatic publishing:

1. Generate an API key from NuGet.org:
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push" permissions
   - Scope it to `Datadog.MAUI.Symbols` package

2. Add as GitHub Secret:
   - Go to repository Settings → Secrets → Actions
   - Add new secret: `NUGET_API_KEY`
   - Paste your NuGet API key

3. (Optional) Configure environment protection:
   - Go to Settings → Environments
   - Create `production` environment
   - Add protection rules (require approvals, etc.)

## Workflow Isolation

The Symbols package workflows are **completely isolated** from the main SDK workflows:

| Aspect | Main SDK | Symbols Package |
|--------|----------|-----------------|
| Workflow | `build-all.yml` | `build-symbols.yml` |
| Version | `Directory.Build.props` | `Datadog.MAUI.Symbols.csproj` |
| Tag Format | `v3.5.0` | `symbols-v1.0.0` |
| Path Trigger | SDK paths | `Datadog.MAUI.Symbols/**` |
| Release Cadence | Tied to upstream SDKs | Independent |

This means:
- ✅ SDK releases don't trigger Symbols builds (unless both change)
- ✅ Symbols releases don't trigger SDK builds
- ✅ No version conflicts or coordination needed
- ✅ Each package can be released independently

## Testing Before Release

### Local Testing

```bash
# Build locally
cd Datadog.MAUI.Symbols
dotnet pack -c Release

# Test in a sample app
cd ../samples/YourTestApp
dotnet add package Datadog.MAUI.Symbols --source ../Datadog.MAUI.Symbols/bin/Release
dotnet publish -f net9.0-android -c Release
```

### CI Testing

Open a PR with changes to `Datadog.MAUI.Symbols/`. The workflow will:
- Build the package
- Validate structure
- Upload artifact for manual testing

## Rollback

If you need to rollback a release:

1. **Unlist from NuGet** (recommended):
   - Go to https://www.nuget.org/packages/Datadog.MAUI.Symbols/manage
   - Unlist the problematic version
   - Users won't see it, but existing references still work

2. **Release a patch version**:
   - Fix the issue
   - Release `1.1.1` with the fix
   - Document the issue in release notes

## FAQ

**Q: Can I release both SDK and Symbols at the same time?**
A: Yes! Just create both tags:
```bash
git tag v3.6.0 symbols-v1.1.0
git push origin v3.6.0 symbols-v1.1.0
```

**Q: What if I accidentally use the wrong tag format?**
A: Delete and recreate the tag:
```bash
git tag -d wrong-tag
git push origin :refs/tags/wrong-tag
git tag symbols-v1.1.0
git push origin symbols-v1.1.0
```

**Q: How do I test the release process without publishing?**
A: Use the manual workflow with `environment: test`, or just run the build workflow without creating a tag.

**Q: Should the Symbols package version match the SDK version?**
A: No! They are independent. Symbols package follows its own semantic versioning based on changes to the symbol upload functionality.
