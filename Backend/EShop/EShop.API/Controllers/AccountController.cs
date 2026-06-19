using System.Security.Claims;
using EShop.API.Data;
using EShop.API.Requests;
using EShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/account")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db) => _db = db;

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);
    private string Role => User.FindFirstValue("role") ?? string.Empty;

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _db.Set<AuthorizedUser>().FirstOrDefaultAsync(u => u.Id == DomainId);
        if (user is null)
            return NotFound(new { error = "User not found." });

        return Ok(new { id = user.Id, role = Role, userAddress = user.UserAddress });
    }

    [HttpPut("address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.NewAddress))
            return BadRequest(new { error = "Address cannot be empty." });

        var user = await _db.Set<AuthorizedUser>().FirstOrDefaultAsync(u => u.Id == DomainId);
        if (user is null)
            return NotFound(new { error = "User not found." });

        // UserAddress has a protected setter; update it through EF Core so we
        // don't have to modify the EShop.Domain entity.
        _db.Entry(user).Property(nameof(AuthorizedUser.UserAddress)).CurrentValue = req.NewAddress.Trim();
        await _db.SaveChangesAsync();

        return Ok(new { id = user.Id, userAddress = user.UserAddress });
    }
}
