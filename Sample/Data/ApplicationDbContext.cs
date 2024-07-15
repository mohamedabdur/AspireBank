using Microsoft.EntityFrameworkCore;
namespace Sample.Data;
using Sample.Models;

public class ApplicationDbContext : DbContext
    {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}
    public DbSet<CustomerInfo> CustomerInfo {get; set;}
    public DbSet<Customers> Customers {get; set;}

    public DbSet<AuthRefreshToken> RefreshTokens {get; set;}
    public DbSet<CustomerAccountInfo> CustomerAccountInfos {get; set;}
    public DbSet<BankBranchInfo> BankBranchInfo {get; set;}
    public DbSet<BankAccountTypeInfo> BankAccountInfo {get; set;}
}


