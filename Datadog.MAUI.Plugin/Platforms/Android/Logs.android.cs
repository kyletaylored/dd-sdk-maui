using Datadog.Maui.Platforms.Android;

namespace Datadog.Maui.Logs;

public static partial class Logs
{
    private static partial ILogger PlatformCreateLogger(string name)
    {
        return new AndroidLogger(name);
    }

    private static partial void PlatformAddAttribute(string key, object value)
    {
        var javaValue = value as Java.Lang.Object ?? new Java.Lang.String(value?.ToString() ?? "");
        global::Datadog.Android.Logs.Logs.AddAttribute(key, javaValue);
    }

    private static partial void PlatformRemoveAttribute(string key)
    {
        global::Datadog.Android.Logs.Logs.RemoveAttribute(key);
    }

    private static partial void PlatformAddTag(string key, string value)
    {
        // Note: Global tag support not available in Android SDK v3.x Logs API
        // Tags should be added at the logger level
    }

    private static partial void PlatformRemoveTag(string key)
    {
        // Note: Global tag support not available in Android SDK v3.x Logs API
        // Tags should be removed at the logger level
    }
}
