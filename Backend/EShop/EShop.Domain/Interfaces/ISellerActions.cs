namespace EShop.Domain.Interfaces;

public interface ISellerActions
{
    void AddProduct(string name, string description, decimal price, int quantity);

    void RemoveProduct(int productId);

    string GetStatistics();

    void UpdateStock(int productId, int newQuantity);
}