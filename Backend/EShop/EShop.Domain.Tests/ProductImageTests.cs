using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class ProductImageTests
{
    [Fact]
    public void IsValid_ValidPath_ReturnsTrue()
    {
        var image = TestFactory.CreateProductImage("images/photo.jpg");

        Assert.True(image.IsValid());
    }

    [Fact]
    public void IsValid_EmptyPath_ReturnsFalse()
    {
        var image = TestFactory.CreateProductImage("");

        Assert.False(image.IsValid());
    }

    [Fact]
    public void IsValid_WhitespacePath_ReturnsFalse()
    {
        var image = TestFactory.CreateProductImage("   ");

        Assert.False(image.IsValid());
    }
}
