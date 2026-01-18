using Datadog.iOS.DatadogRUM;
using Foundation;
using Datadog.iOS.DatadogInternal;

namespace Datadog.Maui.Rum;

public static partial class Rum
{
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StartView(
            key: key,
            name: name,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStopView(string key, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StopView(
            key: key,
            attributes: nsAttributes
        );
    }

    static partial void PlatformAddAction(RumActionType type, string name, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var actionType = MapActionType(type);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.AddAction(
            type: actionType,
            name: name,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStartResource(string key, string method, string url, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StartResource(
            resourceKey: key,
            httpMethod: method,
            urlString: url,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStopResource(string key, int? statusCode, long? size, RumResourceKind kind, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var resourceKind = MapResourceKind(kind);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StopResource(
            resourceKey: key,
            statusCode: statusCode.HasValue ? new NSNumber(statusCode.Value) : null,
            kind: resourceKind,
            size: size.HasValue ? new NSNumber(size.Value) : null,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStopResourceWithError(string key, Exception error, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;
        var nsError = NSError.FromDomain(
            new NSString("Exception"),
            0,
            NSDictionary<NSString, NSObject>.FromObjectAndKey(
                new NSString(error.ToString()),
                NSError.LocalizedDescriptionKey
            )
        );

        monitor.StopResourceWithError(
            resourceKey: key,
            error: nsError,
            attributes: nsAttributes
        );
    }

    static partial void PlatformAddError(string message, RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes)
    {
        var monitor = DDRUMMonitor.Shared;
        var errorSource = MapErrorSource(source);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        if (exception != null)
        {
            var nsError = NSError.FromDomain(
                new NSString("Exception"),
                0,
                NSDictionary<NSString, NSObject>.FromObjectAndKey(
                    new NSString(exception.ToString()),
                    NSError.LocalizedDescriptionKey
                )
            );
            monitor.AddError(
                error: nsError,
                source: errorSource,
                attributes: nsAttributes
            );
        }
        else
        {
            monitor.AddError(
                message: message,
                source: errorSource,
                stack: null,
                attributes: nsAttributes
            );
        }
    }

    static partial void PlatformAddTiming(string name)
    {
        var monitor = DDRUMMonitor.Shared;
        monitor.AddTiming(name: name);
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

    private static DDRUMResourceType MapResourceKind(Maui.Rum.RumResourceKind kind)
    {
        return kind switch
        {
            Maui.Rum.RumResourceKind.Image => DDRUMResourceType.Image,
            Maui.Rum.RumResourceKind.Xhr => DDRUMResourceType.Xhr,
            Maui.Rum.RumResourceKind.Beacon => DDRUMResourceType.Beacon,
            Maui.Rum.RumResourceKind.Css => DDRUMResourceType.Css,
            Maui.Rum.RumResourceKind.Document => DDRUMResourceType.Document,
            Maui.Rum.RumResourceKind.Font => DDRUMResourceType.Font,
            Maui.Rum.RumResourceKind.Js => DDRUMResourceType.Js,
            Maui.Rum.RumResourceKind.Media => DDRUMResourceType.Media,
            Maui.Rum.RumResourceKind.Native => DDRUMResourceType.Native,
            Maui.Rum.RumResourceKind.Other => DDRUMResourceType.Other,
            _ => DDRUMResourceType.Native
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

    private static DDRUMMethod MapHttpMethod(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => DDRUMMethod.Get,
            "POST" => DDRUMMethod.Post,
            "PUT" => DDRUMMethod.Put,
            "DELETE" => DDRUMMethod.Delete,
            "HEAD" => DDRUMMethod.Head,
            "PATCH" => DDRUMMethod.Patch,
            _ => DDRUMMethod.Get
        };
    }

    private static NSDictionary<NSString, NSObject>? ConvertAttributes(Dictionary<string, object> attributes)
    {
        if (attributes == null || attributes.Count == 0)
            return null;

        var keys = attributes.Keys.Select(k => new NSString(k)).ToArray();
        var values = attributes.Values.Select(v => NSObject.FromObject(v)).ToArray();

        return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys);
    }
}
