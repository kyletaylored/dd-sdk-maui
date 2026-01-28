using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatadogMauiSample.Models;

namespace DatadogMauiSample.Services;

public class ShopistApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://fakestoreapi.com";
    private string? _authToken; // Store auth token after login

    public ShopistApiService()
    {
#if IOS
        // On iOS, use Datadog's HTTP message handler for automatic span creation and trace header injection
        var datadogHandler = new Datadog.Maui.Http.DatadogHttpMessageHandler(new[] { "fakestoreapi.com" });
        _httpClient = new HttpClient(datadogHandler)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        System.Diagnostics.Debug.WriteLine("[ShopistAPI] Using DatadogHttpMessageHandler for iOS HTTP tracing");
#else
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
#endif
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DatadogMauiSample/1.0");
    }

    // ============================================================================
    // Authentication
    // ============================================================================

    /// <summary>
    /// Get all users from the API (used to show available login credentials)
    /// </summary>
    public async Task<List<FakeStoreUser>> GetUsersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[API] GET /users - Fetching all users");
            var response = await _httpClient.GetAsync("/users");
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<FakeStoreUser>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {users?.Count ?? 0} users");
            return users ?? new List<FakeStoreUser>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching users: {ex.Message}");
            return new List<FakeStoreUser>();
        }
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    public async Task<(bool success, string? token, string? error)> LoginAsync(string username, string password)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[API] POST /auth/login - Attempting login for user: {username}");

            var loginData = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[API] ✗ Login failed with status: {response.StatusCode}");
                return (false, null, $"Login failed: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<FakeStoreLoginResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _authToken = result?.Token;
            System.Diagnostics.Debug.WriteLine($"[API] ✓ Login successful, token received");
            return (true, _authToken, null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Login error: {ex.Message}");
            return (false, null, ex.Message);
        }
    }

    // ============================================================================
    // Products
    // ============================================================================

    /// <summary>
    /// Get all products with optional limit
    /// </summary>
    public async Task<List<Product>> GetProductsAsync(int? limit = null)
    {
        try
        {
            var url = limit.HasValue ? $"/products?limit={limit.Value}" : "/products";
            System.Diagnostics.Debug.WriteLine($"[API] GET {url} - Fetching products");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<FakeStoreProduct>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products == null)
                return new List<Product>();

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {products.Count} products");

            // Convert FakeStore API products to our display model
            return products.Select(p => new Product
            {
                Id = p.Id.ToString(),
                Name = p.Title,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.Image ?? string.Empty,
                InStock = true,
                Category = p.Category
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }

    /// <summary>
    /// Get a single product by ID
    /// </summary>
    public async Task<Product?> GetProductByIdAsync(string productId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[API] GET /products/{productId} - Fetching product details");
            var response = await _httpClient.GetAsync($"/products/{productId}");
            response.EnsureSuccessStatusCode();

            var product = await response.Content.ReadFromJsonAsync<FakeStoreProduct>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (product == null)
                return null;

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received product: {product.Title}");

            return new Product
            {
                Id = product.Id.ToString(),
                Name = product.Title,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.Image ?? string.Empty,
                InStock = true,
                Category = product.Category
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching product {productId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[API] GET /products/category/{category} - Fetching products");
            var response = await _httpClient.GetAsync($"/products/category/{category}");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<FakeStoreProduct>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products == null)
                return new List<Product>();

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {products.Count} products in category '{category}'");

            return products.Select(p => new Product
            {
                Id = p.Id.ToString(),
                Name = p.Title,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.Image ?? string.Empty,
                InStock = true,
                Category = p.Category
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching products by category: {ex.Message}");
            return new List<Product>();
        }
    }

    /// <summary>
    /// Get all product categories
    /// </summary>
    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[API] GET /products/categories - Fetching categories");
            var response = await _httpClient.GetAsync("/products/categories");
            response.EnsureSuccessStatusCode();

            var categories = await response.Content.ReadFromJsonAsync<List<string>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {categories?.Count ?? 0} categories");
            return categories ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching categories: {ex.Message}");
            return new List<string>();
        }
    }

    // ============================================================================
    // Carts
    // ============================================================================

    /// <summary>
    /// Create a new shopping cart for a user
    /// </summary>
    public async Task<string?> CreateCartAsync(int userId = 1, List<(string productId, int quantity)>? items = null)
    {
        try
        {
            var products = items != null
                ? items.Select(item => new { productId = int.Parse(item.productId), quantity = item.quantity }).ToList()
                : new[] { new { productId = 1, quantity = 1 } }.ToList();

            var cartData = new
            {
                userId,
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                products
            };

            System.Diagnostics.Debug.WriteLine($"[API] POST /carts - Creating cart for user {userId} with {products.Count} items");
            var response = await _httpClient.PostAsJsonAsync("/carts", cartData);
            response.EnsureSuccessStatusCode();

            var cartResponse = await response.Content.ReadFromJsonAsync<FakeStoreCartResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Cart created with ID: {cartResponse?.Id}");
            return cartResponse?.Id.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error creating cart: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get all carts
    /// </summary>
    public async Task<List<FakeStoreCart>> GetAllCartsAsync(int? limit = null)
    {
        try
        {
            var url = limit.HasValue ? $"/carts?limit={limit.Value}" : "/carts";
            System.Diagnostics.Debug.WriteLine($"[API] GET {url} - Fetching all carts");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var carts = await response.Content.ReadFromJsonAsync<List<FakeStoreCart>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {carts?.Count ?? 0} carts");
            return carts ?? new List<FakeStoreCart>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching carts: {ex.Message}");
            return new List<FakeStoreCart>();
        }
    }

    /// <summary>
    /// Get a single cart by ID
    /// </summary>
    public async Task<FakeStoreCart?> GetCartByIdAsync(string cartId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[API] GET /carts/{cartId} - Fetching cart");
            var response = await _httpClient.GetAsync($"/carts/{cartId}");
            response.EnsureSuccessStatusCode();

            var cart = await response.Content.ReadFromJsonAsync<FakeStoreCart>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received cart with {cart?.Products?.Count ?? 0} items");
            return cart;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error fetching cart: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Update a cart (add/remove items)
    /// </summary>
    public async Task<bool> UpdateCartAsync(string cartId, int userId, List<(string productId, int quantity)> items)
    {
        try
        {
            var products = items.Select(item => new { productId = int.Parse(item.productId), quantity = item.quantity }).ToList();

            var cartData = new
            {
                userId,
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                products
            };

            System.Diagnostics.Debug.WriteLine($"[API] PUT /carts/{cartId} - Updating cart with {products.Count} items");
            var response = await _httpClient.PutAsJsonAsync($"/carts/{cartId}", cartData);
            response.EnsureSuccessStatusCode();

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Cart updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error updating cart: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Delete a cart
    /// </summary>
    public async Task<bool> DeleteCartAsync(string cartId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[API] DELETE /carts/{cartId} - Deleting cart");
            var response = await _httpClient.DeleteAsync($"/carts/{cartId}");
            response.EnsureSuccessStatusCode();

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Cart deleted");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Error deleting cart: {ex.Message}");
            return false;
        }
    }

    // ============================================================================
    // Legacy/Helper Methods
    // ============================================================================

    public async Task<string?> AddItemToCartAsync(string cartId, string productId, int quantity = 1, int amountPaid = 500)
    {
        // Update cart with new item
        var success = await UpdateCartAsync(cartId, 1, new List<(string, int)> { (productId, quantity) });
        return success ? $"/carts/{cartId}" : null;
    }

    public async Task<bool> ApplyCouponAsync(string cartId, string couponCode = "100OFF")
    {
        // FakeStore API doesn't have coupon endpoint, so we'll simulate success
        System.Diagnostics.Debug.WriteLine($"[Simulated] Applying coupon {couponCode} to cart {cartId}");
        await Task.Delay(500); // Simulate network delay
        return true;
    }

    public async Task<bool> CheckoutAsync(string checkoutUrl, string cardNumber = "4242424242424242", string cvc = "123")
    {
        // FakeStore API doesn't have checkout endpoint, so we'll simulate success
        System.Diagnostics.Debug.WriteLine($"[Simulated] Checking out with card ending in {cardNumber.Substring(cardNumber.Length - 4)}");
        await Task.Delay(1000); // Simulate network delay
        return true;
    }

    /// <summary>
    /// Simulates a complete purchase flow with multiple API calls
    /// </summary>
    public async Task<bool> SimulateFullPurchaseFlowAsync(string productId)
    {
        try
        {
            // Step 1: Get product details
            var product = await GetProductByIdAsync(productId);
            if (product == null)
            {
                System.Diagnostics.Debug.WriteLine("[API] Failed to get product details");
                return false;
            }

            // Step 2: Create cart
            var cartId = await CreateCartAsync(1, new List<(string, int)> { (productId, 1) });
            if (string.IsNullOrEmpty(cartId))
            {
                System.Diagnostics.Debug.WriteLine("[API] Failed to create cart");
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[API] Created cart: {cartId}");

            // Step 3: Get cart details to verify
            var cart = await GetCartByIdAsync(cartId);
            System.Diagnostics.Debug.WriteLine($"[API] Cart contains {cart?.Products?.Count ?? 0} items");

            // Step 4: Apply coupon
            var couponApplied = await ApplyCouponAsync(cartId);
            System.Diagnostics.Debug.WriteLine($"[API] Coupon applied: {couponApplied}");

            // Step 5: Checkout
            var checkoutSuccess = await CheckoutAsync($"/carts/{cartId}");
            System.Diagnostics.Debug.WriteLine($"[API] Checkout success: {checkoutSuccess}");

            return checkoutSuccess;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] Error in purchase flow: {ex.Message}");
            return false;
        }
    }
}
