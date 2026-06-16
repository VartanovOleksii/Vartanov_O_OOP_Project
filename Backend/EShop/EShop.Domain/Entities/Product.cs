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

    public void IncreaseQuantity(int amount)
    {
        throw new NotImplementedException();
    }

    public void DecreaseQuantity(int amount)
    {
        throw new NotImplementedException();
    }

    public void AddImage(ProductImage image)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(Product? other)
    {
        throw new NotImplementedException();
    }

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}