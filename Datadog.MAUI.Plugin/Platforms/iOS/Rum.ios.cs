using Datadog.iOS.RUM;
using Foundation;

namespace Datadog.Maui.Rum;

public static partial class Rum
{
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = ConvertAttributes(attributes);
        monitor.StartView(key, name, nsAttributes);
    }

    static partial void PlatformStopView(string key, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = ConvertAttributes(attributes);
        monitor.StopView(key, nsAttributes);
    }

    static partial void PlatformAddAction(RumActionType type, string name, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var actionType = MapActionType(type);
        var nsAttributes = ConvertAttributes(attributes);
        monitor.AddAction(actionType, name, nsAttributes);
    }

    // Note: Resource tracking is intentionally NOT implemented here.
    // The iOS SDK's URLSessionInstrumentation automatically tracks HTTP resources when enabled.
    // Manual resource tracking is complex and error-prone. Instead, configure URLSessionInstrumentation
    // in your Datadog initialization code (see Swift example in shopist-appdelegate.swift lines 82, 95).
    static partial void PlatformStartResource(string key, string method, string url, Dictionary<string, object>? attributes)
    {
        // No-op: Let URLSessionInstrumentation handle this automatically
    }

    static partial void PlatformStopResource(string key, int? statusCode, long? size, RumResourceKind kind, Dictionary<string, object>? attributes)
    {
        // No-op: Let URLSessionInstrumentation handle this automatically
    }

    static partial void PlatformStopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes)
    {
        // No-op: Let URLSessionInstrumentation handle this automatically
    }

    static partial void PlatformAddError(string message, RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var errorSource = MapErrorSource(source);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : new NSDictionary<NSString, NSObject>();

        if (exception != null)
        {
            // Create NSError with detailed exception information
            var userInfo = new NSMutableDictionary<NSString, NSObject>
            {
                [NSError.LocalizedDescriptionKey] = new NSString(message ?? exception.Message),
                [new NSString("ExceptionType")] = new NSString(exception.GetType().FullName ?? exception.GetType().Name),
                [new NSString("Message")] = new NSString(exception.Message)
            };

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                userInfo[new NSString("StackTrace")] = new NSString(exception.StackTrace);
            }

            if (exception.InnerException != null)
            {
                userInfo[new NSString("InnerException")] = new NSString(exception.InnerException.ToString());
            }

            var nsError = NSError.FromDomain(
                new NSString(exception.GetType().Name),
                -1,
                userInfo
            );

            monitor.AddError(nsError, errorSource, nsAttributes);
        }
        else
        {
            monitor.AddError(message, errorSource, null, nsAttributes);
        }
    }

    static partial void PlatformAddTiming(string name)
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.AddTiming(name);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.AddAttribute(key, NSObject.FromObject(value));
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.RemoveAttribute(key);
    }

    static partial void PlatformStartSession()
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.StartSession();
    }

    static partial void PlatformStopSession()
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.StopSession();
    }

    private static DDRUMActionType MapActionType(Maui.Rum.RumActionType type)
    {
        return type switch
        {
            Maui.Rum.RumActionType.Tap => DDRUMActionType.Tap,
            Maui.Rum.RumActionType.Scroll => DDRUMActionType.Scroll,
            Maui.Rum.RumActionType.Swipe => DDRUMActionType.Swipe,
            Maui.Rum.RumActionType.Click => DDRUMActionType.Tap,
            Maui.Rum.RumActionType.Custom => DDRUMActionType.Custom,
            _ => DDRUMActionType.Custom
        };
    }

    private static DDRUMErrorSource MapErrorSource(Maui.Rum.RumErrorSource source)
    {
        return source switch
        {
            Maui.Rum.RumErrorSource.Source => DDRUMErrorSource.Source,
            Maui.Rum.RumErrorSource.Network => DDRUMErrorSource.Network,
            Maui.Rum.RumErrorSource.WebView => DDRUMErrorSource.Webview,
            Maui.Rum.RumErrorSource.Custom => DDRUMErrorSource.Custom,
            _ => DDRUMErrorSource.Source
        };
    }

    private static NSDictionary<NSString, NSObject> ConvertAttributes(Dictionary<string, object>? attributes)
    {
        if (attributes == null || attributes.Count == 0)
            return new NSDictionary<NSString, NSObject>();

        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();

        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
