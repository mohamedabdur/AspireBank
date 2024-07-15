using System.Net;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sample.Controllers;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;

namespace SampleTest.ControllersTests;


[TestFixture]
public class LoginControllerTests
{
    private Mock<IAuthService> _mockAuthService;
    private LoginController _loginController;
    private Mock<ILogger<LoginController>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<LoginController>>();
        _loginController = new LoginController(_mockAuthService.Object, _mockLogger.Object);

    }

    [Test]
    public void CustomerRegister_ShouldReturnOk_WhenRegistrationSuccessful()
    {
        // Arrange
        var customer = new CustomersRegisterDTO();
        _mockAuthService.Setup(x => x.PostCustomer(customer)).Returns(true);

        // Act
        var result = _loginController.Register(customer) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(result.Value, Is.EqualTo("Customer Registered successfully"));
    }

    [Test]
    public void CustomerRegister_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var customer = new CustomersRegisterDTO();
        _mockAuthService.Setup(x => x.PostCustomer(customer)).Returns(false);

        // Act
        var result = _loginController.Register(customer) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(result.Value, Is.EqualTo("Customer registration failed"));
    }

    [Test]
    public void CustomerLogin_ShouldReturnOk_WhenLoginSuccessful()
    {
        // Arrange
        var customer = new CustomerLoginDTO();
        var loginResponse = new CustomerLoginResponseDTO()
        {
            AuthTokens = new AuthTokens
            {
                AccessToken = "some auth Token",
                RefreshToken = "some Refresh Token"
            },
            CustomerId = "some CustomerId",
            Message = "Successfully logged in"
        };
        _mockAuthService.Setup(x => x.ExistingCustomer(customer)).Returns(loginResponse.CustomerId);
        _mockAuthService.Setup(x => x.GenerateTokens(customer)).Returns(loginResponse.AuthTokens);

        // Act
        var result = _loginController.Login(customer) as OkObjectResult;
        var returnValue = result?.Value as CustomerLoginResponseDTO;

        // Assert 
        Assert.NotNull(result);
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(loginResponse.CustomerId, Is.EqualTo(returnValue?.CustomerId));
        Assert.That(returnValue?.AuthTokens, Is.EqualTo(loginResponse.AuthTokens));
        Assert.That(returnValue.Message, Is.EqualTo(loginResponse.Message));
    }

    [Test]
    public void CustomerLogin_ShouldReturnBadRequest_WhenLoginFailed()
    {
        // Arrange
        var customer = new CustomerLoginDTO();
        var response = "Invalid username or password";
        _mockAuthService.Setup(x => x.ExistingCustomer(customer)).Returns(response);

        // Act 
        var result = _loginController.Login(customer) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(result.Value, Is.EqualTo(response));
    }

    [Test]
    public void RefreshToken_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        var userName = "some username";
        var refreshToken = "some refresh token";
        var newToken = "some new token";
        _mockAuthService.Setup(x => x.ValidateRefreshToken(userName, refreshToken)).Returns(newToken);

        // Act
        var result = _loginController.RefreshTokens(userName, refreshToken) as OkObjectResult;

        // Assert 
        Assert.NotNull(result);
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(result.Value, Is.EqualTo(newToken));
    }

    [Test]
    public void RefreshToken_ShouldReturnBadRequest_WhenTokenisInvalid()
    {
        // Arrange
        var userName = "some username";
        var refreshToken = "some refresh token";
        _mockAuthService.Setup(x => x.ValidateRefreshToken(userName, refreshToken)).Returns((string)null);

        // Act
        var result = _loginController.RefreshTokens(userName, refreshToken) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(result.Value, Is.EqualTo("Token Generation failed"));
    }
}