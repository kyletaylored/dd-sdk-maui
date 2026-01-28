---
layout: default
title: iOS
nav_order: 2
parent: Guides
has_children: true
---

# iOS Development Guides

Complete guides for iOS development with the Datadog MAUI SDK.

## Core Documentation

### Binding Strategy

**[iOS Binding Strategy](../../guides/ios/binding-strategy)** - Complete approach to iOS bindings:

- **Generated vs Manual Bindings**: Comparison and decision framework
- **API Identification Methodology**: How to determine what to bind
- **Implementation Checklist**: Step-by-step binding process
- **Best Practices**: Lessons learned and recommendations

**Key Topics**:
- Sharpie-generated bindings vs manual
- Minimal viable binding approach
- Public vs internal API identification
- Testing and validation

### API Identification

**[Identifying User-Facing APIs](../../guides/ios/api-identification)** - Methodology for API identification:

- **Analysis Approach**: How to analyze native SDKs
- **Classification System**: Public vs internal APIs
- **Decision Framework**: What to include/exclude
- **Examples**: Real-world API analysis

### Concrete Example

**[RUM Binding Comparison](../../guides/ios/rum-example)** - Before/after binding example:

- **Generated Binding**: What Sharpie produces
- **Manual Binding**: Curated, minimal approach
- **Analysis**: Side-by-side comparison
- **Lessons Learned**: Key takeaways

## Quick Start

### For New iOS Developers

1. Read [iOS Binding Strategy](../../guides/ios/binding-strategy) for the overall approach
2. Use [Identifying User-Facing APIs](../../guides/ios/api-identification) to analyze what to bind
3. Review [RUM Binding Comparison](../../guides/ios/rum-example) for a concrete example

### Common Tasks

| Task | Guide Section |
|------|---------------|
| Start new iOS binding | [Binding Strategy - Implementation Checklist](binding-strategy#implementation-checklist) |
| Identify APIs to bind | [API Identification - Analysis Approach](api-identification#analysis-approach) |
| Decide on approach | [Binding Strategy - Decision Framework](binding-strategy#generated-vs-manual-bindings) |
| See real example | [RUM Comparison - Full Example](../../guides/ios/rum-example) |

## iOS Binding Workflow

```mermaid
graph TD
    A[Start] --> B[Analyze Native SDK]
    B --> C[Identify User-Facing APIs]
    C --> D{Approach?}
    D -->|Generated| E[Use Sharpie]
    D -->|Manual| F[Write Bindings]
    E --> G[Curate & Test]
    F --> G
    G --> H[Validate]
    H --> I[Complete]
```

## Related Documentation

- [Project Guide](../../project/overview) - Overall project structure
- [Scripts Overview](../../architecture/scripts) - Build automation for iOS
- [Packaging Architecture](../../architecture/packaging) - iOS package structure
