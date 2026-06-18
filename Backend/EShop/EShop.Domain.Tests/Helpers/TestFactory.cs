using EShop.Domain.Entities;

namespace EShop.Domain.Tests.Helpers;

internal static class TestFactory
{
    public static Product CreateProduct(
    string name = "Test Product",
    decimal price = 10m,
    int quantity = 5,
    int sellerId = 1)
    {
        return new Product(name, "Test Description", price, quantity, sellerId);
    }

    public static Buyer CreateBuyer(string address = "Test Address", int id = 1)
    {
        var b = (Buyer)Activator.CreateInstance(typeof(Buyer), nonPublic: true)!;
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(b, id);
        typeof(AuthorizedUser).GetProperty(nameof(AuthorizedUser.UserAddress))!.SetValue(b, address);
        return b;
    }

    public static Seller CreateSeller(string address = "Test Address", int id = 1)
    {
        var s = (Seller)Activator.CreateInstance(typeof(Seller), nonPublic: true)!;
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(s, id);
        typeof(AuthorizedUser).GetProperty(nameof(AuthorizedUser.UserAddress))!.SetValue(s, address);
        return s;
    }

    public static ProductImage CreateProductImage(string path = "images/test.jpg", int productId = 1)
    {
        return new ProductImage(productId, path);
    }

    public static CartItem CreateCartItem(int quantity = 1)
    {
        var buyer = CreateBuyer();
        var product = CreateProduct(quantity: quantity);
        return new CartItem(buyer, product, quantity);
    }
}
