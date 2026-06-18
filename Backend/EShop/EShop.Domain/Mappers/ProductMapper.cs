using EShop.Domain.DTOs;
using EShop.Domain.Entities;

namespace EShop.Domain.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.ProductName,
            Description = product.ProductDescription,
            Price = product.ProductPrice,
            Quantity = product.ProductQuantity,
            SellerId = product.SellerId
        };
    }

    public static Product ToEntity(ProductDto dto)
    {
        var product = (Product)Activator.CreateInstance(typeof(Product), nonPublic: true)!;

        typeof(Product).GetProperty(nameof(Product.Id))!
            .SetValue(product, dto.Id);
        typeof(Product).GetProperty(nameof(Product.ProductName))!
            .SetValue(product, dto.Name);
        typeof(Product).GetProperty(nameof(Product.ProductDescription))!
            .SetValue(product, dto.Description);
        typeof(Product).GetProperty(nameof(Product.ProductPrice))!
            .SetValue(product, dto.Price);
        typeof(Product).GetProperty(nameof(Product.ProductQuantity))!
            .SetValue(product, dto.Quantity);
        typeof(Product).GetProperty(nameof(Product.SellerId))!
            .SetValue(product, dto.SellerId);

        return product;
    }
}