using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models;

public class AuthRefreshToken
{
    [Key]
    public Guid Id { get; set;}

    [ForeignKey("Customers")]
    public Guid CustomerId { get; set;}
    public string? UserName { get; set;} 
    public string? RefreshToken{ get; set;}
    public Customers? Customers { get; set;}
    
}