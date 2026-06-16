using EShop.Domain.Entities;
using EShop.Domain.Tests.Helpers;

namespace EShop.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void Create_ValidData_CreatesOrder()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);

        var order = Order.Create(buyer, product, 2);

        Assert.NotNull(order);
    }

    [Fact]
    public void Create_ValidData_SnapshotsPriceInOrderUnitPrice()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);

        var order = Order.Create(buyer, product, 2);

        Assert.Equal(25m, order.OrderUnitPrice);
    }

    [Fact]
    public void Create_ValidData_DecreasesProductStock()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);

        Order.Create(buyer, product, 3);

        Assert.Equal(7, product.ProductQuantity);
    }

    [Fact]
    public void Create_ValidData_AddsToOrderHistory()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);

        Order.Create(buyer, product, 2);

        Assert.Single(buyer.OrderHistory);
    }

    [Fact]
    public void Create_InsufficientStock_ThrowsException()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 2, price: 25m);

        Assert.Throws<InvalidOperationException>(() => Order.Create(buyer, product, 10));
    }

    [Fact]
    public void Create_ZeroQuantity_ThrowsException()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);

        Assert.Throws<ArgumentException>(() => Order.Create(buyer, product, 0));
    }

    [Fact]
    public void IsValid_ValidQuantity_ReturnsTrue()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);
        var order = Order.Create(buyer, product, 2);

        Assert.True(order.IsValid());
    }

    [Fact]
    public void IsValid_ZeroQuantity_ReturnsFalse()
    {
        var buyer = TestFactory.CreateBuyer(id: 1);
        var product = TestFactory.CreateProduct(quantity: 10, price: 25m);
        var order = Order.Create(buyer, product, 2);
        typeof(Order).GetProperty(nameof(Order.OrderQuantity))!.SetValue(order, 0);

        Assert.False(order.IsValid());
    }
}
