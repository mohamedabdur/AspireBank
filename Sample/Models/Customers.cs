using System.ComponentModel.DataAnnotations;

namespace Sample.Models;

public class Customers
{
    [Key]
    public Guid CustomerId { get; set;}
    public string? UserName { get; set;} 
    public string? Password { get; set;} 
    public string? Name { get; set;} 
    public string? PhoneNumber { get; set;}
}