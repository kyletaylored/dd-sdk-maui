using ObjCRuntime;

namespace Datadog.iOS.Internal
{
	/// <summary>
	/// Internal logger level for the Datadog SDK.
	/// </summary>
	[Native]
	public enum DDCoreLoggerLevel : long
	{
		/// <summary>
		/// No logging.
		/// </summary>
		None = 0,

		/// <summary>
		/// Debug level - detailed diagnostic information.
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Warning level - potential issues.
		/// </summary>
		Warn = 2,

		/// <summary>
		/// Error level - errors that affect functionality.
		/// </summary>
		Error = 3,

		/// <summary>
		/// Critical level - severe errors.
		/// </summary>
		Critical = 4
	}
}
