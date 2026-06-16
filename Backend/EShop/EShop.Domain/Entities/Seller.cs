using EShop.Domain.Interfaces;

namespace EShop.Domain.Entities;

public class Seller : AuthorizedUser, ISellerActions
{
    public List<Product> Products { get; private set; } = [];

    public void AddProduct(string name, string description, decimal price, int quantity)
    {
        var product = new Product(name, description, price, quantity, Id);

        if (!product.IsValid())
            throw new ArgumentException("Product data is not valid.");

        Products.Add(product);
    }

    public void RemoveProduct(int productId)
    {
        var product = Products.FirstOrDefault(p => p.Id == productId);

        if (product is null)
            throw new InvalidOperationException("Product not found or does not belong to this seller.");

        Products.Remove(product);
    }

    public string GetStatistics()
    {
        int totalStock = Products.Sum(p => p.ProductQuantity);
        return $"Total products: {Products.Count}, Total stock: {totalStock}";
    }

    public void UpdateStock(int productId, int newQuantity)
    {
        var product = Products.FirstOrDefault(p => p.Id == productId);

        if (product is null)
            throw new InvalidOperationException("Product not found or does not belong to this seller.");

        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(newQuantity));

        int diff = newQuantity - product.ProductQuantity;

        if (diff > 0)
            product.IncreaseQuantity(diff);
        else if (diff < 0)
            product.DecreaseQuantity(Math.Abs(diff));
    }
}