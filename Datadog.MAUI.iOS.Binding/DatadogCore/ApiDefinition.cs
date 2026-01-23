using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using Datadog.iOS.Internal;

namespace Datadog.iOS.Core
{
	// @interface DDCrossPlatformExtension : NSObject
	[BaseType (typeof(NSObject))]
	interface DDCrossPlatformExtension
	{
		// +(void)subscribeToSharedContext:(void (^ _Nonnull)(DDSharedContext * _Nullable))toSharedContext;
		[Static]
		[Export ("subscribeToSharedContext:")]
		void SubscribeToSharedContext (Action<DDSharedContext> toSharedContext);

		// +(void)unsubscribeFromSharedContext;
		[Static]
		[Export ("unsubscribeFromSharedContext")]
		void UnsubscribeFromSharedContext ();
	}

	// @interface DDSharedContext : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDSharedContext
	{
		// @property (readonly, copy, nonatomic) NSString * _Nullable userId;
		[NullAllowed, Export ("userId")]
		string UserId { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nullable accountId;
		[NullAllowed, Export ("accountId")]
		string AccountId { get; }
	}

	// @interface DDConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDConfiguration
	{
		// @property (copy, nonatomic) NSString * _Nonnull clientToken;
		[Export ("clientToken")]
		string ClientToken { get; set; }

		// @property (copy, nonatomic) NSString * _Nonnull env;
		[Export ("env")]
		string Env { get; set; }

		// @property (nonatomic, strong) DDSite * _Nonnull site;
		[Export ("site", ArgumentSemantic.Strong)]
		DDSite Site { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable version;
		[NullAllowed, Export ("version")]
		string Version { get; set; }

		// @property (nonatomic) enum DDBatchSize batchSize;
		[Export ("batchSize", ArgumentSemantic.Assign)]
		DDBatchSize BatchSize { get; set; }

		// @property (nonatomic) enum DDUploadFrequency uploadFrequency;
		[Export ("uploadFrequency", ArgumentSemantic.Assign)]
		DDUploadFrequency UploadFrequency { get; set; }

		// @property (nonatomic) enum DDBatchProcessingLevel batchProcessingLevel;
		[Export ("batchProcessingLevel", ArgumentSemantic.Assign)]
		DDBatchProcessingLevel BatchProcessingLevel { get; set; }

		// @property (copy, nonatomic) NSDictionary * _Nullable proxyConfiguration;
		[NullAllowed, Export ("proxyConfiguration", ArgumentSemantic.Copy)]
		NSDictionary ProxyConfiguration { get; set; }

		// -(void)setEncryption:(id<DDDataEncryption> _Nonnull)encryption;
		[Export ("setEncryption:")]
		void SetEncryption (DDDataEncryption encryption);

		// -(void)setServerDateProvider:(id<DDServerDateProvider> _Nonnull)serverDateProvider;
		[Export ("setServerDateProvider:")]
		void SetServerDateProvider (DDServerDateProvider serverDateProvider);

		// @property (nonatomic, strong) NSBundle * _Nonnull bundle;
		[Export ("bundle", ArgumentSemantic.Strong)]
		NSBundle Bundle { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nonnull additionalConfiguration;
		[Export ("additionalConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> AdditionalConfiguration { get; set; }

		// @property (nonatomic) BOOL backgroundTasksEnabled;
		[Export ("backgroundTasksEnabled")]
		bool BackgroundTasksEnabled { get; set; }

		// -(instancetype _Nonnull)initWithClientToken:(NSString * _Nonnull)clientToken env:(NSString * _Nonnull)env __attribute__((objc_designated_initializer));
		[Export ("initWithClientToken:env:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string clientToken, string env);
	}

	// @protocol DDDataEncryption
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/
	[Protocol]
	[BaseType(typeof(NSObject))]
	interface DDDataEncryption
	{
		// @required -(NSData * _Nullable)encryptWithData:(NSData * _Nonnull)data error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("encryptWithData:error:")]
		[return: NullAllowed]
		NSData EncryptWithData (NSData data, [NullAllowed] out NSError error);

		// @required -(NSData * _Nullable)decryptWithData:(NSData * _Nonnull)data error:(NSError * _Nullable * _Nullable)error __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("decryptWithData:error:")]
		[return: NullAllowed]
		NSData DecryptWithData (NSData data, [NullAllowed] out NSError error);
	}

	// @interface DDDatadog : NSObject
	[BaseType (typeof(NSObject))]
	interface DDDatadog
	{
		// +(void)initializeWithConfiguration:(DDConfiguration * _Nonnull)configuration trackingConsent:(DDTrackingConsent * _Nonnull)trackingConsent;
		[Static]
		[Export ("initializeWithConfiguration:trackingConsent:")]
		void InitializeWithConfiguration (DDConfiguration configuration, DDTrackingConsent trackingConsent);

		// +(enum DDCoreLoggerLevel)verbosityLevel __attribute__((warn_unused_result("")));
		// +(void)setVerbosityLevel:(enum DDCoreLoggerLevel)verbosityLevel;
		[Static]
		[Export ("verbosityLevel")]
		
		DDCoreLoggerLevel VerbosityLevel { get; set; }

		// +(void)setUserInfoWithUserId:(NSString * _Nonnull)userId name:(NSString * _Nullable)name email:(NSString * _Nullable)email extraInfo:(NSDictionary<NSString *,id> * _Nonnull)extraInfo;
		[Static]
		[Export ("setUserInfoWithUserId:name:email:extraInfo:")]
		void SetUserInfoWithUserId (string userId, [NullAllowed] string name, [NullAllowed] string email, NSDictionary<NSString, NSObject> extraInfo);

		// +(void)clearUserInfo;
		[Static]
		[Export ("clearUserInfo")]
		void ClearUserInfo ();

		// +(void)addUserExtraInfo:(NSDictionary<NSString *,id> * _Nonnull)extraInfo;
		[Static]
		[Export ("addUserExtraInfo:")]
		void AddUserExtraInfo (NSDictionary<NSString, NSObject> extraInfo);

		// +(void)setAccountInfoWithAccountId:(NSString * _Nonnull)accountId name:(NSString * _Nullable)name extraInfo:(NSDictionary<NSString *,id> * _Nonnull)extraInfo;
		[Static]
		[Export ("setAccountInfoWithAccountId:name:extraInfo:")]
		void SetAccountInfoWithAccountId (string accountId, [NullAllowed] string name, NSDictionary<NSString, NSObject> extraInfo);

		// +(void)addAccountExtraInfo:(NSDictionary<NSString *,id> * _Nonnull)extraInfo;
		[Static]
		[Export ("addAccountExtraInfo:")]
		void AddAccountExtraInfo (NSDictionary<NSString, NSObject> extraInfo);

		// +(void)clearAccountInfo;
		[Static]
		[Export ("clearAccountInfo")]
		void ClearAccountInfo ();

		// +(void)setTrackingConsentWithConsent:(DDTrackingConsent * _Nonnull)consent;
		[Static]
		[Export ("setTrackingConsentWithConsent:")]
		void SetTrackingConsentWithConsent (DDTrackingConsent consent);

		// +(BOOL)isInitialized __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isInitialized")]
		
		bool IsInitialized { get; }

		// +(void)stopInstance;
		[Static]
		[Export ("stopInstance")]
		void StopInstance ();

		// +(void)clearAllData;
		[Static]
		[Export ("clearAllData")]
		void ClearAllData ();
	}

	// @interface DDSite : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDSite
	{
		// +(DDSite * _Nonnull)us1 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("us1")]
		
		DDSite Us1 { get; }

		// +(DDSite * _Nonnull)us3 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("us3")]
		
		DDSite Us3 { get; }

		// +(DDSite * _Nonnull)us5 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("us5")]
		
		DDSite Us5 { get; }

		// +(DDSite * _Nonnull)eu1 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("eu1")]
		
		DDSite Eu1 { get; }

		// +(DDSite * _Nonnull)ap1 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("ap1")]
		
		DDSite Ap1 { get; }

		// +(DDSite * _Nonnull)ap2 __attribute__((warn_unused_result("")));
		[Static]
		[Export ("ap2")]
		
		DDSite Ap2 { get; }

		// +(DDSite * _Nonnull)us1_fed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("us1_fed")]
		
		DDSite Us1_fed { get; }
	}

	// @protocol DDServerDateProvider
	/*
  Check whether adding [Model] to this declaration is appropriate.
  [Model] is used to generate a C# class that implements this protocol,
  and might be useful for protocols that consumers are supposed to implement,
  since consumers can subclass the generated class instead of implementing
  the generated interface. If consumers are not supposed to implement this
  protocol, then [Model] is redundant and will generate code that will never
  be used.
*/
	[Protocol]
	[BaseType(typeof(NSObject))]
	interface DDServerDateProvider
	{
		// @required -(void)synchronizeWithUpdate:(void (^ _Nonnull)(NSTimeInterval))update;
		[Abstract]
		[Export ("synchronizeWithUpdate:")]
		void SynchronizeWithUpdate (Action<double> update);
	}

	// @interface DDTrackingConsent : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDTrackingConsent
	{
		// +(DDTrackingConsent * _Nonnull)granted __attribute__((warn_unused_result("")));
		[Static]
		[Export ("granted")]
		
		DDTrackingConsent Granted { get; }

		// +(DDTrackingConsent * _Nonnull)notGranted __attribute__((warn_unused_result("")));
		[Static]
		[Export ("notGranted")]
		
		DDTrackingConsent NotGranted { get; }

		// +(DDTrackingConsent * _Nonnull)pending __attribute__((warn_unused_result("")));
		[Static]
		[Export ("pending")]
		
		DDTrackingConsent Pending { get; }
	}

	// @interface DDURLSessionInstrumentation : NSObject
	[BaseType (typeof(NSObject))]
	interface DDURLSessionInstrumentation
	{
		// +(void)enableWithConfiguration:(DDURLSessionInstrumentationConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWithConfiguration:")]
		void EnableWithConfiguration (DDURLSessionInstrumentationConfiguration configuration);

		// +(void)disableWithDelegateClass:(Class<INSUrlSessionDataDelegate> _Nonnull)delegateClass;
		[Static]
		[Export ("disableWithDelegateClass:")]
		void DisableWithDelegateClass (INSUrlSessionDataDelegate delegateClass);
	}

	// @interface DDURLSessionInstrumentationConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDURLSessionInstrumentationConfiguration
	{
		// -(instancetype _Nonnull)initWithDelegateClass:(Class<INSUrlSessionDataDelegate> _Nonnull)delegateClass __attribute__((objc_designated_initializer));
		[Export ("initWithDelegateClass:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSUrlSessionDataDelegate delegateClass);

		// -(void)setFirstPartyHostsTracing:(DDURLSessionInstrumentationFirstPartyHostsTracing * _Nonnull)firstPartyHostsTracing;
		[Export ("setFirstPartyHostsTracing:")]
		void SetFirstPartyHostsTracing (DDURLSessionInstrumentationFirstPartyHostsTracing firstPartyHostsTracing);

		// @property (nonatomic) Class<INSUrlSessionDataDelegate> _Nonnull delegateClass;
		[Export ("delegateClass", ArgumentSemantic.Assign)]
		INSUrlSessionDataDelegate DelegateClass { get; set; }
	}

	// @interface DDURLSessionInstrumentationFirstPartyHostsTracing : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDURLSessionInstrumentationFirstPartyHostsTracing
	{
		// -(instancetype _Nonnull)initWithHostsWithHeaderTypes:(NSDictionary<NSString *,NSSet<DDTracingHeaderType *> *> * _Nonnull)hostsWithHeaderTypes __attribute__((objc_designated_initializer));
		[Export ("initWithHostsWithHeaderTypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary<NSString, NSSet<DDTracingHeaderType>> hostsWithHeaderTypes);

		// -(instancetype _Nonnull)initWithHosts:(NSSet<NSString *> * _Nonnull)hosts __attribute__((objc_designated_initializer));
		[Export ("initWithHosts:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSSet<NSString> hosts);
	}
}
