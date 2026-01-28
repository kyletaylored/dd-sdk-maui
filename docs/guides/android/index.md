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

**[Android Dependencies](../../guides/android/dependencies)** - Complete guide to managing Android dependencies:

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

**[Android Integration Packages](../../guides/android/integrations)** - Optional integration modules:

- **OkHttp Integration**: HTTP client instrumentation
- **OpenTelemetry Integration**: OTel tracing support
- **Adding New Packages**: How to add more integration packages

## Quick Start

### For New Android Developers

1. Read [Android Dependencies](dependencies#the-problem-and-solution) to understand the architecture
2. Use [Quick Reference](dependencies#quick-reference) tables for lookups
3. Check [Troubleshooting](dependencies#troubleshooting) if you hit issues

### Common Tasks

| Task | Guide Section |
|------|---------------|
| Add AndroidX dependency | [Android Dependencies - AndroidX](dependencies#androidx-dependencies) |
| Add Maven library | [Android Dependencies - Maven](dependencies#maven-dependencies) |
| Add integration package | [Integration Packages - Adding More](integrations#adding-more-integration-packages) |
| Fix version conflict | [Dependencies - Troubleshooting](dependencies#troubleshooting) |
| Update SDK version | [Dependencies - Version Updates](dependencies#version-updates) |

## Related Documentation

- [Project Guide](../../project/overview) - Overall project structure
- [Scripts Overview](../../architecture/scripts) - Build automation for Android
- [Packaging Architecture](../../architecture/packaging) - Android package structure
