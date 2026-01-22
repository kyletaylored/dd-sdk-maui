using DatadogMauiSample.Services;
using Datadog.Maui.Rum;
using Datadog.Maui.Logs;

namespace DatadogMauiSample;

public partial class CartPage : ContentPage
{
    private readonly CartService _cartService;
    private readonly ILogger _logger;

    public CartPage()
    {
        InitializeComponent();

        _cartService = CartService.Instance;
        _logger = Logs.CreateLogger("CartPage");

        // Bind to cart service
        CartCollectionView.ItemsSource = _cartService.Items;

        // Subscribe to cart updates
        _cartService.CartUpdated += OnCartUpdated;

        // Start RUM view tracking
        Rum.StartView("cart", "Shopping Cart", new Dictionary<string, object>
        {
            { "screen_class", "CartPage" },
            { "initial_item_count", _cartService.ItemCount }
        });

        _logger.Info("CartPage initialized", null, new Dictionary<string, object>
        {
            { "item_count", _cartService.ItemCount },
            { "total_amount", _cartService.TotalAmount }
        });

        UpdateCartSummary();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop RUM view tracking
        Rum.StopView("cart", new Dictionary<string, object>
        {
            { "final_item_count", _cartService.ItemCount },
            { "final_total_amount", _cartService.TotalAmount }
        });

        _logger.Debug("CartPage disappeared");
    }

    private void OnCartUpdated(object? sender, EventArgs e)
    {
        UpdateCartSummary();

        // Track cart update in RUM
        Rum.AddAction(RumActionType.Custom, "cart_updated", new Dictionary<string, object>
        {
            { "item_count", _cartService.ItemCount },
            { "total_amount", _cartService.TotalAmount }
        });
    }

    private void UpdateCartSummary()
    {
        ItemCountLabel.Text = _cartService.ItemCount.ToString();
        TotalAmountLabel.Text = $"${_cartService.TotalAmount:F2}";
        CheckoutButton.IsEnabled = _cartService.ItemCount > 0;
    }

    private void OnIncreaseQuantityClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string productId)
        {
            _cartService.UpdateQuantity(productId,
                _cartService.Items.First(i => i.Product.Id == productId).Quantity + 1);

            // Track RUM action with readable name (thanks to AutomationId)
            Rum.AddAction(RumActionType.Tap, "increase_quantity", new Dictionary<string, object>
            {
                { "product_id", productId },
                { "new_quantity", _cartService.Items.First(i => i.Product.Id == productId).Quantity }
            });

            _logger.Debug($"Increased quantity for product {productId}");
        }
    }

    private void OnDecreaseQuantityClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string productId)
        {
            var item = _cartService.Items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                var newQuantity = item.Quantity - 1;
                _cartService.UpdateQuantity(productId, newQuantity);

                // Track RUM action
                Rum.AddAction(RumActionType.Tap, "decrease_quantity", new Dictionary<string, object>
                {
                    { "product_id", productId },
                    { "new_quantity", newQuantity }
                });

                _logger.Debug($"Decreased quantity for product {productId}");
            }
        }
    }

    private async void OnRemoveItemClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string productId)
        {
            var item = _cartService.Items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                var confirmed = await DisplayAlert(
                    "Remove Item",
                    $"Remove {item.Product.Name} from cart?",
                    "Yes",
                    "No"
                );

                if (confirmed)
                {
                    _cartService.RemoveItem(productId);

                    // Track RUM action
                    Rum.AddAction(RumActionType.Tap, "remove_item", new Dictionary<string, object>
                    {
                        { "product_id", productId },
                        { "product_name", item.Product.Name }
                    });

                    _logger.Info($"Removed {item.Product.Name} from cart", null, new Dictionary<string, object>
                    {
                        { "product_id", productId }
                    });
                }
            }
        }
    }

    private async void OnClearCartClicked(object? sender, EventArgs e)
    {
        if (_cartService.ItemCount == 0)
        {
            await DisplayAlert("Empty Cart", "Your cart is already empty", "OK");
            return;
        }

        var confirmed = await DisplayAlert(
            "Clear Cart",
            $"Remove all {_cartService.ItemCount} items from cart?",
            "Yes",
            "No"
        );

        if (confirmed)
        {
            var itemCount = _cartService.ItemCount;
            _cartService.Clear();

            // Track RUM action
            Rum.AddAction(RumActionType.Tap, "clear_cart", new Dictionary<string, object>
            {
                { "items_removed", itemCount }
            });

            _logger.Info("Cart cleared", null, new Dictionary<string, object>
            {
                { "items_removed", itemCount }
            });

            await DisplayAlert("Success", "Cart cleared successfully", "OK");
        }
    }

    private async void OnCheckoutClicked(object? sender, EventArgs e)
    {
        if (_cartService.ItemCount == 0)
        {
            await DisplayAlert("Empty Cart", "Please add items to your cart first", "OK");
            return;
        }

        // Track RUM action
        Rum.AddAction(RumActionType.Tap, "checkout_button", new Dictionary<string, object>
        {
            { "item_count", _cartService.ItemCount },
            { "total_amount", _cartService.TotalAmount }
        });

        _logger.Info("Checkout initiated", null, new Dictionary<string, object>
        {
            { "item_count", _cartService.ItemCount },
            { "total_amount", _cartService.TotalAmount }
        });

        // Simulate checkout
        await DisplayAlert(
            "Checkout",
            $"Processing checkout for {_cartService.ItemCount} items\n" +
            $"Total: ${_cartService.TotalAmount:F2}\n\n" +
            $"This is a demo - no actual purchase will be made.",
            "OK"
        );

        // In a real app, you would navigate to a checkout flow here
        // For demo purposes, we'll just clear the cart
        var success = await DisplayAlert(
            "Complete Purchase?",
            "Simulate successful purchase and clear cart?",
            "Yes",
            "No"
        );

        if (success)
        {
            var itemCount = _cartService.ItemCount;
            var totalAmount = _cartService.TotalAmount;

            _cartService.Clear();

            // Track successful purchase
            Rum.AddAction(RumActionType.Custom, "purchase_completed", new Dictionary<string, object>
            {
                { "item_count", itemCount },
                { "total_amount", totalAmount }
            });

            _logger.Info("Purchase completed", null, new Dictionary<string, object>
            {
                { "item_count", itemCount },
                { "total_amount", totalAmount }
            });

            await DisplayAlert("Success", "Purchase completed successfully!", "OK");
        }
    }
}
