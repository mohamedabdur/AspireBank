using AutoMapper;
using Sample.Data;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;

namespace Sample.Repositories;

public class CustomerRepositories : ICustomerRepositories
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public CustomerRepositories(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public string AddCustomerDetails(CustomerInfo customer)
    {
        var response = _dbContext.CustomerInfo.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
        if (response == null)
        {
            _dbContext.CustomerInfo.Add(customer);
            _dbContext.SaveChanges();
            return "Customer Details added";
        }
        return "Customer Details Already Exists";
    }

    public IEnumerable<CustomerInfoDTO> GetCustomerPersonalDetails(Guid customerId)
    {
        var customerDetails = _dbContext.CustomerInfo.Where(x => x.CustomerId == customerId).ToList();
        if (!customerDetails.Any())
        {
            throw new Exception("Customer Not found");
        }
        return _mapper.Map<IEnumerable<CustomerInfoDTO>>(customerDetails);
    }

    public string PostAccountDetails(CustomerAccountInfo customer)
    {
        customer.AccountNumber = GenerateAccountNumber(customer);
        var bankBranchDetails = _dbContext.BankBranchInfo.Where(x => x.BankBranch == customer.BranchName).FirstOrDefault();
        customer.IFSCCode = bankBranchDetails?.IFSCcode;
        _dbContext.CustomerAccountInfos.Add(customer);
        _dbContext.SaveChanges();
        return "Customer Account Details Added Successfully";
    }

    public string GenerateAccountNumber(CustomerAccountInfo customer)
    {
        var accountNumberByBankCode = 24;
        var accountNumberByBranchCode = _dbContext.BankBranchInfo.Where(x => x.BankBranch == customer.BranchName).FirstOrDefault();
        var accountNumberByAccountType = _dbContext.BankAccountInfo.Where(x => x.BankAccountType == customer.AccountType).FirstOrDefault();
        var lastFiveDigit = GenerateRandomDigit();
        var finalAccountnumber = accountNumberByBankCode + accountNumberByBranchCode?.BranchCode + accountNumberByAccountType?.AccountTypeCode + lastFiveDigit;
        return finalAccountnumber;
    }
    public string GenerateRandomDigit()
    {
        Random random = new Random();
        var randomDigit = random.Next(10000, 100000);
        return randomDigit.ToString();
    }

    public IEnumerable<CustomerAccountInfo> GetAccountDetailsByCustomerId(Guid customerId)
    {
        var customerAccountDetails = _dbContext.CustomerAccountInfos.Where(x => x.CustomerId == customerId).ToList();
        if (!customerAccountDetails.Any())
        {
            throw new Exception("Customer Not found");
        }
        return customerAccountDetails;
    }

    public CustomerInfo UpdateCustomerDetails(CustomerInfoDTO customer)
    {
        var customerDetails = _dbContext.CustomerInfo.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
        if (customerDetails == null)
        {
            throw new Exception("Customer Not Found");
        }
        customerDetails.Name = customer.Name;
        customerDetails.Address = customer.Address;
        customerDetails.DateOfBirth = customer.DateOfBirth;
        customerDetails.EmailAddress = customer.EmailAddress;
        customerDetails.FatherName = customer.FatherName;
        customerDetails.Gender = customer.Gender;
        customerDetails.Nationality = customer.Nationality;
        customerDetails.PhoneNumber = customer.PhoneNumber;
        customerDetails.PlaceOfBirth = customer.PlaceOfBirth;

        _dbContext.SaveChanges();

        return customerDetails;
    }

    public CustomerAccountInfo UpdateCustomerAccountDetails(CustomerAccountUpdateInfoDTO customer)
    {
        var customerAccountDetails = _dbContext.CustomerAccountInfos.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
        if (customerAccountDetails == null)
        {
            throw new Exception("Customer not Found");
        }
        using (var memoryStream = new MemoryStream())
        {
            customer.IdProof?.CopyTo(memoryStream);
            customerAccountDetails.CustomerId = customer.CustomerId; ;
            customerAccountDetails.GovernmentId = customer.GovernmentId;
            customerAccountDetails.IdType = customer.IdType;
            customerAccountDetails.IdProof = memoryStream.ToArray();
            customerAccountDetails.AccountType = customer.AccountType;
            customerAccountDetails.EmployementStatus = customer.EmployementStatus;
            customerAccountDetails.OrganisationName = customer.OrganisationName;
            customerAccountDetails.Occupation = customer.Occupation;
            customerAccountDetails.AnnualIncome = customer.AnnualIncome;

            _dbContext.SaveChanges();
            return customerAccountDetails;
        }
    }
}