using ObjCRuntime;

namespace Datadog.iOS.Internal
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
