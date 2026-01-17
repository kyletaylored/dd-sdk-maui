# Datadog MAUI SDK Documentation

Welcome to the Datadog MAUI SDK documentation. This directory contains comprehensive guides for maintaining and extending the SDK bindings.

## Quick Links

- **New to the project?** → [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)
- **Want technical details?** → [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)
- **Having issues with dependencies?** → [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md)
- **Need quick answers?** → [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md)
- **Planning automation?** → [AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md)

## Document Index

### Project Documentation

#### [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)

**Quick overview and current status**

High-level summary of the project with current state, accomplishments, and key metrics. Perfect for:

- Getting a quick update on project progress
- Understanding what's complete and what's in progress
- Seeing key technical achievements
- Finding the right documentation quickly

**Read this when**:

- You're new to the project
- You want a quick status update
- You need to understand the current state
- You're looking for specific documentation

#### [PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)

**Comprehensive technical overview**

Detailed technical documentation covering architecture, build system, decisions, and roadmap. Includes:

- Complete architecture breakdown
- Component descriptions
- Build system and scripts
- CI/CD pipeline design
- Version management
- Technical decisions and rationale
- Implementation roadmap

**Read this when**:

- You need to understand the architecture
- You're contributing to the project
- You're planning significant changes
- You need to understand technical decisions

### Core Documentation

#### [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md)

**Comprehensive guide to Android dependency management**

Essential reading for understanding how dependencies work in the binding projects. Covers:

- The dependency problem and why it exists
- The centralized core pattern (our solution)
- Dependency categories and classification
- Step-by-step implementation guide
- Troubleshooting common errors
- Future update procedures
- Key insights and gotchas

**Read this when**:

- Setting up binding projects for the first time
- Encountering duplicate class errors
- Updating to a new Datadog SDK version
- Contributing dependency-related changes

#### [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md)

**Fast lookup table and command reference**

Quick answers without the theory. Includes:

- Decision tree flowchart
- Dependency lookup table
- Project file templates
- Error code meanings and fixes
- Version update checklist
- Common patterns

**Read this when**:

- You know what you're doing, just need the specifics
- Looking up how to handle a specific dependency
- Need a command or pattern quickly
- Doing a routine SDK version update

#### [AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md)

**Future vision for dependency automation**

Analysis of current vs. desired automation state. Covers:

- Current script capabilities and limitations
- Enhanced configuration-driven system design
- Recommended system architecture
- Configuration file design (deps-config.yaml)
- Implementation plan (5 phases)
- ROI analysis

**Read this when**:

- Planning to improve automation
- Evaluating whether to invest in tooling
- Designing the next generation of scripts
- Understanding the long-term vision

## Getting Started

### For New Contributors

1. Start with [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md) sections:

   - Overview
   - The Dependency Problem
   - The Solution Pattern

2. Bookmark [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md) for daily use

3. When ready to improve tooling, read [AUTOMATION_ROADMAP.md](AUTOMATION_ROADMAP.md)

### For SDK Version Updates

