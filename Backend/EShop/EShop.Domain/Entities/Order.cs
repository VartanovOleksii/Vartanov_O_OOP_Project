using System.Text.Json.Serialization;

namespace EShop.Domain.Entities;

public class Order : Entity
{
    public int SellerId { get; private set; }
    public Seller? OrderSeller { get; private set; }

    public int BuyerId { get; private set; }
    public Buyer? OrderBuyer { get; private set; }

    public int ProductId { get; private set; }
    public Product? OrderProduct { get; private set; }

    public int OrderQuantity { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal OrderUnitPrice { get; private set; }
    public decimal OrderTotalPrice => OrderUnitPrice * OrderQuantity;

    [JsonConstructor]
    private Order() { }

    public static Order Create(Buyer buyer, Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (product.ProductQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        var order = new Order
        {
            BuyerId = buyer.Id,
            OrderBuyer = buyer,
            SellerId = product.SellerId,
            OrderSeller = product.ProductSeller,
            ProductId = product.Id,
            OrderProduct = product,
            OrderQuantity = quantity,
            OrderUnitPrice = product.ProductPrice,  // ← снимок цены
            OrderDate = DateTime.UtcNow
        };

        product.DecreaseQuantity(quantity);
        buyer.OrderHistory.Add(order);

        return order;
    }

    public override bool IsValid() => OrderQuantity > 0;
}