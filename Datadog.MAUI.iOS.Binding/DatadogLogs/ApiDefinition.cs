using System;
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS.Logs
{
	// @interface DDLogEvent : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEvent
	{
		// @property (readonly, copy, nonatomic) NSDate * _Nonnull date;
		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		// @property (readonly, nonatomic) enum DDLogEventStatus status;
		[Export ("status")]
		DDLogEventStatus Status { get; }

		// @property (copy, nonatomic) NSString * _Nonnull message;
		[Export ("message")]
		string Message { get; set; }

		// @property (readonly, nonatomic, strong) DDLogEventError * _Nullable error;
		[NullAllowed, Export ("error", ArgumentSemantic.Strong)]
		DDLogEventError Error { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull serviceName;
		[Export ("serviceName")]
		string ServiceName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull environment;
		[Export ("environment")]
		string Environment { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull loggerName;
		[Export ("loggerName")]
		string LoggerName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull loggerVersion;
		[Export ("loggerVersion")]
		string LoggerVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable threadName;
		[NullAllowed, Export ("threadName")]
		string ThreadName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull applicationVersion;
		[Export ("applicationVersion")]
		string ApplicationVersion { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull applicationBuildNumber;
		[Export ("applicationBuildNumber")]
		string ApplicationBuildNumber { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable buildId;
		[NullAllowed, Export ("buildId")]
		string BuildId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable variant;
		[NullAllowed, Export ("variant")]
		string Variant { get; }

		// @property (readonly, nonatomic, strong) DDLogEventDd * _Nonnull dd;
		[Export ("dd", ArgumentSemantic.Strong)]
		DDLogEventDd Dd { get; }

		// @property (readonly, nonatomic, strong) DDLogEventDevice * _Nonnull device;
		[Export ("device", ArgumentSemantic.Strong)]
		DDLogEventDevice Device { get; }

		// @property (readonly, nonatomic, strong) DDLogEventOperatingSystem * _Nonnull os;
		[Export ("os", ArgumentSemantic.Strong)]
		DDLogEventOperatingSystem Os { get; }

		// @property (readonly, nonatomic, strong) DDLogEventUserInfo * _Nonnull userInfo;
		[Export ("userInfo", ArgumentSemantic.Strong)]
		DDLogEventUserInfo UserInfo { get; }

		// @property (readonly, nonatomic, strong) DDLogEventAccountInfo * _Nullable accountInfo;
		[NullAllowed, Export ("accountInfo", ArgumentSemantic.Strong)]
		DDLogEventAccountInfo AccountInfo { get; }

		// @property (readonly, nonatomic, strong) DDLogEventNetworkConnectionInfo * _Nullable networkConnectionInfo;
		[NullAllowed, Export ("networkConnectionInfo", ArgumentSemantic.Strong)]
		DDLogEventNetworkConnectionInfo NetworkConnectionInfo { get; }

		// @property (readonly, nonatomic, strong) DDLogEventCarrierInfo * _Nullable mobileCarrierInfo;
		[NullAllowed, Export ("mobileCarrierInfo", ArgumentSemantic.Strong)]
		DDLogEventCarrierInfo MobileCarrierInfo { get; }

		// @property (readonly, nonatomic, strong) DDLogEventAttributes * _Nonnull attributes;
		[Export ("attributes", ArgumentSemantic.Strong)]
		DDLogEventAttributes Attributes { get; }

		// @property (copy, nonatomic) NSArray<NSString *> * _Nullable tags;
		[NullAllowed, Export ("tags", ArgumentSemantic.Copy)]
		string[] Tags { get; set; }
	}

	// @interface DDLogEventAccountInfo : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventAccountInfo
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nonnull extraInfo;
		[Export ("extraInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ExtraInfo { get; set; }
	}

	// @interface DDLogEventAttributes : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventAttributes
	{
		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nonnull userAttributes;
		[Export ("userAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserAttributes { get; set; }
	}

	// @interface DDLogEventBinaryImage : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventBinaryImage
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable arch;
		[NullAllowed, Export ("arch")]
		string Arch { get; }

		// @property (readonly, nonatomic) BOOL isSystem;
		[Export ("isSystem")]
		bool IsSystem { get; }

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

	// @interface DDLogEventCarrierInfo : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventCarrierInfo
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierName;
		[NullAllowed, Export ("carrierName")]
		string CarrierName { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable carrierISOCountryCode;
		[NullAllowed, Export ("carrierISOCountryCode")]
		string CarrierISOCountryCode { get; }

		// @property (readonly, nonatomic) BOOL carrierAllowsVOIP;
		[Export ("carrierAllowsVOIP")]
		bool CarrierAllowsVOIP { get; }

		// @property (readonly, nonatomic) enum DDLogEventRadioAccessTechnology radioAccessTechnology;
		[Export ("radioAccessTechnology")]
		DDLogEventRadioAccessTechnology RadioAccessTechnology { get; }
	}

	// @interface DDLogEventDDDevice : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventDDDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull architecture;
		[Export ("architecture")]
		string Architecture { get; }
	}

	// @interface DDLogEventDd : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventDd
	{
		// @property (readonly, nonatomic, strong) DDLogEventDDDevice * _Nonnull device;
		[Export ("device", ArgumentSemantic.Strong)]
		DDLogEventDDDevice Device { get; }
	}

	// @interface DDLogEventDevice : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventDevice
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable architecture;
		[NullAllowed, Export ("architecture")]
		string Architecture { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable brand;
		[NullAllowed, Export ("brand")]
		string Brand { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable locale;
		[NullAllowed, Export ("locale")]
		string Locale { get; }

		// @property (readonly, copy, nonatomic) NSArray<NSString *> * _Nullable locales;
		[NullAllowed, Export ("locales", ArgumentSemantic.Copy)]
		string[] Locales { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable model;
		[NullAllowed, Export ("model")]
		string Model { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable timeZone;
		[NullAllowed, Export ("timeZone")]
		string TimeZone { get; }

		// @property (readonly, nonatomic) enum DDLogEventDeviceDeviceType type;
		[Export ("type")]
		DDLogEventDeviceDeviceType Type { get; }
	}

	// @interface DDLogEventError : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventError
	{
		// @property (copy, nonatomic) NSString * _Nullable kind;
		[NullAllowed, Export ("kind")]
		string Kind { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable message;
		[NullAllowed, Export ("message")]
		string Message { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable stack;
		[NullAllowed, Export ("stack")]
		string Stack { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull sourceType;
		[Export ("sourceType")]
		string SourceType { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable fingerprint;
		[NullAllowed, Export ("fingerprint")]
		string Fingerprint { get; set; }

		// @property (copy, nonatomic) NSArray<DDLogEventBinaryImage *> * _Nullable binaryImages;
		[NullAllowed, Export ("binaryImages", ArgumentSemantic.Copy)]
		DDLogEventBinaryImage[] BinaryImages { get; set; }
	}

	// @interface DDLogEventNetworkConnectionInfo : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventNetworkConnectionInfo
	{
		// @property (readonly, nonatomic) enum DDLogEventReachability reachability;
		[Export ("reachability")]
		DDLogEventReachability Reachability { get; }

		// @property (readonly, copy, nonatomic) NSArray<NSNumber *> * _Nullable availableInterfaces;
		[NullAllowed, Export ("availableInterfaces", ArgumentSemantic.Copy)]
		NSNumber[] AvailableInterfaces { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable supportsIPv4;
		[NullAllowed, Export ("supportsIPv4", ArgumentSemantic.Strong)]
		NSNumber SupportsIPv4 { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable supportsIPv6;
		[NullAllowed, Export ("supportsIPv6", ArgumentSemantic.Strong)]
		NSNumber SupportsIPv6 { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isExpensive;
		[NullAllowed, Export ("isExpensive", ArgumentSemantic.Strong)]
		NSNumber IsExpensive { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable isConstrained;
		[NullAllowed, Export ("isConstrained", ArgumentSemantic.Strong)]
		NSNumber IsConstrained { get; }
	}

	// @interface DDLogEventOperatingSystem : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventOperatingSystem
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull version;
		[Export ("version")]
		string Version { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable build;
		[NullAllowed, Export ("build")]
		string Build { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull versionMajor;
		[Export ("versionMajor")]
		string VersionMajor { get; }
	}

	// @interface DDLogEventUserInfo : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogEventUserInfo
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable id;
		[NullAllowed, Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable email;
		[NullAllowed, Export ("email")]
		string Email { get; }

		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nonnull extraInfo;
		[Export ("extraInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ExtraInfo { get; set; }
	}

	// @interface DDLogger : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDLogger
	{
		// -(void)debug:(NSString * _Nonnull)message;
		[Export ("debug:")]
		void Debug (string message);

		// -(void)debug:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("debug:attributes:")]
		void Debug (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)debug:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("debug:error:attributes:")]
		void Debug (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)info:(NSString * _Nonnull)message;
		[Export ("info:")]
		void Info (string message);

		// -(void)info:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("info:attributes:")]
		void Info (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)info:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("info:error:attributes:")]
		void Info (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)notice:(NSString * _Nonnull)message;
		[Export ("notice:")]
		void Notice (string message);

		// -(void)notice:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("notice:attributes:")]
		void Notice (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)notice:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("notice:error:attributes:")]
		void Notice (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)warn:(NSString * _Nonnull)message;
		[Export ("warn:")]
		void Warn (string message);

		// -(void)warn:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("warn:attributes:")]
		void Warn (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)warn:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("warn:error:attributes:")]
		void Warn (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)error:(NSString * _Nonnull)message;
		[Export ("error:")]
		void Error (string message);

		// -(void)error:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("error:attributes:")]
		void Error (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)error:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("error:error:attributes:")]
		void Error (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)critical:(NSString * _Nonnull)message;
		[Export ("critical:")]
		void Critical (string message);

		// -(void)critical:(NSString * _Nonnull)message attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("critical:attributes:")]
		void Critical (string message, NSDictionary<NSString, NSObject> attributes);

		// -(void)critical:(NSString * _Nonnull)message error:(NSError * _Nonnull)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("critical:error:attributes:")]
		void Critical (string message, NSError error, NSDictionary<NSString, NSObject> attributes);

		// -(void)addAttributeForKey:(NSString * _Nonnull)key value:(id _Nonnull)value;
		[Export ("addAttributeForKey:value:")]
		void AddAttributeForKey (string key, NSObject value);

		// -(void)removeAttributeForKey:(NSString * _Nonnull)key;
		[Export ("removeAttributeForKey:")]
		void RemoveAttributeForKey (string key);

		// -(void)addTagWithKey:(NSString * _Nonnull)key value:(NSString * _Nonnull)value;
		[Export ("addTagWithKey:value:")]
		void AddTagWithKey (string key, string value);

		// -(void)removeTagWithKey:(NSString * _Nonnull)key;
		[Export ("removeTagWithKey:")]
		void RemoveTagWithKey (string key);

		// -(void)addWithTag:(NSString * _Nonnull)tag;
		[Export ("addWithTag:")]
		void AddWithTag (string tag);

		// -(void)removeWithTag:(NSString * _Nonnull)tag;
		[Export ("removeWithTag:")]
		void RemoveWithTag (string tag);

		// +(DDLogger * _Nonnull)createWith:(DDLoggerConfiguration * _Nonnull)configuration __attribute__((warn_unused_result("")));
		[Static]
		[Export ("createWith:")]
		DDLogger CreateWith (DDLoggerConfiguration configuration);
	}

	// @interface DatadogLogs_Swift_554 (DDLogger)
	[Category]
	[BaseType (typeof(DDLogger))]
	interface DDLogger_DatadogLogs_Swift_554
	{
		// -(void)_internal_sync_criticalWithMessage:(NSString * _Nonnull)message error:(NSError * _Nullable)error attributes:(NSDictionary<NSString *,id> * _Nonnull)attributes;
		[Export ("_internal_sync_criticalWithMessage:error:attributes:")]
		void _internal_sync_criticalWithMessage (string message, [NullAllowed] NSError error, NSDictionary<NSString, NSObject> attributes);
	}

	// @interface DDLoggerConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	interface DDLoggerConfiguration
	{
		// @property (copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable name;
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		// @property (nonatomic) BOOL networkInfoEnabled;
		[Export ("networkInfoEnabled")]
		bool NetworkInfoEnabled { get; set; }

		// @property (nonatomic) BOOL bundleWithRumEnabled;
		[Export ("bundleWithRumEnabled")]
		bool BundleWithRumEnabled { get; set; }

		// @property (nonatomic) BOOL bundleWithTraceEnabled;
		[Export ("bundleWithTraceEnabled")]
		bool BundleWithTraceEnabled { get; set; }

		// @property (nonatomic) float remoteSampleRate;
		[Export ("remoteSampleRate")]
		float RemoteSampleRate { get; set; }

		// @property (nonatomic) BOOL printLogsToConsole;
		[Export ("printLogsToConsole")]
		bool PrintLogsToConsole { get; set; }

		// @property (nonatomic) enum DDLogLevel remoteLogThreshold;
		[Export ("remoteLogThreshold", ArgumentSemantic.Assign)]
		DDLogLevel RemoteLogThreshold { get; set; }

		// -(instancetype _Nonnull)initWithService:(NSString * _Nullable)service name:(NSString * _Nullable)name networkInfoEnabled:(BOOL)networkInfoEnabled bundleWithRumEnabled:(BOOL)bundleWithRumEnabled bundleWithTraceEnabled:(BOOL)bundleWithTraceEnabled remoteSampleRate:(float)remoteSampleRate remoteLogThreshold:(enum DDLogLevel)remoteLogThreshold printLogsToConsole:(BOOL)printLogsToConsole __attribute__((objc_designated_initializer));
		[Export ("initWithService:name:networkInfoEnabled:bundleWithRumEnabled:bundleWithTraceEnabled:remoteSampleRate:remoteLogThreshold:printLogsToConsole:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string service, [NullAllowed] string name, bool networkInfoEnabled, bool bundleWithRumEnabled, bool bundleWithTraceEnabled, float remoteSampleRate, DDLogLevel remoteLogThreshold, bool printLogsToConsole);
	}

	// @interface DDLogs : NSObject
	[BaseType (typeof(NSObject))]
	interface DDLogs
	{
		// +(void)enableWith:(DDLogsConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWith:")]
		void EnableWith (DDLogsConfiguration configuration);

		// +(void)addAttributeForKey:(NSString * _Nonnull)key value:(id _Nonnull)value;
		[Static]
		[Export ("addAttributeForKey:value:")]
		void AddAttributeForKey (string key, NSObject value);

		// +(void)removeAttributeForKey:(NSString * _Nonnull)key;
		[Static]
		[Export ("removeAttributeForKey:")]
		void RemoveAttributeForKey (string key);
	}

	// @interface DDLogsConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	interface DDLogsConfiguration
	{
		// @property (copy, nonatomic) NSUrl * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSUrl CustomEndpoint { get; set; }

		// -(instancetype _Nonnull)initWithCustomEndpoint:(NSUrl * _Nullable)customEndpoint __attribute__((objc_designated_initializer));
		[Export ("initWithCustomEndpoint:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSUrl customEndpoint);

		// -(void)setEventMapper:(DDLogEvent * _Nullable (^ _Nonnull)(DDLogEvent * _Nonnull))mapper;
		[Export ("setEventMapper:")]
		void SetEventMapper (Func<DDLogEvent, DDLogEvent> mapper);
	}
}
