using Microsoft.Extensions.Logging;
using Datadog.Maui;
using Datadog.Maui.Configuration;
using DatadogMauiSample.Config;

namespace DatadogMauiSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		// Load Datadog configuration from .env file
		DatadogConfig.LoadFromEnvironment();

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseDatadog(config =>
			{
#if ANDROID
				var clientToken = DatadogConfig.AndroidClientToken;
				var rumApplicationId = DatadogConfig.AndroidRumApplicationId;
#elif IOS
				var clientToken = DatadogConfig.IosClientToken;
				var rumApplicationId = DatadogConfig.IosRumApplicationId;
#else
				var clientToken = "";
				var rumApplicationId = "";
#endif

				config
					.WithClientToken(clientToken)
					.WithEnvironment(DatadogConfig.Environment)
					.WithServiceName(DatadogConfig.ServiceName)
					.WithSite(DatadogSite.US1)
					.WithRum(rum => rum
						.WithApplicationId(rumApplicationId)
						.WithSessionSampleRate(DatadogConfig.SessionSampleRate)
						.WithTelemetrySampleRate(100.0)
						.WithTrackUserInteractions(true)
						.WithVitalsUpdateFrequency(VitalsUpdateFrequency.Average)
					)
					.WithLogs(logs => logs
						.WithNetworkInfoEnabled(true)
						.WithBundleWithRum(true)
					)
					.WithTracing(tracing => tracing
						.WithSampleRate(100.0)
					)
					.WithFirstPartyHosts(DatadogConfig.FirstPartyHosts.ToArray())
					.WithTrackingConsent(TrackingConsent.Granted)
					.WithVerboseLogging(DatadogConfig.VerboseLogging);
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
