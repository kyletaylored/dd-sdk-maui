using ObjCRuntime;

namespace Datadog.iOS.RUM
{
	[Native]
	public enum DDRUMActionEventActionActionType : long
	{
		Custom = 0,
		Click = 1,
		Tap = 2,
		Scroll = 3,
		Swipe = 4,
		ApplicationStart = 5,
		Back = 6
	}

	[Native]
	public enum DDRUMActionEventActionFrustrationFrustrationType : long
	{
		RageClick = 0,
		DeadClick = 1,
		ErrorClick = 2,
		RageTap = 3,
		ErrorTap = 4
	}

	[Native]
	public enum DDRUMActionEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMActionEventDDActionNameSource : long
	{
		None = 0,
		CustomAttribute = 1,
		MaskPlaceholder = 2,
		StandardAttribute = 3,
		TextContent = 4,
		MaskDisallowed = 5,
		Blank = 6
	}

	[Native]
	public enum DDRUMActionEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMActionEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMActionEventDeviceDeviceType : long
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
	public enum DDRUMActionEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMActionEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMActionEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMActionEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMActionEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMActionType : long
	{
		Tap = 0,
		Scroll = 1,
		Swipe = 2,
		Custom = 3
	}

	[Native]
	public enum DDRUMErrorEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMErrorEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMErrorEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMErrorEventDeviceDeviceType : long
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
	public enum DDRUMErrorEventErrorCSPDisposition : long
	{
		None = 0,
		Enforce = 1,
		Report = 2
	}

	[Native]
	public enum DDRUMErrorEventErrorCategory : long
	{
		None = 0,
		Anr = 1,
		AppHang = 2,
		Exception = 3,
		WatchdogTermination = 4,
		MemoryWarning = 5,
		Network = 6
	}

	[Native]
	public enum DDRUMErrorEventErrorCausesSource : long
	{
		Network = 0,
		Source = 1,
		Console = 2,
		Logger = 3,
		Agent = 4,
		Webview = 5,
		Custom = 6,
		Report = 7
	}

	[Native]
	public enum DDRUMErrorEventErrorHandling : long
	{
		None = 0,
		Handled = 1,
		Unhandled = 2
	}

	[Native]
	public enum DDRUMErrorEventErrorResourceProviderProviderType : long
	{
		None = 0,
		Ad = 1,
		Advertising = 2,
		Analytics = 3,
		Cdn = 4,
		Content = 5,
		CustomerSuccess = 6,
		FirstParty = 7,
		Hosting = 8,
		Marketing = 9,
		Other = 10,
		Social = 11,
		TagManager = 12,
		Utility = 13,
		Video = 14
	}

	[Native]
	public enum DDRUMErrorEventErrorResourceRUMMethod : long
	{
		Post = 0,
		Get = 1,
		Head = 2,
		Put = 3,
		Delete = 4,
		Patch = 5,
		Trace = 6,
		Options = 7,
		Connect = 8
	}

	[Native]
	public enum DDRUMErrorEventErrorSource : long
	{
		Network = 0,
		Source = 1,
		Console = 2,
		Logger = 3,
		Agent = 4,
		Webview = 5,
		Custom = 6,
		Report = 7
	}

	[Native]
	public enum DDRUMErrorEventErrorSourceType : long
	{
		None = 0,
		Android = 1,
		Browser = 2,
		Ios = 3,
		ReactNative = 4,
		Flutter = 5,
		Roku = 6,
		Ndk = 7,
		IosIl2cpp = 8,
		NdkIl2cpp = 9
	}

	[Native]
	public enum DDRUMErrorEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMErrorEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMErrorEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMErrorEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMErrorEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMErrorSource : long
	{
		Source = 0,
		Network = 1,
		Webview = 2,
		Console = 3,
		Custom = 4
	}

	[Native]
	public enum DDRUMFeatureOperationFailureReason : long
	{
		Error = 0,
		Abandoned = 1,
		Other = 2
	}

	[Native]
	public enum DDRUMLongTaskEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMLongTaskEventDDProfilingErrorReason : long
	{
		None = 0,
		NotSupportedByBrowser = 1,
		FailedToLazyLoad = 2,
		MissingDocumentPolicyHeader = 3,
		UnexpectedException = 4
	}

	[Native]
	public enum DDRUMLongTaskEventDDProfilingStatus : long
	{
		None = 0,
		Starting = 1,
		Running = 2,
		Stopped = 3,
		Error = 4
	}

	[Native]
	public enum DDRUMLongTaskEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMLongTaskEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMLongTaskEventDeviceDeviceType : long
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
	public enum DDRUMLongTaskEventLongTaskEntryType : long
	{
		None = 0,
		LongTask = 1,
		LongAnimationFrame = 2
	}

