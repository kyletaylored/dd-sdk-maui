using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DatadogMauiSample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    // Note: Datadog initialization is handled by the MAUI plugin in MauiProgram.cs
    // No need to initialize here - the plugin handles both Android and iOS
}
