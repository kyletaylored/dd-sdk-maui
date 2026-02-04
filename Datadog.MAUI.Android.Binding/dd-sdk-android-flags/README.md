# Datadog Android Feature Flags SDK - .NET Binding

.NET MAUI binding for the Datadog Feature Flags SDK for Android. This binding allows you to use Datadog Feature Flags in your .NET MAUI applications to remotely configure features and run A/B tests.

## About

This package provides .NET bindings for the native Datadog Feature Flags Android SDK (`com.datadoghq:dd-sdk-android-flags`). Feature Flags enable you to remotely control feature availability and configuration without redeploying your application.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.Binding.Flags --version 3.5.0
```

**Note:** This binding is automatically included when you install the main `Datadog.MAUI` package. You typically don't need to reference this directly unless you're building custom integrations.

## Features

- **Remote Feature Configuration** - Control feature availability from Datadog dashboard
- **A/B Testing** - Run experiments and measure feature impact
- **Gradual Rollouts** - Release features to percentage of users
- **User Targeting** - Enable features for specific user segments
- **Real-time Updates** - Changes propagate without app restart

## Usage

### Prerequisites

Feature Flags require the Datadog Core SDK to be initialized first:

```csharp
using Com.Datadog.Android.Core.Configuration;
using Com.Datadog.Android.Datadog;

var configuration = new Configuration.Builder(
    clientToken: "YOUR_CLIENT_TOKEN",
    env: "production",
    variant: ""
)
.UseSite(DatadogSite.Us1)
.Build();

Datadog.Initialize(Application.Context, configuration, TrackingConsent.Granted);
```

### Enable Feature Flags

```csharp
using Com.Datadog.Android.Flags;

FeatureFlags.Enable(new FeatureFlagsConfiguration.Builder().Build());
```

### Evaluate Feature Flags

```csharp
using Com.Datadog.Android.Flags;

// Check if a feature is enabled (boolean flag)
bool isNewCheckoutEnabled = FeatureFlags.IsFeatureEnabled("new_checkout_flow");

if (isNewCheckoutEnabled)
{
    // Show new checkout UI
}
else
{
    // Show legacy checkout UI
}

// Get a feature flag value with default
string paymentProvider = FeatureFlags.GetFeatureFlagValue("payment_provider", "stripe");
```

### Advanced Usage: Feature Flag with Context

```csharp
using Com.Datadog.Android.Flags;
using System.Collections.Generic;

// Evaluate with user context for targeting
var context = new Dictionary<string, Java.Lang.Object>
{
    { "user_tier", "premium" },
    { "region", "us-west" }
};

bool showBetaFeatures = FeatureFlags.IsFeatureEnabled("beta_features", context);
```

## Configuration Options

### FeatureFlagsConfiguration.Builder

| Method | Description | Default |
|--------|-------------|---------|
| `Build()` | Build the configuration | - |

## Integration with MAUI Plugin

If you're using the main `Datadog.MAUI` plugin, Feature Flags are automatically enabled when you initialize the SDK. You can evaluate flags using the native Android APIs shown above, or through platform-specific code.

## Native Android Reference

For complete native SDK documentation, see:
- [Datadog Feature Flags for Android](https://docs.datadoghq.com/real_user_monitoring/feature_flags/android/)

## API Reference

### FeatureFlags

Main class for evaluating feature flags.

#### Static Methods

```csharp
// Check if a feature is enabled
public static bool IsFeatureEnabled(string featureKey);
public static bool IsFeatureEnabled(string featureKey, IDictionary<string, Java.Lang.Object> context);

// Get feature flag value
public static T GetFeatureFlagValue<T>(string featureKey, T defaultValue);
public static T GetFeatureFlagValue<T>(string featureKey, T defaultValue, IDictionary<string, Java.Lang.Object> context);
```

## Example: A/B Testing

```csharp
using Com.Datadog.Android.Flags;
using Com.Datadog.Android.Rum;

// Evaluate which checkout variant to show
string checkoutVariant = FeatureFlags.GetFeatureFlagValue("checkout_variant", "control");

// Track the variant in RUM for analysis
GlobalRum.Get().AddAttribute("checkout_variant", checkoutVariant);

switch (checkoutVariant)
{
    case "variant_a":
        // Show checkout variant A
        break;
    case "variant_b":
        // Show checkout variant B
        break;
    default:
        // Show control (default) checkout
        break;
}
```

## Version Information

- **Native SDK Version**: 3.5.0
- **Maven Artifact**: `com.datadoghq:dd-sdk-android-flags:3.5.0`

## License

Apache 2.0 - See main repository LICENSE file.
