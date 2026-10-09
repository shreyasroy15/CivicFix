using CivicFix.Application.DTOs.Auth;
using CivicFix.Domain.Entities;
using CivicFix.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace CivicFix.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        var roleStoreMock = new Mock<IRoleStore<Role>>();
        _roleManagerMock = new Mock<RoleManager<Role>>(roleStoreMock.Object, null, null, null, null);

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(x => x["JWT:Secret"]).Returns("SuperSecretKeyWhichIsAtLeast32BytesLong!!");
        _configMock.Setup(x => x["JWT:TokenValidityInMinutes"]).Returns("60");
        _configMock.Setup(x => x["JWT:ValidIssuer"]).Returns("Issuer");
        _configMock.Setup(x => x["JWT:ValidAudience"]).Returns("Audience");
        _configMock.Setup(x => x["JWT:RefreshTokenValidityInDays"]).Returns("7");

        _authService = new AuthService(_userManagerMock.Object, _roleManagerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ExistingUser_ThrowsException()
    {
        // Arrange
        var request = new RegisterRequestDto { Email = "test@test.com", Password = "Password123!", Username = "testuser" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(new User());

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User already exists!");
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@test.com", Password = "wrongpassword" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid credentials");
    }
}
