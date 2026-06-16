namespace EShop.Domain.Entities;

public abstract class AuthorizedUser : Entity
{
    public string UserId { get; protected set; } = string.Empty;

    public string UserAddress { get; protected set; } = string.Empty;

    public override bool IsValid()
    {
        throw new NotImplementedException();
    }
}