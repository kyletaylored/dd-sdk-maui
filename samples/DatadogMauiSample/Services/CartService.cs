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

    public ObservableCollection<CartItemModel> Items => _items;

    public int ItemCount => _items.Sum(i => i.Quantity);

    public decimal TotalAmount => _items.Sum(i => i.Product.Price * i.Quantity);

    public event EventHandler? CartUpdated;

    private CartService()
    {
    }

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

    public void RemoveItem(string productId)
    {
        var item = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            _items.Remove(item);
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

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

    public void Clear()
    {
        _items.Clear();
        CartUpdated?.Invoke(this, EventArgs.Empty);
    }
}

public class CartItemModel
{
    public Product Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal Subtotal => Product.Price * Quantity;
}
