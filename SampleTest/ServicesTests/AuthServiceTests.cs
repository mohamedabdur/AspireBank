using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;
using Sample.Repositories;
using Sample.Services;

namespace SampleTest.ServicesTests;


[TestFixture]
public class AuthServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IRepositories> _mockRepositories;
    private Mock<IMapper> _mockMapper;
    private AuthService _mockAuthService;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockRepositories = new Mock<IRepositories>();
        _mockMapper = new Mock<IMapper>();
        _mockConfiguration.Setup(config => config["Appsettings:Token"]).Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImFzaGlxaWxhaGkiLCJuYmYiOjE3MjAzMzYxOTMsImV4cCI6MTcyMDMzNjI1MywiaWF0IjoxNzIwMzM2MTkzfQ.IRYBkxggYwo6NwyAbrX_m4gqZiiLQd3OgqyUeFsRRos");
        _mockAuthService = new AuthService(_mockConfiguration.Object, _mockRepositories.Object, _mockMapper.Object);
    }

    [Test]
    public void PostCustomer_WithValidCustomer_ReturnsTrue()
    {
        // Arrange 
        var customers = new Customers();
        var customer = new CustomersRegisterDTO()
        {
            UserName = "testUser",
            Password = "testPassword",
            ConfirmPassword = "testPassword",
            Name = "testName",
            PhoneNumber = "9876543211"
        };
        _mockMapper.Setup(x => x.Map<Customers>(customer)).Returns(customers);
        _mockRepositories.Setup(x => x.AddCustomers(customers)).Returns(true);

        // Act
        var result = _mockAuthService.PostCustomer(customer);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void PostCustomer_WithInValidCustomer_ThrowsValidationException()
    {
        // Arrange
        var customers = new Customers();
        var customer = new CustomersRegisterDTO()
        {
            UserName = "testUsername",
            Password = "testPassword",
            ConfirmPassword = "tstPassword",
            Name = "testName",
            PhoneNumber = "9876543211"
        };

        var validationException = Assert.Throws<ValidationException>(() => _mockAuthService.PostCustomer(customer));
        Assert.That(validationException.Message, Is.EqualTo("Password does not match"));
    }

    [Test]
    public void PostCustomer_WithNullCustomer_ThrowsArgumentNullException()
    {
        // Arrange 
        CustomersRegisterDTO customersRegisterDTO = null;
        // Act & Assert
        var result =  Assert.Throws<ArgumentNullException>(() => _mockAuthService.PostCustomer(customersRegisterDTO));
        Assert.That(result.ParamName , Is.EqualTo("Customer cannot be null"));
    }

    [Test]
    public void ExistingCustomer_WithValidCredentials_ReturnsCustomerId()
    {
        // Arrange
        var customerDto = new CustomerLoginDTO
        {
            UserName = "testUser",
            Password = "testPassword"
        };
        var response = "Customer Id";

        _mockRepositories.Setup(x => x.CheckUser(customerDto)).Returns(response);

        // Act
        var result = _mockAuthService.ExistingCustomer(customerDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void ExistingCustomer_WithInValidCredentials_ReturnsInvalidCredentials()
    {
        var customerDto = new CustomerLoginDTO()
        {
            UserName = "testUser",
            Password = "wrongPassword"
        };
        var response = "Invalid username or password";

        _mockRepositories.Setup(x => x.CheckUser(customerDto)).Returns(response);

        // Act
        var result = _mockAuthService.ExistingCustomer(customerDto);

        // Assert
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void GenerateTokens_ReturnsAuthTokens()
    {
        // Arrange
        var customerDto = new CustomerLoginDTO
        {
            UserName = "testUsername",
            Password = "testPassword"
        };

        // Act
        var result = _mockAuthService.GenerateTokens(customerDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.AccessToken);
        Assert.IsNotEmpty(result.RefreshToken);
    }

    [Test]
    public void ValidateRefreshToken_WithValidToken_ReturnsNewAccessToken()
    {
        // Arrange 
        var userName = "testUsername";
        var refreshToken = "testRefreshToken";
        _mockRepositories.Setup(x => x.ValidateRefreshToken(userName, refreshToken)).Returns(true);


        // Act
        var result = _mockAuthService.ValidateRefreshToken(userName, refreshToken);

        // Assert
        Assert.That(result, Is.Not.EqualTo("Token Generation Failed"));
    }

    [Test]
    public void ValidateRefreshToken_WithInvalidToken_ReturnsInvalidCredentialsMessage()
    {
        // Arrange
        var userName = "testUser";
        var refreshToken = "invalidRefreshToken";

        _mockRepositories.Setup(r => r.ValidateRefreshToken(userName, refreshToken)).Returns(false);

        // Act
        var result = _mockAuthService.ValidateRefreshToken(userName, refreshToken);

        // Assert
        Assert.That(result , Is.EqualTo("Not a Valid Credentials"));
    }

}