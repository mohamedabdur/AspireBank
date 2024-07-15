
using FluentValidation;
using Sample.DTOs;

namespace Sample.Validators;


public class CustomerRegisterValidator : AbstractValidator<CustomersRegisterDTO>
{
    public CustomerRegisterValidator()
    {
        RuleFor(customer => customer.UserName)
        .NotEmpty().WithMessage("Username is required")
        .Length(5,50).WithMessage("Username should be between characters 5 to 50");

        RuleFor(customer => customer.Password)
        .NotEmpty().WithMessage("Password is required")
        .MinimumLength(8).WithMessage("Password should contain atleast 8 characters");

        RuleFor(customer => customer.Name)
        .NotEmpty().WithMessage("Name is required");

        RuleFor(customer => customer.PhoneNumber)
        .NotEmpty().WithMessage("PhoneNumber is required")
        .MinimumLength(10).WithMessage("Invalid Phone number")
        .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number");

        RuleFor(customer => customer.ConfirmPassword)
        .NotEmpty().WithMessage("confirm Password is required")
        .Equal(customer => customer.Password).WithMessage("Password does not match");
    }
}