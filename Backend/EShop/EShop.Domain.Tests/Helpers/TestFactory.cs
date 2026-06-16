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
        var p = (Product)Activator.CreateInstance(typeof(Product), nonPublic: true)!;
        typeof(Product).GetProperty(nameof(Product.ProductName))!.SetValue(p, name);
        typeof(Product).GetProperty(nameof(Product.ProductPrice))!.SetValue(p, price);
        typeof(Product).GetProperty(nameof(Product.ProductQuantity))!.SetValue(p, quantity);
        typeof(Product).GetProperty(nameof(Product.SellerId))!.SetValue(p, sellerId);
        return p;
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

    public static ProductImage CreateProductImage(string path = "images/test.jpg")
    {
        var img = (ProductImage)Activator.CreateInstance(typeof(ProductImage), nonPublic: true)!;
        typeof(ProductImage).GetProperty(nameof(ProductImage.ImagePath))!.SetValue(img, path);
        return img;
    }

    public static CartItem CreateCartItem(int quantity = 1)
    {
        var buyer = CreateBuyer();
        var product = CreateProduct(quantity: quantity);
        return new CartItem(buyer, product, quantity);
    }
}
