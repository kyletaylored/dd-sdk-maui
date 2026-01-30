using ObjCRuntime;
using UIKit;

namespace DatadogMauiSample;

/// <summary>
/// iOS application entry point.
/// </summary>
public class Program
{
	/// <summary>
	/// Main entry point of the iOS application.
	/// </summary>
	/// <param name="args">Command line arguments.</param>
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
