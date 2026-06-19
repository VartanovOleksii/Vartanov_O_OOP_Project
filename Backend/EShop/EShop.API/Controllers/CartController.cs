using System.Security.Claims;
using EShop.API.Data;
using EShop.API.Requests;
using EShop.API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Buyer")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _db;

    public CartController(AppDbContext db) => _db = db;

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var buyer = await _db.Buyers
            .Include(b => b.Cart)
                .ThenInclude(ci => ci.CartProduct)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var items = buyer.Cart.Select(ci => new CartItemResponse
        {
            Id = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.CartProduct?.ProductName ?? string.Empty,
            Price = ci.CartProduct?.ProductPrice ?? 0m,
            CartQuantity = ci.CartQuantity
        }).ToList();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest req)
    {
        var buyer = await _db.Buyers
            .Include(b => b.Cart)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId);
        if (product is null)
            return NotFound(new { error = "Product not found." });

        try
        {
            buyer.AddToCart(product, req.Quantity);
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, new { message = "Added to cart." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var buyer = await _db.Buyers
            .Include(b => b.Cart)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var item = buyer.Cart.FirstOrDefault(ci => ci.Id == cartItemId);
        if (item is null)
            return NotFound(new { error = "Cart item not found." });

        buyer.Cart.Remove(item);
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
