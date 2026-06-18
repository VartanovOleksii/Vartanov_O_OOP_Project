using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Tests;

public class CustomPasswordValidatorTests
{
    private readonly CustomPasswordValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidPassword_ReturnsSuccess()
    {
        var result = await _validator.ValidateAsync(null!, new ApplicationUser(), "SecurePass1");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_ShortPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new ApplicationUser(), "Ab1");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_AllDigitsPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new ApplicationUser(), "12345678");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_NullPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new ApplicationUser(), null);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_ExactlyEightChars_ReturnsSuccess()
    {
        var result = await _validator.ValidateAsync(null!, new ApplicationUser(), "Secure1!");
        Assert.True(result.Succeeded);
    }
}
