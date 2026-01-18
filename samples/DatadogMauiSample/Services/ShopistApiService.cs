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

    public ShopistApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DatadogMauiSample/1.0");
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/products");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<FakeStoreProduct>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products == null)
                return new List<Product>();

            // Convert FakeStore API products to our display model
            return products.Take(20).Select(p => new Product
            {
                Id = p.Id.ToString(),
                Name = p.Title,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.Image ?? string.Empty,
                InStock = true
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<string?> CreateCartAsync()
    {
        try
        {
            var cartData = new
            {
                userId = 1,
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                products = new[] { new { productId = 1, quantity = 1 } }
            };

            var json = JsonSerializer.Serialize(cartData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/carts", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var cartResponse = JsonSerializer.Deserialize<FakeStoreCartResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return cartResponse?.Id.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating cart: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> AddItemToCartAsync(string cartId, string productId, int quantity = 1, int amountPaid = 500)
    {
        try
        {
            var cartData = new
            {
                userId = 1,
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                products = new[] { new { productId = int.Parse(productId), quantity } }
            };

            var json = JsonSerializer.Serialize(cartData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/carts", content);
            response.EnsureSuccessStatusCode();

            return $"/carts/{cartId}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding item to cart: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ApplyCouponAsync(string cartId, string couponCode = "100OFF")
    {
        try
        {
            // FakeStore API doesn't have coupon endpoint, so we'll simulate success
            Console.WriteLine($"[Simulated] Applying coupon {couponCode} to cart {cartId}");
            await Task.Delay(500); // Simulate network delay
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying coupon: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CheckoutAsync(string checkoutUrl, string cardNumber = "4242424242424242", string cvc = "123")
    {
        try
        {
            // FakeStore API doesn't have checkout endpoint, so we'll simulate success
            Console.WriteLine($"[Simulated] Checking out with card ending in {cardNumber.Substring(cardNumber.Length - 4)}");
            await Task.Delay(1000); // Simulate network delay
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during checkout: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SimulateFullPurchaseFlowAsync(string productId)
    {
        try
        {
            // Step 1: Create cart
            var cartId = await CreateCartAsync();
            if (string.IsNullOrEmpty(cartId))
            {
                Console.WriteLine("Failed to create cart");
                return false;
            }

            Console.WriteLine($"Created cart: {cartId}");

            // Step 2: Add item to cart
            var checkoutUrl = await AddItemToCartAsync(cartId, productId);
            if (string.IsNullOrEmpty(checkoutUrl))
            {
                Console.WriteLine("Failed to add item to cart");
                return false;
            }

            Console.WriteLine($"Added item, checkout URL: {checkoutUrl}");

            // Step 3: Apply coupon
            var couponApplied = await ApplyCouponAsync(cartId);
            Console.WriteLine($"Coupon applied: {couponApplied}");

            // Step 4: Checkout
            var checkoutSuccess = await CheckoutAsync(checkoutUrl);
            Console.WriteLine($"Checkout success: {checkoutSuccess}");

            return checkoutSuccess;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in purchase flow: {ex.Message}");
            return false;
        }
    }
}
