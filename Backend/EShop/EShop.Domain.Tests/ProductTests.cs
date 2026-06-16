using EShop.Domain.Events;
using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void IncreaseQuantity_ValidAmount_IncreasesStock()
    {
        var product = TestFactory.CreateProduct(quantity: 5);

        product.IncreaseQuantity(3);

        Assert.Equal(8, product.ProductQuantity);
    }

    [Fact]
    public void IncreaseQuantity_ZeroAmount_ThrowsException()
    {
        var product = TestFactory.CreateProduct(quantity: 5);

        Assert.Throws<ArgumentException>(() => product.IncreaseQuantity(0));
    }

    [Fact]
    public void IncreaseQuantity_ValidAmount_RaisesStockChangedEvent()
    {
        var product = TestFactory.CreateProduct(quantity: 5);
        ProductEventArgs? capturedArgs = null;
        product.StockChanged += (_, e) => capturedArgs = e;

        product.IncreaseQuantity(3);

        Assert.NotNull(capturedArgs);
        Assert.Equal(8, capturedArgs.NewQuantity);
    }

    [Fact]
    public void DecreaseQuantity_ValidAmount_DecreasesStock()
    {
        var product = TestFactory.CreateProduct(quantity: 10);

        product.DecreaseQuantity(4);

        Assert.Equal(6, product.ProductQuantity);
    }

    [Fact]
    public void DecreaseQuantity_InsufficientStock_ThrowsException()
    {
        var product = TestFactory.CreateProduct(quantity: 3);

        Assert.Throws<InvalidOperationException>(() => product.DecreaseQuantity(10));
    }

    [Fact]
    public void DecreaseQuantity_ValidAmount_RaisesStockChangedEvent()
    {
        var product = TestFactory.CreateProduct(quantity: 10);
        ProductEventArgs? capturedArgs = null;
        product.StockChanged += (_, e) => capturedArgs = e;

        product.DecreaseQuantity(4);

        Assert.NotNull(capturedArgs);
        Assert.Equal(6, capturedArgs.NewQuantity);
    }

    [Fact]
    public void AddImage_ValidImage_AddsToProductImages()
    {
        var product = TestFactory.CreateProduct();
        var image = TestFactory.CreateProductImage("images/photo.jpg");

        product.AddImage(image);

        Assert.Single(product.ProductImages);
    }

    [Fact]
    public void AddImage_InvalidImage_ThrowsException()
    {
        var product = TestFactory.CreateProduct();
        var invalidImage = TestFactory.CreateProductImage("");

        Assert.Throws<ArgumentException>(() => product.AddImage(invalidImage));
    }

    [Fact]
    public void CompareTo_HigherPrice_ReturnsNegative()
    {
        var cheap = TestFactory.CreateProduct(price: 5m);
        var expensive = TestFactory.CreateProduct(price: 20m);

        Assert.True(cheap.CompareTo(expensive) < 0);
    }

    [Fact]
    public void CompareTo_LowerPrice_ReturnsPositive()
    {
        var cheap = TestFactory.CreateProduct(price: 5m);
        var expensive = TestFactory.CreateProduct(price: 20m);

        Assert.True(expensive.CompareTo(cheap) > 0);
    }

    [Fact]
    public void CompareTo_EqualPrice_ReturnsZero()
    {
        var p1 = TestFactory.CreateProduct(price: 10m);
        var p2 = TestFactory.CreateProduct(price: 10m);

        Assert.Equal(0, p1.CompareTo(p2));
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var product = TestFactory.CreateProduct(price: 10m);

        Assert.True(product.CompareTo(null) > 0);
    }

    [Fact]
    public void IsValid_ValidProduct_ReturnsTrue()
    {
        var product = TestFactory.CreateProduct(name: "Valid Product", price: 10m, quantity: 5);

        Assert.True(product.IsValid());
    }

    [Fact]
    public void IsValid_EmptyName_ReturnsFalse()
    {
        var product = TestFactory.CreateProduct(name: "");

        Assert.False(product.IsValid());
    }

    [Fact]
    public void IsValid_ZeroPrice_ReturnsFalse()
    {
        var product = TestFactory.CreateProduct(price: 0m);

        Assert.False(product.IsValid());
    }

    [Fact]
    public void IsValid_NegativePrice_ReturnsFalse()
    {
        var product = TestFactory.CreateProduct(price: -5m);

        Assert.False(product.IsValid());
    }
}
