using Microsoft.Extensions.Logging;
// Uncomment when Datadog.MAUI package is available:
// using Datadog.MAUI;

namespace DatadogMauiSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Initialize Datadog SDK
		// Uncomment and configure when Datadog.MAUI package is built:
		/*
		DatadogSdk.Initialize(new DatadogConfiguration
		{
			ClientToken = "YOUR_CLIENT_TOKEN_HERE",
			Environment = "dev",
			ApplicationId = "YOUR_APP_ID_HERE",
			ServiceName = "datadog-maui-sample",
			Site = DatadogSite.US1,
			EnableCrashReporting = true,
			TrackUserInteractions = true,
			TrackNetworkRequests = true,
			SessionSampleRate = 100.0f,
		});
		*/

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
