using GlobalRumMonitor = Datadog.Android.RUM.GlobalRumMonitor;
using RumResourceMethod = Datadog.Android.RUM.RumResourceMethod;
using RumActionType = Datadog.Android.RUM.RumActionType;
using RumResourceKind = Datadog.Android.RUM.RumResourceKind;
using RumErrorSource = Datadog.Android.RUM.RumErrorSource;
using Datadog.Maui.Platforms.Android;

namespace Datadog.Maui.Rum;

public static partial class Rum
{
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
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
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
            rumMonitor.StopView(key, javaAttributes);
        }
        else
        {
            rumMonitor.StopView(key, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformAddAction(global::Datadog.Maui.Rum.RumActionType type, string name, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var actionType = MapActionType(type);

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
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
        var resourceMethod = RumResourceMethod.ValueOf(method.ToUpperInvariant());

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
            rumMonitor.StartResource(key, resourceMethod, url, javaAttributes);
        }
        else
        {
            rumMonitor.StartResource(key, resourceMethod, url, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStopResource(string key, int? statusCode, long? size, global::Datadog.Maui.Rum.RumResourceKind kind, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var resourceKind = MapResourceKind(kind);
        var javaStatusCode = statusCode.HasValue ? Java.Lang.Integer.ValueOf(statusCode.Value) : null;
        var javaSize = size.HasValue ? Java.Lang.Long.ValueOf(size.Value) : null;

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
            rumMonitor.StopResource(key, javaStatusCode, javaSize, resourceKind, javaAttributes);
        }
        else
        {
            rumMonitor.StopResource(key, javaStatusCode, javaSize, resourceKind, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformStopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var throwable = error.ToJavaThrowable();

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
            rumMonitor.StopResourceWithError(key, null, error.Message, global::Datadog.Android.RUM.RumErrorSource.Network!, throwable, javaAttributes);
        }
        else
        {
            rumMonitor.StopResourceWithError(key, null, error.Message, global::Datadog.Android.RUM.RumErrorSource.Network!, throwable, new Dictionary<string, Java.Lang.Object>());
        }
    }

    static partial void PlatformAddError(string message, global::Datadog.Maui.Rum.RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes)
    {
        var rumMonitor = GlobalRumMonitor.Get();
        var errorSource = MapErrorSource(source);
        var throwable = exception?.ToJavaThrowable();

        if (attributes != null && attributes.Count > 0)
        {
            var javaAttributes = attributes.ToDictionary(kvp => kvp.Key, kvp => ConvertToJavaObject(kvp.Value));
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
        var javaValue = value as Java.Lang.Object ?? new Java.Lang.String(value?.ToString() ?? "");
        rumMonitor.AddAttribute(key, javaValue);
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

    private static global::Datadog.Android.RUM.RumActionType MapActionType(global::Datadog.Maui.Rum.RumActionType type)
    {
        return type switch
        {
            global::Datadog.Maui.Rum.RumActionType.Tap => global::Datadog.Android.RUM.RumActionType.Tap!,
            global::Datadog.Maui.Rum.RumActionType.Scroll => global::Datadog.Android.RUM.RumActionType.Scroll!,
            global::Datadog.Maui.Rum.RumActionType.Swipe => global::Datadog.Android.RUM.RumActionType.Swipe!,
            global::Datadog.Maui.Rum.RumActionType.Click => global::Datadog.Android.RUM.RumActionType.Click!,
            global::Datadog.Maui.Rum.RumActionType.Custom => global::Datadog.Android.RUM.RumActionType.Custom!,
            _ => global::Datadog.Android.RUM.RumActionType.Custom!
        };
    }

    private static global::Datadog.Android.RUM.RumResourceKind MapResourceKind(global::Datadog.Maui.Rum.RumResourceKind kind)
    {
        return kind switch
        {
            global::Datadog.Maui.Rum.RumResourceKind.Image => global::Datadog.Android.RUM.RumResourceKind.Image!,
            global::Datadog.Maui.Rum.RumResourceKind.Xhr => global::Datadog.Android.RUM.RumResourceKind.Xhr!,
            global::Datadog.Maui.Rum.RumResourceKind.Beacon => global::Datadog.Android.RUM.RumResourceKind.Beacon!,
            global::Datadog.Maui.Rum.RumResourceKind.Css => global::Datadog.Android.RUM.RumResourceKind.Css!,
            global::Datadog.Maui.Rum.RumResourceKind.Document => global::Datadog.Android.RUM.RumResourceKind.Document!,
            global::Datadog.Maui.Rum.RumResourceKind.Font => global::Datadog.Android.RUM.RumResourceKind.Font!,
            global::Datadog.Maui.Rum.RumResourceKind.Js => global::Datadog.Android.RUM.RumResourceKind.Js!,
            global::Datadog.Maui.Rum.RumResourceKind.Media => global::Datadog.Android.RUM.RumResourceKind.Media!,
            global::Datadog.Maui.Rum.RumResourceKind.Native => global::Datadog.Android.RUM.RumResourceKind.Native!,
            global::Datadog.Maui.Rum.RumResourceKind.Other => global::Datadog.Android.RUM.RumResourceKind.Other!,
            _ => global::Datadog.Android.RUM.RumResourceKind.Native!
        };
    }

    private static global::Datadog.Android.RUM.RumErrorSource MapErrorSource(global::Datadog.Maui.Rum.RumErrorSource source)
    {
        return source switch
        {
            global::Datadog.Maui.Rum.RumErrorSource.Source => global::Datadog.Android.RUM.RumErrorSource.Source!,
            global::Datadog.Maui.Rum.RumErrorSource.Network => global::Datadog.Android.RUM.RumErrorSource.Network!,
            global::Datadog.Maui.Rum.RumErrorSource.WebView => global::Datadog.Android.RUM.RumErrorSource.Webview!,
            global::Datadog.Maui.Rum.RumErrorSource.Custom => global::Datadog.Android.RUM.RumErrorSource.Source!,
            _ => global::Datadog.Android.RUM.RumErrorSource.Source!
        };
    }

    private static Java.Lang.Object ConvertToJavaObject(object value)
    {
        // If it's already a Java object, return as-is
        if (value is Java.Lang.Object javaObj)
        {
            return javaObj;
        }

        // Convert .NET types to Java types
        return value switch
        {
            string s => new Java.Lang.String(s),
            int i => Java.Lang.Integer.ValueOf(i),
            long l => Java.Lang.Long.ValueOf(l),
            double d => Java.Lang.Double.ValueOf(d),
            float f => Java.Lang.Float.ValueOf(f),
            bool b => Java.Lang.Boolean.ValueOf(b),
            byte by => Java.Lang.Byte.ValueOf((sbyte)by),
            short sh => Java.Lang.Short.ValueOf(sh),
            _ => new Java.Lang.String(value?.ToString() ?? "")
        };
    }
}
