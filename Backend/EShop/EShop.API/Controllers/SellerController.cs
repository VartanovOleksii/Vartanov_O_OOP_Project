using System.Security.Claims;
using EShop.API.Data;
using EShop.API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/seller")]
[Authorize(Roles = "Seller")]
public class SellerController : ControllerBase
{
    private readonly AppDbContext _db;

    public SellerController(AppDbContext db) => _db = db;

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var seller = await _db.Sellers.FirstOrDefaultAsync(s => s.Id == DomainId);
        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        return Ok(new
        {
            id = seller.Id,
            userId = seller.UserId,
            userAddress = seller.UserAddress
        });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> Statistics()
    {
        var seller = await _db.Sellers
            .Include(s => s.SoldOrders)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        return Ok(new SellerStatsResponse { Statistics = seller.GetStatistics() });
    }
}
