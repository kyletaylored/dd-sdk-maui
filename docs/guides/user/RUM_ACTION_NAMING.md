---
layout: default
title: RUM Action Naming
parent: User Guides
nav_order: 1
permalink: /guides/user/rum-actions
---

# Improving RUM Action Names in .NET MAUI

By default, Datadog RUM tracks user interactions in MAUI apps, but the action names can be cryptic and hard to understand. For example:

```
tap on BottomNavigationItemView(0x2) on crc646e722b0a9beeeae7.MainActivity
```

This document explains how to improve action naming for better observability.

## The Problem

MAUI uses platform-specific renderers that generate technical class names. Without proper tagging, RUM actions show:
- **Android**: Obfuscated class names like `crc646e722b0a9beeeae7.MainActivity`, `BottomNavigationItemView(0x2)`
- **iOS**: Similar technical identifiers

These names make it difficult to:
- Understand user behavior in RUM dashboards
- Create meaningful funnels and analytics
- Debug user experience issues

## Solution 1: AutomationId Property

The `AutomationId` property in MAUI provides a way to tag UI elements with human-readable identifiers that Datadog RUM can capture.

### How to Use AutomationId

Add the `AutomationId` property to any interactive MAUI element:

```xml
<Button
    x:Name="CheckoutButton"
    AutomationId="checkout_button"
    Text="Checkout"
    Clicked="OnCheckoutClicked" />
```

### Naming Conventions

Use snake_case naming that describes the action:

```xml
<!-- Good Examples -->
<Button AutomationId="add_to_cart_button" ... />
<Button AutomationId="refresh_products_button" ... />
<Button AutomationId="clear_cart_button" ... />
<Button AutomationId="increase_quantity_button" ... />

<!-- Navigation Tabs -->
<ShellContent AutomationId="tab_products" ... />
<ShellContent AutomationId="tab_cart" ... />
<ShellContent AutomationId="tab_profile" ... />

<!-- Lists and Collections -->
<CollectionView AutomationId="products_list" ... />
<CollectionView AutomationId="cart_items_list" ... />
```

### Benefits

1. **Readable RUM Actions**: Instead of `tap on BottomNavigationItemView(0x2)`, you get `tap on checkout_button`
2. **Better Analytics**: Easier to create funnels and analyze user behavior
3. **Consistent Naming**: Same identifier works on both Android and iOS
4. **UI Testing**: AutomationId also improves automated testing

## Solution 2: Manual RUM Action Tracking

For more control, manually track actions with custom names:

```csharp
using Datadog.Maui.Rum;

private async void OnCheckoutClicked(object? sender, EventArgs e)
{
    // Track with custom name and attributes
    Rum.AddAction(RumActionType.Tap, "checkout_button", new Dictionary<string, object>
    {
        { "item_count", cartService.ItemCount },
        { "total_amount", cartService.TotalAmount }
    });

    // Your logic here
    await ProcessCheckout();
}
```

### When to Use Manual Tracking

- Complex user flows that need context
- Adding custom attributes to actions
- Tracking non-UI interactions
- Custom events that don't correspond to UI elements

## Solution 3: Hybrid Approach (Recommended)

Combine both approaches for best results:

```xml
<!-- Use AutomationId for basic identification -->
<Button
    x:Name="CheckoutButton"
    AutomationId="checkout_button"
    Text="Proceed to Checkout"
    Clicked="OnCheckoutClicked" />
```

```csharp
private async void OnCheckoutClicked(object? sender, EventArgs e)
{
    // Add manual tracking with rich context
    Rum.AddAction(RumActionType.Tap, "checkout_initiated", new Dictionary<string, object>
    {
        { "item_count", _cartService.ItemCount },
        { "total_amount", _cartService.TotalAmount },
        { "payment_method", selectedPaymentMethod },
        { "has_coupon", !string.IsNullOrEmpty(couponCode) }
    });

    await ProcessCheckout();
}
```

This gives you:
- Automatic tracking with readable names (via AutomationId)
- Rich context for important actions (via manual tracking)
- Comprehensive view of user behavior

