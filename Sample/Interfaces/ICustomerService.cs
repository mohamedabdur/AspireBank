using Sample.DTOs;
using Sample.Models;

namespace Sample.Interfaces;

public interface ICustomerService
{
    public string AddCustomerPersonalDetails(CustomerInfoDTO customerInfoDTO);

    public IEnumerable<CustomerInfoDTO> GetCustomerDetails(Guid customerId);
    public string AddCustomerAccountDetails(CustomerAccountInfoDTO customer);
    public IEnumerable<CustomerAccountInfo> GetCustomerAccountDetails(Guid customerId);

    public CustomerInfo UpdateCustomerDetails(CustomerInfoDTO customer);

    public CustomerAccountInfo UpdateCustomerAccountDetails(CustomerAccountUpdateInfoDTO customer);
    
}