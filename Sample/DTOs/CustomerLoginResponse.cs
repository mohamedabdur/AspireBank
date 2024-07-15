using Sample.Models;

namespace Sample.DTOs;

public class CustomerLoginResponseDTO
{
    public AuthTokens AuthTokens { get; set; }
    public string CustomerId { get; set; }
    public string Message { get; set; }
}