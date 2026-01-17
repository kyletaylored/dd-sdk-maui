using System;
using DatadogRUM;
using Foundation;
using ObjCRuntime;

namespace DatadogMaui.iOS.RUM
{
	// @protocol DDSwiftUIRUMActionsPredicate
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDSwiftUIRUMActionsPredicate
	{
		// @required -(DDRUMAction * _Nullable)rumActionWith:(NSString * _Nonnull)componentName __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("rumActionWith:")]
		[return: NullAllowed]
		DDRUMAction RumActionWith (string componentName);
	}

	// @interface DDDefaultSwiftUIRUMActionsPredicate <DDSwiftUIRUMActionsPredicate>
	[DisableDefaultCtor]
	interface DDDefaultSwiftUIRUMActionsPredicate : IDDSwiftUIRUMActionsPredicate
	{
		// -(instancetype _Nonnull)initWithIsLegacyDetectionEnabled:(id)isLegacyDetectionEnabled __attribute__((objc_designated_initializer));
		[Export ("initWithIsLegacyDetectionEnabled:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject isLegacyDetectionEnabled);

		// -(DDRUMAction * _Nullable)rumActionWith:(NSString * _Nonnull)componentName __attribute__((warn_unused_result("")));
		[Export ("rumActionWith:")]
		[return: NullAllowed]
		DDRUMAction RumActionWith (string componentName);
	}

	// @protocol DDSwiftUIRUMViewsPredicate
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDSwiftUIRUMViewsPredicate
	{
		// @required -(DDRUMView * _Nullable)rumViewFor:(NSString * _Nonnull)extractedViewName __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("rumViewFor:")]
		[return: NullAllowed]
		DDRUMView RumViewFor (string extractedViewName);
	}

	// @interface DDDefaultSwiftUIRUMViewsPredicate <DDSwiftUIRUMViewsPredicate>
	interface DDDefaultSwiftUIRUMViewsPredicate : IDDSwiftUIRUMViewsPredicate
	{
		// -(DDRUMView * _Nullable)rumViewFor:(NSString * _Nonnull)extractedViewName __attribute__((warn_unused_result("")));
		[Export ("rumViewFor:")]
		[return: NullAllowed]
		DDRUMView RumViewFor (string extractedViewName);
	}

	// @protocol DDUITouchRUMActionsPredicate
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDUITouchRUMActionsPredicate
	{
		// @required -(DDRUMAction * _Nullable)rumActionWithTargetView:(UIView * _Nonnull)targetView __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("rumActionWithTargetView:")]
		[return: NullAllowed]
		DDRUMAction RumActionWithTargetView (UIView targetView);
	}

	// @protocol DDUIKitRUMActionsPredicate <DDUITouchRUMActionsPredicate>
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDUIKitRUMActionsPredicate : IDDUITouchRUMActionsPredicate
	{
	}

	// @interface DDDefaultUIKitRUMActionsPredicate <DDUIKitRUMActionsPredicate>
	interface DDDefaultUIKitRUMActionsPredicate : IDDUIKitRUMActionsPredicate
	{
		// -(DDRUMAction * _Nullable)rumActionWithTargetView:(UIView * _Nonnull)targetView __attribute__((warn_unused_result("")));
		[Export ("rumActionWithTargetView:")]
		[return: NullAllowed]
		DDRUMAction RumActionWithTargetView (UIView targetView);
	}

	// @protocol DDUIKitRUMViewsPredicate
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDUIKitRUMViewsPredicate
	{
		// @required -(DDRUMView * _Nullable)rumViewFor:(UIViewController * _Nonnull)viewController __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("rumViewFor:")]
		[return: NullAllowed]
		DDRUMView RumViewFor (UIViewController viewController);
	}

	// @interface DDDefaultUIKitRUMViewsPredicate <DDUIKitRUMViewsPredicate>
	interface DDDefaultUIKitRUMViewsPredicate : IDDUIKitRUMViewsPredicate
	{
		// -(DDRUMView * _Nullable)rumViewFor:(UIViewController * _Nonnull)viewController __attribute__((warn_unused_result("")));
		[Export ("rumViewFor:")]
		[return: NullAllowed]
		DDRUMView RumViewFor (UIViewController viewController);
	}

	// @interface DDRUMFirstPartyHostsTracing
	[DisableDefaultCtor]
	interface DDRUMFirstPartyHostsTracing
	{
		// -(instancetype _Nonnull)initWithHostsWithHeaderTypes:(id)hostsWithHeaderTypes __attribute__((objc_designated_initializer));
		[Export ("initWithHostsWithHeaderTypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject hostsWithHeaderTypes);

		// -(instancetype _Nonnull)initWithHostsWithHeaderTypes:(id)hostsWithHeaderTypes sampleRate:(float)sampleRate __attribute__((objc_designated_initializer));
		[Export ("initWithHostsWithHeaderTypes:sampleRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject hostsWithHeaderTypes, float sampleRate);

		// -(instancetype _Nonnull)initWithHosts:(id)hosts __attribute__((objc_designated_initializer));
		[Export ("initWithHosts:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject hosts);

		// -(instancetype _Nonnull)initWithHosts:(id)hosts sampleRate:(float)sampleRate __attribute__((objc_designated_initializer));
		[Export ("initWithHosts:sampleRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject hosts, float sampleRate);
	}

	// @interface DDRUM
	interface DDRUM
	{
		// +(void)enableWith:(DDRUMConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWith:")]
		void EnableWith (DDRUMConfiguration configuration);
	}

	// @interface DDRUMAction
	[DisableDefaultCtor]
	interface DDRUMAction
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name attributes:(id)attributes __attribute__((objc_designated_initializer));
		[Export ("initWithName:attributes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSObject attributes);
	}

	// @interface DDRUMActionEvent
	[DisableDefaultCtor]
	interface DDRUMActionEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMActionEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMActionEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventAction * _Nonnull action;
		[Export ("action", ArgumentSemantic.Strong)]
		DDRUMActionEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMActionEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMActionEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMActionEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMActionEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMActionEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMActionEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventSource source;
		[Export ("source")]
		DDRUMActionEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMActionEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMActionEventView View { get; }
	}

	// @interface DDRUMActionEventAction
	[DisableDefaultCtor]
	interface DDRUMActionEventAction
	{
		// @property (readonly, nonatomic, strong) DDRUMActionEventActionCrash * _Nullable crash;
		[NullAllowed, Export ("crash", ArgumentSemantic.Strong)]
		DDRUMActionEventActionCrash Crash { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventActionError * _Nullable error;
		[NullAllowed, Export ("error", ArgumentSemantic.Strong)]
		DDRUMActionEventActionError Error { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventActionFrustration * _Nullable frustration;
		[NullAllowed, Export ("frustration", ArgumentSemantic.Strong)]
		DDRUMActionEventActionFrustration Frustration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable loadingTime;
		[NullAllowed, Export ("loadingTime", ArgumentSemantic.Strong)]
		NSNumber LoadingTime { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventActionLongTask * _Nullable longTask;
		[NullAllowed, Export ("longTask", ArgumentSemantic.Strong)]
		DDRUMActionEventActionLongTask LongTask { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventActionResource * _Nullable resource;
		[NullAllowed, Export ("resource", ArgumentSemantic.Strong)]
		DDRUMActionEventActionResource Resource { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventActionTarget * _Nullable target;
		[NullAllowed, Export ("target", ArgumentSemantic.Strong)]
		DDRUMActionEventActionTarget Target { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventActionActionType type;
		[Export ("type")]
		DDRUMActionEventActionActionType Type { get; }
	}

	// @interface DDRUMActionEventActionCrash
	[DisableDefaultCtor]
	interface DDRUMActionEventActionCrash
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMActionEventActionError
	[DisableDefaultCtor]
	interface DDRUMActionEventActionError
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMActionEventActionFrustration
	[DisableDefaultCtor]
	interface DDRUMActionEventActionFrustration
	{
	}

	// @interface DDRUMActionEventActionLongTask
	[DisableDefaultCtor]
	interface DDRUMActionEventActionLongTask
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMActionEventActionResource
	[DisableDefaultCtor]
	interface DDRUMActionEventActionResource
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMActionEventActionTarget
	[DisableDefaultCtor]
	interface DDRUMActionEventActionTarget
	{
		// @property (copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; set; }
	}

	// @interface DDRUMActionEventApplication
	[DisableDefaultCtor]
	interface DDRUMActionEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMActionEventContainer
	[DisableDefaultCtor]
	interface DDRUMActionEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMActionEventContainerSource source;
		[Export ("source")]
		DDRUMActionEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMActionEventContainerView View { get; }
	}

	// @interface DDRUMActionEventContainerView
	[DisableDefaultCtor]
	interface DDRUMActionEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMActionEventDD
	[DisableDefaultCtor]
	interface DDRUMActionEventDD
	{
		// @property (readonly, nonatomic, strong) DDRUMActionEventDDAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDRUMActionEventDDAction Action { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMActionEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMActionEventDDSession Session { get; }
	}

	// @interface DDRUMActionEventDDAction
	[DisableDefaultCtor]
	interface DDRUMActionEventDDAction
	{
		// @property (nonatomic) enum DDRUMActionEventDDActionNameSource nameSource;
		[Export ("nameSource", ArgumentSemantic.Assign)]
		DDRUMActionEventDDActionNameSource NameSource { get; set; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDDActionPosition * _Nullable position;
		[NullAllowed, Export ("position", ArgumentSemantic.Strong)]
		DDRUMActionEventDDActionPosition Position { get; }

		// @property (readonly, nonatomic, strong) DDRUMActionEventDDActionTarget * _Nullable target;
		[NullAllowed, Export ("target", ArgumentSemantic.Strong)]
		DDRUMActionEventDDActionTarget Target { get; }
	}

	// @interface DDRUMActionEventDDActionPosition
	[DisableDefaultCtor]
	interface DDRUMActionEventDDActionPosition
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull x;
		[Export ("x", ArgumentSemantic.Strong)]
		NSNumber X { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull y;
		[Export ("y", ArgumentSemantic.Strong)]
		NSNumber Y { get; }
	}

	// @interface DDRUMActionEventDDActionTarget
	[DisableDefaultCtor]
	interface DDRUMActionEventDDActionTarget
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable height;
		[NullAllowed, Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable selector;
		[NullAllowed, Export ("selector")]
		string Selector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable width;
		[NullAllowed, Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMActionEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMActionEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMActionEventDDSession
	[DisableDefaultCtor]
	interface DDRUMActionEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMActionEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMActionEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMActionEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMActionEventDevice
	[DisableDefaultCtor]
	interface DDRUMActionEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMActionEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMActionEventDisplay
	[DisableDefaultCtor]
	interface DDRUMActionEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMActionEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMActionEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMActionEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMActionEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMActionEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMActionEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMActionEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMActionEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMActionEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMActionEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMActionEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMActionEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMActionEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMActionEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMActionEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMEventAttributes
	{
	}

	// @interface DDRUMActionEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMActionEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMActionEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMActionEventSession
	[DisableDefaultCtor]
	interface DDRUMActionEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMActionEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMActionEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMActionEventStream
	[DisableDefaultCtor]
	interface DDRUMActionEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMActionEventView
	[DisableDefaultCtor]
	interface DDRUMActionEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable inForeground;
		[NullAllowed, Export ("inForeground", ArgumentSemantic.Strong)]
		NSNumber InForeground { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMConfiguration
	[DisableDefaultCtor]
	interface DDRUMConfiguration
	{
		// -(instancetype _Nonnull)initWithApplicationID:(NSString * _Nonnull)applicationID __attribute__((objc_designated_initializer));
		[Export ("initWithApplicationID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string applicationID);

		// @property (readonly, copy, nonatomic) NSString * _Nonnull applicationID;
		[Export ("applicationID")]
		string ApplicationID { get; }

		// @property (nonatomic) float sessionSampleRate;
		[Export ("sessionSampleRate")]
		float SessionSampleRate { get; set; }

		// @property (nonatomic) float telemetrySampleRate;
		[Export ("telemetrySampleRate")]
		float TelemetrySampleRate { get; set; }

		// @property (nonatomic, strong) id<DDUIKitRUMViewsPredicate> _Nullable uiKitViewsPredicate;
		[NullAllowed, Export ("uiKitViewsPredicate", ArgumentSemantic.Strong)]
		DDUIKitRUMViewsPredicate UiKitViewsPredicate { get; set; }

		// @property (nonatomic, strong) id<DDUIKitRUMActionsPredicate> _Nullable uiKitActionsPredicate;
		[NullAllowed, Export ("uiKitActionsPredicate", ArgumentSemantic.Strong)]
		DDUIKitRUMActionsPredicate UiKitActionsPredicate { get; set; }

		// @property (nonatomic, strong) id<DDSwiftUIRUMViewsPredicate> _Nullable swiftUIViewsPredicate;
		[NullAllowed, Export ("swiftUIViewsPredicate", ArgumentSemantic.Strong)]
		DDSwiftUIRUMViewsPredicate SwiftUIViewsPredicate { get; set; }

		// @property (nonatomic, strong) id<DDSwiftUIRUMActionsPredicate> _Nullable swiftUIActionsPredicate;
		[NullAllowed, Export ("swiftUIActionsPredicate", ArgumentSemantic.Strong)]
		DDSwiftUIRUMActionsPredicate SwiftUIActionsPredicate { get; set; }

		// -(void)setURLSessionTracking:(DDRUMURLSessionTracking * _Nonnull)tracking;
		[Export ("setURLSessionTracking:")]
		void SetURLSessionTracking (DDRUMURLSessionTracking tracking);

		// @property (nonatomic) int trackFrustrations;
		[Export ("trackFrustrations")]
		int TrackFrustrations { get; set; }

		// @property (nonatomic) int trackBackgroundEvents;
		[Export ("trackBackgroundEvents")]
		int TrackBackgroundEvents { get; set; }

		// @property (nonatomic) int trackWatchdogTerminations;
		[Export ("trackWatchdogTerminations")]
		int TrackWatchdogTerminations { get; set; }

		// @property (nonatomic) int longTaskThreshold;
		[Export ("longTaskThreshold")]
		int LongTaskThreshold { get; set; }

		// @property (nonatomic) int appHangThreshold;
		[Export ("appHangThreshold")]
		int AppHangThreshold { get; set; }

		// @property (nonatomic) enum DDRUMVitalsFrequency vitalsUpdateFrequency;
		[Export ("vitalsUpdateFrequency", ArgumentSemantic.Assign)]
		DDRUMVitalsFrequency VitalsUpdateFrequency { get; set; }

		// -(void)setViewEventMapper:(DDRUMViewEvent * _Nonnull (^ _Nonnull)(DDRUMViewEvent * _Nonnull))mapper;
		[Export ("setViewEventMapper:")]
		void SetViewEventMapper (Func<DDRUMViewEvent, DDRUMViewEvent> mapper);

		// -(void)setResourceEventMapper:(DDRUMResourceEvent * _Nullable (^ _Nonnull)(DDRUMResourceEvent * _Nonnull))mapper;
		[Export ("setResourceEventMapper:")]
		void SetResourceEventMapper (Func<DDRUMResourceEvent, DDRUMResourceEvent> mapper);

		// -(void)setActionEventMapper:(DDRUMActionEvent * _Nullable (^ _Nonnull)(DDRUMActionEvent * _Nonnull))mapper;
		[Export ("setActionEventMapper:")]
		void SetActionEventMapper (Func<DDRUMActionEvent, DDRUMActionEvent> mapper);

		// -(void)setErrorEventMapper:(DDRUMErrorEvent * _Nullable (^ _Nonnull)(DDRUMErrorEvent * _Nonnull))mapper;
		[Export ("setErrorEventMapper:")]
		void SetErrorEventMapper (Func<DDRUMErrorEvent, DDRUMErrorEvent> mapper);

		// -(void)setLongTaskEventMapper:(DDRUMLongTaskEvent * _Nullable (^ _Nonnull)(DDRUMLongTaskEvent * _Nonnull))mapper;
		[Export ("setLongTaskEventMapper:")]
		void SetLongTaskEventMapper (Func<DDRUMLongTaskEvent, DDRUMLongTaskEvent> mapper);

		// @property (copy, nonatomic) void (^ _Nullable)(NSString * _Nonnull, int) onSessionStart;
		[NullAllowed, Export ("onSessionStart", ArgumentSemantic.Copy)]
		Action<NSString, int> OnSessionStart { get; set; }

		// @property (copy, nonatomic) NSURL * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSURL CustomEndpoint { get; set; }

		// @property (nonatomic) int trackAnonymousUser;
		[Export ("trackAnonymousUser")]
		int TrackAnonymousUser { get; set; }

		// @property (nonatomic) int trackMemoryWarnings;
		[Export ("trackMemoryWarnings")]
		int TrackMemoryWarnings { get; set; }
	}

	// @interface DDRUMErrorEvent
	[DisableDefaultCtor]
	interface DDRUMErrorEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMErrorEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMErrorEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDRUMErrorEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMErrorEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMErrorEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMErrorEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMErrorEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventError * _Nonnull error;
		[Export ("error", ArgumentSemantic.Strong)]
		DDRUMErrorEventError Error { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventFeatureFlags * _Nullable featureFlags;
		[NullAllowed, Export ("featureFlags", ArgumentSemantic.Strong)]
		DDRUMErrorEventFeatureFlags FeatureFlags { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventFreeze * _Nullable freeze;
		[NullAllowed, Export ("freeze", ArgumentSemantic.Strong)]
		DDRUMErrorEventFreeze Freeze { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMErrorEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMErrorEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventSource source;
		[Export ("source")]
		DDRUMErrorEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMErrorEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMErrorEventView View { get; }
	}

	// @interface DDRUMErrorEventAction
	[DisableDefaultCtor]
	interface DDRUMErrorEventAction
	{
		// @property (readonly, nonatomic, strong) DDRUMErrorEventActionRUMActionID * _Nonnull id;
		[Export ("id", ArgumentSemantic.Strong)]
		DDRUMErrorEventActionRUMActionID Id { get; }
	}

	// @interface DDRUMErrorEventActionRUMActionID
	[DisableDefaultCtor]
	interface DDRUMErrorEventActionRUMActionID
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }
	}

	// @interface DDRUMErrorEventApplication
	[DisableDefaultCtor]
	interface DDRUMErrorEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMErrorEventContainer
	[DisableDefaultCtor]
	interface DDRUMErrorEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMErrorEventContainerSource source;
		[Export ("source")]
		DDRUMErrorEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMErrorEventContainerView View { get; }
	}

	// @interface DDRUMErrorEventContainerView
	[DisableDefaultCtor]
	interface DDRUMErrorEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMErrorEventDD
	[DisableDefaultCtor]
	interface DDRUMErrorEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMErrorEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMErrorEventDDSession Session { get; }
	}

	// @interface DDRUMErrorEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMErrorEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMErrorEventDDSession
	[DisableDefaultCtor]
	interface DDRUMErrorEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMErrorEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMErrorEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMErrorEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMErrorEventDevice
	[DisableDefaultCtor]
	interface DDRUMErrorEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMErrorEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMErrorEventDisplay
	[DisableDefaultCtor]
	interface DDRUMErrorEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMErrorEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMErrorEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMErrorEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMErrorEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMErrorEventError
	[DisableDefaultCtor]
	interface DDRUMErrorEventError
	{
		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorCategory category;
		[Export ("category")]
		DDRUMErrorEventErrorCategory Category { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventErrorCSP * _Nullable csp;
		[NullAllowed, Export ("csp", ArgumentSemantic.Strong)]
		DDRUMErrorEventErrorCSP Csp { get; }

		// @property (copy, nonatomic) NSString * _Nullable fingerprint;
		[NullAllowed, Export ("fingerprint")]
		string Fingerprint { get; set; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorHandling handling;
		[Export ("handling")]
		DDRUMErrorEventErrorHandling Handling { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable handlingStack;
		[NullAllowed, Export ("handlingStack")]
		string HandlingStack { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isCrash;
		[NullAllowed, Export ("isCrash", ArgumentSemantic.Strong)]
		NSNumber IsCrash { get; }

		// @property (copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; set; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventErrorMeta * _Nullable meta;
		[NullAllowed, Export ("meta", ArgumentSemantic.Strong)]
		DDRUMErrorEventErrorMeta Meta { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventErrorResource * _Nullable resource;
		[NullAllowed, Export ("resource", ArgumentSemantic.Strong)]
		DDRUMErrorEventErrorResource Resource { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorSource source;
		[Export ("source")]
		DDRUMErrorEventErrorSource Source { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorSourceType sourceType;
		[Export ("sourceType")]
		DDRUMErrorEventErrorSourceType SourceType { get; }

		// @property (copy, nonatomic) NSString * _Nullable stack;
		[NullAllowed, Export ("stack")]
		string Stack { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable timeSinceAppStart;
		[NullAllowed, Export ("timeSinceAppStart", ArgumentSemantic.Strong)]
		NSNumber TimeSinceAppStart { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable wasTruncated;
		[NullAllowed, Export ("wasTruncated", ArgumentSemantic.Strong)]
		NSNumber WasTruncated { get; }
	}

	// @interface DDRUMErrorEventErrorBinaryImages
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorBinaryImages
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable arch;
		[NullAllowed, Export ("arch")]
		string Arch { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull isSystem;
		[Export ("isSystem", ArgumentSemantic.Strong)]
		NSNumber IsSystem { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable loadAddress;
		[NullAllowed, Export ("loadAddress")]
		string LoadAddress { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable maxAddress;
		[NullAllowed, Export ("maxAddress")]
		string MaxAddress { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull uuid;
		[Export ("uuid")]
		string Uuid { get; }
	}

	// @interface DDRUMErrorEventErrorCSP
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorCSP
	{
		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorCSPDisposition disposition;
		[Export ("disposition")]
		DDRUMErrorEventErrorCSPDisposition Disposition { get; }
	}

	// @interface DDRUMErrorEventErrorCauses
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorCauses
	{
		// @property (copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; set; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorCausesSource source;
		[Export ("source")]
		DDRUMErrorEventErrorCausesSource Source { get; }

		// @property (copy, nonatomic) NSString * _Nullable stack;
		[NullAllowed, Export ("stack")]
		string Stack { get; set; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; }
	}

	// @interface DDRUMErrorEventErrorMeta
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorMeta
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable codeType;
		[NullAllowed, Export ("codeType")]
		string CodeType { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable exceptionCodes;
		[NullAllowed, Export ("exceptionCodes")]
		string ExceptionCodes { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable exceptionType;
		[NullAllowed, Export ("exceptionType")]
		string ExceptionType { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable incidentIdentifier;
		[NullAllowed, Export ("incidentIdentifier")]
		string IncidentIdentifier { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable parentProcess;
		[NullAllowed, Export ("parentProcess")]
		string ParentProcess { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable path;
		[NullAllowed, Export ("path")]
		string Path { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable process;
		[NullAllowed, Export ("process")]
		string Process { get; }
	}

	// @interface DDRUMErrorEventErrorResource
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorResource
	{
		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorResourceRUMMethod method;
		[Export ("method")]
		DDRUMErrorEventErrorResourceRUMMethod Method { get; }

		// @property (readonly, nonatomic, strong) DDRUMErrorEventErrorResourceProvider * _Nullable provider;
		[NullAllowed, Export ("provider", ArgumentSemantic.Strong)]
		DDRUMErrorEventErrorResourceProvider Provider { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull statusCode;
		[Export ("statusCode", ArgumentSemantic.Strong)]
		NSNumber StatusCode { get; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMErrorEventErrorResourceProvider
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorResourceProvider
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable domain;
		[NullAllowed, Export ("domain")]
		string Domain { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventErrorResourceProviderProviderType type;
		[Export ("type")]
		DDRUMErrorEventErrorResourceProviderProviderType Type { get; }
	}

	// @interface DDRUMErrorEventErrorThreads
	[DisableDefaultCtor]
	interface DDRUMErrorEventErrorThreads
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull crashed;
		[Export ("crashed", ArgumentSemantic.Strong)]
		NSNumber Crashed { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull stack;
		[Export ("stack")]
		string Stack { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable state;
		[NullAllowed, Export ("state")]
		string State { get; }
	}

	// @interface DDRUMErrorEventFeatureFlags
	[DisableDefaultCtor]
	interface DDRUMErrorEventFeatureFlags
	{
	}

	// @interface DDRUMErrorEventFreeze
	[DisableDefaultCtor]
	interface DDRUMErrorEventFreeze
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }
	}

	// @interface DDRUMErrorEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMErrorEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMErrorEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMErrorEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMErrorEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMErrorEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMErrorEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMErrorEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMErrorEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMErrorEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMErrorEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMEventAttributes
	{
	}

	// @interface DDRUMErrorEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMErrorEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMErrorEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMErrorEventSession
	[DisableDefaultCtor]
	interface DDRUMErrorEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMErrorEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMErrorEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMErrorEventStream
	[DisableDefaultCtor]
	interface DDRUMErrorEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMErrorEventView
	[DisableDefaultCtor]
	interface DDRUMErrorEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable inForeground;
		[NullAllowed, Export ("inForeground", ArgumentSemantic.Strong)]
		NSNumber InForeground { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMLongTaskEvent
	[DisableDefaultCtor]
	interface DDRUMLongTaskEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventLongTask * _Nonnull longTask;
		[Export ("longTask", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventLongTask LongTask { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventSource source;
		[Export ("source")]
		DDRUMLongTaskEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventView View { get; }
	}

	// @interface DDRUMLongTaskEventAction
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventAction
	{
		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventActionRUMActionID * _Nonnull id;
		[Export ("id", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventActionRUMActionID Id { get; }
	}

	// @interface DDRUMLongTaskEventActionRUMActionID
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventActionRUMActionID
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }
	}

	// @interface DDRUMLongTaskEventApplication
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMLongTaskEventContainer
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMLongTaskEventContainerSource source;
		[Export ("source")]
		DDRUMLongTaskEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventContainerView View { get; }
	}

	// @interface DDRUMLongTaskEventContainerView
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMLongTaskEventDD
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable discarded;
		[NullAllowed, Export ("discarded", ArgumentSemantic.Strong)]
		NSNumber Discarded { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDDProfiling * _Nullable profiling;
		[NullAllowed, Export ("profiling", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDDProfiling Profiling { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDDSession Session { get; }
	}

	// @interface DDRUMLongTaskEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMLongTaskEventDDProfiling
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDDProfiling
	{
		// @property (readonly, nonatomic) enum DDRUMLongTaskEventDDProfilingErrorReason errorReason;
		[Export ("errorReason")]
		DDRUMLongTaskEventDDProfilingErrorReason ErrorReason { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventDDProfilingStatus status;
		[Export ("status")]
		DDRUMLongTaskEventDDProfilingStatus Status { get; }
	}

	// @interface DDRUMLongTaskEventDDSession
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMLongTaskEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMLongTaskEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMLongTaskEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMLongTaskEventDevice
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMLongTaskEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMLongTaskEventDisplay
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMLongTaskEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMLongTaskEventLongTask
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventLongTask
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable blockingDuration;
		[NullAllowed, Export ("blockingDuration", ArgumentSemantic.Strong)]
		NSNumber BlockingDuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventLongTaskEntryType entryType;
		[Export ("entryType")]
		DDRUMLongTaskEventLongTaskEntryType EntryType { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable firstUiEventTimestamp;
		[NullAllowed, Export ("firstUiEventTimestamp", ArgumentSemantic.Strong)]
		NSNumber FirstUiEventTimestamp { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isFrozenFrame;
		[NullAllowed, Export ("isFrozenFrame", ArgumentSemantic.Strong)]
		NSNumber IsFrozenFrame { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable renderStart;
		[NullAllowed, Export ("renderStart", ArgumentSemantic.Strong)]
		NSNumber RenderStart { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable startTime;
		[NullAllowed, Export ("startTime", ArgumentSemantic.Strong)]
		NSNumber StartTime { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable styleAndLayoutStart;
		[NullAllowed, Export ("styleAndLayoutStart", ArgumentSemantic.Strong)]
		NSNumber StyleAndLayoutStart { get; }
	}

	// @interface DDRUMLongTaskEventLongTaskScripts
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventLongTaskScripts
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable duration;
		[NullAllowed, Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable executionStart;
		[NullAllowed, Export ("executionStart", ArgumentSemantic.Strong)]
		NSNumber ExecutionStart { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable forcedStyleAndLayoutDuration;
		[NullAllowed, Export ("forcedStyleAndLayoutDuration", ArgumentSemantic.Strong)]
		NSNumber ForcedStyleAndLayoutDuration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable invoker;
		[NullAllowed, Export ("invoker")]
		string Invoker { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventLongTaskScriptsInvokerType invokerType;
		[Export ("invokerType")]
		DDRUMLongTaskEventLongTaskScriptsInvokerType InvokerType { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable pauseDuration;
		[NullAllowed, Export ("pauseDuration", ArgumentSemantic.Strong)]
		NSNumber PauseDuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sourceCharPosition;
		[NullAllowed, Export ("sourceCharPosition", ArgumentSemantic.Strong)]
		NSNumber SourceCharPosition { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sourceFunctionName;
		[NullAllowed, Export ("sourceFunctionName")]
		string SourceFunctionName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sourceUrl;
		[NullAllowed, Export ("sourceUrl")]
		string SourceUrl { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable startTime;
		[NullAllowed, Export ("startTime", ArgumentSemantic.Strong)]
		NSNumber StartTime { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable windowAttribution;
		[NullAllowed, Export ("windowAttribution")]
		string WindowAttribution { get; }
	}

	// @interface DDRUMLongTaskEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMLongTaskEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMLongTaskEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMLongTaskEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMLongTaskEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMLongTaskEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMLongTaskEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMLongTaskEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMLongTaskEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMLongTaskEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMEventAttributes
	{
	}

	// @interface DDRUMLongTaskEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMLongTaskEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMLongTaskEventSession
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMLongTaskEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMLongTaskEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMLongTaskEventStream
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMLongTaskEventView
	[DisableDefaultCtor]
	interface DDRUMLongTaskEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMMonitor
	[DisableDefaultCtor]
	interface DDRUMMonitor
	{
		// +(DDRUMMonitor * _Nonnull)shared __attribute__((warn_unused_result("")));
		[Static]
		[Export ("shared")]
		[Verify (MethodToProperty)]
		DDRUMMonitor Shared { get; }

		// -(void)currentSessionIDWithCompletion:(void (^ _Nonnull)(NSString * _Nullable))completion;
		[Export ("currentSessionIDWithCompletion:")]
		void CurrentSessionIDWithCompletion (Action<NSString> completion);

		// -(void)stopSession;
		[Export ("stopSession")]
		void StopSession ();

		// -(void)reportAppFullyDisplayed;
		[Export ("reportAppFullyDisplayed")]
		void ReportAppFullyDisplayed ();

		// -(void)addViewAttributeForKey:(NSString * _Nonnull)key value:(id _Nonnull)value;
		[Export ("addViewAttributeForKey:value:")]
		void AddViewAttributeForKey (string key, NSObject value);

		// -(void)addViewAttributes:(id)attributes;
		[Export ("addViewAttributes:")]
		void AddViewAttributes (NSObject attributes);

		// -(void)removeViewAttributeForKey:(NSString * _Nonnull)key;
		[Export ("removeViewAttributeForKey:")]
		void RemoveViewAttributeForKey (string key);

		// -(void)removeViewAttributesForKeys:(id)keys;
		[Export ("removeViewAttributesForKeys:")]
		void RemoveViewAttributesForKeys (NSObject keys);

		// -(void)startViewWithViewController:(UIViewController * _Nonnull)viewController name:(NSString * _Nullable)name attributes:(id)attributes;
		[Export ("startViewWithViewController:name:attributes:")]
		void StartViewWithViewController (UIViewController viewController, [NullAllowed] string name, NSObject attributes);

		// -(void)stopViewWithViewController:(UIViewController * _Nonnull)viewController attributes:(id)attributes;
		[Export ("stopViewWithViewController:attributes:")]
		void StopViewWithViewController (UIViewController viewController, NSObject attributes);

		// -(void)startViewWithKey:(NSString * _Nonnull)key name:(NSString * _Nullable)name attributes:(id)attributes;
		[Export ("startViewWithKey:name:attributes:")]
		void StartViewWithKey (string key, [NullAllowed] string name, NSObject attributes);

		// -(void)stopViewWithKey:(NSString * _Nonnull)key attributes:(id)attributes;
		[Export ("stopViewWithKey:attributes:")]
		void StopViewWithKey (string key, NSObject attributes);

		// -(void)addViewLoadingTimeWithOverwrite:(id)overwrite;
		[Export ("addViewLoadingTimeWithOverwrite:")]
		void AddViewLoadingTimeWithOverwrite (NSObject overwrite);

		// -(void)addTimingWithName:(NSString * _Nonnull)name;
		[Export ("addTimingWithName:")]
		void AddTimingWithName (string name);

		// -(void)addErrorWithMessage:(NSString * _Nonnull)message stack:(NSString * _Nullable)stack source:(enum DDRUMErrorSource)source attributes:(id)attributes;
		[Export ("addErrorWithMessage:stack:source:attributes:")]
		void AddErrorWithMessage (string message, [NullAllowed] string stack, DDRUMErrorSource source, NSObject attributes);

		// -(void)addErrorWithError:(id)error source:(enum DDRUMErrorSource)source attributes:(id)attributes;
		[Export ("addErrorWithError:source:attributes:")]
		void AddErrorWithError (NSObject error, DDRUMErrorSource source, NSObject attributes);

		// -(void)startResourceWithResourceKey:(NSString * _Nonnull)resourceKey request:(NSURLRequest * _Nonnull)request attributes:(id)attributes;
		[Export ("startResourceWithResourceKey:request:attributes:")]
		void StartResourceWithResourceKey (string resourceKey, NSURLRequest request, NSObject attributes);

		// -(void)startResourceWithResourceKey:(NSString * _Nonnull)resourceKey url:(NSURL * _Nonnull)url attributes:(id)attributes;
		[Export ("startResourceWithResourceKey:url:attributes:")]
		void StartResourceWithResourceKey (string resourceKey, NSURL url, NSObject attributes);

		// -(void)startResourceWithResourceKey:(NSString * _Nonnull)resourceKey httpMethod:(enum DDRUMMethod)httpMethod urlString:(NSString * _Nonnull)urlString attributes:(id)attributes;
		[Export ("startResourceWithResourceKey:httpMethod:urlString:attributes:")]
		void StartResourceWithResourceKey (string resourceKey, DDRUMMethod httpMethod, string urlString, NSObject attributes);

		// -(void)addResourceMetricsWithResourceKey:(NSString * _Nonnull)resourceKey metrics:(NSURLSessionTaskMetrics * _Nonnull)metrics attributes:(id)attributes;
		[Export ("addResourceMetricsWithResourceKey:metrics:attributes:")]
		void AddResourceMetricsWithResourceKey (string resourceKey, NSURLSessionTaskMetrics metrics, NSObject attributes);

		// -(void)stopResourceWithResourceKey:(NSString * _Nonnull)resourceKey response:(NSURLResponse * _Nonnull)response size:(NSNumber * _Nullable)size attributes:(id)attributes;
		[Export ("stopResourceWithResourceKey:response:size:attributes:")]
		void StopResourceWithResourceKey (string resourceKey, NSURLResponse response, [NullAllowed] NSNumber size, NSObject attributes);

		// -(void)stopResourceWithResourceKey:(NSString * _Nonnull)resourceKey statusCode:(NSNumber * _Nullable)statusCode kind:(enum DDRUMResourceType)kind size:(NSNumber * _Nullable)size attributes:(id)attributes;
		[Export ("stopResourceWithResourceKey:statusCode:kind:size:attributes:")]
		void StopResourceWithResourceKey (string resourceKey, [NullAllowed] NSNumber statusCode, DDRUMResourceType kind, [NullAllowed] NSNumber size, NSObject attributes);

		// -(void)stopResourceWithErrorWithResourceKey:(NSString * _Nonnull)resourceKey error:(id)error response:(NSURLResponse * _Nullable)response attributes:(id)attributes;
		[Export ("stopResourceWithErrorWithResourceKey:error:response:attributes:")]
		void StopResourceWithErrorWithResourceKey (string resourceKey, NSObject error, [NullAllowed] NSURLResponse response, NSObject attributes);

		// -(void)stopResourceWithErrorWithResourceKey:(NSString * _Nonnull)resourceKey message:(NSString * _Nonnull)message response:(NSURLResponse * _Nullable)response attributes:(id)attributes;
		[Export ("stopResourceWithErrorWithResourceKey:message:response:attributes:")]
		void StopResourceWithErrorWithResourceKey (string resourceKey, string message, [NullAllowed] NSURLResponse response, NSObject attributes);

		// -(void)startActionWithType:(enum DDRUMActionType)type name:(NSString * _Nonnull)name attributes:(id)attributes;
		[Export ("startActionWithType:name:attributes:")]
		void StartActionWithType (DDRUMActionType type, string name, NSObject attributes);

		// -(void)stopActionWithType:(enum DDRUMActionType)type name:(NSString * _Nullable)name attributes:(id)attributes;
		[Export ("stopActionWithType:name:attributes:")]
		void StopActionWithType (DDRUMActionType type, [NullAllowed] string name, NSObject attributes);

		// -(void)addActionWithType:(enum DDRUMActionType)type name:(NSString * _Nonnull)name attributes:(id)attributes;
		[Export ("addActionWithType:name:attributes:")]
		void AddActionWithType (DDRUMActionType type, string name, NSObject attributes);

		// -(void)addAttributeForKey:(NSString * _Nonnull)key value:(id _Nonnull)value;
		[Export ("addAttributeForKey:value:")]
		void AddAttributeForKey (string key, NSObject value);

		// -(void)addAttributes:(id)attributes;
		[Export ("addAttributes:")]
		void AddAttributes (NSObject attributes);

		// -(void)removeAttributeForKey:(NSString * _Nonnull)key;
		[Export ("removeAttributeForKey:")]
		void RemoveAttributeForKey (string key);

		// -(void)removeAttributesForKeys:(id)keys;
		[Export ("removeAttributesForKeys:")]
		void RemoveAttributesForKeys (NSObject keys);

		// -(void)addFeatureFlagEvaluationWithName:(NSString * _Nonnull)name value:(id _Nonnull)value;
		[Export ("addFeatureFlagEvaluationWithName:value:")]
		void AddFeatureFlagEvaluationWithName (string name, NSObject value);

		// -(void)startFeatureOperationWithName:(NSString * _Nonnull)name operationKey:(NSString * _Nullable)operationKey attributes:(id)attributes;
		[Export ("startFeatureOperationWithName:operationKey:attributes:")]
		void StartFeatureOperationWithName (string name, [NullAllowed] string operationKey, NSObject attributes);

		// -(void)succeedFeatureOperationWithName:(NSString * _Nonnull)name operationKey:(NSString * _Nullable)operationKey attributes:(id)attributes;
		[Export ("succeedFeatureOperationWithName:operationKey:attributes:")]
		void SucceedFeatureOperationWithName (string name, [NullAllowed] string operationKey, NSObject attributes);

		// -(void)failFeatureOperationWithName:(NSString * _Nonnull)name operationKey:(NSString * _Nullable)operationKey reason:(enum DDRUMFeatureOperationFailureReason)reason attributes:(id)attributes;
		[Export ("failFeatureOperationWithName:operationKey:reason:attributes:")]
		void FailFeatureOperationWithName (string name, [NullAllowed] string operationKey, DDRUMFeatureOperationFailureReason reason, NSObject attributes);

		// @property (nonatomic) int debug;
		[Export ("debug")]
		int Debug { get; set; }
	}

	// @interface DatadogRUM_Swift_6919 (DDRUMMonitor)
	[Category]
	[BaseType (typeof(DDRUMMonitor))]
	interface DDRUMMonitor_DatadogRUM_Swift_6919
	{
		// -(void)_internal_sync_addError:(id)error source:(enum DDRUMErrorSource)source attributes:(id)attributes;
		[Export ("_internal_sync_addError:source:attributes:")]
		void _internal_sync_addError (NSObject error, DDRUMErrorSource source, NSObject attributes);
	}

	// @interface DDRUMResourceEvent
	[DisableDefaultCtor]
	interface DDRUMResourceEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMResourceEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMResourceEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDRUMResourceEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMResourceEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMResourceEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMResourceEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMResourceEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMResourceEventOperatingSystem Os { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResource * _Nonnull resource;
		[Export ("resource", ArgumentSemantic.Strong)]
		DDRUMResourceEventResource Resource { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMResourceEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventSource source;
		[Export ("source")]
		DDRUMResourceEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMResourceEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMResourceEventView View { get; }
	}

	// @interface DDRUMResourceEventAction
	[DisableDefaultCtor]
	interface DDRUMResourceEventAction
	{
		// @property (readonly, nonatomic, strong) DDRUMResourceEventActionRUMActionID * _Nonnull id;
		[Export ("id", ArgumentSemantic.Strong)]
		DDRUMResourceEventActionRUMActionID Id { get; }
	}

	// @interface DDRUMResourceEventActionRUMActionID
	[DisableDefaultCtor]
	interface DDRUMResourceEventActionRUMActionID
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }
	}

	// @interface DDRUMResourceEventApplication
	[DisableDefaultCtor]
	interface DDRUMResourceEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMResourceEventContainer
	[DisableDefaultCtor]
	interface DDRUMResourceEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMResourceEventContainerSource source;
		[Export ("source")]
		DDRUMResourceEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMResourceEventContainerView View { get; }
	}

	// @interface DDRUMResourceEventContainerView
	[DisableDefaultCtor]
	interface DDRUMResourceEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMResourceEventDD
	[DisableDefaultCtor]
	interface DDRUMResourceEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMResourceEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable discarded;
		[NullAllowed, Export ("discarded", ArgumentSemantic.Strong)]
		NSNumber Discarded { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable parentSpanId;
		[NullAllowed, Export ("parentSpanId")]
		string ParentSpanId { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable rulePsr;
		[NullAllowed, Export ("rulePsr", ArgumentSemantic.Strong)]
		NSNumber RulePsr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMResourceEventDDSession Session { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable spanId;
		[NullAllowed, Export ("spanId")]
		string SpanId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable traceId;
		[NullAllowed, Export ("traceId")]
		string TraceId { get; }
	}

	// @interface DDRUMResourceEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMResourceEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMResourceEventDDSession
	[DisableDefaultCtor]
	interface DDRUMResourceEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMResourceEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMResourceEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMResourceEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMResourceEventDevice
	[DisableDefaultCtor]
	interface DDRUMResourceEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMResourceEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMResourceEventDisplay
	[DisableDefaultCtor]
	interface DDRUMResourceEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMResourceEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMResourceEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMResourceEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMResourceEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMResourceEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMResourceEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMResourceEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMResourceEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMResourceEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMResourceEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMResourceEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMResourceEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMResourceEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMResourceEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMResourceEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMEventAttributes
	{
	}

	// @interface DDRUMResourceEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMResourceEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMResourceEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMResourceEventResource
	[DisableDefaultCtor]
	interface DDRUMResourceEventResource
	{
		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceConnect * _Nullable connect;
		[NullAllowed, Export ("connect", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceConnect Connect { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable decodedBodySize;
		[NullAllowed, Export ("decodedBodySize", ArgumentSemantic.Strong)]
		NSNumber DecodedBodySize { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceDeliveryType deliveryType;
		[Export ("deliveryType")]
		DDRUMResourceEventResourceDeliveryType DeliveryType { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceDNS * _Nullable dns;
		[NullAllowed, Export ("dns", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceDNS Dns { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceDownload * _Nullable download;
		[NullAllowed, Export ("download", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceDownload Download { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable duration;
		[NullAllowed, Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable encodedBodySize;
		[NullAllowed, Export ("encodedBodySize", ArgumentSemantic.Strong)]
		NSNumber EncodedBodySize { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceFirstByte * _Nullable firstByte;
		[NullAllowed, Export ("firstByte", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceFirstByte FirstByte { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceGraphql * _Nullable graphql;
		[NullAllowed, Export ("graphql", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceGraphql Graphql { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceRUMMethod method;
		[Export ("method")]
		DDRUMResourceEventResourceRUMMethod Method { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable protocol;
		[NullAllowed, Export ("protocol")]
		string Protocol { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceProvider * _Nullable provider;
		[NullAllowed, Export ("provider", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceProvider Provider { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceRedirect * _Nullable redirect;
		[NullAllowed, Export ("redirect", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceRedirect Redirect { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceRenderBlockingStatus renderBlockingStatus;
		[Export ("renderBlockingStatus")]
		DDRUMResourceEventResourceRenderBlockingStatus RenderBlockingStatus { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable size;
		[NullAllowed, Export ("size", ArgumentSemantic.Strong)]
		NSNumber Size { get; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceSSL * _Nullable ssl;
		[NullAllowed, Export ("ssl", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceSSL Ssl { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable statusCode;
		[NullAllowed, Export ("statusCode", ArgumentSemantic.Strong)]
		NSNumber StatusCode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable transferSize;
		[NullAllowed, Export ("transferSize", ArgumentSemantic.Strong)]
		NSNumber TransferSize { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceResourceType type;
		[Export ("type")]
		DDRUMResourceEventResourceResourceType Type { get; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }

		// @property (readonly, nonatomic, strong) DDRUMResourceEventResourceWorker * _Nullable worker;
		[NullAllowed, Export ("worker", ArgumentSemantic.Strong)]
		DDRUMResourceEventResourceWorker Worker { get; }
	}

	// @interface DDRUMResourceEventResourceConnect
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceConnect
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceDNS
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceDNS
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceDownload
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceDownload
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceFirstByte
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceFirstByte
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceGraphql
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceGraphql
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable errorCount;
		[NullAllowed, Export ("errorCount", ArgumentSemantic.Strong)]
		NSNumber ErrorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable operationName;
		[NullAllowed, Export ("operationName")]
		string OperationName { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceGraphqlOperationType operationType;
		[Export ("operationType")]
		DDRUMResourceEventResourceGraphqlOperationType OperationType { get; }

		// @property (copy, nonatomic) NSString * _Nullable payload;
		[NullAllowed, Export ("payload")]
		string Payload { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable variables;
		[NullAllowed, Export ("variables")]
		string Variables { get; set; }
	}

	// @interface DDRUMResourceEventResourceGraphqlErrors
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceGraphqlErrors
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable code;
		[NullAllowed, Export ("code")]
		string Code { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; }
	}

	// @interface DDRUMResourceEventResourceGraphqlErrorsLocations
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceGraphqlErrorsLocations
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull column;
		[Export ("column", ArgumentSemantic.Strong)]
		NSNumber Column { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull line;
		[Export ("line", ArgumentSemantic.Strong)]
		NSNumber Line { get; }
	}

	// @interface DDRUMResourceEventResourceGraphqlErrorsPath
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceGraphqlErrorsPath
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable integer;
		[NullAllowed, Export ("integer", ArgumentSemantic.Strong)]
		NSNumber Integer { get; }
	}

	// @interface DDRUMResourceEventResourceProvider
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceProvider
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable domain;
		[NullAllowed, Export ("domain")]
		string Domain { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventResourceProviderProviderType type;
		[Export ("type")]
		DDRUMResourceEventResourceProviderProviderType Type { get; }
	}

	// @interface DDRUMResourceEventResourceRedirect
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceRedirect
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceSSL
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceSSL
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventResourceWorker
	[DisableDefaultCtor]
	interface DDRUMResourceEventResourceWorker
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMResourceEventSession
	[DisableDefaultCtor]
	interface DDRUMResourceEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMResourceEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMResourceEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMResourceEventStream
	[DisableDefaultCtor]
	interface DDRUMResourceEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMResourceEventView
	[DisableDefaultCtor]
	interface DDRUMResourceEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMView
	[DisableDefaultCtor]
	interface DDRUMView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// -(instancetype _Nonnull)initWithName:(NSString * _Nonnull)name attributes:(id)attributes __attribute__((objc_designated_initializer));
		[Export ("initWithName:attributes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSObject attributes);
	}

	// @interface DDRUMViewEvent
	[DisableDefaultCtor]
	interface DDRUMViewEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMViewEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMViewEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMViewEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMViewEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMViewEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventFeatureFlags * _Nullable featureFlags;
		[NullAllowed, Export ("featureFlags", ArgumentSemantic.Strong)]
		DDRUMViewEventFeatureFlags FeatureFlags { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMViewEventOperatingSystem Os { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventPrivacy * _Nullable privacy;
		[NullAllowed, Export ("privacy", ArgumentSemantic.Strong)]
		DDRUMViewEventPrivacy Privacy { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMViewEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventSource source;
		[Export ("source")]
		DDRUMViewEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMViewEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMViewEventView View { get; }
	}

	// @interface DDRUMViewEventApplication
	[DisableDefaultCtor]
	interface DDRUMViewEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMViewEventContainer
	[DisableDefaultCtor]
	interface DDRUMViewEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMViewEventContainerSource source;
		[Export ("source")]
		DDRUMViewEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMViewEventContainerView View { get; }
	}

	// @interface DDRUMViewEventContainerView
	[DisableDefaultCtor]
	interface DDRUMViewEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMViewEventDD
	[DisableDefaultCtor]
	interface DDRUMViewEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDDCLS * _Nullable cls;
		[NullAllowed, Export ("cls", ArgumentSemantic.Strong)]
		DDRUMViewEventDDCLS Cls { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMViewEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull documentVersion;
		[Export ("documentVersion", ArgumentSemantic.Strong)]
		NSNumber DocumentVersion { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDDProfiling * _Nullable profiling;
		[NullAllowed, Export ("profiling", ArgumentSemantic.Strong)]
		DDRUMViewEventDDProfiling Profiling { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDDReplayStats * _Nullable replayStats;
		[NullAllowed, Export ("replayStats", ArgumentSemantic.Strong)]
		DDRUMViewEventDDReplayStats ReplayStats { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMViewEventDDSession Session { get; }
	}

	// @interface DDRUMViewEventDDCLS
	[DisableDefaultCtor]
	interface DDRUMViewEventDDCLS
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable devicePixelRatio;
		[NullAllowed, Export ("devicePixelRatio", ArgumentSemantic.Strong)]
		NSNumber DevicePixelRatio { get; }
	}

	// @interface DDRUMViewEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMViewEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable startSessionReplayRecordingManually;
		[NullAllowed, Export ("startSessionReplayRecordingManually", ArgumentSemantic.Strong)]
		NSNumber StartSessionReplayRecordingManually { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMViewEventDDPageStates
	[DisableDefaultCtor]
	interface DDRUMViewEventDDPageStates
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventDDPageStatesState state;
		[Export ("state")]
		DDRUMViewEventDDPageStatesState State { get; }
	}

	// @interface DDRUMViewEventDDProfiling
	[DisableDefaultCtor]
	interface DDRUMViewEventDDProfiling
	{
		// @property (readonly, nonatomic) enum DDRUMViewEventDDProfilingErrorReason errorReason;
		[Export ("errorReason")]
		DDRUMViewEventDDProfilingErrorReason ErrorReason { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventDDProfilingStatus status;
		[Export ("status")]
		DDRUMViewEventDDProfilingStatus Status { get; }
	}

	// @interface DDRUMViewEventDDReplayStats
	[DisableDefaultCtor]
	interface DDRUMViewEventDDReplayStats
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable recordsCount;
		[NullAllowed, Export ("recordsCount", ArgumentSemantic.Strong)]
		NSNumber RecordsCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable segmentsCount;
		[NullAllowed, Export ("segmentsCount", ArgumentSemantic.Strong)]
		NSNumber SegmentsCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable segmentsTotalRawSize;
		[NullAllowed, Export ("segmentsTotalRawSize", ArgumentSemantic.Strong)]
		NSNumber SegmentsTotalRawSize { get; }
	}

	// @interface DDRUMViewEventDDSession
	[DisableDefaultCtor]
	interface DDRUMViewEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMViewEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMViewEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMViewEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMViewEventDevice
	[DisableDefaultCtor]
	interface DDRUMViewEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMViewEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMViewEventDisplay
	[DisableDefaultCtor]
	interface DDRUMViewEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventDisplayScroll * _Nullable scroll;
		[NullAllowed, Export ("scroll", ArgumentSemantic.Strong)]
		DDRUMViewEventDisplayScroll Scroll { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMViewEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMViewEventDisplayScroll
	[DisableDefaultCtor]
	interface DDRUMViewEventDisplayScroll
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull maxDepth;
		[Export ("maxDepth", ArgumentSemantic.Strong)]
		NSNumber MaxDepth { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull maxDepthScrollTop;
		[Export ("maxDepthScrollTop", ArgumentSemantic.Strong)]
		NSNumber MaxDepthScrollTop { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull maxScrollHeight;
		[Export ("maxScrollHeight", ArgumentSemantic.Strong)]
		NSNumber MaxScrollHeight { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull maxScrollHeightTime;
		[Export ("maxScrollHeightTime", ArgumentSemantic.Strong)]
		NSNumber MaxScrollHeightTime { get; }
	}

	// @interface DDRUMViewEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMViewEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMViewEventFeatureFlags
	[DisableDefaultCtor]
	interface DDRUMViewEventFeatureFlags
	{
	}

	// @interface DDRUMViewEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMViewEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMViewEventPrivacy
	[DisableDefaultCtor]
	interface DDRUMViewEventPrivacy
	{
		// @property (readonly, nonatomic) enum DDRUMViewEventPrivacyReplayLevel replayLevel;
		[Export ("replayLevel")]
		DDRUMViewEventPrivacyReplayLevel ReplayLevel { get; }
	}

	// @interface DDRUMViewEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMViewEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMViewEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMViewEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMViewEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMViewEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMViewEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMViewEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMEventAttributes
	{
	}

	// @interface DDRUMViewEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMViewEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMViewEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMViewEventSession
	[DisableDefaultCtor]
	interface DDRUMViewEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isActive;
		[NullAllowed, Export ("isActive", ArgumentSemantic.Strong)]
		NSNumber IsActive { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sampledForReplay;
		[NullAllowed, Export ("sampledForReplay", ArgumentSemantic.Strong)]
		NSNumber SampledForReplay { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMViewEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMViewEventStream
	[DisableDefaultCtor]
	interface DDRUMViewEventStream
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable bitrate;
		[NullAllowed, Export ("bitrate", ArgumentSemantic.Strong)]
		NSNumber Bitrate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable completionPercent;
		[NullAllowed, Export ("completionPercent", ArgumentSemantic.Strong)]
		NSNumber CompletionPercent { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable duration;
		[NullAllowed, Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable format;
		[NullAllowed, Export ("format")]
		string Format { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable fps;
		[NullAllowed, Export ("fps", ArgumentSemantic.Strong)]
		NSNumber Fps { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable resolution;
		[NullAllowed, Export ("resolution")]
		string Resolution { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable timestamp;
		[NullAllowed, Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable watchTime;
		[NullAllowed, Export ("watchTime", ArgumentSemantic.Strong)]
		NSNumber WatchTime { get; }
	}

	// @interface DDRUMViewEventView
	[DisableDefaultCtor]
	interface DDRUMViewEventView
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventViewAccessibility * _Nullable accessibility;
		[NullAllowed, Export ("accessibility", ArgumentSemantic.Strong)]
		DDRUMViewEventViewAccessibility Accessibility { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewAction * _Nonnull action;
		[Export ("action", ArgumentSemantic.Strong)]
		DDRUMViewEventViewAction Action { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable cpuTicksCount;
		[NullAllowed, Export ("cpuTicksCount", ArgumentSemantic.Strong)]
		NSNumber CpuTicksCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable cpuTicksPerSecond;
		[NullAllowed, Export ("cpuTicksPerSecond", ArgumentSemantic.Strong)]
		NSNumber CpuTicksPerSecond { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewCrash * _Nullable crash;
		[NullAllowed, Export ("crash", ArgumentSemantic.Strong)]
		DDRUMViewEventViewCrash Crash { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable cumulativeLayoutShift;
		[NullAllowed, Export ("cumulativeLayoutShift", ArgumentSemantic.Strong)]
		NSNumber CumulativeLayoutShift { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable cumulativeLayoutShiftTargetSelector;
		[NullAllowed, Export ("cumulativeLayoutShiftTargetSelector")]
		string CumulativeLayoutShiftTargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable cumulativeLayoutShiftTime;
		[NullAllowed, Export ("cumulativeLayoutShiftTime", ArgumentSemantic.Strong)]
		NSNumber CumulativeLayoutShiftTime { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewCustomTimings * _Nullable customTimings;
		[NullAllowed, Export ("customTimings", ArgumentSemantic.Strong)]
		DDRUMViewEventViewCustomTimings CustomTimings { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable domComplete;
		[NullAllowed, Export ("domComplete", ArgumentSemantic.Strong)]
		NSNumber DomComplete { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable domContentLoaded;
		[NullAllowed, Export ("domContentLoaded", ArgumentSemantic.Strong)]
		NSNumber DomContentLoaded { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable domInteractive;
		[NullAllowed, Export ("domInteractive", ArgumentSemantic.Strong)]
		NSNumber DomInteractive { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewError * _Nonnull error;
		[Export ("error", ArgumentSemantic.Strong)]
		DDRUMViewEventViewError Error { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable firstByte;
		[NullAllowed, Export ("firstByte", ArgumentSemantic.Strong)]
		NSNumber FirstByte { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable firstContentfulPaint;
		[NullAllowed, Export ("firstContentfulPaint", ArgumentSemantic.Strong)]
		NSNumber FirstContentfulPaint { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable firstInputDelay;
		[NullAllowed, Export ("firstInputDelay", ArgumentSemantic.Strong)]
		NSNumber FirstInputDelay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable firstInputTargetSelector;
		[NullAllowed, Export ("firstInputTargetSelector")]
		string FirstInputTargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable firstInputTime;
		[NullAllowed, Export ("firstInputTime", ArgumentSemantic.Strong)]
		NSNumber FirstInputTime { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewFlutterBuildTime * _Nullable flutterBuildTime;
		[NullAllowed, Export ("flutterBuildTime", ArgumentSemantic.Strong)]
		DDRUMViewEventViewFlutterBuildTime FlutterBuildTime { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewFlutterRasterTime * _Nullable flutterRasterTime;
		[NullAllowed, Export ("flutterRasterTime", ArgumentSemantic.Strong)]
		DDRUMViewEventViewFlutterRasterTime FlutterRasterTime { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable freezeRate;
		[NullAllowed, Export ("freezeRate", ArgumentSemantic.Strong)]
		NSNumber FreezeRate { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewFrozenFrame * _Nullable frozenFrame;
		[NullAllowed, Export ("frozenFrame", ArgumentSemantic.Strong)]
		DDRUMViewEventViewFrozenFrame FrozenFrame { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewFrustration * _Nullable frustration;
		[NullAllowed, Export ("frustration", ArgumentSemantic.Strong)]
		DDRUMViewEventViewFrustration Frustration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable interactionToNextPaint;
		[NullAllowed, Export ("interactionToNextPaint", ArgumentSemantic.Strong)]
		NSNumber InteractionToNextPaint { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable interactionToNextPaintTargetSelector;
		[NullAllowed, Export ("interactionToNextPaintTargetSelector")]
		string InteractionToNextPaintTargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable interactionToNextPaintTime;
		[NullAllowed, Export ("interactionToNextPaintTime", ArgumentSemantic.Strong)]
		NSNumber InteractionToNextPaintTime { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable interactionToNextViewTime;
		[NullAllowed, Export ("interactionToNextViewTime", ArgumentSemantic.Strong)]
		NSNumber InteractionToNextViewTime { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isActive;
		[NullAllowed, Export ("isActive", ArgumentSemantic.Strong)]
		NSNumber IsActive { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isSlowRendered;
		[NullAllowed, Export ("isSlowRendered", ArgumentSemantic.Strong)]
		NSNumber IsSlowRendered { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewJsRefreshRate * _Nullable jsRefreshRate;
		[NullAllowed, Export ("jsRefreshRate", ArgumentSemantic.Strong)]
		DDRUMViewEventViewJsRefreshRate JsRefreshRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable largestContentfulPaint;
		[NullAllowed, Export ("largestContentfulPaint", ArgumentSemantic.Strong)]
		NSNumber LargestContentfulPaint { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable largestContentfulPaintTargetSelector;
		[NullAllowed, Export ("largestContentfulPaintTargetSelector")]
		string LargestContentfulPaintTargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable loadEvent;
		[NullAllowed, Export ("loadEvent", ArgumentSemantic.Strong)]
		NSNumber LoadEvent { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable loadingTime;
		[NullAllowed, Export ("loadingTime", ArgumentSemantic.Strong)]
		NSNumber LoadingTime { get; }

		// @property (readonly, nonatomic) enum DDRUMViewEventViewLoadingType loadingType;
		[Export ("loadingType")]
		DDRUMViewEventViewLoadingType LoadingType { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewLongTask * _Nullable longTask;
		[NullAllowed, Export ("longTask", ArgumentSemantic.Strong)]
		DDRUMViewEventViewLongTask LongTask { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable memoryAverage;
		[NullAllowed, Export ("memoryAverage", ArgumentSemantic.Strong)]
		NSNumber MemoryAverage { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable memoryMax;
		[NullAllowed, Export ("memoryMax", ArgumentSemantic.Strong)]
		NSNumber MemoryMax { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable networkSettledTime;
		[NullAllowed, Export ("networkSettledTime", ArgumentSemantic.Strong)]
		NSNumber NetworkSettledTime { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformance * _Nullable performance;
		[NullAllowed, Export ("performance", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformance Performance { get; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable refreshRateAverage;
		[NullAllowed, Export ("refreshRateAverage", ArgumentSemantic.Strong)]
		NSNumber RefreshRateAverage { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable refreshRateMin;
		[NullAllowed, Export ("refreshRateMin", ArgumentSemantic.Strong)]
		NSNumber RefreshRateMin { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewResource * _Nonnull resource;
		[Export ("resource", ArgumentSemantic.Strong)]
		DDRUMViewEventViewResource Resource { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable slowFramesRate;
		[NullAllowed, Export ("slowFramesRate", ArgumentSemantic.Strong)]
		NSNumber SlowFramesRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull timeSpent;
		[Export ("timeSpent", ArgumentSemantic.Strong)]
		NSNumber TimeSpent { get; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMViewEventViewAccessibility
	[DisableDefaultCtor]
	interface DDRUMViewEventViewAccessibility
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable assistiveSwitchEnabled;
		[NullAllowed, Export ("assistiveSwitchEnabled", ArgumentSemantic.Strong)]
		NSNumber AssistiveSwitchEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable assistiveTouchEnabled;
		[NullAllowed, Export ("assistiveTouchEnabled", ArgumentSemantic.Strong)]
		NSNumber AssistiveTouchEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable boldTextEnabled;
		[NullAllowed, Export ("boldTextEnabled", ArgumentSemantic.Strong)]
		NSNumber BoldTextEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable buttonShapesEnabled;
		[NullAllowed, Export ("buttonShapesEnabled", ArgumentSemantic.Strong)]
		NSNumber ButtonShapesEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable closedCaptioningEnabled;
		[NullAllowed, Export ("closedCaptioningEnabled", ArgumentSemantic.Strong)]
		NSNumber ClosedCaptioningEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable grayscaleEnabled;
		[NullAllowed, Export ("grayscaleEnabled", ArgumentSemantic.Strong)]
		NSNumber GrayscaleEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable increaseContrastEnabled;
		[NullAllowed, Export ("increaseContrastEnabled", ArgumentSemantic.Strong)]
		NSNumber IncreaseContrastEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable invertColorsEnabled;
		[NullAllowed, Export ("invertColorsEnabled", ArgumentSemantic.Strong)]
		NSNumber InvertColorsEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable monoAudioEnabled;
		[NullAllowed, Export ("monoAudioEnabled", ArgumentSemantic.Strong)]
		NSNumber MonoAudioEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable onOffSwitchLabelsEnabled;
		[NullAllowed, Export ("onOffSwitchLabelsEnabled", ArgumentSemantic.Strong)]
		NSNumber OnOffSwitchLabelsEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable reduceMotionEnabled;
		[NullAllowed, Export ("reduceMotionEnabled", ArgumentSemantic.Strong)]
		NSNumber ReduceMotionEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable reduceTransparencyEnabled;
		[NullAllowed, Export ("reduceTransparencyEnabled", ArgumentSemantic.Strong)]
		NSNumber ReduceTransparencyEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable reducedAnimationsEnabled;
		[NullAllowed, Export ("reducedAnimationsEnabled", ArgumentSemantic.Strong)]
		NSNumber ReducedAnimationsEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable rtlEnabled;
		[NullAllowed, Export ("rtlEnabled", ArgumentSemantic.Strong)]
		NSNumber RtlEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable screenReaderEnabled;
		[NullAllowed, Export ("screenReaderEnabled", ArgumentSemantic.Strong)]
		NSNumber ScreenReaderEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable shakeToUndoEnabled;
		[NullAllowed, Export ("shakeToUndoEnabled", ArgumentSemantic.Strong)]
		NSNumber ShakeToUndoEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable shouldDifferentiateWithoutColor;
		[NullAllowed, Export ("shouldDifferentiateWithoutColor", ArgumentSemantic.Strong)]
		NSNumber ShouldDifferentiateWithoutColor { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable singleAppModeEnabled;
		[NullAllowed, Export ("singleAppModeEnabled", ArgumentSemantic.Strong)]
		NSNumber SingleAppModeEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable speakScreenEnabled;
		[NullAllowed, Export ("speakScreenEnabled", ArgumentSemantic.Strong)]
		NSNumber SpeakScreenEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable speakSelectionEnabled;
		[NullAllowed, Export ("speakSelectionEnabled", ArgumentSemantic.Strong)]
		NSNumber SpeakSelectionEnabled { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable textSize;
		[NullAllowed, Export ("textSize")]
		string TextSize { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable videoAutoplayEnabled;
		[NullAllowed, Export ("videoAutoplayEnabled", ArgumentSemantic.Strong)]
		NSNumber VideoAutoplayEnabled { get; }
	}

	// @interface DDRUMViewEventViewAction
	[DisableDefaultCtor]
	interface DDRUMViewEventViewAction
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewCrash
	[DisableDefaultCtor]
	interface DDRUMViewEventViewCrash
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewCustomTimings
	[DisableDefaultCtor]
	interface DDRUMViewEventViewCustomTimings
	{
	}

	// @interface DDRUMViewEventViewError
	[DisableDefaultCtor]
	interface DDRUMViewEventViewError
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewFlutterBuildTime
	[DisableDefaultCtor]
	interface DDRUMViewEventViewFlutterBuildTime
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull average;
		[Export ("average", ArgumentSemantic.Strong)]
		NSNumber Average { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull max;
		[Export ("max", ArgumentSemantic.Strong)]
		NSNumber Max { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable metricMax;
		[NullAllowed, Export ("metricMax", ArgumentSemantic.Strong)]
		NSNumber MetricMax { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull min;
		[Export ("min", ArgumentSemantic.Strong)]
		NSNumber Min { get; }
	}

	// @interface DDRUMViewEventViewFlutterRasterTime
	[DisableDefaultCtor]
	interface DDRUMViewEventViewFlutterRasterTime
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull average;
		[Export ("average", ArgumentSemantic.Strong)]
		NSNumber Average { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull max;
		[Export ("max", ArgumentSemantic.Strong)]
		NSNumber Max { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable metricMax;
		[NullAllowed, Export ("metricMax", ArgumentSemantic.Strong)]
		NSNumber MetricMax { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull min;
		[Export ("min", ArgumentSemantic.Strong)]
		NSNumber Min { get; }
	}

	// @interface DDRUMViewEventViewFrozenFrame
	[DisableDefaultCtor]
	interface DDRUMViewEventViewFrozenFrame
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewFrustration
	[DisableDefaultCtor]
	interface DDRUMViewEventViewFrustration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewInForegroundPeriods
	[DisableDefaultCtor]
	interface DDRUMViewEventViewInForegroundPeriods
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMViewEventViewJsRefreshRate
	[DisableDefaultCtor]
	interface DDRUMViewEventViewJsRefreshRate
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull average;
		[Export ("average", ArgumentSemantic.Strong)]
		NSNumber Average { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull max;
		[Export ("max", ArgumentSemantic.Strong)]
		NSNumber Max { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable metricMax;
		[NullAllowed, Export ("metricMax", ArgumentSemantic.Strong)]
		NSNumber MetricMax { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull min;
		[Export ("min", ArgumentSemantic.Strong)]
		NSNumber Min { get; }
	}

	// @interface DDRUMViewEventViewLongTask
	[DisableDefaultCtor]
	interface DDRUMViewEventViewLongTask
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewPerformance
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformance
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceCLS * _Nullable cls;
		[NullAllowed, Export ("cls", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceCLS Cls { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceFBC * _Nullable fbc;
		[NullAllowed, Export ("fbc", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceFBC Fbc { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceFCP * _Nullable fcp;
		[NullAllowed, Export ("fcp", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceFCP Fcp { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceFID * _Nullable fid;
		[NullAllowed, Export ("fid", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceFID Fid { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceINP * _Nullable inp;
		[NullAllowed, Export ("inp", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceINP Inp { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceLCP * _Nullable lcp;
		[NullAllowed, Export ("lcp", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceLCP Lcp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceCLS
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceCLS
	{
		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceCLSCurrentRect * _Nullable currentRect;
		[NullAllowed, Export ("currentRect", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceCLSCurrentRect CurrentRect { get; }

		// @property (readonly, nonatomic, strong) DDRUMViewEventViewPerformanceCLSPreviousRect * _Nullable previousRect;
		[NullAllowed, Export ("previousRect", ArgumentSemantic.Strong)]
		DDRUMViewEventViewPerformanceCLSPreviousRect PreviousRect { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull score;
		[Export ("score", ArgumentSemantic.Strong)]
		NSNumber Score { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable targetSelector;
		[NullAllowed, Export ("targetSelector")]
		string TargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable timestamp;
		[NullAllowed, Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceCLSCurrentRect
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceCLSCurrentRect
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull x;
		[Export ("x", ArgumentSemantic.Strong)]
		NSNumber X { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull y;
		[Export ("y", ArgumentSemantic.Strong)]
		NSNumber Y { get; }
	}

	// @interface DDRUMViewEventViewPerformanceCLSPreviousRect
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceCLSPreviousRect
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull x;
		[Export ("x", ArgumentSemantic.Strong)]
		NSNumber X { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull y;
		[Export ("y", ArgumentSemantic.Strong)]
		NSNumber Y { get; }
	}

	// @interface DDRUMViewEventViewPerformanceFBC
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceFBC
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull timestamp;
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceFCP
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceFCP
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull timestamp;
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceFID
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceFID
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable targetSelector;
		[NullAllowed, Export ("targetSelector")]
		string TargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull timestamp;
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceINP
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceINP
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable targetSelector;
		[NullAllowed, Export ("targetSelector")]
		string TargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable timestamp;
		[NullAllowed, Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewPerformanceLCP
	[DisableDefaultCtor]
	interface DDRUMViewEventViewPerformanceLCP
	{
		// @property (copy, nonatomic) NSString * _Nullable resourceUrl;
		[NullAllowed, Export ("resourceUrl")]
		string ResourceUrl { get; set; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable targetSelector;
		[NullAllowed, Export ("targetSelector")]
		string TargetSelector { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull timestamp;
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; }
	}

	// @interface DDRUMViewEventViewResource
	[DisableDefaultCtor]
	interface DDRUMViewEventViewResource
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull count;
		[Export ("count", ArgumentSemantic.Strong)]
		NSNumber Count { get; }
	}

	// @interface DDRUMViewEventViewSlowFrames
	[DisableDefaultCtor]
	interface DDRUMViewEventViewSlowFrames
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull start;
		[Export ("start", ArgumentSemantic.Strong)]
		NSNumber Start { get; }
	}

	// @interface DDRUMVitalAppLaunchEvent
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventSource source;
		[Export ("source")]
		DDRUMVitalAppLaunchEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventView View { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventVital * _Nonnull vital;
		[Export ("vital", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventVital Vital { get; }
	}

	// @interface DDRUMVitalAppLaunchEventApplication
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalAppLaunchEventContainer
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventContainerSource source;
		[Export ("source")]
		DDRUMVitalAppLaunchEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventContainerView View { get; }
	}

	// @interface DDRUMVitalAppLaunchEventContainerView
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDD
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDDProfiling * _Nullable profiling;
		[NullAllowed, Export ("profiling", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDDProfiling Profiling { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDDSession Session { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDDProfiling
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDDProfiling
	{
		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventDDProfilingErrorReason errorReason;
		[Export ("errorReason")]
		DDRUMVitalAppLaunchEventDDProfilingErrorReason ErrorReason { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventDDProfilingStatus status;
		[Export ("status")]
		DDRUMVitalAppLaunchEventDDProfilingStatus Status { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDDSession
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMVitalAppLaunchEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMVitalAppLaunchEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDevice
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMVitalAppLaunchEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDisplay
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMVitalAppLaunchEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMVitalAppLaunchEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalAppLaunchEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMVitalAppLaunchEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMVitalAppLaunchEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMVitalAppLaunchEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMEventAttributes
	{
	}

	// @interface DDRUMVitalAppLaunchEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMVitalAppLaunchEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalAppLaunchEventSession
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMVitalAppLaunchEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMVitalAppLaunchEventStream
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalAppLaunchEventView
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMVitalAppLaunchEventVital
	[DisableDefaultCtor]
	interface DDRUMVitalAppLaunchEventVital
	{
		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventVitalAppLaunchMetric appLaunchMetric;
		[Export ("appLaunchMetric")]
		DDRUMVitalAppLaunchEventVitalAppLaunchMetric AppLaunchMetric { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable vitalDescription;
		[NullAllowed, Export ("vitalDescription")]
		string VitalDescription { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasSavedInstanceStateBundle;
		[NullAllowed, Export ("hasSavedInstanceStateBundle", ArgumentSemantic.Strong)]
		NSNumber HasSavedInstanceStateBundle { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isPrewarmed;
		[NullAllowed, Export ("isPrewarmed", ArgumentSemantic.Strong)]
		NSNumber IsPrewarmed { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalAppLaunchEventVitalStartupType startupType;
		[Export ("startupType")]
		DDRUMVitalAppLaunchEventVitalStartupType StartupType { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }
	}

	// @interface DDRUMVitalDurationEvent
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventSource source;
		[Export ("source")]
		DDRUMVitalDurationEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventView View { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventVital * _Nonnull vital;
		[Export ("vital", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventVital Vital { get; }
	}

	// @interface DDRUMVitalDurationEventApplication
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalDurationEventContainer
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventContainerSource source;
		[Export ("source")]
		DDRUMVitalDurationEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventContainerView View { get; }
	}

	// @interface DDRUMVitalDurationEventContainerView
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalDurationEventDD
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDDSession Session { get; }
	}

	// @interface DDRUMVitalDurationEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMVitalDurationEventDDSession
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMVitalDurationEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMVitalDurationEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMVitalDurationEventDevice
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMVitalDurationEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMVitalDurationEventDisplay
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMVitalDurationEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMVitalDurationEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMVitalDurationEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalDurationEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMVitalDurationEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalDurationEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMVitalDurationEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMVitalDurationEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMVitalDurationEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMVitalDurationEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMVitalDurationEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMEventAttributes
	{
	}

	// @interface DDRUMVitalDurationEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMVitalDurationEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalDurationEventSession
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalDurationEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMVitalDurationEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMVitalDurationEventStream
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalDurationEventView
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMVitalDurationEventVital
	[DisableDefaultCtor]
	interface DDRUMVitalDurationEventVital
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable vitalDescription;
		[NullAllowed, Export ("vitalDescription")]
		string VitalDescription { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull duration;
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }
	}

	// @interface DDRUMVitalOperationStepEvent
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEvent
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMAccount * _Nullable account;
		[NullAllowed, Export ("account", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMAccount Account { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventApplication * _Nonnull application;
		[Export ("application", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventApplication Application { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildVersion;
		[NullAllowed, Export ("buildVersion")]
		string BuildVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMCITest * _Nullable ciTest;
		[NullAllowed, Export ("ciTest", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMCITest CiTest { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMConnectivity * _Nullable connectivity;
		[NullAllowed, Export ("connectivity", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMConnectivity Connectivity { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventContainer * _Nullable container;
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventContainer Container { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMEventAttributes * _Nullable context;
		[NullAllowed, Export ("context", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMEventAttributes Context { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable ddtags;
		[NullAllowed, Export ("ddtags")]
		string Ddtags { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDisplay * _Nullable display;
		[NullAllowed, Export ("display", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDisplay Display { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventSession * _Nonnull session;
		[Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventSource source;
		[Export ("source")]
		DDRUMVitalOperationStepEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventStream * _Nullable stream;
		[NullAllowed, Export ("stream", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventStream Stream { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMSyntheticsTest * _Nullable synthetics;
		[NullAllowed, Export ("synthetics", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMSyntheticsTest Synthetics { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMUser * _Nullable usr;
		[NullAllowed, Export ("usr", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMUser Usr { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventView View { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventVital * _Nonnull vital;
		[Export ("vital", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventVital Vital { get; }
	}

	// @interface DDRUMVitalOperationStepEventApplication
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable currentLocale;
		[NullAllowed, Export ("currentLocale")]
		string CurrentLocale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalOperationStepEventContainer
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventContainer
	{
		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventContainerSource source;
		[Export ("source")]
		DDRUMVitalOperationStepEventContainerSource Source { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventContainerView * _Nonnull view;
		[Export ("view", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventContainerView View { get; }
	}

	// @interface DDRUMVitalOperationStepEventContainerView
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventContainerView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalOperationStepEventDD
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDD
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable browserSdkVersion;
		[NullAllowed, Export ("browserSdkVersion")]
		string BrowserSdkVersion { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDDConfiguration * _Nullable configuration;
		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDDConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable sdkName;
		[NullAllowed, Export ("sdkName")]
		string SdkName { get; }

		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDDSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDDSession Session { get; }
	}

	// @interface DDRUMVitalOperationStepEventDDConfiguration
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDDConfiguration
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull sessionSampleRate;
		[Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }
	}

	// @interface DDRUMVitalOperationStepEventDDSession
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDDSession
	{
		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventDDSessionPlan plan;
		[Export ("plan")]
		DDRUMVitalOperationStepEventDDSessionPlan Plan { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventDDSessionRUMSessionPrecondition sessionPrecondition;
		[Export ("sessionPrecondition")]
		DDRUMVitalOperationStepEventDDSessionRUMSessionPrecondition SessionPrecondition { get; }
	}

	// @interface DDRUMVitalOperationStepEventDevice
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batteryLevel;
		[NullAllowed, Export ("batteryLevel", ArgumentSemantic.Strong)]
		NSNumber BatteryLevel { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable brightnessLevel;
		[NullAllowed, Export ("brightnessLevel", ArgumentSemantic.Strong)]
		NSNumber BrightnessLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable powerSavingMode;
		[NullAllowed, Export ("powerSavingMode", ArgumentSemantic.Strong)]
		NSNumber PowerSavingMode { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventDeviceDeviceType type;
		[Export ("type")]
		DDRUMVitalOperationStepEventDeviceDeviceType Type { get; }
	}

	// @interface DDRUMVitalOperationStepEventDisplay
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDisplay
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventDisplayViewport * _Nullable viewport;
		[NullAllowed, Export ("viewport", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventDisplayViewport Viewport { get; }
	}

	// @interface DDRUMVitalOperationStepEventDisplayViewport
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventDisplayViewport
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull height;
		[Export ("height", ArgumentSemantic.Strong)]
		NSNumber Height { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull width;
		[Export ("width", ArgumentSemantic.Strong)]
		NSNumber Width { get; }
	}

	// @interface DDRUMVitalOperationStepEventOperatingSystem
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMAccount
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMAccount
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMCITest
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMCITest
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull testExecutionId;
		[Export ("testExecutionId")]
		string TestExecutionId { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMConnectivity
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMConnectivity
	{
		// @property (readonly, nonatomic, strong) DDRUMVitalOperationStepEventRUMConnectivityCellular * _Nullable cellular;
		[NullAllowed, Export ("cellular", ArgumentSemantic.Strong)]
		DDRUMVitalOperationStepEventRUMConnectivityCellular Cellular { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventRUMConnectivityEffectiveType effectiveType;
		[Export ("effectiveType")]
		DDRUMVitalOperationStepEventRUMConnectivityEffectiveType EffectiveType { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventRUMConnectivityStatus status;
		[Export ("status")]
		DDRUMVitalOperationStepEventRUMConnectivityStatus Status { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMConnectivityCellular
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMConnectivityCellular
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable technology;
		[NullAllowed, Export ("technology")]
		string Technology { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMEventAttributes
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMEventAttributes
	{
	}

	// @interface DDRUMVitalOperationStepEventRUMSyntheticsTest
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMSyntheticsTest
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable injected;
		[NullAllowed, Export ("injected", ArgumentSemantic.Strong)]
		NSNumber Injected { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull resultId;
		[Export ("resultId")]
		string ResultId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull testId;
		[Export ("testId")]
		string TestId { get; }
	}

	// @interface DDRUMVitalOperationStepEventRUMUser
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventRUMUser
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable anonymousId;
		[NullAllowed, Export ("anonymousId")]
		string AnonymousId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	// @interface DDRUMVitalOperationStepEventSession
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventSession
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nullable hasReplay;
		[NullAllowed, Export ("hasReplay", ArgumentSemantic.Strong)]
		NSNumber HasReplay { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventSessionRUMSessionType type;
		[Export ("type")]
		DDRUMVitalOperationStepEventSessionRUMSessionType Type { get; }
	}

	// @interface DDRUMVitalOperationStepEventStream
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventStream
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDRUMVitalOperationStepEventView
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable referrer;
		[NullAllowed, Export ("referrer")]
		string Referrer { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull url;
		[Export ("url")]
		string Url { get; set; }
	}

	// @interface DDRUMVitalOperationStepEventVital
	[DisableDefaultCtor]
	interface DDRUMVitalOperationStepEventVital
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable vitalDescription;
		[NullAllowed, Export ("vitalDescription")]
		string VitalDescription { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventVitalFailureReason failureReason;
		[Export ("failureReason")]
		DDRUMVitalOperationStepEventVitalFailureReason FailureReason { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable operationKey;
		[NullAllowed, Export ("operationKey")]
		string OperationKey { get; }

		// @property (readonly, nonatomic) enum DDRUMVitalOperationStepEventVitalStepType stepType;
		[Export ("stepType")]
		DDRUMVitalOperationStepEventVitalStepType StepType { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }
	}

	// @interface DDTelemetryConfigurationEvent
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEvent
	{
		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventApplication * _Nullable application;
		[NullAllowed, Export ("application", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventApplication Application { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable effectiveSampleRate;
		[NullAllowed, Export ("effectiveSampleRate", ArgumentSemantic.Strong)]
		NSNumber EffectiveSampleRate { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull service;
		[Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDTelemetryConfigurationEventSource source;
		[Export ("source")]
		DDTelemetryConfigurationEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetry * _Nonnull telemetry;
		[Export ("telemetry", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetry Telemetry { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventView * _Nullable view;
		[NullAllowed, Export ("view", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventView View { get; }
	}

	// @interface DDTelemetryConfigurationEventAction
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventAction
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryConfigurationEventApplication
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryConfigurationEventDD
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventDD
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }
	}

	// @interface DDTelemetryConfigurationEventSession
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventSession
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetry
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetry
	{
		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetryConfiguration * _Nonnull configuration;
		[Export ("configuration", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetryConfiguration Configuration { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetryRUMTelemetryDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetryRUMTelemetryDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetryRUMTelemetryOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetryRUMTelemetryOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryConfiguration
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryConfiguration
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable actionNameAttribute;
		[NullAllowed, Export ("actionNameAttribute")]
		string ActionNameAttribute { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable allowFallbackToLocalStorage;
		[NullAllowed, Export ("allowFallbackToLocalStorage", ArgumentSemantic.Strong)]
		NSNumber AllowFallbackToLocalStorage { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable allowUntrustedEvents;
		[NullAllowed, Export ("allowUntrustedEvents", ArgumentSemantic.Strong)]
		NSNumber AllowUntrustedEvents { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable appHangThreshold;
		[NullAllowed, Export ("appHangThreshold", ArgumentSemantic.Strong)]
		NSNumber AppHangThreshold { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable backgroundTasksEnabled;
		[NullAllowed, Export ("backgroundTasksEnabled", ArgumentSemantic.Strong)]
		NSNumber BackgroundTasksEnabled { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batchProcessingLevel;
		[NullAllowed, Export ("batchProcessingLevel", ArgumentSemantic.Strong)]
		NSNumber BatchProcessingLevel { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batchSize;
		[NullAllowed, Export ("batchSize", ArgumentSemantic.Strong)]
		NSNumber BatchSize { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable batchUploadFrequency;
		[NullAllowed, Export ("batchUploadFrequency", ArgumentSemantic.Strong)]
		NSNumber BatchUploadFrequency { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable betaEncodeCookieOptions;
		[NullAllowed, Export ("betaEncodeCookieOptions", ArgumentSemantic.Strong)]
		NSNumber BetaEncodeCookieOptions { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable compressIntakeRequests;
		[NullAllowed, Export ("compressIntakeRequests", ArgumentSemantic.Strong)]
		NSNumber CompressIntakeRequests { get; }

		// @property (copy, nonatomic) NSString * _Nullable dartVersion;
		[NullAllowed, Export ("dartVersion")]
		string DartVersion { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable defaultPrivacyLevel;
		[NullAllowed, Export ("defaultPrivacyLevel")]
		string DefaultPrivacyLevel { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable enablePrivacyForActionName;
		[NullAllowed, Export ("enablePrivacyForActionName", ArgumentSemantic.Strong)]
		NSNumber EnablePrivacyForActionName { get; set; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetryConfigurationForwardConsoleLogs * _Nullable forwardConsoleLogs;
		[NullAllowed, Export ("forwardConsoleLogs", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetryConfigurationForwardConsoleLogs ForwardConsoleLogs { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable forwardErrorsToLogs;
		[NullAllowed, Export ("forwardErrorsToLogs", ArgumentSemantic.Strong)]
		NSNumber ForwardErrorsToLogs { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryConfigurationEventTelemetryConfigurationForwardReports * _Nullable forwardReports;
		[NullAllowed, Export ("forwardReports", ArgumentSemantic.Strong)]
		DDTelemetryConfigurationEventTelemetryConfigurationForwardReports ForwardReports { get; }

		// @property (copy, nonatomic) NSString * _Nullable imagePrivacyLevel;
		[NullAllowed, Export ("imagePrivacyLevel")]
		string ImagePrivacyLevel { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable initializationType;
		[NullAllowed, Export ("initializationType")]
		string InitializationType { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable invTimeThresholdMs;
		[NullAllowed, Export ("invTimeThresholdMs", ArgumentSemantic.Strong)]
		NSNumber InvTimeThresholdMs { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isMainProcess;
		[NullAllowed, Export ("isMainProcess", ArgumentSemantic.Strong)]
		NSNumber IsMainProcess { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable mobileVitalsUpdatePeriod;
		[NullAllowed, Export ("mobileVitalsUpdatePeriod", ArgumentSemantic.Strong)]
		NSNumber MobileVitalsUpdatePeriod { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable numberOfDisplays;
		[NullAllowed, Export ("numberOfDisplays", ArgumentSemantic.Strong)]
		NSNumber NumberOfDisplays { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable premiumSampleRate;
		[NullAllowed, Export ("premiumSampleRate", ArgumentSemantic.Strong)]
		NSNumber PremiumSampleRate { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable profilingSampleRate;
		[NullAllowed, Export ("profilingSampleRate", ArgumentSemantic.Strong)]
		NSNumber ProfilingSampleRate { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable propagateTraceBaggage;
		[NullAllowed, Export ("propagateTraceBaggage", ArgumentSemantic.Strong)]
		NSNumber PropagateTraceBaggage { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable reactNativeVersion;
		[NullAllowed, Export ("reactNativeVersion")]
		string ReactNativeVersion { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable reactVersion;
		[NullAllowed, Export ("reactVersion")]
		string ReactVersion { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable remoteConfigurationId;
		[NullAllowed, Export ("remoteConfigurationId")]
		string RemoteConfigurationId { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable replaySampleRate;
		[NullAllowed, Export ("replaySampleRate", ArgumentSemantic.Strong)]
		NSNumber ReplaySampleRate { get; }

		// @property (copy, nonatomic) NSString * _Nullable sdkVersion;
		[NullAllowed, Export ("sdkVersion")]
		string SdkVersion { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable sendLogsAfterSessionExpiration;
		[NullAllowed, Export ("sendLogsAfterSessionExpiration", ArgumentSemantic.Strong)]
		NSNumber SendLogsAfterSessionExpiration { get; set; }

		// @property (readonly, nonatomic) enum DDTelemetryConfigurationEventTelemetryConfigurationSessionPersistence sessionPersistence;
		[Export ("sessionPersistence")]
		DDTelemetryConfigurationEventTelemetryConfigurationSessionPersistence SessionPersistence { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable sessionReplaySampleRate;
		[NullAllowed, Export ("sessionReplaySampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionReplaySampleRate { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable sessionSampleRate;
		[NullAllowed, Export ("sessionSampleRate", ArgumentSemantic.Strong)]
		NSNumber SessionSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable silentMultipleInit;
		[NullAllowed, Export ("silentMultipleInit", ArgumentSemantic.Strong)]
		NSNumber SilentMultipleInit { get; }

		// @property (copy, nonatomic) NSString * _Nullable source;
		[NullAllowed, Export ("source")]
		string Source { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable startRecordingImmediately;
		[NullAllowed, Export ("startRecordingImmediately", ArgumentSemantic.Strong)]
		NSNumber StartRecordingImmediately { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable startSessionReplayRecordingManually;
		[NullAllowed, Export ("startSessionReplayRecordingManually", ArgumentSemantic.Strong)]
		NSNumber StartSessionReplayRecordingManually { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable storeContextsAcrossPages;
		[NullAllowed, Export ("storeContextsAcrossPages", ArgumentSemantic.Strong)]
		NSNumber StoreContextsAcrossPages { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable swiftuiActionTrackingEnabled;
		[NullAllowed, Export ("swiftuiActionTrackingEnabled", ArgumentSemantic.Strong)]
		NSNumber SwiftuiActionTrackingEnabled { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable swiftuiViewTrackingEnabled;
		[NullAllowed, Export ("swiftuiViewTrackingEnabled", ArgumentSemantic.Strong)]
		NSNumber SwiftuiViewTrackingEnabled { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable telemetryConfigurationSampleRate;
		[NullAllowed, Export ("telemetryConfigurationSampleRate", ArgumentSemantic.Strong)]
		NSNumber TelemetryConfigurationSampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable telemetrySampleRate;
		[NullAllowed, Export ("telemetrySampleRate", ArgumentSemantic.Strong)]
		NSNumber TelemetrySampleRate { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable telemetryUsageSampleRate;
		[NullAllowed, Export ("telemetryUsageSampleRate", ArgumentSemantic.Strong)]
		NSNumber TelemetryUsageSampleRate { get; }

		// @property (copy, nonatomic) NSString * _Nullable textAndInputPrivacyLevel;
		[NullAllowed, Export ("textAndInputPrivacyLevel")]
		string TextAndInputPrivacyLevel { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable tnsTimeThresholdMs;
		[NullAllowed, Export ("tnsTimeThresholdMs", ArgumentSemantic.Strong)]
		NSNumber TnsTimeThresholdMs { get; }

		// @property (copy, nonatomic) NSString * _Nullable touchPrivacyLevel;
		[NullAllowed, Export ("touchPrivacyLevel")]
		string TouchPrivacyLevel { get; set; }

		// @property (nonatomic) enum DDTelemetryConfigurationEventTelemetryConfigurationTraceContextInjection traceContextInjection;
		[Export ("traceContextInjection", ArgumentSemantic.Assign)]
		DDTelemetryConfigurationEventTelemetryConfigurationTraceContextInjection TraceContextInjection { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable traceSampleRate;
		[NullAllowed, Export ("traceSampleRate", ArgumentSemantic.Strong)]
		NSNumber TraceSampleRate { get; }

		// @property (copy, nonatomic) NSString * _Nullable tracerApi;
		[NullAllowed, Export ("tracerApi")]
		string TracerApi { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable tracerApiVersion;
		[NullAllowed, Export ("tracerApiVersion")]
		string TracerApiVersion { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackAnonymousUser;
		[NullAllowed, Export ("trackAnonymousUser", ArgumentSemantic.Strong)]
		NSNumber TrackAnonymousUser { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackBackgroundEvents;
		[NullAllowed, Export ("trackBackgroundEvents", ArgumentSemantic.Strong)]
		NSNumber TrackBackgroundEvents { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackBfcacheViews;
		[NullAllowed, Export ("trackBfcacheViews", ArgumentSemantic.Strong)]
		NSNumber TrackBfcacheViews { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackCrossPlatformLongTasks;
		[NullAllowed, Export ("trackCrossPlatformLongTasks", ArgumentSemantic.Strong)]
		NSNumber TrackCrossPlatformLongTasks { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackEarlyRequests;
		[NullAllowed, Export ("trackEarlyRequests", ArgumentSemantic.Strong)]
		NSNumber TrackEarlyRequests { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackErrors;
		[NullAllowed, Export ("trackErrors", ArgumentSemantic.Strong)]
		NSNumber TrackErrors { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackFlutterPerformance;
		[NullAllowed, Export ("trackFlutterPerformance", ArgumentSemantic.Strong)]
		NSNumber TrackFlutterPerformance { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackFrustrations;
		[NullAllowed, Export ("trackFrustrations", ArgumentSemantic.Strong)]
		NSNumber TrackFrustrations { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackInteractions;
		[NullAllowed, Export ("trackInteractions", ArgumentSemantic.Strong)]
		NSNumber TrackInteractions { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackLongTask;
		[NullAllowed, Export ("trackLongTask", ArgumentSemantic.Strong)]
		NSNumber TrackLongTask { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackNativeErrors;
		[NullAllowed, Export ("trackNativeErrors", ArgumentSemantic.Strong)]
		NSNumber TrackNativeErrors { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackNativeLongTasks;
		[NullAllowed, Export ("trackNativeLongTasks", ArgumentSemantic.Strong)]
		NSNumber TrackNativeLongTasks { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackNativeViews;
		[NullAllowed, Export ("trackNativeViews", ArgumentSemantic.Strong)]
		NSNumber TrackNativeViews { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackNetworkRequests;
		[NullAllowed, Export ("trackNetworkRequests", ArgumentSemantic.Strong)]
		NSNumber TrackNetworkRequests { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackResources;
		[NullAllowed, Export ("trackResources", ArgumentSemantic.Strong)]
		NSNumber TrackResources { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable trackSessionAcrossSubdomains;
		[NullAllowed, Export ("trackSessionAcrossSubdomains", ArgumentSemantic.Strong)]
		NSNumber TrackSessionAcrossSubdomains { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackUserInteractions;
		[NullAllowed, Export ("trackUserInteractions", ArgumentSemantic.Strong)]
		NSNumber TrackUserInteractions { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable trackViewsManually;
		[NullAllowed, Export ("trackViewsManually", ArgumentSemantic.Strong)]
		NSNumber TrackViewsManually { get; set; }

		// @property (readonly, nonatomic) enum DDTelemetryConfigurationEventTelemetryConfigurationTrackingConsent trackingConsent;
		[Export ("trackingConsent")]
		DDTelemetryConfigurationEventTelemetryConfigurationTrackingConsent TrackingConsent { get; }

		// @property (copy, nonatomic) NSString * _Nullable unityVersion;
		[NullAllowed, Export ("unityVersion")]
		string UnityVersion { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useAllowedGraphQlUrls;
		[NullAllowed, Export ("useAllowedGraphQlUrls", ArgumentSemantic.Strong)]
		NSNumber UseAllowedGraphQlUrls { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useAllowedTracingOrigins;
		[NullAllowed, Export ("useAllowedTracingOrigins", ArgumentSemantic.Strong)]
		NSNumber UseAllowedTracingOrigins { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useAllowedTracingUrls;
		[NullAllowed, Export ("useAllowedTracingUrls", ArgumentSemantic.Strong)]
		NSNumber UseAllowedTracingUrls { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable useAllowedTrackingOrigins;
		[NullAllowed, Export ("useAllowedTrackingOrigins", ArgumentSemantic.Strong)]
		NSNumber UseAllowedTrackingOrigins { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useBeforeSend;
		[NullAllowed, Export ("useBeforeSend", ArgumentSemantic.Strong)]
		NSNumber UseBeforeSend { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useCrossSiteSessionCookie;
		[NullAllowed, Export ("useCrossSiteSessionCookie", ArgumentSemantic.Strong)]
		NSNumber UseCrossSiteSessionCookie { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useExcludedActivityUrls;
		[NullAllowed, Export ("useExcludedActivityUrls", ArgumentSemantic.Strong)]
		NSNumber UseExcludedActivityUrls { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable useFirstPartyHosts;
		[NullAllowed, Export ("useFirstPartyHosts", ArgumentSemantic.Strong)]
		NSNumber UseFirstPartyHosts { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useLocalEncryption;
		[NullAllowed, Export ("useLocalEncryption", ArgumentSemantic.Strong)]
		NSNumber UseLocalEncryption { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable usePartitionedCrossSiteSessionCookie;
		[NullAllowed, Export ("usePartitionedCrossSiteSessionCookie", ArgumentSemantic.Strong)]
		NSNumber UsePartitionedCrossSiteSessionCookie { get; }

		// @property (nonatomic, strong) NSNumber * _Nullable usePciIntake;
		[NullAllowed, Export ("usePciIntake", ArgumentSemantic.Strong)]
		NSNumber UsePciIntake { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable useProxy;
		[NullAllowed, Export ("useProxy", ArgumentSemantic.Strong)]
		NSNumber UseProxy { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable useRemoteConfigurationProxy;
		[NullAllowed, Export ("useRemoteConfigurationProxy", ArgumentSemantic.Strong)]
		NSNumber UseRemoteConfigurationProxy { get; set; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useSecureSessionCookie;
		[NullAllowed, Export ("useSecureSessionCookie", ArgumentSemantic.Strong)]
		NSNumber UseSecureSessionCookie { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useTracing;
		[NullAllowed, Export ("useTracing", ArgumentSemantic.Strong)]
		NSNumber UseTracing { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useTrackGraphQlPayload;
		[NullAllowed, Export ("useTrackGraphQlPayload", ArgumentSemantic.Strong)]
		NSNumber UseTrackGraphQlPayload { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useTrackGraphQlResponseErrors;
		[NullAllowed, Export ("useTrackGraphQlResponseErrors", ArgumentSemantic.Strong)]
		NSNumber UseTrackGraphQlResponseErrors { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable useWorkerUrl;
		[NullAllowed, Export ("useWorkerUrl", ArgumentSemantic.Strong)]
		NSNumber UseWorkerUrl { get; }

		// @property (copy, nonatomic) NSString * _Nullable variant;
		[NullAllowed, Export ("variant")]
		string Variant { get; set; }

		// @property (readonly, nonatomic) enum DDTelemetryConfigurationEventTelemetryConfigurationViewTrackingStrategy viewTrackingStrategy;
		[Export ("viewTrackingStrategy")]
		DDTelemetryConfigurationEventTelemetryConfigurationViewTrackingStrategy ViewTrackingStrategy { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryConfigurationForwardConsoleLogs
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryConfigurationForwardConsoleLogs
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryConfigurationForwardReports
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryConfigurationForwardReports
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable string;
		[NullAllowed, Export ("string")]
		string String { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryConfigurationPlugins
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryConfigurationPlugins
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryRUMTelemetryDevice
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryRUMTelemetryDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }
	}

	// @interface DDTelemetryConfigurationEventTelemetryRUMTelemetryOperatingSystem
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventTelemetryRUMTelemetryOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }
	}

	// @interface DDTelemetryConfigurationEventView
	[DisableDefaultCtor]
	interface DDTelemetryConfigurationEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryDebugEvent
	[DisableDefaultCtor]
	interface DDTelemetryDebugEvent
	{
		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventApplication * _Nullable application;
		[NullAllowed, Export ("application", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventApplication Application { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable effectiveSampleRate;
		[NullAllowed, Export ("effectiveSampleRate", ArgumentSemantic.Strong)]
		NSNumber EffectiveSampleRate { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull service;
		[Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDTelemetryDebugEventSource source;
		[Export ("source")]
		DDTelemetryDebugEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventTelemetry * _Nonnull telemetry;
		[Export ("telemetry", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventTelemetry Telemetry { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventView * _Nullable view;
		[NullAllowed, Export ("view", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventView View { get; }
	}

	// @interface DDTelemetryDebugEventAction
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventAction
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryDebugEventApplication
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryDebugEventDD
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventDD
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }
	}

	// @interface DDTelemetryDebugEventSession
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventSession
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryDebugEventTelemetry
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventTelemetry
	{
		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventTelemetryRUMTelemetryDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventTelemetryRUMTelemetryDevice Device { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryDebugEventTelemetryRUMTelemetryOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDTelemetryDebugEventTelemetryRUMTelemetryOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull status;
		[Export ("status")]
		string Status { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; }
	}

	// @interface DDTelemetryDebugEventTelemetryRUMTelemetryDevice
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventTelemetryRUMTelemetryDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }
	}

	// @interface DDTelemetryDebugEventTelemetryRUMTelemetryOperatingSystem
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventTelemetryRUMTelemetryOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }
	}

	// @interface DDTelemetryDebugEventView
	[DisableDefaultCtor]
	interface DDTelemetryDebugEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryErrorEvent
	[DisableDefaultCtor]
	interface DDTelemetryErrorEvent
	{
		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventDD * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventDD Dd { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventAction * _Nullable action;
		[NullAllowed, Export ("action", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventAction Action { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventApplication * _Nullable application;
		[NullAllowed, Export ("application", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventApplication Application { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull date;
		[Export ("date", ArgumentSemantic.Strong)]
		NSNumber Date { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable effectiveSampleRate;
		[NullAllowed, Export ("effectiveSampleRate", ArgumentSemantic.Strong)]
		NSNumber EffectiveSampleRate { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull service;
		[Export ("service")]
		string Service { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventSession * _Nullable session;
		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventSession Session { get; }

		// @property (readonly, nonatomic) enum DDTelemetryErrorEventSource source;
		[Export ("source")]
		DDTelemetryErrorEventSource Source { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventTelemetry * _Nonnull telemetry;
		[Export ("telemetry", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventTelemetry Telemetry { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull type;
		[Export ("type")]
		string Type { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventView * _Nullable view;
		[NullAllowed, Export ("view", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventView View { get; }
	}

	// @interface DDTelemetryErrorEventAction
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventAction
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryErrorEventApplication
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventApplication
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryErrorEventDD
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventDD
	{
		// @property (readonly, nonatomic, strong) NSNumber * _Nonnull formatVersion;
		[Export ("formatVersion", ArgumentSemantic.Strong)]
		NSNumber FormatVersion { get; }
	}

	// @interface DDTelemetryErrorEventSession
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventSession
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @interface DDTelemetryErrorEventTelemetry
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventTelemetry
	{
		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventTelemetryRUMTelemetryDevice * _Nullable device;
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventTelemetryRUMTelemetryDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventTelemetryError * _Nullable error;
		[NullAllowed, Export ("error", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventTelemetryError Error { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; }

		// @property (readonly, nonatomic, strong) DDTelemetryErrorEventTelemetryRUMTelemetryOperatingSystem * _Nullable os;
		[NullAllowed, Export ("os", ArgumentSemantic.Strong)]
		DDTelemetryErrorEventTelemetryRUMTelemetryOperatingSystem Os { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull status;
		[Export ("status")]
		string Status { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable type;
		[NullAllowed, Export ("type")]
		string Type { get; }
	}

	// @interface DDTelemetryErrorEventTelemetryError
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventTelemetryError
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable kind;
		[NullAllowed, Export ("kind")]
		string Kind { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable stack;
		[NullAllowed, Export ("stack")]
		string Stack { get; }
	}

	// @interface DDTelemetryErrorEventTelemetryRUMTelemetryDevice
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventTelemetryRUMTelemetryDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isLowRamDevice;
		[NullAllowed, Export ("isLowRamDevice", ArgumentSemantic.Strong)]
		NSNumber IsLowRamDevice { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable processorCount;
		[NullAllowed, Export ("processorCount", ArgumentSemantic.Strong)]
		NSNumber ProcessorCount { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable totalRam;
		[NullAllowed, Export ("totalRam", ArgumentSemantic.Strong)]
		NSNumber TotalRam { get; }
	}

	// @interface DDTelemetryErrorEventTelemetryRUMTelemetryOperatingSystem
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventTelemetryRUMTelemetryOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; }
	}

	// @interface DDTelemetryErrorEventView
	[DisableDefaultCtor]
	interface DDTelemetryErrorEventView
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }
	}

	// @protocol DDUIPressRUMActionsPredicate
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/[Protocol]
	interface DDUIPressRUMActionsPredicate
	{
		// @required -(DDRUMAction * _Nullable)rumActionWithPress:(enum UIPressType)type targetView:(UIView * _Nonnull)targetView __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("rumActionWithPress:targetView:")]
		[return: NullAllowed]
		DDRUMAction TargetView (UIPressType type, UIView targetView);
	}

	// @interface DDRUMURLSessionTracking
	interface DDRUMURLSessionTracking
	{
		// -(void)setFirstPartyHostsTracing:(DDRUMFirstPartyHostsTracing * _Nonnull)firstPartyHostsTracing;
		[Export ("setFirstPartyHostsTracing:")]
		void SetFirstPartyHostsTracing (DDRUMFirstPartyHostsTracing firstPartyHostsTracing);

		// -(void)setResourceAttributesProvider:(id)provider;
		[Export ("setResourceAttributesProvider:")]
		void SetResourceAttributesProvider (NSObject provider);
	}
}
