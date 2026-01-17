using DatadogTrace;
using Foundation;
using ObjCRuntime;

namespace DatadogMaui.iOS.Trace
{
	// @interface OT
	interface OT
	{
		// @property (readonly, copy, nonatomic, class) NSString * _Nonnull formatTextMap;
		[Static]
		[Export ("formatTextMap")]
		string FormatTextMap { get; }
	}

	// @interface DDB3HTTPHeadersWriter
	[DisableDefaultCtor]
	interface DDB3HTTPHeadersWriter
	{
		// -(instancetype _Nonnull)initWithInjectEncoding:(enum DDInjectEncoding)injectEncoding traceContextInjection:(enum DDTraceContextInjection)traceContextInjection __attribute__((objc_designated_initializer));
		[Export ("initWithInjectEncoding:traceContextInjection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDInjectEncoding injectEncoding, DDTraceContextInjection traceContextInjection);
	}

	// @interface DDHTTPHeadersWriter
	[DisableDefaultCtor]
	interface DDHTTPHeadersWriter
	{
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
*/[Protocol]
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

		// @required -(void)setTag:(NSString * _Nonnull)key boolValue:(id)boolValue;
		[Abstract]
		[Export ("setTag:boolValue:")]
		void SetTag (string key, NSObject boolValue);

		// @required -(void)log:(id)fields;
		[Abstract]
		[Export ("log:")]
		void Log (NSObject fields);

		// @required -(void)log:(id)fields timestamp:(NSDate * _Nullable)timestamp;
		[Abstract]
		[Export ("log:timestamp:")]
		void Log (NSObject fields, [NullAllowed] NSDate timestamp);

		// @required -(id<OTSpan> _Nonnull)setBaggageItem:(NSString * _Nonnull)key value:(NSString * _Nonnull)value __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("setBaggageItem:value:")]
		OTSpan SetBaggageItem (string key, string value);

		// @required -(NSString * _Nullable)getBaggageItem:(NSString * _Nonnull)key __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("getBaggageItem:")]
		[return: NullAllowed]
		string GetBaggageItem (string key);

		// @required -(void)setError:(id)error;
		[Abstract]
		[Export ("setError:")]
		void SetError (NSObject error);

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
		[Verify (MethodToProperty)]
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
*/[Protocol]
	interface OTSpanContext
	{
		// @required -(void)forEachBaggageItem:(int)callback;
		[Abstract]
		[Export ("forEachBaggageItem:")]
		void ForEachBaggageItem (int callback);
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
*/[Protocol]
	interface OTTracer
	{
		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:")]
		OTSpan StartSpan (string operationName);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName tags:(id)tags __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:tags:")]
		OTSpan StartSpan (string operationName, NSObject tags);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(id)tags __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:tags:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, NSObject tags);

		// @required -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(id)tags startTime:(NSDate * _Nullable)startTime __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startSpan:childOf:tags:startTime:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, NSObject tags, [NullAllowed] NSDate startTime);

		// @required -(id<OTSpan> _Nonnull)startRootSpan:(NSString * _Nonnull)operationName tags:(id)tags startTime:(NSDate * _Nullable)startTime customSampleRate:(NSNumber * _Nullable)customSampleRate __attribute__((warn_unused_result("")));
		[Abstract]
		[Export ("startRootSpan:tags:startTime:customSampleRate:")]
		OTSpan StartRootSpan (string operationName, NSObject tags, [NullAllowed] NSDate startTime, [NullAllowed] NSNumber customSampleRate);

		// @required -(id)inject:(id<OTSpanContext> _Nonnull)spanContext format:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(id)error;
		[Abstract]
		[Export ("inject:format:carrier:error:")]
		NSObject Inject (OTSpanContext spanContext, string format, NSObject carrier, NSObject error);

		// @required -(id)extractWithFormat:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(id)error;
		[Abstract]
		[Export ("extractWithFormat:carrier:error:")]
		NSObject ExtractWithFormat (string format, NSObject carrier, NSObject error);
	}

	// @interface DDTrace
	interface DDTrace
	{
		// +(void)enableWith:(DDTraceConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWith:")]
		void EnableWith (DDTraceConfiguration configuration);
	}

	// @interface DDTraceConfiguration
	interface DDTraceConfiguration
	{
		// @property (nonatomic) float sampleRate;
		[Export ("sampleRate")]
		float SampleRate { get; set; }

		// @property (copy, nonatomic) NSString * _Nullable service;
		[NullAllowed, Export ("service")]
		string Service { get; set; }

		// -(void)setURLSessionTracking:(DDTraceURLSessionTracking * _Nonnull)tracking;
		[Export ("setURLSessionTracking:")]
		void SetURLSessionTracking (DDTraceURLSessionTracking tracking);

		// @property (nonatomic) int bundleWithRumEnabled;
		[Export ("bundleWithRumEnabled")]
		int BundleWithRumEnabled { get; set; }

		// @property (nonatomic) int networkInfoEnabled;
		[Export ("networkInfoEnabled")]
		int NetworkInfoEnabled { get; set; }

		// @property (copy, nonatomic) NSURL * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSURL CustomEndpoint { get; set; }
	}

	// @interface DDTraceFirstPartyHostsTracing
	[DisableDefaultCtor]
	interface DDTraceFirstPartyHostsTracing
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

	// @interface DDTraceURLSessionTracking
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

	// @interface DDTracer <OTTracer>
	[DisableDefaultCtor]
	interface DDTracer : IOTTracer
	{
		// +(id<OTTracer> _Nonnull)shared __attribute__((warn_unused_result("")));
		[Static]
		[Export ("shared")]
		[Verify (MethodToProperty)]
		OTTracer Shared { get; }

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName __attribute__((warn_unused_result("")));
		[Export ("startSpan:")]
		OTSpan StartSpan (string operationName);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName tags:(id)tags __attribute__((warn_unused_result("")));
		[Export ("startSpan:tags:")]
		OTSpan StartSpan (string operationName, NSObject tags);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(id)tags __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:tags:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, NSObject tags);

		// -(id<OTSpan> _Nonnull)startSpan:(NSString * _Nonnull)operationName childOf:(id<OTSpanContext> _Nullable)parent tags:(id)tags startTime:(NSDate * _Nullable)startTime __attribute__((warn_unused_result("")));
		[Export ("startSpan:childOf:tags:startTime:")]
		OTSpan StartSpan (string operationName, [NullAllowed] OTSpanContext parent, NSObject tags, [NullAllowed] NSDate startTime);

		// -(id<OTSpan> _Nonnull)startRootSpan:(NSString * _Nonnull)operationName tags:(id)tags startTime:(NSDate * _Nullable)startTime customSampleRate:(NSNumber * _Nullable)customSampleRate __attribute__((warn_unused_result("")));
		[Export ("startRootSpan:tags:startTime:customSampleRate:")]
		OTSpan StartRootSpan (string operationName, NSObject tags, [NullAllowed] NSDate startTime, [NullAllowed] NSNumber customSampleRate);

		// -(id)inject:(id<OTSpanContext> _Nonnull)spanContext format:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(id)error;
		[Export ("inject:format:carrier:error:")]
		NSObject Inject (OTSpanContext spanContext, string format, NSObject carrier, NSObject error);

		// -(id)extractWithFormat:(NSString * _Nonnull)format carrier:(id _Nonnull)carrier error:(id)error;
		[Export ("extractWithFormat:carrier:error:")]
		NSObject ExtractWithFormat (string format, NSObject carrier, NSObject error);
	}

	// @interface DDW3CHTTPHeadersWriter
	[DisableDefaultCtor]
	interface DDW3CHTTPHeadersWriter
	{
		// -(instancetype _Nonnull)initWithTraceContextInjection:(enum DDTraceContextInjection)traceContextInjection __attribute__((objc_designated_initializer));
		[Export ("initWithTraceContextInjection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DDTraceContextInjection traceContextInjection);
	}
}
