using EShop.Domain.Interfaces;

namespace EShop.Domain.Entities;

public class Buyer : AuthorizedUser, IBuyerActions
{
    public List<CartItem> Cart { get; private set; } = [];
    public List<Order> OrderHistory { get; private set; } = [];

    public void AddToCart(Product product, int quantity)
    {
        throw new NotImplementedException();
    }

    public List<Order> GetHistory()
    {
        throw new NotImplementedException();
    }


    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}