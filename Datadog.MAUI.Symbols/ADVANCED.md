# Datadog.MAUI.Symbols — Advanced Usage

This document covers advanced scenarios for `Datadog.MAUI.Symbols`, including CI/CD, debug builds, build IDs, bundled `datadog-ci`, and local development.

If you’re just getting started, read **README.md** first.

---

## Execution model (important context)

This package runs:

* **At build / publish time**
* As an **MSBuild target**
* Before your app ever runs

As a result:

* `appsettings.json` is ignored
* Runtime configuration does not apply
* Everything is driven by **MSBuild properties** and **environment variables**

---

## CI/CD usage

### GitHub Actions (Android + iOS)

```yaml
name: Build and Upload Symbols

on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: macos-latest
    env:
      DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
      DD_SITE: datadoghq.com
      DD_BUILD_FLAVOR: production

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'

      - name: Publish Android
        run: dotnet publish -f net9.0-android -c Release

      - name: Publish iOS
        run: dotnet publish -f net9.0-ios -c Release
```

### Staging / environment separation

```bash
export DD_BUILD_FLAVOR=staging
dotnet publish -f net9.0-android -c Release
```

Each flavor uploads symbols independently.

---

## Debug builds

By default, symbol uploads only run in **Release**.

To enable uploads in Debug:

```xml
<PropertyGroup>
  <DatadogUploadInDebug>true</DatadogUploadInDebug>
</PropertyGroup>
```

This is useful for:

* internal testing
* validating CI pipelines
* verifying symbol discovery paths

---

## Dry-run mode

Dry-run validates configuration without uploading anything.

```xml
<PropertyGroup>
  <DatadogDryRun>true</DatadogDryRun>
</PropertyGroup>
```

Expected output will show:

* detected platform
* discovered symbol paths
* command invocation

…but no network requests.

---

## Build ID behavior

### What the plugin does

* Generates a unique **build ID per publish**
* Embeds it into the compiled app (for future use)
* Uses it internally for symbol association **only when supported**

### What it does *not* do

* Does **not** append build IDs to app versions
* Does **not** require you to configure anything manually
* Does **not** affect runtime SDK initialization

### Why this matters

Datadog symbolication depends on the **app version matching exactly** between:

* uploaded symbols
* runtime crash reports

Mixing build IDs into the version is the #1 cause of broken symbolication.
This plugin explicitly prevents that.

---

## Bundled `datadog-ci`

### Why a bundled CLI exists

Some Datadog CLI features may be required **before** they’re released upstream.

Bundling provides:

* deterministic behavior
* no registry authentication
* no dependency on GitHub Packages
* safe opt-in until upstream catches up

### Default behavior

```text
DatadogUseBundledCi = false
```

* Uses upstream `@datadog/datadog-ci`
* Compatible with public releases

### Enable bundled CLI

```xml
<PropertyGroup>
  <DatadogUseBundledCi>true</DatadogUseBundledCi>
</PropertyGroup>
```

When enabled:

* the bundled tarball is used via `npx --package`
* build ID support is activated
* upstream CLI limitations are bypassed

You can disable this flag once upstream supports the same features.

---

## Local development (working on the plugin itself)

### Use ProjectReference instead of NuGet

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Datadog.MAUI.Symbols\Datadog.MAUI.Symbols.csproj"
                    ReferenceOutputAssembly="false" />
</ItemGroup>

<UsingTask TaskName="Datadog.MAUI.Symbols.UploadSymbolsTask"
           AssemblyFile="..\..\Datadog.MAUI.Symbols\bin\$(Configuration)\netstandard2.0\Datadog.MAUI.Symbols.dll" />

<Import Project="..\..\Datadog.MAUI.Symbols\build\Datadog.MAUI.Symbols.targets" />
```

This allows:

* editing task code
* rebuilding automatically
* testing changes without packing NuGet

---

## Android symbol discovery details

The plugin searches for `mapping.txt` in this order:

1. `$(OutputPath)\mapping.txt`
2. `$(OutputPath)\..\mapping.txt`
3. `$(IntermediateOutputPath)\mapping.txt`
4. `$(ProjectDir)\obj\$(Configuration)\$(TargetFramework)\android-arm64\mapping.txt`

If none are found, upload is skipped (with a warning).

---

## iOS symbol discovery details

The plugin searches for `.dSYM` in:

1. `$(OutputPath)\$(AssemblyName).app.dSYM`
2. `$(OutputPath)\..\$(AssemblyName).app.dSYM`
3. `$(AppBundleDir).dSYM`

---

## Verbose logging

To isolate Datadog output:

```bash
dotnet publish -f net9.0-android -c Release 2>&1 | grep "\[Datadog\]"
```

---

## Common failure modes

### Symbols upload succeeds but crashes aren’t symbolicated

* App version mismatch between:

  * symbol upload
  * RUM SDK initialization
* Fix: remove `DatadogAppVersion` override and rely on `ApplicationDisplayVersion`

### `mapping.txt` missing

* R8 / ProGuard not enabled
* Debug build without `DatadogUploadInDebug=true`

### `npx` fails in CI

* Node not installed
* Node version < 18
* PATH misconfiguration

---

## When *not* to use this package

* If you want runtime-only behavior
* If you need symbol uploads triggered manually
* If you want to manage `datadog-ci` yourself

This package is opinionated and intentionally automated.

---

## License

Apache-2.0
