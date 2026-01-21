using Datadog.Maui.Logs;
using Com.Datadog.Android.Log;

namespace Datadog.Maui.Platforms.Android;

internal class AndroidLogger : ILogger
{
    private readonly Logger _nativeLogger;

    public AndroidLogger(string name)
    {
        Name = name;
        _nativeLogger = new Logger.Builder()
            .SetName(name)
            .Build();
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

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            _nativeLogger.Log(nativeLevel, message, error?.ToJavaThrowable(), javaAttributes);
        }
        else
        {
            _nativeLogger.Log(nativeLevel, message, error?.ToJavaThrowable(), null);
        }
    }

    public void AddAttribute(string key, object value)
    {
        var javaValue = value as Java.Lang.Object ?? new Java.Lang.String(value?.ToString() ?? "");
        _nativeLogger.AddAttribute(key, javaValue);
    }

    public void RemoveAttribute(string key)
    {
        _nativeLogger.RemoveAttribute(key);
    }

    public void AddTag(string key, string value)
    {
        _nativeLogger.AddTag(key, value);
    }

    public void RemoveTag(string key)
    {
        _nativeLogger.RemoveTag(key);
    }

    private static int MapLogLevel(Logs.LogLevel level)
    {
        return level switch
        {
            Logs.LogLevel.Debug => (int)global::Android.Util.LogPriority.Debug,
            Logs.LogLevel.Info => (int)global::Android.Util.LogPriority.Info,
            Logs.LogLevel.Notice => (int)global::Android.Util.LogPriority.Info,
            Logs.LogLevel.Warn => (int)global::Android.Util.LogPriority.Warn,
            Logs.LogLevel.Error => (int)global::Android.Util.LogPriority.Error,
            Logs.LogLevel.Critical => (int)global::Android.Util.LogPriority.Error,
            Logs.LogLevel.Alert => (int)global::Android.Util.LogPriority.Error,
            Logs.LogLevel.Emergency => (int)global::Android.Util.LogPriority.Error,
            _ => (int)global::Android.Util.LogPriority.Info
        };
    }
}

internal static class ExceptionExtensions
{
    public static Java.Lang.Throwable? ToJavaThrowable(this Exception? exception)
    {
        if (exception == null)
            return null;

        return new Java.Lang.Exception(exception.Message, exception.InnerException?.ToJavaThrowable());
    }
}
