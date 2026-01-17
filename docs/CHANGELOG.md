---
layout: default
title: Changelog
nav_order: 9
description: "Release history and notable changes for the Datadog SDK for .NET MAUI"
permalink: /changelog
---

# Changelog

All notable changes to the Datadog SDK for .NET MAUI will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project scaffolding
- iOS binding project with XCFramework support
- Android binding project with Maven dependency management
- Main plugin with platform abstraction layer
- Cross-platform API for RUM, Logs, and Tracing
- Build scripts for downloading iOS frameworks and Android artifacts
- Sample MAUI application
- GitHub Actions CI/CD workflow
- Comprehensive documentation

### Planned
- Generate complete iOS bindings using Objective Sharpie
- Implement full Android binding with all Datadog modules
- Complete platform-specific implementations
- Add unit tests
- Add integration tests
- Performance benchmarks
- Advanced features (Session Replay, WebView tracking, Feature Flags)

## [3.5.0] - TBD

### Added
- Initial release matching Datadog native SDK version 3.5.0
- Support for .NET 8, 9, 10
- iOS support (iOS 12.0+)
- Android support (API 21+)
- Core SDK features:
  - Real User Monitoring (RUM)
  - Log Collection
  - APM Distributed Tracing
  - Crash Reporting
  - User identification and attributes
  - Custom events and metrics
- Platform-specific bindings:
  - iOS: DatadogCore, DatadogRUM, DatadogLogs, DatadogTrace, DatadogCrashReporting, DatadogSessionReplay, DatadogWebViewTracking, DatadogFlags, DatadogInternal
  - Android: All dd-sdk-android modules including core, RUM, logs, trace, session replay, webview, flags, and NDK
- Configuration options:
  - Multiple Datadog site support (US1, EU1, US3, US5, US1-FED, AP1)
  - Session and trace sampling rates
  - Feature toggles for crash reporting, user interaction tracking, network tracking
  - Custom global attributes

### Known Limitations
- iOS bindings are placeholder stubs (requires Objective Sharpie generation)
- Android binding may require additional metadata transforms
- Some advanced features not yet implemented
- Documentation is preliminary

[Unreleased]: https://github.com/DataDog/dd-sdk-maui/compare/v3.5.0...HEAD
[3.5.0]: https://github.com/DataDog/dd-sdk-maui/releases/tag/v3.5.0
