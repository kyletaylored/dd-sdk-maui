using Foundation;
using ObjCRuntime;
using WebKit;

namespace Datadog.iOS.WebViewTracking
{
	/// <summary>
	/// Enables Datadog RUM and Logs correlation between native app and WebViews.
	/// </summary>
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDWebViewTracking
	{
		/// <summary>
		/// Enables SDK to correlate Datadog RUM events and Logs from the WebView with native RUM session.
		/// If the content loaded in WebView uses Datadog Browser SDK (v4.2.0+) and matches specified
		/// hosts, web events will be correlated with the RUM session from native SDK.
		/// </summary>
		/// <param name="webView">The web-view to track.</param>
		/// <param name="hosts">A set of hosts instrumented with Browser SDK to capture Datadog events from.</param>
		/// <param name="logsSampleRate">The sampling rate for logs coming from the WebView. Must be a value between 0 and 100, where 0 means no logs will be sent and 100 means all will be uploaded. Default: 100.</param>
		[Static]
		[Export("enableWithWebView:hosts:logsSampleRate:")]
		void EnableWithWebView(WKWebView webView, NSSet<NSString> hosts, float logsSampleRate);

		/// <summary>
		/// Disables Datadog iOS SDK and Datadog Browser SDK integration.
		/// Removes Datadog's ScriptMessageHandler and UserScript from the caller.
		/// Note: This method must be called when the webview can be deinitialized.
		/// </summary>
		/// <param name="webView">The web-view to stop tracking.</param>
		[Static]
		[Export("disableWithWebView:")]
		void DisableWithWebView(WKWebView webView);
	}
}
