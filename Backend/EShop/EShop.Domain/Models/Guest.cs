using EShop.Domain.Entities;
using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Models;

public class Guest
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordValidator<ApplicationUser> _passwordValidator;

    public Guest(
        UserManager<ApplicationUser> userManager,
        IPasswordValidator<ApplicationUser> passwordValidator)
    {
        _userManager = userManager;
        _passwordValidator = passwordValidator;
    }

    public async Task<AuthorizedUser> Register(
        string login,
        string password,
        string address,
        UserRole role)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty.", nameof(login));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty.", nameof(address));

        var existing = await _userManager.FindByNameAsync(login);
        if (existing != null)
            throw new InvalidOperationException($"Login '{login}' is already taken.");

        var tempUser = new ApplicationUser { UserName = login };
        var passwordResult = await _passwordValidator.ValidateAsync(_userManager, tempUser, password);
        if (!passwordResult.Succeeded)
        {
            var errors = string.Join("; ", passwordResult.Errors.Select(e => e.Description));
            throw new ArgumentException($"Invalid password: {errors}", nameof(password));
        }

        var appUser = new ApplicationUser { UserName = login };
        var createResult = await _userManager.CreateAsync(appUser, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        var roleName = role == UserRole.Buyer ? "Buyer" : "Seller";
        await _userManager.AddToRoleAsync(appUser, roleName);

        AuthorizedUser domainUser = role == UserRole.Buyer
            ? new Buyer(appUser.Id, address)
            : new Seller(appUser.Id, address);

        return domainUser;
    }
}
