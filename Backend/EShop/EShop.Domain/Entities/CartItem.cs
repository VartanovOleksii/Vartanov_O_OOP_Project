namespace EShop.Domain.Entities;

public class CartItem : Entity
{
    public int BuyerId { get; private set; }
    public Buyer? CartBuyer { get; private set; }

    public int ProductId { get; private set; }
    public Product? CartProduct { get; private set; }

    public int CartQuantity { get; private set; }

    public void IncreaseQuantity(int amount) => CartQuantity += amount;

    public override bool IsValid()
    {
        if (CartQuantity > 0)
            return true;

        return false;
    }

    public CartItem(Buyer buyer, Product product, int quantity)
    {
        BuyerId = buyer.Id;
        CartBuyer = buyer;
        ProductId = product.Id;
        CartProduct = product;
        CartQuantity = quantity;
    }
}