using ObjCRuntime;

namespace Datadog.iOS.DatadogLogs
{
	[Native]
	public enum DDLogEventDeviceDeviceType : long
	{
		None = 0,
		Mobile = 1,
		Desktop = 2,
		Tablet = 3,
		Tv = 4,
		GamingConsole = 5,
		Bot = 6,
		Other = 7
	}

	[Native]
	public enum DDLogEventInterface : long
	{
		Wifi = 0,
		WiredEthernet = 1,
		Cellular = 2,
		Loopback = 3,
		Other = 4
	}

	[Native]
	public enum DDLogEventRadioAccessTechnology : long
	{
		Gprs = 0,
		Edge = 1,
		Wcdma = 2,
		Hsdpa = 3,
		Hsupa = 4,
		CDMA1x = 5,
		CDMAEVDORev0 = 6,
		CDMAEVDORevA = 7,
		CDMAEVDORevB = 8,
		Ehrpd = 9,
		Lte = 10,
		Unknown = 11
	}

	[Native]
	public enum DDLogEventReachability : long
	{
		Yes = 0,
		Maybe = 1,
		No = 2
	}

	[Native]
	public enum DDLogEventStatus : long
	{
		Debug = 0,
		Info = 1,
		Notice = 2,
		Warn = 3,
		Error = 4,
		Critical = 5,
		Emergency = 6
	}

	[Native]
	public enum DDLogLevel : long
	{
		Debug = 0,
		Info = 1,
		Notice = 2,
		Warn = 3,
		Error = 4,
		Critical = 5
	}
}
