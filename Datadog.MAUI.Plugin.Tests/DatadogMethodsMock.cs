using Datadog.MAUI;

namespace Datadog.MAUI.Tests;

/// <summary>
/// Mock implementation of Datadog SDK interfaces for testing.
/// This allows testing the API surface without requiring native bindings.
/// </summary>
public class DatadogSdkMock : IDatadogSdk
{
    public bool IsInitialized { get; private set; }
    public DatadogConfiguration? Configuration { get; private set; }
    public Dictionary<string, object> Attributes { get; } = new();
    public string? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }

    public void Initialize(DatadogConfiguration configuration)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Already initialized");
        }

        Configuration = configuration;
        IsInitialized = true;

        // Apply initial attributes
        if (configuration.AdditionalAttributes != null)
        {
            foreach (var kvp in configuration.AdditionalAttributes)
            {
                Attributes[kvp.Key] = kvp.Value;
            }
        }
    }

    public void SetUser(string id, string? name = null, string? email = null)
    {
        EnsureInitialized();
        UserId = id;
        UserName = name;
        UserEmail = email;
    }

    public void ClearUser()
    {
        EnsureInitialized();
        UserId = null;
        UserName = null;
        UserEmail = null;
    }

    public void AddAttribute(string key, object value)
    {
        EnsureInitialized();
        Attributes[key] = value;
    }

    public void RemoveAttribute(string key)
    {
        EnsureInitialized();
        Attributes.Remove(key);
    }

    private void EnsureInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Not initialized");
        }
    }

    public void Reset()
    {
        IsInitialized = false;
        Configuration = null;
        Attributes.Clear();
        UserId = null;
        UserName = null;
        UserEmail = null;
    }
}

/// <summary>
/// Mock implementation of Datadog Logger for testing.
/// </summary>
public class DatadogLoggerMock : IDatadogLogger
{
    public List<LogEntry> Logs { get; } = new();

    public void Debug(string message, Dictionary<string, object>? attributes = null)
    {
        Logs.Add(new LogEntry("Debug", message, attributes));
    }

    public void Info(string message, Dictionary<string, object>? attributes = null)
    {
        Logs.Add(new LogEntry("Info", message, attributes));
    }

    public void Warn(string message, Dictionary<string, object>? attributes = null)
    {
        Logs.Add(new LogEntry("Warn", message, attributes));
    }

    public void Error(string message, Dictionary<string, object>? attributes = null)
    {
        Logs.Add(new LogEntry("Error", message, attributes));
    }

    public void Critical(string message, Dictionary<string, object>? attributes = null)
    {
        Logs.Add(new LogEntry("Critical", message, attributes));
    }

    public void Clear()
    {
        Logs.Clear();
    }

    public record LogEntry(string Level, string Message, Dictionary<string, object>? Attributes);
}

/// <summary>
/// Mock implementation of Datadog RUM for testing.
/// </summary>
public class DatadogRumMock : IDatadogRum
{
    public List<RumEvent> Events { get; } = new();

    public void StartView(string key, string name, Dictionary<string, object>? attributes = null)
    {
        Events.Add(new RumEvent("ViewStart", key, name, attributes));
    }

    public void StopView(string key, Dictionary<string, object>? attributes = null)
    {
        Events.Add(new RumEvent("ViewStop", key, null, attributes));
    }

    public void AddAction(string type, string name, Dictionary<string, object>? attributes = null)
    {
        Events.Add(new RumEvent("Action", type, name, attributes));
    }

    public void AddError(string message, string source, Exception? exception = null, Dictionary<string, object>? attributes = null)
    {
        var attrs = attributes ?? new Dictionary<string, object>();
        if (exception != null)
        {
            attrs["exception"] = exception;
        }
        Events.Add(new RumEvent("Error", source, message, attrs));
    }

    public void StartResource(string key, string url, string method)
    {
        var attrs = new Dictionary<string, object>
        {
            { "url", url },
            { "method", method }
        };
        Events.Add(new RumEvent("ResourceStart", key, null, attrs));
    }

    public void StopResource(string key, int statusCode, long size, Dictionary<string, object>? attributes = null)
    {
        var attrs = attributes ?? new Dictionary<string, object>();
        attrs["status_code"] = statusCode;
        attrs["size"] = size;
        Events.Add(new RumEvent("ResourceStop", key, null, attrs));
    }

    public void StopResourceWithError(string key, string message, string source)
    {
        var attrs = new Dictionary<string, object>
        {
            { "error_message", message },
            { "error_source", source }
        };
        Events.Add(new RumEvent("ResourceError", key, null, attrs));
    }

    public void Clear()
    {
        Events.Clear();
    }

    public record RumEvent(string Type, string Key, string? Value, Dictionary<string, object>? Attributes);
}

/// <summary>
/// Mock implementation of Datadog Trace for testing.
/// </summary>
public class DatadogTraceMock : IDatadogTrace
{
    public List<SpanMock> Spans { get; } = new();

    public IDatadogSpan StartSpan(string operationName, Dictionary<string, object>? tags = null)
    {
        var span = new SpanMock(operationName, tags);
        Spans.Add(span);
        return span;
    }

    public void Clear()
    {
        Spans.Clear();
    }
}

/// <summary>
/// Mock implementation of a Datadog Span for testing.
/// </summary>
public class SpanMock : IDatadogSpan
{
    public string OperationName { get; }
    public Dictionary<string, string> Tags { get; } = new();
    public Exception? Error { get; private set; }
    public bool IsFinished { get; private set; }
    public DateTime StartTime { get; }
    public DateTime? FinishTime { get; private set; }

    public SpanMock(string operationName, Dictionary<string, object>? tags = null)
    {
        OperationName = operationName;
        StartTime = DateTime.UtcNow;

        if (tags != null)
        {
            foreach (var kvp in tags)
            {
                Tags[kvp.Key] = kvp.Value?.ToString() ?? "";
            }
        }
    }

    public void SetTag(string key, string value)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Cannot set tag on finished span");
        }
        Tags[key] = value;
    }

    public void SetError(Exception exception)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Cannot set error on finished span");
        }
        Error = exception;
    }

    public void Finish()
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Span already finished");
        }
        IsFinished = true;
        FinishTime = DateTime.UtcNow;
    }

    public void Dispose()
    {
        if (!IsFinished)
        {
            Finish();
        }
    }
}
