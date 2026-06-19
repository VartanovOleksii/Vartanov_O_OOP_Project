using System.Security.Claims;
using EShop.API.Data;
using EShop.Domain.DTOs;
using EShop.Domain.Interfaces;
using EShop.Domain.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Controllers;

[ApiController]
[Route("api/json")]
public class JsonDataController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJsonDataPort<ProductExportDto> _productPort;
    private readonly IJsonDataPort<OrderExportDto> _orderPort;

    public JsonDataController(
        AppDbContext db,
        IJsonDataPort<ProductExportDto> productPort,
        IJsonDataPort<OrderExportDto> orderPort)
    {
        _db = db;
        _productPort = productPort;
        _orderPort = orderPort;
    }

    private int DomainId => int.Parse(User.FindFirstValue("domainId")!);

    private async Task<string> ReadBodyAsync()
    {
        using var reader = new StreamReader(Request.Body);
        return await reader.ReadToEndAsync();
    }

    [HttpGet("products")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> ExportProducts()
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        var dtos = seller.Products.Select(ProductMapper.ToDto);
        var json = _productPort.Export(dtos);
        return Content(json, "application/json");
    }

    [HttpPost("products")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> ImportProducts()
    {
        var seller = await _db.Sellers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == DomainId);

        if (seller is null)
            return NotFound(new { error = "Seller not found." });

        var json = await ReadBodyAsync();
        var dtos = _productPort.Import(json);

        int count = 0;
        foreach (var dto in dtos)
        {
            var product = ProductMapper.ToEntity(dto);
            seller.Products.Add(product);
            count++;
        }

        await _db.SaveChangesAsync();
        return Ok(new { imported = count });
    }

    [HttpGet("orders")]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> ExportOrders()
    {
        var buyer = await _db.Buyers
            .Include(b => b.OrderHistory)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var dtos = buyer.OrderHistory.Select(OrderMapper.ToDto);
        var json = _orderPort.Export(dtos);
        return Content(json, "application/json");
    }

    [HttpPost("orders")]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> ImportOrders()
    {
        var buyer = await _db.Buyers
            .Include(b => b.OrderHistory)
            .FirstOrDefaultAsync(b => b.Id == DomainId);

        if (buyer is null)
            return NotFound(new { error = "Buyer not found." });

        var json = await ReadBodyAsync();
        var dtos = _orderPort.Import(json);

        int count = 0;
        foreach (var dto in dtos)
        {
            var order = OrderMapper.ToEntity(dto);
            buyer.OrderHistory.Add(order);
            count++;
        }

        await _db.SaveChangesAsync();
        return Ok(new { imported = count });
    }
}
