using Sample.DTOs;
using Sample.Models;

namespace Sample.Interfaces;

public interface IAuthService
{
    public bool PostCustomer(CustomersRegisterDTO customer);
    public string ExistingCustomer(CustomerLoginDTO customer);
    public AuthTokens GenerateTokens(CustomerLoginDTO customer);
    public string GenerateAccessToken(string userName);
    public string GenerateRefreshToken(CustomerLoginDTO customer);
    public string ValidateRefreshToken(string userName, string refreshToken);

}