using DatadogRUM;
using Foundation;
using DatadogInternal;

namespace Datadog.Maui.Rum;

public static partial class Rum
{
    static partial void PlatformStartView(string key, string name, Dictionary<string, object>? attributes)
    {
        var monitor = RUMMonitor.Shared();
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StartView(
            key: key,
            name: name,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStopView(string key, Dictionary<string, object>? attributes)
    {
        var monitor = RUMMonitor.Shared();
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StopView(
            key: key,
            attributes: nsAttributes
        );
    }

    static partial void PlatformAddAction(RumActionType type, string name, Dictionary<string, object>? attributes)
    {
        var monitor = RUMMonitor.Shared();
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
        var monitor = RUMMonitor.Shared();
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        monitor.StartResource(
            resourceKey: key,
            httpMethod: MapHttpMethod(method),
            urlString: url,
            attributes: nsAttributes
        );
    }

    static partial void PlatformStopResource(string key, int? statusCode, long? size, RumResourceKind kind, Dictionary<string, object>? attributes)
    {
        var monitor = RUMMonitor.Shared();
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
        var monitor = RUMMonitor.Shared();
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;
        var ddError = new DDError(error);

        monitor.StopResourceWithError(
            resourceKey: key,
            error: ddError,
            attributes: nsAttributes
        );
    }

    static partial void PlatformAddError(string message, RumErrorSource source, Exception? exception, Dictionary<string, object>? attributes)
    {
        var monitor = RUMMonitor.Shared();
        var errorSource = MapErrorSource(source);
        var nsAttributes = attributes != null ? ConvertAttributes(attributes) : null;

        if (exception != null)
        {
            var ddError = new DDError(exception);
            monitor.AddError(
                error: ddError,
                source: errorSource,
                attributes: nsAttributes
            );
        }
        else
        {
            monitor.AddError(
                message: message,
                source: errorSource,
                attributes: nsAttributes
            );
        }
    }

    static partial void PlatformAddTiming(string name)
    {
        var monitor = RUMMonitor.Shared();
        monitor.AddTiming(name: name);
    }

    static partial void PlatformAddAttribute(string key, object value)
    {
        var monitor = RUMMonitor.Shared();
        monitor.AddAttribute(
            forKey: key,
            value: NSObject.FromObject(value)
        );
    }

    static partial void PlatformRemoveAttribute(string key)
    {
        var monitor = RUMMonitor.Shared();
        monitor.RemoveAttribute(forKey: key);
    }

    static partial void PlatformStartSession()
    {
        var monitor = RUMMonitor.Shared();
        monitor.StartSession();
    }

    static partial void PlatformStopSession()
    {
        var monitor = RUMMonitor.Shared();
        monitor.StopSession();
    }

    private static RUMActionType MapActionType(Maui.Rum.RumActionType type)
    {
        return type switch
        {
            Maui.Rum.RumActionType.Tap => RUMActionType.Tap,
            Maui.Rum.RumActionType.Scroll => RUMActionType.Scroll,
            Maui.Rum.RumActionType.Swipe => RUMActionType.Swipe,
            Maui.Rum.RumActionType.Click => RUMActionType.Tap,
            Maui.Rum.RumActionType.Custom => RUMActionType.Custom,
            _ => RUMActionType.Custom
        };
    }

    private static RUMResourceType MapResourceKind(Maui.Rum.RumResourceKind kind)
    {
        return kind switch
        {
            Maui.Rum.RumResourceKind.Image => RUMResourceType.Image,
            Maui.Rum.RumResourceKind.Xhr => RUMResourceType.Xhr,
            Maui.Rum.RumResourceKind.Beacon => RUMResourceType.Beacon,
            Maui.Rum.RumResourceKind.Css => RUMResourceType.Css,
            Maui.Rum.RumResourceKind.Document => RUMResourceType.Document,
            Maui.Rum.RumResourceKind.Font => RUMResourceType.Font,
            Maui.Rum.RumResourceKind.Js => RUMResourceType.Js,
            Maui.Rum.RumResourceKind.Media => RUMResourceType.Media,
            Maui.Rum.RumResourceKind.Native => RUMResourceType.Native,
            Maui.Rum.RumResourceKind.Other => RUMResourceType.Other,
            _ => RUMResourceType.Native
        };
    }

    private static RUMErrorSource MapErrorSource(Maui.Rum.RumErrorSource source)
    {
        return source switch
        {
            Maui.Rum.RumErrorSource.Source => RUMErrorSource.Source,
            Maui.Rum.RumErrorSource.Network => RUMErrorSource.Network,
            Maui.Rum.RumErrorSource.WebView => RUMErrorSource.Webview,
            Maui.Rum.RumErrorSource.Custom => RUMErrorSource.Custom,
            _ => RUMErrorSource.Source
        };
    }

    private static RUMMethod MapHttpMethod(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => RUMMethod.Get,
            "POST" => RUMMethod.Post,
            "PUT" => RUMMethod.Put,
            "DELETE" => RUMMethod.Delete,
            "HEAD" => RUMMethod.Head,
            "PATCH" => RUMMethod.Patch,
            _ => RUMMethod.Get
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
