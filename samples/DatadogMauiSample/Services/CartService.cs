using DatadogMauiSample.Models;
using System.Collections.ObjectModel;

namespace DatadogMauiSample.Services;

/// <summary>
/// Singleton service for managing shopping cart state
/// </summary>
public class CartService
{
    private static CartService? _instance;
    private static readonly object _lock = new();

    /// <summary>
    /// Gets the singleton instance of the cart service.
    /// </summary>
    public static CartService Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new CartService();
                return _instance;
            }
        }
    }

    private readonly ObservableCollection<CartItemModel> _items = new();

    /// <summary>
    /// Gets the collection of items in the cart.
    /// </summary>
    public ObservableCollection<CartItemModel> Items => _items;

    /// <summary>
    /// Gets the total number of items in the cart.
    /// </summary>
    public int ItemCount => _items.Sum(i => i.Quantity);

    /// <summary>
    /// Gets the total amount of all items in the cart.
    /// </summary>
    public decimal TotalAmount => _items.Sum(i => i.Product.Price * i.Quantity);

    /// <summary>
    /// Event raised when the cart is updated.
    /// </summary>
    public event EventHandler? CartUpdated;

    private CartService()
    {
    }

    /// <summary>
    /// Adds an item to the cart or updates quantity if it already exists.
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <param name="quantity">The quantity to add.</param>
    public void AddItem(Product product, int quantity = 1)
    {
        var existingItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItemModel
            {
                Product = product,
                Quantity = quantity
            });
        }

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    /// <param name="productId">The product ID to remove.</param>
    public void RemoveItem(string productId)
    {
        var item = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            _items.Remove(item);
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Updates the quantity of an item in the cart.
    /// </summary>
    /// <param name="productId">The product ID to update.</param>
    /// <param name="quantity">The new quantity.</param>
    public void UpdateQuantity(string productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.Quantity = quantity;
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        CartUpdated?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// Represents an item in the shopping cart.
/// </summary>
public class CartItemModel
{
    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the subtotal for this cart item.
    /// </summary>
    public decimal Subtotal => Product.Price * Quantity;
}
