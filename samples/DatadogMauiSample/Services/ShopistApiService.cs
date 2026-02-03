using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatadogMauiSample.Models;
using Datadog.Maui.Logs;
using Datadog.Maui.Rum;
using Datadog.Maui.Tracing;

namespace DatadogMauiSample.Services;

/// <summary>
/// Service for interacting with the Shopist and FakeStore APIs.
/// </summary>
public class ShopistApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private const string BaseUrl = "https://fakestoreapi.com";
    private string? _authToken; // Store auth token after login

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopistApiService"/> class.
    /// </summary>
    public ShopistApiService()
    {
        // Create a logger for this service
        _logger = Logs.CreateLogger("shopist-api");
#if IOS && FALSE  // Temporarily disabled for testing
        // On iOS, use Datadog's HTTP message handler for automatic span creation and trace header injection
        var datadogHandler = new Datadog.Maui.Http.DatadogHttpMessageHandler(new[] { "fakestoreapi.com" });
        _httpClient = new HttpClient(datadogHandler)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        var msg = "[ShopistAPI] Using DatadogHttpMessageHandler for iOS HTTP tracing";
        System.Diagnostics.Debug.WriteLine(msg);
        Console.WriteLine(msg);
#else
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        var msg = "[ShopistAPI] Using plain HttpClient (DatadogHttpMessageHandler disabled for testing)";
        System.Diagnostics.Debug.WriteLine(msg);
        Console.WriteLine(msg);
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
        // Start a trace span for login operation
        using var span = Tracer.StartSpan("api.login");
        span.SetTag("username", username);

        // Track as RUM resource
        var resourceKey = $"login_{Guid.NewGuid()}";
        Rum.StartResource(resourceKey, "POST", $"{BaseUrl}/auth/login");

        try
        {
            _logger.Info("Attempting user login", error: null, attributes: new Dictionary<string, object>
            {
                { "username", username }
            });

            System.Diagnostics.Debug.WriteLine($"[API] POST /auth/login - Attempting login for user: {username}");

            var loginData = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[API] ✗ Login failed with status: {response.StatusCode}");

                _logger.Warn("Login failed", error: null, attributes: new Dictionary<string, object>
                {
                    { "username", username },
                    { "status_code", (int)response.StatusCode }
                });

                span.SetTag("login_success", "false");
                span.SetTag("status_code", ((int)response.StatusCode).ToString());

                Rum.StopResource(resourceKey, statusCode: (int)response.StatusCode, kind: RumResourceKind.Xhr);
                Rum.AddAction(RumActionType.Custom, "login_failed", new Dictionary<string, object>
                {
                    { "username", username },
                    { "status_code", (int)response.StatusCode }
                });

                return (false, null, $"Login failed: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<FakeStoreLoginResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _authToken = result?.Token;
            System.Diagnostics.Debug.WriteLine($"[API] ✓ Login successful, token received");

            _logger.Info("Login successful", error: null, attributes: new Dictionary<string, object>
            {
                { "username", username }
            });

            span.SetTag("login_success", "true");
            Rum.StopResource(resourceKey, statusCode: (int)response.StatusCode, kind: RumResourceKind.Xhr);
            Rum.AddAction(RumActionType.Custom, "login_success", new Dictionary<string, object>
            {
                { "username", username }
            });

            return (true, _authToken, null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] ✗ Login error: {ex.Message}");

            _logger.Error("Login error", ex, new Dictionary<string, object>
            {
                { "username", username }
            });

            span.SetError(ex);
            Rum.StopResourceWithError(resourceKey, ex);
            Rum.AddError(ex, RumErrorSource.Network, new Dictionary<string, object>
            {
                { "username", username },
                { "operation", "login" }
            });

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
        // Start a distributed trace span
        using var span = Tracer.StartSpan("api.get_products");
        if (limit.HasValue)
        {
            span.SetTag("limit", limit.Value.ToString());
        }

        // Track as RUM resource
        var resourceKey = $"get_products_{Guid.NewGuid()}";
        var url = limit.HasValue ? $"/products?limit={limit.Value}" : "/products";
        Rum.StartResource(resourceKey, "GET", $"{BaseUrl}{url}");

        try
        {
            _logger.Info("Fetching products", error: null, attributes: new Dictionary<string, object>
            {
                { "limit", limit ?? -1 },
                { "url", url }
            });

            System.Diagnostics.Debug.WriteLine($"[API] GET {url} - Fetching products");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<FakeStoreProduct>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products == null)
            {
                _logger.Warn("Received null products response");
                Rum.StopResource(resourceKey, statusCode: (int)response.StatusCode, size: 0, kind: RumResourceKind.Xhr);
                return new List<Product>();
            }

            System.Diagnostics.Debug.WriteLine($"[API] ✓ Received {products.Count} products");

            _logger.Info("Products fetched successfully", error: null, attributes: new Dictionary<string, object>
            {
                { "product_count", products.Count }
            });

            span.SetTag("product_count", products.Count.ToString());
            Rum.StopResource(resourceKey,
                statusCode: (int)response.StatusCode,
                size: response.Content.Headers.ContentLength ?? 0,
                kind: RumResourceKind.Xhr);

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

            _logger.Error("Failed to fetch products", ex, new Dictionary<string, object>
            {
                { "url", url }
            });

            span.SetError(ex);
            Rum.StopResourceWithError(resourceKey, ex);
            Rum.AddError(ex, RumErrorSource.Network);

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

    /// <summary>
    /// Adds an item to a cart.
    /// </summary>
    /// <param name="cartId">The cart ID.</param>
    /// <param name="productId">The product ID.</param>
    /// <param name="quantity">The quantity to add.</param>
    /// <param name="amountPaid">The amount paid.</param>
    /// <returns>The cart URL if successful, null otherwise.</returns>
    public async Task<string?> AddItemToCartAsync(string cartId, string productId, int quantity = 1, int amountPaid = 500)
    {
        // Update cart with new item
        var success = await UpdateCartAsync(cartId, 1, new List<(string, int)> { (productId, quantity) });
        return success ? $"/carts/{cartId}" : null;
    }

    /// <summary>
    /// Applies a coupon to a cart.
    /// </summary>
    /// <param name="cartId">The cart ID.</param>
    /// <param name="couponCode">The coupon code to apply.</param>
    /// <returns>True if successful.</returns>
    public async Task<bool> ApplyCouponAsync(string cartId, string couponCode = "100OFF")
    {
        // FakeStore API doesn't have coupon endpoint, so we'll simulate success
        System.Diagnostics.Debug.WriteLine($"[Simulated] Applying coupon {couponCode} to cart {cartId}");
        await Task.Delay(500); // Simulate network delay
        return true;
    }

    /// <summary>
    /// Performs checkout with payment details.
    /// </summary>
    /// <param name="checkoutUrl">The checkout URL.</param>
    /// <param name="cardNumber">The card number.</param>
    /// <param name="cvc">The card CVC.</param>
    /// <returns>True if successful.</returns>
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
        // Create a parent span for the entire purchase flow
        using var purchaseSpan = Tracer.StartSpan("api.purchase_flow");
        purchaseSpan.SetTag("product_id", productId);

        // Track this as a custom RUM action
        Rum.AddAction(RumActionType.Custom, "purchase_flow_started", new Dictionary<string, object>
        {
            { "product_id", productId }
        });

        try
        {
            _logger.Info("Starting purchase flow", error: null, attributes: new Dictionary<string, object>
            {
                { "product_id", productId }
            });

            // Step 1: Get product details
            using (var getProductSpan = Tracer.StartSpan("api.get_product", purchaseSpan))
            {
                getProductSpan.SetTag("product_id", productId);
                var product = await GetProductByIdAsync(productId);
                if (product == null)
                {
                    _logger.Error("Failed to get product details in purchase flow");
                    System.Diagnostics.Debug.WriteLine("[API] Failed to get product details");
                    Rum.AddError("Product not found", RumErrorSource.Source, attributes: new Dictionary<string, object>
                    {
                        { "product_id", productId },
                        { "step", "get_product" }
                    });
                    return false;
                }
                getProductSpan.SetTag("product_name", product.Name);
                getProductSpan.SetTag("product_price", product.Price.ToString());
            }

            // Step 2: Create cart
            string? cartId;
            using (var createCartSpan = Tracer.StartSpan("api.create_cart", purchaseSpan))
            {
                cartId = await CreateCartAsync(1, new List<(string, int)> { (productId, 1) });
                if (string.IsNullOrEmpty(cartId))
                {
                    _logger.Error("Failed to create cart in purchase flow");
                    System.Diagnostics.Debug.WriteLine("[API] Failed to create cart");
                    Rum.AddError("Cart creation failed", RumErrorSource.Source, attributes: new Dictionary<string, object>
                    {
                        { "product_id", productId },
                        { "step", "create_cart" }
                    });
                    return false;
                }
                createCartSpan.SetTag("cart_id", cartId);
                System.Diagnostics.Debug.WriteLine($"[API] Created cart: {cartId}");
            }

            purchaseSpan.SetTag("cart_id", cartId);
            Rum.AddTiming("cart_created");

            // Step 3: Get cart details to verify
            using (var getCartSpan = Tracer.StartSpan("api.get_cart", purchaseSpan))
            {
                var cart = await GetCartByIdAsync(cartId);
                var itemCount = cart?.Products?.Count ?? 0;
                getCartSpan.SetTag("item_count", itemCount.ToString());
                System.Diagnostics.Debug.WriteLine($"[API] Cart contains {itemCount} items");
            }

            // Step 4: Apply coupon
            using (var couponSpan = Tracer.StartSpan("api.apply_coupon", purchaseSpan))
            {
                var couponApplied = await ApplyCouponAsync(cartId);
                couponSpan.SetTag("coupon_applied", couponApplied.ToString());
                System.Diagnostics.Debug.WriteLine($"[API] Coupon applied: {couponApplied}");
                Rum.AddTiming("coupon_applied");
            }

            // Step 5: Checkout
            bool checkoutSuccess;
            using (var checkoutSpan = Tracer.StartSpan("api.checkout", purchaseSpan))
            {
                checkoutSuccess = await CheckoutAsync($"/carts/{cartId}");
                checkoutSpan.SetTag("checkout_success", checkoutSuccess.ToString());
                System.Diagnostics.Debug.WriteLine($"[API] Checkout success: {checkoutSuccess}");
            }

            if (checkoutSuccess)
            {
                _logger.Info("Purchase flow completed successfully", error: null, attributes: new Dictionary<string, object>
                {
                    { "product_id", productId },
                    { "cart_id", cartId }
                });

                purchaseSpan.SetTag("purchase_success", "true");
                Rum.AddAction(RumActionType.Custom, "purchase_completed", new Dictionary<string, object>
                {
                    { "product_id", productId },
                    { "cart_id", cartId }
                });
            }

            return checkoutSuccess;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[API] Error in purchase flow: {ex.Message}");

            _logger.Error("Purchase flow failed", ex, new Dictionary<string, object>
            {
                { "product_id", productId }
            });

            purchaseSpan.SetError(ex);
            Rum.AddError(ex, RumErrorSource.Source, new Dictionary<string, object>
            {
                { "product_id", productId },
                { "flow", "purchase" }
            });

            return false;
        }
    }
}
