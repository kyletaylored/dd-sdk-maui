using Datadog.iOS.RUM;
using Foundation;
using UIKit;

namespace Datadog.Maui.Platforms.iOS;

/// <summary>
/// Custom RUM views predicate that filters out MAUI's internal view controllers
/// and only tracks meaningful user-facing views.
/// </summary>
public class MauiRumViewsPredicate : DDUIKitRUMViewsPredicate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MauiRumViewsPredicate"/> class.
    /// </summary>
    public MauiRumViewsPredicate()
    {
    }

    public override DDRUMView? RumViewFor(UIViewController viewController)
    {
        var viewControllerType = viewController.GetType().FullName ?? viewController.GetType().Name;

        // Filter out MAUI's internal view controllers
        if (IsMauiInternalViewController(viewControllerType))
        {
            return null; // Don't track this view controller
        }

        // Extract a meaningful view name from the view controller type
        var viewName = ExtractViewName(viewControllerType);

        // Create RUM view with the extracted name
        var attributes = new NSDictionary<NSString, NSObject>();
        return new DDRUMView(viewName, attributes);
    }

    private bool IsMauiInternalViewController(string typeName)
    {
        // Filter very short/generic names first
        if (typeName.Length <= 3 || typeName == "STWeb")
        {
            return true;
        }

        // List of MAUI internal view controller patterns to ignore
        var internalPatterns = new[]
        {
            // MAUI Shell internals
            "Shell",

            // MAUI platform internals
            "Microsoft.Maui.Controls.Platform",
            "Microsoft.Maui.Platform.PageViewController",
            "Microsoft.Maui.Platform.ContentViewController",
            "Microsoft.Maui.Controls.Handlers.Compatibility",

            // UIKit system controllers
            "UIKit.UINavigationController",
            "UIKit.UITabBarController",
            "UIKit.UIAlert",
            "UIKit.UIInputWindow",
            "UIKit.UIWindow",

            // Generic/system controllers
            "HostingController",
        };

        foreach (var pattern in internalPatterns)
        {
            if (typeName.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private string ExtractViewName(string typeName)
    {
        // Try to extract a meaningful name from the type
        // For example: "DatadogMauiSample.Views.CartPage" -> "CartPage"

        var parts = typeName.Split('.');
        var lastPart = parts[^1];

        // Remove common suffixes
        lastPart = lastPart
            .Replace("Page", "", StringComparison.OrdinalIgnoreCase)
            .Replace("View", "", StringComparison.OrdinalIgnoreCase)
            .Replace("Controller", "", StringComparison.OrdinalIgnoreCase);

        // If empty after removing suffixes, use the original last part
        if (string.IsNullOrWhiteSpace(lastPart))
        {
            lastPart = parts[^1];
        }

        // Convert PascalCase to "Pascal Case" for better readability
        return AddSpacesToPascalCase(lastPart);
    }

    private string AddSpacesToPascalCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var result = new System.Text.StringBuilder();
        result.Append(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && !char.IsUpper(text[i - 1]))
            {
                result.Append(' ');
            }
            result.Append(text[i]);
        }

        return result.ToString();
    }
}
