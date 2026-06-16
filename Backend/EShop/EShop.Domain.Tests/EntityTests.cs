using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class EntityTests
{
    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        var e1 = TestFactory.CreateBuyer(id: 1);
        var e2 = TestFactory.CreateBuyer(id: 1);

        Assert.True(e1.Equals(e2));
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var e1 = TestFactory.CreateBuyer(id: 1);
        var e2 = TestFactory.CreateBuyer(id: 2);

        Assert.False(e1.Equals(e2));
    }

    [Fact]
    public void Equals_NullOther_ReturnsFalse()
    {
        var e1 = TestFactory.CreateBuyer(id: 1);

        Assert.False(e1.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameId_ReturnsSameHash()
    {
        var e1 = TestFactory.CreateBuyer(id: 42);
        var e2 = TestFactory.CreateBuyer(id: 42);

        Assert.Equal(e1.GetHashCode(), e2.GetHashCode());
    }
}
