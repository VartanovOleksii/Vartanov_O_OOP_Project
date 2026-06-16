using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace EShop.Domain.Tests;

public class CustomPasswordValidatorTests
{
    private class TestUser { }

    private readonly CustomPasswordValidator<TestUser> _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidPassword_ReturnsSuccess()
    {
        var result = await _validator.ValidateAsync(null!, new TestUser(), "SecurePass1");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_ShortPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new TestUser(), "Ab1");

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_AllDigitsPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new TestUser(), "12345678");

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_NullPassword_ReturnsFailure()
    {
        var result = await _validator.ValidateAsync(null!, new TestUser(), null);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ValidateAsync_ExactlyEightChars_ReturnsSuccess()
    {
        var result = await _validator.ValidateAsync(null!, new TestUser(), "Secure1!");

        Assert.True(result.Succeeded);
    }
}
