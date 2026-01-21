# Datadog MAUI SDK Documentation

Complete documentation for developers working on or using the Datadog MAUI SDK.

---

## Quick Start

**New to the project?** → [Project Guide](PROJECT_GUIDE.md)

**Building packages?** → [Scripts Overview](SCRIPTS_OVERVIEW.md)

**Working with Android?** → [Android Dependencies](ANDROID_DEPENDENCIES.md)

**Working with iOS?** → [iOS Binding Strategy](IOS_BINDING_STRATEGY.md)

---

## Core Documentation

### For SDK Users

- **[Getting Started](GETTING_STARTED.md)** - Installation and basic usage
- **[Unified API Design](UNIFIED_API_DESIGN.md)** - Cross-platform API reference

### For SDK Developers

- **[Project Guide](PROJECT_GUIDE.md)** - Architecture, status, and structure
- **[Scripts Overview](SCRIPTS_OVERVIEW.md)** - Build automation and tools
- **[Workflow Architecture](WORKFLOW_ARCHITECTURE.md)** - CI/CD pipelines
- **[Packaging Architecture](PACKAGING_ARCHITECTURE.md)** - NuGet package structure

### Android Development

- **[Android Dependencies](ANDROID_DEPENDENCIES.md)** - Complete dependency management guide
  - Maven to NuGet mapping
  - Centralized core pattern
  - Quick reference tables
  - Troubleshooting
- **[Android Integration Packages](ANDROID_INTEGRATION_PACKAGES.md)** - Optional integrations (OkHttp, OpenTelemetry)

### iOS Development

- **[iOS Binding Strategy](IOS_BINDING_STRATEGY.md)** - Complete approach and methodology
  - Comparison of generated vs manual bindings
  - API identification methodology
  - Implementation checklist
- **[Identifying User-Facing APIs](IDENTIFYING_USER_FACING_APIS.md)** - How to determine which APIs to expose
- **[RUM Binding Comparison](RUM_BINDING_COMPARISON.md)** - Concrete before/after example

### Maintenance

- **[Changelog](CHANGELOG.md)** - Version history
- **[Contributing](CONTRIBUTING.md)** - How to contribute
- **[Automation Roadmap](AUTOMATION_ROADMAP.md)** - Future improvements

---

## Documentation by Task

### Adding a New Android Package

