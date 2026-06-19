using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EShop.API.Data;
using EShop.API.Requests;
using EShop.API.Responses;
using EShop.Domain.Entities;
using EShop.Domain.Models;
using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly Guest _guest;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(
        AppDbContext db,
        Guest guest,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        _db = db;
        _guest = guest;
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!Enum.TryParse<UserRole>(req.Role, ignoreCase: true, out var role))
            return BadRequest(new { error = $"Invalid role '{req.Role}'. Expected 'Buyer' or 'Seller'." });

        try
        {
            var domainUser = await _guest.Register(req.Username, req.Password, req.Address, role);

            if (domainUser is Buyer buyer)
                _db.Buyers.Add(buyer);
            else if (domainUser is Seller seller)
                _db.Sellers.Add(seller);

            await _db.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                domainId = domainUser.Id,
                role = role.ToString()
            });
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Unauthorized(new { error = "Invalid username or password." });

        var roles = await _userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? string.Empty;

        int domainId;
        if (roleName == "Buyer")
        {
            var buyer = await _db.Buyers.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (buyer is null)
                return Unauthorized(new { error = "Domain buyer not found." });
            domainId = buyer.Id;
        }
        else if (roleName == "Seller")
        {
            var seller = await _db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (seller is null)
                return Unauthorized(new { error = "Domain seller not found." });
            domainId = seller.Id;
        }
        else
        {
            return Unauthorized(new { error = "User has no assigned role." });
        }

        var token = GenerateToken(user.Id, roleName, domainId);

        return Ok(new LoginResponse
        {
            Token = token,
            Role = roleName,
            DomainId = domainId
        });
    }

    private string GenerateToken(string userId, string role, int domainId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim("role", role),
            new Claim("domainId", domainId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
