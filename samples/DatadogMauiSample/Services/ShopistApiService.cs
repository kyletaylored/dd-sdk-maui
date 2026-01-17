using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatadogMauiSample.Models;

namespace DatadogMauiSample.Services;

public class ShopistApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.shopist.io";

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
            var response = await _httpClient.GetAsync("/products.json");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var productsResponse = JsonSerializer.Deserialize<ProductsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (productsResponse?.Products == null)
                return new List<Product>();

            // Convert API products to our display model
            return productsResponse.Products.Take(20).Select(p => new Product
            {
                Id = p.Variants.FirstOrDefault()?.Id ?? p.Id,
                Name = p.Title,
                Description = $"{p.Vendor} - {p.Product_type}",
                Price = decimal.TryParse(p.Variants.FirstOrDefault()?.Price, out var price) ? price : 0m,
                ImageUrl = p.Image?.Src ?? string.Empty,
                InStock = p.Variants.FirstOrDefault()?.Available ?? false
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
            var response = await _httpClient.PostAsync("/carts", null);
            response.EnsureSuccessStatusCode();

            // Extract cart ID from Location header
            if (response.Headers.Location != null)
            {
                var location = response.Headers.Location.ToString();
                var match = Regex.Match(location, @"/carts/(\d+)");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
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
            var request = new AddItemRequest
            {
                Cart_id = cartId,
                Cart_item = new CartItem
                {
                    Product_id = productId,
                    Quantity = quantity,
                    Amount_paid = amountPaid
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/add_item.json", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var addItemResponse = JsonSerializer.Deserialize<AddItemResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return addItemResponse?.Url;
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
            var request = new ApplyCouponRequest
            {
                Cart_id = cartId
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/apply_coupon/{couponCode}", content);
            return response.IsSuccessStatusCode;
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
            var request = new CheckoutRequest
            {
                Checkout = new CheckoutDetails
                {
                    Card_number = cardNumber,
                    Cvc = cvc,
                    Exp = "10/28"
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(checkoutUrl, content);
            return response.IsSuccessStatusCode;
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
