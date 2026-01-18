using Datadog.Maui.Logs;
using Datadog.iOS.DatadogLogs;
using Datadog.iOS.DatadogInternal;
using Foundation;

namespace Datadog.Maui.Platforms.iOS;

internal class IOSLogger : ILogger
{
    private readonly DDLogger _nativeLogger;

    public IOSLogger(string name)
    {
        Name = name;

        var config = new DDLoggerConfiguration(
            service: null,
            name: name,
            networkInfoEnabled: true,
            bundleWithRumEnabled: true,
            bundleWithTraceEnabled: true,
            remoteSampleRate: 100.0f,
            remoteLogThreshold: DDLogLevel.Debug,
            printLogsToConsole: false
        );

        _nativeLogger = DDLogger.CreateWith(config);
    }

    public string Name { get; }

    public void Debug(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Debug(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Debug(message, nsAttributes);
        }
    }

    public void Info(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Info(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Info(message, nsAttributes);
        }
    }

    public void Notice(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Notice(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Notice(message, nsAttributes);
        }
    }

    public void Warn(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Warn(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Warn(message, nsAttributes);
        }
    }

    public void Error(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Error(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Error(message, nsAttributes);
        }
    }

    public void Critical(string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();
        if (error != null)
        {
            var nsError = NSError.FromDomain(new NSString("Exception"), 0, NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()), NSError.LocalizedDescriptionKey));
            _nativeLogger.Critical(message, nsError, nsAttributes);
        }
        else
        {
            _nativeLogger.Critical(message, nsAttributes);
        }
    }

    public void Log(Logs.LogLevel level, string message, Exception? error = null, Dictionary<string, object>? attributes = null)
    {
        switch (level)
        {
            case Logs.LogLevel.Debug:
                Debug(message, error, attributes);
                break;
            case Logs.LogLevel.Info:
                Info(message, error, attributes);
                break;
            case Logs.LogLevel.Notice:
                Notice(message, error, attributes);
                break;
            case Logs.LogLevel.Warn:
                Warn(message, error, attributes);
                break;
            case Logs.LogLevel.Error:
                Error(message, error, attributes);
                break;
            case Logs.LogLevel.Critical:
                Critical(message, error, attributes);
                break;
        }
    }

    public void AddAttribute(string key, object value)
    {
        _nativeLogger.AddAttributeForKey(key, NSObject.FromObject(value));
    }

    public void RemoveAttribute(string key)
    {
        _nativeLogger.RemoveAttributeForKey(key);
    }

    public void AddTag(string key, string value)
    {
        _nativeLogger.AddTagWithKey(key, value);
    }

    public void RemoveTag(string key)
    {
        _nativeLogger.RemoveTagWithKey(key);
    }

    private static NSDictionary<NSString, NSObject> ConvertAttributes(Dictionary<string, object> attributes)
    {
        if (attributes == null || attributes.Count == 0)
            return new NSDictionary<NSString, NSObject>();

        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();

        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
