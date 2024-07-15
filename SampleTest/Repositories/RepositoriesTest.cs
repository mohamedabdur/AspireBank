using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sample.Data;
using Sample.DTOs;
using Sample.Models;
using Sample.Repositories;

namespace SampleTest.RepositoriesTests;

[TestFixture]
public class RepositoriesTests
{
    private ApplicationDbContext _dbContext;
    private Mock<IMapper> _mapperMock;
    private Repositories _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDatabase")
           .Options;
        _dbContext = new ApplicationDbContext(options);

        _mapperMock = new Mock<IMapper>();
        _repository = new Repositories(_dbContext, _mapperMock.Object);
    }

    // Clean up the resourse after each Test.
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted(); // Deletes in-memory DB after each Test.
        _dbContext.Dispose(); // Disposes ApplicationDbContext instance.
    }

    [Test]
    public void AddCustomers_CustomerDoesNotExist_AddsCustomer()
    {
        // Arrange
        var customer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "newUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };

        // Act
        var result = _repository.AddCustomers(customer);

        // Assert
        Assert.IsTrue(result);
        Assert.That(_dbContext.Customers.Count(), Is.EqualTo(1));
        Assert.That(_dbContext.Customers.First().UserName, Is.EqualTo("newUser"));
    }

    [Test]
    public void AddCustomers_CustomerExists_ThrowsException()
    {
        // Arrange
        var existingCustomer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "ExistingUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };
        _dbContext.Customers.Add(existingCustomer);
        _dbContext.SaveChanges();

        var customer = new Customers
        {
            CustomerId = Guid.NewGuid(),
            UserName = "ExistingUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };

        // Act & Assert
        var result = Assert.Throws<Exception>(() => _repository.AddCustomers(customer));
        Assert.That(result.Message, Is.EqualTo("username already exists"));
        Assert.That(_dbContext.Customers.Count(), Is.EqualTo(1));
    }

    [Test]
    public void CheckUser_ValidCredentials_ReturnsCustomerId()
    {
        // Arrange 
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("testPassword");
        var existingCustomer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "existingUsername",
            Password = passwordHash,
            Name = "testName",
            PhoneNumber = "9952100123"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var newCustomer = new CustomerLoginDTO()
        {
            UserName = "existingUsername",
            Password = "testPassword"
        };

        // Act
        var result = _repository.CheckUser(newCustomer);

        // Arrange 
        Assert.That(result, Is.EqualTo(existingCustomer.CustomerId.ToString()));
    }

    [Test]
    public void CheckUser_InValidUsername_ReturnsInvalidMessage()
    {
        // Arrange 
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("testPassword");
        var existingCustomer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "existingUsername",
            Password = passwordHash,
            Name = "testName",
            PhoneNumber = "9952100123"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var newCustomer = new CustomerLoginDTO()
        {
            UserName = "wrongUsername",
            Password = "testPassword"
        };

        var response = "Invalid username or password";

        // Act
        var result = _repository.CheckUser(newCustomer);

        // Arrange 
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void CheckUser_InValidPassword_ReturnsInvalidMessage()
    {
        // Arrange 
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("testPassword");
        var existingCustomer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "existingUsername",
            Password = passwordHash,
            Name = "testName",
            PhoneNumber = "9952100123"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var newCustomer = new CustomerLoginDTO()
        {
            UserName = "existingUsername",
            Password = "WrongtestPassword"
        };

        var response = "Invalid username or password";

        // Act
        var result = _repository.CheckUser(newCustomer);

        // Arrange 
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void CheckUser_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange 
        CustomerLoginDTO customer = null;
        var response = "Value cannot be null";
        // Act & Assert
        var result = Assert.Throws<ArgumentNullException>(() => _repository.CheckUser(customer));
        Assert.That(result.ParamName, Is.EqualTo(response));
    }

    [Test]
    public void AddRefreshToken_ValidCustomer_AddsRefreshToken()
    {
        // Arrange
        var customer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "testUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();

        var customerLoginDto = new CustomerLoginDTO
        {
            UserName = "testUser",
            Password = "testPassword"
        };
        var randomNumber = new byte[32];
        new Random().NextBytes(randomNumber);

        // Act
        var result = _repository.AddRefreshToken(customerLoginDto, randomNumber);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(_dbContext.RefreshTokens.Count(), Is.EqualTo(1));
        Assert.That(_dbContext.RefreshTokens.First().UserName, Is.EqualTo(customer.UserName));
        Assert.That(_dbContext.RefreshTokens.First().CustomerId, Is.EqualTo(customer.CustomerId));
    }

    [Test]
    public void AddRefreshToken_CustomerNotFound_ThrowsException()
    {
        // Arrange
        var customerLoginDto = new CustomerLoginDTO
        {
            UserName = "nonExistingtUser",
            Password = "testPassword"
        };
        var randomNumber = new byte[32];
        new Random().NextBytes(randomNumber);
        var response = "Customer not found";

        // Act & Assert
        var result = Assert.Throws<Exception>(() => _repository.AddRefreshToken(customerLoginDto, randomNumber));
        Assert.That(result.Message, Is.EqualTo(response));
    }

    [Test]
    public void ValidateRefreshToken_ValidToken_ReturnsTrue()
    {
        // Arrange
        var customer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "testUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();

        var refreshToken = new AuthRefreshToken
        {
            CustomerId = customer.CustomerId,
            UserName = customer.UserName,
            RefreshToken = "validToken"
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.ValidateRefreshToken("testUser", "validToken");

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateRefreshToken_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var customer = new Customers()
        {
            CustomerId = Guid.NewGuid(),
            UserName = "testUser",
            Password = "testPassword",
            Name = "testName",
            PhoneNumber = "9952199189"
        };
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();

        var refreshToken = new AuthRefreshToken
        {
            CustomerId = customer.CustomerId,
            UserName = customer.UserName,
            RefreshToken = "validToken"
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.ValidateRefreshToken("testUser", "invalidToken");

        // Assert
        Assert.IsFalse(result);
    }
}
