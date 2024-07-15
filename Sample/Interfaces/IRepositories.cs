using Sample.DTOs;
using Sample.Models;

namespace Sample.Interfaces;

public interface IRepositories
{
    public bool AddCustomers(Customers customers);
    public string CheckUser(CustomerLoginDTO customer);
    public string AddRefreshToken(CustomerLoginDTO customer, byte[] randomNumber);
    public bool ValidateRefreshToken(string userName, string refreshToken);
}