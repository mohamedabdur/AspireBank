using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;
using Sample.Services;

namespace SampleTest.ServicesTests;

[TestFixture]
public class CustomerServiceTest
{
    private Mock<ICustomerRepositories> _mockCustomerRepositories;
    private Mock<IMapper> _mockMapper;
    private CustomerService _customerService;
    [SetUp]
    public void SetUp()
    {
        _mockCustomerRepositories = new Mock<ICustomerRepositories>();
        _mockMapper = new Mock<IMapper>();
        _customerService = new CustomerService(_mockCustomerRepositories.Object, _mockMapper.Object);
    }

    [Test]
    public void AddCustomerPersonalDetails_ValidCustomerDetails_ReturnsResponse()
    {
        // Arrange
        var customerInfo = new CustomerInfo();
        var customerInfoDTO = new CustomerInfoDTO()
        {
            CustomerId = Guid.NewGuid(),
            Name = "testName",
            FatherName = "testFatherName",
            Gender = "testGender",
            Nationality = "testNationality",
            DateOfBirth = DateTime.Today,
            Address = "testAddress",
            PlaceOfBirth = "testPOB",
            PhoneNumber = "9952199189",
            EmailAddress = "Email@gmail.com",
        };
        var response = "Customer Details Added";
        _mockMapper.Setup(x => x.Map<CustomerInfo>(customerInfoDTO)).Returns(customerInfo);
        _mockCustomerRepositories.Setup(x => x.AddCustomerDetails(customerInfo)).Returns(response);

        // Act
        var result = _customerService.AddCustomerPersonalDetails(customerInfoDTO);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void AddCustomerPersonalDetails_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        CustomerInfoDTO customerDto = null;

        // Act & Assert
        var result = Assert.Throws<ArgumentNullException>(() => _customerService.AddCustomerPersonalDetails(customerDto));
        Assert.That(result.ParamName, Is.EqualTo("Customer cannot be null"));
    }

    [Test]
    public void AddCustomerPersonalDetails_InvalidCustomer_ThrowsValidationException()
    {
        // Arrange
        var customerDto = new CustomerInfoDTO
        {
            // Invalid data
            CustomerId = Guid.NewGuid(),
            Name = "testName",
            FatherName = "testFatherName",
            Gender = "testGender",
            Nationality = "testNationality",
            DateOfBirth = DateTime.Now,
            Address = "testAddress",
            PlaceOfBirth = "testPOB",
            PhoneNumber = "9952199189",
            EmailAddress = "Email@gmail.com",
        };

        // Act & Assert
        var result = Assert.Throws<ValidationException>(() => _customerService.AddCustomerPersonalDetails(customerDto));
        Assert.That(result.Message, Is.EqualTo("Date of Birth cannot be in the future"));
    }

    [Test]
    public void GetCustomerDetails_ValidCustomerId_ReturnsListofCustomerDetails()
    {
        var customerId = Guid.NewGuid();
        var response = new List<CustomerInfoDTO>();
        _mockCustomerRepositories.Setup(x => x.GetCustomerPersonalDetails(customerId)).Returns(response);

        // Act
        var result = _customerService.GetCustomerDetails(customerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(response));
    }

    [Test]
    public void GetCustomerDetails_InvalidCustomerId_ThrowsException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockCustomerRepositories.Setup(r => r.GetCustomerPersonalDetails(customerId)).Throws(new Exception("Customer not found"));

        // Act & Assert
        Assert.Throws<Exception>(() => _customerService.GetCustomerDetails(customerId));
    }


    [Test]
    public void AddCustomerAccountDetails_ValidCustomer_ReturnsResponse()
    {
        // Arrange
        var customerAccountInfo = new CustomerAccountInfo();
        var response ="Customer Account Details Added Successfully";
        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var customerDto = new CustomerAccountInfoDTO
        {
            CustomerId = Guid.NewGuid(),
            GovernmentId = "123456789",
            IdType = "Passport",
            IdProof = file,
            AccountType = "Savings",
            BranchName = "Main Branch",
            AgreedToPrivacyPolicy = true,
            AgreedToTermsAndConditions = true,
            EmployementStatus = "Employed",
            OrganisationName = "Company Inc.",
            Occupation = "Developer",
            AnnualIncome = "50000"
        };

        using (var memoryStream = new MemoryStream())
        {
            customerDto.IdProof?.CopyTo(memoryStream);
            var customerAccounts = new CustomerAccountInfo
            {
                CustomerId = customerDto.CustomerId,
                GovernmentId = customerDto.GovernmentId,
                IdType = customerDto.IdType,
                IdProof = memoryStream.ToArray(),
                AccountType = customerDto.AccountType,
                BranchName = customerDto.BranchName,
                AgreedToPrivacyPolicy = customerDto.AgreedToPrivacyPolicy,
                AgreedToTermsAndConditions = customerDto.AgreedToTermsAndConditions,
                EmployementStatus = customerDto.EmployementStatus,
                OrganisationName = customerDto.OrganisationName,
                Occupation = customerDto.Occupation,
                AnnualIncome = customerDto.AnnualIncome
            };

        _mockCustomerRepositories.Setup(r => r.PostAccountDetails(customerAccounts)).Returns(response);

        }
        // Act
        var result = _customerService.AddCustomerAccountDetails(customerDto);
        // Assert
        Assert.That(result , Is.Not.Null);
        Assert.That(result, Is.EqualTo(response));
    }


    [Test]
    public void UpdateCustomerDetails_ValidCustomer_ReturnsUpdatedCustomer()
    {
        // Arrange
        var customerDto = new CustomerInfoDTO()
        {
            CustomerId = Guid.NewGuid(),
            Name = "testName",
            FatherName = "testFatherName",
            Gender = "testGender",
            Nationality = "testNationality",
            DateOfBirth = DateTime.Today,
            Address = "testAddress",
            PlaceOfBirth = "testPOB",
            PhoneNumber = "9952199189",
            EmailAddress = "Email@gmail.com",
        };
        var response = new CustomerInfo();
        _mockCustomerRepositories.Setup(r => r.UpdateCustomerDetails(customerDto)).Returns(response);

        // Act
        var result = _customerService.UpdateCustomerDetails(customerDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result, Is.EqualTo(response));
    }
    [Test]
    public void UpdateCustomerDetails_NullCustomer_ThrowsArgumentNullException()
    {
        // Arrange
        CustomerInfoDTO customerDto = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _customerService.UpdateCustomerDetails(customerDto));
    }
}