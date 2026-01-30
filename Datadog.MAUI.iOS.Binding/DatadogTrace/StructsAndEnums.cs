using ObjCRuntime;

namespace Datadog.iOS.Trace
{
	/// <summary>
	/// Encoding format for injecting trace context headers.
	/// </summary>
	[Native]
	public enum DDInjectEncoding : long
	{
		/// <summary>
		/// Multiple headers format - separate headers for each trace context value.
		/// </summary>
		Multiple = 0,

		/// <summary>
		/// Single header format - all trace context values in one header.
		/// </summary>
		Single = 1
	}

	/// <summary>
	/// Strategy for injecting trace context into HTTP requests.
	/// </summary>
	[Native]
	public enum DDTraceContextInjection : long
	{
		/// <summary>
		/// Inject trace context for all requests.
		/// </summary>
		All = 0,

		/// <summary>
		/// Inject trace context only for sampled requests.
		/// </summary>
		Sampled = 1
	}
}
