using System.Security.Claims;
using EShop.API.Data;
using EShop.API.Requests;
using EShop.API.Responses;
using EShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(Roles = "Buyer")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db) => _db = db;

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);

    private static OrderResponse ToResponse(Order o) => new()
    {
        Id = o.Id,
        ProductId = o.ProductId,
        ProductName = o.OrderProduct?.ProductName ?? string.Empty,
        SellerId = o.SellerId,
        Quantity = o.OrderQuantity,
        UnitPrice = o.OrderUnitPrice,
        TotalPrice = o.OrderTotalPrice,
        OrderDate = o.OrderDate
    };

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var buyer = await _db.Buyers
            .Include(b => b.OrderHistory)
                .ThenInclude(o => o.OrderProduct)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        return Ok(buyer.OrderHistory.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var buyer = await _db.Buyers
            .Include(b => b.OrderHistory)
                .ThenInclude(o => o.OrderProduct)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var order = buyer.OrderHistory.FirstOrDefault(o => o.Id == id);
        if (order is null)
            return NotFound(new { error = "Order not found." });

        return Ok(ToResponse(order));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        var buyer = await _db.Buyers
            .Include(b => b.OrderHistory)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var product = await _db.Products
            .Include(p => p.ProductSeller)
                .ThenInclude(s => s!.SoldOrders)
            .FirstOrDefaultAsync(p => p.Id == req.ProductId);

        if (product is null)
            return NotFound(new { error = "Product not found." });

        try
        {
            var order = Order.Create(buyer, product, req.Quantity);
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, ToResponse(order));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
