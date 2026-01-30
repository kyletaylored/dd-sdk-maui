using ObjCRuntime;

namespace Datadog.iOS.Core
{
	/// <summary>
	/// Batch processing level for Datadog data uploads.
	/// </summary>
	[Native]
	public enum DDBatchProcessingLevel : long
	{
		/// <summary>
		/// Low processing level - minimal CPU usage.
		/// </summary>
		Low = 0,

		/// <summary>
		/// Medium processing level - balanced CPU usage.
		/// </summary>
		Medium = 1,

		/// <summary>
		/// High processing level - maximum throughput.
		/// </summary>
		High = 2
	}

	/// <summary>
	/// Size of batches for uploading Datadog data.
	/// </summary>
	[Native]
	public enum DDBatchSize : long
	{
		/// <summary>
		/// Small batch size - more frequent uploads, less data per request.
		/// </summary>
		Small = 0,

		/// <summary>
		/// Medium batch size - balanced frequency and data size.
		/// </summary>
		Medium = 1,

		/// <summary>
		/// Large batch size - less frequent uploads, more data per request.
		/// </summary>
		Large = 2
	}

	/// <summary>
	/// Frequency of data uploads to Datadog servers.
	/// </summary>
	[Native]
	public enum DDUploadFrequency : long
	{
		/// <summary>
		/// Frequent uploads - data sent more often.
		/// </summary>
		Frequent = 0,

		/// <summary>
		/// Average upload frequency - balanced approach.
		/// </summary>
		Average = 1,

		/// <summary>
		/// Rare uploads - data sent less frequently to conserve resources.
		/// </summary>
		Rare = 2
	}

	/// <summary>
	/// Log verbosity level for the Datadog SDK internal logger.
	/// </summary>
	[Native]
	public enum DDCoreLoggerLevel : long
	{
		/// <summary>
		/// Debug level - detailed diagnostic information.
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Warning level - potential issues that don't prevent operation.
		/// </summary>
		Warn = 1,

		/// <summary>
		/// Error level - errors that affect functionality.
		/// </summary>
		Error = 2,

		/// <summary>
		/// Critical level - severe errors requiring immediate attention.
		/// </summary>
		Critical = 3
	}
}
