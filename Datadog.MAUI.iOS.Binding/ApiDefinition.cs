using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using WebKit;

namespace Datadog.iOS
{
	// ========================================
	// DatadogCore - SDK Initialization
	// ========================================

	/// <summary>
	/// Main Datadog SDK class for initialization and configuration.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDDatadog
	{
		// SDK Initialization

		/// <summary>
		/// Initializes the Datadog SDK with the given configuration and tracking consent.
		/// </summary>
		/// <param name="configuration">SDK configuration containing client token, environment, etc.</param>
		/// <param name="trackingConsent">Initial user tracking consent level.</param>
		[Static]
		[Export("initializeWith:trackingConsent:")]
		void Initialize(DDConfiguration configuration, DDTrackingConsent trackingConsent);

		// SDK State Management

		/// <summary>
		/// Checks if the Datadog SDK has been initialized.
		/// </summary>
		[Static]
		[Export("isInitialized")]
		bool IsInitialized { get; }

		// User Information

		/// <summary>
		/// Sets user information for tracking purposes.
		/// </summary>
		/// <param name="id">User identifier (optional).</param>
		/// <param name="name">User name (optional).</param>
		/// <param name="email">User email (optional).</param>
		/// <param name="extraInfo">Additional user attributes (optional).</param>
		[Static]
		[Export("setUserInfoWithId:name:email:extraInfo:")]
		void SetUserInfo([NullAllowed] string id, [NullAllowed] string name, [NullAllowed] string email, [NullAllowed] NSDictionary extraInfo);

		/// <summary>
		/// Clears all user information.
		/// </summary>
		[Static]
		[Export("clearUserInfo")]
		void ClearUserInfo();

		// Tracking Consent

		/// <summary>
		/// Updates the tracking consent level.
		/// </summary>
		/// <param name="trackingConsent">New tracking consent level.</param>
		[Static]
		[Export("setTrackingConsent:")]
		void SetTrackingConsent(DDTrackingConsent trackingConsent);

		// Verbosity

		/// <summary>
		/// Sets the SDK verbosity level for debugging.
		/// </summary>
		/// <param name="verbosityLevel">Log verbosity level.</param>
		[Static]
		[Export("setVerbosityLevel:")]
		void SetVerbosityLevel(DDCoreLoggerLevel verbosityLevel);

		// Data Management

		/// <summary>
		/// Clears all locally stored data.
		/// </summary>
		[Static]
		[Export("clearAllData")]
		void ClearAllData();
	}

	/// <summary>
	/// Configuration for the Datadog SDK.
	/// </summary>
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDConfiguration
	{
		// Initialization

		/// <summary>
		/// Creates a new SDK configuration.
		/// </summary>
		/// <param name="clientToken">Your Datadog client token.</param>
		/// <param name="env">Environment name (e.g., "prod", "staging").</param>
		[Export("initWithClientToken:env:")]
		NativeHandle Constructor(string clientToken, string env);

		// Required Properties

		/// <summary>
		/// Client token used to authenticate SDK requests.
		/// </summary>
		[Export("clientToken")]
		string ClientToken { get; set; }

		/// <summary>
		/// Environment name for data segregation.
		/// </summary>
		[Export("env")]
		string Env { get; set; }

		// Optional Properties

		/// <summary>
		/// Datadog site to send data to (default: US1).
		/// </summary>
		[Export("site", ArgumentSemantic.Assign)]
		DDSite Site { get; set; }

		/// <summary>
		/// Service name for this application.
		/// </summary>
		[NullAllowed, Export("service")]
		string Service { get; set; }

		/// <summary>
		/// Application version number.
		/// </summary>
		[NullAllowed, Export("version")]
		string Version { get; set; }

		// Performance Configuration

		/// <summary>
		/// Batch size for uploading data.
		/// </summary>
		[Export("batchSize", ArgumentSemantic.Assign)]
		DDBatchSize BatchSize { get; set; }

		/// <summary>
		/// Upload frequency for sending data to Datadog.
		/// </summary>
		[Export("uploadFrequency", ArgumentSemantic.Assign)]
		DDUploadFrequency UploadFrequency { get; set; }
	}

	// ========================================
	// DatadogRUM - Real User Monitoring
	// ========================================

	/// <summary>
	/// Static class for enabling RUM (Real User Monitoring).
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDRUM
	{
		/// <summary>
		/// Enables RUM monitoring with the given configuration.
		/// </summary>
		/// <param name="configuration">RUM configuration.</param>
		[Static]
		[Export("enable:")]
		void Enable(DDRUMConfiguration configuration);
	}

