namespace EShop.Domain.Entities;

public class ProductImage : Entity
{
    public int ProductId { get; private set; }
    public Product? ImageProduct { get; private set; }

    public string ImagePath { get; private set; } = string.Empty;

    public string? ImageAltText { get; private set; }

    private ProductImage() { }

    public ProductImage(int productId, string imagePath, string? imageAltText = null)
    {
        ProductId = productId;
        ImagePath = imagePath;
        ImageAltText = imageAltText;
    }

    public override bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(ImagePath))
            return false;

        return true;
    }
}
