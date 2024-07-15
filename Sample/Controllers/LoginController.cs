using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sample.DTOs;
using Sample.Interfaces;
namespace Sample.Controllers;



[ApiController]
[Route("[Controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginController> _logger;
    public LoginController(IAuthService authService, ILogger<LoginController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    [HttpPost]
    public IActionResult Register([FromBody] CustomersRegisterDTO customers)
    {
        try
        {
            _logger.LogInformation("Registering Customer");
            var response = _authService.PostCustomer(customers);
            if (response)
            {
                return Ok("Customer Registered successfully");
            }
            else
            {
                return BadRequest("Customer registration failed");
            }
        }
        catch (ArgumentNullException exception)
        {
            _logger.LogError(exception , "Error Occurred while Registering the customer");
            return BadRequest(exception.Message);
        }
         catch (ValidationException exception)
        {
            _logger.LogError($"{exception.Message} \n Validation Exception Occurred");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Registering the Customer");
            return StatusCode(500 , "Internal Server Error");
        }
    }
    [HttpPost]
    public IActionResult Login([FromBody] CustomerLoginDTO customer)
    {
        try
        {
            _logger.LogInformation("Customer Logging in");
            var response = _authService.ExistingCustomer(customer);
            if (response=="Invalid username or password")
            {
               return BadRequest(response);
            }
            var token = _authService.GenerateTokens(customer);
                return Ok(new CustomerLoginResponseDTO {
                    AuthTokens = token,
                    CustomerId = response,
                    Message = "Successfully logged in"
                    });
        }
        catch(ArgumentNullException exception)
        {
            _logger.LogError(exception , "Error Occurred while Logging in");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Logging in");
            return StatusCode(500 , "Internal Server Error");
        }
        
    }
    
    [HttpPost]
    public IActionResult RefreshTokens(string username,string refreshToken)
    {
        try
        {
            _logger.LogInformation("Validating RefreshTokens");
            var newToken = _authService.ValidateRefreshToken(username, refreshToken);
            if(newToken!=null)
            {
                return Ok(newToken);
            }
            else
            {
                return BadRequest("Token Generation failed");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Logging in");
            return StatusCode(500 , "Internal Server Error");
        }
    }
}