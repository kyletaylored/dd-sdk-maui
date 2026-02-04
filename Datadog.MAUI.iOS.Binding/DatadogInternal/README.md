# Datadog iOS Internal - .NET Binding

.NET MAUI binding for the Datadog Internal infrastructure package for iOS. This package provides internal utilities and shared types used across Datadog iOS SDK modules.

## About

This package provides .NET bindings for the native `DatadogInternal` iOS framework. It contains internal infrastructure components that are shared across all Datadog iOS SDK modules (Core, RUM, Logs, Trace, etc.).

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.Binding.DatadogInternal --version 3.5.0
```

**Important:** This is an infrastructure package that is **not intended for direct application use**. It is automatically included as a transitive dependency when you install other Datadog iOS bindings.

## Purpose

DatadogInternal provides:

- **Shared Utilities** - Common helper functions used across SDK modules
- **Internal Types** - Data structures and interfaces for inter-module communication
- **Foundation Extensions** - Internal extensions to Foundation framework types
- **Telemetry Infrastructure** - Internal telemetry and diagnostics
- **Protocol Definitions** - Internal protocols for SDK module integration

## When This Package Is Needed

This package is automatically referenced when you use:
- `Datadog.MAUI.iOS.Binding.DatadogCore`
- `Datadog.MAUI.iOS.Binding.DatadogRUM`
- `Datadog.MAUI.iOS.Binding.DatadogLogs`
- `Datadog.MAUI.iOS.Binding.DatadogTrace`
- `Datadog.MAUI.iOS.Binding.DatadogSessionReplay`
- Any other Datadog iOS binding package

## Usage

You do not need to write code against this package directly. The Datadog iOS SDK modules use it internally for:

- Cross-module data sharing
- Internal event processing
- Shared configuration
- Telemetry collection

### Example Dependency Graph

```
Your MAUI App
    └─> Datadog.MAUI
        └─> Datadog.MAUI.iOS.Binding.DatadogRUM
            └─> Datadog.MAUI.iOS.Binding.DatadogCore
                └─> Datadog.MAUI.iOS.Binding.DatadogInternal ← Transitive dependency
```

## Development Notes

If you're developing custom Datadog integrations or contributing to the SDK bindings, you may encounter types from this namespace:

```csharp
using DatadogInternal;

// Internal types are typically not documented for public use
// They may change between SDK versions
```

**Caution:** Internal APIs are not covered by semantic versioning guarantees and may change in minor or patch releases. Avoid depending on internal types in production code.

## Integration with MAUI Plugin

The `Datadog.MAUI` plugin does not expose DatadogInternal types in its public API. All interactions with the Datadog SDK should go through the documented public interfaces:

- `Datadog.Maui.Datadog` - Main SDK initialization
- `Datadog.Maui.Rum.GlobalRum` - RUM tracking
- `Datadog.Maui.Logs.Logger` - Log collection
- `Datadog.Maui.Tracing.Tracer` - Distributed tracing

## Package Contents

This binding includes the `DatadogInternal.xcframework` which contains:
- iOS arm64 (device)
- iOS arm64 Simulator
- iOS x86_64 Simulator (legacy)

## Native iOS Reference

DatadogInternal is not documented in public Datadog documentation as it's intended for internal SDK use only.

For public-facing APIs, see:
- [Datadog iOS SDK Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)

## Version Information

- **Native SDK Version**: 3.5.0
- **XCFramework**: `DatadogInternal.xcframework`
- **Supported iOS**: 12.0+

## Dependencies

This package has no external dependencies (it's a base infrastructure package).

## License

Apache 2.0 - See main repository LICENSE file.

## Support

If you encounter issues with this package:
1. Ensure you're using compatible versions of all Datadog iOS bindings (same version)
2. Check that your MAUI project targets a supported iOS version (12.0+)
3. File issues at the main repository issue tracker

**Do not attempt to use DatadogInternal APIs directly**unless you are contributing to the SDK itself.
