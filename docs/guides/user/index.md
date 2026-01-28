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

### RUM Action Naming

**[RUM Action Naming](rum-actions)** - Improve RUM action names for better observability:

- **AutomationId Property**: Tag MAUI elements with readable identifiers
- **Manual Action Tracking**: Add custom RUM actions with rich context
- **Best Practices**: Naming conventions and implementation patterns
- **Examples**: Complete cart implementation with proper naming

**Key Topics**:
- Why action names are cryptic by default
- Using AutomationId for automatic tracking
- Manual RUM action tracking with attributes
- Hybrid approach for comprehensive tracking

### Mapping File Uploads

**[Mapping File Uploads](../../guides/user/mapping-files)** - Upload mapping files for crash symbolication:

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

1. **Want readable RUM action names?** → Read [RUM Action Naming](rum-actions)
2. **Using RUM Error Tracking?** → Read [Mapping File Uploads](../../guides/user/mapping-files)
3. **Using ProGuard/R8?** → See [R8 Configuration](mapping-files#prerequisites-enable-r8-code-shrinking)
4. **Using Native Code (NDK)?** → See [NDK Symbol Upload](mapping-files#ndk-symbol-files)

### Common Tasks

| Task | Guide Section |
|------|---------------|
| Improve RUM action names | [RUM Action Naming](rum-actions) |
| Add AutomationId to elements | [RUM Action Naming - AutomationId](rum-actions#solution-1-automationid-property) |
| Track custom actions | [RUM Action Naming - Manual Tracking](rum-actions#solution-2-manual-rum-action-tracking) |
| Enable R8 | [Mapping Files - Prerequisites](mapping-files#prerequisites-enable-r8-code-shrinking) |
| Find mapping file | [Mapping Files - File Locations](../../guides/user/mapping-files) |
| Upload with CLI | [Mapping Files - Datadog CLI](mapping-files#method-1-datadog-cli) |
| Automate upload | [Mapping Files - CI/CD](../../guides/user/mapping-files) |

## Related Documentation

- [Getting Started](../../getting-started/installation) - SDK installation and setup
- [Android Dependencies](../android/dependencies) - Android-specific configuration
- [Unified API Design](../../getting-started/api-reference) - SDK API reference
