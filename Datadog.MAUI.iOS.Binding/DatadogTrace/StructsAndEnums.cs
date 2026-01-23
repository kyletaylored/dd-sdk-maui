using ObjCRuntime;

namespace Datadog.iOS.Trace
{
	[Native]
	public enum DDInjectEncoding : long
	{
		Multiple = 0,
		Single = 1
	}

	[Native]
	public enum DDTraceContextInjection : long
	{
		All = 0,
		Sampled = 1
	}
}
