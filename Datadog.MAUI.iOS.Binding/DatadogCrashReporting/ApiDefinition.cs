using Foundation;

namespace DatadogMaui.iOS.CrashReporting
{
	// @interface DDCrashReporter
	interface DDCrashReporter
	{
		// +(void)enable;
		[Static]
		[Export ("enable")]
		void Enable ();
	}
}
