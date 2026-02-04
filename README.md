# Datadog SDK for .NET MAUI

[![NuGet](https://img.shields.io/nuget/v/Datadog.MAUI.svg)](https://www.nuget.org/packages/Datadog.MAUI/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

Unofficial [Datadog](https://www.datadoghq.com/) SDK for .NET MAUI applications, providing comprehensive monitoring and observability for iOS and Android mobile apps.

## Features

- **Real User Monitoring (RUM)** - Track user sessions, views, actions, and performance metrics
- **Log Collection** - Centralized logging with automatic context enrichment
- **APM Distributed Tracing** - End-to-end visibility across your mobile and backend services
- **Crash Reporting** - Automatic crash detection and symbolication
- **Session Replay** - Visual reproduction of user sessions
- **WebView Tracking** - Monitor hybrid app content
- **Feature Flags** - Remote configuration and A/B testing
- **Network Request Tracking** - Automatic HTTP request monitoring

## Installation

Install the NuGet package in your .NET MAUI project:

```bash
dotnet add package Datadog.MAUI
```

Or via Package Manager Console:

```powershell
Install-Package Datadog.MAUI
```

## Quick Start

### 1. Initialize the SDK

In your `MauiProgram.cs`, initialize Datadog before building the app:

```csharp
using Datadog.Maui;
using Datadog.Maui.Configuration;
using Datadog.Maui.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            // Initialize Datadog
            .UseDatadog(datadog =>
            {
                datadog.SetClientToken(
                    android: "YOUR_ANDROID_CLIENT_TOKEN",
                    ios: "YOUR_IOS_CLIENT_TOKEN"
                );
                datadog.Environment = "production";
                datadog.ServiceName = "my-maui-app";
                datadog.Site = DatadogSite.US1;
                datadog.TrackingConsent = TrackingConsent.Granted;

                datadog.EnableRum(rum =>
                {
                    rum.SetApplicationId(
                        android: "YOUR_ANDROID_RUM_ID",
                        ios: "YOUR_IOS_RUM_ID"
                    );
                });

                datadog.EnableLogs();

                datadog.EnableTracing(tracing =>
                {
                    tracing.SetFirstPartyHosts(new[] { "api.example.com" });
                });

                datadog.EnableSessionReplay(sessionReplay =>
                {
                    sessionReplay.SetSampleRate(20);
                    sessionReplay.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs);
                    sessionReplay.SetImagePrivacy(ImagePrivacy.MaskNonBundledOnly);
                    sessionReplay.SetTouchPrivacy(TouchPrivacy.Show);
                });
            });

        return builder.Build();
    }
}
```

### 2. Set User Information

```csharp
using Datadog.Maui;

// Set user information for RUM and Logs
Datadog.SetUser(new UserInfo
{
    Id = "user-123",
    Name = "John Doe",
    Email = "john.doe@example.com"
});
```

### 3. Track Custom Events

#### Logging

```csharp
using Datadog.Maui.Logs;

var logger = Logger.Create("MyLogger");
logger.Info("User logged in successfully");
logger.Error("Failed to process payment", error: null, attributes: new Dictionary<string, object>
{
    { "user_id", "user-123" },
    { "amount", 99.99 }
});
```

#### RUM Views

```csharp
using Datadog.Maui.Rum;

GlobalRum.Get().StartView("checkout", "Checkout Page");

// ... user interacts with the page ...

GlobalRum.Get().StopView("checkout");
```

#### Custom Actions

```csharp
using Datadog.Maui.Rum;

GlobalRum.Get().AddAction(
    RumActionType.Custom,
    "Purchase Button",
    attributes: new Dictionary<string, object>
    {
        { "product_id", "prod-456" },
        { "price", 49.99 }
    }
);
```

#### Distributed Tracing

```csharp
using Datadog.Maui.Tracing;

using var span = Tracer.StartSpan("process_payment");
span.SetTag("payment_method", "credit_card");

try
{
    // Your business logic
    ProcessPayment();
}
catch (Exception ex)
{
    span.SetError(ex);
    throw;
}
// Span automatically finishes when disposed
```

## Configuration

### Datadog Sites

Specify your Datadog site based on your account region:

| Site          | DatadogSite Value     | Description                 |
| ------------- | --------------------- | --------------------------- |
| US1 (default) | `DatadogSite.US1`     | `https://app.datadoghq.com` |
| EU1           | `DatadogSite.EU1`     | `https://app.datadoghq.eu`  |
| US3           | `DatadogSite.US3`     | `https://us3.datadoghq.com` |
| US5           | `DatadogSite.US5`     | `https://us5.datadoghq.com` |
| US1-FED       | `DatadogSite.US1_FED` | `https://app.ddog-gov.com`  |
| AP1           | `DatadogSite.AP1`     | `https://ap1.datadoghq.com` |

### Sample Rates

Control the percentage of sessions and traces collected:

```csharp
.UseDatadog(datadog =>
{
    // ... other config ...

    datadog.EnableRum(rum =>
    {
        rum.SetApplicationId(android: "...", ios: "...");
        rum.SetSessionSampleRate(75.0f); // Sample 75% of RUM sessions
    });

    datadog.EnableTracing(tracing =>
    {
        tracing.SetSampleRate(50.0f); // Sample 50% of traces
    });

    datadog.EnableSessionReplay(sessionReplay =>
    {
        sessionReplay.SetSampleRate(20); // Sample 20% for session replay
    });
});
```

### Global Attributes

Add custom attributes to all RUM events:

```csharp
using Datadog.Maui.Rum;

// Add global attributes at runtime
GlobalRum.Get().AddAttribute("user_tier", "premium");
GlobalRum.Get().AddAttribute("app_version", "1.2.3");

// Remove attributes when no longer needed
GlobalRum.Get().RemoveAttribute("user_tier");
```

## Platform Requirements

### iOS

- **Minimum iOS Version**: 12.0
- **Supported .NET Versions**: .NET 8, 9, 10
- **XCFrameworks**: Automatically included in the binding package

### Android

- **Minimum API Level**: 21 (Android 5.0)
- **Supported .NET Versions**: .NET 9, 10
- **Maven Dependencies**: Automatically resolved via AndroidMavenLibrary

## Building from Source

### Prerequisites

- .NET 8+ SDK
- macOS with Xcode 14+ (for iOS development)
- Android SDK API Level 34+ (for Android)
- Bash (for build scripts)

### Clone and Build

```bash
git clone https://github.com/DataDog/dd-sdk-maui.git
cd dd-sdk-maui

# Download iOS XCFrameworks
./scripts/download-ios-frameworks.sh

# Build all projects (Android dependencies are fetched automatically)
./scripts/build.sh

# Or build manually with dotnet
dotnet build Datadog.MAUI.sln -c Release

# Pack NuGet packages
dotnet pack -c Release -o artifacts/packages
```

For more detailed build instructions, see [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md).

### Running the Sample App

The repository includes a comprehensive sample app demonstrating all SDK features:

```bash
# Quick start (auto-loads .env from sample directory)
cd samples/DatadogMauiSample
cp .env.example .env       # Copy and edit with your Datadog credentials

cd ../..                   # Return to repository root
make run-ios              # iOS - automatically loads samples/DatadogMauiSample/.env
make run-android          # Android - automatically loads samples/DatadogMauiSample/.env
```

See [samples/DatadogMauiSample/README.md](samples/DatadogMauiSample/README.md) for detailed setup instructions.

## Documentation

📚 **[Full Documentation Site](https://kyletaylored.github.io/dd-sdk-maui/)**

### For SDK Users

Complete guides for using the SDK in your application:

- **[Using the SDK](https://kyletaylored.github.io/dd-sdk-maui/getting-started/using-the-sdk)** - Installation, configuration, and usage
- **[API Reference](https://kyletaylored.github.io/dd-sdk-maui/api-reference)** - Complete API documentation
- **[Code Examples](https://kyletaylored.github.io/dd-sdk-maui/examples)** - Real-world usage examples
- **[What is NLI?](https://kyletaylored.github.io/dd-sdk-maui/guides/nli-primer)** - Understanding Native Library Interop

### For SDK Developers

Guides for building and contributing to the SDK:

- **[Installation & Setup](https://kyletaylored.github.io/dd-sdk-maui/getting-started/installation)** - Development environment setup
- **[Developer Guide](https://kyletaylored.github.io/dd-sdk-maui/getting-started/developer-guide)** - Development workflows
- **[Android Dependencies](https://kyletaylored.github.io/dd-sdk-maui/guides/android/dependencies)** - Android binding development
- **[iOS Binding Strategy](https://kyletaylored.github.io/dd-sdk-maui/guides/ios/binding-strategy)** - iOS binding development
- **[Project Overview](https://kyletaylored.github.io/dd-sdk-maui/project/overview)** - Architecture and technical details

### Datadog Resources

- [Datadog Documentation](https://docs.datadoghq.com/)
- [RUM Mobile Monitoring](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/)
- [iOS SDK Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [Android SDK Documentation](https://docs.datadoghq.com/real_user_monitoring/android/)

## Contributing

Contributions are welcome! Please see [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) for details on:

- Setting up your development environment
- Building the SDK from source
- Running tests
- Submitting pull requests
- Code style and conventions

## License

[Apache License 2.0](LICENSE)

## Support

- **GitHub Issues**: [Report a bug](https://github.com/kyletaylored/dd-sdk-maui/issues)
<!-- - **Datadog Support**: [Contact support](https://docs.datadoghq.com/help/) -->
