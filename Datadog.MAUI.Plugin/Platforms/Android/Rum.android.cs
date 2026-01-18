using Com.Datadog.Android.Rum;
using Datadog.Maui.Platforms.Android;

namespace Datadog.Maui.Rum;

public static partial class Rum
{
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.StartView(key, name, javaAttributes);
        }
        else
        {
            rumMonitor.StartView(key, name, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStopView(string key, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.StopView(key, javaAttributes);
        }
        else
        {
            rumMonitor.StopView(key, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformAddAction(RumActionType type, string name, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var actionType = MapActionType(type);

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.AddAction(actionType, name, javaAttributes);
        }
        else
        {
            rumMonitor.AddAction(actionType, name, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStartResource(string key, string method, string url, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.StartResource(key, method, url, javaAttributes);
        }
        else
        {
            rumMonitor.StartResource(key, method, url, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStopResource(string key, int? statusCode, long? size, RumResourceKind kind, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var resourceKind = MapResourceKind(kind);

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.StopResource(key, statusCode, size, resourceKind, javaAttributes);
        }
        else
        {
            rumMonitor.StopResource(key, statusCode, size, resourceKind, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var throwable = error.ToJavaThrowable();

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.StopResourceWithError(key, null, error.Message, RumErrorSource.Network, throwable, javaAttributes);
        }
        else
        {
            rumMonitor.StopResourceWithError(key, null, error.Message, RumErrorSource.Network, throwable, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformAddError(string message, RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var errorSource = MapErrorSource(source);
        var throwable = exception?.ToJavaThrowable();

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => (Java.Lang.Object)kvp.Value);
            rumMonitor.AddError(message, errorSource, throwable, javaAttributes);
        }
        else
        {
            rumMonitor.AddError(message, errorSource, throwable, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformAddTiming(string name)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        rumMonitor.AddTiming(name);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        rumMonitor.AddAttribute(key, value);
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        rumMonitor.RemoveAttribute(key);
    }

    static partial void PlatformStartSession()
    {
        // Android doesn't have explicit session start - sessions are automatic
    }

    static partial void PlatformStopSession()
    {
        var rumMonitor = GlobalRumMonitor.Get();
        rumMonitor.StopSession();
    }

    private static RumActionType MapActionType(Maui.Rum.RumActionType type)
    {
        return type switch
        {
            Maui.Rum.RumActionType.Tap => Com.Datadog.Android.Rum.RumActionType.Tap,
            Maui.Rum.RumActionType.Scroll => Com.Datadog.Android.Rum.RumActionType.Scroll,
            Maui.Rum.RumActionType.Swipe => Com.Datadog.Android.Rum.RumActionType.Swipe,
            Maui.Rum.RumActionType.Click => Com.Datadog.Android.Rum.RumActionType.Click,
            Maui.Rum.RumActionType.Custom => Com.Datadog.Android.Rum.RumActionType.Custom,
            _ => Com.Datadog.Android.Rum.RumActionType.Custom
        };
    }

    private static RumResourceKind MapResourceKind(Maui.Rum.RumResourceKind kind)
    {
        return kind switch
        {
            Maui.Rum.RumResourceKind.Image => Com.Datadog.Android.Rum.RumResourceKind.Image,
            Maui.Rum.RumResourceKind.Xhr => Com.Datadog.Android.Rum.RumResourceKind.Xhr,
            Maui.Rum.RumResourceKind.Beacon => Com.Datadog.Android.Rum.RumResourceKind.Beacon,
            Maui.Rum.RumResourceKind.Css => Com.Datadog.Android.Rum.RumResourceKind.Css,
            Maui.Rum.RumResourceKind.Document => Com.Datadog.Android.Rum.RumResourceKind.Document,
            Maui.Rum.RumResourceKind.Font => Com.Datadog.Android.Rum.RumResourceKind.Font,
            Maui.Rum.RumResourceKind.Js => Com.Datadog.Android.Rum.RumResourceKind.Js,
            Maui.Rum.RumResourceKind.Media => Com.Datadog.Android.Rum.RumResourceKind.Media,
            Maui.Rum.RumResourceKind.Native => Com.Datadog.Android.Rum.RumResourceKind.Native,
            Maui.Rum.RumResourceKind.Other => Com.Datadog.Android.Rum.RumResourceKind.Other,
            _ => Com.Datadog.Android.Rum.RumResourceKind.Native
        };
    }

    private static RumErrorSource MapErrorSource(Maui.Rum.RumErrorSource source)
    {
        return source switch
        {
            Maui.Rum.RumErrorSource.Source => Com.Datadog.Android.Rum.RumErrorSource.Source,
            Maui.Rum.RumErrorSource.Network => Com.Datadog.Android.Rum.RumErrorSource.Network,
            Maui.Rum.RumErrorSource.WebView => Com.Datadog.Android.Rum.RumErrorSource.Webview,
            Maui.Rum.RumErrorSource.Custom => Com.Datadog.Android.Rum.RumErrorSource.Source,
            _ => Com.Datadog.Android.Rum.RumErrorSource.Source
        };
    }
}