1. Read [Android Integration Packages](ANDROID_INTEGRATION_PACKAGES.md#adding-more-integration-packages)
2. Use [Scripts Overview](SCRIPTS_OVERVIEW.md#adding-a-new-android-integration-package) workflow
3. Reference [_reference/ANDROID_PACKAGES_ANALYSIS.md](_reference/ANDROID_PACKAGES_ANALYSIS.md) for available packages

### Understanding Android Dependencies

1. Start with [Android Dependencies](ANDROID_DEPENDENCIES.md#the-problem-and-solution)
2. Use [Quick Reference](ANDROID_DEPENDENCIES.md#quick-reference) for lookups
3. Check [Troubleshooting](ANDROID_DEPENDENCIES.md#troubleshooting) if stuck

### Upgrading Datadog SDK Version

1. Run `./scripts/update-sdk-version.sh NEW_VERSION`
2. Follow [Version Updates](ANDROID_DEPENDENCIES.md#version-updates) guide
3. Test with `./scripts/pack.sh`

### Creating iOS Bindings

1. Read [iOS Binding Strategy](IOS_BINDING_STRATEGY.md)
2. Use [Identifying User-Facing APIs](IDENTIFYING_USER_FACING_APIS.md) methodology
3. Reference [RUM Binding Comparison](RUM_BINDING_COMPARISON.md) for examples

### Understanding the Build System

1. Start with [Scripts Overview](SCRIPTS_OVERVIEW.md)
2. Review [Workflow Architecture](WORKFLOW_ARCHITECTURE.md) for CI/CD
3. See [Packaging Architecture](PACKAGING_ARCHITECTURE.md) for package structure

---

## Reference Documentation

Historical and reference documentation in `_reference/` directory:

- **[_reference/BUILD_PROCESS_VALIDATION.md](_reference/BUILD_PROCESS_VALIDATION.md)** - Build validation record
- **[_reference/SCRIPT_AUDIT.md](_reference/SCRIPT_AUDIT.md)** - Original script audit
- **[_reference/SCRIPT_FIXES_COMPLETE.md](_reference/SCRIPT_FIXES_COMPLETE.md)** - Bash compatibility fixes
- **[_reference/ANDROID_PACKAGES_ANALYSIS.md](_reference/ANDROID_PACKAGES_ANALYSIS.md)** - All 27 available Android packages
- **[_reference/NEW_ANDROID_PACKAGES_STATUS.md](_reference/NEW_ANDROID_PACKAGES_STATUS.md)** - Integration package implementation history
- **[_reference/android_dep_research.md](_reference/android_dep_research.md)** - Dependency verification research

---

## Document Status

| Document | Purpose | Status |
|----------|---------|--------|
| **Core Guides** |
| [PROJECT_GUIDE.md](PROJECT_GUIDE.md) | Complete project overview | ✅ Complete |
| [ANDROID_DEPENDENCIES.md](ANDROID_DEPENDENCIES.md) | Android dependency management | ✅ Complete |
| [IOS_BINDING_STRATEGY.md](IOS_BINDING_STRATEGY.md) | iOS binding approach | ✅ Complete |
| [SCRIPTS_OVERVIEW.md](SCRIPTS_OVERVIEW.md) | Build automation | ✅ Complete |
| **Architecture** |
| [WORKFLOW_ARCHITECTURE.md](WORKFLOW_ARCHITECTURE.md) | CI/CD pipelines | ✅ Complete |
| [PACKAGING_ARCHITECTURE.md](PACKAGING_ARCHITECTURE.md) | Package structure | ✅ Complete |
| **Platform-Specific** |
| [ANDROID_INTEGRATION_PACKAGES.md](ANDROID_INTEGRATION_PACKAGES.md) | Android integrations | ✅ Complete |
| [IDENTIFYING_USER_FACING_APIS.md](IDENTIFYING_USER_FACING_APIS.md) | API methodology | ✅ Complete |
| [RUM_BINDING_COMPARISON.md](RUM_BINDING_COMPARISON.md) | iOS binding example | ✅ Complete |
| **User Guides** |
| [UNIFIED_API_DESIGN.md](UNIFIED_API_DESIGN.md) | API specification | 🚧 In Progress |
| [GETTING_STARTED.md](GETTING_STARTED.md) | User setup guide | 🚧 In Progress |
| **Planning** |
| [CHANGELOG.md](CHANGELOG.md) | Version history | ✅ Complete |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Contribution guide | ✅ Complete |
| [AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md) | Future plans | ✅ Complete |

**Legend**: ✅ Complete | 🚧 In Progress | 📝 Planned

---

## Documentation Philosophy

### Single Source of Truth

Each topic has ONE comprehensive document. No overlapping or redundant content.

**Example**: Android dependencies
- ✅ ONE doc: `ANDROID_DEPENDENCIES.md` (comprehensive with quick reference built-in)
- ❌ NOT: Separate docs for "comprehensive", "quick reference", and "version mapping"

### Progressive Disclosure

Documents start with quick start, then provide depth for those who need it.

**Structure**:
1. Quick navigation at top
2. Problem statement and solution
3. Quick reference tables
4. Detailed implementation guide
5. Advanced topics
6. Troubleshooting

### Keep It Current

- Update docs when code changes
- Archive old docs to `_reference/` instead of deleting
- Mark status in tables above
- Date all major updates

---

## Contributing to Documentation

### Before Adding a New Document

**Ask yourself**:
1. Does this content belong in an existing doc?
2. Is this temporary (status update) or permanent (guide)?
3. Will users actually need this 6 months from now?

**If temporary**: Put in `_reference/` with date in filename

**If adding value**: Create new doc and update this README

### Document Template

```markdown
# Document Title

Brief 1-sentence description of what this doc covers.

---

## Quick Start

Links to jump to specific sections for common tasks.

---

## Main Content

Organized logically with clear headers.

---

## Related Documentation

Links to related docs.

---

**Last Updated**: YYYY-MM-DD
```

### Review Checklist

- [ ] Clear purpose stated at top
- [ ] Quick navigation/TOC for long docs
- [ ] Code examples tested and working
- [ ] Cross-references to related docs
- [ ] No duplicate content
- [ ] Updated this README with new doc

---

## External Resources

- **[Datadog Mobile SDK Docs](https://docs.datadoghq.com/mobile/)** - Official Datadog documentation
- **[dd-sdk-android GitHub](https://github.com/DataDog/dd-sdk-android)** - Native Android SDK
- **[dd-sdk-ios GitHub](https://github.com/DataDog/dd-sdk-ios)** - Native iOS SDK
- **[.NET MAUI Docs](https://learn.microsoft.com/en-us/dotnet/maui/)** - Microsoft MAUI
- **[Android Bindings Guide](https://learn.microsoft.com/en-us/xamarin/android/platform/binding-java-library/)** - Xamarin Android bindings
- **[iOS Bindings Guide](https://learn.microsoft.com/en-us/xamarin/ios/platform/binding-objective-c/)** - Xamarin iOS bindings

---

## Getting Help

**For Package Users**:
- Review [Getting Started](GETTING_STARTED.md)
- Check [Unified API Design](UNIFIED_API_DESIGN.md)
- File issues on GitHub

**For SDK Developers**:
- Start with [Project Guide](PROJECT_GUIDE.md)
- Review platform-specific guide (Android/iOS)
- Check [Scripts Overview](SCRIPTS_OVERVIEW.md) for build issues

---

**Last Updated**: 2026-01-20
**Total Active Docs**: 17
**Documentation Maintainer**: SDK Development Team
