using EShop.Domain.DTOs;
using EShop.Domain.Mappers;
using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class MappersTests
{
    [Fact]
    public void ProductMapper_ToDto_MapsAllFields()
    {
        var product = TestFactory.CreateProduct("Widget", 19.99m, 10, sellerId: 2);

        var dto = ProductMapper.ToDto(product);

        Assert.Equal(product.ProductName, dto.Name);
        Assert.Equal(product.ProductPrice, dto.Price);
        Assert.Equal(product.ProductQuantity, dto.Quantity);
        Assert.Equal(product.SellerId, dto.SellerId);
    }

    [Fact]
    public void ProductMapper_ToEntity_MapsAllFields()
    {
        var dto = new ProductExportDto
        {
            Id = 1,
            Name = "Widget",
            Description = "A widget",
            Price = 19.99m,
            Quantity = 10,
            SellerId = 2
        };

        var product = ProductMapper.ToEntity(dto);

        Assert.Equal(dto.Name, product.ProductName);
        Assert.Equal(dto.Price, product.ProductPrice);
        Assert.Equal(dto.Quantity, product.ProductQuantity);
        Assert.Equal(dto.SellerId, product.SellerId);
    }

    [Fact]
    public void OrderMapper_ToDto_MapsAllFields()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m, sellerId: 2);
        var order = EShop.Domain.Entities.Order.Create(buyer, product, 2);

        var dto = OrderMapper.ToDto(order);

        Assert.Equal(order.BuyerId, dto.BuyerId);
        Assert.Equal(order.SellerId, dto.SellerId);
        Assert.Equal(order.ProductId, dto.ProductId);
        Assert.Equal(order.OrderQuantity, dto.Quantity);
        Assert.Equal(order.OrderUnitPrice, dto.UnitPrice);
    }

    [Fact]
    public void OrderMapper_ToDto_CalculatesTotalPrice()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);
        var order = EShop.Domain.Entities.Order.Create(buyer, product, 3);

        var dto = OrderMapper.ToDto(order);

        Assert.Equal(75m, dto.TotalPrice);
    }

    [Fact]
    public void OrderMapper_ToEntity_MapsAllFields()
    {
        var dto = new OrderExportDto
        {
            Id = 1,
            BuyerId = 2,
            SellerId = 3,
            ProductId = 4,
            Quantity = 2,
            UnitPrice = 25m,
            TotalPrice = 50m,
            OrderDate = new DateTime(2025, 1, 1)
        };

        var order = OrderMapper.ToEntity(dto);

        Assert.Equal(dto.BuyerId, order.BuyerId);
        Assert.Equal(dto.SellerId, order.SellerId);
        Assert.Equal(dto.ProductId, order.ProductId);
        Assert.Equal(dto.Quantity, order.OrderQuantity);
        Assert.Equal(dto.UnitPrice, order.OrderUnitPrice);
    }
}
