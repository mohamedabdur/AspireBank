using Sample.DTOs;
using Sample.Models;

namespace Sample.Interfaces;

public interface ICustomerRepositories 
{
    public string AddCustomerDetails(CustomerInfo cutomer);

    public IEnumerable<CustomerInfoDTO> GetCustomerPersonalDetails(Guid customerId);
    public string PostAccountDetails(CustomerAccountInfo customer);

    public IEnumerable<CustomerAccountInfo> GetAccountDetailsByCustomerId(Guid customerId);

    public CustomerInfo UpdateCustomerDetails(CustomerInfoDTO customer);

    public CustomerAccountInfo UpdateCustomerAccountDetails(CustomerAccountUpdateInfoDTO customer);
}