# Datadog.MAUI.Symbols

An MSBuild SDK for .NET MAUI that automatically uploads debug symbols
(**iOS dSYMs** and **Android R8 / ProGuard mapping files**) to Datadog during `dotnet publish`.

Runs at **build time**, not at app runtime.

---

## What it does

* Uploads Android `mapping.txt` (R8 / ProGuard)
* Uploads iOS `.dSYM` bundles
* Runs automatically during `dotnet publish`
* Uses `datadog-ci` under the hood

---

## Prerequisites

* **Node.js (>= 18)** – required for `npx`
* **Datadog API key**

  * `DD_API_KEY` environment variable **or**
  * `DatadogApiKey` MSBuild property
* **Android**: R8 / ProGuard enabled for Release builds

---

## Installation

```bash
dotnet add package Datadog.MAUI.Symbols
```

---

## Minimal setup

```xml
<PropertyGroup>
  <DatadogServiceName>com.company.myapp</DatadogServiceName>
</PropertyGroup>
```

```bash
export DD_API_KEY=your-datadog-api-key
dotnet publish -f net9.0-android -c Release
```

Defaults:

* **Version**: `ApplicationDisplayVersion`
* **Flavor**: `release`
* **Service**: `DatadogServiceName`

---

## Platform-specific services (recommended)

```xml
<PropertyGroup>
  <DatadogServiceNameAndroid>com.company.myapp.android</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>com.company.myapp.ios</DatadogServiceNameiOS>
</PropertyGroup>
```

---

## Build flavors

```xml
<PropertyGroup>
  <DatadogFlavor Condition="'$(DD_BUILD_FLAVOR)' != ''">
    $(DD_BUILD_FLAVOR)
  </DatadogFlavor>
</PropertyGroup>
```

```bash
export DD_BUILD_FLAVOR=production
dotnet publish -f net9.0-android -c Release
```

---

## App version vs build ID (important)

* **App version**

  * Comes from `DatadogAppVersion` or `ApplicationDisplayVersion`
  * **Must match** the version reported by the Datadog RUM SDK

* **Build ID**

  * Generated per build
  * Used internally for symbol association

---

## Using with Datadog MAUI RUM SDK

This plugin generates build metadata at build time and exposes it at runtime via a generated class:

```csharp
Datadog.MAUI.Symbols.DatadogBuildInfo
````

This allows you to pass the **same Build ID and Variant** used during symbol upload into the Datadog MAUI RUM SDK, ensuring correct crash and error symbolication.

### Example

```csharp
using Datadog.MAUI.Symbols;

// ...

datadog.EnableRum(rum =>
{
    // Existing RUM configuration
    rum.SetApplicationId(
        android: datadogSettings.Android.RumApplicationId,
        ios: datadogSettings.iOS.RumApplicationId
    );

    // Important: pass build metadata used for symbol uploads
    rum.Variant = DatadogBuildInfo.Variant;
    rum.BuildId = DatadogBuildInfo.BuildId;
});
```

**Notes:**

* `BuildId` is generated per build and matches the ID used when uploading symbols.
* `Variant` corresponds to the effective build variant (for example: `debug`, `release`, `staging`).
* **Do not** append the build ID to the app version. App version must exactly match the version reported by the RUM SDK.

---

## Bundled `datadog-ci` (optional)

This package ships with a bundled `datadog-ci` tarball.

### Default

* Uses upstream `@datadog/datadog-ci`
* No behavior change

### Enable bundled version

Use this for enabling build ID support until it's officially supported.

```xml
<PropertyGroup>
  <DatadogUseBundledCi>true</DatadogUseBundledCi>
</PropertyGroup>
```

---

## Full configuration reference

```xml
<PropertyGroup>
  <DatadogApiKey>...</DatadogApiKey>
  <DatadogServiceName>...</DatadogServiceName>
  <DatadogServiceNameAndroid>...</DatadogServiceNameAndroid>
  <DatadogServiceNameiOS>...</DatadogServiceNameiOS>
  <DatadogAppVersion>1.2.3</DatadogAppVersion>
  <DatadogFlavor>production</DatadogFlavor>
  <DatadogSite>datadoghq.com</DatadogSite>
  <DatadogDryRun>false</DatadogDryRun>
  <DatadogUploadEnabled>true</DatadogUploadEnabled>
  <DatadogUploadInDebug>false</DatadogUploadInDebug>
  <DatadogUseBundledCi>false</DatadogUseBundledCi>
</PropertyGroup>
```

---

## Android setup (R8)

```xml
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
  <AndroidLinkTool>r8</AndroidLinkTool>
</PropertyGroup>
```

---

## Usage

```bash
dotnet publish -f net9.0-android -c Release
dotnet publish -f net9.0-ios -c Release
```

The plugin:

1. Detects platform
2. Finds symbols
3. Uploads them to Datadog

---

## Troubleshooting

### `npx` not found

Install Node.js and verify:

```bash
npx --version
```

### Android `mapping.txt` missing

Ensure R8 / ProGuard is enabled for Release builds.

### iOS dSYM missing

Ensure Release builds generate debug symbols.

---

## License

Apache-2.0
