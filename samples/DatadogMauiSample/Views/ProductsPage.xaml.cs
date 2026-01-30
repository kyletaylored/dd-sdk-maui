using DatadogMauiSample.Models;
using DatadogMauiSample.Services;
using Datadog.Maui.Rum;
using Datadog.Maui.Logs;

namespace DatadogMauiSample.Views;

/// <summary>
/// Page for browsing and purchasing products.
/// </summary>
public partial class ProductsPage : ContentPage
{
    private readonly ShopistApiService _apiService;
    private readonly CartService _cartService;
    private readonly ILogger _logger;
    private List<Product> _products = new();
    private List<string> _categories = new();
    private string? _selectedCategory = null;
    private int? _currentLimit = 20;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsPage"/> class.
    /// </summary>
    public ProductsPage()
    {
        InitializeComponent();
        _apiService = new ShopistApiService();
        _cartService = CartService.Instance;
        _logger = Logs.CreateLogger("ProductsPage");

        // Add converters for data binding
        Resources.Add("StockStatusConverter", new StockStatusConverter());
        Resources.Add("StockColorConverter", new StockColorConverter());

        // Start RUM view tracking
        Rum.StartView("products", "Products Page", new Dictionary<string, object>
        {
            { "screen_class", "ProductsPage" }
        });

        _logger.Info("ProductsPage initialized");

        // Set default limit
        LimitPicker.SelectedIndex = 2; // 20

        LoadCategoriesAsync();
        LoadProductsAsync();
    }

    private async void LoadCategoriesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[ProductsPage] Loading categories");
            _categories = await _apiService.GetCategoriesAsync();

            // Add "All Categories" at the beginning
            var categoryList = new List<string> { "All Categories" };
            categoryList.AddRange(_categories);

            CategoryPicker.ItemsSource = categoryList;
            CategoryPicker.SelectedIndex = 0;

