using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class BuyerTests
{
    [Fact]
    public void AddToCart_ValidProduct_AddsToCart()
    {
        var buyer = TestFactory.CreateBuyer();
        var product = TestFactory.CreateProduct(quantity: 10);

        buyer.AddToCart(product, 2);

        Assert.Single(buyer.Cart);
    }

    [Fact]
    public void AddToCart_ProductAlreadyInCart_IncreasesQuantity()
    {
        var buyer = TestFactory.CreateBuyer();
        var product = TestFactory.CreateProduct(quantity: 10);
        buyer.AddToCart(product, 2);

        buyer.AddToCart(product, 3);

        Assert.Equal(5, buyer.Cart[0].CartQuantity);
    }

    [Fact]
    public void AddToCart_QuantityExceedsStock_ThrowsException()
    {
        var buyer = TestFactory.CreateBuyer();
        var product = TestFactory.CreateProduct(quantity: 3);

        Assert.Throws<ArgumentException>(() => buyer.AddToCart(product, 10));
    }

    [Fact]
    public void AddToCart_ZeroQuantity_ThrowsException()
    {
        var buyer = TestFactory.CreateBuyer();
        var product = TestFactory.CreateProduct(quantity: 10);

        Assert.Throws<ArgumentException>(() => buyer.AddToCart(product, 0));
    }

    [Fact]
    public void GetHistory_ReturnsOrderHistory()
    {
        var buyer = TestFactory.CreateBuyer();

        var history = buyer.GetHistory();

        Assert.NotNull(history);
        Assert.IsType<List<EShop.Domain.Entities.Order>>(history);
    }

    [Fact]
    public void IsValid_ValidAddress_ReturnsTrue()
    {
        var buyer = TestFactory.CreateBuyer(address: "123 Main St");

        Assert.True(buyer.IsValid());
    }
}
