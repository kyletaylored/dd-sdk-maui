using Foundation;
using UIKit;

namespace DatadogMauiSample;

/// <summary>
/// iOS application delegate for DatadogMauiSample.
/// </summary>
/// <remarks>
/// Datadog initialization is now handled in MauiProgram.cs using the unified builder pattern API.
/// This keeps the platform-specific code minimal and demonstrates the recommended integration approach.
/// </remarks>
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <summary>
    /// Called when the application finishes launching.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <param name="launchOptions">The launch options.</param>
    /// <returns>True if launch was successful.</returns>
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        // Configure tab bar appearance before MAUI initializes
        ConfigureTabBarAppearance();

        var result = base.FinishedLaunching(application, launchOptions ?? new NSDictionary());

        // Remove liquid glass effect after MAUI creates the tab bar
        RemoveLiquidGlassEffect();

        return result;
    }

    private void RemoveLiquidGlassEffect()
    {
        // Find and remove the _UIBarBackground view that creates the liquid glass effect
        UIViewController? rootVC = null;

        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        {
            // iOS 13+ - use window scenes
            var windowScene = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);

            rootVC = windowScene?.Windows.FirstOrDefault()?.RootViewController;
        }
        else
        {
            // iOS 12 and earlier - use deprecated KeyWindow
            #pragma warning disable CA1422
            rootVC = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            #pragma warning restore CA1422
        }

        if (rootVC != null)
        {
            RemoveBackgroundFromTabBar(rootVC);
        }
    }

    private void RemoveBackgroundFromTabBar(UIViewController viewController)
    {
        // Check if it's a UITabBarController or contains one
        if (viewController is UITabBarController tabBarController)
        {
            RemoveBarBackground(tabBarController.TabBar);
        }
        else if (viewController.PresentedViewController != null)
        {
            RemoveBackgroundFromTabBar(viewController.PresentedViewController);
        }

        // Check child view controllers
        foreach (var child in viewController.ChildViewControllers)
        {
            RemoveBackgroundFromTabBar(child);
        }
    }

    private void RemoveBarBackground(UITabBar tabBar)
    {
        foreach (var subview in tabBar.Subviews)
        {
            var typeName = subview.GetType().Name;
            // Remove _UIBarBackground which creates the liquid glass effect
            if (typeName == "_UIBarBackground")
            {
                subview.RemoveFromSuperview();
            }
        }
    }

    private void ConfigureTabBarAppearance()
    {
        // Configure tab bar for iOS 15+
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            var appearance = new UITabBarAppearance();
            appearance.ConfigureWithOpaqueBackground();

            // Set solid background color
            appearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);

            // Disable shadow/separator
            appearance.ShadowColor = UIColor.Clear;

            // Configure tab bar items using the inline item appearance
            var itemAppearance = appearance.InlineLayoutAppearance;

            // Normal state (unselected)
            itemAppearance.Normal.IconColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f);
            itemAppearance.Normal.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f)
            };

            // Selected state
            itemAppearance.Selected.IconColor = UIColor.White;
            itemAppearance.Selected.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            // Apply to all layout types
            appearance.StackedLayoutAppearance = itemAppearance;
            appearance.InlineLayoutAppearance = itemAppearance;
            appearance.CompactInlineLayoutAppearance = itemAppearance;

            // Apply the appearance
            UITabBar.Appearance.StandardAppearance = appearance;
            UITabBar.Appearance.ScrollEdgeAppearance = appearance;
        }
        else
        {
            // Fallback for iOS 14 and earlier
            UITabBar.Appearance.BackgroundColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);
            UITabBar.Appearance.TintColor = UIColor.White;
            UITabBar.Appearance.UnselectedItemTintColor = UIColor.FromRGBA(0xFF, 0xFF, 0xFF, 0.7f);
            UITabBar.Appearance.BarTintColor = UIColor.FromRGB(0x51, 0x2B, 0xD4);
        }
    }

    /// <summary>
    /// Creates the MAUI application.
    /// </summary>
    /// <returns>The MAUI application instance.</returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
