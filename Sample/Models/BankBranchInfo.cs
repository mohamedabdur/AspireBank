using System.ComponentModel.DataAnnotations;

namespace Sample.Models;

public class BankBranchInfo
{
    [Key]
    public Guid Id { get; set; }
    public string? BankBranch { get; set; }
    public string? BranchCode { get; set; }
    public string? IFSCcode { get; set; }
}