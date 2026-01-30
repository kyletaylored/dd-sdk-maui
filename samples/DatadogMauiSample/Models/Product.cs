namespace DatadogMauiSample.Models;

/// <summary>
/// Represents a product in the application.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the product image URL.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the product is in stock.
    /// </summary>
    public bool InStock { get; set; } = true;

    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Represents a product from the FakeStore API.
/// </summary>
public class FakeStoreProduct
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product image URL.
    /// </summary>
    public string? Image { get; set; }
}

/// <summary>
/// Represents a user from the FakeStore API.
/// </summary>
public class FakeStoreUser
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public FakeStoreUserName? Name { get; set; }

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public FakeStoreAddress? Address { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Represents a user's name from the FakeStore API.
/// </summary>
public class FakeStoreUserName
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string Firstname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string Lastname { get; set; } = string.Empty;
}

/// <summary>
/// Represents an address from the FakeStore API.
/// </summary>
public class FakeStoreAddress
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street name.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the zip code.
    /// </summary>
    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geolocation coordinates.
    /// </summary>
    public FakeStoreGeolocation? Geolocation { get; set; }
}

/// <summary>
/// Represents geolocation coordinates from the FakeStore API.
/// </summary>
public class FakeStoreGeolocation
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public string Lat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public string Long { get; set; } = string.Empty;
}

/// <summary>
/// Represents a login response from the FakeStore API.
/// </summary>
public class FakeStoreLoginResponse
{
    /// <summary>
    /// Gets or sets the authentication token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Represents a shopping cart from the FakeStore API.
/// </summary>
public class FakeStoreCart
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the cart creation date.
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of products in the cart.
    /// </summary>
    public List<FakeStoreCartProduct>? Products { get; set; }
}

/// <summary>
/// Represents a product in a shopping cart from the FakeStore API.
/// </summary>
public class FakeStoreCartProduct
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Represents a cart response from the FakeStore API.
/// </summary>
public class FakeStoreCartResponse
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the cart date.
    /// </summary>
    public string Date { get; set; } = string.Empty;
}

/// <summary>
/// Represents a products response from the Shopist API.
/// </summary>
public class ProductsResponse
{
    /// <summary>
    /// Gets or sets the list of products.
    /// </summary>
    public List<ProductItem>? Products { get; set; }
}

/// <summary>
/// Represents a product item from the Shopist API.
/// </summary>
public class ProductItem
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor name.
    /// </summary>
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product type.
    /// </summary>
    public string Product_type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product variants.
    /// </summary>
    public List<ProductVariant>? Variants { get; set; }

    /// <summary>
    /// Gets or sets the product image.
    /// </summary>
    public ProductImage? Image { get; set; }
}

/// <summary>
/// Represents a product variant from the Shopist API.
/// </summary>
public class ProductVariant
{
    /// <summary>
    /// Gets or sets the variant ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant price.
    /// </summary>
    public string Price { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the variant is available.
    /// </summary>
    public bool Available { get; set; }
}

/// <summary>
/// Represents a product image from the Shopist API.
/// </summary>
public class ProductImage
{
    /// <summary>
    /// Gets or sets the image source URL.
    /// </summary>
    public string Src { get; set; } = string.Empty;
}

/// <summary>
/// Represents a cart response from the Shopist API.
/// </summary>
public class CartResponse
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public string CartId { get; set; } = string.Empty;
}

/// <summary>
/// Represents an add item request for the Shopist API.
/// </summary>
public class AddItemRequest
{
    /// <summary>
    /// Gets or sets the cart item to add.
    /// </summary>
    public CartItem? Cart_item { get; set; }

    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public string Cart_id { get; set; } = string.Empty;
}

/// <summary>
/// Represents a cart item for the Shopist API.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public string Product_id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount paid.
    /// </summary>
    public int Amount_paid { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Represents an add item response from the Shopist API.
/// </summary>
public class AddItemResponse
{
    /// <summary>
    /// Gets or sets the response URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Represents an apply coupon request for the Shopist API.
/// </summary>
public class ApplyCouponRequest
{
    /// <summary>
    /// Gets or sets the cart ID.
    /// </summary>
    public string Cart_id { get; set; } = string.Empty;
}

/// <summary>
/// Represents a checkout request for the Shopist API.
/// </summary>
public class CheckoutRequest
{
    /// <summary>
    /// Gets or sets the checkout details.
    /// </summary>
    public CheckoutDetails? Checkout { get; set; }
}

/// <summary>
/// Represents checkout details for the Shopist API.
/// </summary>
public class CheckoutDetails
{
    /// <summary>
    /// Gets or sets the card number.
    /// </summary>
    public string Card_number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card CVC.
    /// </summary>
    public string Cvc { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card expiration date.
    /// </summary>
    public string Exp { get; set; } = string.Empty;
}
