using Datadog.Maui.Platforms.Android;

namespace Datadog.Maui.Logs;

public static partial class Logs
{
    static partial ILogger PlatformCreateLogger(string name)
    {
        return new AndroidLogger(name);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        Com.Datadog.Android.Log.Logs.AddAttribute(key, value);
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        Com.Datadog.Android.Log.Logs.RemoveAttribute(key);
    }

    static partial void PlatformAddTag(string key, string value)
    {
        Com.Datadog.Android.Log.Logs.AddTag(key, value);
    }

    static partial void PlatformRemoveTag(string key)
    {
        Com.Datadog.Android.Log.Logs.RemoveTag(key);
    }
}
