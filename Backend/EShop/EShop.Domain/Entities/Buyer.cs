using EShop.Domain.Interfaces;

namespace EShop.Domain.Entities;

public class Buyer : AuthorizedUser, IBuyerActions
{
    public List<CartItem> Cart { get; private set; } = [];
    public List<Order> OrderHistory { get; private set; } = [];

    public void AddToCart(Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (quantity > product.ProductQuantity)
            throw new ArgumentException("Quantity exceeds available stock.", nameof(quantity));

        var existing = Cart.FirstOrDefault(ci => ci.ProductId == product.Id);

        if (existing != null)
            existing.IncreaseQuantity(quantity);
        else
            Cart.Add(new CartItem(this, product, quantity));
    }

    public List<Order> GetHistory() => OrderHistory;
}