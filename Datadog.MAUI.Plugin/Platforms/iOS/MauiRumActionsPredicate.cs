using Datadog.iOS.RUM;
using Foundation;
using UIKit;

namespace Datadog.Maui.Platforms.iOS;

/// <summary>
/// Custom RUM actions predicate that enables automatic UIKit action tracking.
/// This is a workaround for the binding issue where DDDefaultUIKitRUMActionsPredicate
/// doesn't properly inherit from DDUIKitRUMActionsPredicate.
/// </summary>
/// <remarks>
/// This class implements the required RumActionWithTargetView method to provide
/// default action tracking behavior similar to DDDefaultUIKitRUMActionsPredicate.
/// It tracks taps on interactive UI elements like buttons, controls, and views with
/// accessibility labels.
/// </remarks>
public class MauiRumActionsPredicate : DDUIKitRUMActionsPredicate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MauiRumActionsPredicate"/> class.
    /// </summary>
    public MauiRumActionsPredicate()
    {
    }

    /// <summary>
    /// Determines whether a tap on the given view should be tracked as a RUM action.
    /// </summary>
    /// <param name="targetView">The view that was tapped.</param>
    /// <returns>A DDRUMAction if the tap should be tracked, null otherwise.</returns>
    [Export("rumActionWithTargetView:")]
    public DDRUMAction? RumActionWithTargetView(UIView targetView)
    {
        // Track taps on interactive elements
        if (!ShouldTrackView(targetView))
        {
            return null;
        }

        // Generate action name from the view
        var actionName = GetActionName(targetView);

        // Create RUM action with the extracted name
        var attributes = new NSDictionary<NSString, NSObject>();
        return new DDRUMAction(actionName, attributes);
    }

    /// <summary>
    /// Determines if a view should be tracked for user interactions.
    /// </summary>
    private bool ShouldTrackView(UIView view)
    {
        // Track UIControls (buttons, switches, sliders, etc.)
        if (view is UIControl)
        {
            return true;
        }

        // Track views with accessibility labels (indicates they're interactive)
        if (!string.IsNullOrEmpty(view.AccessibilityLabel))
        {
            return true;
        }

        // Track views with accessibility identifiers
        if (!string.IsNullOrEmpty(view.AccessibilityIdentifier))
        {
            return true;
        }

        // Track table/collection view cells
        if (view is UITableViewCell || view is UICollectionViewCell)
        {
            return true;
        }

        // Don't track non-interactive system views
        return false;
    }

    /// <summary>
    /// Extracts a meaningful action name from the view.
    /// </summary>
    private string GetActionName(UIView view)
    {
        // Try accessibility label first (most descriptive)
        if (!string.IsNullOrEmpty(view.AccessibilityLabel))
        {
            return view.AccessibilityLabel;
        }

        // Try accessibility identifier
        if (!string.IsNullOrEmpty(view.AccessibilityIdentifier))
        {
            return view.AccessibilityIdentifier;
        }

        // For buttons, try to get the title
        if (view is UIButton button && !string.IsNullOrEmpty(button.CurrentTitle))
        {
            return button.CurrentTitle;
        }

        // Fall back to view type name
        var typeName = view.GetType().Name;

        // Remove "UI" prefix if present
        if (typeName.StartsWith("UI"))
        {
            typeName = typeName.Substring(2);
        }

        return typeName;
    }
}