	[Native]
	public enum DDRUMLongTaskEventLongTaskScriptsInvokerType : long
	{
		None = 0,
		UserCallback = 1,
		EventListener = 2,
		ResolvePromise = 3,
		RejectPromise = 4,
		ClassicScript = 5,
		ModuleScript = 6
	}

	[Native]
	public enum DDRUMLongTaskEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMLongTaskEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMLongTaskEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMLongTaskEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMLongTaskEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMMethod : long
	{
		Post = 0,
		Get = 1,
		Head = 2,
		Put = 3,
		Delete = 4,
		Patch = 5,
		Connect = 6,
		Trace = 7,
		Options = 8
	}

	[Native]
	public enum DDRUMResourceEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMResourceEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMResourceEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMResourceEventDeviceDeviceType : long
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
	public enum DDRUMResourceEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMResourceEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMResourceEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMResourceEventResourceDeliveryType : long
	{
		None = 0,
		Cache = 1,
		NavigationalPrefetch = 2,
		Other = 3
	}

	[Native]
	public enum DDRUMResourceEventResourceGraphqlOperationType : long
	{
		None = 0,
		Query = 1,
		Mutation = 2,
		Subscription = 3
	}

	[Native]
	public enum DDRUMResourceEventResourceProviderProviderType : long
	{
		None = 0,
		Ad = 1,
		Advertising = 2,
		Analytics = 3,
		Cdn = 4,
		Content = 5,
		CustomerSuccess = 6,
		FirstParty = 7,
		Hosting = 8,
		Marketing = 9,
		Other = 10,
		Social = 11,
		TagManager = 12,
		Utility = 13,
		Video = 14
	}

	[Native]
	public enum DDRUMResourceEventResourceRUMMethod : long
	{
		None = 0,
		Post = 1,
		Get = 2,
		Head = 3,
		Put = 4,
		Delete = 5,
		Patch = 6,
		Trace = 7,
		Options = 8,
		Connect = 9
	}

	[Native]
	public enum DDRUMResourceEventResourceRenderBlockingStatus : long
	{
		None = 0,
		Blocking = 1,
		NonBlocking = 2
	}

	[Native]
	public enum DDRUMResourceEventResourceResourceType : long
	{
		Document = 0,
		Xhr = 1,
		Beacon = 2,
		Fetch = 3,
		Css = 4,
		Js = 5,
		Image = 6,
		Font = 7,
		Media = 8,
		Other = 9,
		Native = 10
	}

	[Native]
	public enum DDRUMResourceEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMResourceEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMViewEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMViewEventDDPageStatesState : long
	{
		Active = 0,
		Passive = 1,
		Hidden = 2,
		Frozen = 3,
		Terminated = 4
	}

	[Native]
	public enum DDRUMViewEventDDProfilingErrorReason : long
	{
		None = 0,
		NotSupportedByBrowser = 1,
		FailedToLazyLoad = 2,
		MissingDocumentPolicyHeader = 3,
		UnexpectedException = 4
	}

	[Native]
	public enum DDRUMViewEventDDProfilingStatus : long
	{
		None = 0,
		Starting = 1,
		Running = 2,
		Stopped = 3,
		Error = 4
	}

	[Native]
	public enum DDRUMViewEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMViewEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMViewEventDeviceDeviceType : long
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
	public enum DDRUMViewEventPrivacyReplayLevel : long
	{
		Allow = 0,
		Mask = 1,
		MaskUserInput = 2
	}

