using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Datadog.Maui;
using Datadog.Maui.Configuration;
using DatadogMauiSample.Config;
using DatadogMauiSample.Services;

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
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				// Register custom WebView handler for Android to enable Datadog tracking
				handlers.AddHandler<WebView, Platforms.Android.DatadogWebViewHandler>();
#endif
			});

		builder.Services.AddSingleton<IDatadogInitializer, DatadogInitializer>();

		// Note: Datadog is initialized via DI at app construction with platform-specific SDK calls.

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		app.Services.GetRequiredService<IDatadogInitializer>().Initialize();
		return app;
	}
}
