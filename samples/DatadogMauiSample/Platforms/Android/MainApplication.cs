using Android.App;
using Android.Runtime;

namespace DatadogMauiSample;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Datadog initialization is now handled by the unified MAUI plugin via MauiProgram.cs
        // No need for platform-specific initialization code here
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
