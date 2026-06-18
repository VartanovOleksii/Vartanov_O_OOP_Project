using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Services;

public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(
        UserManager<ApplicationUser> manager,
        ApplicationUser user,
        string? password)
    {
        if (string.IsNullOrEmpty(password))
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError { Description = "Password is required." }));

        if (password.Length < 8)
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError { Description = "Password must be at least 8 characters." }));

        if (password.All(char.IsDigit))
            return Task.FromResult(IdentityResult.Failed(
                new IdentityError { Description = "Password cannot consist only of digits." }));

        return Task.FromResult(IdentityResult.Success);
    }
}