	/// <summary>
	/// Configuration for RUM monitoring.
	/// </summary>
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDRUMConfiguration
	{
		/// <summary>
		/// Creates a RUM configuration with the application ID.
		/// </summary>
		/// <param name="applicationId">RUM application ID from Datadog.</param>
		[Export("initWithApplicationID:")]
		NativeHandle Constructor(string applicationId);

		/// <summary>
		/// RUM application identifier.
		/// </summary>
		[Export("applicationID")]
		string ApplicationId { get; }

		/// <summary>
		/// Session sample rate (0.0 to 100.0). Default: 100.0 (all sessions).
		/// </summary>
		[Export("sessionSampleRate")]
		float SessionSampleRate { get; set; }

		/// <summary>
		/// Enables automatic UIKit view tracking.
		/// </summary>
		[Export("trackUIKitViews")]
		void TrackUIKitViews();

		/// <summary>
		/// Enables automatic UIKit action (tap, swipe) tracking.
		/// </summary>
		[Export("trackUIKitActions")]
		void TrackUIKitActions();

		/// <summary>
		/// Enables long task tracking with the given threshold (in seconds).
		/// </summary>
		/// <param name="threshold">Duration threshold for considering a task "long" (default: 0.1s).</param>
		[Export("trackLongTasks:")]
		void TrackLongTasks(double threshold);

		/// <summary>
		/// Enables tracking of events when app is in background.
		/// </summary>
		/// <param name="enabled">Whether to track background events.</param>
		[Export("trackBackgroundEvents:")]
		void TrackBackgroundEvents(bool enabled);
	}

