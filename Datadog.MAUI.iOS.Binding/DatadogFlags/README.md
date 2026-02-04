# Datadog iOS Feature Flags - .NET Binding

.NET MAUI binding for the Datadog Feature Flags SDK for iOS. This binding enables remote feature configuration and A/B testing in your iOS applications.

## About

This package provides .NET bindings for the native `DatadogFlags` iOS framework. Feature Flags allow you to remotely control feature availability and configuration without redeploying your application.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Binding.DatadogFlags --version 3.5.0
```

**Note:** This binding is automatically included when you install the main `Datadog.MAUI` package. You typically don't need to reference this directly unless you're building custom integrations.

## Features

- **Remote Configuration** - Control features from Datadog dashboard
- **A/B Testing** - Run experiments and measure impact
- **Gradual Rollouts** - Release to percentage of users
- **User Targeting** - Enable features for specific segments
- **Real-time Updates** - Changes propagate instantly

## Usage

### Prerequisites

Initialize Datadog Core SDK first:

```csharp
using Datadog.iOS;
using Datadog.iOS.DatadogCore;
using Foundation;

var config = new DDDatadog.Configuration(
    clientToken: "YOUR_CLIENT_TOKEN",
    env: "production"
);
config.Site = DDSite.Us1;

DDDatadog.Initialize(config, TrackingConsent.Granted);
```

### Enable Feature Flags

```csharp
using Datadog.iOS.DatadogFlags;

DDFeatureFlags.Enable(new DDFeatureFlagsConfiguration());
```

### Evaluate Feature Flags

```csharp
using Datadog.iOS.DatadogFlags;

// Check if feature is enabled
bool isNewCheckoutEnabled = DDFeatureFlags.IsFeatureEnabled("new_checkout_flow");

if (isNewCheckoutEnabled)
{
    // Show new checkout UI
}

// Get feature flag value with default
string paymentProvider = DDFeatureFlags.GetFeatureFlagValue("payment_provider", "stripe");
```

### Advanced: Evaluation with Context

```csharp
using Datadog.iOS.DatadogFlags;
using Foundation;

// Create context for targeting
var context = NSDictionary.FromObjectsAndKeys(
    new NSObject[] { new NSString("premium"), new NSString("us-west") },
    new NSObject[] { new NSString("user_tier"), new NSString("region") }
);

bool showBetaFeatures = DDFeatureFlags.IsFeatureEnabled("beta_features", context);
```

## Configuration

### DDFeatureFlagsConfiguration

Basic configuration for Feature Flags initialization.

```csharp
var config = new DDFeatureFlagsConfiguration();
DDFeatureFlags.Enable(config);
```

## Integration with MAUI Plugin

The `Datadog.MAUI` plugin automatically includes Feature Flags. Evaluate flags using platform-specific code:

```csharp
#if IOS
using Datadog.iOS.DatadogFlags;

var isEnabled = DDFeatureFlags.IsFeatureEnabled("my_feature");
#endif
```

## API Reference

### DDFeatureFlags

Main class for evaluating feature flags.

#### Static Methods

```csharp
// Enable Feature Flags
public static void Enable(DDFeatureFlagsConfiguration configuration);

// Check if feature is enabled
public static bool IsFeatureEnabled(string featureKey);
public static bool IsFeatureEnabled(string featureKey, NSDictionary context);

// Get feature flag value
public static T GetFeatureFlagValue<T>(string featureKey, T defaultValue);
public static T GetFeatureFlagValue<T>(string featureKey, T defaultValue, NSDictionary context);
```

## Example: A/B Testing

```csharp
using Datadog.iOS.DatadogFlags;
using Datadog.iOS.DatadogRUM;

// Evaluate checkout variant
string checkoutVariant = DDFeatureFlags.GetFeatureFlagValue("checkout_variant", "control");

// Track variant in RUM
DDGlobalRUM.Get().AddAttribute("checkout_variant", new NSString(checkoutVariant));

switch (checkoutVariant)
{
    case "variant_a":
        // Show variant A
        break;
    case "variant_b":
        // Show variant B
        break;
    default:
        // Show control
        break;
}
```

## Native iOS Reference

For complete native SDK documentation, see:
- [Datadog Feature Flags for iOS](https://docs.datadoghq.com/real_user_monitoring/feature_flags/ios/)

## Version Information

- **Native SDK Version**: 3.5.0
- **XCFramework**: `DatadogFlags.xcframework`
- **Supported iOS**: 12.0+

## License

Apache 2.0 - See main repository LICENSE file.
