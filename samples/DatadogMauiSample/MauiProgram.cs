using Microsoft.Extensions.Logging;
using Datadog.Maui;
using Datadog.Maui.Configuration;
using DatadogMauiSample.Config;

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
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				// Register custom WebView handler for Android to enable Datadog tracking
				handlers.AddHandler<WebView, Platforms.Android.DatadogWebViewHandler>();
#endif
			});

		// Note: Datadog is initialized platform-specifically:
		// - Android: See Platforms/Android/MainApplication.cs
		// - iOS: See Platforms/iOS/AppDelegate.cs
		// This approach allows for platform-specific configuration using native SDK APIs.

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
