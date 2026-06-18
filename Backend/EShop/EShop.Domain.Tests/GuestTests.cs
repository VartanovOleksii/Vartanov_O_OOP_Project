using EShop.Domain.Entities;
using EShop.Domain.Models;
using EShop.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace EShop.Domain.Tests;

public class GuestTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IPasswordValidator<ApplicationUser>> _validatorMock;
    private readonly Guest _guest;

    public GuestTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _validatorMock = new Mock<IPasswordValidator<ApplicationUser>>();

        _guest = new Guest(_userManagerMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Register_EmptyLogin_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _guest.Register("", "Pass123!", "Address", UserRole.Buyer));
    }

    [Fact]
    public async Task Register_EmptyAddress_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _guest.Register("user1", "Pass123!", "", UserRole.Buyer));
    }

    [Fact]
    public async Task Register_DuplicateLogin_ThrowsInvalidOperationException()
    {
        _userManagerMock
            .Setup(m => m.FindByNameAsync("existing"))
            .ReturnsAsync(new ApplicationUser { UserName = "existing" });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _guest.Register("existing", "Pass123!", "Addr", UserRole.Buyer));
    }

    [Fact]
    public async Task Register_InvalidPassword_ThrowsArgumentException()
    {
        _userManagerMock
            .Setup(m => m.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UserManager<ApplicationUser>>(),
                                        It.IsAny<ApplicationUser>(),
                                        It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Password too short." }));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _guest.Register("newuser", "123", "Addr", UserRole.Buyer));
    }

    [Fact]
    public async Task Register_ValidBuyer_ReturnsBuyerInstance()
    {
        SetupSuccessfulRegistration("user1", "user-id-1");

        var result = await _guest.Register("user1", "Pass123!", "Test St 1", UserRole.Buyer);

        Assert.IsType<Buyer>(result);
        Assert.Equal("user-id-1", result.UserId);
        Assert.Equal("Test St 1", result.UserAddress);
    }

    [Fact]
    public async Task Register_ValidSeller_ReturnsSellerInstance()
    {
        SetupSuccessfulRegistration("seller1", "seller-id-1");

        var result = await _guest.Register("seller1", "Pass123!", "Market St 5", UserRole.Seller);

        Assert.IsType<Seller>(result);
        Assert.Equal("Market St 5", result.UserAddress);
    }

    private void SetupSuccessfulRegistration(string userName, string userId)
    {
        _userManagerMock
            .Setup(m => m.FindByNameAsync(userName))
            .ReturnsAsync((ApplicationUser?)null);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UserManager<ApplicationUser>>(),
                                        It.IsAny<ApplicationUser>(),
                                        It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((u, _) => u.Id = userId)
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
    }
}
