using ObjCRuntime;

namespace Datadog.iOS.Logs
{
	/// <summary>
	/// Device type for log events.
	/// </summary>
	[Native]
	public enum DDLogEventDeviceDeviceType : long
	{
		/// <summary>
		/// No device type specified.
		/// </summary>
		None = 0,

		/// <summary>
		/// Mobile device (phone).
		/// </summary>
		Mobile = 1,

		/// <summary>
		/// Desktop computer.
		/// </summary>
		Desktop = 2,

		/// <summary>
		/// Tablet device.
		/// </summary>
		Tablet = 3,

		/// <summary>
		/// Television or smart TV.
		/// </summary>
		Tv = 4,

		/// <summary>
		/// Gaming console.
		/// </summary>
		GamingConsole = 5,

		/// <summary>
		/// Bot or automated script.
		/// </summary>
		Bot = 6,

		/// <summary>
		/// Other device type.
		/// </summary>
		Other = 7
	}

	/// <summary>
	/// Network interface type.
	/// </summary>
	[Native]
	public enum DDLogEventInterface : long
	{
		/// <summary>
		/// WiFi connection.
		/// </summary>
		Wifi = 0,

		/// <summary>
		/// Wired Ethernet connection.
		/// </summary>
		WiredEthernet = 1,

		/// <summary>
		/// Cellular data connection.
		/// </summary>
		Cellular = 2,

		/// <summary>
		/// Loopback interface (localhost).
		/// </summary>
		Loopback = 3,

		/// <summary>
		/// Other interface type.
		/// </summary>
		Other = 4
	}

	/// <summary>
	/// Radio access technology for cellular connections.
	/// </summary>
	[Native]
	public enum DDLogEventRadioAccessTechnology : long
	{
		/// <summary>
		/// GPRS (2G).
		/// </summary>
		Gprs = 0,

		/// <summary>
		/// EDGE (2.5G).
		/// </summary>
		Edge = 1,

		/// <summary>
		/// WCDMA (3G).
		/// </summary>
		Wcdma = 2,

		/// <summary>
		/// HSDPA (3.5G).
		/// </summary>
		Hsdpa = 3,

		/// <summary>
		/// HSUPA (3.75G).
		/// </summary>
		Hsupa = 4,

		/// <summary>
		/// CDMA 1x (2G).
		/// </summary>
		CDMA1x = 5,

		/// <summary>
		/// CDMA EVDO Rev 0 (3G).
		/// </summary>
		CDMAEVDORev0 = 6,

		/// <summary>
		/// CDMA EVDO Rev A (3G).
		/// </summary>
		CDMAEVDORevA = 7,

		/// <summary>
		/// CDMA EVDO Rev B (3G).
		/// </summary>
		CDMAEVDORevB = 8,

		/// <summary>
		/// eHRPD (3.9G).
		/// </summary>
		Ehrpd = 9,

		/// <summary>
		/// LTE (4G).
		/// </summary>
		Lte = 10,

		/// <summary>
		/// Unknown technology.
		/// </summary>
		Unknown = 11
	}

	/// <summary>
	/// Network reachability status.
	/// </summary>
	[Native]
	public enum DDLogEventReachability : long
	{
		/// <summary>
		/// Network is reachable.
		/// </summary>
		Yes = 0,

		/// <summary>
		/// Network reachability is uncertain.
		/// </summary>
		Maybe = 1,

		/// <summary>
		/// Network is not reachable.
		/// </summary>
		No = 2
	}

	/// <summary>
	/// Log event status/severity level.
	/// </summary>
	[Native]
	public enum DDLogEventStatus : long
	{
		/// <summary>
		/// Debug level - detailed diagnostic information.
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Info level - informational messages.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Notice level - normal but significant events.
		/// </summary>
		Notice = 2,

		/// <summary>
		/// Warning level - potential issues.
		/// </summary>
		Warn = 3,

		/// <summary>
		/// Error level - error conditions.
		/// </summary>
		Error = 4,

		/// <summary>
		/// Critical level - critical conditions.
		/// </summary>
		Critical = 5,

		/// <summary>
		/// Emergency level - system is unusable.
		/// </summary>
		Emergency = 6
	}

	/// <summary>
	/// Logger severity level.
	/// </summary>
	[Native]
	public enum DDLogLevel : long
	{
		/// <summary>
		/// Debug level.
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Info level.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Notice level.
		/// </summary>
		Notice = 2,

		/// <summary>
		/// Warning level.
		/// </summary>
		Warn = 3,

		/// <summary>
		/// Error level.
		/// </summary>
		Error = 4,

		/// <summary>
		/// Critical level.
		/// </summary>
		Critical = 5
	}
}
