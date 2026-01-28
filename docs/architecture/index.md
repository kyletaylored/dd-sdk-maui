---
layout: default
title: Architecture
nav_order: 3
has_children: true
---

# Architecture

Learn about the internal structure, build system, and workflows of the Datadog MAUI SDK.

## Overview

The Datadog MAUI SDK is built on a multi-layered architecture:

1. **Native SDK Bindings** - Xamarin bindings for Android and iOS native SDKs
2. **Platform Implementations** - Platform-specific code for Android and iOS
3. **Unified API Layer** - Cross-platform .NET MAUI API
4. **NuGet Packages** - Organized package structure for distribution

## Documentation

### Build System & Workflows

- **[Workflow Architecture](../architecture/workflow_architecture.html)** - CI/CD pipelines and GitHub Actions
  - Build validation workflow
  - Package publishing workflow
  - Documentation deployment
  - Automation triggers

- **[Scripts Overview](../architecture/scripts_overview.html)** - Build automation and utility scripts
  - Android binding scripts
  - iOS binding scripts
  - Packaging scripts
  - Testing scripts

### Package Structure

- **[Packaging Architecture](../architecture/packaging_architecture.html)** - NuGet package organization
  - Package hierarchy
  - Dependencies between packages
  - Platform-specific packages
  - Unified plugin package

## Quick Reference

| Topic | Document | Description |
|-------|----------|-------------|
| CI/CD | [Workflow Architecture](../architecture/workflow_architecture.html) | GitHub Actions workflows |
| Build Scripts | [Scripts Overview](../architecture/scripts_overview.html) | Automation scripts and tools |
| Packages | [Packaging Architecture](../architecture/packaging_architecture.html) | NuGet package structure |

## Related Documentation

- [Project Guide](../project/project_guide.html) - Complete project overview
- [Android Dependencies](../guides/android/android_dependencies.html) - Android dependency management
- [iOS Binding Strategy](../guides/ios/ios_binding_strategy.html) - iOS binding approach
