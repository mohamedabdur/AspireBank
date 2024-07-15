using Microsoft.AspNetCore.Mvc;
using Sample.Interfaces;
using Sample.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Sample.Controllers;

[ApiController]
[Route("[Controller]/[action]")]
// [Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService , ILogger<CustomerController> logger)
    {   
        _logger = logger;
        _customerService = customerService;
    }

    [HttpPost]
    public IActionResult PostCustomerDetails([FromBody] CustomerInfoDTO customer)
    {
        try
        {
            _logger.LogInformation("Adding Customer Personal Details");
            var response = _customerService.AddCustomerPersonalDetails(customer);
            return Ok(response);
        }
        catch (ValidationException exception)
        {
            _logger.LogError($"{exception.Message} \n Validation Exception Occurred");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Adding Customer Details for Customer Name {CustomerName}",customer.Name);
            return StatusCode(500 , "Internal Server Error");
        }
    }

    [HttpGet]
    public IActionResult GetCustomerDetails(Guid customerID)
    {
        try
        {
            _logger.LogInformation("Fetching Customer Personal Details");
            var response = _customerService.GetCustomerDetails(customerID);
            return Ok(response);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Fetching customer Details for CustomerId {CustomerId}" , customerID);
            return StatusCode(500 , "Internal Server Error");
        }
    }

    [HttpPost]
    public IActionResult PostCustomerAccountDetails([FromForm] CustomerAccountInfoDTO customer)
    {
        try
        {
            _logger.LogInformation("Adding Customer Account Details");
            var response = _customerService.AddCustomerAccountDetails(customer);
            return Ok(response);
        }
        catch (ValidationException exception)
        {
            _logger.LogError($"{exception.Message} \n Validation Exception Occurred");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Adding Customer Account Details for customer Id {CustomerId}" , customer.CustomerId);
            return StatusCode(500 , "Internal Server Error");
        }
        
    }

    [HttpGet]
    public IActionResult GetCustomerAccountDetails(Guid customerId)
    {
        try
        {
            _logger.LogInformation("Fetching Customer Account Details");
            var response = _customerService.GetCustomerAccountDetails(customerId);
            return Ok(response);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while Fetching customer Details for CustomerId {CustomerId}" , customerId);
            return StatusCode(500 , "Internal Server Error");
        }
    }

    [HttpPut]
    public IActionResult PutCustomerDetails([FromBody] CustomerInfoDTO customer)
    {
        try
        {
            _logger.LogInformation("Updating Customer Personal Details");
            var response = _customerService.UpdateCustomerDetails(customer);
            return Ok(response);
        }
        catch (ValidationException exception)
        {
            _logger.LogError($"{exception.Message} \n Validation Exception Occurred");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while updating Customer Details for customer Id {CustomerId}" , customer.CustomerId);
            return StatusCode(500 , "Internal Server Error");
        }
    }

    [HttpPut]
    public IActionResult PutCustomerAccountDetails([FromForm] CustomerAccountUpdateInfoDTO customer)
    {
        try
        {
            _logger.LogInformation("Updating Customer Account Details");
            var response = _customerService.UpdateCustomerAccountDetails(customer);
            return Ok(response);
        }
        catch (ValidationException exception)
        {
            _logger.LogError($"{exception.Message} \n Validation Exception Occurred");
            return BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception , "Error occurred while updating Customer Account Details for customer Id {CustomerId}" , customer.CustomerId);
            return StatusCode(500 , "Internal Server Error");
        }
    }
}