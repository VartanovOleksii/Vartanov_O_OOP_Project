using EShop.Domain.DTOs;
using EShop.Domain.Entities;

namespace EShop.Domain.Mappers;

public static class OrderMapper
{
    public static OrderDto ToDto(Order order)
    {
        OrderDto dto = new OrderDto();

        dto.Id = order.Id;
        dto.BuyerId = order.BuyerId;
        dto.SellerId = order.SellerId;
        dto.ProductId = order.ProductId;
        dto.Quantity = order.OrderQuantity;
        dto.UnitPrice = order.OrderUnitPrice;
        dto.TotalPrice = order.OrderQuantity * order.OrderUnitPrice;
        dto.OrderDate = order.OrderDate;

        return dto;
    }

    public static Order ToEntity(OrderDto dto)
    {
        var order = (Order)Activator.CreateInstance(typeof(Order), nonPublic: true)!;

        typeof(Order).GetProperty(nameof(Order.Id))!
            .SetValue(order, dto.Id);
        typeof(Order).GetProperty(nameof(Order.BuyerId))!
            .SetValue(order, dto.BuyerId);
        typeof(Order).GetProperty(nameof(Order.SellerId))!
            .SetValue(order, dto.SellerId);
        typeof(Order).GetProperty(nameof(Order.ProductId))!
            .SetValue(order, dto.ProductId);
        typeof(Order).GetProperty(nameof(Order.OrderQuantity))!
            .SetValue(order, dto.Quantity);
        typeof(Order).GetProperty(nameof(Order.OrderUnitPrice))!
            .SetValue(order, dto.UnitPrice);
        typeof(Order).GetProperty(nameof(Order.OrderDate))!
            .SetValue(order, dto.OrderDate);

        return order;
    }
}
