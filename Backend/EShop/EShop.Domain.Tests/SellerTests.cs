using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class SellerTests
{
    [Fact]
    public void AddProduct_ValidData_AddsToProducts()
    {
        var seller = TestFactory.CreateSeller();

        seller.AddProduct("Widget", "A widget", 9.99m, 100);

        Assert.Single(seller.Products);
    }

    [Fact]
    public void AddProduct_InvalidPrice_ThrowsException()
    {
        var seller = TestFactory.CreateSeller();

        Assert.Throws<ArgumentException>(() => seller.AddProduct("Widget", "A widget", -1m, 10));
    }

    [Fact]
    public void RemoveProduct_ExistingProduct_RemovesFromProducts()
    {
        var seller = TestFactory.CreateSeller(id: 1);
        var product = TestFactory.CreateProduct(sellerId: 1);
        seller.AddProduct("Widget", "A widget", 9.99m, 10);
        int productId = seller.Products[0].Id;

        seller.RemoveProduct(productId);

        Assert.Empty(seller.Products);
    }

    [Fact]
    public void RemoveProduct_NonExistingProduct_ThrowsException()
    {
        var seller = TestFactory.CreateSeller();

        Assert.Throws<InvalidOperationException>(() => seller.RemoveProduct(999));
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectString()
    {
        var seller = TestFactory.CreateSeller();

        var stats = seller.GetStatistics();

        Assert.NotNull(stats);
        Assert.NotEmpty(stats);
    }

    [Fact]
    public void UpdateStock_IncreasesQuantity_CallsIncreaseQuantity()
    {
        var seller = TestFactory.CreateSeller(id: 1);
        seller.AddProduct("Widget", "A widget", 9.99m, 5);
        int productId = seller.Products[0].Id;

        seller.UpdateStock(productId, 10);

        Assert.Equal(10, seller.Products[0].ProductQuantity);
    }

    [Fact]
    public void UpdateStock_DecreasesQuantity_CallsDecreaseQuantity()
    {
        var seller = TestFactory.CreateSeller(id: 1);
        seller.AddProduct("Widget", "A widget", 9.99m, 10);
        int productId = seller.Products[0].Id;

        seller.UpdateStock(productId, 3);

        Assert.Equal(3, seller.Products[0].ProductQuantity);
    }

    [Fact]
    public void UpdateStock_NegativeQuantity_ThrowsException()
    {
        var seller = TestFactory.CreateSeller(id: 1);
        seller.AddProduct("Widget", "A widget", 9.99m, 10);
        int productId = seller.Products[0].Id;

        Assert.Throws<ArgumentException>(() => seller.UpdateStock(productId, -5));
    }

    [Fact]
    public void IsValid_ValidAddress_ReturnsTrue()
    {
        var seller = TestFactory.CreateSeller(address: "123 Seller Ave");

        Assert.True(seller.IsValid());
    }
}
