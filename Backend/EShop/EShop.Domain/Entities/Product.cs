using EShop.Domain.Events;

namespace EShop.Domain.Entities;

public class Product : Entity, IComparable<Product>
{
    public string ProductName { get; private set; } = string.Empty;
    public string ProductDescription { get; private set; } = string.Empty;
    public decimal ProductPrice { get; private set; }
    public int ProductQuantity { get; private set; }
    public int SellerId { get; private set; }
    public Seller? ProductSeller { get; private set; }
    public List<ProductImage> ProductImages { get; private set; } = [];

    public event EventHandler<ProductEventArgs>? StockChanged;

    public Product(string name, string description, decimal price, int quantity, int sellerId)
    {
        ProductName = name;
        ProductDescription = description;
        ProductPrice = price;
        ProductQuantity = quantity;
        SellerId = sellerId;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.", nameof(amount));

        int old = ProductQuantity;
        ProductQuantity += amount;
        StockChanged?.Invoke(this, new ProductEventArgs(Id, old, ProductQuantity));
    }

    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.", nameof(amount));

        if (ProductQuantity < amount)
            throw new InvalidOperationException("Insufficient stock.");

        int old = ProductQuantity;
        ProductQuantity -= amount;
        StockChanged?.Invoke(this, new ProductEventArgs(Id, old, ProductQuantity));
    }

    public void AddImage(ProductImage image)
    {
        if (!image.IsValid())
            throw new ArgumentException("Image is not valid.", nameof(image));

        ProductImages.Add(image);
    }

    public int CompareTo(Product? other)
    {
        if (other is null) return 1;
        return ProductPrice.CompareTo(other.ProductPrice);
    }

    public override bool IsValid() =>
        !string.IsNullOrWhiteSpace(ProductName) &&
        ProductPrice > 0 &&
        ProductQuantity >= 0;
}