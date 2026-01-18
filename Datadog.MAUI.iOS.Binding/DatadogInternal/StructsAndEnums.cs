using ObjCRuntime;

namespace Datadog.iOS.DatadogInternal
{
	[Native]
	public enum DDCoreLoggerLevel : long
	{
		None = 0,
		Debug = 1,
		Warn = 2,
		Error = 3,
		Critical = 4
	}
}
