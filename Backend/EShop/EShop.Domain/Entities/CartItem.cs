namespace EShop.Domain.Entities;

public class CartItem : Entity
{
    public int BuyerId { get; private set; }
    public Buyer? CartBuyer { get; private set; }

    public int ProductId { get; private set; }
    public Product? CartProduct { get; private set; }

    public int CartQuantity { get; private set; }

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}