using EShop.Domain.Ports;
using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class JsonDataPortTests
{
    private readonly JsonDataPort<EShop.Domain.Entities.Product> _port = new();

    [Fact]
    public void Export_ValidCollection_ReturnsJsonString()
    {
        var products = new[] { TestFactory.CreateProduct("A", 10m, 5) };

        var json = _port.Export(products);

        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void Export_EmptyCollection_ReturnsEmptyArray()
    {
        var json = _port.Export([]);

        Assert.Equal("[]", json);
    }

    [Fact]
    public void Import_ValidJson_ReturnsCollection()
    {
        var json = "[{\"ProductName\":\"Test\",\"ProductPrice\":10.0}]";

        var result = _port.Import(json);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void Import_EmptyJson_ReturnsEmptyCollection()
    {
        var result = _port.Import("[]");

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
