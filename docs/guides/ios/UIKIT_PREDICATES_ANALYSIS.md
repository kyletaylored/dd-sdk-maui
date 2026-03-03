# iOS UIKit Predicates Analysis

## Overview

The Datadog iOS SDK v2 supports automatic view and action tracking through UIKit predicates. This document describes how the Datadog MAUI SDK bridges these predicates for .NET MAUI applications.

## Native iOS SDK Predicates

The native iOS SDK exposes the following predicates on `DDRUMConfiguration`:

```objc
DDRUMConfiguration *configuration = [[DDRUMConfiguration alloc] initWithApplicationID:@"<rum application id>"];
configuration.uiKitViewsPredicate = [DDDefaultUIKitRUMViewsPredicate new];
configuration.uiKitActionsPredicate = [DDDefaultUIKitRUMActionsPredicate new];
```

---

## Current MAUI SDK Implementation Status

### ✅ Implemented

1. **UIKit View Predicate** — `MauiRumViewsPredicate`
   - File: [Datadog.MAUI.Plugin/Platforms/iOS/MauiRumViewsPredicate.cs](/Datadog.MAUI.Plugin/Platforms/iOS/MauiRumViewsPredicate.cs)
   - Filters out MAUI-internal and UIKit system controllers, exposing only meaningful user-navigated views.
   - Assigned automatically when `TrackViewsAutomatically = true`.

2. **UIKit Action Predicate** — `MauiRumActionsPredicate`
   - File: [Datadog.MAUI.Plugin/Platforms/iOS/MauiRumActionsPredicate.cs](/Datadog.MAUI.Plugin/Platforms/iOS/MauiRumActionsPredicate.cs)
   - Tracks taps on `UIControl`, views with accessibility labels/identifiers, and table/collection view cells.
   - Assigned automatically when `TrackUserInteractions = true`.

3. **URLSession Tracking** — [Datadog.ios.cs](/Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs)
   - First-party host tracing configured on `DDTraceConfiguration` when first-party hosts are provided.

### ❌ Not Implemented

1. **SwiftUI Predicates** — Not applicable to .NET MAUI (MAUI does not use SwiftUI).

---

## Why Custom Predicates Instead of Native Defaults

The native `DDDefaultUIKitRUMViewsPredicate` and `DDDefaultUIKitRUMActionsPredicate` cannot be used directly in Xamarin.iOS/MAUI bindings due to a binding protocol inheritance issue:

- `DDDefaultUIKitRUMActionsPredicate` implements `DDUIKitRUMActionsPredicate` in Objective-C, but the generated binding does not reflect this inheritance chain
- Attempting to cast or wrap `DDDefaultUIKitRUMActionsPredicate` as `DDUIKitRUMActionsPredicate` causes an `InvalidCastException` at runtime
- The same issue exists for the view predicate side

**Solution:** Custom `MauiRumActionsPredicate` and `MauiRumViewsPredicate` classes that properly inherit from the binding's protocol base classes and export the required Objective-C selectors.

---

## Custom Predicate Behavior

### MauiRumViewsPredicate

Extends `DDUIKitRUMViewsPredicate`, implementing `rumViewFor(UIViewController)`.

**Filters out (returns `null`):**
- Short/generic type names (≤3 chars, e.g., `"UI"`)
- All Shell-related controllers (`ShellFlyoutContentRenderer`, `ShellRenderer`, etc.)
- MAUI platform internals (`Microsoft.Maui.Controls.Platform.*`)
- UIKit system dialogs (`UIAlertController`, `UIWindowController`)
- Hosting controllers (`HostingController`, `UIHostingController`)
- Generic platform roots (`RootViewController`, `PageViewController`)

**Passes through:**
- User-defined `ContentPage` subclasses (e.g., `HomePage`, `CheckoutPage`)
- Any controller not matching the above patterns

### MauiRumActionsPredicate

Implements `rumActionWithTargetView(UIView)`, exported via `[Export("rumActionWithTargetView:")]`.

**Tracks taps on:**
- `UIControl` subclasses — buttons, switches, sliders, steppers, text fields
- Views with an `AccessibilityLabel` set
- Views with an `AccessibilityIdentifier` set
- `UITableViewCell` and `UICollectionViewCell`

**Does NOT track:**
- Plain `UIView` instances with no accessibility info
- Background/container views
- System decorators

**Action naming priority:**
1. `AccessibilityLabel` (most semantic)
2. `AccessibilityIdentifier`
3. `UIButton.CurrentTitle`
4. View type name (stripped of "UI" prefix)

---

## iOS Binding Workaround Pattern

This is the standard pattern for implementing Xamarin.iOS protocol subclasses where the binding has protocol inheritance issues:

```csharp
// ❌ Does NOT work — binding inheritance issue
rumConfiguration.UiKitActionsPredicate = new DDDefaultUIKitRUMActionsPredicate();

// ❌ Does NOT work — InvalidCastException at runtime
var wrapped = Runtime.GetINativeObject<DDUIKitRUMActionsPredicate>(
    new DDDefaultUIKitRUMActionsPredicate().Handle, false);

// ✅ Works — custom class inheriting from the protocol base class
public class MauiRumActionsPredicate : DDUIKitRUMActionsPredicate
{
    [Export("rumActionWithTargetView:")]
    public DDRUMAction? RumActionWithTargetView(UIView targetView)
    {
        // custom implementation
    }
}
rumConfiguration.UiKitActionsPredicate = new MauiRumActionsPredicate();
```

---

## Applicability to .NET MAUI

### View Tracking

MAUI `ContentPage` subclasses do surface as named `UIViewController` subclasses in the iOS view hierarchy, so the predicate CAN detect user-defined pages. The key challenge is filtering out the MAUI framework's own generated controllers (Shell, handlers, etc.), which `MauiRumViewsPredicate` handles.

### Action Tracking

MAUI renders `Button` as a `UIButton` and other interactive controls as UIKit equivalents. If those controls have accessibility labels configured, they will be named correctly in RUM. For MAUI `Button` controls without explicit accessibility labels, the fallback is the view type name.

**Best practice:** Set `AutomationId` or `SemanticProperties.Description` on interactive MAUI elements — these propagate to `AccessibilityIdentifier` and `AccessibilityLabel` on iOS, giving RUM actions meaningful names.

```xaml
<!-- XAML: Sets AccessibilityIdentifier → meaningful RUM action name -->
<Button Text="Add to Cart"
        AutomationId="add_to_cart_button"
        Clicked="OnAddToCartClicked" />
```

```csharp
// C#: Sets AccessibilityLabel → meaningful RUM action name
SemanticProperties.SetDescription(myButton, "Add to Cart");
```

### SwiftUI Predicates

**Not applicable.** MAUI does not use SwiftUI views; no MAUI-level bridge is possible.

---

## Manual Tracking (Always Works)

Regardless of automatic predicate tracking, the manual RUM API works reliably for all platforms:

```csharp
// Manual view tracking
protected override void OnAppearing()
{
    base.OnAppearing();
    Rum.StartView("profile_page", "Profile");
}

protected override void OnDisappearing()
{
    base.OnDisappearing();
    Rum.StopView("profile_page");
}

// Manual action tracking
private void OnSignInClicked(object sender, EventArgs e)
{
    Rum.AddAction(RumActionType.Tap, "sign_in_button");
}
```

---

## References

- [Datadog iOS RUM Documentation](https://docs.datadoghq.com/real_user_monitoring/ios/)
- [MAUI Handlers Documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/)
- [MAUI Accessibility](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/accessibility)
- [MAUI Navigation](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation)