1. Follow the checklist in [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md#version-update-checklist)

2. Reference [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md#future-updates) for detailed procedures

3. Document any new patterns or gotchas discovered

### For Troubleshooting

1. Check [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md#error-code-cheat-sheet) for your specific error

2. If not found, consult [ANDROID_DEPENDENCY_MANAGEMENT.md](ANDROID_DEPENDENCY_MANAGEMENT.md#troubleshooting)

3. Still stuck? Review [Key Insights](ANDROID_DEPENDENCY_MANAGEMENT.md#key-insights) section

## Current Status (January 2026)

### ✅ What Works

- **9/9 Android binding projects** build successfully
- **Sample app** compiles and demonstrates SDK usage
- **Dependency pattern** established and documented
- **Duplicate issues** resolved (gson, jetbrains.annotations)
- **Documentation** comprehensive and up-to-date

### 🎯 Future Goals

- Automated dependency categorization from POMs
- NuGet package discovery via API
- Project file generation from configuration
- CI/CD integration for validation

## Key Principles

### 1. Centralize Shared Dependencies

**Core provides, features consume**. Shared dependencies live in core as NuGet packages, feature modules declare them as ignored.

### 2. Use NuGet When Available

**Prefer NuGet over Maven bindings**. Xamarin/AndroidX packages are well-maintained and handle complex binding scenarios.

### 3. Document the Why

**Decisions have reasons**. Every dependency placement should have a documented rationale, especially for non-obvious cases.

### 4. Automate Incrementally

**Make common tasks easy**. Build tooling that saves time on repetitive tasks while allowing manual control for edge cases.

### 5. Test Thoroughly

**Build errors are better than runtime crashes**. Always test builds after dependency changes.

## FAQ

### Q: Why do we use GoogleGson NuGet instead of binding from Maven?

**A**: Gson has complex Java generics and edge cases requiring extensive metadata transforms. The GoogleGson package has these built-in. Direct Maven binding fails with CS0534 errors about unimplemented abstract members.

See: [ANDROID_DEPENDENCY_MANAGEMENT.md - Key Insights #2](ANDROID_DEPENDENCY_MANAGEMENT.md#2-googlegson-must-always-use-nuget-package)

### Q: What's the difference between AndroidMavenLibrary and PackageReference?

**A**:

- `AndroidMavenLibrary` downloads Java AAR/JAR from Maven and optionally generates C# bindings
- `PackageReference` references pre-built NuGet packages (which may contain bindings or native code)

Use `PackageReference` when a good NuGet binding exists, `AndroidMavenLibrary` otherwise.

### Q: When should I add AndroidIgnoredJavaDependency?

**A**: Add it when a dependency is satisfied through another mechanism:

- Transitively from core (via ProjectReference)
- Transitively from MAUI
- Via NuGet package (when Maven also lists it)

This tells MSBuild "don't error about this missing Maven dependency, I've got it covered."

### Q: How do I know if a dependency is in MAUI?

**A**: Check the "MAUI Transitive" list in [DEPENDENCY_QUICK_REFERENCE.md](DEPENDENCY_QUICK_REFERENCE.md#common-dependencies-quick-lookup) or run:

```bash
dotnet list package --include-transitive | grep -i <package-name>
```

### Q: What if I encounter a new dependency not documented here?

**A**:

1. Check NuGet.org for Xamarin/AndroidX bindings
2. Check if it's a Datadog internal module
3. See if MAUI provides it transitively
4. If none of above, it needs Maven download
5. Document your findings in the appropriate guide

## Contributing

When contributing to these docs:

1. **Keep quick reference quick** - Add only essential info
2. **Keep comprehensive guide comprehensive** - Include rationale and context
3. **Update all three docs** - Ensure consistency across guides
4. **Include examples** - Show, don't just tell
5. **Test commands** - Verify all commands work before documenting

### Document Maintenance

| Document                         | Update Frequency  | Trigger                                         |
| -------------------------------- | ----------------- | ----------------------------------------------- |
| ANDROID_DEPENDENCY_MANAGEMENT.md | Per major insight | New pattern discovered, breaking change         |
| DEPENDENCY_QUICK_REFERENCE.md    | Per SDK update    | Version numbers, new common dependencies        |
| AUTOMATION_ROADMAP.md            | Quarterly         | Progress on automation, architectural decisions |
| This README                      | As needed         | New documents added, major changes              |

## Version History

| Version | Date       | Changes                                              |
| ------- | ---------- | ---------------------------------------------------- |
| 1.0     | 2026-01-16 | Initial documentation set created                    |
|         |            | - Documented dependency management pattern           |
|         |            | - Resolved gson and jetbrains.annotations duplicates |
|         |            | - Established automation roadmap                     |

## Feedback

Documentation unclear? Found an error? Have suggestions?

1. Open an issue on GitHub
2. Submit a PR with improvements
3. Add notes to the relevant section

Good documentation is iterative - your feedback makes it better!

---

**Documentation Version**: 1.0
**SDK Version**: 3.5.0
**Last Updated**: 2026-01-16
