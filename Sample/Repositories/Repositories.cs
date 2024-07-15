using AutoMapper;
using Sample.DTOs;
using Sample.Interfaces;
using Sample.Data;
using Sample.Models;

namespace Sample.Repositories;

public class Repositories : IRepositories
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public Repositories(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public bool AddCustomers(Customers customer)
    {
        var existingCustomer = _dbContext.Customers.Where(existingcustomer => existingcustomer.UserName == customer.UserName).FirstOrDefault();
        if (existingCustomer != null)
        {
            throw new Exception("username already exists");
        }
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();
        return true;
    }

    public string CheckUser(CustomerLoginDTO customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException("Value cannot be null");
        }
        var result = _dbContext.Customers.Where(customerobj => customerobj.UserName == customer.UserName).FirstOrDefault();
        if (result != null && BCrypt.Net.BCrypt.Verify(customer.Password, result.Password))
        { 
             return result.CustomerId.ToString();  
        }
        return "Invalid username or password";
    }

    public string AddRefreshToken(CustomerLoginDTO customer, byte[] randomNumber)
    { 
        var customerDetails= _dbContext.Customers.FirstOrDefault(customerobj => customerobj.UserName == customer.UserName);
        if(customerDetails == null)
        {
            throw new Exception("Customer not found");
        }

        var isRefreshtokenExist = _dbContext.RefreshTokens.FirstOrDefault(refreshToken => refreshToken.UserName == customer.UserName);
        if(isRefreshtokenExist != null)
        {
            _dbContext.RefreshTokens.Remove(isRefreshtokenExist);
            _dbContext.SaveChanges();
        }

        var refreshToken = new AuthRefreshToken
        {
            CustomerId = customerDetails.CustomerId,
            UserName = customerDetails.UserName,
            RefreshToken = Convert.ToBase64String(randomNumber),           
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();
        
        return refreshToken.RefreshToken;
    }

    public bool ValidateRefreshToken(string userName, string refreshToken)
    {
        var response = _dbContext.RefreshTokens.Where(customer => customer.UserName == userName && customer.RefreshToken == refreshToken).FirstOrDefault();
        if(response != null)
        {
            return true;
        }
        return false;
    }
}
