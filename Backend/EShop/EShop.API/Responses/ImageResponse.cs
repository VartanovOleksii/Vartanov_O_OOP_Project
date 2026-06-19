namespace EShop.API.Responses;

public class ImageResponse
{
    public int Id { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
}
