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
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);

    private static ProductResponse ToResponse(Product p) => new()
    {
        Id = p.Id,
        Name = p.ProductName,
        Description = p.ProductDescription,
        Price = p.ProductPrice,
        Quantity = p.ProductQuantity,
        SellerId = p.SellerId,
        Images = p.ProductImages.Select(i => new ImageResponse
        {
            Id = i.Id,
            ImagePath = i.ImagePath,
            AltText = i.ImageAltText
        }).ToList()
    };

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var products = await _db.Products
            .Include(p => p.ProductImages)
            .ToListAsync();

        return Ok(products.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _db.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound();

        return Ok(ToResponse(product));
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest req)
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        try
        {
            seller.AddProduct(req.Name, req.Description, req.Price, req.Quantity);
            await _db.SaveChangesAsync();

            var created = seller.Products[^1];
            return StatusCode(StatusCodes.Status201Created, ToResponse(created));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> Delete(int id)
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        try
        {
            seller.RemoveProduct(id);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("{id:int}/images")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> AddImage(int id, [FromBody] AddImageRequest req)
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
                .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        var product = seller.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return NotFound(new { error = "Product not found or does not belong to this seller." });

        try
        {
            product.AddImage(new ProductImage(product.Id, req.ImagePath, req.AltText));
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, ToResponse(product));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}/stock")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest req)
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        try
        {
            seller.UpdateStock(id, req.NewQuantity);
            await _db.SaveChangesAsync();
            return NoContent();
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
