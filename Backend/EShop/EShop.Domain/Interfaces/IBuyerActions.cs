using EShop.Domain.Entities;

namespace EShop.Domain.Interfaces;

public interface IBuyerActions
{
    void AddToCart(Product product, int quantity);

    List<Order> GetHistory();
}