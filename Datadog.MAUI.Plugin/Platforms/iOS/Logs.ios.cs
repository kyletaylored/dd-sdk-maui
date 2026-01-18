using Datadog.Maui.Platforms.iOS;
using DatadogLogs;
using Foundation;

namespace Datadog.Maui.Logs;

public static partial class Logs
{
    static partial ILogger PlatformCreateLogger(string name)
    {
        return new IOSLogger(name);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        DatadogLogs.Logs.AddAttribute(
            forKey: key,
            value: NSObject.FromObject(value)
        );
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        DatadogLogs.Logs.RemoveAttribute(forKey: key);
    }

    static partial void PlatformAddTag(string key, string value)
    {
        DatadogLogs.Logs.AddTag(
            withKey: key,
            value: value
        );
    }

    static partial void PlatformRemoveTag(string key)
    {
        DatadogLogs.Logs.RemoveTag(withKey: key);
    }
}
