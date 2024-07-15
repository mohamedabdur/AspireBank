using System.IdentityModel.Tokens.Jwt;
using Sample.DTOs;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Sample.Interfaces;
using Sample.Validators;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using System.Security.Cryptography;
using Sample.Models;

namespace Sample.Services;

public class AuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly IMapper _mapper;
    private readonly IRepositories _repositories;
    public AuthService(IConfiguration configuration, IRepositories repositories, IMapper mapper)
    {

        _secretKey = configuration["Appsettings:Token"];
        _repositories = repositories;
        _mapper = mapper;
    }


    public bool PostCustomer(CustomersRegisterDTO customer)
    {
        if(customer == null)
        {
            throw new ArgumentNullException("Customer cannot be null");
        }
        var validator = new CustomerRegisterValidator();
        var validationResult = validator.Validate(customer);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToArray();
            throw new ValidationException(errors[0]);
        }
        customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
        var customers = _mapper.Map<Customers>(customer);
        var result = _repositories.AddCustomers(customers);
        if(result)
        {
            return true;
        }
        return false;
    }

    public string ExistingCustomer(CustomerLoginDTO customer)
    {
        var result = _repositories.CheckUser(customer);
        return result;
    }

    public AuthTokens GenerateTokens(CustomerLoginDTO customer)
    {
        var userName = customer.UserName;
        var accessToken = GenerateAccessToken(userName);
        var refreshToken = GenerateRefreshToken(customer);

        return new AuthTokens
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    public string GenerateAccessToken(string userName)
    {
        var tokenHanlder = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new [] 
            {
                new Claim("username", userName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
        };  
        var token = tokenHanlder.CreateToken(tokenDescription);
        return tokenHanlder.WriteToken(token);
    }

     public string GenerateRefreshToken(CustomerLoginDTO customer)
    {
        var randomNumber = new Byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);      
        }
       var token = _repositories.AddRefreshToken(customer,randomNumber);
       if(token != null)
       {
            return token;
       }
       return "Token is null";
    }

    public string ValidateRefreshToken(string userName, string refreshToken)
    {
        var result = _repositories.ValidateRefreshToken(userName,refreshToken);
        if(result)
        {
            var newToken = GenerateAccessToken(userName);
            if(newToken != null)
            {
                return newToken;
            }
            else
            {
                return "Token Generation Failed";
            }
        }
        return "Not a Valid Credentials";
    }
}