## Best Practices

### 1. Name Elements Consistently

```xml
<!-- Pattern: {action}_{element_type} -->
<Button AutomationId="add_to_cart_button" />
<Button AutomationId="remove_item_button" />
<Button AutomationId="refresh_products_button" />

<!-- Pattern: {section}_{element_type} -->
<CollectionView AutomationId="products_list" />
<CollectionView AutomationId="cart_items_list" />

<!-- Pattern: tab_{tab_name} -->
<ShellContent AutomationId="tab_products" />
<ShellContent AutomationId="tab_cart" />
```

### 2. Add Context with Manual Tracking

```csharp
// Track with business context
Rum.AddAction(RumActionType.Tap, "add_to_cart", new Dictionary<string, object>
{
    { "product_id", product.Id },
    { "product_name", product.Name },
    { "product_price", product.Price },
    { "cart_item_count", _cartService.ItemCount },
    { "is_on_sale", product.IsOnSale }
});
```

### 3. Track Key User Flows

Focus on business-critical actions:

```csharp
// Cart operations
Rum.AddAction(RumActionType.Custom, "add_to_cart", attributes);
Rum.AddAction(RumActionType.Custom, "remove_from_cart", attributes);
Rum.AddAction(RumActionType.Custom, "update_quantity", attributes);

// Checkout flow
Rum.AddAction(RumActionType.Custom, "checkout_initiated", attributes);
Rum.AddAction(RumActionType.Custom, "payment_submitted", attributes);
Rum.AddAction(RumActionType.Custom, "purchase_completed", attributes);

// Search and discovery
Rum.AddAction(RumActionType.Custom, "product_searched", attributes);
Rum.AddAction(RumActionType.Custom, "product_filtered", attributes);
Rum.AddAction(RumActionType.Custom, "product_viewed", attributes);
```

### 4. Use Meaningful Attributes

Add attributes that help with analysis:

```csharp
new Dictionary<string, object>
{
    // Business metrics
    { "item_count", 3 },
    { "total_amount", 149.97 },
    { "discount_applied", true },

    // User context
    { "is_authenticated", true },
    { "user_tier", "premium" },

    // Technical context
    { "network_type", "wifi" },
    { "app_version", "1.2.3" }
}
```

## Example: Complete Cart Implementation

See the sample app for a complete implementation:

**XAML ([CartPage.xaml](../../../samples/DatadogMauiSample/CartPage.xaml))**:
```xml
<Button
    AutomationId="checkout_button"
    Text="Proceed to Checkout"
    Clicked="OnCheckoutClicked" />

<Button
    AutomationId="clear_cart_button"
    Text="Clear Cart"
    Clicked="OnClearCartClicked" />

<Button
    AutomationId="increase_quantity_button"
    Clicked="OnIncreaseQuantityClicked" />
```

**Code ([CartPage.xaml.cs](../../../samples/DatadogMauiSample/CartPage.xaml.cs))**:
```csharp
private async void OnCheckoutClicked(object? sender, EventArgs e)
{
    Rum.AddAction(RumActionType.Tap, "checkout_button", new Dictionary<string, object>
    {
        { "item_count", _cartService.ItemCount },
        { "total_amount", _cartService.TotalAmount }
    });

    await ProcessCheckout();
}
```

## Verifying in Datadog

After implementation:

1. **Navigate to RUM > Actions** in your Datadog dashboard
2. Look for readable action names like:
   - `tap on checkout_button`
   - `tap on add_to_cart_button`
   - `tap on tab_cart`
3. Click on an action to see custom attributes
4. Create funnels using these named actions

## Additional Resources

- [Datadog RUM Actions Documentation](https://docs.datadoghq.com/real_user_monitoring/browser/tracking_user_actions/)
- [MAUI AutomationProperties](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/accessibility)
- [Sample App Implementation](../../../samples/DatadogMauiSample/)

---

**Related Guides:**
- [Getting Started](../../getting-started/installation)
- [Mapping File Uploads](mapping-files)
