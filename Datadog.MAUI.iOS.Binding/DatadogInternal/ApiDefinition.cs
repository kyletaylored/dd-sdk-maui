using System;
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS.DatadogInternal
{
	// @interface DDInternalLogger : NSObject
	[BaseType (typeof(NSObject))]
	interface DDInternalLogger
	{
		// +(void)consolePrint:(NSString * _Nonnull)message :(enum DDCoreLoggerLevel)level;
		[Static]
		[Export ("consolePrint::")]
		void ConsolePrint (string message, DDCoreLoggerLevel level);

		// +(void)telemetryDebugWithId:(NSString * _Nonnull)id message:(NSString * _Nonnull)message;
		[Static]
		[Export ("telemetryDebugWithId:message:")]
		void TelemetryDebugWithId (string id, string message);

		// +(void)telemetryErrorWithId:(NSString * _Nonnull)id message:(NSString * _Nonnull)message kind:(NSString * _Nullable)kind stack:(NSString * _Nullable)stack;
		[Static]
		[Export ("telemetryErrorWithId:message:kind:stack:")]
		void TelemetryErrorWithId (string id, string message, [NullAllowed] string kind, [NullAllowed] string stack);
	}

	// @interface DDTracingHeaderType : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDTracingHeaderType
	{
		// @property (readonly, nonatomic, strong, class) DDTracingHeaderType * _Nonnull datadog;
		[Static]
		[Export ("datadog", ArgumentSemantic.Strong)]
		DDTracingHeaderType Datadog { get; }

		// @property (readonly, nonatomic, strong, class) DDTracingHeaderType * _Nonnull b3multi;
		[Static]
		[Export ("b3multi", ArgumentSemantic.Strong)]
		DDTracingHeaderType B3multi { get; }

		// @property (readonly, nonatomic, strong, class) DDTracingHeaderType * _Nonnull b3;
		[Static]
		[Export ("b3", ArgumentSemantic.Strong)]
		DDTracingHeaderType B3 { get; }

		// @property (readonly, nonatomic, strong, class) DDTracingHeaderType * _Nonnull tracecontext;
		[Static]
		[Export ("tracecontext", ArgumentSemantic.Strong)]
		DDTracingHeaderType Tracecontext { get; }
	}
}