	[Native]
	public enum DDRUMViewEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMViewEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMViewEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMViewEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMViewEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMViewEventViewLoadingType : long
	{
		None = 0,
		InitialLoad = 1,
		RouteChange = 2,
		ActivityDisplay = 3,
		ActivityRedisplay = 4,
		FragmentDisplay = 5,
		FragmentRedisplay = 6,
		ViewControllerDisplay = 7,
		ViewControllerRedisplay = 8
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventDDProfilingErrorReason : long
	{
		None = 0,
		NotSupportedByBrowser = 1,
		FailedToLazyLoad = 2,
		MissingDocumentPolicyHeader = 3,
		UnexpectedException = 4
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventDDProfilingStatus : long
	{
		None = 0,
		Starting = 1,
		Running = 2,
		Stopped = 3,
		Error = 4
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventDeviceDeviceType : long
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
	public enum DDRUMVitalAppLaunchEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventVitalAppLaunchMetric : long
	{
		id = 0,
		fd = 1
	}

	[Native]
	public enum DDRUMVitalAppLaunchEventVitalStartupType : long
	{
		None = 0,
		ColdStart = 1,
		WarmStart = 2
	}

	[Native]
	public enum DDRUMVitalDurationEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMVitalDurationEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMVitalDurationEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMVitalDurationEventDeviceDeviceType : long
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
	public enum DDRUMVitalDurationEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMVitalDurationEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMVitalDurationEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMVitalDurationEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMVitalDurationEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMVitalOperationStepEventContainerSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Roku = 5,
		Unity = 6,
		KotlinMultiplatform = 7
	}

	[Native]
	public enum DDRUMVitalOperationStepEventDDSessionPlan : long
	{
		None = 0,
		Plan1 = 1,
		Plan2 = 2
	}

	[Native]
	public enum DDRUMVitalOperationStepEventDDSessionRUMSessionPrecondition : long
	{
		None = 0,
		UserAppLaunch = 1,
		InactivityTimeout = 2,
		MaxDuration = 3,
		BackgroundLaunch = 4,
		Prewarm = 5,
		FromNonInteractiveSession = 6,
		ExplicitStop = 7
	}

	[Native]
	public enum DDRUMVitalOperationStepEventDeviceDeviceType : long
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
	public enum DDRUMVitalOperationStepEventRUMConnectivityEffectiveType : long
	{
		None = 0,
		Slow2g = 1,
		EffectiveType2g = 2,
		EffectiveType3g = 3,
		EffectiveType4g = 4
	}

	[Native]
	public enum DDRUMVitalOperationStepEventRUMConnectivityInterfaces : long
	{
		None = 0,
		Bluetooth = 1,
		Cellular = 2,
		Ethernet = 3,
		Wifi = 4,
		Wimax = 5,
		Mixed = 6,
		Other = 7,
		Unknown = 8,
		InterfacesNone = 9
	}

	[Native]
	public enum DDRUMVitalOperationStepEventRUMConnectivityStatus : long
	{
		Connected = 0,
		NotConnected = 1,
		Maybe = 2
	}

	[Native]
	public enum DDRUMVitalOperationStepEventSessionRUMSessionType : long
	{
		User = 0,
		Synthetics = 1,
		CiTest = 2
	}

	[Native]
	public enum DDRUMVitalOperationStepEventSource : long
	{
		None = 0,
		Android = 1,
		Ios = 2,
		Browser = 3,
		Flutter = 4,
		ReactNative = 5,
		Roku = 6,
		Unity = 7,
		KotlinMultiplatform = 8
	}

	[Native]
	public enum DDRUMVitalOperationStepEventVitalFailureReason : long
	{
		None = 0,
		Error = 1,
		Abandoned = 2,
		Other = 3
	}

	[Native]
	public enum DDRUMVitalOperationStepEventVitalStepType : long
	{
		Start = 0,
		Update = 1,
		Retry = 2,
		End = 3
	}

	[Native]
	public enum DDRUMResourceType : long
	{
		Image = 0,
		Xhr = 1,
		Beacon = 2,
		Css = 3,
		Document = 4,
		Fetch = 5,
		Font = 6,
		Js = 7,
		Media = 8,
		Other = 9,
		Native = 10
	}

	[Native]
	public enum DDTelemetryConfigurationEventSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Unity = 5,
		KotlinMultiplatform = 6
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationSelectedTracingPropagators : long
	{
		None = 0,
		Datadog = 1,
		B3 = 2,
		B3multi = 3,
		Tracecontext = 4
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationSessionPersistence : long
	{
		None = 0,
		LocalStorage = 1,
		Cookie = 2
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationTraceContextInjection : long
	{
		None = 0,
		All = 1,
		Sampled = 2
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationTrackFeatureFlagsForEvents : long
	{
		None = 0,
		Vital = 1,
		Resource = 2,
		Action = 3,
		LongTask = 4
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationTrackingConsent : long
	{
		None = 0,
		Granted = 1,
		NotGranted = 2,
		Pending = 3
	}

	[Native]
	public enum DDTelemetryConfigurationEventTelemetryConfigurationViewTrackingStrategy : long
	{
		None = 0,
		ActivityViewTrackingStrategy = 1,
		FragmentViewTrackingStrategy = 2,
		MixedViewTrackingStrategy = 3,
		NavigationViewTrackingStrategy = 4
	}

	[Native]
	public enum DDTelemetryDebugEventSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Unity = 5,
		KotlinMultiplatform = 6
	}

	[Native]
	public enum DDTelemetryErrorEventSource : long
	{
		Android = 0,
		Ios = 1,
		Browser = 2,
		Flutter = 3,
		ReactNative = 4,
		Unity = 5,
		KotlinMultiplatform = 6
	}

	[Native]
	public enum DDRUMVitalsFrequency : long
	{
		Frequent = 0,
		Average = 1,
		Rare = 2,
		Never = 3
	}
}
