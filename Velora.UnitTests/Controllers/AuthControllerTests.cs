using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Velora.Web.Controllers;
using Velora.Application.Auth.Commands;
using FluentAssertions;
using Xunit;

namespace Velora.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new AuthController(_senderMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationSucceeded()
    {
        // Arrange
        var command = new RegisterUserCommand("test@test.com", "Password123!", "Test", "User");
        _senderMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, "user-id-123"));

        // Act
        var result = await _controller.Register(command);

        // Assert
        var okResult = result.As<OkObjectResult>();
        okResult.StatusCode.Should().Be(200);
        
        var value = okResult.Value.ToString();
        value.Should().Contain("user-id-123");
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginSucceeded()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "Password123!");
        _senderMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, "fake-jwt-token"));

        // Act
        var result = await _controller.Login(command);

        // Assert
        var okResult = result.As<OkObjectResult>();
        okResult.StatusCode.Should().Be(200);
        
        var value = okResult.Value.ToString();
        value.Should().Contain("fake-jwt-token");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenLoginFailed()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "WrongPassword");
        _senderMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, "Geçersiz kullanıcı adı veya şifre."));

        // Act
        var result = await _controller.Login(command);

        // Assert
        var unauthorizedResult = result.As<UnauthorizedObjectResult>();
        unauthorizedResult.StatusCode.Should().Be(401);
    }
}
