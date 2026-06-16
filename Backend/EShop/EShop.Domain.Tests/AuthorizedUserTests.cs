using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class AuthorizedUserTests
{
    [Fact]
    public void IsValid_ValidAddress_ReturnsTrue()
    {
        var user = TestFactory.CreateBuyer(address: "123 Main St");

        Assert.True(user.IsValid());
    }

    [Fact]
    public void IsValid_EmptyAddress_ReturnsFalse()
    {
        var user = TestFactory.CreateBuyer(address: "");

        Assert.False(user.IsValid());
    }

    [Fact]
    public void IsValid_WhitespaceAddress_ReturnsFalse()
    {
        var user = TestFactory.CreateBuyer(address: "   ");

        Assert.False(user.IsValid());
    }
}
