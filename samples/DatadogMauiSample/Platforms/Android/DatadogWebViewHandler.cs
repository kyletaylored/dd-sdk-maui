using Android.Webkit;
using Com.Datadog.Android.Webview;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using AWebView = Android.Webkit.WebView;

namespace DatadogMauiSample.Platforms.Android;

/// <summary>
/// Custom WebView handler for Android that enables Datadog WebView tracking
/// </summary>
public class DatadogWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(AWebView platformView)
    {
        base.ConnectHandler(platformView);

        // Enable Datadog WebView tracking
        try
        {
            // List of hosts that should be tracked as first-party
            var allowedHosts = new List<string>
            {
                "shopist.io",
                "www.shopist.io",
                "datadoghq.com",
                "www.datadoghq.com"
            };

            // Enable WebView tracking for this WebView instance
            WebViewTracking.Enable(platformView, allowedHosts);

            Console.WriteLine($"[Datadog] WebView tracking enabled for hosts: {string.Join(", ", allowedHosts)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Failed to enable WebView tracking: {ex.Message}");
        }
    }
}
