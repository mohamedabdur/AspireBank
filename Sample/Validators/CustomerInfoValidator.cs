using FluentValidation;
using Sample.DTOs;

namespace Sample.Validators;

public class CustomerInfoValidator : AbstractValidator<CustomerInfoDTO>
{
    public CustomerInfoValidator()
    {

        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is Required");

        RuleFor(x => x.FatherName)
        .NotEmpty().WithMessage("Father Name is Required");

        RuleFor(x => x.Gender)
        .NotEmpty().WithMessage("Gender is Required");

        RuleFor(x => x.Nationality)
        .NotEmpty().WithMessage("Nationality is Required");

        RuleFor(x => x.DateOfBirth)
        .NotEmpty().WithMessage("Date of Birth is required")
        .Must(ValidDate).WithMessage("Date should be a Valid ")
        .Must(NotBeInTheFuture).WithMessage("Date of Birth cannot be in the future");

        RuleFor(x => x.Address)
        .NotEmpty().WithMessage("Address is Required");

         RuleFor(x => x.PhoneNumber)
        .NotEmpty().WithMessage("PhoneNumber is required")
        .MinimumLength(10).WithMessage("Invalid Phone number")
        .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number");

        RuleFor(x => x.EmailAddress)
        .NotEmpty().WithMessage("Email is Required")
        .EmailAddress().WithMessage("Must be a valid Email");
        
        RuleFor(x => x.PlaceOfBirth)
        .NotEmpty().WithMessage("Place of Birth is Required");
    }


    private bool ValidDate(DateTime date)
    {
        return !date.Equals(default(DateTime));
    }
     private bool NotBeInTheFuture(DateTime date)
    {
        return date <= DateTime.Today;
    }

}