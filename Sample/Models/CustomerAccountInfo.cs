using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models;

public class CustomerAccountInfo
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("Customers")]
    public Guid CustomerId { get; set;}
    public Customers? Customers { get; set;}
    public string? GovernmentId { get; set;}
    public string? IdType { get; set;}
    public byte[]? IdProof { get; set;}
    public string? AccountType { get; set;}
    public string? BranchName { get; set;}
    public string? IFSCCode { get; set; }
    public string? AccountNumber { get; set; }
    public bool AgreedToTermsAndConditions { get; set; }
    public bool AgreedToPrivacyPolicy { get; set; }
    public string? EmployementStatus { get; set;}
    public string? OrganisationName { get; set; }
    public string? Occupation { get; set; }
    public string? AnnualIncome { get; set; }
}