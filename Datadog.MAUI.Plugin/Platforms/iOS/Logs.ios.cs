using Datadog.Maui.Platforms.iOS;
using Datadog.iOS.Logs;
using Foundation;

namespace Datadog.Maui.Logs;

public static partial class Logs
{
    private static partial ILogger PlatformCreateLogger(string name)
    {
        return new IOSLogger(name);
    }

    private static partial void PlatformAddAttribute(string key, object value)
    {
        DDLogs.AddAttributeForKey(key, NSObject.FromObject(value));
    }

    private static partial void PlatformRemoveAttribute(string key)
    {
        DDLogs.RemoveAttributeForKey(key);
    }

    private static partial void PlatformAddTag(string key, string value)
    {
        // DDLogs doesn't support global tags - tags are per-logger only
        // We could maintain a list and apply to new loggers, but for now we'll no-op
    }

    private static partial void PlatformRemoveTag(string key)
    {
        // DDLogs doesn't support global tags - tags are per-logger only
    }
}
