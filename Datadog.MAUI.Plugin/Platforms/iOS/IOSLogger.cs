using Datadog.Maui.Logs;
using DatadogLogs;
using Foundation;

namespace Datadog.Maui.Platforms.iOS;

internal class IOSLogger : ILogger
{
    private readonly Logger _nativeLogger;

    public IOSLogger(string name)
    {
        Name = name;

        var builder = Logger.Create();
        builder.Set(name: name);
        _nativeLogger = builder.Build();
    }

    public string Name { get; }

    public void Debug(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Debug, message, error, attributes);
    }

    public void Info(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Info, message, error, attributes);
    }

    public void Notice(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Notice, message, error, attributes);
    }

    public void Warn(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Warn, message, error, attributes);
    }

    public void Error(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Error, message, error, attributes);
    }

    public void Critical(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        Log(Logs.LogLevel.Critical, message, error, attributes);
    }

    public void Log(Logs.LogLevel level, string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nativeLevel = MapLogLevel(level);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        if (error != null)
        {
            _nativeLogger.Log(
                level: nativeLevel,
                message: message,
                error: new DatadogInternal.DDError(error),
                attributes: nsAttributes
            );
        }
        else
        {
            _nativeLogger.Log(
                level: nativeLevel,
                message: message,
                attributes: nsAttributes
            );
        }
    }

    public void AddAttribute(string key, object value)
    {
        _nativeLogger.AddAttribute(
            forKey: key,
            value: NSObject.FromObject(value)
        );
    }

    public void RemoveAttribute(string key)
    {
        _nativeLogger.RemoveAttribute(forKey: key);
    }

    public void AddTag(string key, string value)
    {
        _nativeLogger.AddTag(
            withKey: key,
            value: value
        );
    }

    public void RemoveTag(string key)
    {
        _nativeLogger.RemoveTag(withKey: key);
    }

    private static LogLevel MapLogLevel(Logs.LogLevel level)
    {
        return level switch
        {
            Logs.LogLevel.Debug => LogLevel.Debug,
            Logs.LogLevel.Info => LogLevel.Info,
            Logs.LogLevel.Notice => LogLevel.Notice,
            Logs.LogLevel.Warn => LogLevel.Warn,
            Logs.LogLevel.Error => LogLevel.Error,
            Logs.LogLevel.Critical => LogLevel.Critical,
            _ => LogLevel.Info
        };
    }

    private static NSDictionary<NSString, NSObject>? ConvertAttributes(Dictionary<string, object> attributes)
    {
        if (attributes == null || attributes.Count == 0)
            return null;

        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();

        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
