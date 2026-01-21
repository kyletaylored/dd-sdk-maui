using Foundation;
using ObjCRuntime;

namespace Datadog.iOS.DatadogRUM
{
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
		[Export("startViewWithKey:name:attributes:")]
		void StartView(string key, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a view.
		/// </summary>
		[Export("stopViewWithKey:attributes:")]
		void StopView(string key, [NullAllowed] NSDictionary attributes);

		// Action Tracking

		/// <summary>
		/// Adds a user action.
		/// </summary>
		[Export("addActionWithType:name:attributes:")]
		void AddAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Starts tracking a continuous action.
		/// </summary>
		[Export("startActionWithType:name:attributes:")]
		void StartAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a continuous action.
		/// </summary>
		[Export("stopActionWithType:name:attributes:")]
		void StopAction(DDRUMActionType type, string name, [NullAllowed] NSDictionary attributes);

		// Error Tracking

		/// <summary>
		/// Adds an error to RUM.
		/// </summary>
		[Export("addErrorWithMessage:source:stack:attributes:")]
		void AddError(string message, DDRUMErrorSource source, [NullAllowed] string stack, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Adds an error from an NSError object.
		/// </summary>
		[Export("addErrorWithError:source:attributes:")]
		void AddError(NSError error, DDRUMErrorSource source, [NullAllowed] NSDictionary attributes);

		// Resource Tracking

		/// <summary>
		/// Starts tracking a resource.
		/// </summary>
		[Export("startResourceWithKey:httpMethod:urlString:attributes:")]
		void StartResource(string resourceKey, string httpMethod, string urlString, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a successful resource.
		/// </summary>
		[Export("stopResourceWithKey:statusCode:kind:size:attributes:")]
		void StopResource(string resourceKey, [NullAllowed] NSNumber statusCode, DDRUMResourceType kind, [NullAllowed] NSNumber size, [NullAllowed] NSDictionary attributes);

		/// <summary>
		/// Stops tracking a failed resource.
		/// </summary>
		[Export("stopResourceWithErrorWithKey:error:attributes:")]
		void StopResourceWithError(string resourceKey, NSError error, [NullAllowed] NSDictionary attributes);

		// Session Management

		/// <summary>
		/// Stops the current RUM session.
		/// </summary>
		[Export("stopSession")]
		void StopSession();

		/// <summary>
		/// Starts a new RUM session explicitly.
		/// </summary>
		[Export("startSession")]
		void StartSession();

		// Attribute Management

		/// <summary>
		/// Adds a custom attribute to all future RUM events.
		/// </summary>
		[Export("addAttributeForKey:value:")]
		void AddAttribute(string key, NSObject value);

		/// <summary>
		/// Removes a custom attribute from future RUM events.
		/// </summary>
		[Export("removeAttributeForKey:")]
		void RemoveAttribute(string key);

		// Timing Management

		/// <summary>
		/// Adds a custom timing to the current view.
		/// </summary>
		[Export("addTimingWithName:")]
		void AddTiming(string name);
	}

	/// <summary>
	/// DDRUM enables RUM (Real User Monitoring) feature in the Datadog SDK.
	/// </summary>
	[BaseType(typeof(NSObject))]
	interface DDRUM
	{
		/// <summary>
		/// Enables RUM with the specified configuration.
		/// </summary>
		[Static]
		[Export("enableWith:")]
		void EnableWith(DDRUMConfiguration configuration);
	}

	/// <summary>
	/// Configuration for RUM feature.
	/// </summary>
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDRUMConfiguration
	{
		/// <summary>
		/// Initializes a new RUM configuration with the application ID.
		/// </summary>
		[Export("initWithApplicationID:")]
		[DesignatedInitializer]
		NativeHandle Constructor(string applicationID);

		/// <summary>
		/// The RUM application ID.
		/// </summary>
		[Export("applicationID")]
		string ApplicationID { get; }

		/// <summary>
		/// The sampling rate for RUM sessions (0.0 to 100.0).
		/// </summary>
		[Export("sessionSampleRate")]
		float SessionSampleRate { get; set; }

		/// <summary>
		/// The sampling rate for telemetry events (0.0 to 100.0).
		/// </summary>
		[Export("telemetrySampleRate")]
		float TelemetrySampleRate { get; set; }

		/// <summary>
		/// Whether to track frustration signals (rage taps, dead clicks, etc.).
		/// </summary>
		[Export("trackFrustrations")]
		bool TrackFrustrations { get; set; }

		/// <summary>
		/// Whether to track events while the app is in the background.
		/// </summary>
		[Export("trackBackgroundEvents")]
		bool TrackBackgroundEvents { get; set; }

		/// <summary>
		/// Whether to track watchdog terminations.
		/// </summary>
		[Export("trackWatchdogTerminations")]
		bool TrackWatchdogTerminations { get; set; }

		/// <summary>
		/// Threshold for long task detection in seconds.
		/// </summary>
		[Export("longTaskThreshold")]
		double LongTaskThreshold { get; set; }

		/// <summary>
		/// Threshold for app hang detection in seconds.
		/// </summary>
		[Export("appHangThreshold")]
		double AppHangThreshold { get; set; }

		/// <summary>
		/// Frequency for collecting vitals information.
		/// </summary>
		[Export("vitalsUpdateFrequency", ArgumentSemantic.Assign)]
		DDRUMVitalsFrequency VitalsUpdateFrequency { get; set; }

		/// <summary>
		/// Custom endpoint for RUM data.
		/// </summary>
		[NullAllowed, Export("customEndpoint", ArgumentSemantic.Copy)]
		NSUrl CustomEndpoint { get; set; }

		/// <summary>
		/// Whether to track anonymous users.
		/// </summary>
		[Export("trackAnonymousUser")]
		bool TrackAnonymousUser { get; set; }

		/// <summary>
		/// Whether to track memory warnings.
		/// </summary>
		[Export("trackMemoryWarnings")]
		bool TrackMemoryWarnings { get; set; }
	}
}
