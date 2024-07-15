using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sample.Data;
using Sample.DTOs;
using Sample.Models;
using Sample.Repositories;

namespace SampleTest.RepositoriesTests;

[TestFixture]
public class CustomerRepositoriesTests
{
    private ApplicationDbContext _dbContext;
    private Mock<IMapper> _mapperMock;
    private CustomerRepositories _customerRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDatabase")
           .Options;
        _dbContext = new ApplicationDbContext(options);

        _mapperMock = new Mock<IMapper>();
        _customerRepository = new CustomerRepositories(_dbContext, _mapperMock.Object);
    }

    // Clean up the resourse after each Test.
    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted(); // Deletes in-memory DB after each Test.
        _dbContext.Dispose(); // Disposes ApplicationDbContext instance.
    }

    [Test]
    public void AddCustomerDetails_CustomerDoesNotExist_AddsCustomerDetails()
    {
        // Arrange
        var customer = new CustomerInfo
        {
            CustomerId = Guid.NewGuid(),
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };

        // Act
        var result = _customerRepository.AddCustomerDetails(customer);

        // Assert
        Assert.That(result, Is.EqualTo("Customer Details added"));
        Assert.That(_dbContext.CustomerInfo.Count(), Is.EqualTo(1));
    }

    [Test]
    public void AddCustomerDetails_CustomerExist_ReturnsExistingMessage()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingCustomer = new CustomerInfo()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var newCustomer = new CustomerInfo()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };
        var response = "Customer Details Already Exists";

        // Act
        var result = _customerRepository.AddCustomerDetails(newCustomer);

        // Assert
        Assert.That(result, Is.EqualTo(response));
    }


    [Test]
    public void GetCustomerPersonalDetails_CustomerExists_ReturnsDetails()
    {
        var customerId = Guid.NewGuid();
        var existingCustomer = new CustomerInfo()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        _mapperMock.Setup(m => m.Map<IEnumerable<CustomerInfoDTO>>(It.IsAny<IEnumerable<CustomerInfo>>()))
                .Returns(new List<CustomerInfoDTO>
                {
                    new CustomerInfoDTO
                    {
                        CustomerId = customerId,
                        Name = "testName",
                        Address = "testAddress",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        EmailAddress = "test@example.com",
                        FatherName = "testFathername",
                        Gender = "Male",
                        Nationality = "Country",
                        PhoneNumber = "1234567890",
                        PlaceOfBirth = "City",
                    }
                });

        // Act
        var result = _customerRepository.GetCustomerPersonalDetails(customerId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("testName"));
    }

    [Test]
    public void GetCustomerPersonalDetails_CustomerNotFound_ThrowsException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act & Assert
        var result = Assert.Throws<Exception>(() => _customerRepository.GetCustomerPersonalDetails(customerId));
        Assert.That(result.Message, Is.EqualTo("Customer Not found"));
    }

    [Test]
    public void PostCustomerAccountDetails_ValidDetails_AddsAccountDetails()
    {
        // Arrange
        var bankBranch = new BankBranchInfo
        {
            BankBranch = "MainBranch",
            IFSCcode = "IFSC123",
            BranchCode = "123"
        };
        _dbContext.BankBranchInfo.Add(bankBranch);
        _dbContext.SaveChanges();

        var accountType = new BankAccountTypeInfo
        {
            BankAccountType = "Savings",
            AccountTypeCode = "456"
        };
        _dbContext.BankAccountInfo.Add(accountType);
        _dbContext.SaveChanges();

        using (var memoryStream = new MemoryStream())
        {
            var customerAccount = new CustomerAccountInfo
            {
                CustomerId = Guid.NewGuid(),
                GovernmentId = "testId",
                IdType = "testtype",
                IdProof = memoryStream.ToArray(),
                AccountType = "testtype",
                BranchName = "MainBranch",
                AgreedToPrivacyPolicy = true,
                AgreedToTermsAndConditions = true,
                EmployementStatus = "teststatus",
                OrganisationName = "testname",
                Occupation = "testoccupation",
                AnnualIncome = "testincome"
            };

            // Act
            var result = _customerRepository.PostAccountDetails(customerAccount);

            // Assert
            Assert.That(result, Is.EqualTo("Customer Account Details Added Successfully"));
            Assert.That(_dbContext.CustomerAccountInfos.Count(), Is.EqualTo(1));
            Assert.That(_dbContext.CustomerAccountInfos.First().IFSCCode, Is.EqualTo("IFSC123"));
        }
    }

    [Test]
    public void GetCustomerAccountDetailsByCustomerId_CustomerExists_ReturnsDetails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        using (var memoryStream = new MemoryStream())
        {
            var customerAccountInfo = new CustomerAccountInfo
            {
                CustomerId = customerId,
                GovernmentId = "testId",
                IdType = "testtype",
                IdProof = memoryStream.ToArray(),
                AccountType = "Savings",
                BranchName = "MainBranch",
                AgreedToPrivacyPolicy = true,
                AgreedToTermsAndConditions = true,
                EmployementStatus = "teststatus",
                OrganisationName = "testname",
                Occupation = "testoccupation",
                AnnualIncome = "testincome",
                AccountNumber = "1234567890",
                IFSCCode = "IFSC123"

            };
            _dbContext.CustomerAccountInfos.Add(customerAccountInfo);
            _dbContext.SaveChanges();

            // Act
            var result = _customerRepository.GetAccountDetailsByCustomerId(customerId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().AccountNumber, Is.EqualTo("1234567890"));
        }
    }

    [Test]
    public void GetCustomerAccountDetailsByCustomerId_CustomerNotFound_ThrowsException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act & Assert
        var result = Assert.Throws<Exception>(() => _customerRepository.GetAccountDetailsByCustomerId(customerId));
        Assert.That(result.Message, Is.EqualTo("Customer Not found"));
    }

    [Test]
    public void UpdateCustomerDetails_CustomerExist_UpdatesDetails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingCustomer = new CustomerInfo()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var newCustomer = new CustomerInfoDTO()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };

        // Act
        var result = _customerRepository.UpdateCustomerDetails(newCustomer);

        // Assert
        Assert.That(result.Name, Is.EqualTo("testName"));
        Assert.That(result.Address, Is.EqualTo("testAddress"));
    }

    [Test]
    public void UpdateCustomerDetails_CustomerNotFound_ThrowsException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingCustomer = new CustomerInfo()
        {
            CustomerId = customerId,
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test@example.com",
            FatherName = "testFathername",
            Gender = "Male",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };
        _dbContext.Add(existingCustomer);
        _dbContext.SaveChanges();

        var response = "Customer Not Found";
        var newCustomer = new CustomerInfoDTO()
        {
            CustomerId = Guid.NewGuid(),
            Name = "testName",
            Address = "testAddress",
            DateOfBirth = new DateTime(1980, 1, 1),
            EmailAddress = "test2@example.com",
            FatherName = "testFathername2",
            Gender = "FeMale",
            Nationality = "Country",
            PhoneNumber = "1234567890",
            PlaceOfBirth = "City"
        };

        // Act
        var result = Assert.Throws<Exception>(() => _customerRepository.UpdateCustomerDetails(newCustomer));
        Assert.That(result.Message, Is.EqualTo(response));
    }

    [Test]
    public void UpdateCustomerAccountDetails_CustomerExists_UpdatesDetails()
    {
        var customerId = Guid.NewGuid();
        using (var memoryStream = new MemoryStream())
        {
            var customerAccountInfo = new CustomerAccountInfo
            {
                CustomerId = customerId,
                GovernmentId = "testId",
                IdType = "testtype",
                IdProof = memoryStream.ToArray(),
                AccountType = "Savings",
                BranchName = "MainBranch",
                AgreedToPrivacyPolicy = true,
                AgreedToTermsAndConditions = true,
                EmployementStatus = "teststatus",
                OrganisationName = "testname",
                Occupation = "testoccupation",
                AnnualIncome = "testincome",
                AccountNumber = "1234567890",
                IFSCCode = "IFSC123"

            };

            _dbContext.CustomerAccountInfos.Add(customerAccountInfo);
            _dbContext.SaveChanges();
        }

        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        using (var memoryStream = new MemoryStream())
        {
            var updatedCustomer = new CustomerAccountUpdateInfoDTO
            {
                CustomerId = customerId,
                GovernmentId = "newId",
                IdType = "newtype",
                IdProof = file,
                AccountType = "Current",
                EmployementStatus = "NewStatus",
                OrganisationName = "NewOrg",
                Occupation = "NewOccupation",
                AnnualIncome = "100000",

            };
        // Act
        var result = _customerRepository.UpdateCustomerAccountDetails(updatedCustomer);

        // Assert
        Assert.That(result.GovernmentId, Is.EqualTo("newId"));
        Assert.That(result.IdType, Is.EqualTo("newtype"));
        Assert.That(result.AccountType, Is.EqualTo("Current"));
        Assert.That(result.EmployementStatus, Is.EqualTo("NewStatus"));
        Assert.That(result.OrganisationName, Is.EqualTo("NewOrg"));
        Assert.That(result.Occupation, Is.EqualTo("NewOccupation"));
        Assert.That(result.AnnualIncome, Is.EqualTo("100000"));
        }
    }

    [Test]
    public void UpdateCustomerAccountDetails_CustomerNotFound_ThrowsException()
    {
        // Arrange
        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        using (var memoryStream = new MemoryStream())
        {
            var updatedCustomer = new CustomerAccountUpdateInfoDTO
            {
                CustomerId = Guid.NewGuid(),
                GovernmentId = "newId",
                IdType = "newtype",
                IdProof = file,
                AccountType = "Current",
                EmployementStatus = "NewStatus",
                OrganisationName = "NewOrg",
                Occupation = "NewOccupation",
                AnnualIncome = "100000",

            };
            var response = "Customer not Found";
        // Act & Assert
        var result = Assert.Throws<Exception>(() => _customerRepository.UpdateCustomerAccountDetails(updatedCustomer));
        Assert.That(result.Message, Is.EqualTo(response));
        }
    }
}