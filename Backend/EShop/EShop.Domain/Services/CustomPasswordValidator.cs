using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Services;

public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
{
    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
    {
        throw new NotImplementedException();
    }
}
