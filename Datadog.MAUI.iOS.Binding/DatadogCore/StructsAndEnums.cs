using ObjCRuntime;

namespace Datadog.iOS.DatadogCore
{
	[Native]
	public enum DDBatchProcessingLevel : long
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	[Native]
	public enum DDBatchSize : long
	{
		Small = 0,
		Medium = 1,
		Large = 2
	}

	[Native]
	public enum DDUploadFrequency : long
	{
		Frequent = 0,
		Average = 1,
		Rare = 2
	}

	[Native]
	public enum DDCoreLoggerLevel : long
	{
		Debug = 0,
		Warn = 1,
		Error = 2,
		Critical = 3
	}
}
