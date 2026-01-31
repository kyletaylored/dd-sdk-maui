using System;
using Datadog.iOS.Core;
using Datadog.iOS.Internal;
using Foundation;
using ObjCRuntime;


namespace Datadog.iOS.Trace
{
	// @interface OT : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC12DatadogTrace2OT")]
	interface OT
	{
		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull formatTextMap;
		[Static]
		[Export ("formatTextMap")]
		string FormatTextMap { get; }
	}

	// @interface DDB3HTTPHeadersWriter : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDB3HTTPHeadersWriter
	{
		// @property (readonly, copy, nonatomic) NSDictionary<NSString *,NSString *> * _Nonnull traceHeaderFields;
		[Export ("traceHeaderFields", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> TraceHeaderFields { get; }

		// -(instancetype _Nonnull)initWithInjectEncoding:(enum DDInjectEncoding)injectEncoding traceContextInjection:(enum DDTraceContextInjection)traceContextInjection __attribute__((objc_designated_initializer));
		[Export ("initWithInjectEncoding:traceContextInjection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDInjectEncoding injectEncoding, DDTraceContextInjection traceContextInjection);
	}

	// @interface DDHTTPHeadersWriter : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDHTTPHeadersWriter
	{
		// @property (readonly, copy, nonatomic) NSDictionary<NSString *,NSString *> * _Nonnull traceHeaderFields;
		[Export ("traceHeaderFields", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> TraceHeaderFields { get; }

		// -(instancetype _Nonnull)initWithTraceContextInjection:(enum DDTraceContextInjection)traceContextInjection __attribute__((objc_designated_initializer));
		[Export ("initWithTraceContextInjection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDTraceContextInjection traceContextInjection);
	}

	// @protocol OTSpan
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
	interface OTSpan
	{
		// @required @property (readonly, nonatomic, strong) id<OTSpanContext> _Nonnull context;
		[Abstract]
		[Export ("context", ArgumentSemantic.Strong)]
		OTSpanContext Context { get; }

		// @required @property (readonly, nonatomic, strong) id<OTTracer> _Nonnull tracer;
		[Abstract]
		[Export ("tracer", ArgumentSemantic.Strong)]
		OTTracer Tracer { get; }

		// @required -(void)setOperationName:(NSString * _Nonnull)operationName;
		[Abstract]
		[Export ("setOperationName:")]
		void SetOperationName (string operationName);

		// @required -(void)setTag:(NSString * _Nonnull)key value:(NSString * _Nonnull)value;
		[Abstract]
		[Export ("setTag:value:")]
		void SetTag (string key, string value);

		// @required -(void)setTag:(NSString * _Nonnull)key numberValue:(NSNumber * _Nonnull)numberValue;
		[Abstract]
		[Export ("setTag:numberValue:")]
		void SetTag (string key, NSNumber numberValue);

		// @required -(void)setTag:(NSString * _Nonnull)key boolValue:(BOOL)boolValue;
		[Abstract]
		[Export ("setTag:boolValue:")]
		void SetTag (string key, bool boolValue);

		// @required -(void)log:(NSDictionary<NSString *,NSObject *> * _Nonnull)fields;
		[Abstract]
		[Export ("log:")]
		void Log (NSDictionary<NSString, NSObject> fields);

		// @required -(void)log:(NSDictionary<NSString *,NSObject *> * _Nonnull)fields timestamp:(NSDate * _Nullable)timestamp;
		[Abstract]
		[Export ("log:timestamp:")]
		void Log (NSDictionary<NSString, NSObject> fields, [NullAllowed] NSDate timestamp);

		// @required -(id<OTSpan> _Nonnull)setBaggageItem:(NSString * _Nonnull)key value:(NSString * _Nonnull)value __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("setBaggageItem:value:")]
		OTSpan SetBaggageItem (string key, string value);

		// @required -(NSString * _Nullable)getBaggageItem:(NSString * _Nonnull)key __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("getBaggageItem:")]
		[return: NullAllowed]
		string GetBaggageItem (string key);

		// @required -(void)setError:(NSError * _Nonnull)error;
		[Abstract]
		[Export ("setError:")]
		void SetError (NSError error);

		// @required -(void)setErrorWithKind:(NSString * _Nonnull)kind message:(NSString * _Nonnull)message stack:(NSString * _Nullable)stack;
		[Abstract]
		[Export ("setErrorWithKind:message:stack:")]
		void SetErrorWithKind (string kind, string message, [NullAllowed] string stack);

		// @required -(void)finish;
		[Abstract]
		[Export ("finish")]
		void Finish ();

		// @required -(void)finishWithTime:(NSDate * _Nullable)finishTime;
		[Abstract]
		[Export ("finishWithTime:")]
		void FinishWithTime ([NullAllowed] NSDate finishTime);

		// @required -(id<OTSpan> _Nonnull)setActive;
		[Abstract]
		[Export ("setActive")]
		
		OTSpan SetActive { get; }
	}

	// @protocol OTSpanContext
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
	interface OTSpanContext
	{
		// @required -(void)forEachBaggageItem:(BOOL (^ _Nonnull)(NSString * _Nonnull, NSString * _Nonnull))callback;
		[Abstract]
		[Export ("forEachBaggageItem:")]
		void ForEachBaggageItem (Func<NSString, NSString, bool> callback);
	}

	// @protocol OTTracer
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
	interface OTTracer
	{
		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:")]
		OTSpan StartSpan (string operationName);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName tags:(NSDictionary * _Nullable)tags __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:tags:")]
		OTSpan StartSpan (string operationName, [NullAllowed] NSDictionary tags);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(NSDictionary * _Nullable)tags __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:tags:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, [NullAllowed] NSDictionary tags);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(NSDictionary * _Nullable)tags startTime:(NSDate * _Nullable)startTime __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:tags:startTime:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, [NullAllowed] NSDictionary tags, [NullAllowed] NSDate startTime);

		// @required -(id<OTSpan> _Nonnull)startRootSpan:(NSString * _Nonnull)operationName tags:(NSDictionary * _Nullable)tags startTime:(NSDate * _Nullable)startTime customSampleRate:(NSNumber * _Nullable)customSampleRate __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startRootSpan:tags:startTime:customSampleRate:")]
		OTSpan StartRootSpan (string operationName, [NullAllowed] NSDictionary tags, [NullAllowed] NSDate startTime, [NullAllowed] NSNumber customSampleRate);

		// @required -(BOOL)inject:(id<OTSpanContext> _Nonnull)spanContext format:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(NSError * _Nullable * _Nullable)error;
		[Abstract]
		[Export ("inject:format:carrier:error:")]
		bool Inject (OTSpanContext spanContext, string format, NSObject carrier, [NullAllowed] out NSError error);

		// @required -(BOOL)extractWithFormat:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(NSError * _Nullable * _Nullable)error;
		[Abstract]
		[Export ("extractWithFormat:carrier:error:")]
		bool ExtractWithFormat (string format, NSObject carrier, [NullAllowed] out NSError error);
	}

	// @interface DDTrace : NSObject
	[BaseType (typeof(NSObject))]
	interface DDTrace
	{
		// +(void)enableWith:(DDTraceConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWith:")]
		void EnableWith (DDTraceConfiguration configuration);
	}

	// @interface DDTraceConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	interface DDTraceConfiguration
	{
		// @property (nonatomic) float sampleRate;
		[Export ("sampleRate")]
		float SampleRate { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString *,id> * _Nullable tags;
		[NullAllowed, Export ("tags", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Tags { get; set; }

		// -(void)setURLSessionTracking:(DDTraceURLSessionTracking * _Nonnull)tracking;
		[Export ("setURLSessionTracking:")]
		void SetURLSessionTracking (DDTraceURLSessionTracking tracking);

		// @property (nonatomic) BOOL bundleWithRumEnabled;
		[Export ("bundleWithRumEnabled")]
		bool BundleWithRumEnabled { get; set; }

		// @property (nonatomic) BOOL networkInfoEnabled;
		[Export ("networkInfoEnabled")]
		bool NetworkInfoEnabled { get; set; }

		// @property (copy, nonatomic) NSUrl * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSUrl CustomEndpoint { get; set; }
	}

	// @interface DDTraceFirstPartyHostsTracing : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDTraceFirstPartyHostsTracing
	{
		// -(instancetype _Nonnull)initWithHostsWithHeaderTypes:(NSDictionary<NSString *,NSSet<DDTracingHeaderType *> *> * _Nonnull)hostsWithHeaderTypes __attribute__((objc_designated_initializer));
		[Export ("initWithHostsWithHeaderTypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary<NSString, NSSet<DDTracingHeaderType>> hostsWithHeaderTypes);

		// -(instancetype _Nonnull)initWithHostsWithHeaderTypes:(NSDictionary<NSString *,NSSet<DDTracingHeaderType *> *> * _Nonnull)hostsWithHeaderTypes sampleRate:(float)sampleRate __attribute__((objc_designated_initializer));
		[Export ("initWithHostsWithHeaderTypes:sampleRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary<NSString, NSSet<DDTracingHeaderType>> hostsWithHeaderTypes, float sampleRate);

		// -(instancetype _Nonnull)initWithHosts:(NSSet<NSString *> * _Nonnull)hosts __attribute__((objc_designated_initializer));
		[Export ("initWithHosts:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSSet<NSString> hosts);

		// -(instancetype _Nonnull)initWithHosts:(NSSet<NSString *> * _Nonnull)hosts sampleRate:(float)sampleRate __attribute__((objc_designated_initializer));
		[Export ("initWithHosts:sampleRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSSet<NSString> hosts, float sampleRate);
	}

	// @interface DDTraceURLSessionTracking : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDTraceURLSessionTracking
	{
		// -(instancetype _Nonnull)initWithFirstPartyHostsTracing:(DDTraceFirstPartyHostsTracing * _Nonnull)firstPartyHostsTracing __attribute__((objc_designated_initializer));
		[Export ("initWithFirstPartyHostsTracing:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDTraceFirstPartyHostsTracing firstPartyHostsTracing);

		// -(void)setFirstPartyHostsTracing:(DDTraceFirstPartyHostsTracing * _Nonnull)firstPartyHostsTracing;
		[Export ("setFirstPartyHostsTracing:")]
		void SetFirstPartyHostsTracing (DDTraceFirstPartyHostsTracing firstPartyHostsTracing);
	}

	// @interface DDTracer : NSObject <OTTracer>
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDTracer : OTTracer
	{
		// +(id<OTTracer> _Nonnull)shared __attribute__((warn_unused_result("")));
		// Note: Returns DDTracer (concrete type) not OTTracer (protocol)
		[Static]
		[Export ("shared")]
		DDTracer Shared { get; }

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName __attribute__((warn_unused_result("")));
		[Export ("startSpan:")]
		new OTSpan StartSpan (string operationName);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName tags:(NSDictionary * _Nullable)tags __attribute__((warn_unused_result("")));
		[Export ("startSpan:tags:")]
		new OTSpan StartSpan (string operationName, [NullAllowed] NSDictionary tags);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:")]
		new OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(NSDictionary * _Nullable)tags __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:tags:")]
		new OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, [NullAllowed] NSDictionary tags);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(NSDictionary * _Nullable)tags startTime:(NSDate * _Nullable)startTime __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:tags:startTime:")]
		new OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, [NullAllowed] NSDictionary tags, [NullAllowed] NSDate startTime);

		// -(id<OTSpan> _Nonnull)startRootSpan:(NSString * _Nonnull)operationName tags:(NSDictionary * _Nullable)tags startTime:(NSDate * _Nullable)startTime customSampleRate:(NSNumber * _Nullable)customSampleRate __attribute__((warn_unused_result("")));
		[Export ("startRootSpan:tags:startTime:customSampleRate:")]
		new OTSpan StartRootSpan (string operationName, [NullAllowed] NSDictionary tags, [NullAllowed] NSDate startTime, [NullAllowed] NSNumber customSampleRate);

		// -(BOOL)inject:(id<OTSpanContext> _Nonnull)spanContext format:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(NSError * _Nullable * _Nullable)error;
		[Export ("inject:format:carrier:error:")]
		new bool Inject (OTSpanContext spanContext, string format, NSObject carrier, [NullAllowed] out NSError error);

		// -(BOOL)extractWithFormat:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(NSError * _Nullable * _Nullable)error;
		[Export ("extractWithFormat:carrier:error:")]
		new bool ExtractWithFormat (string format, NSObject carrier, [NullAllowed] out NSError error);
	}

	// @interface DDW3CHTTPHeadersWriter : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDW3CHTTPHeadersWriter
	{
		// @property (readonly, copy, nonatomic) NSDictionary<NSString *,NSString *> * _Nonnull traceHeaderFields;
		[Export ("traceHeaderFields", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> TraceHeaderFields { get; }

		// -(instancetype _Nonnull)initWithTraceContextInjection:(enum DDTraceContextInjection)traceContextInjection __attribute__((objc_designated_initializer));
		[Export ("initWithTraceContextInjection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDTraceContextInjection traceContextInjection);
	}
}
