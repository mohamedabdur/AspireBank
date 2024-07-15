using Sample.Models;

namespace Sample.DTOs;

public class CustomerAccountInfoDTO
{
    public Guid CustomerId { get; set;}
    public string? GovernmentId { get; set;}
    public string? IdType { get; set;}
    public IFormFile? IdProof { get; set;}
    public string? AccountType { get; set;}
    public string? BranchName { get; set;}
    public bool AgreedToTermsAndConditions { get; set; }
    public bool AgreedToPrivacyPolicy { get; set; }
    public string? EmployementStatus { get; set;}
    public string? OrganisationName { get; set; }
    public string? Occupation { get; set; }
    public string? AnnualIncome { get; set; }
}