using System.ComponentModel.DataAnnotations;

namespace Sample.Models;

public class BankAccountTypeInfo
{
    [Key]
    public Guid Id { get; set; }
    public string? BankAccountType { get; set; }
    public string? AccountTypeCode { get; set; }
    
}