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

    public static Order Create(Buyer buyer, Product product, int quantity)
    {
        throw new NotImplementedException();
    }

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}