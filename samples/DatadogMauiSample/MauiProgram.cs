using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Datadog.Maui;
using Datadog.Maui.Configuration;
using Datadog.Maui.Extensions;
using DatadogMauiSample.Config;

namespace DatadogMauiSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		// Load appsettings.json from bundled MauiAsset into Configuration
		LoadAppSettings(builder.Configuration);

		// Populate DatadogConfig from the Configuration system
		DatadogConfig.Load(builder.Configuration);

		#if ANDROID
			string serviceName = DatadogConfig.ServiceName;
			string clientToken = DatadogConfig.AndroidClientToken;
			string rumApplicationId = DatadogConfig.AndroidRumApplicationId;
		#elif IOS
			string serviceName = DatadogConfig.ServiceName;
			string clientToken = DatadogConfig.IosClientToken;
			string rumApplicationId = DatadogConfig.IosRumApplicationId;
		#endif

		var ddSite = GetDatadogSite(DatadogConfig.Site);

		// Diagnostic logging to verify credentials
		Console.WriteLine($"[Datadog] === SDK Configuration ===");
		Console.WriteLine($"[Datadog] Client Token: {(clientToken?.Length > 10 ? clientToken[..10] + "..." : clientToken ?? "NULL")}");
		Console.WriteLine($"[Datadog] Application ID: {rumApplicationId ?? "NULL"}");
		Console.WriteLine($"[Datadog] Environment: {DatadogConfig.Environment}");
		Console.WriteLine($"[Datadog] Service Name: {serviceName}");
		Console.WriteLine($"[Datadog] Site: {ddSite}");

		builder
			.UseDatadog(config =>
			{
				config.ClientToken = clientToken;
				config.Environment = DatadogConfig.Environment;
				config.ServiceName = serviceName;
				config.Site = ddSite;
				config.TrackingConsent = TrackingConsent.Granted;
				config.VerboseLogging = DatadogConfig.VerboseLogging;

				config.EnableRum(rum =>
				{
					rum.SetApplicationId(rumApplicationId);
					rum.TrackUserInteractions(true);
					rum.TrackViewsAutomatically(true);
					rum.SetSessionSampleRate((int)DatadogConfig.SessionSampleRate);
				});

				config.EnableLogs(logs => logs.SetSampleRate(100));
			})
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Enable Session Replay via platform-native APIs
		// (not yet exposed in the cross-platform Datadog.Maui builder)
		EnableSessionReplay(DatadogConfig.SessionReplaySampleRate);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	/// <summary>
	/// Copy appsettings.json from bundled MauiAsset to the app data directory,
	/// then load it via AddJsonFile so it's available through IConfiguration.
	/// </summary>
	private static void LoadAppSettings(ConfigurationManager configuration)
	{
		try
		{
			string configFilePath;

#if ANDROID
			var filesDir = Android.App.Application.Context.FilesDir!.AbsolutePath;
			configFilePath = Path.Combine(filesDir, "appsettings.json");

			using (var assetStream = Android.App.Application.Context.Assets!.Open("appsettings.json"))
			using (var fileStream = File.Create(configFilePath))
			{
				assetStream.CopyTo(fileStream);
			}
#elif IOS
			configFilePath = Foundation.NSBundle.MainBundle.PathForResource("appsettings", "json")!;
#else
			configFilePath = "appsettings.json";
#endif

			configuration.AddJsonFile(configFilePath, optional: false, reloadOnChange: false);
			Console.WriteLine("[Datadog] Loaded appsettings.json into Configuration");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[Datadog] ERROR loading appsettings.json: {ex.GetType().Name}: {ex.Message}");
		}
	}

	/// <summary>
	/// Enable Session Replay using platform-native APIs.
	/// The cross-platform Datadog.Maui builder does not yet expose EnableSessionReplay(),
	/// so we call the native bindings directly.
	/// </summary>
	private static void EnableSessionReplay(float sampleRate)
	{
		try
		{
#if ANDROID
			var srConfig = new Datadog.Android.SessionReplay.SessionReplayConfiguration.Builder(sampleRate)
				.SetTextAndInputPrivacy(Datadog.Android.SessionReplay.TextAndInputPrivacy.MaskSensitiveInputs!)
				.SetImagePrivacy(Datadog.Android.SessionReplay.ImagePrivacy.MaskLargeOnly!)
				.SetTouchPrivacy(Datadog.Android.SessionReplay.TouchPrivacy.Show!)
				.StartRecordingImmediately(true)
				.Build();
			Datadog.Android.SessionReplay.SessionReplay.Enable(srConfig);
#elif IOS
			var srConfig = new Datadog.iOS.SessionReplay.DDSessionReplayConfiguration(
				sampleRate,
				Datadog.iOS.SessionReplay.DDTextAndInputPrivacyLevel.SensitiveInputs,
				Datadog.iOS.SessionReplay.DDImagePrivacyLevel.NonBundledOnly,
				Datadog.iOS.SessionReplay.DDTouchPrivacyLevel.Show);
			srConfig.StartRecordingImmediately = true;
			Datadog.iOS.SessionReplay.DDSessionReplay.EnableWith(srConfig);
#endif
			Console.WriteLine($"[Datadog] Session Replay enabled (sample rate: {sampleRate}%)");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[Datadog] ERROR enabling Session Replay: {ex.GetType().Name}: {ex.Message}");
		}
	}

	private static DatadogSite GetDatadogSite(string value)
	{
		if (Enum.TryParse<DatadogSite>(value, ignoreCase: true, out var site))
		{
			return site;
		}

		throw new InvalidOperationException($"Invalid Datadog site '{value}'. Use one of: {string.Join(", ", Enum.GetNames<DatadogSite>())}.");
	}
}
