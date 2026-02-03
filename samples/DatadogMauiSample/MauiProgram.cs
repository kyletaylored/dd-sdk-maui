using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Datadog.Maui;
using Datadog.Maui.Configuration;
using Datadog.Maui.Extensions;
using DatadogMauiSample.Configuration;

namespace DatadogMauiSample;

/// <summary>
/// Entry point for the MAUI application configuration.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Creates and configures the MAUI application.
	/// </summary>
	/// <returns>The configured <see cref="MauiApp"/>.</returns>
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		// Configure appsettings.json loading
		var assembly = Assembly.GetExecutingAssembly();
		using var appsettingsStream = assembly.GetManifestResourceStream("DatadogMauiSample.appsettings.json");

		if (appsettingsStream != null)
		{
			var configBuilder = new ConfigurationBuilder()
				.AddJsonStream(appsettingsStream);

			// Add appsettings.Development.json if it exists (embedded resource)
			using var developmentStream = assembly.GetManifestResourceStream("DatadogMauiSample.appsettings.Development.json");
			if (developmentStream != null)
			{
				configBuilder.AddJsonStream(developmentStream);
			}

			var config = configBuilder.Build();
			builder.Configuration.AddConfiguration(config);

			// Initialize static DatadogConfig with IConfiguration
			Config.DatadogConfig.Initialize(config);
		}

		// Load Datadog settings from configuration
		var datadogSettings = new DatadogSettings();
		builder.Configuration.GetSection("Datadog").Bind(datadogSettings);

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				// Register custom WebView handler for Android to enable Datadog tracking
				handlers.AddHandler<WebView, Platforms.Android.DatadogWebViewHandler>();
#endif
			})
			// Configure Datadog using the unified builder pattern API
			.UseDatadog(datadog =>
			{
				// Core configuration - use helper method to set platform-specific tokens
				datadog.SetClientToken(
					android: datadogSettings.Android.ClientToken ?? string.Empty,
					ios: datadogSettings.iOS.ClientToken ?? string.Empty
				);

				datadog.Environment = datadogSettings.Environment;
				datadog.ServiceName = datadogSettings.ServiceName ?? "shopist-maui-demo";

				// Parse site from string
				datadog.Site = datadogSettings.Site.ToUpperInvariant() switch
				{
					"US1" => DatadogSite.US1,
					"US3" => DatadogSite.US3,
					"US5" => DatadogSite.US5,
					"EU1" => DatadogSite.EU1,
					"AP1" => DatadogSite.AP1,
					"US1_FED" => DatadogSite.US1_FED,
					_ => DatadogSite.US1
				};

				datadog.TrackingConsent = TrackingConsent.Granted;
				datadog.VerboseLogging = datadogSettings.VerboseLogging;

				// Configure first-party hosts for distributed tracing
				datadog.FirstPartyHosts = datadogSettings.FirstPartyHosts;

				// Enable RUM (Real User Monitoring)
				if (datadogSettings.Rum != null)
				{
					datadog.EnableRum(rum =>
					{
						// Use helper method to set platform-specific RUM application IDs
						rum.SetApplicationId(
							android: datadogSettings.Android.RumApplicationId ?? string.Empty,
							ios: datadogSettings.iOS.RumApplicationId ?? string.Empty
						);

						rum.SetSessionSampleRate(datadogSettings.Rum.SessionSampleRate);
						rum.SetTelemetrySampleRate(datadogSettings.Rum.TelemetrySampleRate);
						rum.TrackViewsAutomatically(true);
						rum.TrackUserInteractions(datadogSettings.Rum.TrackUserInteractions);
						rum.TrackResources(true);
						rum.TrackErrors(true);
						rum.SetVitalsUpdateFrequency(VitalsUpdateFrequency.Frequent);
					});
				}

				// Enable Logs
				if (datadogSettings.Logs?.Enabled == true)
				{
					datadog.EnableLogs(logs =>
					{
						// Use default logs configuration
					});
				}

				// Enable APM Tracing
				if (datadogSettings.Tracing != null)
				{
					datadog.EnableTracing(tracing =>
					{
						tracing.SetSampleRate(datadogSettings.Tracing.SampleRate);
						tracing.SetFirstPartyHosts(datadogSettings.FirstPartyHosts);
						tracing.EnableTraceIdGeneration(true);
					});
				}

				// Enable Session Replay
				datadog.EnableSessionReplay(sessionReplay =>
				{
					sessionReplay.SetSampleRate(20); // 20% of sessions
					sessionReplay.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs);
					sessionReplay.SetImagePrivacy(ImagePrivacy.MaskNonBundledOnly);
					sessionReplay.SetTouchPrivacy(TouchPrivacy.Show);
				});
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
