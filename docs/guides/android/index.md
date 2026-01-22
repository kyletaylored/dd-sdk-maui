---
layout: default
title: Android
nav_order: 1
parent: Guides
has_children: true
---

# Android Development Guides

Complete guides for Android development with the Datadog MAUI SDK.

## Core Documentation

### Dependency Management

**[Android Dependencies](../../guides/android/ANDROID_DEPENDENCIES.html)** - Complete guide to managing Android dependencies:

- **The Problem and Solution**: Understanding Maven to NuGet mapping
- **Quick Reference**: Fast lookup tables for dependency handling
- **Implementation Guide**: Step-by-step dependency setup
- **Troubleshooting**: Common issues and solutions

**Key Topics**:
- Centralized core pattern
- Maven to NuGet version mapping
- AndroidMavenLibrary vs PackageReference
- Kotlin stdlib handling
- AndroidX dependency management

### Integration Packages

**[Android Integration Packages](../../guides/android/ANDROID_INTEGRATION_PACKAGES.html)** - Optional integration modules:

- **OkHttp Integration**: HTTP client instrumentation
- **OpenTelemetry Integration**: OTel tracing support
- **Adding New Packages**: How to add more integration packages

## Quick Start

### For New Android Developers

1. Read [Android Dependencies](ANDROID_DEPENDENCIES.html#the-problem-and-solution) to understand the architecture
2. Use [Quick Reference](ANDROID_DEPENDENCIES.html#quick-reference) tables for lookups
3. Check [Troubleshooting](ANDROID_DEPENDENCIES.html#troubleshooting) if you hit issues

### Common Tasks

| Task | Guide Section |
|------|---------------|
| Add AndroidX dependency | [Android Dependencies - AndroidX](ANDROID_DEPENDENCIES.html#androidx-dependencies) |
| Add Maven library | [Android Dependencies - Maven](ANDROID_DEPENDENCIES.html#maven-dependencies) |
| Add integration package | [Integration Packages - Adding More](ANDROID_INTEGRATION_PACKAGES.html#adding-more-integration-packages) |
| Fix version conflict | [Dependencies - Troubleshooting](ANDROID_DEPENDENCIES.html#troubleshooting) |
| Update SDK version | [Dependencies - Version Updates](ANDROID_DEPENDENCIES.html#version-updates) |

## Related Documentation

- [Project Guide](../../project/PROJECT_GUIDE.html) - Overall project structure
- [Scripts Overview](../../architecture/SCRIPTS_OVERVIEW.html) - Build automation for Android
- [Packaging Architecture](../../architecture/PACKAGING_ARCHITECTURE.html) - Android package structure
