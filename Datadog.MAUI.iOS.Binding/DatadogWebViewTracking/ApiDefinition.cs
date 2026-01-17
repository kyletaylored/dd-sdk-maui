using DatadogWebViewTracking;
using Foundation;

namespace DatadogMaui.iOS.WebViewTracking
{
	// @interface DDWebViewTracking
	[DisableDefaultCtor]
	interface DDWebViewTracking
	{
		// +(void)enableWithWebView:(WKWebView * _Nonnull)webView hosts:(id)hosts logsSampleRate:(float)logsSampleRate;
		[Static]
		[Export ("enableWithWebView:hosts:logsSampleRate:")]
		void EnableWithWebView (WKWebView webView, NSObject hosts, float logsSampleRate);

		// +(void)disableWithWebView:(WKWebView * _Nonnull)webView;
		[Static]
		[Export ("disableWithWebView:")]
		void DisableWithWebView (WKWebView webView);
	}
}
