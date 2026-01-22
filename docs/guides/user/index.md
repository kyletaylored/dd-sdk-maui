---
layout: default
title: User Guides
nav_order: 3
parent: Guides
has_children: true
---

# User Guides

Guides for developers using the Datadog MAUI SDK in their applications.

## Available Guides

### Mapping File Uploads

**[Mapping File Uploads](../../guides/user/MAPPING_FILE_UPLOADS.html)** - Upload mapping files for crash symbolication:

- **ProGuard/R8 Mapping Files**: Deobfuscate Android crash reports
- **NDK Symbol Files**: Symbolicate native C/C++ crashes
- **Upload Methods**: Datadog CLI, Gradle plugin, API
- **Configuration**: Enable R8 and generate mapping files

**Key Topics**:
- Why mapping files are needed
- Enabling R8 code shrinking
- Finding generated mapping files
- Upload methods and automation
- CI/CD integration

## Quick Start

### For MAUI App Developers

1. **Using RUM Error Tracking?** → Read [Mapping File Uploads](../../guides/user/MAPPING_FILE_UPLOADS.html)
2. **Using ProGuard/R8?** → See [R8 Configuration](MAPPING_FILE_UPLOADS.html#prerequisites-enable-r8-code-shrinking)
3. **Using Native Code (NDK)?** → See [NDK Symbol Upload](MAPPING_FILE_UPLOADS.html#ndk-symbol-files)

### Common Tasks

| Task | Guide Section |
|------|---------------|
| Enable R8 | [Mapping Files - Prerequisites](MAPPING_FILE_UPLOADS.html#prerequisites-enable-r8-code-shrinking) |
| Find mapping file | [Mapping Files - File Locations](../../guides/user/MAPPING_FILE_UPLOADS.html) |
| Upload with CLI | [Mapping Files - Datadog CLI](MAPPING_FILE_UPLOADS.html#method-1-datadog-cli) |
| Automate upload | [Mapping Files - CI/CD](../../guides/user/MAPPING_FILE_UPLOADS.html) |

## Related Documentation

- [Getting Started](../../getting-started/GETTING_STARTED.html) - SDK installation and setup
- [Android Dependencies](../android/ANDROID_DEPENDENCIES.html) - Android-specific configuration
- [Unified API Design](../../getting-started/UNIFIED_API_DESIGN.html) - SDK API reference
