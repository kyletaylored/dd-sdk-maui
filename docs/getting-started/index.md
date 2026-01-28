---
layout: default
title: Getting Started
nav_order: 2
has_children: true
---

# Getting Started

Welcome to the Datadog SDK for .NET MAUI! This section will help you get up and running quickly.

## For SDK Users

**New to the SDK?** Start here to integrate Datadog into your .NET MAUI application.

### Core Documentation

1. **[Using the SDK](../getting-started/using-the-sdk)** - Complete usage guide
   - Installation and setup
   - Configuration options
   - Logging, RUM, and Tracing
   - Best practices

2. **[API Reference](../api-reference)** - Complete API documentation
   - All classes, methods, and properties
   - Configuration objects
   - Interfaces for dependency injection

3. **[Code Examples](../examples)** - Practical examples
   - Basic setup
   - Real-world scenarios
   - E-commerce, authentication flows
   - MAUI integration patterns

### Quick Start

```csharp
// 1. Install the package
dotnet add package Datadog.MAUI

// 2. Initialize in MauiProgram.cs
builder.UseDatadog(config =>
{
    config.ClientToken = "YOUR_CLIENT_TOKEN";
    config.Environment = "production";
    config.ServiceName = "my-app";
});

// 3. Start using the SDK
var logger = Logs.CreateLogger("my-logger");
logger.Info("Hello, Datadog!");
```

### Quick Links

- **Installation**: [Using the SDK - Installation](../getting-started/using-the-sdk#installation)
- **Configuration**: [Using the SDK - Configuration](../getting-started/using-the-sdk#configuration)
- **Logging**: [API Reference - Logs API](../api-reference#logs-api)
- **RUM**: [API Reference - RUM API](../api-reference#rum-api)
- **Tracing**: [API Reference - Tracing API](../api-reference#tracing-api)

---

## For SDK Developers

**Contributing to the SDK?** This section is for developers building and maintaining the SDK itself.

### Core Documentation

1. **[Installation & Setup](../getting-started/installation)** - Development environment setup
   - Prerequisites and tools
   - Building iOS and Android bindings
   - Creating NuGet packages
   - Testing with sample apps

2. **[Developer Guide](../getting-started/developer-guide)** - Development workflows
   - Project structure
   - Build scripts
   - Testing strategies
   - Troubleshooting

3. **[Unified API Design](../getting-started/unified-api-design)** - API architecture
   - Cross-platform design patterns
   - Platform abstraction layer
   - Interface definitions

### Platform-Specific Guides

- **[Android Development](../guides/android/)** - Android binding development
  - Dependency management
  - Integration packages
  - Maven to NuGet mapping

- **[iOS Development](../guides/ios/)** - iOS binding development
  - Binding strategy
  - API identification
  - XCFramework handling

### Architecture & Build System

- **[Workflow Architecture](../architecture/workflows)** - CI/CD pipelines
- **[Scripts Overview](../architecture/scripts)** - Build automation
- **[Packaging Architecture](../architecture/packaging)** - NuGet structure

---

## What's Next?

### For SDK Users
- Read [Using the SDK](../getting-started/using-the-sdk) for comprehensive usage guide
- Explore [Code Examples](../examples) for practical integration patterns
- Check [API Reference](../api-reference) for complete API documentation

### For SDK Developers
- Follow [Installation & Setup](../getting-started/installation) to set up your development environment
- Read [Developer Guide](../getting-started/developer-guide) for contribution guidelines
- Explore [Architecture](../architecture/) to understand the project structure
