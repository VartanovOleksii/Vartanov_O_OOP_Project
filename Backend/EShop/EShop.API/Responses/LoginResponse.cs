namespace EShop.API.Responses;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int DomainId { get; set; }
}
