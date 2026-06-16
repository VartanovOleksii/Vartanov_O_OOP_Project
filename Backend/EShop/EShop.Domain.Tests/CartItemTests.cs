using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class CartItemTests
{
    [Fact]
    public void IsValid_PositiveQuantity_ReturnsTrue()
    {
        var item = TestFactory.CreateCartItem(quantity: 3);

        Assert.True(item.IsValid());
    }

    [Fact]
    public void IsValid_ZeroQuantity_ReturnsFalse()
    {
        var item = TestFactory.CreateCartItem(quantity: 0);

        Assert.False(item.IsValid());
    }

    [Fact]
    public void IsValid_NegativeQuantity_ReturnsFalse()
    {
        var item = TestFactory.CreateCartItem(quantity: -1);

        Assert.False(item.IsValid());
    }
}
