using System;
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS
{
	// ========================================
	// DatadogCore Enums
	// ========================================

	/// <summary>
	/// User tracking consent level.
	/// </summary>
	[Native]
	public enum DDTrackingConsent : long
	{
		/// <summary>
		/// SDK doesn't collect or send any data.
		/// </summary>
		NotGranted = 0,

		/// <summary>
		/// SDK collects and sends data (user granted consent).
		/// </summary>
		Granted = 1,

		/// <summary>
		/// SDK buffers data locally until consent is granted or not granted.
		/// </summary>
		Pending = 2
	}

	/// <summary>
	/// Datadog site for data upload.
	/// </summary>
	[Native]
	public enum DDSite : long
	{
		/// <summary>
		/// US1 site (datadoghq.com).
		/// </summary>
		US1 = 0,

		/// <summary>
		/// US3 site (us3.datadoghq.com).
		/// </summary>
		US3 = 1,

		/// <summary>
		/// US5 site (us5.datadoghq.com).
		/// </summary>
		US5 = 2,

		/// <summary>
		/// EU1 site (datadoghq.eu).
		/// </summary>
		EU1 = 3,

		/// <summary>
		/// US1_FED site (ddog-gov.com - FedRAMP).
		/// </summary>
		US1_FED = 4,

		/// <summary>
		/// AP1 site (ap1.datadoghq.com).
		/// </summary>
		AP1 = 5
	}

	/// <summary>
	/// Batch size for data upload.
	/// </summary>
	[Native]
	public enum DDBatchSize : long
	{
		/// <summary>
		/// Small batches - upload more frequently, less data per upload.
		/// </summary>
		Small = 0,

		/// <summary>
		/// Medium batches (default).
		/// </summary>
		Medium = 1,

		/// <summary>
		/// Large batches - upload less frequently, more data per upload.
		/// </summary>
		Large = 2
	}

	/// <summary>
	/// Upload frequency for sending data to Datadog.
	/// </summary>
	[Native]
	public enum DDUploadFrequency : long
	{
		/// <summary>
		/// Frequent uploads.
		/// </summary>
		Frequent = 0,

		/// <summary>
		/// Average upload frequency (default).
		/// </summary>
		Average = 1,

		/// <summary>
		/// Rare uploads - conserve battery and bandwidth.
		/// </summary>
		Rare = 2
	}

	/// <summary>
	/// SDK logging verbosity level.
	/// </summary>
	[Native]
	public enum DDCoreLoggerLevel : long
	{
		/// <summary>
		/// No SDK logs.
		/// </summary>
		None = 0,

		/// <summary>
		/// Debug logs (verbose).
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Warning logs only.
		/// </summary>
		Warn = 2,

		/// <summary>
		/// Error logs only.
		/// </summary>
		Error = 3,

		/// <summary>
		/// Critical logs only.
		/// </summary>
		Critical = 4
	}

	// ========================================
	// DatadogRUM Enums
	// ========================================

	/// <summary>
	/// Type of user action.
	/// </summary>
	[Native]
	public enum DDRUMActionType : long
	{
		/// <summary>
		/// Tap action.
		/// </summary>
		Tap = 0,

		/// <summary>
		/// Scroll action.
		/// </summary>
		Scroll = 1,

		/// <summary>
		/// Swipe action.
		/// </summary>
		Swipe = 2,

		/// <summary>
		/// Click action.
		/// </summary>
		Click = 3,

		/// <summary>
		/// Custom action.
		/// </summary>
		Custom = 4
	}

	/// <summary>
	/// Source of an error.
	/// </summary>
	[Native]
	public enum DDRUMErrorSource : long
	{
		/// <summary>
		/// Error from application source code.
		/// </summary>
		Source = 0,

		/// <summary>
		/// Error from network request.
		/// </summary>
		Network = 1,

		/// <summary>
		/// Error from WebView.
		/// </summary>
		WebView = 2,

		/// <summary>
		/// Error from console output.
		/// </summary>
		Console = 3,

		/// <summary>
		/// Custom error.
		/// </summary>
		Custom = 4
	}

	/// <summary>
	/// Type of network resource.
	/// </summary>
	[Native]
	public enum DDRUMResourceType : long
	{
		/// <summary>
		/// Image resource.
		/// </summary>
		Image = 0,

		/// <summary>
		/// XHR (XMLHttpRequest) resource.
		/// </summary>
		Xhr = 1,

		/// <summary>
		/// Beacon resource.
		/// </summary>
		Beacon = 2,

		/// <summary>
		/// CSS stylesheet.
		/// </summary>
		Css = 3,

		/// <summary>
		/// HTML document.
		/// </summary>
		Document = 4,

		/// <summary>
		/// Fetch API resource.
		/// </summary>
		Fetch = 5,

		/// <summary>
		/// Font resource.
		/// </summary>
		Font = 6,

		/// <summary>
		/// JavaScript resource.
		/// </summary>
		Js = 7,

		/// <summary>
		/// Media (audio/video) resource.
		/// </summary>
		Media = 8,

		/// <summary>
		/// Other resource type.
		/// </summary>
		Other = 9,

		/// <summary>
		/// Native platform resource.
		/// </summary>
		Native = 10
	}

	// ========================================
	// DatadogSessionReplay Enums
	// ========================================

	/// <summary>
	/// Privacy level for text and input fields in session replay.
	/// </summary>
	[Native]
	public enum DDTextAndInputPrivacy : long
	{
		/// <summary>
		/// Mask all text and inputs.
		/// </summary>
		MaskAll = 0,

		/// <summary>
		/// Mask sensitive inputs only (passwords, credit cards, etc.).
		/// </summary>
		MaskSensitiveInputs = 1,

		/// <summary>
		/// Allow all text and inputs to be recorded.
		/// </summary>
		AllowAll = 2
	}

	/// <summary>
	/// Privacy level for images in session replay.
	/// </summary>
	[Native]
	public enum DDImagePrivacy : long
	{
		/// <summary>
		/// Mask all images.
		/// </summary>
		MaskAll = 0,

		/// <summary>
		/// Mask images marked as non-bundled only.
		/// </summary>
		MaskNonBundled = 1,

		/// <summary>
		/// Don't mask any images.
		/// </summary>
		MaskNone = 2
	}

	/// <summary>
	/// Privacy level for touch interactions in session replay.
	/// </summary>
	[Native]
	public enum DDTouchPrivacy : long
	{
		/// <summary>
		/// Hide all touch interactions.
		/// </summary>
		Hide = 0,

		/// <summary>
		/// Show all touch interactions.
		/// </summary>
		Show = 1
	}
}