            System.Diagnostics.Debug.WriteLine($"[ProductsPage] Loaded {_categories.Count} categories");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProductsPage] Error loading categories: {ex.Message}");
        }
    }

    private async void OnCategoryChanged(object? sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex == 0)
        {
            _selectedCategory = null; // All categories
        }
        else if (CategoryPicker.SelectedIndex > 0)
        {
            _selectedCategory = _categories[CategoryPicker.SelectedIndex - 1];
        }

        await LoadProductsAsync(showSuccessAlert: false);
    }

    private async void OnLimitChanged(object? sender, EventArgs e)
    {
        if (LimitPicker.SelectedIndex < 0) return;

        var selectedValue = LimitPicker.Items[LimitPicker.SelectedIndex];
        _currentLimit = selectedValue == "All" ? null : int.Parse(selectedValue);

        await LoadProductsAsync(showSuccessAlert: false);
    }

    /// <summary>
    /// Called when the page is disappearing.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop RUM view tracking when page disappears
        Rum.StopView("products");

        _logger.Debug("ProductsPage disappeared");
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadProductsAsync(showSuccessAlert: true);
    }

    private async Task LoadProductsAsync(bool showSuccessAlert = false)
    {
        // Track the action in RUM
        Rum.AddAction(RumActionType.Custom, "load_products", new Dictionary<string, object>
        {
            { "trigger", "user_action" }
        });

        // Add custom timing to RUM view
        var startTime = DateTime.UtcNow;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            _logger.Info("Starting to load products");

            // Load products based on selected category and limit
            if (_selectedCategory != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ProductsPage] Loading products for category: {_selectedCategory}");
                _products = await _apiService.GetProductsByCategoryAsync(_selectedCategory);

                // Apply limit if set
                if (_currentLimit.HasValue && _products.Count > _currentLimit.Value)
                {
                    _products = _products.Take(_currentLimit.Value).ToList();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ProductsPage] Loading all products with limit: {_currentLimit?.ToString() ?? "none"}");
                _products = await _apiService.GetProductsAsync(limit: _currentLimit);
            }

            ProductsCollectionView.ItemsSource = _products;

            // Track successful load timing
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Rum.AddTiming($"products_load_time");

            _logger.Info($"Successfully loaded {_products.Count} products in {duration}ms", null, new Dictionary<string, object>
            {
                { "product_count", _products.Count },
                { "load_duration_ms", duration }
            });

            // Add custom attribute to RUM session
            Rum.AddAttribute("last_product_count", _products.Count);

            if (showSuccessAlert)
            {
                await DisplayAlert("Success", $"Loaded {_products.Count} products", "OK");
            }
        }
        catch (Exception ex)
        {
            // Track error in RUM
            Rum.AddError(
                $"Failed to load products: {ex.Message}",
                RumErrorSource.Source,
                ex,
                new Dictionary<string, object>
                {
                    { "operation", "load_products" },
                    { "error_type", ex.GetType().Name }
                }
            );

            _logger.Error($"Error loading products: {ex.Message}", ex, new Dictionary<string, object>
            {
                { "error_type", ex.GetType().Name },
                { "stack_trace", ex.StackTrace ?? "N/A" }
            });

            await DisplayAlert("Error", $"Failed to load products: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnProductSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product product)
        {
            // Track product selection in RUM
            Rum.AddAction(RumActionType.Tap, "product_selected", new Dictionary<string, object>
            {
                { "product_id", product.Id },
                { "product_name", product.Name },
                { "product_price", product.Price }
            });

            _logger.Debug($"Product selected: {product.Name}", null, new Dictionary<string, object>
            {
                { "product_id", product.Id },
                { "price", product.Price }
            });

            var action = await DisplayActionSheet(
                $"{product.Name}",
                "Cancel",
                null,
                "Add to Cart",
                "Buy Now"
            );

            if (action == "Add to Cart")
            {
                Rum.AddAction(RumActionType.Tap, "add_to_cart_button", new Dictionary<string, object>
                {
                    { "product_id", product.Id }
                });
                await AddToCartAsync(product);
            }
            else if (action == "Buy Now")
            {
                Rum.AddAction(RumActionType.Tap, "buy_now_button", new Dictionary<string, object>
                {
                    { "product_id", product.Id }
                });
                await PurchaseProductAsync(product);
            }

            // Deselect the item
            if (sender is CollectionView collectionView)
            {
                collectionView.SelectedItem = null;
            }
        }
    }

    private async Task AddToCartAsync(Product product)
    {
        try
        {
            // Add to local cart
            _cartService.AddItem(product);

            // Track in RUM with readable action name
            Rum.AddAction(RumActionType.Custom, "add_to_cart", new Dictionary<string, object>
            {
                { "product_id", product.Id },
                { "product_name", product.Name },
                { "product_price", product.Price },
                { "cart_item_count", _cartService.ItemCount }
            });

            _logger.Info($"Added {product.Name} to cart", null, new Dictionary<string, object>
            {
                { "product_id", product.Id },
                { "cart_item_count", _cartService.ItemCount }
            });

            // Also call the API for demo purposes
            var cartId = await _apiService.CreateCartAsync();
            if (cartId != null)
            {
                await _apiService.AddItemToCartAsync(cartId, product.Id);
            }

            await DisplayAlert("Success", $"Added {product.Name} to cart!\n\nTotal items: {_cartService.ItemCount}", "OK");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error adding to cart: {ex.Message}", ex);
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnViewCartClicked(object? sender, EventArgs e)
    {
        // Track RUM action
        Rum.AddAction(RumActionType.Tap, "view_cart_button", new Dictionary<string, object>
        {
            { "cart_item_count", _cartService.ItemCount }
        });

        _logger.Debug("Navigating to cart page");

        // Navigate to cart page
        await Shell.Current.GoToAsync("//CartPage");
    }

    private async Task PurchaseProductAsync(Product product)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var success = await _apiService.SimulateFullPurchaseFlowAsync(product.Id);

            if (success)
            {
                await DisplayAlert("Success", $"Successfully purchased {product.Name}!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Purchase failed. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnTestPurchaseClicked(object? sender, EventArgs e)
    {
        if (_products.Count == 0)
        {
            await DisplayAlert("No Products", "Please load products first", "OK");
            return;
        }

        var firstProduct = _products.First();
        await PurchaseProductAsync(firstProduct);
    }
}

/// <summary>
/// Converts stock status boolean to display text.
/// </summary>
public class StockStatusConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean stock status to display text.
    /// </summary>
    /// <param name="value">The stock status boolean value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>"In Stock" or "Out of Stock" text.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool inStock)
        {
            return inStock ? "In Stock" : "Out of Stock";
        }
        return "Unknown";
    }

    /// <summary>
    /// Converts back from text to boolean (not implemented).
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Not implemented.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts stock status boolean to display color.
/// </summary>
public class StockColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean stock status to a display color.
    /// </summary>
    /// <param name="value">The stock status boolean value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Green if in stock, Red if out of stock, Gray otherwise.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool inStock)
        {
            return inStock ? Colors.Green : Colors.Red;
        }
        return Colors.Gray;
    }

    /// <summary>
    /// Converts back from color to boolean (not implemented).
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Not implemented.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
