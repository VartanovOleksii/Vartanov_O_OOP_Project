namespace EShop.API.Requests;

public class AddImageRequest
{
    public string ImagePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
}
