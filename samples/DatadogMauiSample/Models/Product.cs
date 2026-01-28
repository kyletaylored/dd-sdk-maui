namespace DatadogMauiSample.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool InStock { get; set; } = true;
    public string Category { get; set; } = string.Empty;
}

public class FakeStoreProduct
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Image { get; set; }
}

public class FakeStoreUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public FakeStoreUserName? Name { get; set; }
    public FakeStoreAddress? Address { get; set; }
    public string Phone { get; set; } = string.Empty;
}

public class FakeStoreUserName
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
}

public class FakeStoreAddress
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public FakeStoreGeolocation? Geolocation { get; set; }
}

public class FakeStoreGeolocation
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}

public class FakeStoreLoginResponse
{
    public string Token { get; set; } = string.Empty;
}

public class FakeStoreCart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<FakeStoreCartProduct>? Products { get; set; }
}

public class FakeStoreCartProduct
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class FakeStoreCartResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Date { get; set; } = string.Empty;
}

public class ProductsResponse
{
    public List<ProductItem> Products { get; set; } = new();
}

public class ProductItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Product_type { get; set; } = string.Empty;
    public List<ProductVariant> Variants { get; set; } = new();
    public ProductImage? Image { get; set; }
}

public class ProductVariant
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Price { get; set; } = "0.00";
    public bool Available { get; set; } = true;
}

public class ProductImage
{
    public string Src { get; set; } = string.Empty;
}

public class CartResponse
{
    public string CartId { get; set; } = string.Empty;
}

public class AddItemRequest
{
    public CartItem Cart_item { get; set; } = new();
    public string Cart_id { get; set; } = string.Empty;
}

public class CartItem
{
    public string Product_id { get; set; } = string.Empty;
    public int Amount_paid { get; set; }
    public int Quantity { get; set; }
}

public class AddItemResponse
{
    public string Url { get; set; } = string.Empty;
}

public class ApplyCouponRequest
{
    public string Cart_id { get; set; } = string.Empty;
}

public class CheckoutRequest
{
    public CheckoutDetails Checkout { get; set; } = new();
}

public class CheckoutDetails
{
    public string Card_number { get; set; } = string.Empty;
    public string Cvc { get; set; } = string.Empty;
    public string Exp { get; set; } = "10/28";
}
