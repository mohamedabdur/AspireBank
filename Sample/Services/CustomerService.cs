using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Models;
using Sample.Validators;

namespace Sample.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepositories _customerRepository;
    private readonly IMapper _mapper;


    public CustomerService(ICustomerRepositories repository, IMapper mapper)
    {
        _customerRepository = repository;
        _mapper = mapper;
    }

    public string AddCustomerPersonalDetails(CustomerInfoDTO customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException("Customer cannot be null");
        }

        var validator = new CustomerInfoValidator();
        var validationResult = validator.Validate(customer);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToArray();
            throw new ValidationException(errors[0]);
        }
        var customers = _mapper.Map<CustomerInfo>(customer);
        var response = _customerRepository.AddCustomerDetails(customers);
        if(response == null)
        {
            throw new ArgumentNullException(response);
        }
        return response;
    }
    public IEnumerable<CustomerInfoDTO> GetCustomerDetails(Guid customerId)
    {
        var response = _customerRepository.GetCustomerPersonalDetails(customerId);
        if (response == null)
        {
            throw new Exception();
        }
        return response;
    }

    public string AddCustomerAccountDetails(CustomerAccountInfoDTO customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException();
        }
        
        var validate = new CustomerAccountValidator();
        var validationResult = validate.Validate(customer);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(errors => errors.ErrorMessage).ToArray();
            throw new ValidationException(errors[0]);
            
        }

        using (var memoryStream = new MemoryStream())
        {
            customer.IdProof?.CopyTo(memoryStream);
            var customerAccounts = new CustomerAccountInfo
            {
                CustomerId = customer.CustomerId,
                GovernmentId = customer.GovernmentId,
                IdType = customer.IdType,
                IdProof = memoryStream.ToArray(),
                AccountType = customer.AccountType,
                BranchName = customer.BranchName,
                AgreedToPrivacyPolicy = customer.AgreedToPrivacyPolicy,
                AgreedToTermsAndConditions = customer.AgreedToTermsAndConditions,
                EmployementStatus = customer.EmployementStatus,
                OrganisationName = customer.OrganisationName,
                Occupation = customer.Occupation,
                AnnualIncome = customer.AnnualIncome
            };

            
            if (customerAccounts.AgreedToPrivacyPolicy && customerAccounts.AgreedToTermsAndConditions)
            {
                var response = _customerRepository.PostAccountDetails(customerAccounts);
                return "Customer Account Details Added Successfully";
            }
            return "Please accept the Terms and Conditions and privacy policy";
        }
    }

    public IEnumerable<CustomerAccountInfo> GetCustomerAccountDetails(Guid customerId)
    {
        var accountDetails = _customerRepository.GetAccountDetailsByCustomerId(customerId);
        if(accountDetails == null)
        {
            throw new Exception();
        }
        return accountDetails;
    }

    public CustomerInfo UpdateCustomerDetails(CustomerInfoDTO customer)
    {
        var validator = new CustomerInfoValidator();
        var validationResult = validator.Validate(customer);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToArray();
            throw new ValidationException(errors[0]);
        }
        if(customer == null)
        {
            throw new ArgumentNullException();
        }
        var customerDetails = _customerRepository.UpdateCustomerDetails(customer);
        return customerDetails;
    }

    public CustomerAccountInfo UpdateCustomerAccountDetails(CustomerAccountUpdateInfoDTO customer)
    {
        var validate = new CustomerAccountUpdateValidator();
        var validationResult = validate.Validate(customer);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(errors => errors.ErrorMessage).ToArray();
            throw new ValidationException(errors[0]);
        }
        if(customer == null)
        {
            throw new ArgumentNullException();
        }
        var customerAccountDetails = _customerRepository.UpdateCustomerAccountDetails(customer);
        return customerAccountDetails;
    }
}