using Datadog.Maui.Logs;
using Com.Datadog.Android.Log;

namespace Datadog.Maui.Platforms.Android;

internal class AndroidLogger : ILogger
{
    private readonly Logger _nativeLogger;

    public AndroidLogger(string name)
    {
        Name = name;
        _nativeLogger = Logger.Builder
            .Create()
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
        _nativeLogger.AddAttribute(key, value);
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
            Logs.LogLevel.Debug => Com.Datadog.Android.Log.LogLevel.Debug,
            Logs.LogLevel.Info => Com.Datadog.Android.Log.LogLevel.Info,
            Logs.LogLevel.Notice => Com.Datadog.Android.Log.LogLevel.Info,
            Logs.LogLevel.Warn => Com.Datadog.Android.Log.LogLevel.Warn,
            Logs.LogLevel.Error => Com.Datadog.Android.Log.LogLevel.Error,
            Logs.LogLevel.Critical => Com.Datadog.Android.Log.LogLevel.Error,
            Logs.LogLevel.Alert => Com.Datadog.Android.Log.LogLevel.Error,
            Logs.LogLevel.Emergency => Com.Datadog.Android.Log.LogLevel.Error,
            _ => Com.Datadog.Android.Log.LogLevel.Info
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
