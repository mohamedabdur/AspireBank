using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models;

public class CustomerInfo{
    [Key]
    public Guid Id { get; set;}
    
    [ForeignKey("Customers")]
    public Guid CustomerId { get; set;}
    public string? Name { get; set;}
    public string? FatherName{ get; set;}
    public string? Gender { get; set;}
    public DateTime DateOfBirth { get; set;}
    public string? Nationality{ get; set;}
    public string? Address { get; set;}
    public string? PlaceOfBirth { get; set;}
    public string? PhoneNumber { get; set;}
    public string? EmailAddress { get; set;}
    public Customers? Customers {get; set;}
}