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
using Datadog.Maui.Logs;
using Datadog.Maui.Rum;
using Datadog.Maui.Tracing;

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
            });

        builder.UseDatadog(config =>
        {
            config.ClientToken = "YOUR_CLIENT_TOKEN";
            config.Environment = "production";
            config.ServiceName = "my-maui-app";
            config.Site = DatadogSite.US1;

            config.EnableRum(rum =>
            {
                rum.SetApplicationId("YOUR_RUM_APPLICATION_ID");
                rum.SetSessionSampleRate(100);
                rum.TrackUserInteractions(true);
            });

            config.EnableLogs(logs => logs.SetSampleRate(100));
            config.EnableTracing(tracing => tracing.SetSampleRate(100));
        });

        return builder.Build();
    }
}
```

### 2. Set User Information

```csharp
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
var logger = Logs.CreateLogger("app");
logger.Info("User logged in successfully");
logger.Error("Failed to process payment", attributes: new Dictionary<string, object>
{
    { "user_id", "user-123" },
    { "amount", 99.99 }
});
```

#### RUM Views

```csharp
Rum.StartView("checkout", "Checkout Page");

// ... user interacts with the page ...

Rum.StopView("checkout");
```

#### Custom Actions

```csharp
Rum.AddAction(RumActionType.Tap, "Purchase Button", new Dictionary<string, object>
{
    { "product_id", "prod-456" },
    { "price", 49.99 }
});
```

#### Distributed Tracing

```csharp
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
new RumConfiguration.Builder()
.SetApplicationId("YOUR_RUM_APPLICATION_ID")
.SetSessionSampleRate(75)
.Build();
```

### Global Attributes

Add custom attributes to all events:

```csharp
// During initialization
var tags = new Dictionary<string, string>
{
    { "app_version", "1.2.3" },
    { "build_number", "456" }
};

Datadog.SetTags(tags);
Rum.AddAttribute("user_tier", "premium");
Rum.RemoveAttribute("user_tier");
```

## Platform Requirements

### iOS

- **Minimum iOS Version**: 17.0
- **Supported .NET Versions**: .NET 9, 10
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

## Documentation

### Developer Documentation

For comprehensive information about building, contributing, and understanding this SDK's architecture:

- **[Getting Started](docs/GETTING_STARTED.md)** - Setup and development guide
- **[Project Overview](docs/PROJECT_OVERVIEW.md)** - Architecture and technical details
- **[Android Dependency Management](docs/ANDROID_DEPENDENCY_MANAGEMENT.md)** - Binding dependency patterns
- **[Contributing Guide](docs/CONTRIBUTING.md)** - How to contribute to this project

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