	/// <summary>
	/// RUM monitor for manually tracking views, actions, resources, and errors.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDRUMMonitor
	{
		/// <summary>
		/// Gets the shared RUM monitor instance.
		/// </summary>
		[Static]
		[Export("shared")]
		DDRUMMonitor Shared { get; }

		// View Tracking

		/// <summary>
		/// Starts tracking a view.
		/// </summary>
		/// <param name="key">Unique view identifier (e.g., view controller class name).</param>
		/// <param name="name">Human-readable view name.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("startViewWithKey:name:attributes:")]
		void StartView(string key, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a view.
		/// </summary>
		/// <param name="key">View identifier passed to StartView.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("stopViewWithKey:attributes:")]
		void StopView(string key, [NullAllowed] NSDictionary attributes);

		// Action Tracking

		/// <summary>
		/// Adds a user action (e.g., button tap, custom action).
		/// </summary>
		/// <param name="type">Type of action.</param>
		/// <param name="name">Action name.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("addActionWithType:name:attributes:")]
		void AddAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Starts tracking a continuous action.
		/// </summary>
		/// <param name="type">Type of action.</param>
		/// <param name="name">Action name.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("startActionWithType:name:attributes:")]
		void StartAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a continuous action.
		/// </summary>
		/// <param name="type">Type of action.</param>
		/// <param name="name">Action name matching StartAction.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("stopActionWithType:name:attributes:")]
		void StopAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		// Error Tracking

		/// <summary>
		/// Adds an error to RUM.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="source">Error source (network, source, console, etc.).</param>
		/// <param name="stack">Stack trace (optional).</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("addErrorWithMessage:source:stack:attributes:")]
		void AddError(string message, DDRUMErrorSource source, [NullAllowed] string stack, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Adds an error from an NSError object.
		/// </summary>
		/// <param name="error">NSError object.</param>
		/// <param name="source">Error source.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("addErrorWithError:source:attributes:")]
		void AddError(NSError error, DDRUMErrorSource source, [NullAllowed] NSDictionary attributes);

		// Resource Tracking (for manual network request tracking)

		/// <summary>
		/// Starts tracking a resource (network request).
		/// </summary>
		/// <param name="resourceKey">Unique resource identifier.</param>
		/// <param name="httpMethod">HTTP method (GET, POST, etc.).</param>
		/// <param name="urlString">Request URL.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("startResourceWithKey:httpMethod:urlString:attributes:")]
		void StartResource(string resourceKey, string httpMethod, string urlString, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a successful resource.
		/// </summary>
		/// <param name="resourceKey">Resource identifier passed to StartResource.</param>
		/// <param name="statusCode">HTTP status code.</param>
		/// <param name="size">Response size in bytes (optional).</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("stopResourceWithKey:statusCode:size:attributes:")]
		void StopResource(string resourceKey, nint statusCode, long size, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a failed resource.
		/// </summary>
		/// <param name="resourceKey">Resource identifier passed to StartResource.</param>
		/// <param name="error">Error that occurred.</param>
		/// <param name="attributes">Additional custom attributes (optional).</param>
		[Export("stopResourceWithErrorWithKey:error:attributes:")]
		void StopResourceWithError(string resourceKey, NSError error, [NullAllowed] NSDictionary attributes);

		// Attribute Management

		/// <summary>
		/// Adds a global attribute to all RUM events.
		/// </summary>
		/// <param name="key">Attribute key.</param>
		/// <param name="value">Attribute value.</param>
		[Export("addAttribute:forKey:")]
		void AddAttribute(NSObject value, string key);

		/// <summary>
		/// Removes a global attribute.
		/// </summary>
		/// <param name="key">Attribute key to remove.</param>
		[Export("removeAttributeForKey:")]
		void RemoveAttribute(string key);

		// Session Management

		/// <summary>
		/// Gets the current RUM session ID.
		/// </summary>
		/// <param name="completion">Callback with session ID (may be null if no active session).</param>
		[Export("currentSessionID:")]
		void GetCurrentSessionId(Action<string> completion);
	}

	// ========================================
	// DatadogLogs - Logging
	// ========================================

	/// <summary>
	/// Static class for enabling logging.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDLogs
	{
		/// <summary>
		/// Enables logging with the given configuration.
		/// </summary>
		/// <param name="configuration">Logs configuration (optional, uses defaults if null).</param>
		[Static]
		[Export("enable:")]
		void Enable([NullAllowed] DDLogsConfiguration configuration);
	}

	/// <summary>
	/// Configuration for logging.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDLogsConfiguration
	{
		/// <summary>
		/// Creates a default logs configuration.
		/// </summary>
		[Export("init")]
		NativeHandle Constructor();
	}

	/// <summary>
	/// Logger for sending logs to Datadog.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDLogger
	{
		/// <summary>
		/// Creates a logger with default configuration.
		/// </summary>
		[Static]
		[Export("create")]
		DDLogger Create();

		/// <summary>
		/// Creates a logger with custom configuration.
		/// </summary>
		/// <param name="configuration">Logger configuration.</param>
		[Static]
		[Export("createWith:")]
		DDLogger Create(DDLoggerConfiguration configuration);

		// Logging Methods

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("debug:attributes:")]
		void Debug(string message, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("info:attributes:")]
		void Info(string message, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Logs a notice message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("notice:attributes:")]
		void Notice(string message, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("warn:attributes:")]
		void Warn(string message, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("error:attributes:")]
		void Error(string message, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Logs a critical message.
		/// </summary>
		/// <param name="message">Log message.</param>
		/// <param name="attributes">Additional attributes (optional).</param>
		[Export("critical:attributes:")]
		void Critical(string message, [NullAllowed] NSDictionary attributes);

		// Attribute Management

		/// <summary>
		/// Adds an attribute to all logs from this logger.
		/// </summary>
		/// <param name="key">Attribute key.</param>
		/// <param name="value">Attribute value.</param>
		[Export("addAttribute:forKey:")]
		void AddAttribute(NSObject value, string key);

		/// <summary>
		/// Removes an attribute from this logger.
		/// </summary>
		/// <param name="key">Attribute key to remove.</param>
		[Export("removeAttributeForKey:")]
		void RemoveAttribute(string key);
	}

	/// <summary>
	/// Configuration for creating a logger.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDLoggerConfiguration
	{
		/// <summary>
		/// Creates a default logger configuration.
		/// </summary>
		[Export("init")]
		NativeHandle Constructor();

		/// <summary>
		/// Service name for logs from this logger.
		/// </summary>
		[NullAllowed, Export("service")]
		string Service { get; set; }

		/// <summary>
		/// Logger name (appears in log explorer).
		/// </summary>
		[NullAllowed, Export("name")]
		string Name { get; set; }

		/// <summary>
		/// Whether to send network info with logs.
		/// </summary>
		[Export("networkInfoEnabled")]
		bool NetworkInfoEnabled { get; set; }

		/// <summary>
		/// Whether to bundle logs with active RUM session.
		/// </summary>
		[Export("bundleWithRumEnabled")]
		bool BundleWithRumEnabled { get; set; }

		/// <summary>
		/// Sample rate for logs (0.0 to 100.0).
		/// </summary>
		[Export("remoteSampleRate")]
		float RemoteSampleRate { get; set; }
	}

	// ========================================
	// DatadogTrace - APM Tracing
	// ========================================

	/// <summary>
	/// Static class for enabling APM tracing.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDTrace
	{
		/// <summary>
		/// Enables APM tracing with the given configuration.
		/// </summary>
		/// <param name="configuration">Trace configuration (optional, uses defaults if null).</param>
		[Static]
		[Export("enable:")]
		void Enable([NullAllowed] DDTraceConfiguration configuration);
	}

	/// <summary>
	/// Configuration for APM tracing.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDTraceConfiguration
	{
		/// <summary>
		/// Creates a default trace configuration.
		/// </summary>
		[Export("init")]
		NativeHandle Constructor();

		/// <summary>
		/// Service name for traces.
		/// </summary>
		[NullAllowed, Export("service")]
		string Service { get; set; }

		/// <summary>
		/// Sample rate for traces (0.0 to 100.0).
		/// </summary>
		[Export("sampleRate")]
		float SampleRate { get; set; }

		/// <summary>
		/// Whether to bundle traces with active RUM session.
		/// </summary>
		[Export("bundleWithRumEnabled")]
		bool BundleWithRumEnabled { get; set; }

		/// <summary>
		/// Whether to send network info with traces.
		/// </summary>
		[Export("networkInfoEnabled")]
		bool NetworkInfoEnabled { get; set; }
	}

	/// <summary>
	/// Tracer for creating spans.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDTracer
	{
		/// <summary>
		/// Gets the shared tracer instance.
		/// </summary>
		[Static]
		[Export("shared")]
		DDTracer Shared { get; }

		/// <summary>
		/// Starts a new span.
		/// </summary>
		/// <param name="operationName">Name of the operation being traced.</param>
		/// <param name="tags">Span tags (optional).</param>
		/// <param name="startTime">Custom start time (optional, uses current time if null).</param>
		[Export("startSpan:tags:startTime:")]
		DDSpan StartSpan(string operationName, [NullAllowed] NSDictionary tags, [NullAllowed] NSDate startTime);
	}

	/// <summary>
	/// Represents a trace span.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDSpan
	{
		/// <summary>
		/// Sets a tag on the span.
		/// </summary>
		/// <param name="key">Tag key.</param>
		/// <param name="value">Tag value.</param>
		[Export("setTag:value:")]
		void SetTag(string key, NSObject value);

		/// <summary>
		/// Logs an event to the span.
		/// </summary>
		/// <param name="fields">Event fields.</param>
		[Export("log:")]
		void Log(NSDictionary fields);

		/// <summary>
		/// Finishes the span (marks completion).
		/// </summary>
		[Export("finish")]
		void Finish();

		/// <summary>
		/// Finishes the span with a custom end time.
		/// </summary>
		/// <param name="finishTime">Custom end time.</param>
		[Export("finishWithTime:")]
		void Finish(NSDate finishTime);
	}

	// ========================================
	// DatadogSessionReplay - Session Recording
	// ========================================

	/// <summary>
	/// Static class for enabling session replay.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDSessionReplay
	{
		/// <summary>
		/// Enables session replay with the given configuration.
		/// </summary>
		/// <param name="configuration">Session replay configuration.</param>
		[Static]
		[Export("enable:")]
		void Enable(DDSessionReplayConfiguration configuration);
	}

	/// <summary>
	/// Configuration for session replay.
	/// </summary>
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDSessionReplayConfiguration
	{
		/// <summary>
		/// Creates a session replay configuration.
		/// </summary>
		/// <param name="replaySampleRate">Sample rate for sessions to record (0.0 to 100.0).</param>
		[Export("initWithReplaySampleRate:")]
		NativeHandle Constructor(float replaySampleRate);

		/// <summary>
		/// Sets privacy level for text and input fields.
		/// </summary>
		/// <param name="privacyLevel">Privacy level.</param>
		[Export("setTextAndInputPrivacy:")]
		void SetTextAndInputPrivacy(DDTextAndInputPrivacy privacyLevel);

		/// <summary>
		/// Sets privacy level for images.
		/// </summary>
		/// <param name="privacyLevel">Privacy level.</param>
		[Export("setImagePrivacy:")]
		void SetImagePrivacy(DDImagePrivacy privacyLevel);

		/// <summary>
		/// Sets privacy level for touch interactions.
		/// </summary>
		/// <param name="privacyLevel">Privacy level.</param>
		[Export("setTouchPrivacy:")]
		void SetTouchPrivacy(DDTouchPrivacy privacyLevel);
	}

	// ========================================
	// DatadogCrashReporting - Crash Reports
	// ========================================

	/// <summary>
	/// Static class for enabling crash reporting.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDCrashReporting
	{
		/// <summary>
		/// Enables crash reporting (automatically integrates with RUM).
		/// </summary>
		[Static]
		[Export("enable")]
		void Enable();
	}

	// ========================================
	// DatadogWebViewTracking - WebView Integration
	// ========================================

	/// <summary>
	/// Static class for enabling WebView tracking.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDWebViewTracking
	{
		/// <summary>
		/// Enables tracking for a WKWebView.
		/// </summary>
		/// <param name="webView">WebView to track.</param>
		/// <param name="hosts">Allowed hosts for tracking (optional).</param>
		[Static]
		[Export("enableWithWebView:hosts:")]
		void Enable(WKWebView webView, [NullAllowed] string[] hosts);

		/// <summary>
		/// Disables tracking for a WKWebView.
		/// </summary>
		/// <param name="webView">WebView to stop tracking.</param>
		[Static]
		[Export("disableWithWebView:")]
		void Disable(WKWebView webView);
	}
}
