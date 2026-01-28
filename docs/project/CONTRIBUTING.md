---
layout: default
title: Contributing
parent: Project
nav_order: 2
description: "How to contribute to the Datadog SDK for .NET MAUI"
---

# Contributing to Datadog MAUI SDK

Thank you for your interest in contributing to the Datadog SDK for .NET MAUI!

## Development Setup

### Prerequisites

- .NET 8+ SDK
- macOS with Xcode 14+ (for iOS development)
- Android SDK API Level 34+
- Git

### Getting Started

1. **Clone the repository**

```bash
git clone https://github.com/DataDog/dd-sdk-maui.git
cd dd-sdk-maui
```

2. **Download native frameworks**

```bash
# Download iOS XCFrameworks
./scripts/download-ios-frameworks.sh

# (Optional) Check Android dependencies
./scripts/update-android-dependencies.ps1 -Version 3.5.0 -OutputFile deps.json
```

3. **Build the project**

```bash
./scripts/build.sh Debug
```

## Project Structure

```
dd-sdk-maui/
├── Datadog.MAUI.iOS.Binding/          # iOS native binding
│   ├── *.xcframework/                  # XCFrameworks (git-ignored)
│   ├── ApiDefinition.cs                # Objective-C API definitions
│   └── StructsAndEnums.cs              # Enums and structs
├── Datadog.MAUI.Android.Binding/      # Android native binding
│   ├── Additions/                      # Custom C# code
│   └── Transforms/Metadata.xml         # Binding metadata
├── Datadog.MAUI.Plugin/               # Main cross-platform API
│   ├── Shared/                         # Shared interfaces
│   └── Platforms/                      # Platform implementations
├── samples/DatadogMauiSample/         # Sample application
└── scripts/                            # Build and utility scripts
```

## Making Changes

### Binding Changes

#### iOS Binding

The iOS binding uses Objective-C bindings generated from XCFrameworks:

1. Download the XCFrameworks: `./scripts/download-ios-frameworks.sh`
2. Generate bindings using Objective Sharpie (optional):
   ```bash
   sharpie bind --output=Generated --namespace=DatadogMaui.iOS \
     --sdk=iphoneos17.0 \
     DatadogCore.xcframework/ios-arm64/DatadogCore.framework/Headers/*.h
   ```
3. Update [ApiDefinition.cs](Datadog.MAUI.iOS.Binding/ApiDefinition.cs) with the generated bindings
4. Update [StructsAndEnums.cs](Datadog.MAUI.iOS.Binding/StructsAndEnums.cs) if needed

#### Android Binding

The Android binding uses AndroidMavenLibrary references:

1. Update [Datadog.MAUI.Android.Binding.csproj](Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj) with Maven dependencies
2. If binding errors occur, fix them in [Transforms/Metadata.xml](Datadog.MAUI.Android.Binding/Transforms/Metadata.xml)
3. Add custom code in [Additions/](Datadog.MAUI.Android.Binding/Additions/) if needed

### API Changes

When adding or modifying the public API:

1. Update interfaces in [Datadog.MAUI.Plugin/Shared/](Datadog.MAUI.Plugin/Shared/)
2. Implement for both platforms:
   - [iOS implementation](Datadog.MAUI.Plugin/Platforms/iOS/DatadogSdkImplementation.cs)
   - [Android implementation](Datadog.MAUI.Plugin/Platforms/Android/DatadogSdkImplementation.cs)
3. Update [Documentation Index](../index.html) with usage examples
4. Add tests if applicable

## Testing

### Local Testing

1. Build the packages:
   ```bash
   ./scripts/build.sh Release
   ```

2. Reference the local packages in your test project:
   ```xml
   <ItemGroup>
     <PackageReference Include="Datadog.MAUI" Version="3.5.0">
       <Source>./path/to/dd-sdk-maui/artifacts/packages</Source>
     </PackageReference>
   </ItemGroup>
   ```

3. Test on iOS and Android devices/simulators

### Sample Application

The [samples/DatadogMauiSample](samples/DatadogMauiSample) project can be used for testing:

1. Add your Datadog credentials to [MauiProgram.cs](samples/DatadogMauiSample/MauiProgram.cs)
2. Run on iOS: `dotnet build -f net9.0-ios && open -a Simulator`
3. Run on Android: `dotnet build -f net9.0-android`

## Version Management

### Updating SDK Version

The Datadog SDK version is managed in [Directory.Build.props](Directory.Build.props):

```xml
<DatadogSdkVersion>3.5.0</DatadogSdkVersion>
```

When updating to a new Datadog SDK version:

1. Update `DatadogSdkVersion` in Directory.Build.props
2. Download new iOS frameworks: `./scripts/download-ios-frameworks.sh NEW_VERSION`
3. Update Android dependencies if needed
4. Update CHANGELOG.md
5. Test thoroughly on both platforms

## Code Style

- Follow [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep platform-specific code in Platform folders

## Pull Request Process

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Commit with clear messages: `git commit -m "Add: description of changes"`
5. Push to your fork: `git push origin feature/my-feature`
6. Create a Pull Request

### PR Checklist

- [ ] Code builds without errors
- [ ] Tests pass (if applicable)
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Follows code style guidelines
- [ ] No breaking changes (or documented if necessary)

## Troubleshooting

### iOS Build Fails

- Ensure XCFrameworks are downloaded: `ls Datadog.MAUI.iOS.Binding/*.xcframework`
- Check Xcode version: `xcodebuild -version`
- Clean and rebuild: `dotnet clean && ./scripts/build.sh`

### Android Build Fails

- Check Maven dependencies are accessible
- Review binding errors in build output
- Fix metadata in `Transforms/Metadata.xml`

### Binding Generation Issues

- Use Objective Sharpie for iOS: `brew install sharpie`
- Review generated code before committing
- Test bindings with simple method calls first

## Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [Datadog Documentation](https://docs.datadoghq.com/)
- [Native Library Interop](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/native-library-interop)
- [Xamarin iOS Binding](https://docs.microsoft.com/en-us/xamarin/ios/platform/binding-objective-c/)
- [Xamarin Android Binding](https://docs.microsoft.com/en-us/xamarin/android/platform/binding-java-library/)

## Questions?

- Open an issue for bugs or feature requests
- Check existing issues before creating new ones
- Be respectful and constructive in discussions

Thank you for contributing!
