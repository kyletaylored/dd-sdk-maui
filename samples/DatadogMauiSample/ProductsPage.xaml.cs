using DatadogMauiSample.Models;
using DatadogMauiSample.Services;

namespace DatadogMauiSample;

public partial class ProductsPage : ContentPage
{
    private readonly ShopistApiService _apiService;
    private List<Product> _products = new();

    public ProductsPage()
    {
        InitializeComponent();
        _apiService = new ShopistApiService();

        // Add converters for data binding
        Resources.Add("StockStatusConverter", new StockStatusConverter());
        Resources.Add("StockColorConverter", new StockColorConverter());

        LoadProductsAsync();
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            _products = await _apiService.GetProductsAsync();
            ProductsCollectionView.ItemsSource = _products;

            await DisplayAlert("Success", $"Loaded {_products.Count} products", "OK");
        }
        catch (Exception ex)
        {
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
            var action = await DisplayActionSheet(
                $"{product.Name}",
                "Cancel",
                null,
                "Add to Cart",
                "Buy Now"
            );

            if (action == "Add to Cart")
            {
                await AddToCartAsync(product);
            }
            else if (action == "Buy Now")
            {
                await PurchaseProductAsync(product);
            }

            // Deselect the item
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async Task AddToCartAsync(Product product)
    {
        try
        {
            var cartId = await _apiService.CreateCartAsync();
            if (cartId != null)
            {
                var checkoutUrl = await _apiService.AddItemToCartAsync(cartId, product.Id);
                if (checkoutUrl != null)
                {
                    await DisplayAlert("Success", $"Added {product.Name} to cart!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to add item to cart", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to create cart", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
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

// Converters for data binding
public class StockStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool inStock)
        {
            return inStock ? "In Stock" : "Out of Stock";
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StockColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool inStock)
        {
            return inStock ? Colors.Green : Colors.Red;
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
