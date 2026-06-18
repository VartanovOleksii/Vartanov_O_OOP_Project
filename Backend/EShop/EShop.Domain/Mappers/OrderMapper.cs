using EShop.Domain.DTOs;
using EShop.Domain.Entities;

namespace EShop.Domain.Mappers;

public static class OrderMapper
{
    public static OrderExportDto ToDto(Order order)
    {
        return new OrderExportDto
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            SellerId = order.SellerId,
            ProductId = order.ProductId,
            Quantity = order.OrderQuantity,
            UnitPrice = order.OrderUnitPrice,
            TotalPrice = order.OrderTotalPrice,
            OrderDate = order.OrderDate
        };
    }

    public static Order ToEntity(OrderExportDto dto)
    {
        var order = (Order)Activator.CreateInstance(typeof(Order), nonPublic: true)!;

        typeof(Order).GetProperty(nameof(Order.Id))!.SetValue(order, dto.Id);
        typeof(Order).GetProperty(nameof(Order.BuyerId))!.SetValue(order, dto.BuyerId);
        typeof(Order).GetProperty(nameof(Order.SellerId))!.SetValue(order, dto.SellerId);
        typeof(Order).GetProperty(nameof(Order.ProductId))!.SetValue(order, dto.ProductId);
        typeof(Order).GetProperty(nameof(Order.OrderQuantity))!.SetValue(order, dto.Quantity);
        typeof(Order).GetProperty(nameof(Order.OrderUnitPrice))!.SetValue(order, dto.UnitPrice);
        typeof(Order).GetProperty(nameof(Order.OrderDate))!.SetValue(order, dto.OrderDate);

        return order;
    }
}
