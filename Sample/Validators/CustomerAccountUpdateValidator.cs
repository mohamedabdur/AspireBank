using FluentValidation;
using Sample.DTOs;

namespace Sample.Validators;

public class CustomerAccountUpdateValidator : AbstractValidator<CustomerAccountUpdateInfoDTO>
{
    public CustomerAccountUpdateValidator()
    {
        RuleFor(x => x.GovernmentId)
        .NotEmpty().WithMessage("Government ID is required")
        .Matches("^[a-zA-Z0-9]*$");

        RuleFor(x => x.IdType)
        .NotEmpty().WithMessage("Id type is required");

        RuleFor(x => x.IdProof)
        .NotEmpty().WithMessage("Id proof is required");

        RuleFor(x => x.AccountType)
        .NotEmpty().WithMessage("Account type is required");

        RuleFor(x => x.EmployementStatus)
        .NotEmpty().WithMessage("EmployementStatus is required");

        RuleFor(x => x.OrganisationName)
        .NotEmpty().WithMessage("OrganisationName is required");

        RuleFor(x => x.Occupation)
        .NotEmpty().WithMessage("Occupation is required");

        RuleFor(x => x.AnnualIncome)
        .NotEmpty().WithMessage("AnnualIncome is required");
    }
}