using Foundation;

namespace Datadog.iOS.CrashReporting
{
	// @interface DDCrashReporter : NSObject
	[BaseType (typeof(NSObject))]
	interface DDCrashReporter
	{
		// +(void)enable;
		[Static]
		[Export ("enable")]
		void Enable ();
	}
}
