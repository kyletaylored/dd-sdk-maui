---
layout: default
title: API Reference
nav_order: 3
permalink: /api-reference
---

# API Reference

Complete API reference for the Datadog MAUI SDK.

---

## Table of Contents

- [Core API](#core-api)
- [Logs API](#logs-api)
- [RUM API](#rum-api)
- [Tracing API](#tracing-api)
- [Configuration](#configuration)
- [Enumerations](#enumerations)
- [Interfaces](#interfaces)

---

## Core API

### Datadog

Static class providing the main SDK interface.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsInitialized` | `bool` | Gets whether the SDK has been initialized |
| `Configuration` | `DatadogConfiguration?` | Gets the current configuration |

#### Methods

##### Initialize

```csharp
void Initialize(DatadogConfiguration configuration)
```

Initializes the Datadog SDK with the specified configuration.

**Parameters:**
- `configuration` - The Datadog configuration

**Example:**
```csharp
var config = new DatadogConfiguration.Builder("YOUR_CLIENT_TOKEN")
    .SetEnvironment("production")
    .SetServiceName("my-app")
    .Build();

Datadog.Initialize(config);
```

##### SetUser

```csharp
void SetUser(UserInfo userInfo)
```

Sets user information for tracking.

**Parameters:**
- `userInfo` - User information

**Example:**
```csharp
Datadog.SetUser(new UserInfo
{
    Id = "12345",
    Name = "John Doe",
    Email = "john@example.com"
});
```

##### ClearUser

```csharp
void ClearUser()
```

Clears all user information.

**Example:**
```csharp
Datadog.ClearUser();
```

##### SetTags

```csharp
void SetTags(Dictionary<string, string> tags)
```

Sets global tags for all events.

**Parameters:**
- `tags` - Dictionary of tag key-value pairs

**Example:**
```csharp
Datadog.SetTags(new Dictionary<string, string>
{
    { "version", "1.0.0" },
    { "env", "production" }
});
```

##### SetTrackingConsent

```csharp
void SetTrackingConsent(TrackingConsent consent)
```

Updates the tracking consent status.

**Parameters:**
- `consent` - The tracking consent status

**Example:**
```csharp
Datadog.SetTrackingConsent(TrackingConsent.Granted);
```

---

### DatadogSdk

Static class providing access to SDK instances.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Instance` | `IDatadogSdk` | Gets the Datadog SDK instance |
| `Logger` | `IDatadogLogger` | Gets the Datadog logger instance |
| `Rum` | `IDatadogRum` | Gets the Datadog RUM instance |
| `Trace` | `IDatadogTrace` | Gets the Datadog Trace instance |

**Example:**
```csharp
var logger = DatadogSdk.Logger;
logger.Info("Application started");
```

---

## Logs API

### Logs

Static class for creating and managing loggers.

**Namespace:** `Datadog.Maui`

#### Methods

##### CreateLogger

```csharp
ILogger CreateLogger(string name)
```

Creates or retrieves a logger with the specified name.

**Parameters:**
- `name` - The logger name

**Returns:** An `ILogger` instance

**Example:**
```csharp
var logger = Logs.CreateLogger("my-logger");
logger.Info("Hello, Datadog!");
```

##### AddAttribute

```csharp
void AddAttribute(string key, object value)
```

Adds a global attribute to all logs.

**Parameters:**
- `key` - Attribute key
- `value` - Attribute value

**Example:**
```csharp
Logs.AddAttribute("app_version", "1.0.0");
```

##### RemoveAttribute

```csharp
void RemoveAttribute(string key)
```

Removes a global attribute from all logs.

**Parameters:**
- `key` - Attribute key to remove

##### AddTag

```csharp
void AddTag(string key, string value)
```

Adds a global tag to all logs.

**Parameters:**
- `key` - Tag key
- `value` - Tag value

**Example:**
```csharp
Logs.AddTag("env", "production");
```

##### RemoveTag

```csharp
void RemoveTag(string key)
```

Removes a global tag from all logs.

**Parameters:**
- `key` - Tag key to remove

---

### ILogger

Interface representing a logger instance.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Gets the logger name |

#### Methods

##### Debug

```csharp
void Debug(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs a debug message.

**Parameters:**
- `message` - Log message
- `error` - Optional exception
- `attributes` - Optional attributes

##### Info

```csharp
void Info(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs an info message.

##### Notice

```csharp
void Notice(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs a notice message.

##### Warn

```csharp
void Warn(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs a warning message.

##### Error

```csharp
void Error(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs an error message.

##### Critical

```csharp
void Critical(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs a critical message.

##### Log

```csharp
void Log(LogLevel level, string message, Exception? error = null, Dictionary<string, object>? attributes = null)
```

Logs a message with the specified level.

**Parameters:**
- `level` - Log level
- `message` - Log message
- `error` - Optional exception
- `attributes` - Optional attributes

##### AddAttribute

```csharp
void AddAttribute(string key, object value)
```

Adds an attribute to this logger.

##### RemoveAttribute

```csharp
void RemoveAttribute(string key)
```

Removes an attribute from this logger.

##### AddTag

```csharp
void AddTag(string key, string value)
```

Adds a tag to this logger.

##### RemoveTag

```csharp
void RemoveTag(string key)
```

Removes a tag from this logger.

**Example:**
```csharp
var logger = Logs.CreateLogger("auth");
logger.AddAttribute("component", "authentication");
logger.Info("User logged in", new Dictionary<string, object>
{
    { "user_id", "12345" },
    { "method", "oauth" }
});
```

---

## RUM API

### Rum

Static class for Real User Monitoring.

**Namespace:** `Datadog.Maui`

#### Methods

##### StartView

```csharp
void StartView(string key, string? name = null, Dictionary<string, object>? attributes = null)
```

Starts tracking a view.

**Parameters:**
- `key` - Unique view identifier
- `name` - View name (defaults to key)
- `attributes` - Optional attributes

**Example:**
```csharp
Rum.StartView("home_screen", "Home");
```

##### StopView

```csharp
void StopView(string key, Dictionary<string, object>? attributes = null)
```

Stops tracking a view.

**Parameters:**
- `key` - View identifier
- `attributes` - Optional attributes

**Example:**
```csharp
Rum.StopView("home_screen");
```

##### AddAction

```csharp
void AddAction(RumActionType type, string name, Dictionary<string, object>? attributes = null)
```

Tracks a user action.

**Parameters:**
- `type` - Action type
- `name` - Action name
- `attributes` - Optional attributes

**Example:**
```csharp
Rum.AddAction(RumActionType.Tap, "login_button");
```

##### StartResource

```csharp
void StartResource(string key, string method, string url, Dictionary<string, object>? attributes = null)
```

Starts tracking a resource loading.

**Parameters:**
- `key` - Unique resource identifier
- `method` - HTTP method
- `url` - Resource URL
- `attributes` - Optional attributes

**Example:**
```csharp
Rum.StartResource("api_users", "GET", "https://api.example.com/users");
```

##### StopResource

```csharp
void StopResource(string key, int? statusCode = null, long? size = null,
    RumResourceKind kind = RumResourceKind.Native, Dictionary<string, object>? attributes = null)
```

Stops tracking a resource loading.

**Parameters:**
- `key` - Resource identifier
- `statusCode` - HTTP status code
- `size` - Resource size in bytes
- `kind` - Resource kind
- `attributes` - Optional attributes

**Example:**
```csharp
Rum.StopResource("api_users", statusCode: 200, size: 1024, kind: RumResourceKind.Xhr);
```

##### StopResourceWithError

```csharp
void StopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes = null)
```

Stops resource tracking with an error.

**Parameters:**
- `key` - Resource identifier
- `error` - Exception that occurred
- `attributes` - Optional attributes

**Example:**
```csharp
try
{
    await LoadData();
}
catch (Exception ex)
{
    Rum.StopResourceWithError("api_users", ex);
}
```

##### AddError

```csharp
void AddError(string message, RumErrorSource source = RumErrorSource.Source,
    Exception? exception = null, Dictionary<string, object>? attributes = null)

void AddError(Exception exception, RumErrorSource source = RumErrorSource.Source,
    Dictionary<string, object>? attributes = null)
```

Tracks an error.

**Parameters:**
- `message` - Error message
- `exception` - Exception object
- `source` - Error source
- `attributes` - Optional attributes

**Example:**
```csharp
try
{
    await LoadData();
}
catch (Exception ex)
{
    Rum.AddError(ex, RumErrorSource.Network);
}
```

##### AddTiming

```csharp
void AddTiming(string name)
```

Adds a custom timing to the current view.

**Parameters:**
- `name` - Timing name

**Example:**
```csharp
Rum.AddTiming("data_loaded");
```

##### AddAttribute

```csharp
void AddAttribute(string key, object value)
```

Adds a global attribute to all RUM events.

**Parameters:**
- `key` - Attribute key
- `value` - Attribute value

**Example:**
```csharp
Rum.AddAttribute("user_tier", "premium");
```

##### RemoveAttribute

```csharp
void RemoveAttribute(string key)
```

Removes a global attribute from all RUM events.

##### StartSession

```csharp
void StartSession()
```

Starts a new RUM session.

##### StopSession

```csharp
void StopSession()
```

Stops the current RUM session.

---

## Tracing API

### Tracer

Static class for distributed tracing.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ActiveSpan` | `ISpan?` | Gets the currently active span |

#### Methods

##### StartSpan

```csharp
ISpan StartSpan(string operationName, ISpan? parent = null, DateTimeOffset? startTime = null)
```

Starts a new span.

**Parameters:**
- `operationName` - Operation name
- `parent` - Parent span (optional)
- `startTime` - Start time (optional)

**Returns:** An `ISpan` instance

**Example:**
```csharp
using (var span = Tracer.StartSpan("fetch_data"))
{
    await FetchData();
}
```

##### Inject

```csharp
void Inject(IDictionary<string, string> headers, ISpan? span = null)
void Inject(System.Net.Http.Headers.HttpRequestHeaders headers, ISpan? span = null)
```

Injects trace context into headers.

**Parameters:**
- `headers` - Headers dictionary or HttpRequestHeaders
- `span` - Span to inject (defaults to active span)

**Example:**
```csharp
var request = new HttpRequestMessage(HttpMethod.Get, url);
Tracer.Inject(request.Headers);
```

##### Extract

```csharp
ISpan? Extract(IDictionary<string, string> headers)
```

Extracts trace context from headers.

**Parameters:**
- `headers` - Headers dictionary

**Returns:** Parent span if found, null otherwise

**Example:**
```csharp
var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.First());
var parentSpan = Tracer.Extract(headers);
```

---

### ISpan

Interface representing a trace span.

**Namespace:** `Datadog.Maui`

**Implements:** `IDisposable`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `SpanId` | `string` | Gets the span ID |
| `TraceId` | `string` | Gets the trace ID |

#### Methods

##### SetTag

```csharp
void SetTag(string key, string value)
void SetTag(string key, object value)
```

Sets a tag on the span.

**Parameters:**
- `key` - Tag key
- `value` - Tag value

**Example:**
```csharp
span.SetTag("user_id", "12345");
span.SetTag("item_count", 42);
```

##### SetError

```csharp
void SetError(Exception exception)
void SetError(string message)
```

Sets an error on the span.

**Parameters:**
- `exception` - Exception object
- `message` - Error message

**Example:**
```csharp
try
{
    await PerformOperation();
}
catch (Exception ex)
{
    span.SetError(ex);
}
```

##### AddEvent

```csharp
void AddEvent(string name, Dictionary<string, object>? attributes = null)
```

Adds an event to the span.

**Parameters:**
- `name` - Event name
- `attributes` - Optional attributes

**Example:**
```csharp
span.AddEvent("cache_hit", new Dictionary<string, object>
{
    { "cache_key", "user_12345" }
});
```

##### Finish

```csharp
void Finish()
```

Finishes the span. Called automatically when disposed.

---

## Configuration

### DatadogConfiguration

Main configuration class for the SDK.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `ClientToken` | `string` | Yes | Client token for authentication |
| `Environment` | `string` | Yes | Environment name |
| `ServiceName` | `string` | Yes | Service name |
| `Site` | `DatadogSite` | No | Datadog site (default: US1) |
| `TrackingConsent` | `TrackingConsent` | No | Tracking consent (default: Granted) |
| `GlobalTags` | `Dictionary<string, string>` | No | Global tags |
| `VerboseLogging` | `bool` | No | Enable verbose logging |
| `FirstPartyHosts` | `string[]` | No | First-party hosts for tracing |
| `Rum` | `RumConfiguration?` | No | RUM configuration |
| `Logs` | `LogsConfiguration?` | No | Logs configuration |
| `Tracing` | `TracingConfiguration?` | No | Tracing configuration |

---

### DatadogConfiguration.Builder

Builder class for creating DatadogConfiguration.

**Namespace:** `Datadog.Maui`

#### Constructor

```csharp
Builder(string clientToken)
```

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `SetEnvironment(string)` | `Builder` | Sets environment |
| `SetServiceName(string)` | `Builder` | Sets service name |
| `SetSite(DatadogSite)` | `Builder` | Sets Datadog site |
| `SetTrackingConsent(TrackingConsent)` | `Builder` | Sets tracking consent |
| `AddGlobalTag(string, string)` | `Builder` | Adds a global tag |
| `EnableVerboseLogging()` | `Builder` | Enables verbose logging |
| `SetFirstPartyHosts(params string[])` | `Builder` | Sets first-party hosts |
| `EnableRum(Action<RumConfiguration.Builder>)` | `Builder` | Enables RUM |
| `EnableLogs(Action<LogsConfiguration.Builder>)` | `Builder` | Enables Logs |
| `EnableTracing(Action<TracingConfiguration.Builder>)` | `Builder` | Enables Tracing |
| `Build()` | `DatadogConfiguration` | Builds configuration |

**Example:**
```csharp
var config = new DatadogConfiguration.Builder("YOUR_CLIENT_TOKEN")
    .SetEnvironment("production")
    .SetServiceName("my-app")
    .SetSite(DatadogSite.US1)
    .AddGlobalTag("version", "1.0.0")
    .EnableRum(rum => rum.SetApplicationId("YOUR_APP_ID"))
    .Build();
```

---

### RumConfiguration

RUM-specific configuration.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApplicationId` | `string` | Required | RUM application ID |
| `SessionSampleRate` | `int` | 100 | Session sampling rate (0-100) |
| `TelemetrySampleRate` | `int` | 20 | Telemetry sampling rate (0-100) |
| `TrackViewsAutomatically` | `bool` | true | Auto-track views |
| `TrackUserInteractions` | `bool` | true | Auto-track interactions |
| `TrackResources` | `bool` | true | Auto-track resources |
| `TrackErrors` | `bool` | true | Auto-track errors |
| `VitalsUpdateFrequency` | `VitalsUpdateFrequency` | Average | Vitals update frequency |

---

### LogsConfiguration

Logs-specific configuration.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SampleRate` | `int` | 100 | Sampling rate (0-100) |
| `NetworkInfoEnabled` | `bool` | true | Include network info |
| `BundleWithRum` | `bool` | true | Bundle with RUM sessions |

---

### TracingConfiguration

Tracing-specific configuration.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SampleRate` | `int` | 100 | Sampling rate (0-100) |
| `TraceIdGenerationEnabled` | `bool` | true | Enable trace ID generation |
| `FirstPartyHosts` | `string[]` | Empty | First-party hosts |

---

## Enumerations

### DatadogSite

Datadog site/region enumeration.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `US1` | US1 site (default) |
| `US3` | US3 site |
| `US5` | US5 site |
| `EU1` | EU1 site |
| `US1_FED` | US Government site |
| `AP1` | Asia Pacific site |

---

### TrackingConsent

User tracking consent status.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `Granted` | User has granted consent |
| `NotGranted` | User has not granted consent |
| `Pending` | Consent is pending |

---

### LogLevel

Log level enumeration.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `Debug` | Debug level |
| `Info` | Info level |
| `Notice` | Notice level |
| `Warn` | Warning level |
| `Error` | Error level |
| `Critical` | Critical level |
| `Alert` | Alert level |
| `Emergency` | Emergency level |

---

### RumActionType

RUM action type enumeration.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `Tap` | Tap action |
| `Scroll` | Scroll action |
| `Swipe` | Swipe action |
| `Click` | Click action |
| `Custom` | Custom action |

---

### RumErrorSource

RUM error source enumeration.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `Source` | Error from source code |
| `Network` | Network error |
| `WebView` | WebView error |
| `Custom` | Custom error |

---

### RumResourceKind

RUM resource kind enumeration.

**Namespace:** `Datadog.Maui`

| Value | Description |
|-------|-------------|
| `Image` | Image resource |
| `Xhr` | XHR/Fetch request |
| `Beacon` | Beacon |
| `Css` | CSS resource |
| `Document` | Document |
| `Font` | Font resource |
| `Js` | JavaScript resource |
| `Media` | Media (audio/video) |
| `Native` | Native resource/API call (default) |
| `Other` | Other resource type |

---

### VitalsUpdateFrequency

Vitals update frequency enumeration.

**Namespace:** `Datadog.Maui`

| Value | Update Interval | Description |
|-------|-----------------|-------------|
| `Frequent` | 500ms | Frequent updates |
| `Average` | 1s | Average updates (default) |
| `Rare` | 2s | Rare updates |
| `Never` | N/A | No vitals tracking |

---

## Interfaces

### IDatadogSdk

Main SDK interface.

**Namespace:** `Datadog.Maui`

```csharp
public interface IDatadogSdk
{
    void Initialize(DatadogConfiguration configuration);
    void SetUser(string id, string? name = null, string? email = null);
    void ClearUser();
    void AddAttribute(string key, object value);
    void RemoveAttribute(string key);
}
```

---

### IDatadogLogger

Logger interface.

**Namespace:** `Datadog.Maui`

```csharp
public interface IDatadogLogger
{
    void Debug(string message, Dictionary<string, object>? attributes = null);
    void Info(string message, Dictionary<string, object>? attributes = null);
    void Warn(string message, Dictionary<string, object>? attributes = null);
    void Error(string message, Dictionary<string, object>? attributes = null);
    void Critical(string message, Dictionary<string, object>? attributes = null);
}
```

---

### IDatadogRum

RUM interface.

**Namespace:** `Datadog.Maui`

```csharp
public interface IDatadogRum
{
    void StartView(string key, string name, Dictionary<string, object>? attributes = null);
    void StopView(string key, Dictionary<string, object>? attributes = null);
    void AddAction(string type, string name, Dictionary<string, object>? attributes = null);
    void AddError(string message, string source, Exception? exception = null, Dictionary<string, object>? attributes = null);
    void StartResource(string key, string url, string method);
    void StopResource(string key, int statusCode, long size, Dictionary<string, object>? attributes = null);
    void StopResourceWithError(string key, string message, string source);
}
```

---

### IDatadogTrace

Tracing interface.

**Namespace:** `Datadog.Maui`

```csharp
public interface IDatadogTrace
{
    IDatadogSpan StartSpan(string operationName, Dictionary<string, object>? tags = null);
}
```

---

### IDatadogSpan

Span interface.

**Namespace:** `Datadog.Maui`

**Implements:** `IDisposable`

```csharp
public interface IDatadogSpan : IDisposable
{
    void SetTag(string key, string value);
    void SetError(Exception exception);
    void Finish();
}
```

---

## MAUI Extensions

### MauiAppBuilderExtensions

Extension methods for MauiAppBuilder.

**Namespace:** `Datadog.Maui`

#### Methods

##### UseDatadog

```csharp
MauiAppBuilder UseDatadog(this MauiAppBuilder builder, Action<DatadogConfigurationBuilder> configure)
```

Adds Datadog to the MAUI application.

**Parameters:**
- `builder` - MauiAppBuilder instance
- `configure` - Configuration action

**Returns:** MauiAppBuilder for chaining

**Example:**
```csharp
builder.UseDatadog(config =>
{
    config.ClientToken = "YOUR_CLIENT_TOKEN";
    config.Environment = "production";
    config.ServiceName = "my-app";

    config.EnableRum(rum =>
    {
        rum.SetApplicationId("YOUR_APP_ID");
    });
});
```

---

## Support Classes

### UserInfo

User information class.

**Namespace:** `Datadog.Maui`

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string?` | Unique user identifier |
| `Name` | `string?` | User's name |
| `Email` | `string?` | User's email |
| `ExtraInfo` | `Dictionary<string, object>?` | Additional user attributes |

**Example:**
```csharp
var userInfo = new UserInfo
{
    Id = "12345",
    Name = "John Doe",
    Email = "john@example.com",
    ExtraInfo = new Dictionary<string, object>
    {
        { "plan", "premium" },
        { "signup_date", "2024-01-15" }
    }
};

Datadog.SetUser(userInfo);
```

---

## See Also

- [Using the SDK](getting-started/using-the-sdk) - Complete usage guide
- [Code Examples](examples) - Practical examples
- [Datadog Documentation](https://docs.datadoghq.com/) - Official Datadog docs
