using Foundation;
using UIKit;

namespace DatadogMauiSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        // Configure tab bar appearance before MAUI initializes
        ConfigureTabBarAppearance();

        var result = base.FinishedLaunching(application, launchOptions);

        // Remove liquid glass effect after MAUI creates the tab bar
        RemoveLiquidGlassEffect();

        return result;
    }

    private void RemoveLiquidGlassEffect()
    {
        // Find and remove the _UIBarBackground view that creates the liquid glass effect
        if (UIApplication.SharedApplication.KeyWindow?.RootViewController is UIViewController rootVC)
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

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
