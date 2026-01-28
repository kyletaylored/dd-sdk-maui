# Datadog.MAUI.Symbols - CI/CD Integration

This document explains how the Symbols package integrates with the monorepo's CI/CD infrastructure.

## Overview

The Symbols package has **independent release workflow** from the main SDK packages but integrates with the existing infrastructure for distribution.

## Workflow Architecture

### 1. Build Workflow (`.github/workflows/build-symbols.yml`)

**Triggers:**
- Push to `main`/`develop` with changes in `Datadog.MAUI.Symbols/**`
- Pull requests affecting Symbols code
- Manual workflow dispatch

**Actions:**
- Builds the package (version `1.0.0` independent from SDK `3.5.0`)
- Validates package structure (targets files, DLL, README)
- Uploads artifact for testing
- Creates GitHub Release when tagged with `symbols-v*` format

### 2. Publish Workflow (`.github/workflows/publish-symbols-nuget.yml`)

**Triggers:**
- GitHub Release published with tag `symbols-v*`
- Manual workflow dispatch

**Actions:**
- Builds the package
- Publishes to NuGet.org (requires `NUGET_API_KEY` secret)
- Supports test environment for dry-run

### 3. Documentation + Feed Workflow (`.github/workflows/publish-docs-nuget-feed.yml`)

**Updates:**
- Now includes a step to download the latest Symbols package release
- Adds it to the static NuGet feed on GitHub Pages alongside SDK packages

**How it works:**
```bash
# Downloads latest symbols-v* release
LATEST_SYMBOLS_TAG=$(gh release list | grep "symbols-v" | head -1 | awk '{print $1}')
gh release download "$LATEST_SYMBOLS_TAG" --pattern "*.nupkg" --dir ./pkgs
```

The feed then serves all packages (SDK + Symbols) at:
```
https://your-github-username.github.io/dd-sdk-maui/nuget/index.json
```

## Version Management

### SDK Packages
- **Version**: Controlled by `Directory.Build.props` → `<DatadogSdkVersion>3.5.0</DatadogSdkVersion>`
- **Tag Format**: `v3.5.0`
- **Packages**: All Datadog.MAUI.* packages (except Symbols)

### Symbols Package
- **Version**: Controlled by `Datadog.MAUI.Symbols.csproj` → `<Version>1.0.0</Version>`
- **Overrides**: Uses `<PackageVersion>1.0.0</PackageVersion>` to override Directory.Build.props
- **Tag Format**: `symbols-v1.0.0`
- **Package**: Only `Datadog.MAUI.Symbols`

## Release Process

### Regular Workflow (Automated)

1. **Make changes** to Symbols package
2. **Update version** in `Datadog.MAUI.Symbols.csproj`
3. **Commit and push** → Triggers build workflow
4. **Verify build** passes in GitHub Actions
5. **Create tag** and push:
   ```bash
   git tag symbols-v1.1.0
   git push origin symbols-v1.1.0
   ```
6. **Build workflow** creates GitHub Release automatically
7. **Publish workflow** publishes to NuGet.org (if configured)
8. **Feed workflow** picks up new release next time it runs

### Manual Workflow

If you need to manually publish or update the feed:

```bash
# Manually trigger build
gh workflow run build-symbols.yml

# Manually trigger publish
gh workflow run publish-symbols-nuget.yml -f version=1.0.0 -f environment=production

# Manually trigger feed update (includes latest Symbols)
gh workflow run publish-docs-nuget-feed.yml
```

## Distribution Channels

### 1. NuGet.org (Primary)
- Official distribution
- Discoverable via `dotnet add package Datadog.MAUI.Symbols`
- Published via `publish-symbols-nuget.yml`

### 2. GitHub Releases
- Download `.nupkg` directly from releases page
- Tag format: `symbols-v{version}`
- Includes checksums and release notes

### 3. GitHub Pages Feed (Secondary)
- Static NuGet feed for all packages (SDK + Symbols)
- URL: `https://your-org.github.io/dd-sdk-maui/nuget/index.json`
- Useful for testing pre-release builds
- Add to project with:
  ```xml
  <add key="DatadogMAUI" value="https://your-org.github.io/dd-sdk-maui/nuget/index.json" />
  ```

## Isolation from SDK Releases

The Symbols package is **fully isolated** from SDK releases:

| Aspect | SDK Packages | Symbols Package |
|--------|--------------|-----------------|
| Workflow | `build-all.yml` | `build-symbols.yml` |
| Version Source | `Directory.Build.props` | `Datadog.MAUI.Symbols.csproj` |
| Tag Format | `v{version}` | `symbols-v{version}` |
| Path Triggers | SDK paths | `Datadog.MAUI.Symbols/**` |
| Release Cadence | Tied to upstream SDKs | Independent |
| NuGet Feed | Shared feed (both included) | Shared feed (both included) |

**Benefits:**
- ✅ SDK and Symbols can be released independently
- ✅ No need to coordinate version numbers
- ✅ Symbols can iterate faster without SDK changes
- ✅ No risk of version conflicts
- ✅ Both packages available in same feed for convenience

## Testing Integration

To test that the Symbols package appears in the feed:

1. **Trigger a Symbols release** (create `symbols-v1.0.0` tag)
2. **Wait for workflows** to complete
3. **Manually trigger feed update** if needed:
   ```bash
   gh workflow run publish-docs-nuget-feed.yml
   ```
4. **Verify package in feed**:
   ```bash
   # List packages in feed
   curl -s https://your-org.github.io/dd-sdk-maui/nuget/packages.json | jq '.packages[] | select(.id | contains("Symbols"))'
   ```
5. **Test installation**:
   ```bash
   dotnet add package Datadog.MAUI.Symbols --version 1.0.0 --source https://your-org.github.io/dd-sdk-maui/nuget/index.json
   ```

## Troubleshooting

### Symbols package not appearing in feed

**Cause**: Feed workflow hasn't run since Symbols release

**Solution**: Manually trigger feed workflow:
```bash
gh workflow run publish-docs-nuget-feed.yml
```

### Wrong version in NuGet package

**Cause**: Directory.Build.props overriding version

**Solution**: Verify `Datadog.MAUI.Symbols.csproj` has both:
```xml
<Version>1.0.0</Version>
<PackageVersion>1.0.0</PackageVersion>
```

### Package not found on NuGet.org

**Cause**: Publish workflow not configured or failed

**Solutions**:
1. Check `NUGET_API_KEY` secret is set
2. Verify publish workflow ran successfully
3. Check NuGet.org indexing (can take 5-10 minutes)

## Future Enhancements

Potential improvements to the integration:

1. **Automatic feed update** after Symbols release
   - Add workflow dispatch trigger to feed workflow from Symbols workflow

2. **Version synchronization check**
   - Add CI check to warn if Symbols and SDK versions drift too far

3. **Pre-release support**
   - Support `symbols-v1.0.0-beta` tags for pre-release testing

4. **Feed caching**
   - Cache feed artifacts between workflow runs for faster updates

## References

- [Symbols Package Plan](plan.md)
- [Release Process](RELEASE.md)
- [Build Workflow](.github/workflows/build-symbols.yml)
- [Publish Workflow](.github/workflows/publish-symbols-nuget.yml)
- [Feed Workflow](.github/workflows/publish-docs-nuget-feed.yml)
