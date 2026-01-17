# Datadog MAUI Plugin Tests

Unit tests for the Datadog MAUI SDK shared interfaces and API contracts.

## Overview

This test project validates the Datadog MAUI SDK API surface without requiring platform-specific implementations or native bindings. It uses mock implementations to test:

- Configuration model and defaults
- Interface contracts (IDatadogSdk, IDatadogLogger, IDatadogRum, IDatadogTrace)
- API behavior and error handling
- Data flow and state management

## Project Structure

```
Datadog.MAUI.Plugin.Tests/
├── DatadogSdkTests.cs           # Configuration and initialization tests
├── DatadogMethodsMock.cs        # Mock implementations for testing
├── IDatadogMethodsTests.cs      # Interface contract tests
└── Usings.cs                    # Global usings
```

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=normal"

# Run specific test
dotnet test --filter "FullyQualifiedName~IDatadogSdkTests"

# Generate code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio / VS Code

- Open Test Explorer
- Click "Run All Tests"
- Or right-click individual tests to run them

## Test Categories

### Configuration Tests (DatadogSdkTests.cs)

Tests for `DatadogConfiguration` model:
- ✅ Valid configuration initialization
- ✅ Default values are correct
- ✅ Custom values are respected
- ✅ All Datadog sites are defined
- ✅ Additional attributes storage

### SDK Interface Tests (IDatadogSdkTests)

Tests for `IDatadogSdk` implementation:
- ✅ Initialization (success and double-init protection)
- ✅ User management (set, clear)
- ✅ Attribute management (add, remove)
- ✅ Configuration application
- ✅ State validation

### Logger Interface Tests (IDatadogLoggerTests)

Tests for `IDatadogLogger` implementation:
- ✅ All log levels (Debug, Info, Warn, Error, Critical)
- ✅ Log message storage
- ✅ Attributes attachment

### RUM Interface Tests (IDatadogRumTests)

Tests for `IDatadogRum` implementation:
- ✅ View tracking (start, stop)
- ✅ Action recording
- ✅ Error tracking with exceptions
- ✅ Resource tracking (start, stop, error)
- ✅ Attributes propagation

### Trace Interface Tests (IDatadogTraceTests)

Tests for `IDatadogTrace` implementation:
- ✅ Span creation
- ✅ Tag management
- ✅ Error attachment
- ✅ Span lifecycle (finish, dispose)
- ✅ State validation (can't modify finished spans)

## Mock Implementations

The test project includes comprehensive mock implementations that mirror the expected behavior of platform-specific implementations:

### DatadogSdkMock
- Tracks initialization state
- Stores configuration
- Manages user information
- Handles global attributes

### DatadogLoggerMock
- Records all log entries
- Captures log levels and messages
- Stores attached attributes

### DatadogRumMock
- Tracks RUM events (views, actions, errors, resources)
- Records event metadata
- Validates event flow

### DatadogTraceMock & SpanMock
- Creates and tracks spans
- Manages span lifecycle
- Enforces span state rules (can't modify after finish)

## Test Results

Current status: **30 tests passing** ✅

```
Passed:    30
Failed:     0
Skipped:    0
Duration:  ~16ms
```

## Adding New Tests

When adding new functionality to the Datadog MAUI SDK:

1. **Add interface method** to the appropriate interface file
2. **Update mock implementation** in [DatadogMethodsMock.cs](DatadogMethodsMock.cs:1)
3. **Write tests** in the appropriate test file
4. **Run tests** to ensure they pass

Example:

```csharp
// 1. Add to interface
public interface IDatadogRum
{
    void AddFeatureFlag(string key, bool value);
}

// 2. Update mock
public class DatadogRumMock : IDatadogRum
{
    public Dictionary<string, bool> FeatureFlags { get; } = new();

    public void AddFeatureFlag(string key, bool value)
    {
        FeatureFlags[key] = value;
    }
}

// 3. Write test
[Fact]
public void AddFeatureFlag_ShouldStoreFlag()
{
    // Arrange
    var rum = new DatadogRumMock();

    // Act
    rum.AddFeatureFlag("new_checkout", true);

    // Assert
    Assert.True(rum.FeatureFlags["new_checkout"]);
}
```

## Continuous Integration

Tests run automatically on:
- Every pull request
- Every push to main/develop branches
- Manual workflow dispatch

See [.github/workflows/build.yml](../.github/workflows/build.yml:1) for CI configuration.

## Benefits of This Approach

### Development Velocity
- Test API contracts without native bindings
- Fast test execution (~16ms for 30 tests)
- No platform-specific dependencies

### Quality Assurance
- Validates interface design early
- Catches API inconsistencies
- Documents expected behavior

### Refactoring Safety
- Tests act as regression suite
- Safe to refactor implementations
- Ensures API stability

## Future Enhancements

- [ ] Integration tests with real platform implementations
- [ ] Performance benchmarks
- [ ] Stress tests (high volume events)
- [ ] Thread safety tests
- [ ] Memory leak detection

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [.NET Testing Guide](https://docs.microsoft.com/en-us/dotnet/core/testing/)
