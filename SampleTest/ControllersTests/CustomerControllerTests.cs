using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sample.Controllers;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;

namespace SampleTest.ControllersTests;


[TestFixture]
public class CustomerControllerTests
{
    private Mock<ICustomerService> _mockCustomerService;
    private Mock<ILogger<CustomerController>> _mockLogger;
    private CustomerController _customerController;

    [SetUp]
    public void Setup()
    {
        _mockCustomerService = new Mock<ICustomerService>();
        _mockLogger = new Mock<ILogger<CustomerController>>();
        _customerController = new CustomerController(_mockCustomerService.Object, _mockLogger.Object);
    }

    [Test]
    public void PostCustomerDetails_ShouldReturnOk_WhenCustomerisAddedSuccessfully()
    {
        // Arrange 
        var customer = new CustomerInfoDTO();
        var response = "Customer Details Added";
        _mockCustomerService.Setup(x => x.AddCustomerPersonalDetails(customer)).Returns(response);

        // Act
        var result = _customerController.PostCustomerDetails(customer) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(result.Value, Is.EqualTo(response));
    }


    [Test]
    public void PostCustomerDetails_ShouldReturnInternalServerError_WhenCustomerisNotAdded()
    {
        // Arrange
        var customer = new CustomerInfoDTO();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.AddCustomerPersonalDetails(customer)).Throws(new Exception(response));

        // Act 
        var result = _customerController.PostCustomerDetails(customer) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.That(result.Value, Is.EqualTo(response));
    }

    [Test]
    public void GetCustomerDetails_ShouldReturnOk_WhenCustomerisFetched()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var response = new List<CustomerInfoDTO>();
        _mockCustomerService.Setup(x => x.GetCustomerDetails(customerId)).Returns(response);

        //Act
        var result = _customerController.GetCustomerDetails(customerId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(result.Value, Is.EqualTo(response));
    }

    [Test]
    public void GetCustomerDetails_ShouldReturnInternalServerError_WhenCustomerisNotFetched()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.GetCustomerDetails(customerId)).Throws(new Exception(response));

        // Act
        var result = _customerController.GetCustomerDetails(customerId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value, Is.EqualTo(response));
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    [Test]
    public void PostCustomerAccountDetails_ShouldReturnOk_WhenCustomerAccountDetailsisAddedSuccessfully()
    {
        // Arrange
        var customer = new CustomerAccountInfoDTO();
        var response = "Customer Account Details Added Successfully";
        _mockCustomerService.Setup(x => x.AddCustomerAccountDetails(customer)).Returns(response);

        // Act 
        var result = _customerController.PostCustomerAccountDetails(customer) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value, Is.EqualTo(response));
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public void PostCustomerAccountDetails_ShouldReturnInternalServerError_WhenCustomerAccountDetailsisNotAdded()
    {
        // Arrange
        var customer = new CustomerAccountInfoDTO();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.AddCustomerAccountDetails(customer)).Throws(new Exception(response));

        // Act
        var result = _customerController.PostCustomerAccountDetails(customer) as ObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(response));
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    [Test]
    public void GetCustomerAccountDetails_ShouldReturnOk_WhenCustomerisFetched()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var response = new List<CustomerAccountInfo>();
        _mockCustomerService.Setup(x => x.GetCustomerAccountDetails(customerId)).Returns(response);

        //Act
        var result = _customerController.GetCustomerAccountDetails(customerId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(result.Value, Is.EqualTo(response));
    }

    
    [Test]
    public void GetCustomerAccountDetails_ShouldReturnInternalServerError_WhenCustomerisNotFetched()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.GetCustomerAccountDetails(customerId)).Throws(new Exception(response));

        // Act
        var result = _customerController.GetCustomerAccountDetails(customerId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value, Is.EqualTo(response));
        Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    [Test]
    public void PutCustomerDetails_ShouldReturnOk_WhenCustomerisUpdatedSuccessfully()
    {
        // Arrange
        var customer  = new CustomerInfoDTO();
        var response = new CustomerInfo();
        _mockCustomerService.Setup(x => x.UpdateCustomerDetails(customer)).Returns(response);

        // Act
        var result = _customerController.PutCustomerDetails(customer) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value , Is.EqualTo(response));
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.OK));
    }

  [Test]
    public void PutCustomerDetails_ShouldReturnInternalServerError_WhenCustomerisNotUpdated()
    {
        // Arrange
        var customer  = new CustomerInfoDTO();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.UpdateCustomerDetails(customer)).Throws(new Exception(response));

        // Act
        var result = _customerController.PutCustomerDetails(customer) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value , Is.EqualTo(response));
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }
[Test]
    public void PutCustomerAccountDetails_ShouldReturnOk_WhenCustomerisUpdatedSuccessfully()
    {
        // Arrange
        var customer  = new CustomerAccountUpdateInfoDTO();
        var response = new CustomerAccountInfo();
        _mockCustomerService.Setup(x => x.UpdateCustomerAccountDetails(customer)).Returns(response);

        // Act
        var result = _customerController.PutCustomerAccountDetails(customer) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value , Is.EqualTo(response));
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.OK));
    }

  [Test]
    public void PutCustomerAccountDetails_ShouldReturnInternalServerError_WhenCustomerisNotUpdated()
    {
        // Arrange
        var customer  = new CustomerAccountUpdateInfoDTO();
        var response = "Internal Server Error";
        _mockCustomerService.Setup(x => x.UpdateCustomerAccountDetails(customer)).Throws(new Exception(response));

        // Act
        var result = _customerController.PutCustomerAccountDetails(customer) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Value , Is.EqualTo(response));
        Assert.That(result.StatusCode , Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }
}