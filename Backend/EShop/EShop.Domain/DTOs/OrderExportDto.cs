namespace EShop.Domain.DTOs;

public class OrderExportDto
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
}
