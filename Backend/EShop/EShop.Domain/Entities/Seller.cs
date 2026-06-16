using EShop.Domain.Interfaces;

namespace EShop.Domain.Entities;

public class Seller : AuthorizedUser, ISellerActions
{
    public List<Product> Products { get; private set; } = [];

    public void AddProduct(string name, string description, decimal price, int quantity)
    {
        throw new NotImplementedException();
    }

    public void RemoveProduct(int productId)
    {
        throw new NotImplementedException();
    }

    public string GetStatistics()
    {
        throw new NotImplementedException();
    }
    public void UpdateStock(int productId, int newQuantity)
    {
        throw new NotImplementedException();
    }
}